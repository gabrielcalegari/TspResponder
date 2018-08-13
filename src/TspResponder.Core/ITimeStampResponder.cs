using System.Threading.Tasks;
using TspResponder.Http;

namespace TspResponder
{
    public interface ITimeStampResponder
    {
        Task<TspHttpResponse> Respond(TspHttpRequest tspHttpRequest);
    }
}