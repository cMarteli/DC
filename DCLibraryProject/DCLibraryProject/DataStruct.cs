using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace DCLibraryProject
{
    public class DataStruct
    {
        public uint acctNo;
        public uint pin;
        public int balance;
        public string firstName;
        public string lastName;
        public Bitmap image;

        public DataStruct() //constructor
        {
            acctNo = 0;
            pin = 0;
            balance = 0;
            firstName = "";
            lastName = "";
            image = new Bitmap(1, 1);
        }
    }
}
