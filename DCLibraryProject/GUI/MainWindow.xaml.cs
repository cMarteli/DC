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
using DCServer;

namespace GUI {
    public delegate void SearchSurname(string value, out uint acctNo, out uint pin, out int bal,
        out string fName, out string lName, out byte[] imgBytes); // Delegate for searching surname
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

        /* Convert Byte Array to Bitmap */
        public static Bitmap ByteArrayToBitmap(byte[] byteArray) {
            using (MemoryStream stream = new MemoryStream(byteArray)) {
                return new Bitmap(stream);
            }
        }

        /* Helper method to update UI fields */
        private void UpdateFields(int index, string fName, string lName, int bal, uint acctNo, uint pin, byte[] imageBytes) {
            index_text_box.Text = index.ToString();
            fName_text_box.Text = fName;
            lName_text_box.Text = lName;
            balance_text_box.Text = bal.ToString("C");
            acctNo_text_box.Text = acctNo.ToString();
            pin_text_box.Text = pin.ToString("D4");

            if (imageBytes != null) {
                using (Bitmap image = ByteArrayToBitmap(imageBytes)) {
                    var handle = image.GetHbitmap();
                    image_box.Source = Imaging.CreateBitmapSourceFromHBitmap(
                        handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                }
            }
        }

        private void go_index_btn_Click(object sender, RoutedEventArgs e) {
            try {
                int index = Int32.Parse(index_text_box.Text);
                channel.GetValuesForEntry(index, out uint acctNo, out uint pin, out int bal, out string fName, out string lName, out byte[] imageBytes);
                UpdateFields(index, fName, lName, bal, acctNo, pin, imageBytes);
            } catch (FormatException) {
                fName_text_box.Text = "Not a valid number";
            } catch (FaultException<IndexFault> fex) {
                Console.WriteLine($"Fault while getting value: {fex.Detail.FunctionName}. Problem: {fex.Detail.Reason}");
                fName_text_box.Text = "Index out of range";
            } catch (CommunicationException cex) {
                Console.WriteLine($"{cex.Message} {cex.InnerException} {cex.StackTrace}");
            }
        }

        private void search_surname_btn_Click(object sender, RoutedEventArgs e) {
            /* Initialize the delegate and callback */
            searchSurname = channel.GetValuesForSearch;
            AsyncCallback callback = this.OnSearchSurnameCompletion;

            /* Start the asynchronous operation */
            IAsyncResult result = searchSurname.BeginInvoke(search_text_box.Text, out uint acctNo,
            out uint pin, out int bal, out string fName, out string lName, out byte[] imgBytes, callback, null);
        }

        private void OnSearchSurnameCompletion(IAsyncResult asyncResult) {
            /* Variables for storing the result */
            uint acctNo;
            uint pin;
            int bal;
            string fName;
            string lName;
            byte[] imgBytes;
            int index = 0;  // TODO: Retrieve the actual index if required

            /* End the asynchronous call and get the results */
            AsyncResult asyncObj = (AsyncResult)asyncResult;
            if (!asyncObj.EndInvokeCalled) {
                searchSurname.EndInvoke(out acctNo, out pin, out bal, out fName, out lName, out imgBytes, asyncObj);

                /* Update the GUI */
                this.Dispatcher.Invoke(new Action(() => UpdateFields(index, fName, lName, bal, acctNo, pin, imgBytes)));
            }

            asyncObj.AsyncWaitHandle.Close(); //Clean up
        }
    }
}
