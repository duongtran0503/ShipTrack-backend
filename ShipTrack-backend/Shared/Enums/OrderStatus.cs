namespace ShipTrack_backend.Shared.Enums
{
    public enum OrderStatus
    {
        Pending,          // Đơn mới tạo
        SearchingDriver,  // Đang tìm tài xế
        DriverAssigned,   // Đã có tài xế nhận
        PickedUp,         // Đã lấy hàng
        Delivering,       // Đang giao
        Completed,        // Hoàn thành
        Cancelled,        // Đã hủy
        Failed            // Giao thất bại
    }
}
