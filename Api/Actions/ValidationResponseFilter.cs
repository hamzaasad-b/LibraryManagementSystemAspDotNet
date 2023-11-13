using System.Net;
using Api.Dto.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Api.Actions;

public class ValidationResponseFilter : IActionFilter
{
    public async void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.ModelState.IsValid) return;
        context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;

        var errors = new List<string>();
        foreach (var error in context.ModelState.ToList())
        {
            errors.Add($"{error.Key}: {error.Value}");
        }

        context.Result = new ObjectResult(
            ResponseDto.Failure("Validation Error", errors)
        );
        // var json = JsonConvert.SerializeObject(
        //     ResponseDto.Failure("Validation Error", errors),
        //     new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
        // await context.HttpContext.Response.WriteAsync(json);
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
    }
}