using Microsoft.EntityFrameworkCore;
using ToDo.Models;

namespace ToDo.DataAccess
{
    public class ApplicationDbContext:DbContext
    {
        public DbSet<Item> Items { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=ToDo;Integrated Security=True;TrustServerCertificate=True");
        }
    }
}
