using Hotel.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Context
{
    public class DataContext : IdentityDbContext<ApplicationUser>
    {
        IConfiguration config;

        public DataContext(IConfiguration _config)
        {
            config = _config;
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Guest> Guests { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Invoice> Invoices { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(config.GetConnectionString("DbContext"));
            base.OnConfiguring(optionsBuilder);
        }
    }
}
