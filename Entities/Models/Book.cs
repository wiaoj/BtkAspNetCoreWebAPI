namespace Entities.Models;
public class Book {
    public Guid Id { get; set; }
    public String Title { get; set; }
    public Decimal Price { get; set; }

    // Ref: navigation property
    public Guid CategoryId { get; set; }
    public Category Category { get; set; }
}