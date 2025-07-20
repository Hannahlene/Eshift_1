using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using EShift.Data;
using EShift.Models;

namespace EShift.Controllers
{
    [Authorize(Roles = "Admin")]
    public class JobController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const int PageSize = 5;

        public JobController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Job
        public async Task<IActionResult> Index(string status, int pageNumber = 1)
        {
            var jobsQuery = _context.Jobs.Include(j => j.Customer).AsQueryable();

            if (!string.IsNullOrEmpty(status) && Enum.TryParse(status, out JobStatus jobStatus))
            {
                jobsQuery = jobsQuery.Where(j => j.Status == jobStatus);
            }

            int totalCount = await jobsQuery.CountAsync();
            int totalPages = (int)Math.Ceiling(totalCount / (double)PageSize);

            var jobs = await jobsQuery
                .OrderByDescending(j => j.JobDate)
                .Skip((pageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            ViewData["CurrentFilterStatus"] = status;
            ViewData["CurrentPage"] = pageNumber;
            ViewData["TotalPages"] = totalPages;
            ViewData["HasPreviousPage"] = pageNumber > 1;
            ViewData["HasNextPage"] = pageNumber < totalPages;

            return View(jobs);
        }

        // GET: Job/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var job = await _context.Jobs
                .Include(j => j.Customer)
                .Include(j => j.Loads).ThenInclude(l => l.TransportUnit)
                .Include(j => j.Loads).ThenInclude(l => l.LoadProducts).ThenInclude(lp => lp.Product)
                .FirstOrDefaultAsync(m => m.JobId == id);

            return job == null ? NotFound() : View(job);
        }

        // GET: Job/Create
        // public IActionResult Create()
        // {
        //     var customers = _context.Customers.ToList();
        //     ViewBag.CustomerId = new SelectList(customers, "CustomerId", "CustomerName");
        //     return View();
        // }
            public IActionResult Create()
            {
                var customers = _context.Customers.ToList();

                if (customers == null || !customers.Any())
                {
                    // This will help you know if your DB is empty or disconnected
                    throw new Exception("No customers found in the database.");
                }

                ViewBag.CustomerId = new SelectList(customers, "CustomerId", "Name"); // Adjust "Name" to match your model

                return View();
            }

        // POST: Job/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CustomerId,StartLocation,Destination,JobDate,Status")] Job job)
        {
            if (!ModelState.IsValid)
            {
                ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerName", job.CustomerId);
                return View(job);
            }

            _context.Add(job);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Job created successfully!";
            return RedirectToAction(nameof(Index));
        }

        // GET: Job/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var job = await _context.Jobs.Include(j => j.Customer).FirstOrDefaultAsync(j => j.JobId == id);
            return job == null ? NotFound() : View(job);
        }

        // POST: Job/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Job job)
        {
            ModelState.Remove("CustomerId");
            ModelState.Remove("Customer");

            if (!ModelState.IsValid)
            {
                await SetCustomerOnModel(job);
                ModelState.AddModelError("", "Please correct the highlighted errors and try again.");
                return View(job);
            }

            var existingJob = await _context.Jobs.Include(j => j.Loads).FirstOrDefaultAsync(j => j.JobId == job.JobId);
            if (existingJob == null)
            {
                TempData["ErrorMessage"] = "Job not found.";
                return NotFound();
            }

            if (existingJob.Status != job.Status && job.Status == JobStatus.Completed)
            {
                if (existingJob.Loads.Any(l => l.Status != LoadStatus.Assigned))
                {
                    ModelState.AddModelError("Status", "Cannot complete job unless all loads are assigned.");
                    await SetCustomerOnModel(job);
                    return View(job);
                }
            }

            try
            {
                existingJob.StartLocation = job.StartLocation;
                existingJob.Destination = job.Destination;
                existingJob.JobDate = job.JobDate;
                existingJob.Status = job.Status;

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Job updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException) when (!JobExists(job.JobId))
            {
                TempData["ErrorMessage"] = "Job was deleted by another user.";
                return NotFound();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                await SetCustomerOnModel(job);
                return View(job);
            }
        }

        // GET: Job/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var job = await _context.Jobs.Include(j => j.Customer).FirstOrDefaultAsync(j => j.JobId == id);
            return job == null ? NotFound() : View(job);
        }

        // POST: Job/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var job = await _context.Jobs.Include(j => j.Loads).FirstOrDefaultAsync(j => j.JobId == id);

                if (job == null)
                {
                    TempData["ErrorMessage"] = "Job not found.";
                    await transaction.RollbackAsync();
                    return RedirectToAction(nameof(Index));
                }

                _context.Jobs.Remove(job);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["SuccessMessage"] = "Job deleted successfully!";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData["ErrorMessage"] = $"Error deleting job: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool JobExists(int id) => _context.Jobs.Any(e => e.JobId == id);

        private async Task SetCustomerOnModel(Job job)
        {
            var jobWithCustomer = await _context.Jobs.Include(j => j.Customer).AsNoTracking().FirstOrDefaultAsync(j => j.JobId == job.JobId);
            if (jobWithCustomer != null)
            {
                job.Customer = jobWithCustomer.Customer;
            }
        }
    }
}
