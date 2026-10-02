using ShipTrack_backend;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureServices();


var app = builder.Build();


app.ConfigureMiddleware();

// Kích hoạt Data Seeder khi chạy ứng dụng
await app.SeedDataAsync();


app.Run();