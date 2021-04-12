using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopApp
{
    class Department
    {
        #region fields
        //Example values: departmentId = 1; departmentName = Produce; departmentManager = Carl;
        private int departmentId;
        private String departmentName;
        private String departmentManager;

        //unknown if necessary
        //public List<ShopRoute> shopRoutes;

        #endregion

        #region constructors
        public Department()
        {
            this.DepartmentId = 0;
            this.DepartmentName = "";
            this.DepartmentManager = "";
        }

        public Department(int dID, String dName, String dManager)
        {
            this.DepartmentId = dID;
            this.DepartmentName = dName;
            this.DepartmentManager = dManager;
        }
        #endregion

        #region properties
        public int DepartmentId { get { return this.departmentId; } set { this.departmentId = value; } }
        public String DepartmentName { get { return this.departmentName; } set { this.departmentName = value; } }
        public String DepartmentManager { get { return this.departmentManager; } set { this.departmentManager = value; } }
        #endregion

        #region overrides

        public override string ToString()
        {
            return "Department ID: " + DepartmentId + "\nDepartment Name: " + DepartmentName
                + "\nDepartment Manager: " + DepartmentManager;
        }

        public override bool Equals(object obj)
        {
            if (obj is Department)
            {
                Department dept = (Department)obj;
                return (this.DepartmentId == dept.DepartmentId);
            }
            return false;
        }

        #endregion      
    }
}