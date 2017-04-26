using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    public class HealthController : Controller
    {
        private static bool IsHealthy = true;

        // GET api/values
        [HttpGet("status")]
        public IActionResult Status()
        {
            return IsHealthy ? this.Ok() : this.StatusCode((int)HttpStatusCode.ServiceUnavailable);
        }

        [HttpGet("setstatus/{isHealthy}")]
        public StatusCodeResult SetStatus(bool isHealthy)
        {
            IsHealthy = isHealthy;
            return this.Ok();
        }
    }
}
