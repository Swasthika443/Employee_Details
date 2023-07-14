using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Employee_Details.Models;

public partial class Employee
{
    [Key]
    public int EmpId { get; set; }


    [Required(ErrorMessage = "FirstName is required.")]
    [StringLength(50, ErrorMessage = "First Name cannot exceed 50 characters.")]
    public string? FirstName { get; set; }
    [Required(ErrorMessage = "LastName is required.")]
    [StringLength(50, ErrorMessage = "Last Name cannot exceed 50 characters.")]
    public string? LastName { get; set; }


    [Required(ErrorMessage = "Age is required.")]
    [Range(18, int.MaxValue, ErrorMessage = "Age must be 18 or above.")]
    public int? Age { get; set; }

    [Required(ErrorMessage = "Salary  is required.")]
    public decimal? Salary { get; set; }


    public int? Designation { get; set; }

    public int? Gender { get; set; }



    public virtual Designation? DesignationNavigation { get; set; }

    public virtual Gender? GenderNavigation { get; set; }


}
