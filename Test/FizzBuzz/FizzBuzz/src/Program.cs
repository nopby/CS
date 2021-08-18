using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace FizzBuzz
{
    class Program   
    {
        static void Main(string[] args)
        {
            string path = Path.Combine(Path.GetTempPath(), "socks.sock");
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            Server server = new Server();
            var task = Task.Run(() =>
            {
                server.Start(path);
            });
            Thread.Sleep(2000);


            Client client = new Client();
            client.Start(path);
        }
    }
}