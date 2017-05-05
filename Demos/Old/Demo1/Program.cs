using System;

using Consul;

namespace Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var client = new ConsulClient())
            {
                var nodesResult = client.Catalog.Nodes().GetAwaiter().GetResult();
                var nodes = nodesResult.Response;
                foreach (var node in nodes)
                {
                    Console.WriteLine($"{node.Name} - {node.Address}");
                }
            }
        }
    }
}
