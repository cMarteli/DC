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
        private void UpdateGUI(DataStruct user) {
            Dispatcher.Invoke(() => {
                acctNo_text_box.Text = user.acctNo.ToString();
                pin_text_box.Text = user.pin.ToString("D4");
                balance_text_box.Text = user.balance.ToString("C");
                fName_text_box.Text = user.firstName;
                lName_text_box.Text = user.lastName;
                image_box.Source = BytesToBitmapSource(user.imageBytes);
            });
        }

        private async void Go_btnClick(object sender, RoutedEventArgs e) {
            try {
                await Task.Run(() => {
                    channel.GetValuesForEntry(Int32.Parse(index_text_box.Text), out uint acctNo, out uint pin, out int bal,
                        out string fName, out string lName, out byte[] imageBytes);
                    Dispatcher.Invoke(() => {
                        acctNo_text_box.Text = acctNo.ToString();
                        pin_text_box.Text = pin.ToString("D4");
                        balance_text_box.Text = bal.ToString("C");
                        fName_text_box.Text = fName;
                        lName_text_box.Text = lName;
                        image_box.Source = BytesToBitmapSource(imageBytes);
                    });
                });
            } catch (FormatException) {
                fName_text_box.Text = "Not a valid number";
            } catch (FaultException<IndexFault> fex) {
                Console.WriteLine($"Fault while getting value: {fex.Detail.FunctionName}. Problem: {fex.Detail.Reason}");
                fName_text_box.Text = "Index out of range";
            } catch (CommunicationException cex) {
                Console.WriteLine($"{cex.Message} {cex.InnerException} {cex.StackTrace}");
            }
        }

        private async void Search_btnClick(object sender, RoutedEventArgs e) {
            DataStruct user = await SearchDBAsync(search_text_box.Text);
            if (user != null) {
                UpdateGUI(user);
            }
        }

        private async Task<DataStruct> SearchDBAsync(string searchString) {
            return await Task.Run(() => {
                channel.GetValuesForSearch(searchString, out uint acctNo, out uint pin, out int bal,
                    out string fName, out string lName, out byte[] imgBytes);
                if (acctNo != 0) {
                    DataStruct user = new DataStruct();
                    user.acctNo = acctNo;
                    user.pin = pin;
                    user.balance = bal;
                    user.firstName = fName;
                    user.lastName = lName;
                    user.imageBytes = imgBytes;
                    return user;
                }
                return null;
            });
        }
    }
}
