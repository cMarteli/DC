using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using DCBusinessTier;
using DCDatabase;
using System.ServiceModel;

namespace GUI_Async {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// TODO: Curently not working
    public partial class MainWindow : Window {
        private BusinessServerInterface channel;
        private ChannelFactory<BusinessServerInterface> dataFactory;
        private string searchString;

        public MainWindow() {
            InitializeComponent();

            /* Initialize the channel for communication */
            NetTcpBinding tcp = new NetTcpBinding();
            string URL = "net.tcp://localhost:8200/BusinessService";
            dataFactory = new ChannelFactory<BusinessServerInterface>(tcp, URL);
            channel = dataFactory.CreateChannel();

            /* Update total entries */
            UpdateTotalEntries();
        }
        /* Methods for disabling/re-enabling user input */
        private void DisableInputs() {
            go_index_btn.IsEnabled = false;
            search_surname_btn.IsEnabled = false;
            search_text_box.IsReadOnly = true;
            index_text_box.IsReadOnly = true;
        }
        private void EnableInputs() {
            go_index_btn.IsEnabled = true;
            search_surname_btn.IsEnabled = true;
            search_text_box.IsReadOnly = false;
            index_text_box.IsReadOnly = false;
        }

        /* Helper method for updating total entries */
        private void UpdateTotalEntries() {
            total_text_block.Text = $"Total Entries: {channel.GetNumEntries()}";
        }

        /* Converts a Byte Array to a BitmapSource file that can be displayed */
        public BitmapSource BytesToBitmapSource(byte[] byteArray) {
            BitmapImage bitmapImage = new BitmapImage();
            using (MemoryStream ms = new MemoryStream(byteArray)) {
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = ms;
                bitmapImage.EndInit();
            }
            return bitmapImage;
        }

        /* Method to update UI fields */
        private void UpdateGUI(User user) {
            acctNo_text_box.Text = user.acctNo.ToString();
            pin_text_box.Text = user.pin.ToString("D4");
            balance_text_box.Text = user.balance.ToString("C");
            fName_text_box.Text = user.firstName;
            lName_text_box.Text = user.lastName;
            image_box.Source = BytesToBitmapSource(user.imageBytes);
        }

        private void Go_btnClick(object sender, RoutedEventArgs e) {
            DisableInputs();
            try {
                channel.GetValuesForEntry(Int32.Parse(index_text_box.Text), out uint acctNo, out uint pin, out int bal,
                    out string fName, out string lName, out byte[] imageBytes);
                acctNo_text_box.Text = acctNo.ToString();
                pin_text_box.Text = pin.ToString("D4");
                balance_text_box.Text = bal.ToString("C");
                fName_text_box.Text = fName;
                lName_text_box.Text = lName;
                image_box.Source = BytesToBitmapSource(imageBytes); //TODO: Might need to check for null

            //} catch (FormatException) {
            //    fName_text_box.Text = "Not a valid number";
            //} catch (FaultException<IndexFault> fex) {
            //    Console.WriteLine($"Fault while getting value: {fex.Detail.FunctionName}. Problem: {fex.Detail.Reason}");
            //    fName_text_box.Text = "Index out of range";
            } catch (FaultException fex) {
                Console.WriteLine(fex.Reason);
                status_label.Content = "Invalid Index";
            } catch (CommunicationException cex) {
                Console.WriteLine($"{cex.Message} {cex.InnerException} {cex.StackTrace}");
            }
            /* Re-enable buttons */
            EnableInputs();
        }

        private async void Search_btnClick(object sender, RoutedEventArgs e) {
            DisableInputs();
            try {
                searchString = search_text_box.Text; //field modified here
                String endStatus = "User not found.";
                Task<User> task = new Task<User>(SearchByName); //new task
                task.Start();
                status_label.Visibility = Visibility.Visible;
                status_label.Content = "Searching...";
                User user = await task;
                if(user != null) { //if user is found
                    UpdateGUI(user);
                    endStatus = "End of Search.";
                }
                status_label.Content = endStatus;


            } catch (Exception) {

                throw;
            }
            /* Re-enable buttons */
            EnableInputs();
        }

        private User SearchByName() {
            /* Variables for storing the result */
            channel.GetValuesForSearch(searchString, out uint acctNo, out uint pin, out int bal,
                out string fName, out string lName, out byte[] imgBytes);
            if (acctNo != 0) {
                User user = new User();
                user.acctNo = acctNo;
                user.pin = pin;
                user.balance = bal;
                user.firstName = fName;
                user.lastName = lName;
                user.imageBytes = imgBytes;
                return user;
            }
            return null;
        }
    }
}
