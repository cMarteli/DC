using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

//Error contract class
namespace DCServer
{
    [DataContract]
    public class IndexFault
    {
        private string functionFault;
        private string problemType;

        [DataMember]
        public string FunctionFault
        {
            get { return functionFault; }
            set { functionFault = value; }
        }

        [DataMember]
        public string ProblemType
        {
            get { return problemType; }
            set { problemType = value; }
        }
    }
}
