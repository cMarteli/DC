using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class Student : Person
    {
        private int id, cwa;
        private string university;

        public int Id 
        { 
            get { return id; }
            set { id = value; }
        }

        public string University
        {
            get { return university; }
            set { university = value; }
        }

        public int CWA
        {
            get { return cwa; }
            set { cwa = value; }
        }

        public override string ToString()
        {
            string info = "The students name is " + Name + "\n";
            info = info + "The student's id is " + id + "\n";
            info = info + "The student's university is " + university + "\n";
            info = info + "The student's cwa is " + cwa;
            return info;
        }
    }
}
