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
            Console.WriteLine("Job Service System Online");
        } catch (Exception ex) {
            Console.WriteLine($"An error occurred while starting the service: {ex.Message}");
        }
    }

    public void StopService() {
        _serviceHost?.Close();
        Console.WriteLine("Job Service System Offline");
    }

    public void UpdatePeerList(List<Client> inList) {
        _peerClients = inList;
    }

    public void CheckPeerServiceStatus() {
        foreach (Client peer in _peerClients) {
            // Create a channel
            ChannelFactory<IJobService> channelFactory = new ChannelFactory<IJobService>(new NetTcpBinding(), new EndpointAddress($"net.tcp://{peer.IPAddress}:{peer.Port}/jobs"));
            IJobService proxy = channelFactory.CreateChannel();

            try {
                if (proxy.Ping()) {
                    Console.WriteLine($"Service is running on {peer.IPAddress}:{peer.Port}");
                }
            } catch (Exception e) {
                Console.WriteLine($"Cannot reach service on {peer.IPAddress}:{peer.Port}. Exception: {e.Message}");
            }

            // Close the channel
            channelFactory.Close();
        }
    }

    public void EnqueueJob(Job job) {
        _jobService.EnqueueJob(job);
    }

    public bool HasJob(int port) {
        if (_jobService.HasJob(port)) {
            return true;
        }
        return false;
    }

    public Job DequeueJobFromPeers(int port) {

        foreach (Client peer in _peerClients) {
            ChannelFactory<IJobService> channelFactory = new ChannelFactory<IJobService>(
                new NetTcpBinding(),
                new EndpointAddress($"net.tcp://{peer.IPAddress}:{peer.Port}/jobs"));
            IJobService proxy = channelFactory.CreateChannel();

            try {
                Job job = proxy.DequeueJob();  // try to dequeue job from peer
                if (job != null) {
                    channelFactory.Close();
                    return job;
                }
            } catch (Exception e) {
                Console.WriteLine($"Host: {port}: {e.Message}");
            }

            channelFactory.Close();  // close the channel
        }
        Console.WriteLine($"Returning NULL JOB");
        return null;  // return null if no job could be dequeued
    }

    public void SubmitJobResultToOwner(Job job) {
        int ownerPort = job.Owner;

        // Find the owner among the peer clients.
        Client ownerClient = _peerClients.Find(client => client.Port == ownerPort);

        if (ownerClient != null) {
            ChannelFactory<IJobService> channelFactory = new ChannelFactory<IJobService>(
                new NetTcpBinding(),
                new EndpointAddress($"net.tcp://{ownerClient.IPAddress}:{ownerClient.Port}/jobs"));
            IJobService proxy = channelFactory.CreateChannel();

            try {
                proxy.SubmitJobResult(job);
                Console.WriteLine($"Job result submitted to owner {ownerClient.IPAddress}:{ownerClient.Port}");
            } catch (Exception e) {
                Console.WriteLine($"Failed to submit job result to owner {ownerClient.IPAddress}:{ownerClient.Port}. Exception: {e.Message}");
            }

            channelFactory.Close();  // Close the channel.
        }
        else {
            Console.WriteLine($"Owner for the job result not found. Owner port: {ownerPort}");
        }
    }

    public string GetResult(Guid jobId) {
        return _jobService.GetResult(jobId);
    }
}
