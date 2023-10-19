using System;
using System.Collections.Generic;
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
using DatabaseDLL;
using IChatServerInterfaceDLL;

namespace ChatClient
{

    /// Interaction logic for MainPage.xaml
    public partial class MainPage : Window
    {
        public string Username { get; private set; }
        private IChatServerInterface chatServer;
        public static readonly string chatUrl = "net.tcp://localhost:8100/ChatService";
        public MainPage(string username)
        {
            InitializeComponent();



            // Establish a connection to the chatroom server
            NetTcpBinding tcp = new NetTcpBinding();
            ChannelFactory<IChatServerInterface> chatroomChannelFactory = new ChannelFactory<IChatServerInterface>(tcp, chatUrl);
            chatServer = chatroomChannelFactory.CreateChannel();


            DataContext = new ViewModel.MainPageViewModel();
            Username = username;
            GetUsername.Content = Username;
            MessageBox.Show($"Received username: {Username}");

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

        /// Close the MainPage window.
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();

        }
        /// Create a new chatroom.
        private async void CreateChatroomButton_Click(object sender, RoutedEventArgs e)
        {
            //var await Task.Run(() => ChatServer = (DataContext as ViewModel.MainPageViewModel)._await Task.Run(() => ChatServer; // Access the await Task.Run(() => ChatServer from ViewModel

            var dialog = new ChatroomNameDialog();
            if (dialog.ShowDialog() == true)
            {
                var newChatroomName = dialog.ChatroomName;
                if (string.IsNullOrWhiteSpace(newChatroomName))
                {
                    MessageBox.Show("Please enter a valid chatroom name.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                // Check if chatroom already exists
                if (await Task.Run(() => chatServer.ChatroomExists(newChatroomName)))
                {
                    MessageBox.Show($"Chatroom '{newChatroomName}' already exists. Please choose a different name.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Add the new chatroom to the database
                await Task.Run(() => chatServer.AddChatroom(newChatroomName));

                // Update the GUI with the new chatroom
                var viewModel = DataContext as ViewModel.MainPageViewModel;
                viewModel.Chatrooms.Add(await Task.Run(() => chatServer.SearchChatroomByName(newChatroomName)));
            }
        }
        /// Logout the current user and navigate to the Login window.
        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            // Delete active User
            chatServer.RemoveUser(Username);

            // Navigate to the Login window
            MainWindow loginWindow = new MainWindow();
            loginWindow.Show();

            // Close the current window
            this.Close();
        }
        /// Refresh the list of available chatrooms.
        private async void CreateRefreshButton_Click(object sender, RoutedEventArgs e)
        {

            var viewModel = DataContext as ViewModel.MainPageViewModel;
            var chatRoomList = await Task.Run(() => chatServer.ListChatRooms());
            viewModel.Chatrooms.Clear();

            foreach (var chat in chatRoomList)
            {
                viewModel.Chatrooms.Add(chat);
            }

        }
        /// Handle the event when a chatroom is selected from the list.
        private void ChatroomListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems != null && e.AddedItems.Count > 0)
            {
                var selectedChatroom = e.AddedItems[0] as Chatroom;
                if (selectedChatroom != null)
                {
                    // Add the user to the chatroom
                    chatServer.AddUserToChatroom(Username, selectedChatroom.Name);

                    // Navigate to ChatRoom.xaml page and pass the chatroom name as argument
                    ChatRoom chatRoomWindow = new ChatRoom(selectedChatroom.Name, Username);
                    chatRoomWindow.Show();
                    
                }
            }
        }
    }
}
