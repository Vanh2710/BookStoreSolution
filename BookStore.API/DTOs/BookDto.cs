using System.ComponentModel.DataAnnotations;

namespace BookStore.API.DTOs
{
    public class BookDto
    {
        [Required(ErrorMessage = "Tên sách không được để trống")]
        public string Title { get; set; } = string.Empty;

        public string Author { get; set; } = string.Empty;

        [Range(0, double.MaxValue, ErrorMessage = "Giá sách không được là số âm")]
        public decimal Price { get; set; }

        public string Description { get; set; } = string.Empty;
    }
}