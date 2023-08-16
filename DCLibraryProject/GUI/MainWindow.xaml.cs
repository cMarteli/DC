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

using System.ServiceModel;
using DCServer;
using System.Collections.Specialized;

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
            try {
                channel = dataFactory.CreateChannel();
            }
            catch (Exception)
            {
                Console.WriteLine("Exception!");
            }
            
            //Also, tell me how many entries are in the DB.
            total_text_block.Text = channel.GetNumEntries().ToString();
        }

        private void go_btn_Click(object sender, RoutedEventArgs e)
        {
            int index = 0;
            string fName = "", lName = "";
            int bal = 0;
            uint acct = 0, pin = 0;
            //On click, Get the index....
            index = Int32.Parse(index_text_box.Text);
            //Then, run our RPC function, using the out mode parameters...
            channel.GetValuesForEntry(index, out acct, out pin, out bal, out fName, out lName);
            //And now, set the values in the GUI!
            fName_text_box.Text = fName;
            lName_text_box.Text = lName;
            balance_text_box.Text = bal.ToString("C");
            acctNo_text_box.Text = acct.ToString();
            pin_text_box.Text = pin.ToString("D4");

        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
