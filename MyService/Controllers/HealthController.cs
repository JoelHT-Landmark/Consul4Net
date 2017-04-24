using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Metrics;

namespace MyService.Controllers
{
    [Route("/healthcheck")]
    public class HealthController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
			var result = HealthChecks.GetStatus();

            return View ("~/Views/Health.aspx", result);
        }

        [HttpGet]
        public ActionResult Simple()
        {
            var result = HealthChecks.GetStatus();

            if (result.IsHealthy)
            {
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }

            Response.Status = HttpStatusCode.ServiceUnavailable.ToString();
            return Json(result.Results);
        }
    }
}
