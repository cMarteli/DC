using System;
using System.ServiceModel;

namespace DCServer {
    internal class Program {
        static void Main() {
            /* Display a welcome message */
            Console.WriteLine("Data Server is online!");

            try {
                /* Initialize and use ServiceHost */
                using (ServiceHost host = InitializeServiceHost()) {
                    /* Start the service host */
                    StartServiceHost(host);
                }
            } catch (Exception ex) {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        /*
         * Initializes the Service Host.
         * Returns the initialized ServiceHost.
         */
        private static ServiceHost InitializeServiceHost() {
            NetTcpBinding tcp = new NetTcpBinding();
            ServiceHost host = new ServiceHost(typeof(DataServer));
            host.AddServiceEndpoint(typeof(DataServerInterface), tcp, "net.tcp://0.0.0.0:8100/DataService");
            return host;
        }

        /*
         * Starts the service host and waits for user input to stop it.
         * Takes in the service host to start as a parameter.
         */
        private static void StartServiceHost(ServiceHost host) {
            host.Open();
            Console.WriteLine("System Online");
            Console.ReadLine();
        }
    }
}
