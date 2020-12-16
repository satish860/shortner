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
        private readonly IUniqueIdGenerator uniqueIdGenerator;
        private readonly IUrlRepository urlRepository;

        public ShortnerController(IUniqueIdGenerator uniqueIdGenerator,IUrlRepository urlRepository)
        {
            this.uniqueIdGenerator = uniqueIdGenerator;
            this.urlRepository = urlRepository;
        }

        [HttpPost("shortner")]
        public async Task<IActionResult> CreateShortUrl([FromBody]string url)
        {
            bool isUri = Uri.IsWellFormedUriString(url, UriKind.Absolute);
            if(!isUri)
            {
                return BadRequest("Not a valid URL!!! Come on you can do better");
            }
            var id = await this.uniqueIdGenerator.GetNext();
            var shorturl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}/{Base62Convertor.Convert(id)}";
            var persistanceStatus = await this.urlRepository.SaveUrl(id, url);
            if(persistanceStatus == false)
            {
                throw new Exception("Sorry!! We have some temporary down time. We request to retry after sometime"); 
            }
            return Ok(shorturl);
        }
    }
}
