namespace WebApplication1.DTOs
{
    public class ProductResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? SKU { get; set; }
        public string? TechnicalSpecs { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }

        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty; // Lấy thêm tên Danh mục

        public int BrandId { get; set; }
        public string BrandName { get; set; } = string.Empty;    // Lấy thêm tên Thương hiệu
    }
}
