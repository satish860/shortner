using Microsoft.AspNetCore.Mvc;
using Shortner.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shortner.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}")]
    public class ShortnerController : ControllerBase
    {
        [HttpPost("shortner")]
        public IActionResult CreateShortUrl([FromBody]string url)
        {
            var shorturl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}/{Base62Convertor.Convert(UniqueIdGenerator.GetNext())}";
            return Ok(shorturl);
        }
    }
}
