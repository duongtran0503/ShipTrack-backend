using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ShipTrack_backend.Common.Middleware;
using ShipTrack_backend.Modules.Auth;
using ShipTrack_backend.Shared.Data;
using ShipTrack_backend.Shared.Entities;
using ShipTrack_backend.Shared.Enums;
using System.Globalization;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace ShipTrack_backend
{
    /// <summary>
    /// Lớp Bootstrap chứa các extension methods dùng để cấu hình Service (DI Container) 
    /// và Middleware Pipeline cho ứng dụng, giúp file Program.cs ngắn gọn và sạch đẹp.
    /// </summary>
    public static class Bootstrap
    {
        // Tên Policy cho phép truy cập tài nguyên chéo nguồn (CORS)
        public static readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

        /// <summary>
        /// Tổng hợp toàn bộ các dịch vụ (Services) cần đăng ký cho ứng dụng.
        /// Được gọi tại Program.cs thông qua builder.ConfigureServices()
        /// </summary>
        public static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder builder)
        {

            builder.AddMvcServices()
               .AddSwaggerServices()
               .AddDatabaseServices()
               .AddAuthenticationServices()
               .AddAuthorizationServices()
               .AddHttpContextServices()
               .AddApplicationModules()
               .AddCors();


            return builder;
        }

        /// <summary>
        /// Đăng ký các Module tính năng nghiệp vụ trong hệ thống (Auth, User, Product, v.v.).
        /// Mỗi module sẽ tự quản lý Service và Repository riêng thông qua Extension Method.
        /// </summary>
        public static WebApplicationBuilder AddApplicationModules(this WebApplicationBuilder builder)
        {
            // Đăng ký Module Auth (bao gồm AuthService, AuthRepository...)
            builder.Services.AddAuthenticationModule();

            return builder;
        }

        /// <summary>
        /// Cấu hình các dịch vụ MVC/Controllers, tùy chỉnh Validate dữ liệu và Format JSON trả về API.
        /// </summary>
        public static WebApplicationBuilder AddMvcServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddControllers()
                .ConfigureApiBehaviorOptions(opt =>
                {
                    // Tắt bộ lọc ModelState mặc định của ASP.NET Core để tùy biến Custom Response lỗi
                    opt.SuppressModelStateInvalidFilter = true;
                })
                .AddJsonOptions(opt =>
                {

                    //GIỮ NGUYÊN TIẾNG VIỆT CÓ DẤU TRONG JSON (không bị chuyển thành \u00xx)
                    opt.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
                    // Thêm bộ chuyển đổi thời gian sang chuẩn UTC ISO-8601
                    opt.JsonSerializerOptions.Converters.Add(new UtcDateTimeConverter());
                    // Format chuỗi JSON đẹp mắt (có thụt lề, xuống dòng) khi trả về
                    opt.JsonSerializerOptions.WriteIndented = true;
                    // Bỏ qua các thuộc tính có giá trị null, không hiển thị trong JSON response
                    opt.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                });

            return builder;
        }

        /// <summary>
        /// Cấu hình Swagger/OpenAPI giúp tạo giao diện tài liệu thử nghiệm API trực tiếp trên trình duyệt.
        /// </summary>
        public static WebApplicationBuilder AddSwaggerServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(); // Đã bật để xem giao diện Swagger UI khi chạy test API

            return builder;
        }

        /// <summary>
        /// Cấu hình kết nối Cơ sở dữ liệu (MySQL Server qua Pomelo Entity Framework Core).
        /// Tạm thời để trống/comment nếu bạn chưa khởi tạo file ApplicationDbContext.
        /// </summary>
        public static WebApplicationBuilder AddDatabaseServices(this WebApplicationBuilder builder)
        {
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMySql(
                    connectionString,
                    ServerVersion.AutoDetect(connectionString), // Tự động phát hiện phiên bản MySQL Server
                    mySqlOptions =>
                    {
                        mySqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 5,
                            maxRetryDelay: TimeSpan.FromSeconds(30),
                            errorNumbersToAdd: null);
                        mySqlOptions.CommandTimeout(30);
                    }
                ));

            return builder;

        }

        /// <summary>
        /// Cấu hình cơ chế Xác thực người dùng (Authentication) bằng JWT Bearer Token.
        /// Kiểm tra tính hợp lệ của Token gửi lên trong Header 'Authorization: Bearer <token>'
        /// </summary>
        public static WebApplicationBuilder AddAuthenticationServices(this WebApplicationBuilder builder)
        {
            var jwtSettings = builder.Configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["Secret"] ?? "Chuoi_Secret_Key_Tam_Thoi_Mac_Dinh_32_Ky_Tu!";

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                    ClockSkew = TimeSpan.Zero, // Không cho phép độ trễ thời gian hết hạn của token
                    NameClaimType = "name",
                    RoleClaimType = "role"
                };
            });

            builder.Services.AddAuthorization(options =>
            {
                // Mọi endpoint KHÔNG có [Authorize]/[AllowAnonymous] 
                // đều phải đăng nhập
                options.FallbackPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();

                // Các policy cho từng role
                options.AddPolicy("AdminOnly", p => p.RequireRole("Admin"));
                options.AddPolicy("ManagerOrAbove", p => p.RequireRole("Admin", "Manager"));
            });

            return builder;
        }

        /// <summary>
        /// Cấu hình Phân quyền người dùng (Authorization) dựa trên Role hoặc Policy.
        /// </summary>
        public static WebApplicationBuilder AddAuthorizationServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddAuthorization();

            return builder;
        }

        /// <summary>
        /// Cấu hình HttpContextAccessor để cho phép truy cập thông tin HTTP Context 
        /// (như thông tin User đăng nhập, Headers, IP...) ở bất kỳ Service nào qua Dependency Injection.
        /// </summary>
        public static WebApplicationBuilder AddHttpContextServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddHttpContextAccessor();

            return builder;
        }

        /// <summary>
        /// Cấu hình Chia sẻ tài nguyên chéo nguồn (CORS), cho phép các ứng dụng Frontend 
        /// (như ReactJS running ở http://localhost:3000) gọi API vào Backend.
        /// </summary>
        public static WebApplicationBuilder AddCors(this WebApplicationBuilder builder)
        {
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: MyAllowSpecificOrigins,
                                  policy =>
                                  {
                                      policy.WithOrigins("http://localhost:3000")
                                            .AllowAnyHeader()
                                            .AllowAnyMethod();
                                  });
            });
            return builder;
        }

        /// <summary>
        /// Cấu hình Luồng xử lý Middleware (Request Pipeline) cho ứng dụng HTTP.
        /// Định vị thứ tự nhận và xử lý request/response.
        /// </summary>
        public static WebApplication ConfigureMiddleware(this WebApplication app)
        {
            var supportedCultures = new[] { new CultureInfo("vi-VN"), new CultureInfo("en-US") };
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("vi-VN"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            });
            // Các Custom Middleware xử lý lỗi toàn cục và Ghi Log Request
            app.UseMiddleware<GlobalExceptionMiddleware>();
            app.UseMiddleware<LoggingMiddleware>();

            // Bật giao diện Swagger trong môi trường Development
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Kích hoạt CORS
            app.UseCors(MyAllowSpecificOrigins);

            // Kích hoạt Middleware Xác thực & Phân quyền (bắt buộc đúng thứ tự này)
            app.UseAuthentication();
            app.UseAuthorization();

            // Ánh xạ các Controller Endpoint
            app.MapControllers();

            return app;
        }
        /// <summary>
        /// Custom Converter hỗ trợ tự động chuẩn hóa định dạng ngày giờ DateTime 
        /// sang chuẩn quốc tế UTC ISO-8601 khi gửi/nhận JSON.
        /// </summary>
        public class UtcDateTimeConverter : JsonConverter<DateTime>
        {
            public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                return reader.GetDateTime().ToUniversalTime();
            }

            public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
            {
                writer.WriteStringValue(value.ToUniversalTime().ToString("o"));
            }
        }

        /// <summary>
        /// Khởi tạo dữ liệu mặc định (Seed Data) cho hệ thống khi ứng dụng khởi động.
        /// Sử dụng BCrypt để hash mật khẩu.
        /// </summary>
        public static async Task<WebApplication> SeedDataAsync(this WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<ApplicationDbContext>();

                    // Tự động apply Migration nếu Database chưa tạo
                    if (context.Database.IsRelational())
                    {
                        await context.Database.MigrateAsync();
                    }

                    // Kiểm tra tài khoản Admin mặc định
                    const string adminEmail = "admin@gmail.com";
                    var adminUser = await context.Users.FirstOrDefaultAsync(u => u.Email == adminEmail);

                    if (adminUser == null)
                    {
                        // Mã hóa mật khẩu "123123" bằng BCrypt
                        string passwordHash = BCrypt.Net.BCrypt.HashPassword("123123");

                        var newAdmin = new User
                        {
                            Id = Guid.NewGuid(),
                            FullName = "System Administrator",
                            Email = adminEmail,
                            PasswordHash = passwordHash,
                            PhoneNumber = "0909123456",
                            Role = UserRole.Admin,
                            AvatarUrl = "https://ui-avatars.com/api/?name=System+Admin&background=0D8ABC&color=fff",
                            status = AccountStatus.ACTIVE,
                            CreatedAt = DateTime.UtcNow
                        };

                        await context.Users.AddAsync(newAdmin);
                        await context.SaveChangesAsync();

                        Console.WriteLine("--> [DATA SEEDER] Da khoi tao tai khoan Admin mac dinh: admin@gmail.com / 123123");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"--> [DATA SEEDER ERROR] Loi khoi tao du lieu mau: {ex.Message}");
                }
            }

            return app;
        }
    }
}
