using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShipTrack_backend.Common.Response;
using ShipTrack_backend.Modules.Auth.Dtos.Requests;
using ShipTrack_backend.Modules.Auth.Services;

namespace ShipTrack_backend.Modules.Auth.Controllers
{
    [ApiController]
    [Route("/api/auth")]
    public class AuthController:ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// API Đăng nhập hệ thống
        /// </summary>
         [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse.ErrorInput(ModelState));
            }
            var result = await _authService.LoginAsync(request);
            return Ok(result);
        }
    }
}
