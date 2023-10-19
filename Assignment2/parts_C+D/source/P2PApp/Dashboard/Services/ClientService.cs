using Newtonsoft.Json;
using RestSharp;

namespace Dashboard.Services {
    public class ClientService {
        private const string BaseUrl = "http://localhost:5006/api/Client";  // Update to match your API endpoint

        public List<dynamic> GetClients() {
            var client = new RestClient(BaseUrl);
            var request = new RestRequest("all", Method.Get);

            RestResponse response = client.Execute(request);
            return JsonConvert.DeserializeObject<List<dynamic>>(response.Content);
        }
    }
}
