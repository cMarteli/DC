using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using ClientDesktop.Models;
using ClientDesktop.Services;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;

namespace ClientDesktop {
    public partial class MainWindow : Window {

        private JobServiceHost _jobServiceHost;

        private ClientService _clientService;
        private ScriptEngine _pythonEngine;
        private ScriptScope _pythonScope;
        private int _port = 0;
        private Guid _pendingJobId; // used to track the job that is currently being processed

        private Thread _networkingThread; // Thread for networking tasks

        public MainWindow() {
            InitializeComponent();
            Closing += OnMainWindowClosing;

            // Initialize port
            PromptForPort();

            JobStatus.Text = "Port: " + _port;

            _clientService = new ClientService(_port);

            // Initialize IronPython
            _pythonEngine = Python.CreateEngine();
            _pythonScope = _pythonEngine.CreateScope();

            // Initialize and start the WCF JobService Host
            _jobServiceHost = new JobServiceHost(_port);

            // Initialize Networking Thread
            _networkingThread = new Thread(RunNetworkingTasks);
            _networkingThread.Start();
        }

        private void PromptForPort() {
            PortDialog portDialog = new PortDialog();
            if (portDialog.ShowDialog() == true) {
                _port = portDialog.Port;
            }
            else {
                Close();
            }
        }

        /// <summary>
        /// Main networking thread that runs in the background
        /// Updates peer list and tries to solve other peer's jobs
        /// </summary>
        private void RunNetworkingTasks() {
            while (true) {
                _jobServiceHost.UpdatePeerList(FetchPeers()); // update jobservice's peer list

                // Try to solve other peer's jobs
                //if (_jobServiceHost.HasJob(_port)) {
                Job jobToExec = _jobServiceHost.DequeueJobFromPeers(_port);
                if (jobToExec != null) {
                    MessageBox.Show($"Arrived at peer: {_port} to be processed. Owner: {jobToExec.Owner} Code: {jobToExec.GetDecodedJobCode()}");
                    jobToExec.Result = ExecutePythonJob(jobToExec); // package the result to the job
                    _jobServiceHost.SubmitJobResultToOwner(jobToExec); // submit the job to the jobservice
                }
                //}
                Thread.Sleep(5000); // Wait for 5 seconds before the next cycle
            }
        }



        private void SubmitButton_Click(object sender, RoutedEventArgs e) {
            string pythonCode = PythonCodeInput.Text;
            JobStatus.Text = "Job Pending";
            SubmitButton.IsEnabled = false;
            if (!string.IsNullOrWhiteSpace(pythonCode)) {
                Job newJob = new Job(pythonCode, _port);
                _pendingJobId = newJob.Id; // Track the job that is currently being processed
                _jobServiceHost.EnqueueJob(newJob);
                //MessageBox.Show($"Button Click, Job ID: {newJob.Id}, Code: {newJob.GetDecodedJobCode()}");
            }
            else {
                MessageBox.Show("Please enter some Python code.");
            }
        }


        private string ExecutePythonJob(Job j) {
            try {
                string pythonCode = j.GetDecodedJobCode();
                MessageBox.Show($"About to be processd. Code: {pythonCode}");
                _pythonEngine.Execute(pythonCode, _pythonScope);
                dynamic testFunction = _pythonScope.GetVariable("test_func");
                var result = Convert.ToString(testFunction(23, 4)); // Execute the Python function
                MessageBox.Show($"Processed. Result: {result}");
                return result;

            } catch (Exception e) {
                MessageBox.Show($"An error occurred: {e.Message}");
                return null;
            }
        }

        private List<Client> FetchPeers() {
            return _clientService.GetClients(_port);
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e) {
            string result = _jobServiceHost.GetResult(_pendingJobId);
            if (result != null) {
                JobStatus.Text = "Job Completed";
                ResultList.Items.Add($"Job ID: {_pendingJobId}, Result: {result}");
                SubmitButton.IsEnabled = true;
            }
            else {
                JobStatus.Text = "Job Pending";
            }
        }

        private void OnMainWindowClosing(object sender, System.ComponentModel.CancelEventArgs e) {
            _clientService.UnregisterClient("localhost", _port);
            _jobServiceHost.StopService();
        }


    }
}
