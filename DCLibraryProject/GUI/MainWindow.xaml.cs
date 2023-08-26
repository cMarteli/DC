using System;
using System.Drawing;
using System.IO;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using DCBusinessTier;

namespace GUI {
    public partial class MainWindow : Window {
        private BusinessServerInterface channel;
        private ChannelFactory<BusinessServerInterface> dataFactory;

        public MainWindow() {
            Thread.Sleep(1000); //Forced sleep for 1000ms
            InitializeComponent();

            NetTcpBinding tcp = new NetTcpBinding();
            string URL = "net.tcp://localhost:8200/BusinessService";
            dataFactory = new ChannelFactory<BusinessServerInterface>(tcp, URL);
            channel = dataFactory.CreateChannel();

            //total_text_block.Text = "Total Items: " + channel.GetNumEntries().ToString();
            total_text_block.Text = "Total Items: " + 1000; //TODO: hardcoded change back!
        }

        //Method to deserialize the bitmap image
        public static Bitmap ByteArrayToBitmap(byte[] byteArray) {
            using (MemoryStream stream = new MemoryStream(byteArray)) {
                return new Bitmap(stream);
            }
        }

        private void go_btn_Click(object sender, RoutedEventArgs e) {
            int index = 0;
            string fName = "", lName = "";
            int bal = 0;
            uint acctNo = 0, pin = 0;
            byte[] imageBytes = null;

            try {
                index = Int32.Parse(index_text_box.Text);
            } catch (FormatException) {
                fName_text_box.Text = "Not a valid number";
            }

            try {
                channel.GetValuesForEntry(index, out acctNo, out pin, out bal, out fName, out lName, out imageBytes);
            }
            //catch (FaultException<IndexFault> fex)
            catch (Exception fex) //TODO: Change back
            {
                //Console.WriteLine("Fault while getting value: " + fex.Detail.FunctionName + ". Problem: " + fex.Detail.Reason);
                fName_text_box.Text = "Index out of range";
                //dataFactory.Abort();
            }
            //catch (CommunicationException cex) //TODO: Change back
            //{
            //    Console.WriteLine(cex.Message + cex.InnerException + cex.StackTrace);
            //}
            finally {
                index_text_box.Text = index.ToString();
                fName_text_box.Text = fName;
                lName_text_box.Text = lName;
                balance_text_box.Text = bal.ToString("C");
                acctNo_text_box.Text = acctNo.ToString();
                pin_text_box.Text = pin.ToString("D4");

                if (imageBytes != null) {
                    Bitmap image = ByteArrayToBitmap(imageBytes);
                    var handle = image.GetHbitmap();
                    image_box.Source = Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                }
            }
        }
    }
}
