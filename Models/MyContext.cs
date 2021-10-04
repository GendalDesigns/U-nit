using Microsoft.EntityFrameworkCore;
namespace ApartmentNetwork.Models
{
    public class MyContext : DbContext
    {
        public MyContext(DbContextOptions options) : base(options){}
        public DbSet<User> Users {get; set; }
        public DbSet<Building> Buildings {get; set; }
        public DbSet<Event> Events {get; set; }
        public DbSet<Bulletin> Bulletins {get; set; }
        public DbSet<Comment> Comments {get; set; }
    }
}