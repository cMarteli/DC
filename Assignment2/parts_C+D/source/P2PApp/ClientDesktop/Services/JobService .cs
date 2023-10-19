using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClientDesktop.Models;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;

namespace ClientDesktop.Services {
    public class JobService : IJobService {
        private readonly ScriptEngine _engine;
        private readonly ScriptScope _scope;
        private readonly List<Job> _jobList = new List<Job>();

        public JobService() {
            // Initialize IronPython runtime
            _engine = Python.CreateEngine();
            _scope = _engine.CreateScope();
        }

        public void AddJob(Job job) {
            _jobList.Add(job);
        }

        public List<Job> GetPendingJobs() {
            return _jobList.Where(j => !j.IsCompleted).ToList();
        }

        public async Task<string> ResolveJobAsync(string pythonCode) {
            return await Task.Run(() => {
                _engine.Execute(pythonCode, _scope);
                dynamic testFunction = _scope.GetVariable("test_func");
                return testFunction(23, 4);  // TODO: Hardcoded values for demo purposes
            });
        }
    }
}
