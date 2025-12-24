using AracKiralamaPortal.Data;
using AracKiralamaPortal.Hubs;
using AracKiralamaPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace AracKiralamaPortal.Controllers
{
    [Authorize]
    public class ReservationController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHubContext<ChatHub> _hubContext;

        public ReservationController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IHubContext<ChatHub> hubContext)
        {
            _context = context;
            _userManager = userManager;
            _hubContext = hubContext;
        }

        public IActionResult Create(int? carId)
        {
            ViewBag.Cars = new SelectList(
                _context.Cars.Where(c => c.isAvailable),
                "Id",
                "Model"
            );

            Car selectedCar = null;
            if (carId != null)
            {
                selectedCar = _context.Cars
                    .Include(c => c.Brand)
                    .Include(c => c.VehicleType)
                    .Include(c => c.VehicleSubType)
                    .FirstOrDefault(c => c.Id == carId);
            }
            ViewBag.SelectedCar = selectedCar;

            return View(new Reservation
            {
                CarId = carId ?? 0,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Reservation reservation)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            reservation.UserId = user.Id;
            reservation.Status = "Beklemede";

            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            await _hubContext.Clients.All.SendAsync("NewReservationArrived", new
            {
                id = reservation.Id,
                userId = user.UserName ?? user.Email ?? user.Id,
                startDate = reservation.StartDate.ToString("yyyy-MM-dd"),
                endDate = reservation.EndDate.ToString("yyyy-MM-dd"),
                status = reservation.Status
            });

            TempData["Success"] = "Rezervasyon talebiniz alındı, admin onayını bekliyor.";
            return RedirectToAction("MyReservations");
        }

        public async Task<IActionResult> MyReservations()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var reservations = await _context.Reservations
                .Where(r => r.UserId == user.Id)
                .Include(r => r.Car)
                .Include(r => r.Car.Brand)
                .ToListAsync();

            return View(reservations);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            var reservation = await _context.Reservations.FirstOrDefaultAsync(r => r.Id == id);
            if (reservation == null) return NotFound();

            reservation.Status = "Onaylandı";
            _context.Reservations.Update(reservation);
            await _context.SaveChangesAsync();

            await _hubContext.Clients.All.SendAsync("ReservationStatusChanged", new
            {
                reservationId = reservation.Id,
                userId = reservation.UserId,
                status = reservation.Status
            });

            TempData["Success"] = "Rezervasyon onaylandı.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id)
        {
            var reservation = await _context.Reservations.FirstOrDefaultAsync(r => r.Id == id);
            if (reservation == null) return NotFound();

            reservation.Status = "Reddedildi";
            _context.Reservations.Update(reservation);
            await _context.SaveChangesAsync();

            await _hubContext.Clients.All.SendAsync("ReservationStatusChanged", new
            {
                reservationId = reservation.Id,
                userId = reservation.UserId,
                status = reservation.Status
            });

            TempData["Success"] = "Rezervasyon reddedildi.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var reservation = _context.Reservations.Find(id);
            if (reservation == null) return NotFound();

            _context.Reservations.Remove(reservation);
            _context.SaveChanges();

            return Ok();
        }

    }
}
