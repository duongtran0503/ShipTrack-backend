using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShipTrack_backend.Shared.Entities
{
    public class DriverLocation
    {
        [Key]
        public long Id { get; set; } // Mã định danh bản ghi tọa độ (Tự tăng BigInt để tối ưu)

        [Required]
        public Guid DriverId { get; set; } // Mã tài xế được ghi nhận tọa độ

        public Guid? OrderId { get; set; } // Mã đơn hàng tài xế đang thực hiện (null nếu đang chạy tự do)

        [Required]
        [Column(TypeName = "decimal(10,7)")]
        public decimal Latitude { get; set; } // Vĩ độ GPS tại thời điểm ghi nhận

        [Required]
        [Column(TypeName = "decimal(10,7)")]
        public decimal Longitude { get; set; } // Kinh độ GPS tại thời điểm ghi nhận

        public DateTime RecordedAt { get; set; } = DateTime.UtcNow; // Thời điểm hệ thống ghi nhận tọa độ

        // Navigation Properties
        [ForeignKey(nameof(DriverId))]
        public virtual Driver Driver { get; set; } = null!; // Tài xế tương ứng

        [ForeignKey(nameof(OrderId))]
        public virtual Order? Order { get; set; } // Đơn hàng đang được thực hiện (nếu có)
    }
}
