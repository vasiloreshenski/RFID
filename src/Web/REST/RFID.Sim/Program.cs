namespace RFID.Sim
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;

    internal class Program
    {
        static async Task Main(string[] args)
        {
            ServicePointManager.DefaultConnectionLimit = 120;

            var avg = await StandardAsync();
            Console.WriteLine(avg);
        }


        private static async Task<(long avgAcross, long avgSingle)> StandardAsync()
        {
            var timesInMs = new List<long>();
            var timeSinglInMs = new List<long>();
            var timer = new Stopwatch();
            
            for (int i = 0; i < 60; i++)
            {
                timer.Restart();
                timer.Start();

                var tasks = new List<Task<long>>();
                for (int y = 0; y < 200; y++)
                {
                    tasks.Add(CheckAccessAsync());
                }

                Task.WaitAll(tasks.ToArray());
                
                timer.Stop();
                timesInMs.Add(timer.ElapsedMilliseconds);

                timeSinglInMs.AddRange(tasks.Select(x => x.Result));

                await Task.Delay(TimeSpan.FromSeconds(1));
            }

            return (timesInMs.Sum() / 60, (long)timeSinglInMs.Average());
        }


        private static async Task<long> CheckAccessAsync()
        {
            using (var http = new HttpClient())
            {
                var timer = Stopwatch.StartNew();
                // await http.GetAsync("https://desktop-sb3j0h0/accesscontrol/api/tags/checkaccess?tagNumber=43360620-050e-439d-9921-d83c0802cbf1&accessPointSerialNumber=d38fabb5-b82a-4426-ba06-878272a119db");
                await http.GetAsync("http://192.168.0.105:8080/accesscontrol/api/tags/checkaccess?tagNumber=43360620-050e-439d-9921-d83c0802cbf1&accessPointSerialNumber=d38fabb5-b82a-4426-ba06-878272a119db");
                timer.Stop();
                return timer.ElapsedMilliseconds - 3; // 3 ms is from the network
            }
        }
    }
}
