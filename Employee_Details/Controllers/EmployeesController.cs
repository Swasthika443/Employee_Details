using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Employee_Details.Models;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Data;
using Employee_Details.Models.ViewModel;
using System.Drawing.Printing;
using OfficeOpenXml;
using Hangfire;



using System.IO;

namespace Employees.Controllers
{
    public class EmployeesController : Controller
    {
        private EmployeeContext _context;
        private readonly ILogger<EmployeesController> _logger;
        public EmployeesController(EmployeeContext context, ILogger<EmployeesController> logger)
        {
            _logger = logger;
            _context = context;
        }
        // GET: Employees 
        public IActionResult Index(int page = 1, int pageSize = 5, bool fileUploaded = false, string errorMessage = "")
        {
            ViewBag.FileUploaded = fileUploaded;
            ViewBag.ErrorMessage = errorMessage;

            string xmlData = "<Employee operation='DETAIL'></Employee>"; // XML data for the stored procedure

            var totalEmployees = _context.Employees.Count();  //counts the total number of employees.
            var totalPages = (int)Math.Ceiling((double)totalEmployees / pageSize);
            var startIndex = (page - 1) * pageSize;

            var employees = _context.Employees
                .FromSqlRaw("EXEC sp_Employee @employeeData", new SqlParameter("@employeeData", xmlData))
                .AsEnumerable() // Perform composition on the client side
                .Skip(startIndex)
                .Take(pageSize)
                .ToList();


            var genderIds = employees.Select(e => e.Gender).Distinct().ToList();
            var genders = _context.Genders.Where(g => genderIds.Contains(g.GenderId)).ToList();



            foreach (var employee in employees)
            {
                employee.GenderNavigation = genders.FirstOrDefault(g => g.GenderId == employee.Gender);
            }



            var DesignationIds = employees.Select(e => e.Designation).Distinct().ToList();
            var designations = _context.Designations.Where(g => DesignationIds.Contains(g.DesignationId)).ToList();



            foreach (var employee in employees)
            {
                employee.DesignationNavigation = designations.FirstOrDefault(g => g.DesignationId == employee.Designation);
            }



            var employeeView = new EmployeeView()
            {
                Employees = employees.AsQueryable(),
                CurrentPage = page,
                TotalPages = totalPages,
                PageSize = pageSize,

            };



            return View(employeeView);



        }



        // GET: Employees/Details/5 
        // GET: Employees/Details/5 
        public IActionResult Details(int? id)
        {
            string xmlData = $"<Employee operation='SELECTBYID' ID='{id}'></Employee>"; // XML data for the stored procedure 
            var employee = _context.Employees.FromSqlRaw("EXEC sp_Employee @employeeData", new SqlParameter("@employeeData", xmlData)).AsEnumerable().FirstOrDefault();
            return View(employee);
        }



        // GET: Employees/Create 
        public async Task<IActionResult> Create()
        {
            ViewData["Designation"] = new SelectList(_context.Designations, "DesignationName", "DesignationName");
            ViewData["Gender"] = new SelectList(_context.Genders, "GenderName", "GenderName");
            return View();
        }
        // POST: Employees/Create 
        // To protect from overposting attacks, enable the specific properties you want to bind to. 
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        // POST: Employees/Create
        // POST: Employees/Create 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Employee employee)
        {
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(employee.GenderNavigation?.GenderName))
                {
                    var selectedGender = await _context.Genders.FirstOrDefaultAsync(g => g.GenderName == employee.GenderNavigation.GenderName);
                    if (selectedGender != null)
                    {
                        employee.Gender = selectedGender.GenderId;
                    }
                }
                if (!string.IsNullOrEmpty(employee.DesignationNavigation?.DesignationName))
                {
                    var selectedDesignation = await _context.Designations.FirstOrDefaultAsync(d => d.DesignationName == employee.DesignationNavigation.DesignationName);
                    if (selectedDesignation != null)
                    {
                        employee.Designation = selectedDesignation.DesignationId;
                    }
                }
                string xmlData = $"<Employee operation='INSERT'><FirstName>{employee.FirstName}</FirstName><LastName>{employee.LastName}</LastName><Gender>{employee.Gender}</Gender><Age>{employee.Age}</Age><Designation>{employee.Designation}</Designation><Salary>{employee.Salary}</Salary></Employee>";
                var xmlParameter = new SqlParameter("@employeeData", SqlDbType.Xml)
                {
                    Value = xmlData
                };
                await _context.Database.ExecuteSqlRawAsync("EXEC sp_Employee @employeeData", xmlParameter);
                return RedirectToAction(nameof(Index));
            }
            return View(employee);



        }




        // GET: Employees/Edit/5



        public async Task<IActionResult> Edit(int? id)



        {



            if (id == null || _context.Employees == null)



            {
                return NotFound();
            }
            string xmlData = $"<Employee operation='SELECTBYID' ID='{id}'></Employee>"; // XML data for the stored procedure 
            var employee = _context.Employees.FromSqlRaw("EXEC sp_Employee @employeeData", new SqlParameter("@employeeData", xmlData)).AsEnumerable().FirstOrDefault();
            if (employee == null)
            {
                return NotFound();
            }
            ViewData["Designation"] = new SelectList(_context.Designations, "DesignationName", "DesignationName", employee.Designation);
            ViewData["Gender"] = new SelectList(_context.Genders, "GenderName", "GenderName", employee.Gender);
            return View(employee);
        }
        // POST: Employees/Edit/5 
        // To protect from overposting attacks, enable the specific properties you want to bind to. 
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598. 
        // POST: Employees/Edit/5 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Employee employee)
        {
            if (id != employee.EmpId)



            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(employee.GenderNavigation?.GenderName))
                {
                    var selectedGender = await _context.Genders.FirstOrDefaultAsync(g => g.GenderName == employee.GenderNavigation.GenderName);
                    if (selectedGender != null)
                    {
                        employee.Gender = selectedGender.GenderId;
                    }
                }
                if (!string.IsNullOrEmpty(employee.DesignationNavigation?.DesignationName))
                {
                    var selectedDesignation = await _context.Designations.FirstOrDefaultAsync(d => d.DesignationName == employee.DesignationNavigation.DesignationName);
                    if (selectedDesignation != null)
                    {
                        employee.Designation = selectedDesignation.DesignationId;
                    }
                }



                string xmlData = $"<Employee operation='UPDATE' ID='{id}'><FirstName>{employee.FirstName}</FirstName><LastName>{employee.LastName}</LastName><Gender>{employee.Gender}</Gender><Age>{employee.Age}</Age><Designation>{employee.Designation}</Designation><Salary>{employee.Salary}</Salary></Employee>";
                var xmlParameter = new SqlParameter("@employeeData", SqlDbType.Xml)
                {
                    Value = xmlData
                };
                await _context.Database.ExecuteSqlRawAsync("EXEC sp_Employee @employeeData", xmlParameter);
                return RedirectToAction(nameof(Index));
            }
            return View(employee);
        }




        // GET: Employees/Delete/5 
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            string xmlData = $"<Employee operation='SELECTBYID' ID='{id}'></Employee>"; // XML data for the stored procedure 
            var employee = _context.Employees.FromSqlRaw("EXEC sp_Employee @employeeData", new SqlParameter("@employeeData", xmlData)).AsEnumerable().FirstOrDefault();
            if (employee == null)
            {
                return NotFound();
            }
            ViewData["Message"] = "Are you sure you want to delete this employee?";
            return View(employee);
        }



        // POST: Employees/Delete/5 
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)

        {
            string xmlData = $"<Employee operation='DELETE' ID='{id}'></Employee>"; // XML data for the stored procedure 
            _context.Database.ExecuteSqlRaw("EXEC sp_Employee @employeeData", new SqlParameter("@employeeData", xmlData));
            return RedirectToAction(nameof(Index));
        }
        private bool EmployeeExists(int id)
        {
            return (_context.Employees?.Any(e => e.EmpId == id)).GetValueOrDefault();
        }

        // GET: Employees/UploadExcel
        public async Task<IActionResult> UploadExcel()
        {
            return View();
        }

        //Employees/UploadExcel
        [HttpPost]
        public IActionResult UploadExcel(IFormFile file)
        {
            if (file == null || file.Length <= 0)
            {
                // Handle invalid file upload
                return RedirectToAction(nameof(Index), new { fileUploaded = false });
            }

            try
            {
                // Save the uploaded file to a specific location
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Excel", "file.xlsx");
                _logger.LogInformation("Processing filepath: {FilePath}", filePath);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                    _logger.LogInformation("Processing is uploaded");
                }

                using (var package = new ExcelPackage(new FileInfo(filePath)))
                {
                    var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                    _logger.LogInformation("Processing is fetched");

                    if (worksheet != null)
                    {
                        var rowCount = worksheet.Dimension.Rows;
                       
                        _logger.LogInformation("rowcount is :{rowCount}",rowCount);

                        for (int row = 2; row <= rowCount; row++) // Assuming the data starts from row 2 (header row excluded)
                        {
                            var firstName = worksheet.Cells[row, 1].Value?.ToString();
                            var lastName = worksheet.Cells[row, 2].Value?.ToString();
                            var ageString = worksheet.Cells[row, 3].GetValue<string>();

                            var salaryString = worksheet.Cells[row, 4].GetValue<string>();
                            var designationString = worksheet.Cells[row, 5].GetValue<string>();
                            var genderString = worksheet.Cells[row, 6].GetValue<string>();

                            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName))
                            {
                                // Skip rows with missing first name or last name
                                continue;
                            }

                        var employee = new Employee()
                            {
                                FirstName = firstName,
                                LastName = lastName,
                                Age = int.TryParse(ageString, out int age) ? age : (int?)null,
                                Salary = decimal.TryParse(salaryString, out decimal salary) ? salary : (decimal?)null,
                                Designation = int.TryParse(designationString, out int designation) ? designation : 0,
                                Gender = int.TryParse(genderString, out int gender) ? gender : 0,
                            };

                            // Save the employee to the database
                            _context.Employees.Add(employee);

                            _logger.LogInformation("{employee} is added",employee);
                        }

                        _context.Database.ExecuteSqlRaw("DISABLE TRIGGER ALL ON Employees");

                        _context.SaveChanges(); // Save changes to the database

                        _logger.LogInformation("changes are saved to database");
                        _context.Database.ExecuteSqlRaw("ENABLE TRIGGER ALL ON Employees");
                    }
                }

                return RedirectToAction(nameof(Index), new { fileUploaded = true });
            }
            catch (Exception ex)
            {
                // Log the exception for troubleshooting
                _logger.LogError("Exception occurred during file processing: {Exception}", ex.ToString());

                // Redirect to the Index action with fileUploaded set to false
                return RedirectToAction(nameof(Index), new { fileUploaded = false });
            }





        }

        // GET: Employees/ExportExcel
        public IActionResult ExportExcel()
        {
            var employees = _context.Employees.ToList();



            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Employees");
                worksheet.Cells.LoadFromCollection(employees, true);



                var stream = new MemoryStream(package.GetAsByteArray());



                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "file.xlsx");
            }
        }

     

    }
}


 