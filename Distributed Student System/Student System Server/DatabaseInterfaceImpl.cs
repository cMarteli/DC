using StudentDLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Student_System_Server
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public class DatabaseInterfaceImpl : DatabaseInterface
    {
        public int GetNumEntries()
        {
            return StudentList.Students().Count;
        }

        public void GetValuesForEntry(int index, out string name, out int id, out string university)
        {
            List<Student> slist = StudentList.Students();
            name = slist[index - 1].Name;
            id = slist[index - 1].Id;
            university = slist[index - 1].University;
        }
    }
}
