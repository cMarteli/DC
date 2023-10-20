using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using ClientDesktop.Models;

namespace ClientDesktop.Services {
    [ServiceContract]
    public interface IJobService {
        [OperationContract]
        Task<List<Job>> GetPendingJobsAsync();

        [OperationContract]
        Task<Job> GetJobByIdAsync(Guid jobId);

        [OperationContract]
        Task SubmitJobResultAsync(Job job);

        [OperationContract]
        Task AddNewJobAsync(Job job);
    }
}
