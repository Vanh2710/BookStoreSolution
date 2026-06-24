using Microsoft.AspNetCore.Mvc;
using BookStore.Repositories;
using BookStore.Models.Entities;
using BookStore.API.DTOs;

namespace BookStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly IRepository<Book> _bookRepo;

        // DI: Inject Generic Repository dành riêng cho Entity Book
        public BookController(IRepository<Book> bookRepo)
        {
            _bookRepo = bookRepo;
        }

        // 1. GET: api/books (Lấy toàn bộ danh sách sách)
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var books = await _bookRepo.GetAllAsync();
            return Ok(books);
        }

        // 2. GET: api/books/{id} (Lấy chi tiết 1 cuốn sách theo ID)
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var book = await _bookRepo.GetByIdAysnc(id);
            if (book == null) return NotFound("Không tìm thấy cuốn sách bạn yêu cầu!");
            return Ok(book);
        }

        // 3. POST: api/books (Thêm mới 1 cuốn sách)
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BookDto dto)
        {
            var newBook = new Book
            {
                Title = dto.Title,
                Author = dto.Author,
                Price = dto.Price,
                Description = dto.Description,
                CreatedAt = DateTime.UtcNow
            };

            await _bookRepo.AddAsync(newBook);
            await _bookRepo.SaveChangesAsync();

            return Ok(new { Message = "Thêm sách thành công rồi nhé!", Data = newBook });
        }

        // 4. PUT: api/books/{id} (Cập nhật thông tin sách)
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] BookDto dto)
        {
            var book = await _bookRepo.GetByIdAysnc(id);
            if (book == null) return NotFound("Sách định sửa không tồn tại!");

            // Cập nhật các trường dữ liệu mới từ DTO sang Entity
            book.Title = dto.Title;
            book.Author = dto.Author;
            book.Price = dto.Price;
            book.Description = dto.Description;

            _bookRepo.Update(book);
            await _bookRepo.SaveChangesAsync();

            return Ok("Cập nhật thông tin sách thành công!");
        }

        // 5. DELETE: api/books/{id} (Xóa sách)
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var book = await _bookRepo.GetByIdAysnc(id);
            if (book == null) return NotFound("Sách định xóa không tồn tại!");

            _bookRepo.Delete(book);
            await _bookRepo.SaveChangesAsync();

            return Ok("Đã xóa cuốn sách này khỏi hệ thống!");
        }
    }
}