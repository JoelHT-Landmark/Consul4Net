using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace WebApi
{
    public class Program
    {
        private static Random Randomiser = new Random(DateTime.UtcNow.Millisecond);

        public static void Main(string[] args)
        {
            var portNumber = 5601; // + Randomiser.Next(100);


            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseUrls($"http://localhost:{portNumber}")
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();
			host.Run();
        }
    }
}
