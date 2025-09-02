using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Account_microservice.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
/*
namespace Account_microservice.DTO
{
    public class AccountDbContext: IdentityDbContext<ApplicationUser>
    {
        public AccountDbContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<User> Users { get; set; }
    }
}
*/

// Data/AppDbContext.cs

namespace account_microservice.Data
{
    public class AccountDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        public AccountDbContext(DbContextOptions<AccountDbContext> options) : base(options) { }
    }
}
