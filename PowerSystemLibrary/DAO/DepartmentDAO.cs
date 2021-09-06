using PowerSystemLibrary.DBContext;
using PowerSystemLibrary.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemLibrary.DAO
{
    public class DepartmentDAO
    {
        public object List(PowerSystemDBContext db)
        {

            List<Department> departmentList = new List<Department>();
            List<Department> departmentAllList = db.Department.Where(t => t.IsDelete != true).ToList();
            departmentList = departmentAllList.Where(t => (t.ParentID == null || t.ParentID == 0) && t.IsDelete !=true).ToList();
            List<object> List = new List<object>();
            foreach (Department dep in departmentList)
            {
                List.Add(new
                {
                    value = dep.ID,
                    name = dep.Name,
                    pid = dep.ParentID,
                    children = GetChildren(dep.ID, departmentAllList)
                });
            }
            return List;
        }

        private List<object> GetChildren(int pid, List<Department> DepartmentList)
        {
            List<Department> DepList = DepartmentList.Where(p => p.ParentID == pid).ToList();
            List<object> List = new List<object>();
            foreach (Department dep in DepList)
            {
                List.Add(new
                {
                    value = dep.ID,
                    name = dep.Name,
                    pid = dep.ParentID,
                    children = GetChildren(dep.ID, DepartmentList)
                });
            }
            return List;
        }
    }
}
