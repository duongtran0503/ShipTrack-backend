using ShipTrack_backend.Modules.Auth.Repositories;
using ShipTrack_backend.Modules.Auth.Services;

namespace ShipTrack_backend.Modules.Auth
{
    public static class AuthModuleExtension
    {
        public static IServiceCollection AddAuthenticationModule(this IServiceCollection services)
        {
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IJwtProvider, JwtProvider>();
           services.AddScoped<IAuthService, AuthService>();

            return services;
        }
    }
}
