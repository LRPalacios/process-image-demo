using ProcessImageDemo.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace ProcessImageDemo.Controllers
{
    public class ImagesController : ApiController
    {
        // POST api/values
        public async Task<HttpResponseMessage> Post()
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                return new HttpResponseMessage(HttpStatusCode.UnsupportedMediaType);
            }

            string root = HttpContext.Current.Server.MapPath("~/App_Data");
            // Thumbnails folder must already exist on App_Data
            string thumnbnailDirectory = Path.Combine(root, "thumbnails");
            var provider = new CustomMultipartFormDataStreamProvider(root);

            try
            {
                await Request.Content.ReadAsMultipartAsync(provider);
                foreach (MultipartFileData file in provider.FileData)
                {
                    string[] AbosuluteFileName = file.LocalFileName.Split('\\');
                    string fileName = AbosuluteFileName[AbosuluteFileName.Length - 1];
                    WriteThumbnail(file.LocalFileName, thumnbnailDirectory, fileName);
                }
                return Request.CreateResponse(HttpStatusCode.OK);

            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }

        }

        public void WriteThumbnail(string imagePath, string thumbnailsDirectory, string thumbnailName)
        {
            Bitmap srcBmp = new Bitmap(imagePath);
            bool isLandscape = (srcBmp.Width > srcBmp.Height);
            int width, height;
            if (isLandscape)
            {
                width = 320;
                height = 240;
            }
            else
            {
                width = 240;
                height = 320;
            }
            float ratio = srcBmp.Width / srcBmp.Height;
            SizeF newSize = new SizeF(width, height * ratio);
            Bitmap target = new Bitmap((int)newSize.Width, (int)newSize.Height);
            using (Graphics graphics = Graphics.FromImage(target))
            {
                graphics.CompositingQuality = CompositingQuality.HighSpeed;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.DrawImage(srcBmp, 0, 0, newSize.Width, newSize.Height);
                target.Save(Path.Combine(thumbnailsDirectory, thumbnailName), ImageFormat.Jpeg);
            }
        }
    }
}
