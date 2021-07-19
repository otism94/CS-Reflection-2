using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Reflection.Data;
using Reflection.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Reflection.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly Context _context;
        private bool _disposed = false;

        public EmployeesController(Context context)
        {
            _context = context;
        }

        /// <summary>
        /// GET: Assigned employees.
        /// </summary>
        /// <param name="s">Sort order.</param>
        /// <param name="f">Current filter (i.e. search term when there is a sort order).</param>
        /// <param name="t">Search term.</param>
        /// <param name="p">Page number.</param>
        /// <returns>Paginated list view of assigned employees.</returns>
        public async Task<IActionResult> Index(
            string s,
            string f,
            string t,
            int? p)
        {
            ViewData["CurrentSort"] = s;
            ViewData["NameSortParam"] = String.IsNullOrEmpty(s) ? "Name_desc" : "";
            ViewData["CompanySortParam"] = s == "Company" ? "Company_desc" : "Company";

            if (t != null)
            {
                p = 1;
            }
            else
            {
                t = f;
            }

            ViewData["CurrentFilter"] = t;

            var employees = (from e in _context.Employees
                             join c in _context.Companies on e.CompanyId equals c.CompanyId
                             select e).Include("Company");

            ViewData["UnassignedEmployees"] = _context.Employees.Where(e => e.CompanyId == null).Count();

            if (!String.IsNullOrEmpty(t))
            {
                employees = employees.AsQueryable().Where(e => e.LastName.Contains(t)
                    || e.FirstName.Contains(t)
                    || (e.LastName + ", " + e.FirstName).Contains(t)
                    || (e.FirstName + " " + e.LastName).Contains(t)
                    || e.Email.Contains(t)
                    || e.Phone.Contains(t)
                    || e.Company.Name.Contains(t));
            }

            employees = s switch
            {
                "Name_desc" => employees.OrderByDescending(e => e.LastName),
                "Company" => employees.OrderBy(e => e.CompanyId),
                "Company_desc" => employees.OrderByDescending(e => e.CompanyId),
                _ => employees.OrderBy(e => e.LastName),
            };

            int pageSize = 10;

            return View(await PaginatedList<Employee>.CreateAsync(employees.AsNoTracking(), p ?? 1, pageSize));
        }

        /// <summary>
        /// GET: Unassigned employees.
        /// </summary>
        /// <param name="s">Sort order.</param>
        /// <param name="f">Current filter (i.e. search term when there is a sort order).</param>
        /// <param name="t">Search term.</param>
        /// <param name="p">Page number.</param>
        /// <returns>Paginated list view of unassigned employees.</returns>
        public async Task<IActionResult> Unassigned(
            string s,
            string f,
            string t,
            int? p)
        {
            ViewData["CurrentSort"] = s;
            ViewData["NameSortParam"] = String.IsNullOrEmpty(s) ? "name_desc" : "";

            if (t != null)
            {
                p = 1;
            }
            else
            {
                t = f;
            }

            ViewData["CurrentFilter"] = t;

            var employees = _context.Employees.Where(e => e.CompanyId == null);
            if (!employees.Any())
            {
                return RedirectToAction(nameof(Index));
            }

            if (!String.IsNullOrEmpty(t))
            {
                employees = employees.Where(e => e.LastName.Contains(t)
                    || e.FirstName.Contains(t)
                    || (e.LastName + ", " + e.FirstName).Contains(t)
                    || (e.FirstName + " " + e.LastName).Contains(t)
                    || e.Email.Contains(t)
                    || e.Phone.Contains(t));
            }

            employees = s switch
            {
                "name_desc" => employees.OrderByDescending(e => e.LastName),
                _ => employees.OrderBy(e => e.LastName),
            };

            int pageSize = 10;

            return View(await PaginatedList<Employee>.CreateAsync(employees.AsNoTracking(), p ?? 1, pageSize));
        }

        /// <summary>
        /// GET: Employee details.
        /// </summary>
        /// <param name="id">Entity's EmployeeId.</param>
        /// <returns>Employee details view.</returns>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.Company)
                .FirstOrDefaultAsync(e => e.EmployeeId == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        /// <summary>
        /// GET: Create employee.
        /// </summary>
        /// <returns>Create employee view.</returns>
        public IActionResult Create()
        {
            ViewData["CompanyId"] = new SelectList(_context.Companies, "CompanyId", "Name");
            return View();
        }

        /// <summary>
        /// POST: Create employee.
        /// </summary>
        /// <param name="employee">User-entered details for the employee entity.</param>
        /// <returns>Employees index view on success.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FirstName,LastName,Email,Phone,CompanyId")] Employee employee)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(employee);
                    await _context.SaveChangesAsync();
                    TempData["MessageSuccess"] = $"<strong>{employee.FirstName} {employee.LastName}</strong> has been added.";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to save changes. " +
                "Try again, and if the problem persists " +
                "see your system administrator.");
            }

            ViewData["CompanyId"] = new SelectList(_context.Companies, "CompanyId", "Name", employee.CompanyId);
            return View(employee);
        }

        /// <summary>
        /// GET: Edit employee view.
        /// </summary>
        /// <param name="id">Entity's EmployeeId.</param>
        /// <returns>Employee edit view.</returns>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.Company)
                .FirstOrDefaultAsync(e => e.EmployeeId == id);
            if (employee == null)
            {
                return NotFound();
            }
            
            ViewData["CompanyId"] = new SelectList(_context.Companies, "CompanyId", "Name", employee.CompanyId);
            
            return View(employee);
        }

        /// <summary>
        /// POST: Edit employee.
        /// </summary>
        /// <param name="id">Entity's EmployeeId.</param>
        /// <param name="employee">User-entered details for the employee entity.</param>
        /// <returns>Employees index view on success.</returns>
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id, [Bind("FirstName,LastName,Email,Phone,CompanyId")] Employee employee)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employeeToUpdate = await _context.Employees.FirstOrDefaultAsync(e => e.EmployeeId == id);
            bool redirectToUnassigned = false;
            if (employeeToUpdate.CompanyId == null)
            {
                redirectToUnassigned = true;
            }

            if (await TryUpdateModelAsync<Employee>(
                employeeToUpdate,
                "",
                e => e.FirstName, e => e.LastName, e => e.Email, e => e.Phone, e => e.CompanyId))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    TempData["MessageSuccess"] = $"<strong>{employeeToUpdate.FirstName} {employeeToUpdate.LastName}</strong> has been updated.";
                    
                    if (redirectToUnassigned && _context.Employees.Where(e => e.CompanyId == null).Any())
                    {
                        return RedirectToAction(nameof(Unassigned));
                    }

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists, " +
                    "see your system administrator.");
                }
            }

            return View(employeeToUpdate);
        }

        /// <summary>
        /// GET: Employee delete view.
        /// </summary>
        /// <param name="id">Entity's EmployeeId.</param>
        /// <param name="saveChangesError">Whether the view was accessed via a failed post attempt.</param>
        /// <returns>Employee delete view.</returns>
        public async Task<IActionResult> Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .AsNoTracking()
                .Include(e => e.Company)
                .FirstOrDefaultAsync(e => e.EmployeeId == id);
            if (employee == null)
            {
                return NotFound();
            }

            if (saveChangesError.GetValueOrDefault())
            {
                ViewData["ErrorMessage"] =
                    "Delete failed. Try again, and if the problem persists " +
                    "see your system administrator.";
            }

            return View(employee);
        }

        /// <summary>
        /// POST: Delete employee.
        /// </summary>
        /// <param name="id">Entity's EmployeeId.</param>
        /// <returns>Employees index view on success.</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return RedirectToAction(nameof(Index));
            }

            bool redirectToUnassigned = false;
            if (employee.CompanyId == null)
            {
                redirectToUnassigned = true;
            }

            try
            {
                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();
                TempData["MessageSuccess"] = $"<strong>{employee.FirstName} {employee.LastName}</strong> has been deleted.";
                
                if (redirectToUnassigned && _context.Employees.Where(e => e.CompanyId == null).Any())
                {
                    return RedirectToAction(nameof(Unassigned));
                }

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                TempData["MessageFailure"] = "Failed to delete entry. Please try again.";
                return RedirectToAction(nameof(Delete), new { id, saveChangesError = true });
            }
        }

        /// <summary>
        /// Checks whether an employee with the supplied ID exists.
        /// </summary>
        /// <param name="id">Entity's EmployeeId.</param>
        /// <returns>True if employee exists, otherwise false.</returns>
        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.EmployeeId == id);
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
