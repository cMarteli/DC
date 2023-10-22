using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using ClientDesktop.Models;
using ClientDesktop.Services;

namespace ClientDesktop {
    public partial class MainWindow : Window {
        private static string IP = "localhost";
        private JobServiceHost _jobServiceHost;

        private ClientService _clientService;
        private IronPyService _pythonService;
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
            _pythonService = new IronPyService();

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

        private void RunNetworkingTasks() {
            while (true) {
                Thread.Sleep(5000);
                _jobServiceHost.UpdatePeerList(FetchPeers());

                try {
                    _jobServiceHost.CheckPeerServiceStatus();
                    Job jobToExec = _jobServiceHost.DequeueJobFromPeers(_port);
                    if (Object.ReferenceEquals(jobToExec, null)) { // if no job is found, skip the rest of the loop
                        continue;
                    }
                    jobToExec.SetResult(ExecutePythonJob(jobToExec)); // Sets jobs as completed and packages the result
                    _jobServiceHost.SubmitJobResultToOwner(jobToExec);
                    _clientService.IncrementCompletedJobs(IP, _port);
                } catch (Exception e) {
                    Console.Out.WriteLine($"RunNetworkingTasks: {e.Message}");
                }
            }
        }




        private void SubmitButton_Click(object sender, RoutedEventArgs e) {
            string pythonCode = PythonCodeInput.Text;
            JobStatus.Text = "Job Pending";
            SubmitButton.IsEnabled = false;
            if (!string.IsNullOrWhiteSpace(pythonCode)) {
                Job newJob = new Job(pythonCode, _port);
                _pendingJobId = newJob.Id; // Track the job that is currently being processed
                _jobServiceHost.PostJob(newJob);
                //MessageBox.Show($"Button Click, Job ID: {newJob.Id}, Code: {newJob.GetDecodedJobCode()}");
            }
            else {
                MessageBox.Show("Please enter some Python code.");
            }
        }

        internal string ExecutePythonJob(Job j) {
            return _pythonService.ExecutePythonJob(j.GetDecodedJobCode());
        }

        private List<Client> FetchPeers() {
            return _clientService.GetClients(_port);
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e) {
            try {
                string result = _jobServiceHost.GetResult(_pendingJobId);
                JobStatus.Text = "Job Completed";
                ResultList.Items.Add($"Job ID: {_pendingJobId}, Result: {result}");
                SubmitButton.IsEnabled = true;

            } catch (Exception ex) {
                JobStatus.Text = "Job Pending";
                MessageBox.Show(ex.Message);
            }
        }

        private void OnMainWindowClosing(object sender, System.ComponentModel.CancelEventArgs e) {
            _clientService.UnregisterClient("localhost", _port);
            _jobServiceHost.StopService();
        }


    }
}
