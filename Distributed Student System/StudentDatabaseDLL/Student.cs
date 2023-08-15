using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentDLL
{
    public class Student: Person
    {
        private int id;
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

        public override string ToString() 
        {
            string info = "The student's name is " + Name + "\n";
            info=info + "The student's id is " + id + "\n";
            info = info+ "The student's university is " + university;
            return info;
        }
        
    }
}
