using System.Windows;

namespace ClientDesktop {
    public partial class PortDialog : Window {
        public int Port { get; private set; }

        public PortDialog() {
            InitializeComponent();
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e) {
            if (int.TryParse(PortTextBox.Text, out int port)) {
                Port = port;
                DialogResult = true;
                Close();
            }
            else {
                MessageBox.Show("Please enter a valid port number.");
            }
        }
    }
}
