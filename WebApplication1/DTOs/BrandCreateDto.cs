using Microsoft.AspNetCore.Http;

namespace WebApplication1.DTOs
{
    public class BrandCreateDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public IFormFile? Image { get; set; }
    }
}
