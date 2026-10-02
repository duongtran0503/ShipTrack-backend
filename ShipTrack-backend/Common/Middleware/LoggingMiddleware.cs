using System.Diagnostics;

namespace ShipTrack_backend.Common.Middleware
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LoggingMiddleware> _logger;


        public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            var request = context.Request;


            try
            {
                await _next(context);
                stopwatch.Stop();

                // Ghi log response thành công
                _logger.LogInformation("Request Success:{Time}: {Method} {Path} - {StatusCode} - {ElapsedMs}ms",
                    DateTime.Now.ToString(), request.Method, request.Path, context.Response.StatusCode, stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();

                // Ghi log lỗi
                _logger.LogError(ex, "❌ API Error: {Method} {Path} - {StatusCode} - {ElapsedMs}ms - Error: {ErrorMessage}",
                    request.Method, request.Path, context.Response.StatusCode, stopwatch.ElapsedMilliseconds, ex.Message);

                throw;
            }
        }

    }
}
