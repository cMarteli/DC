using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using DCServer;

namespace DCBusinessTier {
    internal class BusinessServer : BusinessServerInterface {
        private readonly DataServerInterface _businessServer;
        /* New field for log count */
        private uint LogNumber;

        public BusinessServer() {
            ChannelFactory<DataServerInterface> channelFactory;
            NetTcpBinding tcp = new NetTcpBinding();
            //Set the URL and create the connection!
            string URL = "net.tcp://localhost:8100/DataService";
            channelFactory = new ChannelFactory<DataServerInterface>(tcp, URL);
            _businessServer = channelFactory.CreateChannel();

            /* Initialize log count */
            LogNumber = 0;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Log(string logString) {
            LogNumber++;
            string logEntry = $"Log #{LogNumber}: {logString}\n";
            
            File.AppendAllText("BusinessServerLog.txt", logEntry);// Append log to a file

            Console.WriteLine(logEntry) ;// Optional: Also write to console
        }
        public int GetNumEntries() {
            int numEntries = _businessServer.GetNumEntries();
            Log($"GetNumEntries called. Number of Entries: {numEntries}");
            return numEntries;
        }

        public void GetValuesForEntry(int index, out uint acctNo, out uint pin, out int bal, out string fName, out string lName, out byte[] imgBytes) {
            _businessServer.GetValuesForEntry(index, out acctNo, out pin, out bal, out fName, out lName, out imgBytes);
            Log($"GetValuesForEntry called with index: {index}. Account No: {acctNo}, PIN: {pin}, Balance: {bal}, First Name: {fName}, Last Name: {lName}");
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
            /* Add a log entry */
            if (acctNo != 0) {
                Log($"GetValuesForSearch called with search text: '{searchText}'. Match found with Account No: {acctNo}, PIN: {pin}, Balance: {bal}, First Name: {fName}, Last Name: {lName}");
            }
            else {
                Log($"GetValuesForSearch called with search text: '{searchText}'. No match found.");
            }
        }
    }
}