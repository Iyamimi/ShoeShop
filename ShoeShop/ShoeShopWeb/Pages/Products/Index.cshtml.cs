using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ShoeShopLibrary.Contexts;
using ShoeShopLibrary.Models;

namespace ShoeShopWeb.Pages.Products
{
    public class IndexModel : PageModel
    {
        private readonly ShoeShopDbContext _context;

        public IndexModel(ShoeShopDbContext context)
        {
            _context = context;
        }

        public List<Product> Products { get; set; } = new();
        public List<Manufacturer> Manufacturers { get; set; } = new();

        // Загрузка товаров (есть фильтрация, сортировка и поиск)
        public async Task OnGetAsync(
            string? search = null,
            int? manufacturerId = null,
            decimal? maxPrice = null,
            bool onlyDiscount = false,
            bool onlyInStock = false,
            string sortBy = "name")
        {
            Manufacturers = await _context.Manufacturers
                .OrderBy(m => m.Name)
                .ToListAsync();

            var query = _context.Products
                .Include(p => p.Manufacturer)
                .Include(p => p.Supplier)
                .Include(p => p.Category)
                .AsQueryable();

            // Поиск по описанию товара
            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();
                query = query.Where(p =>
                    (p.Description != null && p.Description.ToLower().Contains(search)) ||
                    p.Name.ToLower().Contains(search));
            }

            // Фильтрация по производителю
            if (manufacturerId.HasValue)
            {
                query = query.Where(p => p.ManufacturerId == manufacturerId.Value);
            }

            // Фильтрация по цене не более указанной
            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= maxPrice.Value);
            }

            // Отображение только товаров со скидкой
            if (onlyDiscount)
            {
                query = query.Where(p => p.Discount > 0);
            }

            // Оображение только товаров в наличии
            if (onlyInStock)
            {
                query = query.Where(p => p.Quantity > 0);
            }

            // Сортировка по названию, поставщику, цене
            query = sortBy switch
            {
                "name" => query.OrderBy(p => p.Name),
                "supplier" => query.OrderBy(p => p.Supplier.Name),
                "price" => query.OrderBy(p => p.Price * (100 - p.Discount) / 100),
                "price_desc" => query.OrderByDescending(p => p.Price * (100 - p.Discount) / 100),
                _ => query.OrderBy(p => p.Name)
            };

            Products = await query.ToListAsync();
        }

        // Заказ
        public async Task<IActionResult> OnPostOrderAsync(string article)
        {
            try
            {
                var userId = HttpContext.Session.GetString("UserId");
                var userRole = HttpContext.Session.GetString("Role");

                if (string.IsNullOrEmpty(userId))
                {
                    return RedirectToPage("/Account/Login");
                }

                if (userRole != "Клиент")
                {
                    TempData["ErrorMessage"] = "Только клиенты могут оформлять заказы";
                    return RedirectToPage("./Index");
                }

                var product = await _context.Products.FirstOrDefaultAsync(p => p.Article == article);
                if (product == null)
                {
                    TempData["ErrorMessage"] = "Товар не найден";
                    return RedirectToPage("./Index");
                }

                if (product.Quantity <= 0)
                {
                    TempData["ErrorMessage"] = $"Товар '{product.Name}' отсутствует на складе";
                    return RedirectToPage("./Index");
                }

                var user = await _context.Users
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.UserId == int.Parse(userId));

                if (user == null)
                {
                    TempData["ErrorMessage"] = "Пользователь не найден";
                    return RedirectToPage("./Index");
                }

                var customer = await _context.Customers
                    .FirstOrDefaultAsync(c => c.FullName == user.FullName);

                if (customer == null)
                {
                    customer = new Customer { FullName = user.FullName };
                    _context.Customers.Add(customer);
                    await _context.SaveChangesAsync();
                }

                var order = new Order
                {
                    OrderDate = DateOnly.FromDateTime(DateTime.Today),
                    DeliveryDate = DateOnly.FromDateTime(DateTime.Today.AddDays(7)), //(через 7 дней)
                    CustomerId = customer.CustomerId,
                    PickupCode = new Random().Next(100, 1000),
                    Status = "Новый"
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                var orderItem = new OrderItem
                {
                    OrderId = order.OrderId,
                    Article = article,
                    Quantity = 1
                };

                _context.OrderItems.Add(orderItem);

                product.Quantity -= 1;
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Заказ №{order.OrderId} успешно создан! Код получения: {order.PickupCode}";
                return RedirectToPage("./Index");
            }
            catch (DbUpdateException dbEx)
            {
                TempData["ErrorMessage"] = $"Ошибка базы данных при создании заказа. Возможно, товара недостаточно на складе.";
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                var errorDetails = ex.Message;
                if (ex.InnerException != null)
                {
                    errorDetails += $" | Внутренняя ошибка: {ex.InnerException.Message}";
                }

                TempData["ErrorMessage"] = $"Ошибка создания заказа: {errorDetails}";
                return RedirectToPage("./Index");
            }
        }
    }
}
