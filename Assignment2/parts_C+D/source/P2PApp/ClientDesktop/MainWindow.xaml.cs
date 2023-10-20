using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows;
using ClientDesktop.Services;

namespace ClientDesktop {
    public partial class MainWindow : Window {
        private ServiceHost host;
        private readonly ClientService _clientService;
        private readonly JobService _jobService;
        private int _completedJobs = 0;
        private int selectedPort = 0;

        public MainWindow() {
            InitializeComponent();
            Closing += MainWindow_Closing; // Close WCF service host when window closes


            _jobService = new JobService();
            _clientService = new ClientService(_jobService);

            PortDialog portDialog = new PortDialog();
            if (portDialog.ShowDialog() == true) {
                selectedPort = portDialog.Port;

                // Continue initialization with the port
                InitializeWCFService(selectedPort);
                StartNetworkingThread();
            }
            else {
                // Close the application if the dialog is cancelled or no port is entered
                Close();
            }
        }

        // TODO: invalid port?
        private void InitializeWCFService(int port) {
            _clientService.RegisterClient("localhost", port); // Register this client with the API
            Uri baseAddress = new Uri($"http://localhost:{port}/JobService");
            host = new ServiceHost(typeof(JobService), baseAddress);

            // Define an endpoint for the service.
            host.AddServiceEndpoint(typeof(IJobService), new WSHttpBinding(), "");

            host.Open();
        }


        private void StartNetworkingThread() {
            Task.Run(async () => {
                while (true) {
                    // Discover active clients
                    List<dynamic> activeClients = _clientService.GetClients();
                    // Check each client for jobs and perform them
                    await PerformJobsFromClients(activeClients);
                    await Task.Delay(2000); // Sleep for 2 seconds
                }
            });
        }

        private async void SubmitButton_Click(object sender, RoutedEventArgs e) {
            string pythonCode = PythonCodeInput.Text;

            if (IsInputValid(pythonCode)) {
                DisableUI();
                try {
                    // Distribute Python code to peer GUIs
                    var result = await _clientService.DistributePythonCodeToPeersAsync(pythonCode);
                    MessageBox.Show($"Execution completed. Result: {result}");
                } catch (Exception ex) {
                    MessageBox.Show($"An error occurred: {ex.Message}");
                } finally {
                    EnableUI();
                }
            }
        }

        private bool IsInputValid(string input) {
            if (string.IsNullOrWhiteSpace(input)) {
                MessageBox.Show("Please enter some Python code.");
                return false;
            }
            return true;
        }

        private void DisableUI() {
            SubmitButton.IsEnabled = false;
            CheckStatusButton.IsEnabled = false;
        }

        private void EnableUI() {
            SubmitButton.IsEnabled = true;
            CheckStatusButton.IsEnabled = true;
        }

        private void CheckStatusButton_Click(object sender, RoutedEventArgs e) {

        }

        // Method to update the Job Status text
        public void UpdateJobStatus(string status) {
            Dispatcher.Invoke(() => {
                JobStatus.Text = $"Job Status: {status}";
            });
        }

        private async Task PerformJobsFromClients(List<dynamic> clients) {
            foreach (var client in clients) {
                var jobs = await _clientService.CheckForJobs(client);

                if (jobs.Any()) {
                    foreach (var job in jobs) {
                        try {
                            var result = await _jobService.ResolveJobAsync(job.PythonCode);
                            IncrementCompletedJobs();
                            // Send results back to the originating client if needed
                        } catch (Exception e) {
                            // Log or report job execution errors
                        }
                    }
                }
            }
        }

        public void IncrementCompletedJobs() {
            _completedJobs++;
            _clientService.IncrementCompletedJobs("localhost", selectedPort); //TODO : Fix this NOT INCREMENTING
            UpdateJobStatus($"Completed Jobs: {_completedJobs}");
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e) {
            _clientService.UnregisterClient("localhost", selectedPort); // Unregister this client with the API
            host?.Close();
        }

        private bool IsPortAvailable(int port) {
            bool isAvailable = true;

            try {
                // Try to open the port
                System.Net.Sockets.TcpListener tcpListener = new System.Net.Sockets.TcpListener(System.Net.IPAddress.Any, port);
                tcpListener.Start();
                tcpListener.Stop();
            } catch (Exception) {
                isAvailable = false;
            }

            return isAvailable;
        }

    }
}
