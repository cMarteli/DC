using IChatServerInterfaceDLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Set up ChatServer
            Console.WriteLine("Server connection established");
            ServiceHost host;
            NetTcpBinding tcp = new NetTcpBinding();

            host = new ServiceHost(typeof(ChatServer));
            host.AddServiceEndpoint(typeof(IChatServerInterface), tcp,
                    "net.tcp://0.0.0.0:8100/ChatService");
            host.Open();

            Console.WriteLine("System Online");
            Console.ReadLine();

            host.Close();
        }
    }
}
