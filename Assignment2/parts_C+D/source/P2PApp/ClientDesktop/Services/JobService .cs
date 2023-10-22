using System;
using System.Collections.Concurrent;
using System.Linq;
using System.ServiceModel;
using ClientDesktop.Models;

namespace ClientDesktop.Services {
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = true, InstanceContextMode = InstanceContextMode.Single)]
    public class JobService : IJobService {
        private ConcurrentQueue<Job> _jobQueue; // Queue for pending jobs
        private ConcurrentDictionary<Guid, String> _completedJobs;
        // Dictionary for completed jobs

        public JobService() {
            Console.WriteLine("Job Service System Online");
            _jobQueue = new ConcurrentQueue<Job>();
            _completedJobs = new ConcurrentDictionary<Guid, String>();
        }
        public bool HasJob(int port) {
            if (_jobQueue.Count != 0 && _jobQueue.FirstOrDefault().Owner != port) {
                return true;
            }
            return false;
        }

        public void EnqueueJob(Job job) {
            LogInformation($"EnqueueJob: Owner: {job.Owner} Code: {job.GetDecodedJobCode()}");
            _jobQueue.Enqueue(job);
        }

        public Job DequeueJob() {
            if (_jobQueue.TryDequeue(out Job job)) {
                return job;
            }
            throw new Exception("DequeueJob: No job in queue");
        }

        public void SubmitJobResult(Job job) {
            LogInformation("SubmitJobResult: " + job.Result.ToString());
            _completedJobs.TryAdd(job.Id, job.Result.ToString());
        }

        public string GetResult(Guid jobId) {
            if (_completedJobs.ContainsKey(jobId)) {
                return _completedJobs[jobId];
            }
            throw new Exception("GetResult: Couldn't obtain result");
        }

        public bool Ping() {
            return true;
        }

        // Helper method for logging
        private void LogInformation(string message) {
            Console.WriteLine(message);
        }
    }
}
