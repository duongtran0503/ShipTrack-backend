using Microsoft.EntityFrameworkCore;
using ShipTrack_backend.Shared.Entities;

namespace ShipTrack_backend.Shared.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Đăng ký các DbSet (Bảng trong Database) tại đây
        public DbSet<User> Users { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<OrderStatusHistory> OrderStatusHistories { get; set; }
        public DbSet<DriverLocation> DriverLocations { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Chuyển các Enum thành dạng String khi lưu vào Database
            modelBuilder.Entity<User>()
                .Property(u => u.Role)
                .HasConversion<string>();

            modelBuilder.Entity<Vehicle>()
                .Property(v => v.VehicleType)
                .HasConversion<string>();

            modelBuilder.Entity<Order>()
                .Property(o => o.Status)
                .HasConversion<string>();

            modelBuilder.Entity<OrderStatusHistory>()
                .Property(h => h.Status)
                .HasConversion<string>();

            // Cấu hình Unique Index
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Driver>()
                .HasIndex(d => d.LicenseNumber)
                .IsUnique();

            modelBuilder.Entity<Vehicle>()
                .HasIndex(v => v.LicensePlate)
                .IsUnique();

            modelBuilder.Entity<Order>()
                .HasIndex(o => o.OrderCode)
                .IsUnique();

            // Cấu hình mối quan hệ 1-1 giữa User và Driver
            modelBuilder.Entity<User>()
                .HasOne(u => u.DriverProfile)
                .WithOne(d => d.User)
                .HasForeignKey<Driver>(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Tránh lỗi Cascade Delete trùng lặp trên MySQL với địa chỉ Order
            modelBuilder.Entity<Order>()
                .HasOne(o => o.PickupAddress)
                .WithMany()
                .HasForeignKey(o => o.PickupAddressId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.DeliveryAddress)
                .WithMany()
                .HasForeignKey(o => o.DeliveryAddressId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
