using System;
using System.Collections.Generic;
using System.ServiceModel;
using ClientDesktop.Models;
using ClientDesktop.Services;

public class JobServiceHost {

    private static IJobService _jobService;
    private readonly ServiceHost _serviceHost;
    private readonly int _port;
    private List<Client> _peerClients = new List<Client>(); // List of peer clients

    public JobServiceHost(int port) {
        _jobService = new JobService();
        _port = port;
        _serviceHost = new ServiceHost(_jobService);
        try {
            _serviceHost.AddServiceEndpoint(typeof(IJobService), new NetTcpBinding(), $"net.tcp://localhost:{_port}/jobs");
            _serviceHost.Open();
        } catch (Exception ex) {
            Console.WriteLine($"JobServiceHost: An error occurred while starting the service: {ex.Message}");
        }
    }

    public void StopService() {
        _serviceHost?.Close();
        Console.WriteLine("StopService: Job Service System Offline");
    }

    public void UpdatePeerList(List<Client> inList) {
        _peerClients = inList;
    }

    public void CheckPeerServiceStatus() {
        ChannelFactory<IJobService> channelFactory = new ChannelFactory<IJobService>(new NetTcpBinding());
        foreach (Client peer in _peerClients) {
            channelFactory.Endpoint.Address = new EndpointAddress($"net.tcp://{peer.IPAddress}:{peer.Port}/jobs");
            IJobService proxy = channelFactory.CreateChannel();
            try {
                if (proxy.Ping()) {
                    Console.WriteLine($"Service is running on {peer.IPAddress}:{peer.Port}/jobs");
                }
            } catch (Exception e) {
                Console.WriteLine($"CheckPeerServiceStatus: Cannot reach service on {peer.IPAddress}:{peer.Port}. Exception: {e.Message}");
                ((IClientChannel)proxy).Abort();  // Abort the proxy channel in case of an exception
            } finally {
                ((IClientChannel)proxy).Close();  // Close the proxy channel
            }
        }
        channelFactory.Close();  // Close the channel factory after the loop
    }


    public void PostJob(Job job) {
        _jobService.EnqueueJob(job);
    }

    public Job DequeueJobFromPeers(int port) {
        using (ChannelFactory<IJobService> channelFactory = new ChannelFactory<IJobService>(new NetTcpBinding())) {
            foreach (Client peer in _peerClients) {
                channelFactory.Endpoint.Address = new EndpointAddress($"net.tcp://{peer.IPAddress}:{peer.Port}/jobs");
                IJobService proxy = channelFactory.CreateChannel();
                IClientChannel channel = (IClientChannel)proxy;

                try {
                    if (proxy.HasJob(port)) {
                        Job job = proxy.DequeueJob();
                        return job;
                    }
                } catch (TimeoutException e) {
                    Console.WriteLine($"Timeout: {e.Message}");
                    channel.Abort();
                } catch (CommunicationException e) {
                    Console.WriteLine($"Communication Error: {e.Message}");
                    channel.Abort();
                } catch (Exception e) {
                    Console.WriteLine($"General Error: {e.Message}");
                    channel.Abort();
                } finally {
                    channel.Close();
                }
            }
        }

        throw new Exception("DequeueJobFromPeers: No job found");
    }

    public void SubmitJobResultToOwner(Job job) {
        int ownerPort = job.Owner;

        // Find the owner among the peer clients.
        Client ownerClient = _peerClients.Find(client => client.Port == ownerPort);

        if (ownerClient != null) {
            using (ChannelFactory<IJobService> channelFactory = new ChannelFactory<IJobService>(
                new NetTcpBinding(),
                new EndpointAddress($"net.tcp://{ownerClient.IPAddress}:{ownerClient.Port}/jobs"))) {
                IJobService proxy = channelFactory.CreateChannel();
                IClientChannel channel = (IClientChannel)proxy;

                try {
                    proxy.SubmitJobResult(job);
                    Console.WriteLine($"SubmitJobResultToOwner: {ownerClient.IPAddress}:{ownerClient.Port}");
                } catch (CommunicationException e) {
                    Console.WriteLine($"Communication Error: {e.Message}");
                    channel.Abort();
                } catch (TimeoutException e) {
                    Console.WriteLine($"Timeout: {e.Message}");
                    channel.Abort();
                } catch (Exception e) {
                    Console.WriteLine($"SubmitJobResultToOwner: Failed to submit job {ownerClient.IPAddress}:{ownerClient.Port}. Exception: {e.Message}");
                    channel.Abort();
                } finally {
                    channel.Close();
                }
            }
        }
        else {
            Console.WriteLine($"SubmitJobResultToOwner: Owner for the job result not found. Owner port: {ownerPort}");
        }
    }

    public string GetResult(Guid jobId) {
        return _jobService.GetResult(jobId);
    }
}
