using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UniversityManagement.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        [Required(ErrorMessage = "required Field.")]
        public string PostOrClass { get; set; }

        
        [Column(TypeName = "nvarchar(10)")]
        public string Type { get; set; } = "Employee";

    }

    
}
