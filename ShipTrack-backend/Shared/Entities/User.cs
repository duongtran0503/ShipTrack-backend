using ShipTrack_backend.Shared.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShipTrack_backend.Shared.Entities
{
    public class User
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid(); // Mã định danh duy nhất của người dùng

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty; // Họ và tên đầy đủ

        [Required]
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty; // Email đăng nhập (duy nhất)

        [Required]
        [MaxLength(255)]
        public string PasswordHash { get; set; } = string.Empty; // Mật khẩu đã mã hóa (BCrypt/PBKDF2)

        [MaxLength(20)]
        public string? PhoneNumber { get; set; } // Số điện thoại liên lạc

        [Required]
        [Column(TypeName = "varchar(20)")]
        public UserRole Role { get; set; } = UserRole.Customer; // Vai trò: Customer, Driver, Admin

        [MaxLength(255)]
        public string? AvatarUrl { get; set; } // Đường dẫn ảnh đại diện

        public AccountStatus  status { get; set; } = AccountStatus.ACTIVE; // Trạng thái tài khoản (active: Hoạt động, block: Bị khóa)

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Thời điểm đăng ký tài khoản

        public DateTime? UpdatedAt { get; set; } // Thời điểm cập nhật thông tin gần nhất

        // Navigation Properties (Quan hệ bảng)
        public virtual Driver? DriverProfile { get; set; } // Hồ sơ tài xế (nếu Role = Driver)
        public virtual ICollection<Address> Addresses { get; set; } = new List<Address>(); // Danh sách địa chỉ đã lưu
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>(); // Danh sách đơn hàng đã đặt
    }
}
