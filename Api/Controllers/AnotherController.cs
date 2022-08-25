using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class AnotherController : ControllerBase
    {
        [HttpGet]
        public string Get()
        {
            return "This is the result of AnotherController";
        }
    }
}
