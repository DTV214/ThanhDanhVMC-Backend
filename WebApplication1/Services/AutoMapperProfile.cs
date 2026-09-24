using AutoMapper;
using WebApplication1.DTOs;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // Chuyển đổi 2 chiều giữa Model gốc và DTO
            CreateMap<Category, CategoryDto>().ReverseMap();
            CreateMap<Brand, BrandDto>().ReverseMap();

            CreateMap<CategoryCreateDto, Category>()
    .ForMember(dest => dest.ImageUrl, opt => opt.Ignore());

            CreateMap<BrandCreateDto, Brand>()
                .ForMember(dest => dest.ImageUrl, opt => opt.Ignore());
            // Cấu hình mapping cho Product
            CreateMap<ProductCreateDto, Product>();
            CreateMap<Product, ProductResponseDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.BrandName, opt => opt.MapFrom(src => src.Brand.Name));
        }
    }
}