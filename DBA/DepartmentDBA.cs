using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopApp.DBA
{
    class DepartmentDBA
    {
        private static SqlConnection con = new SqlConnection(Properties.Settings.Default.connection_String);

        #region department
        #region addEditDelete
        public static bool addDepartment(Department d)
        {
            string insert = "INSERT INTO Department " +
                            "(Id, Name, Manager)" +
                            "VALUES (@Id, @Name, @Manager) ";

            SqlCommand insertCommand = new SqlCommand(insert, con);
            insertCommand.Parameters.AddWithValue("@Id", d.DepartmentId);
            insertCommand.Parameters.AddWithValue("@Name", d.DepartmentName);
            insertCommand.Parameters.AddWithValue("@Manager", d.DepartmentManager);

            int count = 0;
            try
            {
                con.Open();
                count = insertCommand.ExecuteNonQuery();
            }
            catch (SqlException ex) { throw ex; }
            finally { con.Close(); }

            return count > 0;
        }

        public static bool editDepartment(Department d, int eId)
        {
            string update = "UPDATE Department SET Id = @Id, Name = @Name, Manager = @Manager WHERE Id = @EId";

            SqlCommand updateCommand = new SqlCommand(update, con);
            updateCommand.Parameters.AddWithValue("@Id", d.DepartmentId);
            updateCommand.Parameters.AddWithValue("@Name", d.DepartmentName);
            updateCommand.Parameters.AddWithValue("@Manager", d.DepartmentManager);
            updateCommand.Parameters.AddWithValue("@EId", eId);

            int count = 0;
            try
            {
                con.Open();
                count = updateCommand.ExecuteNonQuery();
            }
            catch (SqlException ex) { throw ex; }
            finally { con.Close(); }

            return count > 0;
        }

        public static void deleteDepartment(int dId)
        {
            string delete = "DELETE Department WHERE Id = @Id";

            SqlCommand deleteCommand = new SqlCommand(delete, con);
            deleteCommand.Parameters.AddWithValue("@Id", dId);

            try
            {
                con.Open();
                deleteCommand.ExecuteNonQuery();
            }
            catch (Exception ex) { throw ex; }
            finally { con.Close(); }
        }
        #endregion
        #region getters
        public static List<Department> getDepartments()
        {
            string select = "SELECT * FROM Department";

            SqlCommand selectCommand = new SqlCommand(select, con);
            List<Department> departments = new List<Department>();
            con.Open();
            SqlDataReader reader = selectCommand.ExecuteReader();
            while (reader.Read())
            {
                Department d = new Department(Int32.Parse(reader["Id"].ToString()), reader["Name"].ToString(),
                    reader["Manager"].ToString());
                departments.Add(d);
            }
            con.Close();

            return departments;
        }

        public static Department getDepartmentById(int dId)
        {
            string select = "SELECT * FROM Department WHERE Id = @Id";
            SqlCommand selectCommand = new SqlCommand(select, con);
            selectCommand.Parameters.AddWithValue("@id", dId);
            Department department = new Department();
            con.Open();
            SqlDataReader reader = selectCommand.ExecuteReader();
            while (reader.Read())
            {
                department = new Department(Int32.Parse(reader["Id"].ToString()), reader["Name"].ToString(),
                    reader["Manager"].ToString());
            }
            con.Close();

            return department;
        }
        #endregion
        #endregion
    }
}
