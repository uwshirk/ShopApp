using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopApp.DBA
{
    class LocationDBA
    {
        private static SqlConnection con = new SqlConnection(Properties.Settings.Default.connection_String);

        #region location
        #region addEditDelete
        public static bool addLocation(Location l)
        {
            string insert = "INSERT INTO Location " +
                            "(AisleLoc, SectionLoc, SpotLoc, Department)" +
                            "VALUES (@AisleLoc, @SectionLoc, @SpotLoc, @Dept) ";

            SqlCommand insertCommand = new SqlCommand(insert, con);
            insertCommand.Parameters.AddWithValue("@AisleLoc", l.AisleLoc);
            insertCommand.Parameters.AddWithValue("@SectionLoc", l.SectionLoc);
            insertCommand.Parameters.AddWithValue("@SpotLoc", l.SpotLoc);
            insertCommand.Parameters.AddWithValue("@Dept", l.LocationDepartment.DepartmentId);

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

        public static bool editLocation(Location l, int eId)
        {
            string update = "UPDATE Location SET AisleLoc = @AisleLoc, SectionLoc = @SectionLoc, SpotLoc = @SpotLoc, Department = @Dept WHERE Id = @EId";

            SqlCommand updateCommand = new SqlCommand(update, con);
            updateCommand.Parameters.AddWithValue("@AisleLoc", l.AisleLoc);
            updateCommand.Parameters.AddWithValue("@SectionLoc", l.SectionLoc);
            updateCommand.Parameters.AddWithValue("@SpotLoc", l.SpotLoc);
            updateCommand.Parameters.AddWithValue("@Dept", l.LocationDepartment.DepartmentId);
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

        public static void deleteLocation(int lId)
        {
            string delete = "DELETE Location WHERE Id = @Id";

            SqlCommand deleteCommand = new SqlCommand(delete, con);
            deleteCommand.Parameters.AddWithValue("@Id", lId);

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
        public static List<Location> getLocations()
        {
            string select = "SELECT * FROM Location";

            SqlCommand selectCommand = new SqlCommand(select, con);
            List<Location> locations = new List<Location>();
            con.Open();
            SqlDataReader reader = selectCommand.ExecuteReader();
            while (reader.Read())
            {
                Department lDepartment = DepartmentDBA.getDepartmentById(Int32.Parse(reader["Department"].ToString()));
                Location l = new Location(Int32.Parse(reader["Id"].ToString()), reader["AisleLoc"].ToString(),
                    reader["SectionLoc"].ToString(), reader["SpotLoc"].ToString(), lDepartment);
                locations.Add(l);
            }
            con.Close();

            return locations;
        }

        public static Location getLocationById(int lId)
        {
            string select = "SELECT * FROM Location WHERE Id = @lId";
            SqlCommand selectCommand = new SqlCommand(select, con);
            selectCommand.Parameters.AddWithValue("@lId", lId);
            Location location = new Location();
            con.Open();
            SqlDataReader reader = selectCommand.ExecuteReader();
            while (reader.Read())
            {
                Department lDepartment = DepartmentDBA.getDepartmentById(Int32.Parse(reader["Department"].ToString()));
                location = new Location(Int32.Parse(reader["Id"].ToString()), reader["AisleLoc"].ToString(),
                    reader["SectionLoc"].ToString(), reader["SpotLoc"].ToString(), lDepartment);
            }
            con.Close();

            return location;
        }

        public static Location getLocationByLoc(String loc)
        {
            String[] locationSplit = loc.Split('-');
            string select = "SELECT * FROM Location WHERE AisleLoc = @AisleLoc AND SectionLoc = @SectionLoc AND SpotLoc = @SpotLoc";
            SqlCommand selectCommand = new SqlCommand(select, con);
            selectCommand.Parameters.AddWithValue("@AisleLoc", locationSplit[0]);
            selectCommand.Parameters.AddWithValue("@SectionLoc", locationSplit[1]);
            selectCommand.Parameters.AddWithValue("@SpotLoc", locationSplit[2]);
            Location location = new Location();
            con.Open();
            SqlDataReader reader = selectCommand.ExecuteReader();
            while (reader.Read())
            {
                Department lDepartment = DepartmentDBA.getDepartmentById(Int32.Parse(reader["Id"].ToString()));
                location = new Location(Int32.Parse(reader["Id"].ToString()), reader["AisleLoc"].ToString(),
                    reader["SectionLoc"].ToString(), reader["SpotLoc"].ToString(), lDepartment);
            }
            con.Close();

            return location;
        }
        #endregion
        #endregion
    }
}
