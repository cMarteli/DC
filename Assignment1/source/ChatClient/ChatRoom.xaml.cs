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

namespace ChatClient
{
    /// Interaction logic for ChatRoom.xaml
    public partial class ChatRoom : Window
    {
        private IChatServerInterface chatServer;
        public static readonly string chatUrl = "net.tcp://localhost:8100/ChatService";
        public string ChatroomName { get; private set; }
        public string Username { get; private set; }
        public ChatRoom(string chatroomName, string username)
        {
            InitializeComponent();

            // Initialize connection to the chat server
            ChannelFactory<IChatServerInterface> channelFact;
            NetTcpBinding tcp = new NetTcpBinding();

            string URL = "net.tcp://localhost:8100/ChatService";
            channelFact = new ChannelFactory<IChatServerInterface>(tcp, URL);
            chatServer = channelFact.CreateChannel();
            DataContext = new ViewModel.ChatRoomViewModel();

            // Store the chatroom name and username
            ChatroomName = chatroomName;
            Username = username;

            GetChatRoomName.Content = ChatroomName;
            UsernameLabel.Content = Username;
    
        }

        /// <summary>
        /// Clears text when clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MessageTyping_GotFocus(object sender, EventArgs e) {
            if (MessageTyping.Text.Equals("Enter message here:")) {
                MessageTyping.Text = "";
            }
        }
        /// Handle the mouse down event to enable dragging of the window.
        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }
        /// Minimize the window.
        private void ButtonMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        /// Toggle between window's maximized and normal states.
        private void WindowStateButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
            }
            else
                this.WindowState = WindowState.Maximized;
        }

        /// Close the ChatRoom window.
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            chatServer.RemoveUserFromChatroom(Username, ChatroomName);
            this.Close();

        }
        /// Send a message to the chatroom.
        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {

            var messageText = MessageTyping.Text;

            if (string.IsNullOrWhiteSpace(messageText))
            {
                MessageBox.Show("Please enter a valid text.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Add the message to the database
            User user = chatServer.SearchUserByName(Username);
            Message message = new Message(user, messageText);
            await Task.Run(() => chatServer.AddMessageToChatroom(ChatroomName, message));

            // Update the ViewModel's Messages collection
            var viewModel = DataContext as ViewModel.ChatRoomViewModel;
            viewModel?.Messages.Add(message);


        }
        /// <summary>
        /// Refresh the list of active users and messages in the chatroom.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void RefreshUsersButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Fetch the list of active users in the current chat room
                var usersList = await Task.Run(() => chatServer.ListUsersInChatroom(ChatroomName));
                var messageList = await Task.Run(() => chatServer.ListMessagesInChatroom(ChatroomName));
                var viewModel = DataContext as ViewModel.ChatRoomViewModel;
                if (viewModel != null)
                {
                    viewModel.Users.Clear();
                    foreach (var user in usersList)
                    {
                        viewModel.Users.Add(user);
                    }
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

        /// <summary>
        /// Handle the file sharing button click to upload a file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UploadButton_Click(object sender, RoutedEventArgs e) {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text files (*.txt)|*.txt|Bitmap Files (*.bmp)|*.bmp";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            if (openFileDialog.ShowDialog() == true) {
                string selectedFilePath = openFileDialog.FileName;
                chatServer.AddSharedFileToChatroom(ChatroomName, chatServer.CreateSharedFile(selectedFilePath));
            }
        }

        /// <summary>
        /// Refresh the list of shared files in the chatroom
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void RefreshSharedFilesButton_Click(object sender, RoutedEventArgs e) {
            try {
                var sharedFilesList = await Task.Run(() => chatServer.GetAllSharedFilesFromChatroom(ChatroomName));
                var viewModel = DataContext as ViewModel.ChatRoomViewModel;
                if (viewModel != null) {
                    viewModel.SharedFiles.Clear();
                    foreach (var file in sharedFilesList) {
                        viewModel.SharedFiles.Add(file);
                    }
                }
            } catch (Exception ex) {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            // Get the clicked item
            var item = ((FrameworkElement)e.OriginalSource).DataContext as SharedFile;
            if (item != null) {
                // Call the method to download the file
                DownloadFile(item);
            }
        }

        /// <summary>
        /// Gets file from db and opens a save dialog
        /// </summary>
        /// <param name="sharedFile"></param>
        private void DownloadFile(SharedFile sharedFile) {
            try {
                // Fetch the file data from the server
                var fileData = chatServer.GetSharedFileFromChatroom(ChatroomName, sharedFile.FileName);
                // Create a SaveFileDialog to let the user specify where to save the file
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.FileName = sharedFile.FileName;
                saveFileDialog.Filter = "Text files (*.txt)|*.txt|Bitmap Files (*.bmp)|*.bmp";
                if (saveFileDialog.ShowDialog() == true) {
                    // Save the file to the specified path
                    File.WriteAllBytes(saveFileDialog.FileName, fileData.FileData);
                }
            } catch (Exception ex) {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        /// <summary>
        /// Click event to private message user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void UserListView_MouseDoubleClick(object sender, MouseButtonEventArgs e) {            
            var clickedUser = ((FrameworkElement)e.OriginalSource).DataContext as User; // Get the clicked User
            if (clickedUser != null && !clickedUser.username.Equals(this.Username)) { //if not empty or self
                // Change this if needed
                var privateMessage = new PrivateMessage(clickedUser.Username, this.Username, chatServer);
                try
                {
                    PrivateChatroom privChatRoom = chatServer.GetPrivateChatroom(clickedUser.Username, this.Username);
                }
                catch (Exception ex) when (ex is FaultException<KeyNotFoundException> || ex is KeyNotFoundException || ex is FaultException)
                {
                    string roomName = clickedUser.Username + "-" + this.Username;
                    await Task.Run(() => chatServer.AddPrivateChatroom(roomName, clickedUser.Username, this.Username));
                }
               privateMessage.Show();

            }
        }

    }
}
