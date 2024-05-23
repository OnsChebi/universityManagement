using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UniversityManagement.Models
{
    public class Person
    {
        [Key]
        public int PersonId { get; set; }

        [Range(1,int.MaxValue,ErrorMessage ="Please select a category.")]
        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        [Column(TypeName = "nvarchar(75)")]
        public string FullName { get; set; }

        [Column(TypeName = "nvarchar(75)")]
        public string? Comment { get; set; }

        public DateTime BirthDate { get; set; }

        


    }
}
