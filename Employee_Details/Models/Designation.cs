using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Employee_Details.Models;

public partial class Designation
{
    [Key]
    public int DesignationId { get; set; }

    public string? DesignationName { get; set; } 

    

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
