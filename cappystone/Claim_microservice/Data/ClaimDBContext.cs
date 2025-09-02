using Microsoft.EntityFrameworkCore;
using Claim_microservice.Models;

namespace Claim_microservice.Data
{
    public class ClaimDbContext : DbContext
    {
        public ClaimDbContext(DbContextOptions<ClaimDbContext> options) : base(options) { }

        public DbSet<Claim> Claims { get; set; }
    }
}
