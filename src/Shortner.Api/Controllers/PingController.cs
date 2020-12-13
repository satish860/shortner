using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shortner.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/ping")]
    public class PingController : ControllerBase
    {
        public IActionResult Get()
        {
            return Ok("I am alive");
        }
    }
}
