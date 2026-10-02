````markdown
# 🚚 ShipTrack Backend API

Hệ thống Backend cho ứng dụng theo dõi và quản lý đơn hàng theo thời gian thực (Real-time Order Tracking System), được xây dựng trên nền tảng **ASP.NET Core 10 Web API** và **Entity Framework Core**.

---

## 🏗️ Kiến trúc Source Code (Architecture)

Dự án được thiết kế theo hướng **Modular Monolith** kết hợp với kiến trúc phân tầng (Layered Architecture), giúp code sạch sẽ, dễ bảo trì và mở rộng:

```text
ShipTrack-backend/
├── Common/                  # Các thành phần dùng chung toàn ứng dụng
│   ├── Exceptions/          # Custom Exception (AppException, ErrorCode)
│   ├── Middleware/          # Global Exception, Logging Middleware
│   └── Response/            # Chuẩn hóa định dạng API Response
├── Modules/                 # Các module chức năng độc lập (Domain Modules)
│   └── Auth/                # Module Xác thực & Phân quyền
│       ├── Controllers/     # API Endpoints
│       ├── Dtos/            # Data Transfer Objects (Request/Response)
│       ├── ErrorCodes/      # Mã lỗi nghiệp vụ riêng cho Auth
│       ├── Repositories/    # Tầng truy xuất dữ liệu
│       └── Services/        # Tầng xử lý logic nghiệp vụ
├── Shared/                  # Tài nguyên & Cấu hình dùng chung
│   ├── Data/                # EF Core ApplicationDbContext & Migrations
│   ├── Entities/            # Database Models (User, Order, v.v.)
│   └── Enums/               # Các Enums hệ thống (UserRole, AccountStatus...)
├── Bootstrap.cs             # Extension Methods cấu hình Services & Middleware
└── Program.cs               # File khởi chạy ứng dụng (Minimal entry point)
```
````

---

## 📦 Các Packages chính & Công dụng

Hệ thống sử dụng các thư viện NuGet chính được chia theo từng nhóm chức năng:

### 1. Database & ORM (MySQL)

- **`Pomelo.EntityFrameworkCore.MySql`**: Driver kết nối và thao tác với CSDL MySQL thông qua Entity Framework Core.
- **`Microsoft.EntityFrameworkCore.Tools`**: Công cụ dòng lệnh hỗ trợ tạo và quản lý Migrations.
- **`Microsoft.EntityFrameworkCore.Design`**: Hỗ trợ EF Core Design-time cho quá trình sinh Migration.

### 2. Xác thực & Bảo mật (Authentication & Security)

- **`Microsoft.AspNetCore.Authentication.JwtBearer`**: Middleware xác thực người dùng bằng **JWT (JSON Web Token)**.
- **`System.IdentityModel.Tokens.Jwt`**: Thư viện khởi tạo và decode chuỗi JWT Bearer Token.
- **`BCrypt.Net-Next`**: Thư viện mã hóa (hash) và kiểm tra mật khẩu an toàn.

### 3. Tài liệu API & Hệ thống

- **`Swashbuckle.AspNetCore`**: Tự động tạo tài liệu giao diện kiểm thử **Swagger UI** tương tác trực tiếp.

---

## 🛠️ Hướng dẫn Cài đặt & Chạy ứng dụng

### 1. Yêu cầu hệ thống

- **.NET 10.0 SDK** trở lên.
- **XAMPP Control Panel** (hoặc MySQL Server tương đương).

### 2. Cấu hình Cơ sở dữ liệu (MySQL qua XAMPP)

1. Mở **XAMPP Control Panel** và nhấn **Start** dịch vụ **MySQL** (và Apache nếu cần dùng phpMyAdmin).
2. Tạo một Database mới có tên exact là: `shiptrack_db` (bảng mã `utf8mb4_unicode_ci`).
3. Kiểm tra Chuỗi kết nối (Connection String) trong file `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=shiptrack_db;User=root;Password=;"
}

```

_(Thay đổi `User` và `Password` nếu cấu hình MySQL của bạn khác mặc định)._

### 3. Chạy Migration (Khởi tạo Bảng trong CSDL)

Mở Terminal/Package Manager Console tại thư mục gốc dự án và chạy câu lệnh:

```bash
dotnet ef database update

```

> **Lưu ý:** Nếu hệ thống tự động chạy `MigrateAsync()` lúc khởi động (đã tích hợp sẵn trong code), bước này sẽ tự thực hiện khi bạn F5/chạy ứng dụng.

---

## 🚀 Khởi chạy Ứng dụng & Tài khoản Mặc định

### 1. Chạy dự án

sử dụng lệnh CLI hoặc nhấn **F5** trong Visual Studio / Rider:

```bash
dotnet run

```

### 2. Thông tin Tài khoản Admin mặc định

Khi ứng dụng khởi động lần đầu, hệ thống sẽ tự động Seed dữ liệu tài khoản Quản trị viên:

- **Email:** `admin@gmail.com`
- **Mật khẩu:** `123123`
- **Quyền:** `Admin`

---

## 📍 Truy cập & Trạng thái Hệ thống

- **Kiểm tra trạng thái hệ thống (Health Check):**
  Mở trình duyệt và truy cập vào đường dẫn:
  👉 **`http://localhost:5131/api`**
- **Giao diện thử nghiệm API (Swagger UI):**
  👉 **`http://localhost:5131/swagger`**
