using System.Threading.Tasks;
using TspResponder.Core.Http;

namespace TspResponder.Core
{
    public interface ITimeStampResponder
    {
        Task<TspHttpResponse> Respond(TspHttpRequest tspHttpRequest);
    }
}