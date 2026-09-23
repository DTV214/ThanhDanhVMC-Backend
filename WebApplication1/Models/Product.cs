using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? SKU { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; } // Sẽ dùng lưu link từ Cloudinary

        // Khóa ngoại
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        public int BrandId { get; set; }
        public Brand Brand { get; set; } = null!;
    }
}