using Entities.LogModel;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Services.Contracts;

namespace Presentation.ActionFilters;
public class LogFilterAttribute : ActionFilterAttribute {
    private readonly ILoggerService logger;

    public LogFilterAttribute(ILoggerService logger) {
        this.logger = logger;
    }

    public override void OnActionExecuting(ActionExecutingContext context) {
        this.logger.LogInfo(this.Log("OnActionExecuting", context.RouteData));
    }

    private String Log(String modelName, RouteData routeData) {
        LogDetails logDetails = new() {
            ModelName = modelName,
            Controller = routeData.Values["controller"],
            Action = routeData.Values["Id"]
        };

        if(routeData.Values.Count >= 3)
            logDetails.Id = routeData.Values["Id"];

        return logDetails.ToString();
    }
}