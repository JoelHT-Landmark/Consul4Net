using System;
using System.Threading.Tasks;

namespace ShowNodes
{
    public static class MainClass
    {
        public static void Main(string[] args)
        {
            ShowNodes().GetAwaiter().GetResult();

            Console.Write("Press ENTER to quit...");
            Console.ReadLine();
        }

        public static async Task ShowNodes()
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
    }
}
