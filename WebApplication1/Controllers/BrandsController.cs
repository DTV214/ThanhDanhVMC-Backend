using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.DTOs;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;

        public BrandsController(ApplicationDbContext context, IMapper mapper, IPhotoService photoService)
        {
            _context = context;
            _mapper = mapper;
            _photoService = photoService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BrandDto>>> GetBrands()
        {
            var brands = await _context.Brands.ToListAsync();
            return Ok(_mapper.Map<IEnumerable<BrandDto>>(brands));
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<BrandDto>> PostBrand([FromForm] BrandCreateDto brandDto)
        {
            var brand = _mapper.Map<Brand>(brandDto);

            if (brandDto.Image != null)
            {
                var result = await _photoService.AddPhotoAsync(brandDto.Image);
                if (result.Error == null && result.SecureUrl != null)
                {
                    brand.ImageUrl = result.SecureUrl.AbsoluteUri;
                }
            }

            _context.Brands.Add(brand);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetBrands), new { id = brand.Id }, _mapper.Map<BrandDto>(brand));
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBrand(int id, BrandDto brandDto)
        {
            if (id != brandDto.Id) return BadRequest("ID không hợp lệ");
            var brand = await _context.Brands.FindAsync(id);
            if (brand == null) return NotFound();

            _mapper.Map(brandDto, brand);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBrand(int id)
        {
            var brand = await _context.Brands.FindAsync(id);
            if (brand == null) return NotFound();

            _context.Brands.Remove(brand);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
