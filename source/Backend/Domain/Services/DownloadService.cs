using System;
using System.Net;
using System.Security.Cryptography;
using System.Threading.Tasks;
using no.miles.at.Backend.Domain.ValueTypes;

namespace no.miles.at.Backend.Domain.Services
{
    public class DownloadService
    {
        public static async Task<Picture> DownloadPhoto(string imageUrl, string name, Action<string> info, Action<string,Exception> error )
        {
            Picture photo = null;
            if (imageUrl != null)
            {
                byte[] picture = null;
                string extension = null;
                string contentType = string.Empty;

                try
                {
                    var client = new WebClient();
                    picture = await client.DownloadDataTaskAsync(imageUrl);
                    contentType = client.ResponseHeaders["Content-Type"];

                    var urlWithoutQueryParameters = imageUrl;

                    if (imageUrl.IndexOf("?", StringComparison.Ordinal) >= 0)
                    {
                        urlWithoutQueryParameters = imageUrl.Substring(0, imageUrl.IndexOf("?", StringComparison.Ordinal));
                    }

                    extension = urlWithoutQueryParameters.Substring(imageUrl.LastIndexOf(".", StringComparison.Ordinal))
                        .Replace(".", string.Empty);
                    
                    info(string.Format("Found image of . {0} format", extension));
                }
                catch (Exception ex)
                {
                    error("Error downloading image", ex);
                }
                if (picture != null)
                {
                    var hash = MD5.Create().ComputeHash(picture);
                    photo = new Picture(name, extension, picture, contentType, hash);
                }
            }
            return photo;
        } 
    }
}