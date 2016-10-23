using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;

using PhotoCloud.Models;
using System;

namespace PhotoCloud.Infrastructure.Data.EF.Repositories
{
    public class PhotoRepository : IDisposable
    {
        private readonly PhotoCloudDbContext context = new PhotoCloudDbContext();

        public async Task<IEnumerable<Photo>> GetPhotosAsync()
        {
            return await context.Photos.ToListAsync();
        }

        public async Task<Photo> GetPhotoByIdAsync(int id)
        {
            return await context.Photos.FindAsync(id);
        }

        public async Task AddAsync(Photo photo)
        {
            context.Photos.Add(photo);

            await context.SaveChangesAsync();
        }

        public async Task RemoveAsync(int id)
        {
            var photo = await context.Photos.FirstOrDefaultAsync(p=>p.PhotoId == id);

            if (photo == null)
            {
                throw new Exception("Photo does not exist or has been deleted.");
            }

            context.Photos.Remove(photo);

            await context.SaveChangesAsync();
        }

        public void Dispose()
        {
            context.Dispose();
        }
    }
}
