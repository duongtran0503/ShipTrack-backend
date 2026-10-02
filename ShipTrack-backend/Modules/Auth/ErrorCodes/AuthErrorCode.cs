using ShipTrack_backend.Common.Exceptions;

namespace ShipTrack_backend.Modules.Auth.ErrorCodes
{
    public class AuthErrorCode : IErrorCode
    {
        public static readonly AuthErrorCode LoginFail = new(400, "Đăng nhập thất bại", false);
        public static readonly AuthErrorCode TokenExpired = new(401, "Token hết hạn", false);
        public static readonly AuthErrorCode AccessDenied = new(403, "Không có quyền truy cập", false);
        public static readonly AuthErrorCode AccountLocked = new(423, "Tài khoản bị khóa", false);
        public static readonly AuthErrorCode AccountExisted = new(400, "Tài khoản đã tồn tại", false);
        public static readonly AuthErrorCode UserNotFound = new(400, "Tài khoản người dùng không tồn tại", false);

        public int StatusCode { get; }
        public string Message { get; }
        public bool Success { get; }

        private AuthErrorCode(int statusCode, string message, bool success)
        {
            StatusCode = statusCode;
            Message = message;
            Success = success;
        }
    }
}
