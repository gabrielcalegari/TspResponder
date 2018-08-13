using System.Net.Http;
using System.Threading.Tasks;
using TspResponder.Http;

namespace TspResponder.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="HttpRequestMessage"/>
    /// </summary>
    public static class HttpRequestExtensions
    {
        /// <summary>
        /// Converts the <see cref="HttpRequestMessage"/> to <see cref="TspHttpRequest"/>
        /// </summary>
        /// <param name="requestMessage"><see cref="HttpRequestMessage"/></param>
        /// <returns><see cref="TspHttpRequest"/></returns>
        public static async Task<TspHttpRequest> ToTspHttpRequest(this HttpRequestMessage requestMessage)
        {
            var tspHttpRequest = new TspHttpRequest
            {
                MediaType = requestMessage.Content.Headers.ContentType.MediaType,
                Content = await requestMessage.Content.ReadAsByteArrayAsync()
            };

            return tspHttpRequest;
        }
    }
}
