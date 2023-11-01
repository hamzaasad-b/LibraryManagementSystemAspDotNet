using System.Net;
using Api.Dto.Common;
using Domain.Common;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    public abstract class BaseController : ControllerBase
    {
        private const string DefaultSuccessMessage = "Success";
        private const string DefaultFailMessage = "Operation failed.";
        private const string DefaultNotFoundMessage = "No record found.";
        private const string DefaultBadRequest = "Bad Request.";

        protected BaseController(ILogger logger)
        {
            Logger = logger;
        }

        protected ILogger Logger { get; }

        protected ResponseDto Success(string? message = null)
        {
            return Success<object>(null, message);
        }

        protected ResponseDto Fail(string? message = null)
        {
            return Fail<object>(null, message);
        }

        protected ResponseDto SuccessOrFail(bool success, string? message = null)
        {
            return success
                ? Success<object>(null, message)
                : Fail<object>(null, message);
        }

        protected ResponseDto<T?> Success<T>(T? data, string? message = null)
        {
            return new ResponseDto<T?>(data, message ?? DefaultSuccessMessage, true);
        }

        protected ResponseDto<T?> Fail<T>(T? data, string? message = null)
        {
            return new ResponseDto<T?>(data, message ?? DefaultFailMessage, false);
        }

        protected ResponseDto<T?> NoResult<T>(string? message = null, string? error = null)
        {
            if (error is not null)
                Logger.LogError(error);

            return new ResponseDto<T?>(default, message ?? DefaultNotFoundMessage, true);
        }

        protected ResponseDto<T?> WithHttpStatus<T>(HttpStatusCode statusCode, T? data, bool? success,
            string? message = null,
            List<string>? errors = null) where T : class
        {
            HttpContext.Response.StatusCode = (int)statusCode;
            return new ResponseDto<T?>(data, message ?? DefaultFailMessage, success ?? false)
            {
                Errors = errors
            };
        }

        /// <summary>
        ///     Returns a Bad Request.
        ///     Use this when the Api input parameters are incorrect.
        /// </summary>
        protected ResponseDto<T?> BadRequest<T>(string? message = null, List<string>? errors = null)
        {
            HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return new ResponseDto<T?> { Errors = errors, Success = false, Message = message };
        }

        protected ResponseDto<T?> Unauthorized<T>(string? message = null) where T : class
        {
            HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            return new ResponseDto<T?>(null, message, false);
        }

        protected ResponseDto<T?> NotFound<T>(string? message = null, string? error = null)
        {
            if (error is not null)
                Logger.LogError(error);

            HttpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
            return new ResponseDto<T?>(default, message ?? DefaultNotFoundMessage, false);
        }

        protected ResponseDto<T?> Result<T>(ServiceResult<T?> result, bool notFoundOnEmpty = false)
        {
            if (notFoundOnEmpty && result.Data == null)
                return NotFound<T>();

            return result.Success
                ? Success(result.Data)
                : BadRequest<T>(DefaultBadRequest, result.Errors);
        }

        protected ResponseDto GetModelStateErrorResponse()
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            return ResponseDto.Failure("Validation failed.", errors);
        }
    }
}