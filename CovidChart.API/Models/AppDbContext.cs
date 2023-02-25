using Microsoft.EntityFrameworkCore;

namespace CovidChart.API.Models
{
    public class AppDbContext:DbContext
    {
        public DbSet<Covid> Covids { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
    }
}
