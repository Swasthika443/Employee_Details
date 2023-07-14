namespace Employee_Details.Models.ViewModel
{
    public class EmployeeView
    {

            public IQueryable<Employee> Employees { get; set; }


            public int PageSize { get; set; }
            public int CurrentPage { get; set; }
            public int TotalPages { get; set; }
          

    }
    }



