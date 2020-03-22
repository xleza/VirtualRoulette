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
                    context.Result = new JsonResult(badRequestException.Message);
                    break;
                case NotFoundException notFoundException:
                    context.HttpContext.Response.ContentType = "application/json";
                    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    context.Result = new JsonResult(notFoundException.Message);
                    break;
            }
        }
    }
}
