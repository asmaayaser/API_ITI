using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Models
{
    public class ITIEntity:IdentityDbContext<ApplicationUser>
    {
        public ITIEntity() { }
        public ITIEntity(DbContextOptions options):base(options) { }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Department { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=DESKTOP-1M4O4BQ;Initial Catalog=WebAPI_ITI;Integrated Security=True");
            base.OnConfiguring(optionsBuilder);

        }

    }
}
