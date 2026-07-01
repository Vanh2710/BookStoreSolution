using BookStore.Models.Data;
using Microsoft.EntityFrameworkCore;
using BookStore.Repositories;
using Scalar.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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

var jwtKey = builder.Configuration.GetSection("JwtSettings:Key").Value!;
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration.GetSection("JwtSettings:Issuer").Value,
        ValidateAudience = true,
        ValidAudience = builder.Configuration.GetSection("JwtSettings:Audience").Value,
        ValidateLifetime = true, 
        ClockSkew = TimeSpan.Zero 
    };
});

var app = builder.Build();

// Cấu hình HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

