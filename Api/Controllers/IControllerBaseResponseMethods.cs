using Api.Dto.Response;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Api.Controllers;

public interface IControllerBaseResponseMethods
{
    IApiResponse CreatedResponse();
    IApiResponse NoContentResponse();
    IApiResponse OkResponse();
    IApiResponse AcceptedResponse();
    IApiResponse BadRequestResponse();
    IApiResponse ConflictResponse();
    IApiResponse ForbidResponse();
    IApiResponse NotFoundResponse();
    IApiResponse UnauthorizedResponse();
    IApiResponse StatusCodeResponse<T>(int statusCode, T? data);
    IApiResponse UnprocessableEntityResponse();
}