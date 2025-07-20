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
    public class LorryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LorryController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Lorry
        public async Task<IActionResult> Index()
        {
            var lorries = await _context.Lorries.ToListAsync();
            return View(lorries);
        }

        // GET: Lorry/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var lorry = await _context.Lorries.FirstOrDefaultAsync(m => m.LorryId == id);
            if (lorry == null) return NotFound();

            return View(lorry);
        }

        // GET: Lorry/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Lorry/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Lorry lorry)
        {
            if (!ModelState.IsValid)
                return View(lorry);

            _context.Lorries.Add(lorry);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Lorry/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var lorry = await _context.Lorries.FindAsync(id);
            if (lorry == null) return NotFound();

            return View(lorry);
        }

        // POST: Lorry/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Lorry lorry)
        {
            if (!ModelState.IsValid)
                return View(lorry);

            var existing = await _context.Lorries.FindAsync(lorry.LorryId);
            if (existing == null) return NotFound();

            existing.NumberPlate = lorry.NumberPlate;
            existing.Model = lorry.Model;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Lorry/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var lorry = await _context.Lorries.FirstOrDefaultAsync(m => m.LorryId == id);
            if (lorry == null) return NotFound();

            return View(lorry);
        }

        // POST: Lorry/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var lorry = await _context.Lorries.FindAsync(id);
            if (lorry != null)
            {
                _context.Lorries.Remove(lorry);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool LorryExists(int id)
        {
            return _context.Lorries.Any(e => e.LorryId == id);
        }
    }
}
