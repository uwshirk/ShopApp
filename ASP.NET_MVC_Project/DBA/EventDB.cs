using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;
using EmailTestApp.Models;

namespace EmailTestApp.DBA
{
    public static class EventDB
    {
        //private static string conString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        private static SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        private static string select, delete, update, insert;

        #region EventsTable
        public static List<EventModel> getLeagueEvents(string league)
        {
            select = "SELECT League, EventName, EventDate, StartTime, EndTime, Location, EventsTable.Description " +
                            "FROM EventsTable right join League on League = LeagueName " +
                            "WHERE League = @league ORDER BY League";

            SqlCommand selectCommand = new SqlCommand(select, con);
            selectCommand.Parameters.AddWithValue("@league", league);
            List<EventModel> events = new List<EventModel>();

            try
            {
                con.Open();
                SqlDataReader reader = selectCommand.ExecuteReader();
                while (reader.Read())
                {
                    EventModel e = new EventModel(reader["League"].ToString(),
                        reader["EventName"].ToString(), reader["EventDate"].ToString(),
                        reader["StartTime"].ToString(), reader["EndTime"].ToString(),
                        reader["Location"].ToString(), reader["Description"].ToString());
                    events.Add(e);
                }
            }
            catch { }
            finally { con.Close(); }
            
            return events;
        }
        //this method gets all events from the current date to the end month set in HomeController
        public static List<EventModel> getCurrentEvents(int endMonth)
        {
            List<EventModel> results = new List<EventModel>();
            select = "";

            DateTime currentDate = DateTime.Now;
            //if the end month was 0 then we just get all events
            if (endMonth < 1)
            {
                select = "SELECT * FROM EventsTable WHERE CAST(EventDate AS Date) >= '" + currentDate.ToShortDateString() +
                    "'  ORDER BY EventDate";
            }
            //otherwise we get them from now until the end month
            else
            {
                DateTime addMonthDate = DateTime.Now.AddMonths(endMonth);
                select = "SELECT * FROM EventsTable WHERE CAST(EventDate AS Date) < '" + addMonthDate.ToShortDateString() + 
                    "' AND CAST(EventDate AS Date) >= '" + currentDate.ToShortDateString() + "'  ORDER BY EventDate";
            }            
            
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
            SqlCommand cmd = new SqlCommand(select, con);
            try
            {
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    results.Add(new EventModel(reader["League"].ToString(), reader["EventName"].ToString(),
                                reader["EventDate"].ToString(), reader["StartTime"].ToString(),
                                reader["EndTime"].ToString(), reader["Location"].ToString(),
                                reader["Description"].ToString()));
                }
            }
            catch { }
            finally { con.Close(); }
            
            return results;
        }
        //same as the getCurrentEvents(int endMonth) but these events are filtered by user
        public static List<EventModel> getCurrentEventsByUser(int endMonth, string userName)
        {
            List<EventModel> results = new List<EventModel>();
            select = "";

            DateTime currentDate = DateTime.Now;

            if (endMonth < 1)
            {
                select = "SELECT E.League, E.EventName, E.EventDate, E.StartTime, E.EndTime, E.Location, E.Description" +
                    " FROM EventsTable E, League_Members LM" +
                    " WHERE CAST([EventsTable].EventDate AS Date) >= '" + currentDate.ToShortDateString() +
                    "' AND LM.MembEmail = '" + userName +
                    "' AND E.League = LM.League ORDER BY E.EventDate";
            }
            else
            {
                DateTime addMonthDate = DateTime.Now.AddMonths(endMonth);
                select = "SELECT E.League, E.EventName, E.EventDate, E.StartTime, E.EndTime, E.Location, E.Description" +
                    " FROM EventsTable E, League_Members LM" +
                    " WHERE CAST(E.EventDate AS Date) < '" + addMonthDate.ToShortDateString() +
                    "' AND CAST(E.EventDate AS Date) >= '" + currentDate.ToShortDateString() +
                    "' AND LM.MembEmail = '" + userName +
                    "' AND E.League = LM.League  ORDER BY E.EventDate";
            }

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
            SqlCommand cmd = new SqlCommand(select, con);
            try
            {
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    results.Add(new EventModel(reader["League"].ToString(), reader["EventName"].ToString(),
                                reader["EventDate"].ToString(), reader["StartTime"].ToString(),
                                reader["EndTime"].ToString(), reader["Location"].ToString(),
                                reader["Description"].ToString()));
                }
            }
            catch { }
            finally { con.Close(); }

            return results;
        }

        public static bool addEvent(EventModel e)
        {
            insert = "INSERT INTO EventsTable (League, EventName, EventDate, StartTime, " +
                            "EndTime, Location, Description) " +
                            "Values (@LName, @EName, @EDate, @Start, @End, @Loc, @Desc)";

            SqlCommand insertCommand = new SqlCommand(insert, con);
            insertCommand.Parameters.AddWithValue("@LName", e.LeagueRef);
            insertCommand.Parameters.AddWithValue("@EName", e.EventName);
            insertCommand.Parameters.AddWithValue("@EDate", e.Date);
            insertCommand.Parameters.AddWithValue("@Start", e.StartTime);
            insertCommand.Parameters.AddWithValue("@End", e.EndTime);
            insertCommand.Parameters.AddWithValue("@Desc", e.Description);

            int count = 0;
            try
            {
                con.Open();
                count = insertCommand.ExecuteNonQuery();
            }
            catch (Exception ex) { throw ex; }
            finally { con.Close(); }

            return count > 0;
        }

        public static EventModel getEvent(string date, string league)
        {
            select = "SELECT * FROM EventsTable " +
                            "WHERE League = @league and EventDate = @date";

            SqlCommand selectCommand = new SqlCommand(select, con);
            selectCommand.Parameters.AddWithValue("@league", league);
            selectCommand.Parameters.AddWithValue("@date", date);
            EventModel e = new EventModel();

            try
            {
                con.Open();
                SqlDataReader reader = selectCommand.ExecuteReader();
                while (reader.Read())
                {
                    e = new EventModel(reader["League"].ToString(), reader["EventName"].ToString(),
                                reader["EventDate"].ToString(), reader["StartTime"].ToString(),
                                reader["EndTime"].ToString(), reader["Location"].ToString(),
                                reader["Description"].ToString());
                }
            }
            catch { }
            finally { con.Close(); }
            
            return e;
        }

        public static EventModel getNextLeagueEvent(string league)
        {
            select = "SELECT TOP 1 * FROM EventsTable " +
                            "WHERE League = @league order by EventDate";

            SqlCommand selectCommand = new SqlCommand(select, con);
            selectCommand.Parameters.AddWithValue("@league", league);
            EventModel e = new EventModel();

            try
            {
                con.Open();
                SqlDataReader reader = selectCommand.ExecuteReader();
                while (reader.Read())
                {
                    e = new EventModel(reader["League"].ToString(), reader["EventName"].ToString(),
                                reader["EventDate"].ToString(), reader["StartTime"].ToString(),
                                reader["EndTime"].ToString(), reader["Location"].ToString(),
                                reader["Description"].ToString());
                }
            }
            catch { }
            finally { con.Close(); }

            return e;
        }

        public static string getEventLeague(string eName, string eDate)
        {
            select = "SELECT League FROM EventsTable " +
                            "WHERE EventName = @name and EventDate = @date";

            SqlCommand selectCommand = new SqlCommand(select, con);
            selectCommand.Parameters.AddWithValue("@name", eName);
            selectCommand.Parameters.AddWithValue("@date", DateTime.Parse(eDate));
            string league = "";

            try
            {
                con.Open();
                SqlDataReader reader = selectCommand.ExecuteReader();
                while (reader.Read())
                {
                    league = reader["League"].ToString();
                }
            }
            catch { }
            finally { con.Close(); }
            
            return league;
        }

        public static void deleteEvent(EventModel eve)
        {
            delete = "DELETE FROM EventsTable " +
                            "WHERE EventDate = @date and EventName = @name";

            SqlCommand deleteCommand = new SqlCommand(delete, con);
            deleteCommand.Parameters.AddWithValue("@date", eve.Date);
            deleteCommand.Parameters.AddWithValue("@name", eve.EventName);

            try
            {
                con.Open();
                deleteCommand.ExecuteNonQuery();
            }
            catch (Exception ex) { throw ex; }
            finally { con.Close(); }
        }
        #endregion

        #region Groups
        public static bool createGroup(GroupModel group)
        {
            insert = "INSERT Groups VALUES (@gname, @event, @date, @start, 0, @limit)";
            SqlCommand insertCommand = new SqlCommand(insert, con);
            insertCommand.Parameters.AddWithValue("@gname", group.GroupName);
            insertCommand.Parameters.AddWithValue("@event", group.EventName);
            insertCommand.Parameters.AddWithValue("@date", group.EventDate);
            insertCommand.Parameters.AddWithValue("@start", group.StartTime);
            insertCommand.Parameters.AddWithValue("@limit", group.Limit);
            
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

        public static bool updateGroup(GroupModel group, int gID)
        {
            update = "UPDATE Groups SET GroupName = @gname, EventName = @event, "
                +"EventDate = @date, StartTime = @start, GroupScore = @score, Limit = @limit WHERE Group_ID = @id";

            SqlCommand updateCommand = new SqlCommand(update, con);
            updateCommand.Parameters.AddWithValue("@gname", group.GroupName);
            updateCommand.Parameters.AddWithValue("@event", group.EventName);
            updateCommand.Parameters.AddWithValue("@date", group.EventDate);
            updateCommand.Parameters.AddWithValue("@start", group.StartTime);
            updateCommand.Parameters.AddWithValue("@score", group.GroupScore);
            updateCommand.Parameters.AddWithValue("@limit", group.Limit);
            updateCommand.Parameters.AddWithValue("@id", gID);

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

        public static GroupModel getGroup(int id)
        {
            select = "SELECT * FROM Groups WHERE Group_ID = @id";

            SqlCommand selectCommand = new SqlCommand(select, con);
            selectCommand.Parameters.AddWithValue("@id", id);

            GroupModel group = new GroupModel();
            try
            {
                con.Open();
                SqlDataReader reader = selectCommand.ExecuteReader();
                while (reader.Read())
                {
                    group = new GroupModel(reader["GroupName"].ToString(),
                        reader["EventName"].ToString(), reader["EventDate"].ToString(),
                        reader["StartTime"].ToString(), Convert.ToInt32(reader["Limit"].ToString()));
                    group.GroupScore = Convert.ToInt32(reader["GroupScore"].ToString());
                }
            }
            catch { }
            finally
            {
                con.Close();
            }

            group.GroupMembers = MemberDB.getGroupMembers(id);

            return group;
        }

        public static Dictionary<int, GroupModel> getGroups(EventModel e)
        {
            return getGroups(e.EventName, e.Date);
        }

        public static Dictionary<int, GroupModel> getGroups(string eName, string date)
        {
            select = "Select * from Groups WHERE EventName = @name and EventDate = @date "+
                      "ORDER BY StartTime";

            SqlCommand selectCommand = new SqlCommand(select, con);
            selectCommand.Parameters.AddWithValue("@name", eName);
            selectCommand.Parameters.AddWithValue("@date", date);

            Dictionary<int, GroupModel> groups = new Dictionary<int, GroupModel>();
            try
            {
                con.Open();
                SqlDataReader reader = selectCommand.ExecuteReader();
                while (reader.Read())
                {
                    groups.Add(Convert.ToInt32(reader["Group_ID"].ToString()),
                        new GroupModel(reader["GroupName"].ToString(), eName, date,
                            reader["StartTime"].ToString(), Convert.ToInt32(reader["Limit"].ToString())));
                }
            }
            catch { }
            finally
            {
                con.Close();
            }

            foreach (KeyValuePair<int, GroupModel> g in groups)
            {
                g.Value.GroupMembers = MemberDB.getGroupMembers(g.Key);
            }

            return groups;
        }

        public static bool addMemberToGroup(int gID, MembersModel me)
        {
            return addMemberToGroup(gID, me.MemberEmail);
        }

        public static bool addMemberToGroup(int gID, string email)
        {
            insert = "INSERT Group_Members VALUES (@group, @email, 0)";
            SqlCommand insertCommand = new SqlCommand(insert, con);
            insertCommand.Parameters.AddWithValue("@group", gID);
            insertCommand.Parameters.AddWithValue("@email", email);

            int count = 0;
            try
            {
                con.Open();
                count = insertCommand.ExecuteNonQuery();
            }
            catch (Exception ex) { throw ex; }
            finally { con.Close(); }

            return count > 0;
        }

        public static bool removeMemberFromGroup(int gID, MembersModel me)
        {
            return removeMemberFromGroup(gID, me.MemberEmail);
        }

        public static bool removeMemberFromGroup(int gID, string email)
        {
            delete = "DELETE Group_Members where MyGroup = @gname and MembEmail = @email";

            SqlCommand deleteCommand = new SqlCommand(delete, con);
            deleteCommand.Parameters.AddWithValue("@gname", gID);
            deleteCommand.Parameters.AddWithValue("@email", email);
            int count = 0;

            try
            {
                con.Open();
                count = deleteCommand.ExecuteNonQuery();
            }
            catch (SqlException ex) { throw ex; }
            finally { con.Close(); }

            return count > 0;
        }

        public static bool deleteGroup(GroupModel group)
        {
            delete = "DELETE Groups Where GroupName = @gname and EventDate = @date and StartTime = @stime";

            SqlCommand deleteCommand = new SqlCommand(delete, con);
            deleteCommand.Parameters.AddWithValue("@gname", group.GroupName);
            deleteCommand.Parameters.AddWithValue("@date", group.EventDate);
            deleteCommand.Parameters.AddWithValue("@stime", group.StartTime);
            int count = 0;
            try
            {
                con.Open();
                count = deleteCommand.ExecuteNonQuery();
            }
            catch (Exception ex) { throw ex; }
            finally { con.Close(); }

            return count > 0;
        }

        public static bool deleteGroup(int gID)
        {
            delete = "DELETE Groups Where Group_ID = @gname";

            SqlCommand deleteCommand = new SqlCommand(delete, con);
            deleteCommand.Parameters.AddWithValue("gname", gID);
            int count = 0;
            try
            {
                con.Open();
                count = deleteCommand.ExecuteNonQuery();
            }
            catch (Exception ex) { throw ex; }
            finally { con.Close(); }

            return count > 0;
        }
        #endregion
    }
}