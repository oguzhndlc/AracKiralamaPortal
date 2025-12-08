using AracKiralamaPortal.Models;
using AracKiralamaPortal.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AracKiralamaPortal.Controllers
{
    // Bu controller'a sadece AdminCookie ile giriş yapan kullanıcılar erişebilir
    [Authorize(Roles = "Admin")]
    public class CarController : Controller
    {
        // Veritabanı işlemleri için UnitOfWork injection
        private readonly IUnitOfWork _unitOfWork;

        public CarController(IUnitOfWork unitOfWork)
        {
            // Constructor üzerinden UnitOfWork'i alıyoruz
            _unitOfWork = unitOfWork;
        }

        // Araç listesi sayfası
        public IActionResult Index()
        {
            // Tüm araçları markalarıyla birlikte (Include) getiriyoruz
            var cars = _unitOfWork.Cars.GetAll(q => q.Include(c => c.Brand));
            return View(cars);
        }

        // Araç ekleme sayfası (GET)
        [HttpGet]
        public IActionResult Create()
        {
            // Marka dropdown'ı için tüm markaları ViewBag ile gönderiyoruz
            ViewBag.Brands = _unitOfWork.Brands.GetAll().ToList();
            return View();
        }

        // Araç ekleme işlemi (POST)
        [HttpPost]
        public IActionResult Create(Car car, IFormFile imageFile)
        {
            // Model is valid mi (zorunlu alanlar dolu mu)
            if (ModelState.IsValid)
            {
                // Eğer bir resim yüklenmişse
                if (imageFile != null && imageFile.Length > 0)
                {
                    // Resimlerin kaydedileceği klasör
                    string uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/cars");

                    // Klasör yoksa oluştur
                    if (!Directory.Exists(uploadDir))
                        Directory.CreateDirectory(uploadDir);

                    // Benzersiz dosya adı oluştur
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);

                    // Tam kayıt yolunu oluştur
                    string filePath = Path.Combine(uploadDir, fileName);

                    // Dosyayı fiziksel olarak sunucuya kaydet
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        imageFile.CopyTo(stream);
                    }

                    // Araba nesnesine resim yolunu kaydet
                    car.ImagePath = "/images/cars/" + fileName;
                }

                // Aracı veritabanına ekle
                _unitOfWork.Cars.Add(car);
                _unitOfWork.Save();

                return RedirectToAction("Index");
            }

            // ModelState hata varsa markaları tekrar gönderiyoruz
            ViewBag.Brands = _unitOfWork.Brands.GetAll().ToList();
            return View(car);
        }

        // Araç düzenleme sayfası (GET)
        [HttpGet]
        public IActionResult Edit(int id)
        {
            // Id'ye göre aracı al
            var car = _unitOfWork.Cars.Get(id);

            // Kategori dropdown için tüm markalar
            ViewBag.Brands = _unitOfWork.Brands.GetAll().ToList();

            return View(car);
        }

        // Araç düzenleme işlemi (POST)
        [HttpPost]
        public IActionResult Edit(Car car, IFormFile imageFile)
        {
            // Düzenlenecek aracı eski haliyle al
            var existingCar = _unitOfWork.Cars.Get(car.Id);

            if (existingCar == null)
                return NotFound();

            // Eski resmi sakla
            string oldImage = existingCar.ImagePath;

            // Eğer yeni resim yüklenmişse
            if (imageFile != null && imageFile.Length > 0)
            {
                string uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/cars");

                if (!Directory.Exists(uploadDir))
                    Directory.CreateDirectory(uploadDir);

                // Yeni resim dosya adı
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                string filePath = Path.Combine(uploadDir, fileName);

                // Yeni resmi yükle
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    imageFile.CopyTo(stream);
                }

                // Eski resim var ise sil
                if (!string.IsNullOrEmpty(oldImage))
                {
                    string oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", oldImage.TrimStart('/'));

                    if (System.IO.File.Exists(oldPath))
                        System.IO.File.Delete(oldPath);
                }

                // Yeni resim yolunu kaydet
                existingCar.ImagePath = "/images/cars/" + fileName;
            }
            else
            {
                // Yeni resim yüklenmediyse eski resmi koru
                existingCar.ImagePath = oldImage;
            }

            // Diğer alanları güncelle
            existingCar.Model = car.Model;
            existingCar.Year = car.Year;
            existingCar.Price = car.Price;
            existingCar.BrandId = car.BrandId;
            existingCar.isAvailable = car.isAvailable;

            // Güncelle ve kaydet
            _unitOfWork.Cars.Update(existingCar);
            _unitOfWork.Save();

            return RedirectToAction("Index");
        }

        // Araç silme işlemi
        [HttpPost]
        public IActionResult Delete(int id)
        {
            // Silinecek aracı bul
            var car = _unitOfWork.Cars.Get(id);

            if (car == null)
                return NotFound();

            // Fotoğraf varsa fiziksel olarak sil
            if (!string.IsNullOrEmpty(car.ImagePath))
            {
                string fullPath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    car.ImagePath.TrimStart('/')
                );

                if (System.IO.File.Exists(fullPath))
                    System.IO.File.Delete(fullPath);
            }

            // Aracı veritabanından sil
            _unitOfWork.Cars.Remove(car);
            _unitOfWork.Save();

            return Ok();
        }
    }
}
