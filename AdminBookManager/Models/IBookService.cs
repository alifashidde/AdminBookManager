namespace AdminBookManager.Models
{
    public interface IBookService
    {
        Task<IEnumerable<Book>> GetAllBooksAsync();
        Task<Book> GetBookByIdAsync(int id);  // Asynchronous version
        Book GetBookById(int id);  // Synchronous version
        List<Book> GetBooksByIds(List<int> bookIds);
    }

}
