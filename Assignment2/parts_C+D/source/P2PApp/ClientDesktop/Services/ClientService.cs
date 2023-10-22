using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using ClientDesktop.Models;
using Newtonsoft.Json;
using RestSharp;

namespace ClientDesktop.Services {
    public class ClientService {
        private const string BaseUrl = "http://localhost:5006/api/client";  // API endpoint
        private readonly RestClient _client;

        public ClientService(int askingClientPort) {
            _client = new RestClient(BaseUrl);
            RegisterClient("localhost", askingClientPort); // Register the client on startup
        }

        private RestResponse ExecuteRequest(RestRequest request) {
            return (RestResponse)_client.Execute(request);
        }

        private bool CheckResponse(RestResponse response) {
            if (response.StatusCode == System.Net.HttpStatusCode.OK) {
                return true;
            }
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

        public List<Client> GetClients(int askingClientPort) {
            try {
                var request = new RestRequest("all", Method.Get);
                RestResponse response = ExecuteRequest(request);
                if (CheckResponse(response)) {
                    var allClients = JsonConvert.DeserializeObject<List<Client>>(response.Content);
                    return allClients.Where(client => client.Port != askingClientPort).ToList();
                }
                else {
                    throw new InvalidOperationException($"Failed to get clients: {response.ErrorMessage}");
                }
            } catch (Exception ex) {
                MessageBox.Show($"An error occurred while fetching clients: {ex.Message}");
                throw new InvalidOperationException($"An error occurred while fetching clients: {ex.Message}");
            }
        }


        public void IncrementCompletedJobs(string ipAddress, int port) {
            var request = new RestRequest($"increment/{ipAddress}/{port}", Method.Put);

            ExecuteRequest(request);
        }
    }
}
