using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mashovFinal.Models
{
    public class Department
    {
        int numDepartment;
        string nameDepartment;

        public int NumDepartment { get => numDepartment; set => numDepartment = value; }
        public string NameDepartment { get => nameDepartment; set => nameDepartment = value; }


        public Department() { }
        public Department(int num , string name)
        {
            numDepartment = num;
            nameDepartment = name;
        }

        public List<Department> getList()
        {
            DBservices dbs = new DBservices();
            return dbs.getAllDep();
        }

        public List<string> getYears()
        {
            DBservices dbs = new DBservices();
            return dbs.getAllHebYear();
        }
        public int insert(string d)
        {
            DBservices dbs = new DBservices();
            return dbs.insert(null,d);
        }

    }
}