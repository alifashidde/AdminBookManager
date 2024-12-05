using AdminBookManager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;

namespace AdminBookManager.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AdminBookManagerContext _context; // Use the correct DbContext name

        // Modify the constructor to include the DbContext
        public HomeController(ILogger<HomeController> logger, AdminBookManagerContext context) // Updated here
        {
            _logger = logger;
            _context = context; // Assign the DbContext to a private field
        }

        // Modify the Index action to retrieve books from the database
        public IActionResult Index()
        {
     

            // Retrieve a list of books from the database
            var books = _context.Books.ToList();

            // Pass the books to the view
            return View(books);
        }

        public string VerifyAndDecodeToken(string token)
        {

            // Retrieve the secret key from appsettings.json
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("46278411218d8fb75f55766b5c26281fff9fa1b65284e52f8aa74fbc27a83d6b"));

            var tokenHandler = new JwtSecurityTokenHandler();

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key
            };

            try
            {
                // Validate the token and extract the claims
                var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

                // Extract claims
                var jwtToken = validatedToken as JwtSecurityToken;
                var userId = jwtToken?.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;
                var email = jwtToken?.Claims.FirstOrDefault(c => c.Type == "email")?.Value;

                // Return a string with the decoded information
                return $"UserId: {jwtToken}";
            }
            catch (SecurityTokenException)
            {
                return "Invalid token.";
            }
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
