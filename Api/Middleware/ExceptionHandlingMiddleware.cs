using System.Net;
using Api.Dto.Common;
using Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Api.Middleware
{
    public static class GenericApiErrorHandler
    {
        public static async Task HandleErrorAsync(HttpContext context, ILogger? logger, bool isDev)
        {
            var status = HttpStatusCode.InternalServerError;
            context.Response.StatusCode = (int)status;
            context.Response.ContentType = "application/json";

            var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
            if (contextFeature == null)
                return;

            var ex = contextFeature.Error;

            var response = new ResponseDto { Success = false };

            if (ex is ServiceException)
            {
                status = HttpStatusCode.BadRequest;
                response.Message = ex.Message;
            }
            else
            {
                response.Message = "Internal Server Error.";

                //response.TraceId = Sentry.SentrySdk.CaptureException(ex).ToString();

                if (isDev)
                {
                    AddExceptions(ex);

                    void AddExceptions(Exception? e)
                    {
                        while (true)
                        {
                            if (e is null) return;
                            response.Errors?.Add($"{e.Message}\n{e.StackTrace}\n");
                            e = e.InnerException;
                        }
                    }
                }
            }

            var json = JsonConvert.SerializeObject(response,
                new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });

            context.Response.StatusCode = (int)status;
            await context.Response.WriteAsync(json);
        }
    }
}