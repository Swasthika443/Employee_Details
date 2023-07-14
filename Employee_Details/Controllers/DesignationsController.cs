using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Employee_Details.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Employee_Details.Controllers
{
    public class DesignationsController : Controller
    {
        private readonly EmployeeContext _context;

        public DesignationsController(EmployeeContext context)
        {
            _context = context;
        }

        // GET: Designations
        public async Task<IActionResult> Index(string message)
        {
            string xmlData = "<Designation operation='DETAIL'></Designation>"; // XML data for the stored procedure
            var designation = await _context.Designations.FromSqlRaw("EXEC sp_Designation @designationData", new SqlParameter("@designationData", xmlData)).ToListAsync();
            return View(designation);
        }

       
        // GET: Designations/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Designations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Designation designation)
        {
            if (ModelState.IsValid)
            {
                string xmlData = $"<Designation operation='INSERT'><DesignationId>{designation.DesignationId}</DesignationId><DesignationName>{designation.DesignationName}</DesignationName></Designation>";

                var xmlParameter = new SqlParameter("@designationData", SqlDbType.Xml)
                {
                    Value = xmlData
                };

                await _context.Database.ExecuteSqlRawAsync("EXEC sp_Designation @designationData", xmlParameter);

                return RedirectToAction(nameof(Index));
            }
            return View(designation);
        }

      
        // GET: Designations/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.Designations == null)
            {
                return NotFound();
            }
            string xmlData = $"<Designation operation='SELECTBYID' ID='{id}'></Designation>"; // XML data for the stored procedure
            var designation = _context.Designations.FromSqlRaw("EXEC sp_Designation @designationData", new SqlParameter("@designationData", xmlData)).AsEnumerable().FirstOrDefault();
            if (designation == null)
            {
                return NotFound();
            }

            return View(designation);
        }

        // POST: Designations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var designation = await _context.Designations.FindAsync(id);
            if (designation == null)
            {
                return NotFound();
            }
            var employees = await _context.Employees.Where(e => e.Designation == id).ToListAsync();
            if (employees.Count > 0)
            {
                TempData["Message"] = "Delete operation failed. This gender is associated with one or more employees.";

                return RedirectToAction(nameof(Index), new { message = TempData["Message"] });
            }

            string xmlData = $"<Designation operation='DELETE' ID='{id}'></Designation>"; // XML data for the stored procedure

            var xmlParameter = new SqlParameter("@designationData", SqlDbType.Xml)
            {
                Value = xmlData
            };

            await _context.Database.ExecuteSqlRawAsync("EXEC sp_Designation @designationData", xmlParameter);

            TempData["Message"] = "Delete operation successful.";
            return RedirectToAction(nameof(Index), new { message = TempData["Message"] });
        }

        private bool DesignationExists(string id)
        {
          return (_context.Designations?.Any(e => e.DesignationName == id)).GetValueOrDefault();
        }
    }
}
