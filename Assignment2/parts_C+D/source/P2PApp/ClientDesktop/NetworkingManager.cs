namespace ClientDesktop {
    public class NetworkingManager {
        //private readonly ClientService _clientService;
        //private readonly JobService _jobServiceHost;
        //private readonly ScriptEngine _pythonEngine;
        //private readonly ScriptScope _pythonScope;
        //private readonly int _port;

        //public NetworkingManager(ClientService clientService, JobService jobService, int port) {
        //    _clientService = clientService;
        //    _jobServiceHost = jobService;

        //    _pythonEngine = Python.CreateEngine();
        //    _pythonScope = _pythonEngine.CreateScope();

        //    _port = port;
        //}

        //public async Task StartNetworkingLoop() {
        //    while (true) {
        //        List<Client> activeClients = _clientService.GetClients();
        //        await PerformJobsFromClients(activeClients);
        //        await Task.Delay(2000);
        //    }
        //}

        //private async Task PerformJobsFromClients(List<Client> clients) {
        //    foreach (Client client in clients) {
        //        List<Job> jobs = await _clientService.GetJobsFromPeer(client);

        //        if (jobs.Any()) {
        //            await ExecuteJobs(jobs);
        //        }
        //    }
        //}

        //private async Task ExecuteJobs(IEnumerable<Job> jobs) {
        //    foreach (var job in jobs) {
        //        if (job == null || job.IsCompleted) {
        //            return;
        //        }
        //        try {
        //            var result = await Task.Run(() => ExecutePythonCode(job.GetDecodedJobCode()));
        //            job.SetResult(result);
        //            _clientService.IncrementCompletedJobs("localhost", _port); // TODO: test this
        //            await _jobServiceHost.SubmitJobResultAsync(job);
        //        } catch (Exception e) {
        //            // Log or handle errors
        //        }
        //    }
        //}

        //private dynamic ExecutePythonCode(string code) {
        //    try {
        //        return _pythonEngine.Execute(code, _pythonScope);
        //    } catch (Exception e) {
        //        MessageBox.Show("An error occured solving the python code: " + e.Message);
        //        return null;
        //    }
        //}
    }
}
