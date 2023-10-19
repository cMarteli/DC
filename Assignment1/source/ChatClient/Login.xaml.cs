using ChatServer;
using DatabaseDLL;
using IChatServerInterfaceDLL;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ChatClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IChatServerInterface channel;

        public MainWindow()
        {
            InitializeComponent();

            ChannelFactory<IChatServerInterface> channelFact;
            NetTcpBinding tcp = new NetTcpBinding();

            string URL = "net.tcp://localhost:8100/ChatService";
            channelFact = new ChannelFactory<IChatServerInterface>(tcp, URL);
            channel = channelFact.CreateChannel();
        }
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            string inputUserName = UserName.Text;

            if (UserName.Text.Equals("")) {
                MessageBox.Show("Username cannot be blank.");
                return;
            }

            User user = await Task.Run(() => GetUserByUsername(inputUserName));
            Login(user);

            


        }

        private User GetUserByUsername(string username)
        {
            User user;
            try
            {
                user = channel.SearchUserByName(username);
            }
            catch (Exception ex) when (ex is FaultException<KeyNotFoundException> || ex is KeyNotFoundException || ex is FaultException)
            {
                user = new User("");
                channel.AddUser(username);
            }
            catch (Exception ex) when (ex is CommunicationException)
            {
                user = new User("");
            }
            return user;
        }

        /// <summary>
        /// Checks if a name already exists if not opens a new page
        /// </summary>
        /// <param name="user"></param>
        private void Login(User user)
        {
            if (channel.UserExists(user.Username)) {
                MessageBox.Show("This username is already in use.");
                return;
            }
            MainPage mainPage = new MainPage(UserName.Text); // Use the text directly from the TextBox
            mainPage.Show();
            this.Close();
        }

        /// <summary>
        /// Clears text when clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserName_GotFocus(object sender, EventArgs e) {
            if (UserName.Text.Equals("Enter username")) {
                UserName.Text = "";
            }
        }
    }
}