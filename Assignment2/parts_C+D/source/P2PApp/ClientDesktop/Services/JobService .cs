using System;
using System.Collections.Concurrent;
using System.Linq;
using System.ServiceModel;
using ClientDesktop.Models;

namespace ClientDesktop.Services {
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = true, InstanceContextMode = InstanceContextMode.Single)]
    public class JobService : IJobService {
        private ConcurrentQueue<Job> _jobQueue = new ConcurrentQueue<Job>(); // Queue for pending jobs
        private ConcurrentDictionary<Guid, String> _completedJobs = new ConcurrentDictionary<Guid, String>();
        // Dictionary for completed jobs

        public bool HasJob(int port) {
            if (_jobQueue.Count != 0 && _jobQueue.FirstOrDefault().Owner != port) {
                return true;
            }
            return false;
        }

        public void EnqueueJob(Job job) {
            _jobQueue.Enqueue(job);
            LogInformation($"Enqueue|| Owner: {job.Owner} Code: {job.GetDecodedJobCode()}");
        }

        public Job DequeueJob() {
            if (_jobQueue.TryDequeue(out Job job)) {
                return job;
            }
            throw new Exception("No job in queue");
        }

        public void SubmitJobResult(Job job) {
            _completedJobs.TryAdd(job.Id, job.Result.ToString());
            LogInformation("Submitted Job Result: " + job.Result.ToString());
        }

        public string GetResult(Guid jobId) {
            if (_completedJobs.ContainsKey(jobId)) {
                return _completedJobs[jobId];
            }
            throw new Exception("Couldn't obtain result");
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
