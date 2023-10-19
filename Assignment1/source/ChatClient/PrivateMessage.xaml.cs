using DatabaseDLL;
using IChatServerInterfaceDLL;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
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


namespace ChatClient {
    /// <summary>
    /// Interaction logic for PrivateMessage.xaml
    /// </summary>
    public partial class PrivateMessage : Window {
        public string ReceiverUsername { get; private set; }
        public string SenderUsername { get; private set; }

        private IChatServerInterface chatServer;
        public PrivateMessage(string receiverUsername, string senderUsername, IChatServerInterface chatServerInterface) {
            InitializeComponent();
            ReceiverUsername = receiverUsername;
            SenderUsername = senderUsername;

            // Initialize the chat server interface
            chatServer = chatServerInterface;

            DataContext = new ViewModel.PrivateChatRoomViewModel();
            RefreshGUI();

        }

        /// <summary>
        /// Clears text when in focus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MessageTextBox_GotFocus(object sender, RoutedEventArgs e) {
            if (MessageTextBox.Text == "Type your message here...") {
                MessageTextBox.Text = "";
            }
        }
        /// <summary>
        /// Lets user drag window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Border_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.ChangedButton == MouseButton.Left) {
                this.DragMove();
            }
        }

        /// Minimize the window.
        private void ButtonMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        
       
        /// Close the ChatRoom window.
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void SendMessageButton_Click(object sender, RoutedEventArgs e) {
            string messageText = MessageTextBox.Text;

            if (string.IsNullOrWhiteSpace(messageText)) {
                MessageBox.Show("Please enter a valid message.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Add the message to the database
            User user = chatServer.SearchUserByName(SenderUsername);
            Message message = new Message(user, messageText);
            //await Task.Run(() => chatServer.AddMessageToPrivateChatroom(SenderUsername, ReceiverUsername, message));
            await Task.Run(() => chatServer.AddMessageToPrivateChatroom(SenderUsername, ReceiverUsername, messageText));

            // Update the ViewModel's Messages collection
            var viewModel = DataContext as ViewModel.PrivateChatRoomViewModel;
            viewModel?.Messages.Add(message);
            //var viewModel = DataContext as ViewModel.MainPageViewModel;
            //viewModel.Chatrooms.Add(await Task.Run(() => chatServer.SearchChatroomByName(newChatroomName)));

            // Clears the text box
            MessageTextBox.Text = "";
            RefreshGUI();

        }

        /// <summary>
        /// Refresh the list of active users and messages in the chatroom.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
         
            RefreshGUI();
        }

        private async void RefreshGUI()
        {
            try
            {
                // Fetch the list of active users in the current chat room
                var messageList = await Task.Run(() => chatServer.ListMessagesInPrivateChatroom(SenderUsername, ReceiverUsername));
                var viewModel = DataContext as ViewModel.PrivateChatRoomViewModel;
                if (viewModel != null)
                {
                    viewModel.Messages.Clear();
                    foreach (var message in messageList)
                    {
                        viewModel.Messages.Add(message);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception or display it to the user
                MessageBox.Show($"An error occurred: {ex.Message}");
                
            }
        }
    }
}
