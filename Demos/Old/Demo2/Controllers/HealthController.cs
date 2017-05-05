using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Consul;

using Microsoft.AspNetCore.Mvc;

namespace Demo2.Controllers
{
    [Route("health")]
    public class HealthController : Controller
    {
        [HttpGet("status")]
        public ActionResult Status()
        {
            return this.IsHealthy() ? this.Ok() : this.StatusCode(503);
        }

        private bool IsHealthy()
        {
            using (var client = new ConsulClient()) {
                var kvResult = client.KV.Get("WebApi.ServiceHealth").GetAwaiter().GetResult();
                if ((kvResult == null) || (kvResult.Response == null))
                {
                    return true;
                }

                var healthState = Encoding.UTF8.GetString(kvResult.Response.Value);
                var isHealthy = Convert.ToBoolean(healthState);

                return isHealthy;
            }
        }
    }
}