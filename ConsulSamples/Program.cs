using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Consul;
using DnsClient;

namespace ConsulSamples
{
	public static class MainClass
	{
		public static void Main(string[] args)
		{
            DoMenu();

			Console.Write("Press ENTER to quit...");
			Console.ReadLine();
		}

        public static void DoMenu()
        {
            ShowMenu();
            var quitRequested = false;
            while (!quitRequested)
            {
                Console.Write("> ");
                var selection = Console.ReadLine();

                switch(selection.Trim().ToLowerInvariant())
                {
                    case "?":
                    case "h":
                    case "help":
                        ShowMenu();
                        break;

                    case "ak":
                    case "addkeyvalue":
                        AddKeyValue().GetAwaiter().GetResult();
                        break;

                    case "lk":
                    case "listkeys":
                        ListKeys().GetAwaiter().GetResult();
                        break;

                    case "dk":
                    case "deletekeyvalue":
                        DeleteKeyValue().GetAwaiter().GetResult();
                        break;

					case "gk":
					case "getkeyvalue":
						GetKeyValue().GetAwaiter().GetResult();
						break;
					
                    case "ds":
                    case "deleteservice":
                        DeleteService().GetAwaiter().GetResult();
                        break;

                        case "di":
                        case "deleteserviceinstance":
                        DeleteServiceInstance().GetAwaiter().GetResult();
                        break;

					case "gs":
					case "getservice":
                        GetService().GetAwaiter().GetResult();
						break;

					case "gd":
					case "getservicebydns":
						GetServiceByDns().GetAwaiter().GetResult();
						break;

					case "ln":
                    case "listnodes":
                        ListNodes().GetAwaiter().GetResult();
                        break;

					case "ls":
					case "listservices":
						ListServices().GetAwaiter().GetResult();
						break;

                    case "rs":
                    case "registerservice":
                        RegisterService().GetAwaiter().GetResult();
                        break;

					case "q":
                    case "quit":
                        quitRequested = true;
                        break;
				}
            }
        }

        public static void ShowMenu()
        {
			Console.WriteLine("Available Commands:");
            Console.WriteLine("===================");
            Console.WriteLine("ak / AddKeyValue - Adds a key/value pair to the Consul KV store.");
			Console.WriteLine("dk / DeleteKeyValue - Deletes a key/value pair from the Consul KV store.");
			Console.WriteLine("ds / DeleteService - Delete all instances of a service");
			Console.WriteLine("di / DeleteServiceInstance - Delete a specific service instance");
			Console.WriteLine("gd / GetServiceByDns - Gets DNS entries for all instances of a regustered service.");
			Console.WriteLine("gh / GetServiceHealth - Gets Health entries for all instances of a regustered service.");
			Console.WriteLine("gk / GetKeyValue - Gets a key/value pair to the Consul KV store.");
			Console.WriteLine("gs / GetService - Gets the instances of a registered service");
			Console.WriteLine("lk / ListKeys - Lists the keys currently in the Consul KV store.");
			Console.WriteLine("ln / ListNodes - Lists the nodes in the cluster");
			Console.WriteLine("ls / ListServices - Lists the services registered"); 
            Console.WriteLine("rs / RegisterService - Register a new service");
        }

        public static async Task AddKeyValue()
        {
			Console.Write("Key >");
			var keyName = Console.ReadLine();

            Console.Write("Value >");
            var dataValue = Console.ReadLine();

			using (var client = new Consul.ConsulClient())
			{
                var data = Encoding.UTF8.GetBytes(dataValue);
                var result = await client.KV.Put(new KVPair(keyName) { Value = data });
				Console.WriteLine($" - {result.StatusCode}");
			}
		}

        public static async Task GetKeyValue()
        {
			Console.Write("Key >");
			var keyName = Console.ReadLine();

            using (var client = new Consul.ConsulClient())
            {
                var result = await client.KV.Get(keyName);
                Console.WriteLine($" - {Encoding.UTF8.GetString(result.Response.Value)}");
            }
        }

        public static async Task ListKeys()
        {
            using (var client = new Consul.ConsulClient())
			{
                var nodesResult = await client.KV.List(string.Empty);
				var nodes = nodesResult.Response;

				foreach (var node in nodes)
				{
                    Console.WriteLine($"{node.Key}");
				}
			}            
        }

        public static async Task DeleteKeyValue()
        {
			Console.Write("Key >");
			var keyName = Console.ReadLine();

			using (var client = new Consul.ConsulClient())
			{
                var result = await client.KV.Delete(keyName);
				Console.WriteLine($" - {result.StatusCode}");
			}

		}

        public static async Task DeleteService()
        {
            Console.Write("Service >");
            var serviceName = Console.ReadLine();

            using (var client = new Consul.ConsulClient())
            {
				var serviceResult = await client.Catalog.Service(serviceName);
				var service = serviceResult.Response;

				foreach (var serviceInstance in service)
                {
                    var result = await client.Agent.ServiceDeregister(serviceInstance.ServiceID);
                    Console.WriteLine($" - {serviceInstance.ServiceID} - {result.StatusCode}");
                }
            }
        }

        public static async Task DeleteServiceInstance()
        {
            Console.Write("Service Id >");
            var serviceId = Console.ReadLine();

            using (var client = new Consul.ConsulClient())
			{
				var result = await client.Agent.ServiceDeregister(serviceId);
				Console.WriteLine($" - {result.StatusCode}");
			}
        }

        public static async Task RegisterService()
        {
            Console.Write("Service >");
            var serviceName = Console.ReadLine();
            Console.Write("Address >");
            var serviceAddress = Console.ReadLine();
            Console.Write("Port >");
            var servicePort = Convert.ToInt16(Console.ReadLine());

			var registration = new AgentServiceRegistration()
			{
				ID = $"{serviceName}-{Guid.NewGuid()}",
				Name = serviceName,
				Address = $"http://{serviceAddress}",
				Port = servicePort,
				Tags = new[] { "Flibble", "Wotsit", "Aardvark" }
			};

            using (var client = new Consul.ConsulClient())
            {
                var result = await client.Agent.ServiceRegister(registration);
                Console.WriteLine($" - {result.StatusCode}");
            }
        }

		public static async Task ListNodes()
		{
			using (var client = new Consul.ConsulClient())
			{
				var nodesResult = await client.Catalog.Nodes();
				var nodes = nodesResult.Response;

				foreach (var node in nodes)
				{
					Console.WriteLine($"{node.Name} - {node.Address}");
				}
			}
		}

		public static async Task ListServices()
		{
			using (var client = new Consul.ConsulClient())
			{
                var servicesResult = await client.Catalog.Services();
				var services = servicesResult.Response;

				foreach (var service in services)
				{
                    var serviceData = service.Value.Length > 0 ? service.Value.Aggregate((current, next) => current + ", " + next) : string.Empty;
                    Console.WriteLine($"{service.Key} - {serviceData}");
				}
			}
		}

        public static async Task GetService()
        {
            Console.Write("Service > ");
            var serviceName = Console.ReadLine();

            using (var client = new Consul.ConsulClient())
            {
                var serviceResult = await client.Catalog.Service(serviceName);
                var service = serviceResult.Response;

                foreach (var serviceInstance in service)
                {
                    Console.WriteLine($"{serviceInstance.ServiceID} / {serviceInstance.Address} - {serviceInstance.ServiceAddress}:{serviceInstance.ServicePort} ({serviceInstance.ServiceName})");
                }
            }
        }

		public static async Task GetServiceHealth()
		{
			Console.Write("Service > ");
			var serviceName = Console.ReadLine();

			using (var client = new Consul.ConsulClient())
			{
                var serviceResult = await client.Health.Service(serviceName);
				var service = serviceResult.Response;

				foreach (var serviceInstance in service)
				{
                    Console.WriteLine($"{serviceInstance.Service.ID} / {serviceInstance.Service.Address}");
                    foreach (var check in serviceInstance.Checks)
                    {
                        Console.WriteLine($" - {check.CheckID} - {check.Status}");
                    }
				}
			}
		}
        public static async Task GetServiceByDns()
        {
			Console.Write("Service > ");
			var serviceName = Console.ReadLine();

            var dnsName = serviceName.Trim().ToLowerInvariant() + ".service.consul";

			var endpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8600);
			var client = new LookupClient(endpoint);

            var queryResult = await client.QueryAsync(dnsName, QueryType.A);

            if (queryResult.AllRecords.Any())
            {
                foreach (var result in queryResult.AllRecords)
                {
                    Console.WriteLine($"{result.ToString()}");
                }
            }
        }
	}
}
