using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ECommerceSystem.Api.Data
{
    public class WebDBContextFactory : IDesignTimeDbContextFactory<WebDBContext>
    {
        public WebDBContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<WebDBContext>();
            // Chèn connection string phù hợp
            optionsBuilder.UseSqlServer("Server=host.docker.internal,1433;Database=NewbieShop;User Id=sa;Password=bang31102004;TrustServerCertificate=True  ;");

            return new WebDBContext(optionsBuilder.Options);
        }
    }
}
