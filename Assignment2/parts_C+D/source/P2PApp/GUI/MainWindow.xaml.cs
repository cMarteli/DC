using System;
using System.Threading.Tasks;
using System.Windows;
using GUI.Services;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;

namespace P2PClient {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        private readonly ClientService _clientService;
        public MainWindow() {
            InitializeComponent();
            _clientService = new ClientService();
        }

        // Event handler for the Submit button
        private async void SubmitButton_Click(object sender, RoutedEventArgs e) {
            // Get the text from the TextBox
            string pythonCode = PythonCodeInput.Text;

            int var1, var2;
            var1 = 23;
            var2 = 4;

            // Validate the input, if necessary
            if (string.IsNullOrWhiteSpace(pythonCode)) {
                MessageBox.Show("Please enter some Python code.");
                return;
            }

            // Disable the submit button to prevent multiple submissions
            SubmitButton.IsEnabled = false;

            // Initialize IronPython runtime
            ScriptEngine engine = Python.CreateEngine();
            ScriptScope scope = engine.CreateScope();

            try {
                // Execute the Python code
                var result = await Task.Run(() => {
                    engine.Execute(pythonCode, scope);
                    // Get the function from the Python code
                    dynamic testFunction = scope.GetVariable("test_func");
                    return testFunction(var1, var2);
                });

                // Display result
                MessageBox.Show($"Execution completed. Result: {result}");

            } catch (Exception ex) {
                MessageBox.Show($"An error occurred: {ex.Message}");
            } finally {
                // Re-enable the submit button
                SubmitButton.IsEnabled = true;
            }
        }


        private void CheckStatusButton_Click(object sender, RoutedEventArgs e) {

        }

        // Method to update the Job Status text
        public void UpdateJobStatus(string status) {
            // Update the TextBlock with new job status
            JobStatus.Text = $"Job Status: {status}";
        }
    }
}
