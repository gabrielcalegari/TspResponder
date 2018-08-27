using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TspResponder.Core.Http;

namespace TspResponder.AspNetCore
{
    /// <summary>
    /// Set of extension methods for <see cref="HttpRequest"/>
    /// </summary>
    public static class HttpRequestExtensions
    {
        /// <summary>
        /// Converts <see cref="HttpRequest"/> to <see cref="TspHttpRequest"/>
        /// </summary>
        /// <param name="request"><see cref="HttpRequest"/></param>
        /// <returns><see cref="TspHttpRequest"/></returns>
        public static async Task<TspHttpRequest> ToTspHttpRequest(this HttpRequest request)
        {
            var tspHttpRequest = new TspHttpRequest
            {
                MediaType = request.ContentType,
                Content = await request.GetRawBodyBytesAsync()
            };

            return tspHttpRequest;
        }

        /// <summary>
        /// Retrieves the raw body as a byte array from the Request.Body stream
        /// </summary>
        /// <param name="request">The <see cref="HttpRequest"/></param>
        /// <returns></returns>
        private static async Task<byte[]> GetRawBodyBytesAsync(this HttpRequest request)
        {
            using (var ms = new MemoryStream(2048))
            {
                await request.Body.CopyToAsync(ms);
                return ms.ToArray();
            }
        }
    }
}
