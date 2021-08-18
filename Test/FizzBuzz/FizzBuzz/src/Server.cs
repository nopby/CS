using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FizzBuzz
{
    public class Data
    {
        public List<dynamic> result;
    }
    public class CustomNamesContractResolver : DefaultContractResolver {

        public string Name;
        public CustomNamesContractResolver(string name)
        {
            this.Name = name;
        }
        protected override IList<JsonProperty> CreateProperties(System.Type type, MemberSerialization memberSerialization)
        {
            // Let the base class create all the JsonProperties 
            // using the short names
            IList<JsonProperty> list = base.CreateProperties(type, memberSerialization);

            // Now inspect each property and replace the 
            // short name with the real property name
            foreach (JsonProperty prop in list)
            {
                if (prop.UnderlyingName == "result") //change this to your implementation!
                    prop.PropertyName = this.Name;

            }

            return list;
        }
    }
    public class Server
    {

        public void Start(String path)
        {
            var endPoint = new UnixDomainSocketEndPoint(path);
            try
            {
                using (var server = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.Unspecified))
                {
                    server.Bind(endPoint);
                    Console.WriteLine($"[Server] Listening … ..{path}");
                    server.Listen(1);
                    using (Socket accepted = server.Accept())
                    {
                        Console.WriteLine("[Server] Connection Accepted …" + accepted.RemoteEndPoint.ToString());
                        var bytes = new byte[1024];
                        while (true)
                        {
                            // Receive
                            int byteRecv = accepted.Receive(bytes);
                            String str = Encoding.UTF8.GetString(bytes, 0, byteRecv);
                            Console.WriteLine($"[Server] GET: {str}");
                            try
                            {
                                // Convert
                                var buffer = JsonConvert.DeserializeObject<dynamic>(str);
                                Data data = new Data();
                                data.result = new List<dynamic>();
                                for (var i = (int)buffer.from; i < (int) buffer.to; i++)
                                {
                                    if (i % 3 == 0 && i % 5 == 0)
                                    {
                                        string sum = buffer.fizz + buffer.buzz;
                                        data.result.Add(sum);
                                    }
                                    else if (i % 3 == 0)
                                    {
                                        string sum = buffer.fizz;
                                        data.result.Add(sum);
                                    }
                                    else if (i % 5 == 0)
                                    {
                                        string sum = buffer.buzz;
                                        data.result.Add(sum);
                                    }
                                    else
                                    {
                                        int sum = i;
                                        data.result.Add(sum);
                                    }
                                }
                                JsonSerializerSettings settings = new JsonSerializerSettings();
                                settings.ContractResolver = new CustomNamesContractResolver((string)buffer.id);
                                var json = JsonConvert.SerializeObject(data, settings);

                                var newBuffer = JObject.Parse(json).ToString(Formatting.None);

                                Console.WriteLine($"[Server] Send: {newBuffer}\\n");
                                // Send
                                accepted.Send(Encoding.UTF8.GetBytes(newBuffer + "\\n"));
                           
                            } catch (Exception e)
                            {
                                string err = "Failed to initilize JSON string format";
                                Console.WriteLine($"[Server] {err}");
                                Console.WriteLine(e);
                                accepted.Send(Encoding.UTF8.GetBytes(err));
                            }
                            
                        }

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
