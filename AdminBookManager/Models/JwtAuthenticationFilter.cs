using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace AdminBookManager.Models
{
    public class JwtAuthenticationFilter : IActionFilter
    {
        private readonly string _secretKey;

        public JwtAuthenticationFilter(IConfiguration configuration)
        {
            // Load the secret key securely from appsettings.json or environment variables
            _secretKey = configuration["JwtSettings:SecretKey"];
            if (string.IsNullOrEmpty(_secretKey))
            {
                throw new ArgumentException("JWT Secret Key is not configured properly.");
            }
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            // Retrieve token from Authorization header
            var authorizationHeader = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            var token = authorizationHeader?.Split(" ").Last();

            // Check if token is missing or invalid
            if (string.IsNullOrEmpty(token) || !VerifyToken(token))
            {
                context.Result = new UnauthorizedResult(); // Respond with HTTP 401
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // Not used in this scenario
        }

        private bool VerifyToken(string token)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var tokenHandler = new JwtSecurityTokenHandler();

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false, // Set to true if you validate the issuer
                ValidateAudience = false, // Set to true if you validate the audience
                ValidateLifetime = true, // Ensure the token is not expired
                ValidateIssuerSigningKey = true, // Validate the signature of the token
                IssuerSigningKey = key
            };

            try
            {
                tokenHandler.ValidateToken(token, validationParameters, out _);
                return true; // Token is valid
            }
            catch (SecurityTokenException)
            {
                return false; // Token is invalid
            }
        }
    }
}
