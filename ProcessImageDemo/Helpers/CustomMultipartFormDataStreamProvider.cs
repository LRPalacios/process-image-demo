using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;

namespace ProcessImageDemo.Helpers
{
    public class CustomMultipartFormDataStreamProvider : MultipartFormDataStreamProvider
    {
        public CustomMultipartFormDataStreamProvider(string path) : base(path) { }

        public override string GetLocalFileName(HttpContentHeaders headers)
        {
            string fileName = "";
            if (!string.IsNullOrWhiteSpace(headers.ContentDisposition.FileName))
            {
                var extension = Uri.UnescapeDataString(headers.ContentDisposition.FileName).Split('.')[1].Trim();
                fileName = (Guid.NewGuid().ToString() + '.' + extension).Replace("\"", string.Empty).Trim().Replace(" ", string.Empty).ToString();
            }
            return fileName;
        }

        public override Task ExecutePostProcessingAsync()
        {
            return base.ExecutePostProcessingAsync();
        }
    }
}