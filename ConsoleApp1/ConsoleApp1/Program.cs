using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<Student> studentList = StudentList.Students();

            foreach (Student student in studentList)
            {
                Console.WriteLine("**********************************************");
                Console.WriteLine(student);

            }
            Console.WriteLine("**********************************************");
            Console.ReadLine();
        }
    }
}
