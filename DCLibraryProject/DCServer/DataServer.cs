using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using DCLibraryProject;
using System.Drawing;

namespace DCServer
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public class DataServer : DataServerInterface
    {
        private DatabaseClass ds;
        public DataServer() { 
            ds = new DatabaseClass(); //intantiates db class
        }
        public int GetNumEntries() {
            return ds.GetNumRecords();
        }

        public void GetValuesForEntry(int index, out uint acctNo, out uint pin, out int bal, out string fName, out string lName, out Bitmap image) {

            try
            {
                acctNo = ds.GetAcctNoByIndex(index);
                pin = ds.GetPINByIndex(index);
                bal = ds.GetBalanceByIndex(index);
                fName = ds.GetFirstNameByIndex(index);
                lName = ds.GetLastNameByIndex(index);
                image = ds.GetImageByIndex(index);

            }
            catch (ArgumentOutOfRangeException)
            {
                IndexFault inf = new IndexFault();
                inf.FunctionFault = "GetValuesForEntry";
                inf.ProblemType = "index out of range";
                throw new FaultException<IndexFault>(inf);
            }

        }
    }
}
