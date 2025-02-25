using Microsoft.EntityFrameworkCore;
using OrderedData.Models;

namespace OrderedData.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<UsersInfoModel> UsersInfo { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<UsersInfoModel>().ToTable("usersinfo");
        }
    }
} 