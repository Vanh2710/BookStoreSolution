using System;

namespace BookStore.Models.Entities
{
  public class Book
  {
    public int Id {get; set;}
    public string Title {get; set;} = string.Empty;
    public string Author { get; set; } = string.Empty;
    public decimal Price {get; set;} = 0;
    public string? Description {get; set;} = string.Empty;
    public DateTime CreatedAt {get; set;} = DateTime.UtcNow;
  }
}