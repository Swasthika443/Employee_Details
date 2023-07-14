using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Employee_Details.Models;
using Microsoft.Data.SqlClient;
using System.Xml.Linq;
using System.Data;

namespace Employee_Details.Controllers
{
    public class GendersController : Controller
    {
        private readonly EmployeeContext _context;

        public GendersController(EmployeeContext context)
        {
            _context = context;
        }

        // GET: Genders
        public async Task<IActionResult> Index(string message)
        {
          

            string xmlData = "<Gender operation='DETAIL'></Gender>"; // XML data for the stored procedure
            var genders = await _context.Genders.FromSqlRaw("EXEC sp_Gender @genderData", new SqlParameter("@genderData", xmlData)).ToListAsync();
            return View(genders);
           
        }

        

        // GET: Genders/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Genders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Gender gender)
        {
            if (ModelState.IsValid)
            {
                string xmlData = $"<Gender operation='INSERT'><GenderId>{gender.GenderId}</GenderId><GenderName>{gender.GenderName}</GenderName></Gender>";

                var xmlParameter = new SqlParameter("@genderData", SqlDbType.Xml)
                {
                    Value = xmlData
                };

                await _context.Database.ExecuteSqlRawAsync("EXEC sp_Gender @genderData", xmlParameter);

                return RedirectToAction(nameof(Index));
            }

            return View(gender);
        }

      
        // GET: Genders/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.Genders == null)
            {
                return NotFound();
            }

            string xmlData = $"<Gender operation='SELECTBYID' ID='{id}'></Gender>"; // XML data for the stored procedure
            var gender = _context.Genders.FromSqlRaw("EXEC sp_Gender @genderData", new SqlParameter("@genderData", xmlData)).AsEnumerable().FirstOrDefault();
            if (gender == null)
            {
                return NotFound();
            }

            return View(gender);
        }

        // POST: Genders/Delete/5
        // POST: Genders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) 
        {
            var gender = await _context.Genders.FindAsync(id);
            if (gender == null)
            {
                return NotFound();
            }

            var employees = await _context.Employees.Where(e => e.Gender == id).ToListAsync();
            if (employees.Count > 0)
            {
                TempData["Message"] = "Delete operation failed. This gender is associated with one or more employees.";
                return RedirectToAction(nameof(Index), new { message = TempData["Message"] });
            }

            string xmlData = $"<Gender operation='DELETE' ID='{id}'></Gender>"; // XML data for the stored procedure

            var xmlParameter = new SqlParameter("@genderData", System.Data.SqlDbType.Xml)
            {
                Value = xmlData
            };

            await _context.Database.ExecuteSqlRawAsync("EXEC sp_Gender @genderData", xmlParameter);

            TempData["Message"] = "Delete operation successful.";
            return RedirectToAction(nameof(Index), new { message = TempData["Message"] });
        }


        private bool GenderExists(string id)
        {
          return (_context.Genders?.Any(e => e.GenderName == id)).GetValueOrDefault();
        }
    }
}
