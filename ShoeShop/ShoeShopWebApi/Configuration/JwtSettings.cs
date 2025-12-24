using Microsoft.IdentityModel.Tokens;
using System.Text;
// Конфигурация JWT для аутентификации в API
public class JwtSettings
{
    // Cервер, который выдаёт токены
    public const string Issuer = "ShoeStoreServer";

    // Клиентское приложение, которое принимает токены
    public const string Audience = "ShoeStoreClient";

    // Секретный ключ для подписи токенов
    private const string SecretKey = "your-32-char-secret-key-here-12345";

    // Создаёт симметричный ключ безопасности для подписи токенов
    public static SymmetricSecurityKey GetSymmetricSecurityKey()
    {
        return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
    }
}

