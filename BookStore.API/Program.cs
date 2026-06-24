using BookStore.Models.Data;
using Microsoft.EntityFrameworkCore;
using BookStore.Repositories;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Lấy chuỗi Connection String từ file appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// 2. Đăng ký DbContext sử dụng PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// Thêm cấu hình cho Controllers (để sau này nhận các file API Controller của mình)
builder.Services.AddControllers();

// Cấu hình giao diện Swagger/OpenAPI mặc định
builder.Services.AddOpenApi();

// Đăng ký Generic Repository vào hệ thống DI
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

var app = builder.Build();

// Cấu hình HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();

