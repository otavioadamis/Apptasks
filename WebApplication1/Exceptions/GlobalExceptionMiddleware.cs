using System.Net;
using System.Text.Json;

namespace WebApplication1.Exceptions
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public GlobalExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            if (exception is UserFriendlyException userFriendlyException)
            {
                // Handle UserFriendlyException with custom error message.
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                var errorResponse = new ErrorResponse
                {
                    Message = userFriendlyException.Message
                };
                var json = JsonSerializer.Serialize(errorResponse);
                await context.Response.WriteAsync(json);
            }
            else
            {
                //return a generic error response.
                var errorResponse = new ErrorResponse
                {
                    Message = "Oops! Something went wrong!"
                };
                var json = JsonSerializer.Serialize(errorResponse);
                await context.Response.WriteAsync(json);
            }
        }

        public class ErrorResponse
        {
            public string Message { get; set; }
            //Todo, maybe add more details to the error, but avoiding passing informations that can break the security.
        }
    }
}
