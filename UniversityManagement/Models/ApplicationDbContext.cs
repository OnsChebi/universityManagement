using Microsoft.EntityFrameworkCore;

namespace UniversityManagement.Models
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions options):base(options)
        {}

        public DbSet<Person> Persons { get; set; }
        public DbSet<Category> Categories { get; set; }
    }
}
