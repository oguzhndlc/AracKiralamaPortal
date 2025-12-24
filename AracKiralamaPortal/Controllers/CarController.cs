using AracKiralamaPortal.Models;
using AracKiralamaPortal.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AracKiralamaPortal.Controllers
{
    [Authorize(Roles = "Admin,Employee")]
    public class CarController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };

        public CarController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var cars = _unitOfWork.Cars.GetAll(q => q
            .Include(c => c.Brand)
            .Include(c => c.VehicleType)
            .Include(c => c.VehicleSubType));
            return View(cars);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var defaultTypeId = _unitOfWork.VehicleTypes.GetAll().FirstOrDefault()?.Id ?? 0;
            var car = new Car { VehicleTypeId = defaultTypeId };

            PopulateDropdowns(car);
            return View(car);
        }

        [HttpPost]
        public IActionResult Create(Car car, IFormFile imageFile)
        {

            if (imageFile != null)
            {
                var ext = Path.GetExtension(imageFile.FileName).ToLower();
                if (!_allowedExtensions.Contains(ext))
                {
                    ModelState.AddModelError("", "Sadece resim dosyaları yüklenebilir (jpg, png, webp).");
                }
            }

            if (!ModelState.IsValid)
            {
                TempData["ToastErrors"] = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => string.IsNullOrWhiteSpace(e.ErrorMessage) ? "Alan geçersiz" : e.ErrorMessage)
                    .ToList();
                Console.WriteLine("");
                PopulateDropdowns(car);
                return View(car);
            }

            if (imageFile != null && imageFile.Length > 0)
                car.ImagePath = UploadImage(imageFile);
            _unitOfWork.Cars.Add(car);
            _unitOfWork.Save();
            TempData["ToastSuccess"] = "Araç başarıyla eklendi.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var car = _unitOfWork.Cars.Get(id);
            if (car == null) return NotFound();

            PopulateDropdowns(car);
            return View(car);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Car car, IFormFile? imageFile)
        {
            var existingCar = _unitOfWork.Cars.Get(car.Id);
            if (existingCar == null) return NotFound();

            if (imageFile != null && imageFile.Length > 0)
            {
                var ext = Path.GetExtension(imageFile.FileName).ToLower();
                if (!_allowedExtensions.Contains(ext))
                    ModelState.AddModelError("", "Sadece jpg, png veya webp dosyaları yüklenebilir.");
            }

            if (string.IsNullOrWhiteSpace(car.Color))
                ModelState.AddModelError("Color", "Lütfen bir renk seçin.");

            if (!ModelState.IsValid)
            {
                TempData["ToastErrors"] = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                PopulateDropdowns(existingCar);
                return View(existingCar);
            }

            if (imageFile != null && imageFile.Length > 0)
                existingCar.ImagePath = UploadImage(imageFile, existingCar.ImagePath);

            existingCar.Model = car.Model;
            existingCar.Year = car.Year;
            existingCar.Price = car.Price;
            existingCar.VehicleTypeId = car.VehicleTypeId;
            existingCar.VehicleSubTypeId = car.VehicleSubTypeId;
            existingCar.BrandId = car.BrandId;
            existingCar.isAvailable = car.isAvailable;
            existingCar.FuelType = car.FuelType;
            existingCar.GearType = car.GearType;
            existingCar.Mileage = car.Mileage;
            existingCar.EngineCapacity = car.EngineCapacity;
            existingCar.EnginePower = car.EnginePower;
            existingCar.HasAC = car.HasAC;
            existingCar.HasGPS = car.HasGPS;
            existingCar.Color = car.Color;

            _unitOfWork.Cars.Update(existingCar);
            _unitOfWork.Save();

            TempData["ToastSuccess"] = "Araç başarıyla güncellendi.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var car = _unitOfWork.Cars.Get(id);
            if (car == null) return NotFound();

            if (!string.IsNullOrEmpty(car.ImagePath))
                DeleteImage(car.ImagePath);

            _unitOfWork.Cars.Remove(car);
            _unitOfWork.Save();
            return Ok();
        }

        [HttpPost]
        public IActionResult UpdateAvailability(int id)
        {
            var car = _unitOfWork.Cars.Get(id);
            if (car == null) return NotFound();

            car.isAvailable = !car.isAvailable;
            _unitOfWork.Cars.Update(car);
            _unitOfWork.Save();

            return Json(new { success = true, isAvailable = car.isAvailable });
        }

        private string UploadImage(IFormFile imageFile, string? oldImagePath = null)
        {
            string uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/cars");
            if (!Directory.Exists(uploadDir))
                Directory.CreateDirectory(uploadDir);

            string fileName = Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
            string filePath = Path.Combine(uploadDir, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            imageFile.CopyTo(stream);

            if (!string.IsNullOrEmpty(oldImagePath))
                DeleteImage(oldImagePath);

            return "/images/cars/" + fileName;
        }

        private void DeleteImage(string imagePath)
        {
            string fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", imagePath.TrimStart('/'));
            if (System.IO.File.Exists(fullPath))
                System.IO.File.Delete(fullPath);
        }

        [HttpGet]
        public IActionResult GetSubTypes(int vehicleTypeId)
        {
            var subTypes = _unitOfWork.VehicleSubTypes
                .Find(v => v.VehicleTypeId == vehicleTypeId)
                .Select(v => new { v.Id, v.Name })
                .ToList();

            return Json(subTypes);
        }

        private void PopulateDropdowns(Car? car = null)
        {
            var vehicleTypes = _unitOfWork.VehicleTypes.GetAll();
            int selectedTypeId = car?.VehicleTypeId ?? vehicleTypes.FirstOrDefault()?.Id ?? 1;

            ViewBag.VehicleTypes = new SelectList(vehicleTypes, "Id", "Name", selectedTypeId);

            var subTypes = _unitOfWork.VehicleSubTypes.Find(v => v.VehicleTypeId == selectedTypeId);
            ViewBag.VehicleSubTypes = new SelectList(subTypes, "Id", "Name", car?.VehicleSubTypeId ?? subTypes.FirstOrDefault()?.Id);

            ViewBag.Brands = new SelectList(_unitOfWork.Brands.GetAll(), "Id", "Name", car?.BrandId);

            var fuelTypes = new List<string> { "Benzin", "Dizel", "Elektrik", "Hibrit", "LPG" };
            ViewBag.FuelTypes = new SelectList(fuelTypes, car?.FuelType);

            var gearTypes = new List<string> { "Manuel", "Otomatik", "Yarı-Otomatik" };
            ViewBag.GearTypes = new SelectList(gearTypes, car?.GearType);

            ViewBag.Availability = new List<SelectListItem>
            {
                new SelectListItem { Value = "true", Text = "🟢 Müsait", Selected = car?.isAvailable == true },
                new SelectListItem { Value = "false", Text = "🔴 Müsait Değil", Selected = car?.isAvailable == false }
            };
        }
    }
}
