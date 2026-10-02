using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShipTrack_backend.Shared.Entities
{
    public class Address
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid(); // Mã định danh duy nhất của địa chỉ

        public Guid? UserId { get; set; } // Mã người dùng sở hữu địa chỉ này (null nếu địa chỉ vãng lai)

        [MaxLength(50)]
        public string? Label { get; set; } // Nhãn gợi nhớ (Ví dụ: Nhà riêng, Công ty, Kho hàng)

        [Required]
        [MaxLength(255)]
        public string FullAddress { get; set; } = string.Empty; // Địa chỉ chi tiết (Số nhà, tên đường...)

        [MaxLength(100)]
        public string? Ward { get; set; } // Tên Phường / Xã

        [MaxLength(100)]
        public string? District { get; set; } // Tên Quận / Huyện

        [MaxLength(100)]
        public string? City { get; set; } // Tên Tỉnh / Thành phố

        [Column(TypeName = "decimal(10,7)")]
        public decimal? Latitude { get; set; } // Vĩ độ toạ độ GPS

        [Column(TypeName = "decimal(10,7)")]
        public decimal? Longitude { get; set; } // Kinh độ toạ độ GPS

        public bool IsDefault { get; set; } = false; // Đặt làm địa chỉ mặc định của người dùng

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Thời điểm tạo địa chỉ

        // Navigation Property
        [ForeignKey(nameof(UserId))]
        public virtual User? User { get; set; } // Người dùng sở hữu địa chỉ
    }
}
