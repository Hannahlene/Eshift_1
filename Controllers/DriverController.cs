using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using EShift.Data;
using EShift.Models;

namespace EShift.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DriverController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DriverController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Driver
        public async Task<IActionResult> Index()
        {
            var drivers = await _context.Drivers.ToListAsync();
            return View(drivers);
        }

        // GET: Driver/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var driver = await FindDriverAsync(id);
            return driver == null ? NotFound() : View(driver);
        }

        // GET: Driver/Create
        public IActionResult Create() => View();

        // POST: Driver/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Driver driver)
        {
            if (!ModelState.IsValid)
                return View(driver);

            _context.Add(driver);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Driver/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var driver = await FindDriverAsync(id);
            return driver == null ? NotFound() : View(driver);
        }

        // POST: Driver/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Driver driver)
        {
            if (!ModelState.IsValid)
                return View(driver);

            var existing = await _context.Drivers.FindAsync(driver.DriverId);
            if (existing == null)
                return NotFound();

            existing.Name = driver.Name;
            existing.LicenseNumber = driver.LicenseNumber;
            existing.Phone = driver.Phone;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Driver/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var driver = await FindDriverAsync(id);
            return driver == null ? NotFound() : View(driver);
        }

        // POST: Driver/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var driver = await _context.Drivers.FindAsync(id);
            if (driver != null)
            {
                _context.Drivers.Remove(driver);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private Task<Driver> FindDriverAsync(int? id)
        {
            if (id == null)
                return Task.FromResult<Driver>(null);

            return _context.Drivers.FirstOrDefaultAsync(d => d.DriverId == id);
        }
    }
}
