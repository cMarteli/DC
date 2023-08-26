using System;
using System.Drawing;
using System.IO;
using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using DCServer;

namespace GUI
{
    public partial class MainWindow : Window
    {
        private DataServerInterface channel;
        private ChannelFactory<DataServerInterface> dataFactory;

        public MainWindow()
        {
            InitializeComponent();

            NetTcpBinding tcp = new NetTcpBinding();
            string URL = "net.tcp://localhost:8100/DataService";
            dataFactory = new ChannelFactory<DataServerInterface>(tcp, URL);
            channel = dataFactory.CreateChannel();

            total_text_block.Text = "Total Items: " + channel.GetNumEntries().ToString();
        }

        //Method to deserialize the bitmap image
        public static Bitmap ByteArrayToBitmap(byte[] byteArray)
        {
            using (MemoryStream stream = new MemoryStream(byteArray))
            {
                return new Bitmap(stream);
            }
        }

        private void go_btn_Click(object sender, RoutedEventArgs e)
        {
            int index = 0;
            string fName = "", lName = "";
            int bal = 0;
            uint acctNo = 0, pin = 0;
            byte[] imageBytes = null;

            try
            {
                index = Int32.Parse(index_text_box.Text);
            }
            catch (FormatException)
            {
                fName_text_box.Text = "Not a valid number";
            }

            try
            {
                channel.GetValuesForEntry(index, out acctNo, out pin, out bal, out fName, out lName, out imageBytes);
            }
            catch (FaultException<IndexFault> fex)
            {
                Console.WriteLine("Fault while getting value: " + fex.Detail.FunctionName + ". Problem: " + fex.Detail.Reason);
                fName_text_box.Text = "Index out of range";
                dataFactory.Abort();
            }
            catch (CommunicationException cex)
            {
                Console.WriteLine(cex.Message + cex.InnerException + cex.StackTrace);
            }
            finally
            {
                index_text_box.Text = index.ToString();
                fName_text_box.Text = fName;
                lName_text_box.Text = lName;
                balance_text_box.Text = bal.ToString("C");
                acctNo_text_box.Text = acctNo.ToString();
                pin_text_box.Text = pin.ToString("D4");

                if (imageBytes != null)
                {
                    Bitmap image = ByteArrayToBitmap(imageBytes);
                    var handle = image.GetHbitmap();
                    image_box.Source = Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                }
            }
        }
    }
}
