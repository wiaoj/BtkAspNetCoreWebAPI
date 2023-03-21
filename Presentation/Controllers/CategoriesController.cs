using Microsoft.AspNetCore.Mvc;
using Services.Contracts;

namespace Presentation.Controllers;
[ApiController]
[Route("api/categories")]
public class CategoriesController : ControllerBase {
    private readonly IServiceManager services;

    public CategoriesController(IServiceManager services) {
        this.services = services;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllCategoriesAsync() {
        return this.Ok(await this.services.CategoryService.GetAllCategoriesAsync(false));
    }

    [HttpGet("{id:Guid}")]
    public async Task<IActionResult> GetAllCategoriesAsync([FromRoute] Guid id) {
        return this.Ok(await this.services.CategoryService.GetOneCategoryByIdAsync(id, false));
    }
}