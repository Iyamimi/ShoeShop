using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoeShopLibrary.Contexts;
using ShoeShopWebApi.DTOs;

namespace ShoeShopWebApi.Controllers
{
    /// <summary>
    /// Контроллер для работы с заказами: получение заказов пользователя, обновление статуса и даты доставки.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly ShoeShopDbContext _context;

        public OrdersController(ShoeShopDbContext context)
        {
            _context = context;
        }

        // Получение заказов пользователя(авторизованного) по логину
        [HttpGet("user/{login}")]
        [Authorize]
        public IActionResult GetOrdersByUser(string login)
        {
            var user = _context.Users
                .FirstOrDefault(u => u.Login == login);

            if (user == null)
                return NotFound("Пользователь не найден");

            var orders = _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                .Where(o => o.Customer.FullName == user.FullName)
                .ToList();

            return Ok(orders);
        }

        // Изменение статуса заказа и даты доставки(доступно администраторам и менеджерам)
        [HttpPut("{id}")]
        [Authorize(Roles = "Администратор,Менеджер")]
        public IActionResult UpdateOrder(int id, [FromBody] OrderUpdateDto dto)
        {
            var order = _context.Orders.Find(id);
            if (order == null)
                return NotFound();

            // Обновляем статус заказа, если он передан в DTO
            if (!string.IsNullOrEmpty(dto.Status))
                order.Status = dto.Status;

            // Обновляем дату доставки, если она передана в DTO
            if (dto.DeliveryDate.HasValue)
                order.DeliveryDate = dto.DeliveryDate.Value;

            _context.SaveChanges();
            return Ok(order);
        }
    }
}
