using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ShipTrack_backend
{

    [ApiController]
    [Route("api")]
    public class IntroductionController : ControllerBase
    {
        // =========================================================================
        // DANH SÁCH ENDPOINTS & MODULES 
        // =========================================================================
        private static readonly List<ModuleEndpointGroup> SystemModules = new()
        {
            new ModuleEndpointGroup
            {
                ModuleName = "System Introduction",
                Description = "Cung cap thong tin tong quan va Health Check backend",
                Endpoints = new List<string>
                {
                    "GET /api (System Info & Health Check)"
                }
            },
            new ModuleEndpointGroup
            {
                ModuleName = "Auth Module",
                Description = "Quan ly xac thuc, dang nhap va phan quyen JWT",
                Endpoints = new List<string>
                {
                    "POST /api/auth/login (Dang phat trien)",
                    "POST /api/auth/register (Dang phat trien)",
                    "POST /api/auth/refresh-token (Dang phat trien)"
                }
            },
            new ModuleEndpointGroup
            {
                ModuleName = "User Module",
                Description = "Quan ly thong tin nguoi dung va khach hang",
                Endpoints = new List<string>
                {
                    "GET /api/users/profile (Dang phat trien)",
                    "PUT /api/users/profile (Dang phat trien)"
                }
            }
        };

        private readonly IWebHostEnvironment _env;
        private readonly IServiceProvider _serviceProvider;

        public IntroductionController(IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            _env = env;
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// API Health Check & System Info
        /// Endpoint: GET /api
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetSystemInfo()
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}";

            // Kiểm tra kết nối CSDL (nếu đã đăng ký DbContext)
            var dbStatus = new DatabaseStatusInfo();
            try
            {
                var dbContextType = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a => a.GetTypes())
                    .FirstOrDefault(t => typeof(DbContext).IsAssignableFrom(t) && !t.IsAbstract);

                if (dbContextType != null && _serviceProvider.GetService(dbContextType) is DbContext dbContext)
                {
                    bool canConnect = await dbContext.Database.CanConnectAsync();
                    dbStatus.IsConnected = canConnect;
                    dbStatus.Message = canConnect ? "Ket noi MySQL thanh cong!" : "Khong the ket noi toi MySQL Database.";
                }
                else
                {
                    dbStatus.IsConnected = false;
                    dbStatus.Message = "DbContext chua duoc dang ky trong AddDatabaseServices()";
                }
            }
            catch (Exception ex)
            {
                dbStatus.IsConnected = false;
                dbStatus.Message = $"Loi ket noi CSDL: {ex.Message}";
            }

            var response = new SystemInfoResponse
            {
                ServiceName = "ShipTrack Backend API System",
                Environment = _env.EnvironmentName,
                Status = "Success",
                StatusCode = 200,
                BaseUrl = baseUrl +"/api",
                ServerTimeUtc = DateTime.UtcNow,
                Version = "1.0.0",
                DatabaseStatus = dbStatus,
                Modules = SystemModules
            };

            return Ok(response);
        }
    }

    // ==========================================
    // DATA TRANSFER OBJECTS (DTOs)
    // ==========================================
    public class SystemInfoResponse
    {
        public string ServiceName { get; set; } = string.Empty;
        public string Environment { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int StatusCode { get; set; }
        public string BaseUrl { get; set; } = string.Empty;
        public DateTime ServerTimeUtc { get; set; }
        public string Version { get; set; } = string.Empty;
        public DatabaseStatusInfo DatabaseStatus { get; set; } = new();
        public List<ModuleEndpointGroup> Modules { get; set; } = new();
    }

    public class DatabaseStatusInfo
    {
        public string Provider { get; set; } = "MySQL (Pomelo EF Core)";
        public bool IsConnected { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class ModuleEndpointGroup
    {
        public string ModuleName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<string> Endpoints { get; set; } = new();
    }
}
