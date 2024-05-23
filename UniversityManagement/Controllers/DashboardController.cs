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
            int nbrStudents = await _context.Persons
                 .Where(p => p.Category.Type == "Students")
                 .CountAsync();
            ViewBag.nbrStudents = nbrStudents.ToString("N0");
            //Total Employees
            int nbrEmployees = await _context.Persons
                .Where(p => p.Category.Type == "Empolyee")
                .CountAsync();
            ViewBag.nbrStudents = nbrStudents.ToString("N0");

            //Total Persons
            int Persons = nbrStudents + nbrEmployees;
            ViewBag.Persons = Persons;

            // Doughnut Chart - Students By Category
            var doughnutChartData = SelectedPersons
                .Where(i => i.Category.Type == "Students")
                .GroupBy(j => j.Category.CategoryId)
                .Select(k => new
                {
                    categoryTitle = k.First().Category.PostOrClass,
                    nbrStudents = k.Count(),
                })
                .OrderByDescending(l => l.nbrStudents)
                .ToList();
            ViewBag.DoughnutChartData = doughnutChartData;

            // Spline Chart - Students vs Employees
            var studentsSummary = SelectedPersons
                .Where(i => i.Category.Type == "Students")
                .GroupBy(j => j.BirthDate)
                .Select(k => new SplineChartData
                {
                    Day = k.First().BirthDate.ToString("dd-MMM"),
                    Students = k.Count(),
                })
                .ToList();

            var employeesSummary = SelectedPersons
                .Where(i => i.Category.Type == "Employees")
                .GroupBy(j => j.BirthDate)
                .Select(k => new SplineChartData
                {
                    Day = k.First().BirthDate.ToString("dd-MMM"),
                    Employee = k.Count(),
                })
                .ToList();

            var last7Days = Enumerable.Range(0, 7)
                .Select(i => StartDate.AddDays(i).ToString("dd-MMM"))
                .ToArray();

            var splineChartData = from day in last7Days
                                  join students in studentsSummary on day equals students.Day into dayStudentsJoined
                                  from students in dayStudentsJoined.DefaultIfEmpty()
                                  join employees in employeesSummary on day equals employees.Day into dayEmployeesJoined
                                  from employees in dayEmployeesJoined.DefaultIfEmpty()
                                  select new SplineChartData
                                  {
                                      Day = day,
                                      Students = students?.Students ?? 0,
                                      Employee = employees?.Employee ?? 0,
                                  };
            ViewBag.SplineChartData = splineChartData.ToList();

            // Recent Persons
            var recentPersons = await _context.Persons
                .Include(i => i.Category)
                .OrderByDescending(j => j.BirthDate)
                .Take(5)
                .ToListAsync();
            ViewBag.RecentPersons = recentPersons;

            return View();
        }
    }

    public class SplineChartData
    {
        public string Day { get; set; }
        public int Students { get; set; }
        public int Employee { get; set; }
    }
}
