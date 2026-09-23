using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Brand
    {
        [Key]
        public int Id { get; set; }
        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        public string? LogoUrl { get; set; }
        public List<Product> Products { get; set; } = new List<Product>();
    }
}