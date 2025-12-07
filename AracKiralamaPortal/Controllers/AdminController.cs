using AracKiralamaPortal.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AracKiralamaPortal.Controllers
{
    [Authorize(AuthenticationSchemes = "AdminCookie")]
    public class AdminController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public AdminController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            ViewBag.BrandCount = _unitOfWork.Brands.GetAll().Count();
            ViewBag.CarCount = _unitOfWork.Cars.GetAll().Count();
            return View();
        }
    }
}
