using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using ClientDesktop.Models;
using ClientDesktop.Services;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;

namespace ClientDesktop {
    public class NetworkingManager {
        private readonly ClientService _clientService;
        private readonly JobService _jobService;
        private readonly ScriptEngine _pythonEngine;
        private readonly ScriptScope _pythonScope;
        private readonly int _port;

        public NetworkingManager(ClientService clientService, JobService jobService, int port) {
            _clientService = clientService;
            _jobService = jobService;

            _pythonEngine = Python.CreateEngine();
            _pythonScope = _pythonEngine.CreateScope();

            _port = port;
        }

        public async Task StartNetworkingLoop() {
            while (true) {
                List<dynamic> activeClients = _clientService.GetClients();
                await PerformJobsFromClients(activeClients);
                await Task.Delay(2000);
            }
        }

        private async Task PerformJobsFromClients(List<dynamic> clients) {
            foreach (var client in clients) {
                var jobs = await _clientService.CheckForJobs(client);

                if (jobs.Any()) {
                    await ExecuteJobs(jobs);
                }
            }
        }

        private async Task ExecuteJobs(IEnumerable<Job> jobs) {
            foreach (var job in jobs) {
                try {
                    var result = await Task.Run(() => ExecutePythonCode(job.GetDecodedJobCode()));
                    job.SetResult(result);
                    _clientService.IncrementCompletedJobs("localhost", _port); // TODO: test this
                    await _jobService.SubmitJobResultAsync(job);
                } catch (Exception e) {
                    // Log or handle errors
                }
            }
        }

        private dynamic ExecutePythonCode(string code) {
            try {
                return _pythonEngine.Execute(code, _pythonScope);
            } catch (Exception e) {
                MessageBox.Show("An error occured solving the python code: " + e.Message);
                return null;
            }
        }
    }
}
