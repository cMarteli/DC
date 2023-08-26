using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using DCServer;

namespace DCBusinessTier
{
    internal class BusinessServer : BusinessServerInterface
    {
        private DataServerInterface _businessServer;

        public BusinessServer() {
            ChannelFactory<DataServerInterface> channelFactory;
            NetTcpBinding tcp = new NetTcpBinding();
            //Set the URL and create the connection!
            string URL = "net.tcp://localhost:8100/DataService";
            channelFactory = new ChannelFactory<DataServerInterface>(tcp, URL);
            _businessServer = channelFactory.CreateChannel();
        }
        public int GetNumEntries()
        {
            return _businessServer.GetNumEntries();
        }

        public void GetValuesForEntry(int index, out uint acctNo, out uint pin, out int bal, out string fName, out string lName, out Bitmap image)
        {
            _businessServer.GetValuesForEntry(index, out acctNo, out pin, out bal, out fName, out lName, out image);
        }

        public void GetValuesForSearch(string searchText, out uint acctNo, out uint pin, out int bal, out string fName, out string lName, out Bitmap image)
        {
            acctNo = 0;
            pin = 0;
            bal = 0;
            fName = "";
            lName = "";
            image = new Bitmap(1, 1);
            int numEntry = _businessServer.GetNumEntries();
            for (int i = 1; i <= numEntry; i++)
            {
                uint sAcctNo, sPin;
                int sBal;
                string sfName, slName;
                Bitmap sImage;

                _businessServer.GetValuesForEntry(i, out sAcctNo, out sPin, out sBal, out sfName, out slName, out sImage);
                if (sfName.ToLower().Contains(searchText.ToLower()))
                {
                    acctNo = sAcctNo;
                    pin = sPin;
                    bal = sBal;
                    fName = sfName;
                    lName = slName;
                    image = sImage;

                    break;
                }
            }
            Thread.Sleep(5000); //Forced sleep for two seconds
        }
    }
}
