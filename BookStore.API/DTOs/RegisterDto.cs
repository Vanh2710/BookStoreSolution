using System.ComponentModel.DataAnnotations;

namespace BookStore.API.DTOs
{
    public class RegisterDto
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6, ErrorMessage = "Mật khẩu phải từ 6 ký tự trở lên!")]
        public string Password { get; set; } = string.Empty;
    }
}