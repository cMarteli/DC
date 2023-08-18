using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

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
                Bitmap image;

                generator.GetNextAccount(out pin, out acctNo, out firstName, out lastName, out balance, out image);

                // Create a new DataStruct instance and populate its fields
                DataStruct entry = new DataStruct
                {
                    acctNo = acctNo,
                    pin = pin,
                    balance = balance,
                    firstName = firstName,
                    lastName = lastName,
                    image = image
                };

                // Add the generated entry to the list
                dataStruct.Add(entry);
            }
        }

        private Boolean indexNotValid(int index) {
            return (index >= NUMBER_OF_ENTRIES || index < 0);
        } 

        public uint GetAcctNoByIndex(int index) //TODO: Throws ArgumentOutOfRangeException
        {
            if (indexNotValid(index))
            {
                return 0;
            }
            
            return dataStruct[index].acctNo;
        }

        public uint GetPINByIndex(int index)
        {
            if (indexNotValid(index))
            {
                return 0;
            }
            return dataStruct[index].pin;
        }

        public string GetFirstNameByIndex(int index)
        {
            if (indexNotValid(index))
            {
                return "No Entry";
            }
            return dataStruct[index].firstName;
        }

        public string GetLastNameByIndex(int index)
        {
            if (indexNotValid(index))
            {
                return "No Entry";
            }
            return dataStruct[index].lastName;
        }

        public int GetBalanceByIndex(int index)
        {
            if (indexNotValid(index))
            {
                return 0;
            }
            return dataStruct[index].balance;
        }

        public Bitmap GetImageByIndex(int index) //TODO: Throws 'System.ArgumentOutOfRangeException
        {
            return dataStruct[index].image;
        }

        public int GetNumRecords()
        {
            return dataStruct.Count;
        }


    }
}

