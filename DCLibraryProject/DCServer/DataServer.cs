using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using DCDatabase;

namespace DCServer {
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    internal class DataServer : DataServerInterface {
        public int GetNumEntries() {
            return DatabaseClass.Instance.UserData().Count;
        }

        public void GetValuesForEntry(int index, out uint acctNo, out uint pin, out int bal, out string fName, out string lName, out byte[] imgBytes) {

            try {
                List<User> dcData = DatabaseClass.Instance.UserData();
                acctNo = dcData[index - 1].acctNo;
                pin = dcData[index - 1].pin;
                bal = dcData[index - 1].balance;
                fName = dcData[index - 1].firstName;
                lName = dcData[index - 1].lastName;
                imgBytes = dcData[index - 1].imageBytes;

            } catch (FormatException) {
                IndexFault inf = new IndexFault();
                inf.FunctionName = "GetValuesForEntry";
                inf.Reason = "Incorrect Input Type";
                throw new FaultException<IndexFault>(inf);
            } catch (ArgumentOutOfRangeException) {
                IndexFault inf = new IndexFault();
                inf.FunctionName = "GetValuesForEntry";
                inf.Reason = "index out of range";
                throw new FaultException<IndexFault>(inf);
            }

        }
    }
}