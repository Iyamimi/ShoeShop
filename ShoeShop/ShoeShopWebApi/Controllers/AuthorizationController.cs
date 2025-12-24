using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ShoeShopLibrary.Contexts;
using ShoeShopLibrary.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ShoeShopWebApi.Controllers
{
    /// <summary>
    /// Контроллер для аутентификации пользователей. Предоставляет API для входа (login) с выдачей JWT-токена.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorizationController : ControllerBase
    {
        private readonly ShoeShopDbContext _context;

        public AuthorizationController(ShoeShopDbContext context)
        {
            _context = context;
        }

        // Авторизация пользователя по логину и паролю с использованием JWT
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto request)
        {
            var user = _context.Users
                .Include(u => u.Role)
                .FirstOrDefault(u => u.Login == request.Email && u.Password == request.Password);

            if (user == null)
                return Unauthorized("Неверный логин или пароль");

            if (user.Role == null)
                return Unauthorized("Pоль пользователя не найдена");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Login),
                new Claim(ClaimTypes.Role, user.Role.RoleName),
                new Claim("UserId", user.UserId.ToString()),
                new Claim("FullName", user.FullName)
            };

            // Создание JWT токена
            var jwt = new JwtSecurityToken(
                issuer: JwtSettings.Issuer,
                audience: JwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(120)),
                signingCredentials: new SigningCredentials(
                    JwtSettings.GetSymmetricSecurityKey(),
                    SecurityAlgorithms.HmacSha256));

            var token = new JwtSecurityTokenHandler().WriteToken(jwt);

            return Ok(new
            {
                token = token,
                role = user.Role.RoleName,
                name = user.FullName
            });
        }
    }
}
