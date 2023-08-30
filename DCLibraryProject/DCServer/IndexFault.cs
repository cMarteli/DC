using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

/** Error contract class **/
namespace DCServer {
    [DataContract]
    public class IndexFault {
        private string functionFault;
        private FaultReasonText reasonText;
        private FaultReasonText reason;

        [DataMember]
        public string FunctionName {
            get { return functionFault; }
            set { functionFault = value; }
        }

        [DataMember]
        public FaultReasonText ReasonText {
            get { return reasonText; }
            set { reasonText = value; }
        }

    }
}
