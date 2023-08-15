using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class StudentList
    {

        public static List<Student> Students() 
        {
            List<Student> slist = new List<Student>();

            Student student1 = new Student();
            student1.Name = "Robert";
            student1.University = "Curtin";
            student1.Id = 101;
            student1.CWA = 61;

            Student student2 = new Student();
            student2.Name = "Mia";
            student2.University = "EWU";
            student2.Id = 102;
            student2.CWA = 76;

            Student student3 = new Student();
            student3.Name = "Adam";
            student3.University = "Murdoch";
            student3.Id = 103;
            student3.CWA = 81;

            Student student4 = new Student();
            student4.Name = "Chola";
            student4.University = "Notre Dame";
            student4.Id = 104;
            student4.CWA = 50;

            Student student5 = new Student();
            student5.Name = "Johny";
            student5.University = "UWA";
            student5.Id = 105;
            student5.CWA = 57;

            Student student6 = new Student();
            student6.Name = "Jane";
            student6.University = "ECU";
            student6.Id = 106;
            student6.CWA = 89;

            slist.Add(student1);
            slist.Add(student2);
            slist.Add(student3);
            slist.Add(student4);
            slist.Add(student5);
            slist.Add(student6);

            return slist;

        }
    }
}
