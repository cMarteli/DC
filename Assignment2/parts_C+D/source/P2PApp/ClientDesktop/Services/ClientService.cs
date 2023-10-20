using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using ClientDesktop.Models;
using Newtonsoft.Json;
using RestSharp;

namespace ClientDesktop.Services {
    public class ClientService {
        private const string BaseUrl = "http://localhost:5006/api/client";  // API endpoint
        private readonly RestClient _client;

        public ClientService() {
            _client = new RestClient(BaseUrl);  // Consider using Dependency Injection in real-world applications
        }

        private RestResponse ExecuteRequest(RestRequest request) {
            return (RestResponse)_client.Execute(request);
        }

        private async Task<RestResponse> ExecuteRequestAsync(RestRequest request) {
            return (RestResponse)await _client.ExecuteAsync(request);
        }

        private bool CheckResponse(RestResponse response) {
            if (response.StatusCode == System.Net.HttpStatusCode.OK) {
                return true;
            }
            // TODO: Add more error handling logic here
            return false;
        }

        public bool RegisterClient(string ipAddress, int port) {
            var request = new RestRequest("new", Method.Post);
            request.AddJsonBody(new { IPAddress = ipAddress, Port = port });

            RestResponse response = ExecuteRequest(request);
            return CheckResponse(response);
        }

        public bool UnregisterClient(string ipAddress, int port) {
            var request = new RestRequest($"remove/{ipAddress}/{port}", Method.Delete);

            RestResponse response = ExecuteRequest(request);
            return CheckResponse(response);
        }


        public List<dynamic> GetClients() {
            var request = new RestRequest("all", Method.Get);

            RestResponse response = ExecuteRequest(request);
            return JsonConvert.DeserializeObject<List<dynamic>>(response.Content);
        }

        public void IncrementCompletedJobs(string ipAddress, int port) {
            var request = new RestRequest($"increment/{ipAddress}/{port}", Method.Put);

            ExecuteRequest(request);
        }

        //TODO: I don't think this endpoint exists
        public async Task<List<dynamic>> CheckForJobs(dynamic clientInfo) {
            string url = $"http://{clientInfo.IPAddress}:{clientInfo.Port}/api";
            var client = new RestClient(url);
            var request = new RestRequest("jobs", Method.Get);

            RestResponse response = await ExecuteRequestAsync(request);
            if (CheckResponse(response)) {
                return JsonConvert.DeserializeObject<List<dynamic>>(response.Content);
            }
            return new List<dynamic>();
        }

        public async Task<List<dynamic>> GetJobsFromPeer(dynamic clientInfo) {
            string url = $"net.tcp://{clientInfo.IPAddress}:{clientInfo.Port}/jobService";
            ChannelFactory<IJobService> factory = null;
            IJobService proxy = null;

            try {
                factory = new ChannelFactory<IJobService>(
                    new NetTcpBinding(),
                    new EndpointAddress(url)
                );
                proxy = factory.CreateChannel();

                var jobs = await proxy.GetPendingJobsAsync();
                factory.Close();  // Close the factory if the operation is successful
                return jobs.Select(j => new { j.Id, j.IsCompleted }).Cast<dynamic>().ToList();
            } catch (Exception ex) {
                Console.WriteLine($"An error occurred while fetching jobs from peer: {ex.Message}");
                if (factory != null) {
                    factory.Abort();  // Abort the factory in case of an error
                }
                return new List<dynamic>();  // Return an empty list in case of an error
            }
        }



        public async Task<List<Job>> DistributeJob(Job job) {
            List<dynamic> peerClients = GetClients();
            List<Job> resolvedJobs = new List<Job>();

            foreach (var peer in peerClients) {
                string url = $"http://{peer.IPAddress}:{peer.Port}/jobService";
                var client = new RestClient(url);
                var request = new RestRequest("execute", Method.Post);
                request.AddParameter("jobId", job.Id.ToString());

                RestResponse response = await ExecuteRequestAsync(request);
                if (CheckResponse(response)) {
                    Job resultJob = JsonConvert.DeserializeObject<Job>(response.Content);
                    resolvedJobs.Add(resultJob);
                }
            }
            return resolvedJobs;
        }
    }
}
