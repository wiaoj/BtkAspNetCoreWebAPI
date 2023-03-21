using Entities.LinkModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Presentation.Controllers;
[ApiController]
[Route("api")]
[ApiExplorerSettings(GroupName = "v1")]
public class RootController : ControllerBase {
    private readonly LinkGenerator linkGenerator;

    public RootController(LinkGenerator linkGenerator) {
        this.linkGenerator = linkGenerator;
    }

    [HttpGet(Name = "GetRoot")]
    public async Task<IActionResult> GetRoot([FromHeader(Name = "Accept")] String mediaType) {
        if(mediaType.Contains("application/vnd.btkakademi.apiroot")) {
            List<Link> list = new() {
                new(href: this.linkGenerator.GetUriByName(this.HttpContext, nameof(GetRoot), new{}),
                    rel: "_self",
                    method: "GET"),
                new(href: this.linkGenerator.GetUriByName(HttpContext, nameof(BooksController.GetAllBooksAsync), new{}),
                    rel: "books",
                    method: "GET"  ),
                new(href: this.linkGenerator.GetUriByName(HttpContext, nameof(BooksController.CreateOneBookAsync), new{}),
                     rel: "books",
                     method: "POST"),
            };
            return Ok(list);
        }
        return NoContent(); // 204
    }
}