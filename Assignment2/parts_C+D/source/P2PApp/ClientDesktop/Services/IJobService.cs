using System;
using System.ServiceModel;
using ClientDesktop.Models;

namespace ClientDesktop.Services {
    [ServiceContract]
    public interface IJobService {
        [OperationContract]
        bool HasJob(int port);
        [OperationContract]
        void EnqueueJob(Job job);
        [OperationContract]
        Job DequeueJob();
        [OperationContract]
        void SubmitJobResult(Job job);
        [OperationContract]
        string GetResult(Guid jobId);
        [OperationContract]
        bool Ping();
    }
}