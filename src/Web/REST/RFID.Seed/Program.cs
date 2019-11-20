namespace RFID.Seed
{
    using RFID.Seed.Internal;
    using System;
    using System.Threading.Tasks;

    internal class Program
    {
        static async Task Main(string[] args)
        {
            await SeedClient.SeedAsync();
        }
    }
}
