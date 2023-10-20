using System;
using System.ServiceModel;
using ClientDesktop.Services;

namespace ClientDesktop {
    public class JobServiceHostManager : IDisposable {
        private ServiceHost _serviceHost;
        private readonly int _port;

        public JobServiceHostManager(int port) {
            _port = port;
        }

        public void StartService() {
            try {
                NetTcpBinding tcp = new NetTcpBinding();
                _serviceHost = new ServiceHost(typeof(JobService));
                _serviceHost.AddServiceEndpoint(typeof(IJobService), tcp, $"net.tcp://localhost:{_port}/jobService");
                _serviceHost.Open();
                Console.WriteLine("Job Service System Online");
            } catch (Exception ex) {
                Console.WriteLine($"An error occurred while starting the service: {ex.Message}");
            }
        }

        public JobService GetServiceHost() {
            return _serviceHost?.SingletonInstance as JobService;
        }

        public void StopService() {
            _serviceHost?.Close();
            Console.WriteLine("Job Service System Offline");
        }

        public void Dispose() {
            StopService();
        }
    }
}
