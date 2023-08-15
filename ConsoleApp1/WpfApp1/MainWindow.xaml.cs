using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ConsoleApp1;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        /** Search Button event click - Finds student name give ID **/
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string val = Student_ID_Box.Text; //id to search for
            int intVal = 0;
            try{ intVal = int.Parse(val);
            }
            catch (FormatException){ Console.WriteLine("FormatException"); 
            }
            String searchFail = "No target found!";
            Boolean searchSuccess = false;
            Student target = new Student();
            List<Student> studentlist = StudentList.Students();

            foreach (Student student in studentlist)
            {
                if(student.Id == intVal){
                    target = student;
                    searchSuccess = true;
                }
            }
            if (searchSuccess){
                Student_Name_Box.Text = target.Name;
                University_Name_Box.Text = target.University;
                CWA_Box.Text = target.CWA.ToString();
            }
            else{
                Student_Name_Box.Text = searchFail;
                University_Name_Box.Text = searchFail;
                CWA_Box.Text = searchFail;
            }
        }
    }
}
