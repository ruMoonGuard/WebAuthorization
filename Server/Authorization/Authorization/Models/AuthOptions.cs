using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Authorization.Models
{
    public class AuthOptions
    {
        public const string ISSUER = "MyAuthServer"; // издатель токена
        public const string AUDIENCE = "http://localhost:51884/"; // потребитель токена
        const string KEY = "mysupersecret_secretkey!123qwerty";   // ключ для шифрации
        public const int LIFETIME = 100; // время жизни токена - 100 минут
        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        }
    }
}