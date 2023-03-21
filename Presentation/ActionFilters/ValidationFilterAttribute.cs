using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Presentation.ActionFilters;
public class ValidationFilterAttribute : ActionFilterAttribute {
    public override void OnActionExecuting(ActionExecutingContext context) {
        Object controller = context.RouteData.Values["controller"];
        Object action = context.RouteData.Values["action"];

        // Dto
        Object? param = context.ActionArguments.SingleOrDefault(predicate => predicate.Value.ToString().Contains("Dto")).Value;

        if(param is null) {
            context.Result = new BadRequestObjectResult($"Object is null. Controller : {controller} Action : {action}");
            return; // 400
        }

        if(context.ModelState.IsValid is false)
            context.Result = new UnprocessableEntityObjectResult(context.ModelState); // 422 
    }
}