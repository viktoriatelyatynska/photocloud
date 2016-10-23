using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Net;

using PhotoCloud.Api.Models;
using PhotoCloud.Api.Providers;
using PhotoCloud.Infrastructure.Data.EF.Repositories;
using PhotoCloud.Models;

namespace PhotoCloud.Api.Controllers
{
    [RoutePrefix("api/photos")]
    public class PhotosController : ApiController
    {
        private readonly string rootPath;
        private readonly string storageDirectory;

        public PhotosController()
        {
            storageDirectory = "PhotoStorage";
            rootPath = HttpContext.Current.Server.MapPath("~/" + storageDirectory);
        }

        [HttpGet]
        public async Task<IEnumerable<PhotoModel>> GetPhotosAsync()
        {
            IEnumerable<Photo> photos;

            using (var repository = new PhotoRepository())
            {
                photos = await repository.GetPhotosAsync();
            }

            IEnumerable<PhotoModel> photosModels = photos.Select(photo => new PhotoModel
            {
                PhotoId = photo.PhotoId,
                Url = this.getPhotoUrl(photo.Path)
            });

            return photosModels;
        }

        [HttpPost]
        public async Task<IHttpActionResult> PostPhoto()
        {
            try
            {
                //check if content is content multipart
                if (!Request.Content.IsMimeMultipartContent())
                {
                    throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
                }

                //provider that uploads file(image) to server path (rootPath - path where file will be saved)
                var provider = new DefaultFormDataStreamProvider(rootPath);
                //reading incomming content
                await Request.Content.ReadAsMultipartAsync(provider);

                //create photo instance
                var photo = new Photo(provider.FileName);

                //create instance of repository
                using (var repository = new PhotoRepository())
                {
                    //adding information about photo to database
                    await repository.AddAsync(photo);
                }

                //returning status code 200 and information about saved photo
                //status code 200 means success. Methods Ok() returns status code 200
                return Ok(new PhotoModel
                {
                    PhotoId = photo.PhotoId,
                    Url = this.getPhotoUrl(photo.Path),
                });
            }
            catch (Exception e)
            {
                //returns status code 200 and information about error
                return BadRequest(e.Message);
            }
        }

        [Route("{id}")]
        [HttpDelete]
        public async Task<IHttpActionResult> RemoveAsync([FromUri]int id)
        {
            try
            {
                using (var repository = new PhotoRepository())
                {
                    var photo = await repository.GetPhotoByIdAsync(id);

                    if (photo != null)
                    {
                        var path = Path.Combine(rootPath, photo.Path);

                        if (File.Exists(path))
                        {
                            File.Delete(path);
                        }

                        await repository.RemoveAsync(id);
                    }
                }

                return Ok();
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        private string getPhotoUrl(string path)
        {
            Uri uri;

            var indexOfApi = Request.RequestUri.AbsoluteUri.IndexOf("/api/");
            var url = Request.RequestUri.AbsoluteUri.Substring(0, indexOfApi);

            return string.Format("{0}/{1}/{2}", url, this.storageDirectory, path);

            //if (indexOfApi > -1)
            //{
            //    var directory = Request.RequestUri.LocalPath.Substring(0, indexOfApi);

            //    uri = new Uri(string.Format("{0}://{1}/{2}/{3}/{4}", Request.RequestUri.Scheme, Request.RequestUri.Authority, directory, storageDirectory, path));

            //    return uri.AbsoluteUri;
            //}

            //uri = new Uri(string.Format("{0}://{1}/{2}/{3}", Request.RequestUri.Scheme, Request.RequestUri.Authority, storageDirectory, path));

            //return uri.AbsoluteUri;
        }
    }
}