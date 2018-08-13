using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TspResponder.AspNetCore;

namespace TspResponder.App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TspController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("The service is running...");
        }

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            var tspHttpRequest = await Request.ToTspHttpRequest();
            var tspHttpResponse = await TimeStampResponder.Respond(tspHttpRequest);
            return new TspActionResult(tspHttpResponse);
        }

        private ITimeStampResponder TimeStampResponder { get; }

        public TspController(ITimeStampResponder timeStampResponder)
        {
            TimeStampResponder = timeStampResponder;
        }
    }
}