using ShipTrack_backend.Common.Exceptions;
using ShipTrack_backend.Common.Response;
using System.Net;
using System.Text.Json;

namespace ShipTrack_backend.Common.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {

            try
            {

                await _next(context);

                // Chỉ ghi đè 401/403 do mặc định của ASP.NET Core sinh ra (khi chưa có Body)
                if (!context.Response.HasStarted
                      && (context.Response.StatusCode == 401 || context.Response.StatusCode == 403))
                {
                    _logger.LogWarning("Intercepting {StatusCode} for {Path}",
                        context.Response.StatusCode, context.Request.Path);

                    await RewriteAuthResponseAsync(context);
                }

            }


            catch (AppException ex)
            {
                _logger.LogWarning(ex.ErrorCode.Message, "Business exception occurred");
                await HandleAppExceptionAsync(context, ex);
                return;
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "Unhandled exception");
                await HandleUnhandledExceptionAsync(context, ex);
                return;
            }
        }

        private static Task HandleAppExceptionAsync(HttpContext context, AppException ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = ex.ErrorCode.StatusCode;

            var response = new ApiResponse<object>
            {
                Success = ex.ErrorCode.Success,
                Message = ex.Message,
                StatusCode = ex.ErrorCode.StatusCode,

            };

            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }

        private static Task HandleUnhandledExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var response = new ApiResponse<object>
            {
                Success = false,
                Message = "Lỗi server rồi kiểm tra server đi bản ê:))",
                StatusCode = 500
            };

            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }

        private async Task RewriteAuthResponseAsync(HttpContext context)
        {
            try
            {



                var response = context.Response.StatusCode switch
                {
                    401 => new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Người dùng chưa đăng nhập",
                        StatusCode = 401,

                    },
                    403 => new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Không có quyền truy cập",
                        StatusCode = 403,

                    },
                    _ => new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Lỗi xác thực",
                        StatusCode = context.Response.StatusCode,

                    }
                };

                context.Response.ContentType = "application/json";

                var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                await context.Response.WriteAsync(jsonResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to rewrite auth response");
            }
        }
    }
}
