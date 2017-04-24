using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyService.Controllers
{
    [Route("/api/values")]
    public class ValuesController : Controller
    {
        private static Random Randomiser = new System.Random(DateTime.UtcNow.Millisecond);

        [HttpGet]
        public ActionResult Single()
        {
            var resultValue = new
            {
                Id = Guid.NewGuid().ToString(),
                Server = Environment.MachineName,
                Value = Randomiser.Next(256)
            };

            return Json(resultValue);
        }
    }
}
