using System;
using System.Net.Http;
using RestTemplateCore;

namespace RestTemplateCoreTest
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var http = new HttpClient())
            {
                SendSms sms = new SendSms
                {
                    Msg = "hello",
                    PhoneNum = "188"
                };
                RestTemplate rest = new RestTemplate(http);
                rest.ConsulServerUrl = "http://127.0.0.1:8500";
                var result = rest.PostAsync("http://MsgService/api/SMS/Send_MI", sms).Result;
                Console.WriteLine(result.StatusCode);

                var getResult = rest.GetForEntityAsync<Product[]>("http://ProductService/api/Product").Result;
                foreach(var g in getResult.Body)
                {
                    Console.WriteLine(g.Id+" "+g.Name);
                }

                var getResult2 = rest.GetForEntityAsync<Product>("http://ProductService/api/Product/1").Result;
                Console.WriteLine(getResult2.Body.Name);
            }


            Console.ReadKey();
        }
    }

    class SendSms
    {
        public string PhoneNum { get; set; }

        public string Msg { get; set; }
    }

    class Product
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }
    }
}
