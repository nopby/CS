using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FizzBuzz
{
    public class Client
    {
        /*
        public List<string> ReadLine()
        {
            List<string> lines = new List<string>();
            while (true)
            {
                Console.WriteLine("Input: ");
                string line = Console.ReadLine();
                if (line.Contains("\\n"))
                {
                    line.Replace("\\n", string.Empty);
                    lines.Add(line);
                    break;
                }
                else
                {
                    lines.Add(line);
                }

            }
            return lines;
        }
        */
        public string ReadLine()
        {
            StringBuilder sb = new StringBuilder();
            while (true)
            {
                Console.Write("Input: ");
                string line = Console.ReadLine();
                sb.AppendLine(line);
                if (line.Contains("\\n"))
                {
                    break;
                }
                if (line.Contains("exit"))
                {
                    break;
                }

            }
            sb.Replace("\\n", " ");
            return sb.ToString();
        }
        public void Start(String path)
        {
            var endPoint = new UnixDomainSocketEndPoint(path);
            try
            {
                using (var client = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.Unspecified))
                {
                    client.Connect(endPoint);
                    Console.WriteLine($"[Client] Connected to … ..{path}");
                    string str = string.Empty;
                    var bytes = new byte[1024];
                    while (true)
                    {
                        
                        Console.WriteLine("[Client] Send JSON string (terminate string with \\n or send 'exit' to terminate command) \n");
                        str = ReadLine();
                        if (str.Contains("exit"))
                        {
                            break;
                        }
                        client.Send(Encoding.UTF8.GetBytes(str));

                        int byteRecv = client.Receive(bytes);
                        string strByte = Encoding.UTF8.GetString(bytes, 0, byteRecv);
                        Console.WriteLine($"[Client] GET: {strByte}  ");
                    }
                }
            }
            finally
            {
                try { File.Delete(path); }
                catch { }
            }
        }
    }
}
