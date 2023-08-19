using System;
using System.Drawing;
using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using DCServer;

namespace GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DataServerInterface channel;
        private ChannelFactory<DataServerInterface> dataFactory;

        public MainWindow()
        {
            InitializeComponent();

            /* This is a factory that generates remote connections to our remote class. This
            is what hides the RPC stuff! */
            //ChannelFactory<DataServerInterface> dataFactory;
            NetTcpBinding tcp = new NetTcpBinding();

            //Set the URL and create the connection!
            string URL = "net.tcp://localhost:8100/DataService";
            dataFactory = new ChannelFactory<DataServerInterface>(tcp, URL);
            channel = dataFactory.CreateChannel();

            //Also, tell me how many entries are in the DB.
            total_text_block.Text = "Total Items: " + channel.GetNumEntries().ToString();
            
        }

        private void go_btn_Click(object sender, RoutedEventArgs e)
        {
            int index = 0;
            string fName = "", lName = "";
            int bal = 0;
            uint acctNo = 0, pin = 0;
            Bitmap image = new Bitmap(1,1);
            //On click, Get the index....
            try {
                index = Int32.Parse(index_text_box.Text);
            }
            catch (FormatException) {
                fName_text_box.Text = "Not a valid number";
            }
            //Then, run our RPC function, using the out mode parameters...
            try {
                channel.GetValuesForEntry(index, out acctNo, out pin, out bal, out fName, out lName, out image);
            }
            catch (FaultException<IndexFault> fex) {
                Console.WriteLine("FaultException<IndexFault>: Fault while getting value " + fex.Detail.FunctionFault + ". Problem: " + fex.Detail.ProblemType);
                fName_text_box.Text = "Index out of range";
                dataFactory.Abort();
            }
            catch (CommunicationException cex) {
                
                Console.WriteLine(cex.Message);                
                Console.WriteLine(cex.InnerException);
                Console.WriteLine("**********************************************************TEST**********************************************************");
                Console.WriteLine(cex.StackTrace);
            } //TODO: this is throwing when same value is loaded twice
            finally {
                //Set the values in the GUI
                index_text_box.Text = index.ToString();
                fName_text_box.Text = fName;
                lName_text_box.Text = lName;
                balance_text_box.Text = bal.ToString("C");
                acctNo_text_box.Text = acctNo.ToString();
                pin_text_box.Text = pin.ToString("D4");
                //converts bitmap
                if (image != null) {
                    var handle = image.GetHbitmap();
                    image_box.Source = Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                }

            }
            
        }


    }
}
