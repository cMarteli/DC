using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;

namespace ClientDesktop.Services {
    public class ClientService {
        private const string BaseUrl = "http://localhost:5006/api/Client";  // Update to match your API endpoint

        public bool RegisterClient(string ipAddress, int port) {
            var client = new RestClient(BaseUrl);
            var request = new RestRequest("new", Method.Post);

            // Prepare JSON object as per your API requirement
            var newClient = new { IPAddress = ipAddress, Port = port };
            request.AddJsonBody(newClient);

            RestResponse response = client.Execute(request);
            return response.StatusCode == System.Net.HttpStatusCode.OK;
        }

        public List<dynamic> GetClients() {
            var client = new RestClient(BaseUrl);
            var request = new RestRequest("all", Method.Get);

            RestResponse response = client.Execute(request);
            return JsonConvert.DeserializeObject<List<dynamic>>(response.Content);
        }

        public async Task<List<dynamic>> CheckForJobs(dynamic clientInfo) {
            // Assume an API endpoint "jobs" on peer clients to get pending Python jobs
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
                // Assume an API endpoint "execute" on peer clients to run Python code
                string url = $"http://{peer.IPAddress}:{peer.Port}/api";
                var client = new RestClient(url);
                var request = new RestRequest("execute", Method.Post);

                request.AddJsonBody(new { Code = pythonCode });

                RestResponse response = await client.ExecuteAsync(request);
                if (response.StatusCode == System.Net.HttpStatusCode.OK) {
                    dynamic result = JsonConvert.DeserializeObject(response.Content);
                    results.Add(result);
                }
            }
            return results;
        }
    }
}
