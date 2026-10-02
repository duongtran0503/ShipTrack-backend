using ShipTrack_backend.Shared.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShipTrack_backend.Shared.Entities
{
    public class OrderStatusHistory
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid(); // Mã định danh bản ghi lịch sử

        [Required]
        public Guid OrderId { get; set; } // Mã đơn hàng tương ứng

        [Required]
        [Column(TypeName = "varchar(30)")]
        public OrderStatus Status { get; set; } // Trạng thái được chuyển sang tại thời điểm này

        public Guid? ChangedBy { get; set; } // Mã người thực hiện thay đổi (Khách, Tài xế, Admin hoặc System)

        [MaxLength(255)]
        public string? Note { get; set; } // Lý do hoặc ghi chú chi tiết khi chuyển trạng thái

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Thời điểm diễn ra sự thay đổi

        // Navigation Properties
        [ForeignKey(nameof(OrderId))]
        public virtual Order Order { get; set; } = null!; // Đơn hàng liên quan

        [ForeignKey(nameof(ChangedBy))]
        public virtual User? ChangedByUser { get; set; } // Người tác động chuyển trạng thái
    }
}
