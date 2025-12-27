using Microsoft.EntityFrameworkCore;
using SlutProv.Api.Models;

namespace SlutProv.Api.Data
{

    public class ImageDbContext : DbContext
    {
        public ImageDbContext(DbContextOptions<ImageDbContext> options) : base(options) { }

        public DbSet<ImageMetadata> Images { get; set; }
    }
}
