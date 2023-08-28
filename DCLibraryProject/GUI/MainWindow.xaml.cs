using System;
using System.Drawing;
using System.IO;
using System.Runtime.Remoting.Messaging;
using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using DCBusinessTier;
using DCDatabase;
using System.Security.Cryptography;
using System.Reflection;
using System.Threading;

namespace GUI {
    public delegate DataStruct SearchSurname(string searchString); // Delegate for searching surname
    public partial class MainWindow : Window {
        private BusinessServerInterface channel;
        private ChannelFactory<BusinessServerInterface> dataFactory;
        private SearchSurname searchSurname; // Reference to search surname method

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
            //bitmapImage.Freeze(); //makes it shareable accross threads
            return bitmapImage;
        }

        /* Method to update UI fields */
        private void UpdateGUI(DataStruct user) {
            // Update all text fields at once
            Dispatcher.Invoke(() => {
                acctNo_text_box.Text = user.acctNo.ToString();
                pin_text_box.Text = user.pin.ToString("D4");
                balance_text_box.Text = user.balance.ToString("C");
                fName_text_box.Text = user.firstName;
                lName_text_box.Text = user.lastName;

                /* Updates image. ***NOTE: Code repetition but if we try to call BytesToBitmapSource() from here
                 * we get a System.InvalidOperationException */
                using (MemoryStream ms = new MemoryStream(user.imageBytes)) {
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.StreamSource = ms;
                    bitmapImage.EndInit();
                    image_box.Source = bitmapImage;
                }
            });
        }

        private void Go_btnClick(object sender, RoutedEventArgs e) {
            try {
                channel.GetValuesForEntry(Int32.Parse(index_text_box.Text), out uint acctNo, out uint pin, out int bal,
                    out string fName, out string lName, out byte[] imageBytes);
                acctNo_text_box.Text = acctNo.ToString();
                pin_text_box.Text = pin.ToString("D4");
                balance_text_box.Text = bal.ToString("C");
                fName_text_box.Text = fName;
                lName_text_box.Text = lName;
                image_box.Source = BytesToBitmapSource(imageBytes); //TODO: Might need to check for null

            } catch (FormatException) {
                fName_text_box.Text = "Not a valid number";
            } catch (FaultException<IndexFault> fex) {
                Console.WriteLine($"Fault while getting value: {fex.Detail.FunctionName}. Problem: {fex.Detail.Reason}");
                fName_text_box.Text = "Index out of range";
            } catch (CommunicationException cex) {
                Console.WriteLine($"{cex.Message} {cex.InnerException} {cex.StackTrace}");
            }
        }
        /* Search button click method, Runs on a separate thread */
        private void Search_btnClick(object sender, RoutedEventArgs e) {
            
            searchSurname = SearchDB; //points delegate to SearchDB
            AsyncCallback callback = this.OnSearchCompletion; // Initialize the delegate and callback

            /* Start the asynchronous operation */
            IAsyncResult result = searchSurname.BeginInvoke(search_text_box.Text, callback, null); //TODO; **searchSurname** was null.
        }

        private DataStruct SearchDB(string searchString) {
            /* Variables for storing the result */
            channel.GetValuesForSearch(searchString, out uint acctNo, out uint pin, out int bal, 
                out string fName, out string lName, out byte[] imgBytes); //delegate assigned
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
        }

        private void OnSearchCompletion(IAsyncResult asyncResult) {

            DataStruct user = null;
            SearchSurname searchSurname = null;            
            AsyncResult asyncObj = (AsyncResult)asyncResult; // End the asynchronous call and get the results
            if (!asyncObj.EndInvokeCalled) {
                searchSurname = (SearchSurname)asyncObj.AsyncDelegate;
                user = searchSurname.EndInvoke(asyncObj);                
                UpdateGUI(user); // Update the GUI
            }

            asyncObj.AsyncWaitHandle.Close(); //Clean up
        }
    }
}
