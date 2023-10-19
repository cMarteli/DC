using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using ClientDesktop.Models;

namespace ClientDesktop.Services {
    [ServiceContract]
    public interface IJobService {
        [OperationContract]
        Task<string> ResolveJobAsync(string pythonCode);

        [OperationContract]
        void AddJob(Job job);

        [OperationContract]
        List<Job> GetPendingJobs();
    }
}
