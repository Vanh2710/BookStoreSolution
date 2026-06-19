using BookStore.Models.Data;
using Microsoft.EntityFrameworkCore;

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

var app = builder.Build();

// Cấu hình HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "BookStore API v1");
    });
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();

