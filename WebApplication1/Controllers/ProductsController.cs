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
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;
        public ProductsController(ApplicationDbContext context, IMapper mapper, IPhotoService photoService)
        {
            _context = context;
            _mapper = mapper;   
            _photoService = photoService;
        }

        // 1. Lấy danh sách Sản phẩm (Có Phân trang, Tìm kiếm, Lọc)
        [HttpGet]
        public async Task<ActionResult<PagedResult<ProductResponseDto>>> GetProducts(
            [FromQuery] string? search,
            [FromQuery] int? categoryId,
            [FromQuery] int? brandId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            // Bước 1: Khởi tạo câu truy vấn cơ bản (chưa chạy DB)
            var query = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .AsQueryable(); // Cho phép chắp vá thêm điều kiện trước khi thực thi

            // Bước 2: Áp dụng các bộ lọc nếu có
            if (!string.IsNullOrWhiteSpace(search))
            {
                // Tìm kiếm không phân biệt hoa thường theo Tên hoặc SKU
                query = query.Where(p => p.Name.ToLower().Contains(search.ToLower()) ||
                                         (p.SKU != null && p.SKU.ToLower().Contains(search.ToLower())));
            }

            if (categoryId.HasValue && categoryId > 0)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            if (brandId.HasValue && brandId > 0)
            {
                query = query.Where(p => p.BrandId == brandId.Value);
            }

            // Bước 3: Đếm tổng số lượng sản phẩm sau khi lọc (để tính tổng số trang)
            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            // Bước 4: Thực hiện phân trang và lấy dữ liệu thật
            var products = await query
                .OrderByDescending(p => p.Id) // Sản phẩm mới nhất xếp lên đầu
                .Skip((page - 1) * pageSize) // Bỏ qua các sản phẩm của trang trước
                .Take(pageSize)              // Lấy số lượng của trang hiện tại
                .ToListAsync();

            // Bước 5: Đóng gói vào DTO PagedResult
            var result = new PagedResult<ProductResponseDto>
            {
                Items = _mapper.Map<List<ProductResponseDto>>(products),
                TotalCount = totalCount,
                PageSize = pageSize,
                CurrentPage = page,
                TotalPages = totalPages
            };

            return Ok(result);
        }

        // 2. Thêm mới Sản phẩm có upload ảnh
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ProductResponseDto>> PostProduct([FromForm] ProductCreateDto productDto)
        {
            var categoryExists = await _context.Categories.AnyAsync(c => c.Id == productDto.CategoryId);
            var brandExists = await _context.Brands.AnyAsync(b => b.Id == productDto.BrandId);

            if (!categoryExists || !brandExists)
                return BadRequest("Danh mục hoặc Thương hiệu không tồn tại.");

            var product = _mapper.Map<Product>(productDto);

            // Xử lý upload ảnh lên Cloudinary nếu có file đính kèm
            if (productDto.ImageFile != null)
            {
                var uploadResult = await _photoService.AddPhotoAsync(productDto.ImageFile);
                if (uploadResult.Error != null) return BadRequest(uploadResult.Error.Message);

                // Lấy link ảnh an toàn từ Cloudinary lưu vào database
                product.ImageUrl = uploadResult.SecureUrl.AbsoluteUri;
            }

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            await _context.Entry(product).Reference(p => p.Category).LoadAsync();
            await _context.Entry(product).Reference(p => p.Brand).LoadAsync();

            return CreatedAtAction(nameof(GetProducts), new { id = product.Id }, _mapper.Map<ProductResponseDto>(product));
        }

        // 3. Cập nhật Sản phẩm (Khóa Auth)
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, ProductCreateDto productDto)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound("Không tìm thấy sản phẩm.");

            var categoryExists = await _context.Categories.AnyAsync(c => c.Id == productDto.CategoryId);
            var brandExists = await _context.Brands.AnyAsync(b => b.Id == productDto.BrandId);

            if (!categoryExists || !brandExists)
                return BadRequest("Danh mục hoặc Thương hiệu không hợp lệ.");

            _mapper.Map(productDto, product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // 4. Xóa Sản phẩm (Khóa Auth)
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }


    }
}