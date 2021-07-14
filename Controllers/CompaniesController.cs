using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Reflection.Data;
using Reflection.Models;

namespace Reflection.Controllers
{
    public class CompaniesController : Controller
    {
        private readonly Context _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private bool _disposed = false;
        
        public CompaniesController(Context context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        // GET: Companies
        public async Task<IActionResult> Index(
            string sortOrder,
            string currentFilter,
            string searchString,
            int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParam"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["EmployeesSortParam"] = sortOrder == "Employees" ? "employees_desc" : "Employees";

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            //var companies = from c in _context.Companies
            //                 select c;
            var companies = _context.Companies
                .Include(c => c.Employees)
                .Select(c => c);

            if (!String.IsNullOrEmpty(searchString))
            {
                companies = companies.Where(c => c.Name.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    companies = companies.OrderByDescending(c => c.Name);
                    break;
                case "Employees":
                    companies = companies.OrderBy(c => c.Employees.Count);
                    break;
                case "employees_desc":
                    companies = companies.OrderByDescending(c => c.Employees.Count);
                    break;
                default:
                    companies = companies.OrderBy(c => c.Name);
                    break;
            }

            int pageSize = 10;
            return View(await PaginatedList<Company>.CreateAsync(companies.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // GET: Companies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var company = await _context.Companies
                .Include(c => c.Employees)
                .FirstOrDefaultAsync(c => c.CompanyId == id);
            if (company == null)
            {
                return NotFound();
            }

            return View(company);
        }

        // GET: Companies/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Companies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Email,Website,LogoFile")] Company company)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (company.LogoFile != null)
                    {
                        // Generate a unique filename for the logo.
                        string wwwRootPath = _hostEnvironment.WebRootPath;
                        string extension = Path.GetExtension(company.LogoFile.FileName);
                        string filePrefix = Guid.NewGuid().ToString() + "_";
                        string fileName = company.LogoName = filePrefix + DateTime.Now.ToString("yymmssfff") + extension;
                        string filePath = Path.Combine(wwwRootPath + "/img/", fileName);

                        // Save the file to wwwroot/img/.
                        using (FileStream fileStream = new(filePath, FileMode.Create))
                        {
                            await company.LogoFile.CopyToAsync(fileStream);
                        }
                    }

                    _context.Add(company);
                    await _context.SaveChangesAsync();
                    TempData["MessageSuccess"] = $"<strong>{company.Name}</strong> has been added.";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to save changes. " +
                "Try again, and if the problem persists " +
                "see your system administrator.");
            }
            return View(company);
        }

        // GET: Companies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var company = await _context.Companies.FindAsync(id);
            if (company == null)
            {
                return NotFound();
            }
            return View(company);
        }

        // POST: Companies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id, Company company)
        {
            if (id == null)
            {
                return NotFound();
            }

            var companyToUpdate = await _context.Companies.FirstOrDefaultAsync(c => c.CompanyId == id);

            if (ModelState.IsValid && company.LogoFile != null)
            {
                // Generate a unique filename for the logo.
                string wwwRootPath = _hostEnvironment.WebRootPath;
                string extension = Path.GetExtension(company.LogoFile.FileName);
                string filePrefix = Guid.NewGuid().ToString() + "_";
                string fileName = company.LogoName = filePrefix + DateTime.Now.ToString("yymmssfff") + extension;
                string filePath = Path.Combine(wwwRootPath + "/img/", fileName);

                // Save the file to wwwroot/img/.
                using (FileStream fileStream = new(filePath, FileMode.Create))
                {
                    await company.LogoFile.CopyToAsync(fileStream);
                }

                // Delete the old logo.
                var imagePath = Path.Combine(wwwRootPath, "img", companyToUpdate.LogoName);
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }

                // Update the Company's FileName value.
                companyToUpdate.LogoName = fileName;
            }

            if (await TryUpdateModelAsync<Company>(
                    companyToUpdate,
                    "",
                    c => c.Name, c => c.Email, c => c.Website, c => c.LogoName))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    TempData["MessageSuccess"] = $"<strong>{company.Name}</strong> has been updated.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists, " +
                    "see your system administrator.");
                }
            }

            return View(companyToUpdate);
        }

        // GET: Companies/Delete/5
        public async Task<IActionResult> Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            var company = await _context.Companies
                .Include(c => c.Employees)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.CompanyId == id);
            if (company == null)
            {
                return NotFound();
            }

            if (saveChangesError.GetValueOrDefault())
            {
                ViewData["ErrorMessage"] =
                    "Delete failed. Try again, and if the problem persists " +
                    "see your system administrator.";
            }

            return View(company);
        }

        // POST: Companies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, string deleteEmployees)
        {
            var company = await _context.Companies.FindAsync(id);
            if (company == null)
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                if (company.LogoName != null)
                {
                    var imagePath = Path.Combine(_hostEnvironment.WebRootPath, "img", company.LogoName);
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }

                var employees = _context.Employees.Where(e => e.CompanyId == id);
                if (deleteEmployees != null)
                {
                    
                    foreach (var employee in employees)
                    {
                        _context.Employees.Remove(employee);
                    }
                }
                else
                {
                    foreach (var employee in employees)
                    {
                        employee.CompanyId = null;
                    }
                }

                _context.Companies.Remove(company);
                await _context.SaveChangesAsync();
                TempData["MessageSuccess"] = $"<strong>{company.Name}</strong> has been deleted.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                TempData["MessageFailure"] = "Failed to delete entry. Please try again.";
                return RedirectToAction(nameof(Delete), new { id, saveChangesError = true });
            }
        }

        private bool CompanyExists(int id)
        {
            return _context.Companies.Any(e => e.CompanyId == id);
        }

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _context.Dispose();
            }

            _disposed = true;

            base.Dispose(disposing);
        }
    }
}
