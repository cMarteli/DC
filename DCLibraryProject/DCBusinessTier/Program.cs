using System;
using System.ServiceModel;

namespace DCBusinessTier {
    internal class Program {
        static void Main() {
            /* Display a welcome message */
            Console.WriteLine("Welcome to the business server!");

            try {
                /* Initialize and use ServiceHost within a 'using' block for automatic cleanup */
                using (ServiceHost host = InitializeServiceHost()) {
                    /* Start the service host and wait for user input to stop it */
                    StartServiceHost(host);
                }
            } catch (Exception ex) {
                Console.WriteLine($"An error occured: {ex.Message}");
            }
        }

        /*
         * Initialize the ServiceHost.
         * Return the initialized ServiceHost.
         */
        private static ServiceHost InitializeServiceHost() {
            NetTcpBinding tcp = new NetTcpBinding();
            ServiceHost host = new ServiceHost(typeof(BusinessServer));
            host.AddServiceEndpoint(typeof(BusinessServerInterface), tcp, "net.tcp://0.0.0.0:8200/BusinessService");
            return host;
        }

        /*
         * Start the service host and wait for user input to stop it.
         * Takes in the service host to start as a parameter.
         */
        private static void StartServiceHost(ServiceHost host) {
            host.Open();
            Console.WriteLine("System Online");
            Console.ReadLine();
        }
    }
}
