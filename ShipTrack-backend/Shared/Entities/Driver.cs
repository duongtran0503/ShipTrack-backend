using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShipTrack_backend.Shared.Entities
{
    public class Driver
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid(); // Mã định danh duy nhất của tài xế

        [Required]
        public Guid UserId { get; set; } // Mã người dùng (Khoá ngoại liên kết tới User)

        [Required]
        [MaxLength(50)]
        public string LicenseNumber { get; set; } = string.Empty; // Số bằng lái xe

        public DateTime LicenseExpiry { get; set; } // Ngày hết hạn bằng lái xe

        public bool IsAvailable { get; set; } = true; // Trạng thái sẵn sàng nhận đơn (true: Bật app chờ đơn)

        [Column(TypeName = "decimal(10,7)")]
        public decimal? CurrentLatitude { get; set; } // Vĩ độ vị trí hiện tại của tài xế

        [Column(TypeName = "decimal(10,7)")]
        public decimal? CurrentLongitude { get; set; } // Kinh độ vị trí hiện tại của tài xế

        public DateTime? LastLocationUpdate { get; set; } // Thời điểm cập nhật GPS gần nhất

        [Column(TypeName = "decimal(3,2)")]
        public decimal Rating { get; set; } = 5.00m; // Điểm đánh giá trung bình (1.00 -> 5.00)

        public int TotalTrips { get; set; } = 0; // Tổng số chuyến hàng đã hoàn thành

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Ngày đăng ký trở thành tài xế

        // Navigation Properties
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; } = null!; // Thông tin tài khoản user gốc

        public virtual ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>(); // Danh sách xe đăng ký
        public virtual ICollection<Order> AssignedOrders { get; set; } = new List<Order>(); // Danh sách đơn hàng nhận giao
        public virtual ICollection<DriverLocation> LocationHistories { get; set; } = new List<DriverLocation>(); // Lịch sử lộ trình di chuyển
    }
}
