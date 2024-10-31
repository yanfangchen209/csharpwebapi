using System.IdentityModel.Tokens.Jwt; // For creating and handling JWT tokens.
using System.Security.Claims; // Provides classes for managing user claims in tokens.
using System.Security.Cryptography; // For generating cryptographic random bytes.
using System.Text; // For encoding strings as byte arrays.
using Microsoft.AspNetCore.Cryptography.KeyDerivation; // For securely deriving keys from passwords.
using Microsoft.IdentityModel.Tokens; // For handling token signing and validation.

namespace DotnetAPI.Helpers
{
    public class AuthHelper
    {
        // Stores a configuration object for accessing application settings.
        private IConfiguration _config;

        // Constructor that initializes the configuration through dependency injection (DI).
        // IConfiguration is registered by default in ASP.NET Core's DI container.
        public AuthHelper(IConfiguration config) {
            _config = config;
        }

        // Method to hash the password with a provided salt using PBKDF2.
        public byte[] GetPasswordHash(string password, byte[] passwordSalt) {
            // Combines a secret key from configuration with the provided salt.
            string passwordSaltPlusKey = _config.GetSection("AppSettings:PasswordKey").Value +
                                         Convert.ToBase64String(passwordSalt);

            // Returns the derived key by hashing the password with the salt+key combination.
            return KeyDerivation.Pbkdf2(
                password: password,
                salt: Encoding.ASCII.GetBytes(passwordSaltPlusKey),
                prf: KeyDerivationPrf.HMACSHA256, // HMAC-SHA256 as the pseudo-random function.
                iterationCount: 1000000, // Number of iterations to strengthen the hash.
                numBytesRequested: 256 / 8 // Size of the derived key in bytes.
            );
        }

        // Method to generate a secure random salt for password hashing.
        public byte[] GenerateSaltForPassword() {
            byte[] passwordSalt = new byte[128 / 8]; // Define a salt size (16 bytes).

            // Fill the byte array with cryptographically secure random bytes.
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetNonZeroBytes(passwordSalt); // Avoids zero bytes to strengthen the salt.
            }
            return passwordSalt; // Returns the generated salt.
        }

        // Method to create a JWT token containing a user ID claim.
        public string CreateToken(int userId)
        {
            // Defines a claim array with a single "userId" claim.
            Claim[] claims = new Claim[] {
                new Claim("userId", userId.ToString()) // Associates the user's ID with the token.
            };
            
            // Retrieves the token key from the configuration.
            string? tokenKeyString = _config.GetSection("AppSettings:TokenKey").Value;

            // Creates a symmetric security key from the token key string.
            SymmetricSecurityKey tokenKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    tokenKeyString != null ? tokenKeyString : ""
                )
            );

            // Configures the signing credentials using HMAC-SHA512.
            SigningCredentials credentials = new SigningCredentials(
                tokenKey, 
                SecurityAlgorithms.HmacSha512Signature
            );

            // Creates a security token descriptor with claims, credentials, and an expiry date.
            SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims), // Sets the token's claims.
                SigningCredentials = credentials, // Sets the token's signing credentials.
                Expires = DateTime.Now.AddDays(1) // Sets the token's expiration time.
            };

            // Initializes the JWT token handler.
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

            // Creates the token based on the descriptor and returns it as a string.
            SecurityToken token = tokenHandler.CreateToken(descriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
