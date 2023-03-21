using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net.Http.Headers;

namespace Presentation.ActionFilters;
public class ValidateMediaTypeAttribute : ActionFilterAttribute {
    public override void OnActionExecuting(ActionExecutingContext context) {
        Boolean acceptHeaderPresent = context.HttpContext.Request.Headers.ContainsKey("Accept");

        if(acceptHeaderPresent is false) {
            context.Result = new BadRequestObjectResult($"Accept header is missing!");
            return;
        }

        String? mediaType = context.HttpContext.Request.Headers["Accept"].FirstOrDefault();

        if(MediaTypeHeaderValue.TryParse(mediaType, out MediaTypeHeaderValue? outMediaType) is false) {
            context.Result = new BadRequestObjectResult($"Media type not present. Please add Accept header with required media type.");
            return;
        }

        context.HttpContext.Items.Add("AcceptHeaderMediaType", outMediaType);
    }
}