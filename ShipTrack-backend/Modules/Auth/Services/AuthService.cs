using ShipTrack_backend.Common.Exceptions;
using ShipTrack_backend.Modules.Auth.Dtos.Requests;
using ShipTrack_backend.Modules.Auth.Dtos.Response;
using ShipTrack_backend.Modules.Auth.ErrorCodes;
using ShipTrack_backend.Modules.Auth.Repositories;
using ShipTrack_backend.Shared.Enums;

namespace ShipTrack_backend.Modules.Auth.Services
{
    public interface IAuthService
    {
        Task<AuthResponse> LoginAsync(LoginRequest request);
    }
    public class AuthService:IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IJwtProvider _jwtProvider;

        public AuthService(IAuthRepository authRepository, IJwtProvider jwtProvider)
        {
            _authRepository = authRepository;
            _jwtProvider = jwtProvider;
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            // 1. Kiểm tra sự tồn tại của User
            var user = await _authRepository.GetUserByEmailAsync(request.Email);
            if (user == null)
            {
                throw new AppException(AuthErrorCode.UserNotFound);
            }

            // 2. Kiểm tra tài khoản bị khóa
            if (!user.status.ToString().Equals(AccountStatus.ACTIVE.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                throw new AppException(AuthErrorCode.AccountLocked);
            }

            // 3. Khai báo xác thực mật khẩu qua BCrypt
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
            if (!isPasswordValid)
            {
                throw new AppException(AuthErrorCode.LoginFail);
            }

            // 4. Sinh AccessToken và RefreshToken
            var accessToken = _jwtProvider.GenerateAccessToken(user);
            var refreshToken = _jwtProvider.GenerateRefreshToken();

            return new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                User = new UserSummaryResponse
                {
                    Id = user.Id,
                    Email = user.Email,
                    FullName = user.FullName,
                    Role = user.Role.ToString()
                }
            };
        }
    }
}
