using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ShoeShopLibrary.Contexts;
using ShoeShopLibrary.Models;

namespace ShoeShopWeb.Pages.Orders
{
    public class IndexModel : PageModel
    {
        private readonly ShoeShopDbContext _context;

        public IndexModel(ShoeShopDbContext context)
        {
            _context = context;
        }

        public List<Order> Orders { get; set; } = new();

        // Загрузка заказов (зависит от роли)
        public async Task<IActionResult> OnGetAsync()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
            {
                return RedirectToPage("/Account/Login");
            }

            var userId = HttpContext.Session.GetString("UserId");
            var userRole = HttpContext.Session.GetString("Role");

            IQueryable<Order> query = _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.ArticleNavigation)
                .Include(o => o.Customer);

            // Если пользователь - клиент, показываем только его заказы
            if (userRole == "Клиент")
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.UserId == int.Parse(userId));

                if (user != null)
                {
                    var customer = await _context.Customers
                        .FirstOrDefaultAsync(c => c.FullName == user.FullName);

                    if (customer != null)
                    {
                        query = query.Where(o => o.CustomerId == customer.CustomerId);
                    }
                    else
                    {
                        customer = new Customer { FullName = user.FullName };
                        _context.Customers.Add(customer);
                        await _context.SaveChangesAsync();

                        query = query.Where(o => o.CustomerId == customer.CustomerId);
                    }
                }
                else
                {
                    Orders = new List<Order>();
                    return Page();
                }
            }
            // Если пользователь - менеджер или администратор, показываем все заказы

            Orders = await query.ToListAsync();
            return Page();
        }

        // Расчет итоговой стоимости заказа
        public decimal GetTotalPrice(Order order)
        {
            return order.OrderItems.Sum(oi =>
                (oi.ArticleNavigation.Price * (100 - oi.ArticleNavigation.Discount) / 100) * oi.Quantity);
        }

        public bool IsClient()
        {
            return HttpContext.Session.GetString("Role") == "Клиент";
        }
    }
}
