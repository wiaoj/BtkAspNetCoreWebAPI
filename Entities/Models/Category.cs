namespace Entities.Models;
public class Category {
    public Guid Id { get; set; }
    public String? Name { get; set; }

    // Ref : Collection navigation property
    //public ICollection<Book> Books { get; set; }
}