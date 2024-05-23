using Microsoft.EntityFrameworkCore;

namespace _4praktika.Models
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options)
           : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=chat;Username=postgres;Password=mirandolina");
            optionsBuilder.UseSnakeCaseNamingConvention();
        }
    }
}