using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TspResponder.Http;

namespace TspResponder.AspNetCore
{
    /// <inheritdoc />
    public class TspActionResult : IActionResult
    {
        /// <inheritdoc />
        public async Task ExecuteResultAsync(ActionContext context)
        {
            var contentResult = new FileContentResult(TspHttpResponse.Content, TspHttpResponse.MediaType);
            await contentResult.ExecuteResultAsync(context);
        }

        /// <summary>
        /// A <see cref="TspHttpResponse"/> from TspResponder
        /// </summary>
        private TspHttpResponse TspHttpResponse { get; }

        /// <summary>
        /// Creates a <see cref="IActionResult"/> for Timestamp responses
        /// </summary>
        /// <param name="tspHttpResponse"><see cref="TspHttpResponse"/></param>
        public TspActionResult(TspHttpResponse tspHttpResponse)
        {
            TspHttpResponse = tspHttpResponse;
        }
    }
}
