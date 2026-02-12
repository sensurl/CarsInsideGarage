using CarsInsideGarage.Services.GarageSession;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarsInsideGarage.Data;

namespace CarsInsideGarage.Controllers
{
    public class ParkingSessionsController : Controller
    {
        private readonly IParkingSessionService _service;
        private readonly GarageDbContext _context;

        public ParkingSessionsController(IParkingSessionService service, GarageDbContext dbContext)
        {
            _service = service;
            _context = dbContext;
        }

        public async Task<IActionResult> Active(int carId)
        {
            var session = await _service.GetActiveSessionByCarAsync(carId);

            if (session == null)
                return View("NoActiveSession");

            return View(session);
        }

        [HttpPost]
        public async Task<IActionResult> Pay(int sessionId, decimal amount)
        {
            try
            {
                await _service.PayAsync(sessionId, amount);

                // Get the CarId via the service
                int carId = await _service.GetCarIdBySessionId(sessionId);

                return RedirectToAction("Active", new { carId });
            }
            catch (Exception ex)
            {
                // Showing why payment failed (e.g. negative amount)
                TempData["Error"] = ex.Message;
                int carId = await _service.GetCarIdBySessionId(sessionId);
                return RedirectToAction("Active", new { carId });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Start(int garageId, int carId)
        {
            try
            {
                // 1. Call the service to create the entry in the DB
                await _service.StartSessionAsync(garageId, carId);

                // 2. Redirect to the "Active" view so the user sees their ticket
                return RedirectToAction("Active", new { carId = carId });
            }
            catch (Exception ex)
            {
                // If the car is already parked, show the error
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index", "Cars");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Exit(int sessionId)
        {
            int carId = await _service.GetCarIdBySessionId(sessionId);

            try
            {
                await _service.EndSessionAsync(sessionId);

                // UX Decision: Instead of Redirecting to "Active" (which will show 'No Session'), 
                // maybe redirect to Car Details or a 'Goodbye' success page.
                return RedirectToAction("Active", new { carId });
            }
            catch (Exception ex)
            {
                // This catches the "Outstanding balance" exception from Service
                TempData["Error"] = ex.Message;
                return RedirectToAction("Active", new { carId });
            }
        }


        
    }

}
