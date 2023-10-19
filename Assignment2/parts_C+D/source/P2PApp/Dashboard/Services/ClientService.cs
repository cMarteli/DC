using Newtonsoft.Json;
using RestSharp;

namespace Dashboard.Services {
    public class ClientService {
        private const string BaseUrl = "http://localhost:5006/api/Client";  // Update to match your API endpoint

        public bool RegisterClient(int id, string ipAddress, int port) {
            var client = new RestClient(BaseUrl);
            var request = new RestRequest("new", Method.Post);

            // Prepare JSON object as per your API requirement
            var newClient = new { Id = id, IPAddress = ipAddress, Port = port };
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

        public dynamic GetClientById(int id) {
            var client = new RestClient(BaseUrl);
            var request = new RestRequest(id.ToString(), Method.Get);

            RestResponse response = client.Execute(request);
            return JsonConvert.DeserializeObject(response.Content);
        }

        public bool DeleteClient(int id) {
            var client = new RestClient(BaseUrl);
            var request = new RestRequest(id.ToString(), Method.Delete);

            RestResponse response = client.Execute(request);
            return response.StatusCode == System.Net.HttpStatusCode.OK;
        }
    }
}
