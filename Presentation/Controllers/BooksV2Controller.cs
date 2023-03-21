using Microsoft.AspNetCore.Mvc;
using Services.Contracts;

namespace Presentation.Controllers;
//[ApiVersion("2.0", Deprecated = true)]
[ApiController]
[Route("api/books")]
[ApiExplorerSettings(GroupName = "v2")]
public class BooksV2Controller : ControllerBase {
    private readonly IServiceManager manager;

    public BooksV2Controller(IServiceManager manager) {
        this.manager = manager;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllBooksAsync() {
        List<Entities.Models.Book> books = await this.manager.BookService.GetAllBooksAsync(false);
        var booksV2 = books.Select(book => new {
            Title = book.Title,
            Id = book.Id
        });
        return this.Ok(booksV2);
    }
}