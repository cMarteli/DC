using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using ClientDesktop.Services;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;

namespace ClientDesktop {
    public partial class MainWindow : Window {
        private ClientService _clientService;
        private JobServiceHostManager _jobServiceHostManager;
        private ScriptEngine _pythonEngine;
        private ScriptScope _pythonScope;
        private int _selectedPort = 0;

        private Thread _networkingThread;

        public MainWindow() {
            InitializeComponent();
            Closing += OnMainWindowClosing;

            // Initialize ClientService first
            _clientService = new ClientService();

            // Initialize IronPython
            _pythonEngine = Python.CreateEngine();
            _pythonScope = _pythonEngine.CreateScope();

            // Initialize port and networking
            PromptForPort();

            // Initialize and start the WCF Service Host
            _jobServiceHostManager = new JobServiceHostManager(_selectedPort);
            _jobServiceHostManager.StartService();

            // Initialize Networking Thread
            _networkingThread = new Thread(RunNetworkingTasks);
            _networkingThread.Start();
        }

        private void PromptForPort() {
            PortDialog portDialog = new PortDialog();
            if (portDialog.ShowDialog() == true) {
                _selectedPort = portDialog.Port;
                _clientService.RegisterClient("localhost", _selectedPort);
            }
            else {
                Close();
            }
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e) {
            string pythonCode = PythonCodeInput.Text;
            if (!string.IsNullOrWhiteSpace(pythonCode)) {
                ExecutePythonJob(pythonCode);
            }
            else {
                MessageBox.Show("Please enter some Python code.");
            }
        }

        private void ExecutePythonJob(string pythonCode) {
            try {
                _pythonEngine.Execute(pythonCode, _pythonScope);
                // Assuming "test_func" is a defined Python function
                dynamic testFunction = _pythonScope.GetVariable("test_func");
                string result = Convert.ToString(testFunction(23, 4)); // Execute the Python function
                MessageBox.Show($"Job Completed: {result}");
            } catch (Exception e) {
                MessageBox.Show($"An error occurred: {e.Message}");
            }
        }

        private async void RunNetworkingTasks() {
            while (true) {
                List<dynamic> peerClients = _clientService.GetClients();
                foreach (var client in peerClients) {
                    if (client.Port != _selectedPort) { // Skip self
                        List<dynamic> jobs = await _clientService.GetJobsFromPeer(client);
                        foreach (var job in jobs) {
                            ExecutePythonJob(job.Code);  // Assuming the Python code is in 'Code' property
                        }
                    }
                }
                Thread.Sleep(5000); // Wait for 5 seconds before the next cycle
            }
        }


        private void OnMainWindowClosing(object sender, System.ComponentModel.CancelEventArgs e) {
            _clientService.UnregisterClient("localhost", _selectedPort);
            _jobServiceHostManager?.Dispose(); // Dispose to stop the service
        }
    }
}
