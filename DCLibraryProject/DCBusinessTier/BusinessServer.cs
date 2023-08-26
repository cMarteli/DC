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

namespace DCBusinessTier {
    internal class BusinessServer : BusinessServerInterface {
        private readonly DataServerInterface _businessServer;

        public BusinessServer() {
            ChannelFactory<DataServerInterface> channelFactory;
            NetTcpBinding tcp = new NetTcpBinding();
            //Set the URL and create the connection!
            string URL = "net.tcp://localhost:8100/DataService";
            channelFactory = new ChannelFactory<DataServerInterface>(tcp, URL);
            _businessServer = channelFactory.CreateChannel();
        }
        public int GetNumEntries() {
            return _businessServer.GetNumEntries();
        }

        public void GetValuesForEntry(int index, out uint acctNo, out uint pin, out int bal, out string fName, out string lName, out byte[] imgBytes) {
            _businessServer.GetValuesForEntry(index, out acctNo, out pin, out bal, out fName, out lName, out imgBytes);
        }

        public void GetValuesForSearch(string searchText, out uint acctNo, out uint pin, out int bal, out string fName, out string lName, out byte[] imgBytes) {
            acctNo = 0;
            pin = 0;
            bal = 0;
            fName = "";
            lName = "";
            imgBytes = null;
            int numEntry = _businessServer.GetNumEntries();
            for (int i = 1; i <= numEntry; i++) {
                _businessServer.GetValuesForEntry(i, out uint sAcctNo, out uint sPin, out int sBal, out string sfName, out string slName, out byte[] sImage);
                if (slName.ToLower().Contains(searchText.ToLower())) //searches by last name
                {
                    acctNo = sAcctNo;
                    pin = sPin;
                    bal = sBal;
                    fName = sfName;
                    lName = slName;
                    imgBytes = sImage;

                    break;
                }
            }
            //Thread.Sleep(4000); //Forced sleep for 4000ms
        }
    }
}