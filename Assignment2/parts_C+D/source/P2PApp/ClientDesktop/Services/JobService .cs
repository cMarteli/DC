using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using ClientDesktop.Models;

namespace ClientDesktop.Services {
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false, InstanceContextMode = InstanceContextMode.Single)]
    public class JobService : IJobService {
        private readonly ConcurrentDictionary<Guid, Job> _jobList = new ConcurrentDictionary<Guid, Job>();

        public async Task<List<Job>> GetPendingJobsAsync() {
            return await Task.Run(() => _jobList.Values.Where(j => !j.IsCompleted).ToList());
        }

        public async Task<Job> GetJobByIdAsync(Guid jobId) {
            return await Task.Run(() => {
                if (_jobList.TryGetValue(jobId, out Job job)) {
                    return job;
                }
                return null;
            });
        }

        public async Task SubmitJobResultAsync(Job job) {
            await Task.Run(() => {
                if (_jobList.TryGetValue(job.Id, out Job jobToComplete)) {
                    jobToComplete.SetResult(job.Result);
                }
            });
        }

        public async Task AddNewJobAsync(Job job) {
            await Task.Run(() => {
                _jobList.TryAdd(job.Id, job);
            });
        }
    }
}
