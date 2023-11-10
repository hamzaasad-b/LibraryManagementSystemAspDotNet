using System.Net;
using Api.Dto.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Results;

namespace Api.FluentValidations;

public class ApiResponseResultFactory : IFluentValidationAutoValidationResultFactory
{
    public IActionResult CreateActionResult(ActionExecutingContext context,
        ValidationProblemDetails? validationProblemDetails)

    {
        context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        if (validationProblemDetails is null) return new ObjectResult(new ResponseDto() { Success = false });
        var errorList = new List<string>();
        foreach (var error in validationProblemDetails.Errors.ToList())
        {
            errorList.Add($"{error.Key}: {string.Join(", ", error.Value)}");
        }

        return new ObjectResult(
            ResponseDto.Failure("Validation Error", errors: errorList)
        );
    }
}