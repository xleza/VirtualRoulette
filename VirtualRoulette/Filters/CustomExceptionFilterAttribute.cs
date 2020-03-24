using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using VirtualRoulette.Exceptions;

namespace VirtualRoulette.Filters
{
    public sealed class CustomExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            switch (context.Exception)
            {
                case BadRequestException badRequestException:
                    context.HttpContext.Response.ContentType = "application/json";
                    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    context.Result = new JsonResult(ApiError.From(badRequestException));
                    break;
                case NotFoundException notFoundException:
                    context.HttpContext.Response.ContentType = "application/json";
                    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    context.Result = new JsonResult(ApiError.From(notFoundException));
                    break;
                case ConcurrencyException concurrencyException:
                    context.HttpContext.Response.ContentType = "application/json";
                    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Conflict;
                    context.Result = new JsonResult(ApiError.From(concurrencyException));
                    break;
            }
        }

        public class ApiError
        {
            public int Code { get; private set; }
            public string Message { get; private set; }

            public static ApiError From(BadRequestException exception)
                => new ApiError
                {
                    Code = (int)HttpStatusCode.BadRequest,
                    Message = exception.Message
                };

            public static ApiError From(NotFoundException exception)
                => new ApiError
                {
                    Code = (int)HttpStatusCode.NotFound,
                    Message = exception.Message
                };

            public static ApiError From(ConcurrencyException exception)
                => new ApiError
                {
                    Code = (int)HttpStatusCode.Conflict,
                    Message = exception.Message
                };
        }
    }
}
