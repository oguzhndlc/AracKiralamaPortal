using AracKiralamaPortal.Models;
using AracKiralamaPortal.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AracKiralamaPortal.Controllers
{
    [Authorize(AuthenticationSchemes = "AdminCookie")]
    public class BrandController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public BrandController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var brands = _unitOfWork.Brands.GetAll();
            return View(brands);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Brand brand)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Brands.Add(brand);
                _unitOfWork.Save();
                return RedirectToAction("Index");
            }

            return View(brand);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var brand = _unitOfWork.Brands.Get(id);
            return View(brand);
        }

        [HttpPost]
        public IActionResult Edit(Brand brand)
        {
            _unitOfWork.Brands.Update(brand);
            _unitOfWork.Save();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var brand = _unitOfWork.Brands.Get(id);
            if (brand != null)
            {
                _unitOfWork.Brands.Remove(brand);
                _unitOfWork.Save();
            }
            return Ok();
        }
    }
}
