namespace EConstructionApp.WebAPI.Extensions.Exceptions
{
    public static class ConfigureGlobalExceptionHandlerMiddleware
    {
        public static void UseGlobalExceptionHandlerMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
        }
    }
}
