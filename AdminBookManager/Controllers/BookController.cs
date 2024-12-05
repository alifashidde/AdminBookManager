using AdminBookManager.Models;
using AdminBookManager.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Stripe;
using System.IdentityModel.Tokens.Jwt;
using System.Text;



namespace AdminBookManager.Controllers
{
    [ServiceFilter(typeof(JwtAuthenticationFilter))]
    public class BookController : Controller
    {
        private readonly AdminBookManagerContext _context;
        private readonly IBookService _bookService;
        private readonly StripeClient _stripeClient;
        private readonly PaymentService _paymentService;
        private readonly IOrderService _orderService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<BookController> _logger;


        //private readonly TokenService tokenService;

        public BookController(AdminBookManagerContext context, IBookService bookService, StripeClient stripeClient, PaymentService paymentService, IOrderService orderService, IConfiguration configuration)
        {
            _context = context;
            _bookService = bookService;
            _stripeClient = stripeClient;
            _paymentService = paymentService;
            _orderService = orderService;
            _configuration = configuration;
           

        }


        // GET: api/Book/Index
        [HttpGet]
        public IActionResult Index()
        {
           

            // Retrieve a list of books from the database
            var books = _context.Books.ToList();

            // Pass the books to the view
            return View(books);
        }

        public IActionResult Books()
        {
            // Retrieve all books from the database
            var books = _context.Books.ToList();

            // Pass the list of books to the view
            return View("Books", books); // Specify the "Books" view explicitly
        }

        public string VerifyAndDecodeToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            // Load validation parameters from configuration
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _configuration["JwtSettings:Issuer"], // Load issuer from appsettings.json
                ValidAudience = _configuration["JwtSettings:Audience"], // Load audience from appsettings.json
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]) // Load secret key from appsettings.json
                )
            };

            try
            {
                // Validate the token and extract claims
                var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

                // Ensure the token is a valid JWT and uses the correct signing algorithm
                if (!(validatedToken is JwtSecurityToken jwtToken) ||
                    !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256Signature, StringComparison.OrdinalIgnoreCase))
                {
                    throw new SecurityTokenException("Invalid token algorithm.");
                }

                // Extract claims
                var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
                var email = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Email)?.Value;

                // Return decoded claims as a formatted string
                return $"UserId: {userId}, Email: {email}";
            }
            catch (SecurityTokenException ex)
            {
                return $"Invalid token: {ex.Message}";
            }
            
        }

        // GET: api/Book/List

        [HttpGet]
        public async Task<IActionResult> BookList()
        {
            var token = HttpContext.Request.Query["token"];
            var decoded = VerifyAndDecodeToken(token);
            Console.WriteLine("Token: " + token);
            ViewData["Token"] = decoded;
            var books = await _context.Books.ToListAsync();
            return View(books);
        }

        // GET: api/Book/ViewDetails/{id}
        [HttpGet]
        public async Task<IActionResult> ViewDetails(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return NotFound();

            return View(book);
        }

        // GET: api/Book/Details/{id}
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var book = await _context.Books.FirstOrDefaultAsync(m => m.Id == id);
            if (book == null) return NotFound();

            return View(book);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var token = HttpContext.Request.Query["token"];
            var decoded = VerifyAndDecodeToken(token);
            Console.WriteLine("Token: " + token);
            ViewData["Token"] = token; // Store the token in ViewData so it can be used in the form
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Author,Genre,ISBN,Price,Description,Stock,ImagePath")] Book book, IFormFile imageFile, string token)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }

                    book.ImagePath = "/images/" + uniqueFileName;
                }

                _context.Add(book);
                await _context.SaveChangesAsync();

                // Redirect to Index with the token preserved
                return RedirectToAction(nameof(Index), new { token });
            }

            // If validation fails, return the token back to the view
            ViewData["Token"] = token;
            return View(book);
        }


        public IActionResult Edit(int id)
        {
            var token = HttpContext.Request.Query["token"];
            var decoded = VerifyAndDecodeToken(token);
            Console.WriteLine("Token: " + token);
            ViewData["Token"] = decoded;
            var book = _context.Books.FirstOrDefault(b => b.Id == id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Author,Genre,ISBN,Price,Description,Stock")] Book book, string token)
        {
            if (id != book.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(book);
                    await _context.SaveChangesAsync();

                    // Redirect to Index with the token preserved
                    return RedirectToAction(nameof(Index), new { token });
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    Console.WriteLine($"Concurrency error: {ex.Message}");
                    if (!_context.Books.Any(e => e.Id == book.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                    return StatusCode(500, "An error occurred while saving the book. Please try again later.");
                }
            }

            // If validation fails, return the token back to the view
            ViewData["Token"] = token;
            return View(book);
        }



        [HttpGet]
        public async Task<IActionResult> Delete(int id, string token)
        {
            try
            {
                // Find the book by ID
                var book = await _context.Books.FindAsync(id);
                if (book == null)
                {
                    return NotFound();
                }

                // Remove the book and save changes
                _context.Books.Remove(book);
                await _context.SaveChangesAsync();

                // Redirect to Index, passing the token along
                return RedirectToAction(nameof(Index), new { token });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while deleting the book: {ex.Message}");
                return StatusCode(500, "An error occurred while deleting the book. Please try again later.");
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Books.FindAsync(id);
            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Payment(int id, int quantity)
        {
            var book = _bookService.GetBookById(id);
            if (book == null) return NotFound();

            var totalPrice = book.Price * quantity;
            var paymentModel = new Payment
            {
                BookTitle = book.Title,
                Price = book.Price,
                Quantity = quantity,
                TotalPrice = totalPrice
            };

            return View(paymentModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompletePurchase(Payment payment, string PaymentToken, string PaymentMethod, string token)
        {
            // Check if token is being passed correctly
            if (string.IsNullOrEmpty(token))
            {
                Console.WriteLine("Token is missing in CompletePurchase.");
                return Unauthorized("Token is missing.");
            }

            // Assuming the payment service needs token for authentication or processing
            var paymentResult = await _paymentService.ProcessPayment(payment, PaymentToken, PaymentMethod);

            if (paymentResult.IsSuccess)
            {
                TempData["PaymentDetails"] = JsonConvert.SerializeObject(new SimplePaymentDetails
                {
                    BookTitle = payment.BookTitle,
                    Quantity = payment.Quantity,
                    TotalPrice = (int)payment.TotalPrice
                });

                // Redirect to PaymentSuccess, passing the token
                return RedirectToAction("PaymentSuccess", new { token });
            }
            else
            {
                // In case of payment failure, pass token to keep session state intact
                ViewBag.Token = token;
                return View("PaymentError");
            }
        }

        // Allow access via /Book/PaymentSuccess
        [HttpGet]
        public IActionResult PaymentSuccess(string token)
        {
            if (TempData["PaymentDetails"] is string paymentDetailsJson)
            {
                var paymentDetails = JsonConvert.DeserializeObject<SimplePaymentDetails>(paymentDetailsJson);
                if (paymentDetails == null)
                {
                    return View("PaymentError");
                }

                var paymentDetailsViewModel = new PaymentDetailsViewModel
                {
                    BookTitle = paymentDetails.BookTitle,
                    Quantity = paymentDetails.Quantity,
                    TotalPrice = paymentDetails.TotalPrice
                };

                // Pass token to view via ViewData
                ViewData["Token"] = token;

                return View(paymentDetailsViewModel);
            }

            return View("PaymentError");
        }

        [HttpGet]
        public IActionResult Error(int orderId)
        {
            return View();
        }

        private bool BookExists(int id) => _context.Books.Any(e => e.Id == id);
    }
}
