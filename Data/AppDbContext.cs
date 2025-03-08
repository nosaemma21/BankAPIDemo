using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BankAccountManager.Data;

public class AppDbContext(DbContextOptions options, IConfiguration configuration)
    : IdentityDbContext<AppUser>(options)
{
    private readonly IConfiguration _configuration = configuration;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseSqlServer(_configuration.GetConnectionString("DefaultConnection"));
    }
}
