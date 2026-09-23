using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public List<Product> Products { get; set; } = new List<Product>();
    }
}