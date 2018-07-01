using Consul;
using System;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace 服务消费者1
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var consul = new ConsulClient(c =>
            {
                c.Address = new Uri("http://127.0.0.1:8500");
            }))
            {
                //var services = consul.Agent.Services().Result.Response;
                //foreach (var item in services.Values)
                //{
                //    Console.WriteLine($"id={item.ID},service={item.Service},addr={item.Address},port={item.Port}");
                //}

                //客户端负载均衡
                var services = consul.Agent.Services().Result.Response.Values.Where(s => s.Service.Equals("MsgService", StringComparison.OrdinalIgnoreCase));
                int index = new Random().Next(services.Count());
                var service = services.ElementAt(index);
                Console.WriteLine($"index={index},id={service.ID},service={service.Service},addr={service.Address},port={service.Port}");

                using (HttpClient http = new HttpClient())
                {
                    using (var httpContent = new StringContent("{phoneNum:'110',msg:'help'}", Encoding.UTF8, "application/json"))
                    {
                        var result = http.PostAsync($"http://{service.Address}:{service.Port}/api/SMS/Send_LX", httpContent).Result;
                        Console.WriteLine(result.StatusCode);
                    }
                }
            }
            Console.ReadKey();

        }
    }
}
