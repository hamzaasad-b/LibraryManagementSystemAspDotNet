namespace Api.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                // Handle the exception and generate an appropriate response.
                // You can log the exception, customize the error response, etc.
                context.Response.StatusCode = 500; // Internal Server Error
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsync($"An error occurred: {ex.Message}");
            }
        }


    }
}
