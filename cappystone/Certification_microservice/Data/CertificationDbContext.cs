using Certification_microservice.Models;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Worker_microservice.Models;

namespace Certification_microservice.Data
{
    public class CertificationDbContext : DbContext
    {
        public CertificationDbContext(DbContextOptions<CertificationDbContext> options) : base(options) { }

        public DbSet<Certification> Certifications { get; set; }
        public DbSet<Worker> Workers { get; set; }
    }
}
