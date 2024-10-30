
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace DotnetAPI.Helpers
{
    public class AuthHelper{
        IConfiguration _config;
        //config is injected by the dependency injection (DI) container. In ASP.NET Core, IConfiguration is registered with the DI container by default
        public AuthHelper(IConfiguration config) {
            _config = config;
        }

        public byte[] GetPasswordHash(string password, byte[] passwordSalt) {
            string passwordSaltPlusKey = _config.GetSection("AppSettings:PasswordKey").Value +
            Convert.ToBase64String(passwordSalt);

            return KeyDerivation.Pbkdf2(
                password: password,
                salt: Encoding.ASCII.GetBytes(passwordSaltPlusKey),
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 1000000,
                numBytesRequested: 256 / 8
            );
        }



        public byte[] GenerateSaltForPassword() {
            byte[] passwordSalt = new byte[128 / 8];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetNonZeroBytes(passwordSalt);
            }
            return passwordSalt;
        }
        
    }
}