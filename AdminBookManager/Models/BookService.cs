using Microsoft.EntityFrameworkCore;

namespace AdminBookManager.Models
{
    public class BookService : IBookService
    {
        private readonly AdminBookManagerContext _context;

        public BookService(AdminBookManagerContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Book>> GetAllBooksAsync()
        {
            return await _context.Books.ToListAsync();
        }

        public List<Book> GetBooksByIds(List<int> bookIds)
        {
            if (bookIds == null || !bookIds.Any())
            {
                return new List<Book>();  // Return an empty list if bookIds are empty or null
            }

            return _context.Books.Where(b => bookIds.Contains(b.Id)).ToList();
        }

        // Implement GetBookByIdAsync
        public async Task<Book> GetBookByIdAsync(int id)
        {
            return await _context.Books.FindAsync(id);  // Fetch the book by ID asynchronously
        }

        public Book GetBookById(int id)  // Implementation of the GetBookById method
        {
            return _context.Books.Find(id);  // Synchronous version of fetching book by ID
        }
    }
}
