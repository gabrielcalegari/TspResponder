using System.Net;
using System.Net.Http;
using TspResponder.Http;

namespace TspResponder.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="TspHttpResponse"/>
    /// </summary>
    public static class HttpResponseExtensions
    {
        /// <summary>
        /// Converts the <see cref="TspHttpResponse"/> to <see cref="HttpResponseMessage"/>
        /// </summary>
        /// <param name="tspHttpResponse"><see cref="TspHttpResponse"/></param>
        /// <returns><see cref="HttpResponseMessage"/></returns>
        public static HttpResponseMessage ToHttpResponseMessage(this TspHttpResponse tspHttpResponse)
        {
            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(tspHttpResponse.Content)
            };

            httpResponseMessage.Content.Headers.ContentType.MediaType = tspHttpResponse.MediaType;
            return httpResponseMessage;
        }
    }
}
