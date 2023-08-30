using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using DCDatabase;

namespace DCServer {
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false, IncludeExceptionDetailInFaults = true)]
    internal class DataServer : DataServerInterface {
        public int GetNumEntries() {
            return DatabaseClass.Instance.UserData().Count;
        }

        public void GetValuesForEntry(int index, out uint acctNo, out uint pin, out int bal, out string fName, out string lName, out byte[] imgBytes) {

            try {
                if (index <= 0 || index > DatabaseClass.Instance.UserData().Count) {
                    throw new ArgumentOutOfRangeException();
                }
                List<User> dcData = DatabaseClass.Instance.UserData();
                acctNo = dcData[index - 1].acctNo;
                pin = dcData[index - 1].pin;
                bal = dcData[index - 1].balance;
                fName = dcData[index - 1].firstName;
                lName = dcData[index - 1].lastName;
                imgBytes = dcData[index - 1].imageBytes;

            } catch (FormatException) {
                IndexFault inf = new IndexFault {
                    FunctionName = "GetValuesForEntry"
                };
                inf.ReasonText = new FaultReasonText($"Incorrect Input Type. At function: {inf.FunctionName}");
                FaultReason reason = new FaultReason(inf.ReasonText);

                throw new FaultException(reason);
            } catch (ArgumentOutOfRangeException) {
                IndexFault inf = new IndexFault {
                    FunctionName = "GetValuesForEntry"
                };
                inf.ReasonText = new FaultReasonText($"Index out of range. At function: {inf.FunctionName}");
                FaultReason reason = new FaultReason(inf.ReasonText);

                throw new FaultException(reason);
            }

        }
    }
}