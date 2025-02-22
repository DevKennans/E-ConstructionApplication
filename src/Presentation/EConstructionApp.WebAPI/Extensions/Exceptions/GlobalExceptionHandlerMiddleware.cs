using EConstructionApp.WebAPI.Extensions.Exceptions.Helpers;
using FluentValidation;
using System.Net;
using System.Net.Mime;

namespace EConstructionApp.WebAPI.Extensions.Exceptions
{
    public class GlobalExceptionHandlerMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext httpContext, RequestDelegate next)
        {
            try
            {
                await next(httpContext);
            }
            catch (Exception exception)
            {
                await HandleExceptionAsync(httpContext, exception);
            }
        }

        private static Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
        {
            httpContext.Response.ContentType = MediaTypeNames.Application.Json;

            int statusCode = (int)HttpStatusCode.InternalServerError;
            httpContext.Response.StatusCode = statusCode;

            if (exception.GetType() == typeof(ValidationException))
                return httpContext.Response.WriteAsync(new ExceptionResponseModel()
                {
                    Errors = ((ValidationException)exception).Errors.Select(error => error.ErrorMessage),
                    StatusCode = (int)HttpStatusCode.UnprocessableEntity
                }.ToString());

            List<string> errors = new List<string>
            {
                $"Error message: {exception.Message}"
            };

            return httpContext.Response.WriteAsync(new ExceptionResponseModel()
            {
                StatusCode = statusCode,
                Errors = errors
            }.ToString());
        }
    }
}
