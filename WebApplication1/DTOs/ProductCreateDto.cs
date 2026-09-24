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
        public List<IFormFile>? Images { get; set; }
        public int StockQuantity { get; set; }
        public bool IsActive { get; set; } = true;

        [Required]
        public int CategoryId { get; set; }
        [Required]
        public int BrandId { get; set; }
    }
}
