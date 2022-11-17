using System;
using System.Net.Http;
using System.Threading.Tasks;
using Common.Messages;
using EasyNetQ;

namespace FirstApp
{
    public static class Program
    {
        private const int FIBONACCI_FIRST = 1;
        private const string FIBONACCI_API = "http://localhost:8080/api/fibonacci/";

        public static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Starting application");
                if (args.Length != 1 || !int.TryParse(args[0], out int parallelThreadsNumber)) return;

                using (var bus = RabbitHutch.CreateBus("host=localhost"))
                using (var httpClient = new HttpClient())
                {
                    Console.WriteLine("Initializing Message Bus");
                    bus.PubSub.Subscribe<FibonacciMessage>("fibonacci_id", msg =>
                    {
                        return Task.Run(() =>
                        {
                            Console.WriteLine($"Number received: {msg.Number}");
                            httpClient.GetAsync($"{FIBONACCI_API}{msg.Number}");

                        });
                    });

                    Console.WriteLine("Press Enter when the SecondApp is ready");
                    Console.ReadLine();

                    for (int i = 0; i < parallelThreadsNumber; i++)
                    {
                        httpClient.GetAsync($"{FIBONACCI_API}{FIBONACCI_FIRST}");
                    }

                    Console.WriteLine("Press any key to exit");
                    Console.ReadKey();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occured: {ex.Message}");
            }
        }
    }
}
