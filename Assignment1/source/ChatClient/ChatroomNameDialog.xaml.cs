using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ChatClient
{
    public partial class ChatroomNameDialog : Window
    {
        public string ChatroomName { get; private set; }

        public ChatroomNameDialog()
        {
            InitializeComponent();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            ChatroomName = ChatroomNameTextBox.Text;
            if (ChatroomName.Equals("")) {
                MessageBox.Show("Name cannot be blank.");
                return;
            }
            this.DialogResult = true;
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        /// <summary>
        /// Clears text when clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChatroomNameTextBox_GotFocus(object sender, EventArgs e) {
            if (ChatroomNameTextBox.Text.Equals("Type the chatroom name")) {
                ChatroomNameTextBox.Text = "";
            }
        }
    }
}