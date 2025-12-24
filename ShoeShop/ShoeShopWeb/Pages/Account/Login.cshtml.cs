using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ShoeShopLibrary.Contexts;
using ShoeShopLibrary.DTOs;

namespace ShoeShopWeb.Pages.Account
{
    public class Login : PageModel
    {
        private readonly ShoeShopDbContext _context;

        public Login(ShoeShopDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public LoginDto LoginData { get; set; } = new();

        public string ErrorMessage { get; set; }

        // Проверка авторизации при загрузке страницы
        public IActionResult OnGet()
        {
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))

                return RedirectToPage("/Products/Index");
            return Page();
        }

        // Обработка входа пользователя(сохранение данных)
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Login == LoginData.Email && u.Password == LoginData.Password);

            if (user == null)
            {
                ErrorMessage = "Неверный логин или пароль";
                return Page();
            }

            HttpContext.Session.SetString("UserId", user.UserId.ToString());
            HttpContext.Session.SetString("UserName", user.FullName);
            HttpContext.Session.SetString("UserLogin", user.Login);
            HttpContext.Session.SetString("Role", user.Role.RoleName);

            return RedirectToPage("/Products/Index");
        }
    }
}
