
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using Microsoft.IdentityModel.Tokens;
    using System;
    using System.IdentityModel.Tokens.Jwt;
using System.Text;


namespace AdminBookManager.Services
{
    public class TokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Method to verify and decode the JWT token
        public void VerifyAndDecodeToken(string token)
        {
            // Retrieve the secret key from appsettings.json
            var key = _configuration["SECRET_KEY"];

            // Create a token handler and validate the token
            var tokenHandler = new JwtSecurityTokenHandler();

            // Set up the token validation parameters
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
            };

            try
            {
                // Validate the token and extract the claims
                var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

                // If validation is successful, you can now access the claims
                var jwtToken = validatedToken as JwtSecurityToken;
                var userId = jwtToken?.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;
                var isLoggedIn = jwtToken?.Claims.FirstOrDefault(c => c.Type == "isLoggedIn")?.Value;

                Console.WriteLine($"UserId: {userId}, IsLoggedIn: {isLoggedIn}");
            }
            catch (SecurityTokenException)
            {
                Console.WriteLine("Invalid token.");
            }
        }
    }

}
