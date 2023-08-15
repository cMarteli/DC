using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DCLibraryProject
{
    public class DatabaseClass
    {
        private const int NUMBER_OF_ENTRIES = 1000;

        private List<DataStruct> dataStruct;

        public DatabaseClass()
        {
            dataStruct = new List<DataStruct>();
            LoadData(); // Load data upon construction
        }

        private void LoadData()
        {
            DBGenerator generator = new DBGenerator(); // Create an instance of DBGenerator

            for (int i = 0; i < NUMBER_OF_ENTRIES; i++)
            {
                uint pin, acctNo;
                string firstName, lastName;
                int balance;

                generator.GetNextAccount(out pin, out acctNo, out firstName, out lastName, out balance);

                // Create a new DataStruct instance and populate its fields
                DataStruct entry = new DataStruct
                {
                    acctNo = acctNo,
                    pin = pin,
                    balance = balance,
                    firstName = firstName,
                    lastName = lastName
                };

                // Add the generated entry to the list
                dataStruct.Add(entry);
            }
        }

        public uint GetAcctNoByIndex(int index)
        {
            return dataStruct[index].acctNo;
        }

        public uint GetPINByIndex(int index)
        {
            return dataStruct[index].pin;
        }

        public string GetFirstNameByIndex(int index)
        {
            return dataStruct[index].firstName;
        }

        public string GetLastNameByIndex(int index)
        {
            return dataStruct[index].lastName;
        }

        public int GetBalanceByIndex(int index)
        {
            return dataStruct[index].balance;
        }

        public int GetNumRecords()
        {
            return dataStruct.Count;
        }
    }
}

