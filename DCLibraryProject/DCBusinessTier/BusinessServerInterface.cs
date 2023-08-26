using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Drawing;
using DCServer;

namespace DCBusinessTier {
    [ServiceContract]
    public interface BusinessServerInterface {
        [OperationContract]
        int GetNumEntries();
        [OperationContract]
        [FaultContract(typeof(IndexFault))]
        void GetValuesForEntry(int index, out uint acctNo, out uint pin, out int bal, out string fName, out string lName, out byte[] imgBytes);

        [OperationContract]
        void GetValuesForSearch(string searchText, out uint acctNo, out uint pin, out int bal, out string fName, out string lName, out byte[] imgBytes);
    }
}
