using System.ComponentModel.DataAnnotations;

namespace WebApplication1.DTOs
{
    public class ProductCreateDto
    {
        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;
        public string? SKU { get; set; }
        public string? TechnicalSpecs { get; set; }
        public string? Description { get; set; }
        // Thay đổi quan trọng: Nhận file ảnh thay vì đường link string
        public IFormFile? ImageFile { get; set; }

        [Required]
        public int CategoryId { get; set; }
        [Required]
        public int BrandId { get; set; }
    }
}
