using Microsoft.EntityFrameworkCore;
using webApi.Models.Entity;

namespace webApi.Models.Data
{
    public class AppDbcontext : DbContext
    {
        public AppDbcontext(DbContextOptions<AppDbcontext> options) : base(options)
        {
        }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<User> Users { get; set; }

    }
}
