using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Đăng ký PhotoService
builder.Services.AddScoped<IPhotoService, PhotoService>();
// Đăng ký PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
// Đăng ký AutoMapper
// Đăng ký AutoMapper
builder.Services.AddAutoMapper(config =>
{
    config.AddProfile<AutoMapperProfile>();
});
// Đăng ký hệ thống xác thực JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

// Đăng ký chính sách CORS cho Next.js
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowNextJsApp", policy =>
    {
        policy.WithOrigins(
                "http://localhost:3000",          // Dành cho lúc code Frontend ở máy tính (Local)
                "https://cty-thanhdanh-vmc.vercel.app/" // Dành cho lúc đưa Next.js lên Vercel (Production)
                                                  // (Bạn có thể thêm các domain khác vào đây sau này nếu có)
            )
            .AllowAnyHeader() // Cho phép Next.js gửi Token lên
            .AllowAnyMethod() // Cho phép gọi GET, POST, PUT, DELETE
            .AllowCredentials(); // Cho phép gửi cookie/thông tin xác thực nếu cần
    });
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ThanhDanhVMC API", Version = "v1" });

    // Cấu hình nút Authorize trên Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Nhập 'Bearer [khoảng trắng] {token của bạn}'. Ví dụ: Bearer eyJhbGci...",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
// KÍCH HOẠT CORS: Bắt buộc phải nằm ở đây (Sau HttpsRedirection và Trước Authentication)
app.UseCors("AllowNextJsApp");
app.UseAuthentication(); // <-- Thêm dòng này để kiểm tra Token (Ai đang gọi?)
app.UseAuthorization();  // <-- Dòng có sẵn (Người này có quyền làm gì?)

app.UseAuthorization();

app.MapControllers();

app.Run();
