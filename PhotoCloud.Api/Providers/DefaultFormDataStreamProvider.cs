using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;

namespace PhotoCloud.Api.Providers
{
    public class DefaultFormDataStreamProvider : MultipartFormDataStreamProvider
    {
        #region Constructor

        public DefaultFormDataStreamProvider(string rootPath) : base(rootPath)
        {
        }

        public DefaultFormDataStreamProvider(string rootPath, int bufferSize) : base(rootPath, bufferSize)
        {
        }

        #endregion

        #region Properties

        public string FileName { get; private set; }

        #endregion

        #region Public Methods

        public override string GetLocalFileName(HttpContentHeaders headers)
        {
            if (headers?.ContentDisposition != null)
            {
                var filename = headers.ContentDisposition.FileName.Trim();

                var extension = Path.GetExtension(filename);

                FileName = string.Format("{0}{1}", Guid.NewGuid(), extension);
            }
            else
            {
                FileName = base.GetLocalFileName(headers);
            }

            return FileName;
        }

        #endregion
    }
}
