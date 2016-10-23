using System.Data.Entity;

using PhotoCloud.Models;

namespace PhotoCloud.Infrastructure.Data.EF
{
    public class PhotoCloudDbContext : DbContext
    {
        public DbSet<Photo> Photos { get; set; }
    }
}
