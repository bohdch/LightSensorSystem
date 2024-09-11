using System.Net;
using System.Text.Json;
using Serilog;

namespace LightSensorAPI.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IWebHostEnvironment _env;

        public ExceptionHandlingMiddleware(RequestDelegate next, IWebHostEnvironment env)
        {
            _next = next;
            _env = env;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                LogException(ex, httpContext);
                
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            HttpStatusCode statusCode = HttpStatusCode.BadRequest;
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var responseEntity = new
            {
                Code = statusCode,
                ErrorMessage = exception.Message,
                State = "Exception"
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(responseEntity));
        }

        private void LogException(Exception ex, HttpContext context)
        {
            var log = Log.ForContext<ExceptionHandlingMiddleware>();
            
            var enhancedStackTrace = ex.StackTrace.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
            string source = enhancedStackTrace.Split()[4].Split('.')[^2];

            if (_env.IsEnvironment("QA") && context.User.Identity.IsAuthenticated)
            {
                // Construct the log message including user info only in QA environment
                string clientInfo = $"{context.User.Identity.Name}";

                log.Error("\tExceptionType: {ExceptionType}\n\tMessage: {Message}\n\tSource: {Source}\n\tAuthenticated Client: {ClientInfo}\n\tStackTrace: {StackTrace}}",
                    ex.GetType(),
                    ex.Message,
                    source,
                    clientInfo,
                    enhancedStackTrace);
            }
            else
            {
                // Construct the log message without user info for non-QA environments
                log.Error("\tExceptionType: {ExceptionType}\n\tMessage: {Message}\n\tSource: {Source}\n\tStackTrace: {StackTrace}",
                    ex.GetType(),
                    ex.Message,
                    source,
                    enhancedStackTrace);
            }
        }
    }
}
