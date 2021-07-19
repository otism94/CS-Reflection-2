using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Reflection.Data;
using Reflection.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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

        /// <summary>
        /// GET: Companies.
        /// </summary>
        /// <param name="s">Sort order.</param>
        /// <param name="f">Current filter (i.e. search term when there is a sort order).</param>
        /// <param name="t">Search term.</param>
        /// <param name="p">Page number.</param>
        /// <returns>Paginated list view of companies.</returns>
        public async Task<IActionResult> Index(
            string s,
            string f,
            string t,
            int? p)
        {
            ViewData["CurrentSort"] = s;
            ViewData["NameSortParam"] = String.IsNullOrEmpty(s) ? "name_desc" : "";
            ViewData["EmployeesSortParam"] = s == "Employees" ? "employees_desc" : "Employees";

            if (t != null)
            {
                p = 1;
            }
            else
            {
                t = f;
            }

            ViewData["CurrentFilter"] = t;

            var companies = _context.Companies
                .Include(c => c.Employees)
                .Select(c => c);

            if (!String.IsNullOrEmpty(t))
            {
                companies = companies.Where(c => c.Name.Contains(t)
                || c.Email.Contains(t)
                || c.Website.Contains(t));
            }

            companies = s switch
            {
                "name_desc" => companies.OrderByDescending(c => c.Name),
                "Employees" => companies.OrderBy(c => c.Employees.Count),
                "employees_desc" => companies.OrderByDescending(c => c.Employees.Count),
                _ => companies.OrderBy(c => c.Name),
            };

            int pageSize = 10;

            return View(await PaginatedList<Company>.CreateAsync(companies.AsNoTracking(), p ?? 1, pageSize));
        }

        /// <summary>
        /// GET: Company details.
        /// </summary>
        /// <param name="id">Entity's CompanyId.</param>
        /// <returns>Company details view.</returns>
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

        /// <summary>
        /// GET: Create company.
        /// </summary>
        /// <returns>Create company view.</returns>
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// POST: Create company.
        /// </summary>
        /// <param name="company">User-entered details for the company entity.</param>
        /// <returns>Companies index view on success.</returns>
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
                        {
                            using FileStream fileStream = new(filePath, FileMode.Create);
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

        /// <summary>
        /// GET: Edit company view.
        /// </summary>
        /// <param name="id">Entity's CompanyId.</param>
        /// <returns>Company edit view.</returns>
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

        /// <summary>
        /// POST: Edit company.
        /// </summary>
        /// <param name="id">Entity's CompanyId.</param>
        /// <param name="company">User-entered details for the company entity.</param>
        /// <returns>Companies index view on success.</returns>
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id, [Bind("Name,Email,Website,LogoFile")] Company company)
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

        /// <summary>
        /// GET: Company delete view.
        /// </summary>
        /// <param name="id">Entity's CompanyId.</param>
        /// <param name="saveChangesError">Whether the view was accessed via a failed post attempt.</param>
        /// <returns>Company delete view.</returns>
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

        /// <summary>
        /// POST: Delete company.
        /// </summary>
        /// <param name="id">Entity's CompanyId.</param>
        /// <param name="deleteEmployees">Checkbox value for whether to delete company's employees.</param>
        /// <returns>Companies index view on success.</returns>
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

        /// <summary>
        /// Checks whether a company with the supplied ID exists.
        /// </summary>
        /// <param name="id">Entity's CompanyId.</param>
        /// <returns>True if company exists, otherwise false.</returns>
        private bool CompanyExists(int id)
        {
            return _context.Companies.Any(e => e.CompanyId == id);
        }

        /// <summary>
        /// Disposes the context once operations are complete.
        /// </summary>
        /// <param name="disposing">Whether the context needs to be disposed.</param>
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
