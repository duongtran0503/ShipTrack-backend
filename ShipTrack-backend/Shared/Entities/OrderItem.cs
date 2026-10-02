using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShipTrack_backend.Shared.Entities
{
    public class OrderItem
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid(); // Mã định danh duy nhất của món hàng

        [Required]
        public Guid OrderId { get; set; } // Mã đơn hàng chứa món này

        [Required]
        [MaxLength(150)]
        public string ItemName { get; set; } = string.Empty; // Tên món hàng / sản phẩm

        public int Quantity { get; set; } = 1; // Số lượng món hàng

        [Column(TypeName = "decimal(10,2)")]
        public decimal? Weight { get; set; } // Khối lượng món hàng (kg)

        [Column(TypeName = "decimal(18,2)")]
        public decimal? Price { get; set; } // Đơn giá / Khai báo giá trị của món hàng (VNĐ)

        [MaxLength(255)]
        public string? Note { get; set; } // Ghi chú riêng cho món này (VD: Đóng gói cẩn thận)

        // Navigation Property
        [ForeignKey(nameof(OrderId))]
        public virtual Order Order { get; set; } = null!; // Thông tin đơn hàng chứa sản phẩm
    }
}
