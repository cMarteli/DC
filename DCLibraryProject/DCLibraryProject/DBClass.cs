using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DCLibraryProject
{
    internal class DatabaseClass
    {
        List<DataStruct> dataStruct;

        public DatabaseClass()
        {
            dataStruct = new List<DataStruct>();
        }
        public uint GetAcctNoByIndex(int index)
        {
            return 0;
        }
        public uint GetPINByIndex(int index)
        {
            return 0;
        }
        public string GetFirstNameByIndex(int index)
        {
            return null;
        }
        public string GetLastNameByIndex(int index)
        {
            return null;
        }
        public int GetBalanceByIndex(int index)
        {
            return 0;

        }
        public int GetNumRecords()
        {
            return 0;
        }
    }
    

}
