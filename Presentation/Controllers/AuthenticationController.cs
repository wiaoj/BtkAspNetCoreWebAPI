using Entities.DataTransferObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Presentation.ActionFilters;
using Services.Contracts;

namespace Presentation.Controllers;
[ApiController]
[Route("api/authentication")]
[ApiExplorerSettings(GroupName = "v1")]
public class AuthenticationController : ControllerBase {
    private readonly IServiceManager service;
    public AuthenticationController(IServiceManager service) {
        this.service = service;
    }

    [HttpPost]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> RegisterUser([FromBody] UserForRegistrationDto userForRegistrationDto) {
        Microsoft.AspNetCore.Identity.IdentityResult result =
            await this.service.AuthenticationService.RegisterUser(userForRegistrationDto);

        if(result.Succeeded is false) {
            foreach(Microsoft.AspNetCore.Identity.IdentityError error in result.Errors)
                this.ModelState.TryAddModelError(error.Code, error.Description);

            return this.BadRequest(this.ModelState);
        }

        return this.StatusCode(StatusCodes.Status201Created);
    }

    [HttpPost("login")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> Authenticate([FromBody] UserForAuthenticationDto user) {
        if(await this.service.AuthenticationService.ValidateUser(user) is false)
            return this.Unauthorized(); // 401

        TokenDto response = await this.service.AuthenticationService.CreateToken(populateExp: true);

        return this.Ok(response);
    }

    [HttpPost("refresh")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> Refresh([FromBody] TokenDto tokenDto) {
        TokenDto response = await this.service.AuthenticationService.RefreshToken(tokenDto);
        return this.Ok(response);
    }
}