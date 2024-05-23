using UniversityManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace UniversityManagement.Controllers
{
    public class DashboardController : Controller
    {

        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ActionResult> Index()
        {
            //Last 7 Days
            DateTime StartDate = DateTime.Today.AddDays(-6);
            DateTime EndDate = DateTime.Today;

            List<Person> SelectedPersons = await _context.Persons
                .Include(x => x.Category)
                .Where(y => y.BirthDate >= StartDate && y.BirthDate <= EndDate)
                .ToListAsync();

            //Total Students
            int nbrStudents = SelectedPersons
            .Count(i => i.Category.Type == "Students");
            ViewBag.nbrStudents = nbrStudents.ToString("C0");


            //Total Employee
            int nbrEmployees = SelectedPersons
                .Count(i => i.Category.Type == "Employee");
            ViewBag.nbrEmployees = nbrEmployees.ToString("C0");

            //Persons
            int Persons = nbrStudents + nbrEmployees;
            CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");
            culture.NumberFormat.CurrencyNegativePattern = 1;
            ViewBag.Persons = String.Format(culture, "{0:C0}", Persons);

            //Doughnut Chart - Employee By Category
            ViewBag.DoughnutChartData = SelectedPersons
                .Where(i => i.Category.Type == "Students") // Change to "Student" to filter students
                .GroupBy(j => j.Category.CategoryId)
                .Select(k => new
                {
                    categoryTitle = k.First().Category.PostOrClass,
                    count = k.Count(), // Count the number of students in each category
                    formattedCount = k.Count().ToString("N0"), // Format the count as a string with thousands separators
                })
                .OrderByDescending(l => l.count) // Order by count in descending order
                .ToList();


            //Spline Chart - Students vs Employee

            //Students
            List<SplineChartData> StudentsSummary = SelectedPersons
                .Where(i => i.Category.Type == "Students")
                .GroupBy(j => j.BirthDate)
                .Select(k => new SplineChartData()
                {
                    day = k.First().BirthDate.ToString("dd-MMM"),
                    Students = k.Count(),
                })
                .ToList();

            //Employee
            List<SplineChartData> EmployeeSummary = SelectedPersons
                .Where(i => i.Category.Type == "Employee")
                .GroupBy(j => j.BirthDate)
                .Select(k => new SplineChartData()
                {
                    day = k.First().BirthDate.ToString("dd-MMM"),
                    Employee = k.Count(),
                })
                .ToList();

            //Combine Students & Employee
            string[] Last7Days = Enumerable.Range(0, 7)
                .Select(i => StartDate.AddDays(i).ToString("dd-MMM"))
                .ToArray();

            ViewBag.SplineChartData = from day in Last7Days
                                      join Students in StudentsSummary on day equals Students.day into dayStudentsJoined
                                      from Students in dayStudentsJoined.DefaultIfEmpty()
                                      join Employee in EmployeeSummary on day equals Employee.day into EmployeeJoined
                                      from Employee in EmployeeJoined.DefaultIfEmpty()
                                      select new
                                      {
                                          day = day,
                                          Students = Students == null ? 0 : Students.Students,
                                          Employee = Employee == null ? 0 : Employee.Employee,
                                      };
            //Recent Persons
            ViewBag.RecentPersons = await _context.Persons
                .Include(i => i.Category)
                .OrderByDescending(j => j.BirthDate)
                .Take(5)
                .ToListAsync();


            return View();
        }
    }

    public class SplineChartData
    {
        public string day;
        public int Students;
        public int Employee;

    }
}
