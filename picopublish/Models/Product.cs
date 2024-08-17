using System.ComponentModel.DataAnnotations;

namespace picopublish.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }
    }
}
