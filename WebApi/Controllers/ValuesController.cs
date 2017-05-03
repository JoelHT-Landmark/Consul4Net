using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using Consul;

using DnsClient;
using DnsClient.Protocol;

using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    
    public class ValuesController : Controller
    {
        private static Random Randomiser = new Random(DateTime.UtcNow.Millisecond);

        private static string[] GameValues = { 
            "Rock", "Paper", "Scissors", "Lizard", "Spock"
        };

        [Route("Values")]
        [HttpGet]
        public async Task<string> Get()
        {
            return GameValues[Randomiser.Next(GameValues.Length)];
        }

        [Route("Values/All")]
        [HttpGet]

        public async Task<IEnumerable<string>> All()
        {
            var result = new List<string>();
            var endpoints = await GetServiceUrls();

			using (var client = new HttpClient())
			{
                foreach (var endpoint in endpoints)
                {
                    var response = await client.GetAsync(endpoint);
                    var endpointValue = await response.Content.ReadAsStringAsync();
                    result.Add($"{endpoint} - {endpointValue}");
                }
            }

            return result;
        }

        private async Task<IEnumerable<string>> GetServiceUrls()
        {
            var serviceName = "webapi";

            using (var client = new Consul.ConsulClient())
            {
                var serviceResult = await client.Catalog.Service(serviceName);
                var service = serviceResult.Response;

                var result = new List<string>();
                foreach (var serviceInstance in service)
                {
                    result.Add($"{serviceInstance.ServiceAddress}:{serviceInstance.ServicePort}/Values");
                }

                return result;
            }
        }

        private async Task<IEnumerable<DnsResourceRecord>> GetServiceEndpoints()
        {
            var dnsName = "webapi.service.consul";

            var endpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8600);
            var client = new LookupClient(endpoint);

            var queryResult = await client.QueryAsync(dnsName, QueryType.A);

            var results = new List<DnsResourceRecord>();
            if (queryResult.AllRecords.Any())
            {
                foreach (var result in queryResult.AllRecords)
                {
                    results.Add(result);
                }
            }

            return results;
        }
    }
}
