using System.ComponentModel.DataAnnotations;

namespace ShoeShopLibrary.DTOs
{
    // Данные для аутентификации пользователя в системе
    public class LoginDto
    {
        // Электронная почта пользователя
        [Required(ErrorMessage = "Введите логин")]
        [EmailAddress(ErrorMessage = "Введите корректный логин")]
        [Display(Name = "Логин")]
        public string Email { get; set; } = string.Empty;

        // Пароль пользователя для аутентификации
        [Required(ErrorMessage = "Введите пароль")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; } = string.Empty;
    }
}
