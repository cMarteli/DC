using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;

namespace ClientDesktop.Services {
    public class ClientService {
        private readonly JobService _jobService;
        private const string BaseUrl = "http://localhost:5006/api/Client";  // Update to match your API endpoint

        public ClientService(JobService jobService) {
            _jobService = jobService;
        }
        public bool RegisterClient(string ipAddress, int port) {
            var client = new RestClient(BaseUrl);
            var request = new RestRequest("new", Method.Post);

            // Prepare JSON object as per your API requirement
            var newClient = new { IPAddress = ipAddress, Port = port };
            request.AddJsonBody(newClient);

            RestResponse response = client.Execute(request);
            return response.StatusCode == System.Net.HttpStatusCode.OK;
        }

        public bool UnregisterClient(string ipAddress, int port) {
            var client = new RestClient(BaseUrl);
            // Update the URL to include the ipAddress and port in the URL itself
            var request = new RestRequest($"remove/{ipAddress}/{port}", Method.Delete);

            RestResponse response = client.Execute(request);
            return response.StatusCode == System.Net.HttpStatusCode.OK;
        }


        public List<dynamic> GetClients() {
            var client = new RestClient(BaseUrl);
            var request = new RestRequest("all", Method.Get);

            RestResponse response = client.Execute(request);
            return JsonConvert.DeserializeObject<List<dynamic>>(response.Content);
        }

        public void IncrementCompletedJobs(string ipAddress, int port) {
            var client = new RestClient(BaseUrl);
            // Include the ipAddress and port in the URL for the request
            var request = new RestRequest($"increment/{ipAddress}/{port}", Method.Put);

            client.Execute(request);
        }


        public async Task<List<dynamic>> CheckForJobs(dynamic clientInfo) {
            string url = $"http://{clientInfo.IPAddress}:{clientInfo.Port}/api";
            var client = new RestClient(url);
            var request = new RestRequest("jobs", Method.Get); // Assuming the client exposes this API

            RestResponse response = await client.ExecuteAsync(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK) {
                return JsonConvert.DeserializeObject<List<dynamic>>(response.Content);
            }
            return new List<dynamic>();  // return empty list if no jobs or error
        }

        public async Task<List<dynamic>> DistributePythonCodeToPeersAsync(string pythonCode) {
            List<dynamic> peerClients = GetClients();
            List<dynamic> results = new List<dynamic>();

            foreach (var peer in peerClients) {
                // Replace the API call with a direct method invocation
                string result = await _jobService.ResolveJobAsync(pythonCode);
                results.Add(result);
            }
            return results;
        }

    }
}
