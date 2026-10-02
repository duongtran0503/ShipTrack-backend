using ShipTrack_backend.Shared.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShipTrack_backend.Shared.Entities
{
    public class Vehicle
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid(); // Mã định danh duy nhất của phương tiện

        [Required]
        public Guid DriverId { get; set; } // Mã tài xế sở hữu xe

        [Required]
        [MaxLength(20)]
        public string LicensePlate { get; set; } = string.Empty; // Biển số xe

        [Required]
        [Column(TypeName = "varchar(50)")]
        public VehicleType VehicleType { get; set; } = VehicleType.Motorbike; // Loại xe: Motorbike, Car, Truck

        [MaxLength(50)]
        public string? Brand { get; set; } // Hãng xe (Honda, Yamaha, Hyundai...)

        [MaxLength(50)]
        public string? Model { get; set; } // Dòng xe (Wave Alpha, SH, Accent...)

        [MaxLength(30)]
        public string? Color { get; set; } // Màu sơn xe

        [Column(TypeName = "decimal(10,2)")]
        public decimal? Capacity { get; set; } // Tải trọng tối đa của xe (tính bằng kg)

        public bool IsActive { get; set; } = true; // Trạng thái xe đang được sử dụng (true: Hoạt động)

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Ngày đăng ký phương tiện

        // Navigation Property
        [ForeignKey(nameof(DriverId))]
        public virtual Driver Driver { get; set; } = null!; // Thông tin tài xế chủ sở hữu
    }
}
