using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PhotoCloud.Models;

using Newtonsoft.Json;
using Windows.Storage;
using System.IO;

using HttpClient = System.Net.Http.HttpClient;
using System.Net.Http;
using System.Net.Http.Handlers;
using System.ComponentModel;
using System.Net;

namespace PhotoCloud.Clients
{
    public class PhotoClient
    {
        //private string baseAddress = "http://localhost/PhotoCloud/";
        private string baseAddress = "http://localhost:10500/";
        
        #region Events

        public event EventHandler<ProgressChangedEventArgs> ProgressChanged;

        #endregion

        #region Public Methods

        public async Task<IEnumerable<Photo>> GetPhotosAsync()
        {
            var jsonString = await getAsync("api/photos");

            return JsonConvert.DeserializeObject<IEnumerable<Photo>>(jsonString);
        }

        public async Task<Photo> UploadPhotoAsync(IStorageFile file)
        {
            using (var stream = await file.OpenStreamForReadAsync())
            {
                using (var content = new MultipartFormDataContent())
                {
                    using (var streamContent = new StreamContent(stream))
                    {
                        content.Add(streamContent, "file", "file.jpg");

                        using (var handler = new ProgressMessageHandler())
                        {
                            handler.InnerHandler = new HttpClientHandler();
                            handler.HttpReceiveProgress += Handler_HttpReceiveProgress;

                            using (var client = new HttpClient(handler))
                            {
                                client.BaseAddress = new Uri(this.baseAddress);
                                var response = await client.PostAsync("api/photos", content);

                                var responseContent = await response.Content.ReadAsStringAsync();

                                if (!response.IsSuccessStatusCode)
                                {
                                    throw new Exception(string.Format("{0} : {1}", response.StatusCode, responseContent));
                                }

                                return JsonConvert.DeserializeObject<Photo>(responseContent);
                            }
                        }
                    }
                }
            }
        }

        public async Task DownloadPhotoAsync(string url, IStorageFile file)
        {
            var request = WebRequest.Create(url);
            using (var response = await request.GetResponseAsync())
            {
                using (var inputStream = response.GetResponseStream())
                {
                    using (var outputStream = await file.OpenStreamForWriteAsync())
                    {
                        var received = 0;
                        var buffer = new byte[1024 * 2];

                        while ((received = await inputStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                        {
                            await outputStream.WriteAsync(buffer, 0, received);
                        }
                    }
                }
            }
        }

        public async Task RemovePhotoAsync(int id)
        {
            await deleteAsync("api/photos/" + id);
        }

        #endregion

        #region Event Methods

        private void Handler_HttpReceiveProgress(object sender, HttpProgressEventArgs e)
        {
            if (ProgressChanged != null)
            {
                ProgressChanged(this, new ProgressChangedEventArgs(e.ProgressPercentage, e.UserState));
            }
        }

        #endregion

        #region Helper Methods

        private async Task<string> getAsync(string requestUri)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.baseAddress);
                var response = await client.GetAsync(requestUri);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(response.StatusCode.ToString());
                }

                return await response.Content.ReadAsStringAsync();
            }
        }

        private async Task<string> deleteAsync(string requestUri)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.baseAddress);
                var response = await client.DeleteAsync(requestUri);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(response.StatusCode.ToString());
                }

                return await response.Content.ReadAsStringAsync();
            }
        }

        #endregion
    }
}
