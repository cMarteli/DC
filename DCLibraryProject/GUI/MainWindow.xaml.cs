using System;
using System.Collections.Generic;
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
using System.Drawing;

using System.ServiceModel;
using DCServer;
using System.Collections.Specialized;
using System.Windows.Interop;

namespace GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DataServerInterface channel;

        public MainWindow()
        {
            InitializeComponent();

            /* This is a factory that generates remote connections to our remote class. This
            is what hides the RPC stuff! */
            ChannelFactory<DataServerInterface> dataFactory;
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
            Bitmap image = null;
            //On click, Get the index....
            try
            {
                index = Int32.Parse(index_text_box.Text);

            }
            catch (FormatException)
            {
                index_text_box.Text = "NaN"; //TODO: maybe a better error message?
            }
            catch (ArgumentOutOfRangeException)
            {
                IndexFault inf = new IndexFault();
                inf.Operation = "index";
                inf.ProblemType = "out of range";
                throw new FaultException<IndexFault>(inf);
            }
            //Then, run our RPC function, using the out mode parameters...
            try
            {
                channel.GetValuesForEntry(index, out acctNo, out pin, out bal, out fName, out lName, out image);
            }
            catch (FaultException<IndexFault> inf)
            {
                Console.WriteLine("Fault:" + inf.Message);
            }
            
            //And now, set the values in the GUI!
            var handle = image.GetHbitmap();
            
            fName_text_box.Text = fName;
            lName_text_box.Text = lName;
            balance_text_box.Text = bal.ToString("C");
            acctNo_text_box.Text = acctNo.ToString();
            pin_text_box.Text = pin.ToString("D4");

            try
            {
                image_box.Source = Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            catch (Exception ex) { //TODO: remove general exception
                Console.WriteLine("Fault:" + ex.Message);
            }

        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
