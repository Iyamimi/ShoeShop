using Microsoft.EntityFrameworkCore;
using ShoeShopLibrary.Contexts;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ShoeShopDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Добавляем поддержку сессий(для хранения данных)
builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();

builder.Services.AddRazorPages();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();
app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapRazorPages();
app.Run();
