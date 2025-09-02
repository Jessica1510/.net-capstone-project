using Microsoft.EntityFrameworkCore;
using Worker_microservice.Models;

namespace Worker_microservice.Data
{
    public class WorkerDBContext : DbContext
    {
        public WorkerDBContext(DbContextOptions<WorkerDBContext> options) : base(options)
        {

        }

        public DbSet<Worker> Workers { get; set; }
    }
}
