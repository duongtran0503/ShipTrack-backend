using ShipTrack_backend.Shared.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShipTrack_backend.Shared.Entities
{
    public class Order
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid(); // Mã định danh duy nhất của đơn hàng

        [Required]
        [MaxLength(20)]
        public string OrderCode { get; set; } = string.Empty; // Mã đơn hàng hiển thị (Ví dụ: ORD-20261002-001)

        [Required]
        public Guid CustomerId { get; set; } // Mã khách hàng tạo đơn

        public Guid? DriverId { get; set; } // Mã tài xế nhận giao (null khi chưa có ai nhận)

        [Required]
        public Guid PickupAddressId { get; set; } // Mã địa chỉ lấy hàng

        [Required]
        public Guid DeliveryAddressId { get; set; } // Mã địa chỉ giao hàng

        [Required]
        [Column(TypeName = "varchar(30)")]
        public OrderStatus Status { get; set; } = OrderStatus.Pending; // Trạng thái đơn (Pending, Delivering, Completed...)

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; } = 0; // Tổng số tiền đơn hàng (Tiền hàng + Phí ship)

        [Column(TypeName = "decimal(18,2)")]
        public decimal ShippingFee { get; set; } = 0; // Cước phí vận chuyển

        [Column(TypeName = "decimal(10,2)")]
        public decimal? Distance { get; set; } // Khoảng cách quãng đường giao hàng (km)

        public int? EstimatedTime { get; set; } // Thời gian giao hàng dự kiến (phút)

        [MaxLength(500)]
        public string? Note { get; set; } // Ghi chú của khách hàng (VD: Hàng dễ vỡ, gọi trước 5p)

        [MaxLength(255)]
        public string? CancelReason { get; set; } // Lý do hủy đơn (nếu đơn bị hủy)

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Thời điểm khách tạo đơn
        public DateTime? AcceptedAt { get; set; } // Thời điểm tài xế bấm nhận đơn
        public DateTime? PickedUpAt { get; set; } // Thời điểm tài xế đã lấy được hàng
        public DateTime? DeliveredAt { get; set; } // Thời điểm giao thành công cho người nhận
        public DateTime? CancelledAt { get; set; } // Thời điểm đơn bị hủy

        // Navigation Properties
        [ForeignKey(nameof(CustomerId))]
        public virtual User Customer { get; set; } = null!; // Thông tin khách hàng tạo đơn

        [ForeignKey(nameof(DriverId))]
        public virtual Driver? Driver { get; set; } // Thông tin tài xế nhận đơn

        [ForeignKey(nameof(PickupAddressId))]
        public virtual Address PickupAddress { get; set; } = null!; // Chi tiết địa chỉ lấy hàng

        [ForeignKey(nameof(DeliveryAddressId))]
        public virtual Address DeliveryAddress { get; set; } = null!; // Chi tiết địa chỉ giao hàng

        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>(); // Danh sách mặt hàng trong đơn
        public virtual ICollection<OrderStatusHistory> StatusHistories { get; set; } = new List<OrderStatusHistory>(); // Nhật ký thay đổi trạng thái
    }
}
