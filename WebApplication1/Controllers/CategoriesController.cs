using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.DTOs;
using WebApplication1.Models;
using Microsoft.AspNetCore.Authorization;
using WebApplication1.Services;
namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;

        public CategoriesController(ApplicationDbContext context, IMapper mapper, IPhotoService photoService)
        {
            _context = context;
            _mapper = mapper;
            _photoService = photoService;
        }

        // 1. Lấy danh sách (GET)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories()
        {
            var categories = await _context.Categories.ToListAsync();
            return Ok(_mapper.Map<IEnumerable<CategoryDto>>(categories));
        }

        // 2. Thêm mới (POST)
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<CategoryDto>> PostCategory([FromForm] CategoryCreateDto categoryDto)
        {
            var category = _mapper.Map<Category>(categoryDto);

            if (categoryDto.Image != null)
            {
                var result = await _photoService.AddPhotoAsync(categoryDto.Image);
                if (result.Error == null && result.SecureUrl != null)
                {
                    category.ImageUrl = result.SecureUrl.AbsoluteUri;
                }
            }

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCategories), new { id = category.Id }, _mapper.Map<CategoryDto>(category));
        }

        // 3. Cập nhật (PUT)
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategory(int id, [FromForm] CategoryCreateDto categoryDto)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound();

            category.Name = categoryDto.Name;
            category.Description = categoryDto.Description;

            if (categoryDto.Image != null)
            {
                var result = await _photoService.AddPhotoAsync(categoryDto.Image);
                if (result.Error != null) return BadRequest(result.Error.Message);
                if (result.SecureUrl == null) return BadRequest("Không thể lấy đường dẫn ảnh sau khi tải lên.");

                category.ImageUrl = result.SecureUrl.AbsoluteUri;
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // 4. Xóa (DELETE)
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound();

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
