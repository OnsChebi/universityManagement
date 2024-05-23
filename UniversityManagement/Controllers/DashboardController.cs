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
