using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoeShopLibrary.Contexts;
using ShoeShopLibrary.Models;

namespace ShoeShopWebApi.Controllers
{
    /// <summary>
    /// Контроллер для управления товарами
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ShoeShopDbContext _context;

        public ProductsController(ShoeShopDbContext context)
            => _context = context;

        // Получаем список всех товаров
        [HttpGet]
        public IActionResult GetProducts()
        {
            var products = _context.Products.ToList();
            return Ok(products);
        }

        // Получаем товар по артикулу
        [HttpGet("{article}")]
        public IActionResult GetProduct(string article)
        {
            var product = _context.Products.Find(article);
            if (product == null)
                return NotFound();
            return Ok(product);
        }

        // Добавление товара(доступно администратору и менеджеру)
        [HttpPost]
        [Authorize(Roles = "Администратор,Менеджер")]
        public IActionResult PostProduct([FromBody] Product product)
        {
            _context.Products.Add(product);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetProduct), new { article = product.Article }, product);
        }

        // Обновление товара(доступно администратору и менеджеру)
        [HttpPut("{article}")]
        [Authorize(Roles = "Администратор,Менеджер")]
        public IActionResult PutProduct(string article, [FromBody] Product product)
        {
            // Проверка по артикулу
            if (article != product.Article)
                return BadRequest();

            _context.Entry(product).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.SaveChanges();
            return NoContent();
        }

        // Удаление товара(доступно администратору и менеджеру)
        [HttpDelete("{article}")]
        [Authorize(Roles = "Администратор,Менеджер")]
        public IActionResult DeleteProduct(string article)
        {
            var product = _context.Products.Find(article);
            if (product == null)
                return NotFound();

            _context.Products.Remove(product);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
