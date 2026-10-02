using ShipTrack_backend.Shared.Data;
using ShipTrack_backend.Shared.Entities;
using Microsoft.EntityFrameworkCore;
namespace ShipTrack_backend.Modules.Auth.Repositories
{
    public interface IAuthRepository
    {
        Task<User?> GetUserByEmailAsync(string email);
    }
    public class AuthRepository:IAuthRepository
    {
        private readonly ApplicationDbContext _context;

        public AuthRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
        }
    }
}
