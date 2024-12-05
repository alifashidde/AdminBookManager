using System.ComponentModel.DataAnnotations;

namespace AdminBookManager.Models
{
    public class Book
    {
        public int Id { get; set; } // Primary key

        public string Title { get; set; }      // NVARCHAR(255)
        public string Author { get; set; }     // NVARCHAR(255)
        public string Genre { get; set; }      // NVARCHAR(100)
        public string ISBN { get; set; }       // NVARCHAR(20)
        public decimal Price { get; set; }     // DECIMAL(18, 2)
        public string Description { get; set; } // NVARCHAR(MAX)
        public int Stock { get; set; }         // INT

        
        public string? ImagePath { get; set; }
        public int Quantity { get; set; }
    }

}
