using System;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;

namespace ClientDesktop.Services {
    public class IronPyService {
        private ScriptEngine _pythonEngine;
        private ScriptScope _pythonScope;

        public IronPyService() {
            _pythonEngine = Python.CreateEngine();
            _pythonScope = _pythonEngine.CreateScope();
        }

        public string ExecutePythonJob(string pythonCode) {
            try {
                Console.WriteLine($"ExecutePythonJob: Processing...  Code: {pythonCode}");
                _pythonEngine.Execute(pythonCode, _pythonScope);
                dynamic testFunction = _pythonScope.GetVariable("test_func");
                var result = Convert.ToString(testFunction(23, 4)); // Execute the Python function
                Console.WriteLine($"Processed. Result: {result}");
                return result;
            } catch (Exception e) {
                Console.WriteLine($"ExecutePythonJob: Exception: {e.Message}");
                return null;
            }
        }
    }
}
