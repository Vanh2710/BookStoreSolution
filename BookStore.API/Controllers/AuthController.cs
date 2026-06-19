using Microsoft.AspNetCore.Mvc;
using BookStore.Repositories;
using BookStore.Models.Entities;
using BookStore.API.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace BookStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IRepository<User> _userRepo;
        private readonly IConfiguration _config;

        // DI: Inject Repository của User và Cấu hình hệ thống (để lấy JWT Key)
        public AuthController(IRepository<User> userRepo, IConfiguration config)
        {
            _userRepo = userRepo;
            _config = config;
        }

        // 1. API ĐĂNG KÝ (SIGNUP)
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            // Kiểm tra xem Username đã tồn tại trong DB chưa
            var allUsers = await _userRepo.GetAllAsync();
            if (allUsers.Any(u => u.Username == dto.Username))
            {
                return BadRequest("Tên tài khoản này đã tồn tại rồi bạn ơi!");
            }

            // BĂM MẬT KHẨU bằng BCrypt trước khi lưu
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            // Tạo đối tượng User mới từ DTO
            var newUser = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = hashedPassword,
                Role = "User", // Mặc định đăng ký mới là quyền User thường
                CreatedAt = DateTime.UtcNow
            };

            // Lưu vào DB thông qua Repository
            await _userRepo.AddAsync(newUser);
            var success = await _userRepo.SaveChangesAsync();

            if (!success) return BadRequest("Có lỗi xảy ra khi tạo tài khoản!");

            return Ok("Đăng ký tài khoản thành công rực rỡ!");
        }

        // 2. API ĐĂNG NHẬP (LOGIN)
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            // Tìm User theo Username
            var allUsers = await _userRepo.GetAllAsync();
            var user = allUsers.FirstOrDefault(u => u.Username == dto.Username);

            // Nếu không tìm thấy hoặc mật khẩu băm không khớp
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            {
                return BadRequest("Tài khoản hoặc mật khẩu không chính xác!");
            }

            // Nếu đúng -> Tiến hành tạo JWT Token cấp cho User
            var token = CreateToken(user);

            return Ok(new { 
                Message = "Đăng nhập thành công!",
                Token = token 
            });
        }

        // HÀM PHỤ TRỢ: Sinh chuỗi Token JWT
        private string CreateToken(User user)
        {
            // 1. Định nghĩa các thông tin (Claims) sẽ bọc vào trong Token
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            // 2. Lấy chuỗi Secret Key từ file appsettings.json và mã hóa nó thành mảng byte
            var keyString = _config.GetSection("JwtSettings:Key").Value!;
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));

            // 3. Tạo chữ ký (Signature) bằng thuật toán HmacSha256
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // 4. Thiết lập các thông số cho Token (Người phát hành, người nhận, thời hạn hết hạn)
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(1), // Token có giá trị trong 1 ngày
                SigningCredentials = creds,
                Issuer = _config.GetSection("JwtSettings:Issuer").Value,
                Audience = _config.GetSection("JwtSettings:Audience").Value
            };

            // 5. Khởi tạo handler để vẽ và sinh ra chuỗi Token dạng string
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}