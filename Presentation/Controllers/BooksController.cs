using Entities.DataTransferObjects;
using Entities.RequestFeatures;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Presentation.ActionFilters;
using Services.Contracts;
using System.Text.Json;

namespace Presentation.Controllers;
//[ApiVersion("1.0")]
[ApiExplorerSettings(GroupName = "v1")]
[ServiceFilter(typeof(LogFilterAttribute))]
[ApiController]
[Route("api/books")]
//[ResponseCache(CacheProfileName ="5mins")]
//[HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge = 80)]
public class BooksController : ControllerBase {
    private readonly IServiceManager manager;
    public BooksController(IServiceManager manager) {
        this.manager = manager;
    }

    [Authorize]
    [HttpHead]
    [HttpGet(Name = "GetAllBooksAsync")]
    [ServiceFilter(typeof(ValidateMediaTypeAttribute))]
    //[ResponseCache(Duration = 60)]
    public async Task<IActionResult> GetAllBooksAsync([FromQuery] BookParameters bookParameters) {
        LinkParameters linkParameters = new(bookParameters, this.HttpContext);

        (Entities.LinkModels.LinkResponse linkResponse, MetaData metaData) = await this.manager
            .BookService
            .GetAllBooksAsync(linkParameters, false);

        this.Response.Headers.Add("X-Pagination",
            JsonSerializer.Serialize(metaData));

        return linkResponse.HasLinks ?
            this.Ok(linkResponse.LinkedEntities) :
            this.Ok(linkResponse.ShapedEntities);
    }

    [Authorize]
    [HttpGet("{id:Guid}")]
    public async Task<IActionResult> GetOneBookAsync([FromRoute(Name = "id")] Guid id) {
        BookDto book = await this.manager.BookService.GetOneBookByIdAsync(id, false);

        return this.Ok(book);
    }

    [Authorize]
    [HttpGet("details")]
    public async Task<IActionResult> GetAllBooksWithDetailsAsync() {
        return this.Ok(await this.manager.BookService.GetAllBooksWithDetailsAsync(false));
    }

    [Authorize]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    [HttpPost(Name = "CreateOneBookAsync")]
    public async Task<IActionResult> CreateOneBookAsync([FromBody] BookDtoForInsertion bookDto) {
        BookDto book = await this.manager.BookService.CreateOneBookAsync(bookDto);
        return this.StatusCode(201, book); // CreatedAtRoute()
    }

    [Authorize(Roles = "Editor, Admin")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    [HttpPut("{id:Guid}")]
    public async Task<IActionResult> UpdateOneBookAsync([FromRoute(Name = "id")] Guid id, [FromBody] BookDtoForUpdate bookDto) {
        await this.manager.BookService.UpdateOneBookAsync(id, bookDto, false);
        return this.NoContent(); // 204
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:Guid}")]
    public async Task<IActionResult> DeleteOneBookAsync([FromRoute(Name = "id")] Guid id) {
        await this.manager.BookService.DeleteOneBookAsync(id, false);
        return this.NoContent();
    }

    [Authorize(Roles = "Editor, Admin")]
    [HttpPatch("{id:Guid}")]
    public async Task<IActionResult> PartiallyUpdateOneBookAsync([FromRoute(Name = "id")] Guid id,
        [FromBody] JsonPatchDocument<BookDtoForUpdate> bookPatch) {

        if(bookPatch is null)
            return this.BadRequest(); // 400

        (BookDtoForUpdate bookDtoForUpdate, Entities.Models.Book book) = await this.manager.BookService.GetOneBookForPatchAsync(id, false);

        bookPatch.ApplyTo(bookDtoForUpdate, (Microsoft.AspNetCore.JsonPatch.Adapters.IObjectAdapter)this.ModelState);

        this.TryValidateModel(bookDtoForUpdate);

        if(this.ModelState.IsValid is false)
            return this.UnprocessableEntity(this.ModelState);

        await this.manager.BookService.SaveChangesForPatchAsync(bookDtoForUpdate, book);

        return this.NoContent(); // 204
    }

    [Authorize]
    [HttpOptions]
    public IActionResult GetBooksOptions() {
        this.Response.Headers.Add("Allow", "GET, PUT, POST, PATCH, DELETE, HEAD, OPTIONS");
        return this.Ok();
    }
}