using System;
using SecretSanta.TelegramBase;

namespace SecretSanta
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(StringConstants.StartAppMsg);
            SantaClient client = new SantaClient();
            client.Start();
            Console.ReadLine();
        }
    }
}