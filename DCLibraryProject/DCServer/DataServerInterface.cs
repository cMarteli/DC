using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace DCServer
{
    [ServiceContract]
    public interface DataServerInterface
    {
        [OperationContract]
        int GetNumEntries();
        [OperationContract]
        void GetValuesForEntry(int index, out uint acctNo, out uint pin, out int bal, out string fName, out string lName);
    }      

    
}
