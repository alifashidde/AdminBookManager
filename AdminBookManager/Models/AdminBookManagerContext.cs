using Microsoft.EntityFrameworkCore;

namespace AdminBookManager.Models
{
    public class AdminBookManagerContext : DbContext
    {
        public AdminBookManagerContext(DbContextOptions<AdminBookManagerContext> options)
            : base(options)
        {
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Payment> Payments { get; set; }
    }
}
