using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace EmailTestApp.Models
{
    public class ObjectFactory
    {
        public Dictionary<int, MembersModel> TheMembers { get; set; }
        public Dictionary<int, LeagueModel> TheLeagues { get; set; }
        public Dictionary<int, EventModel> TheEvents { get; set; }

        public ObjectFactory(string connectionStringName)
        {
            string connString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            SqlConnection sqlcn = new SqlConnection(connString);
            try
            {
                sqlcn.Open();

                SqlCommand sqlcmd = new SqlCommand("SELECT * FROM Member", sqlcn);
                SqlDataReader sqlReader = sqlcmd.ExecuteReader();
                this.TheMembers = new Dictionary<int, MembersModel>();
                int index = 0;
                while (sqlReader.Read())
                {
                    MembersModel newMember = new MembersModel(sqlReader["FirstName"].ToString(),
                        sqlReader["LastName"].ToString(), sqlReader["Email"].ToString(),
                        sqlReader["PasswordHash"].ToString());
                    newMember.ConfirmPassword = newMember.PasswordHash;
                    this.TheMembers.Add(index, newMember);
                    index++;
                }
                sqlReader.Close();

                sqlcmd.CommandText = "SELECT * FROM EventsTable";
                sqlReader = sqlcmd.ExecuteReader();
                this.TheEvents = new Dictionary<int, EventModel>();
                index = 0;
                while (sqlReader.Read())
                {
                    this.TheEvents.Add(index, new EventModel(sqlReader["League"].ToString(),
                        sqlReader["EventName"].ToString(), sqlReader["EventDate"].ToString(),
                        sqlReader["StartTime"].ToString(), sqlReader["EndTime"].ToString(),
                        sqlReader["Location"].ToString(), sqlReader["Description"].ToString()));
                    index++;
                }
                sqlReader.Close();

                sqlcmd.CommandText = "SELECT * FROM League";
                sqlReader = sqlcmd.ExecuteReader();
                this.TheLeagues = new Dictionary<int, LeagueModel>();
                index = 0;
                while (sqlReader.Read())
                {
                    LeagueModel newLeague = new LeagueModel(sqlReader["LeagueName"].ToString(),
                        sqlReader["Commissioner"].ToString(), sqlReader["Description"].ToString());
                    newLeague.Members = EmailTestApp.DBA.MemberDB.getLeagueMembers(newLeague.LeagueName);
                    newLeague.Events = EmailTestApp.DBA.EventDB.getLeagueEvents(newLeague.LeagueName);
                    this.TheLeagues.Add(index, newLeague);
                    index++;
                }
                sqlReader.Close();
            }
            catch { }
            finally
            {
                sqlcn.Close();
            }
        }

        public void CreateMember(MembersModel newMember, string connectionStringName)
        {
            string connString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            SqlConnection sqlcn = new SqlConnection(connString);
            try
            {
                sqlcn.Open();

                String query = "INSERT INTO Member (FirstName, LastName, EMail, PasswordHash) VALUES(@FirstName, @LastName, @MemberEmail, @PasswordHash)";
                SqlCommand command = new SqlCommand(query, sqlcn);

                command.Parameters.Add("@FirstName", newMember.FirstName);
                command.Parameters.Add("@LastName", newMember.LastName);
                command.Parameters.Add("@MemberEmail", newMember.MemberEmail);
                command.Parameters.Add("@PasswordHash", newMember.PasswordHash);
                command.ExecuteNonQuery();
                sqlcn.Close();
            }
            catch { }
            finally
            {
                sqlcn.Close();
            }
        }

        public void CreateLeague(LeagueModel newLeague, string connectionStringName)
        {
            string connString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            SqlConnection sqlcn = new SqlConnection(connString);
            try
            {
                sqlcn.Open();

                String query = "INSERT INTO League (LeagueName, Commissioner, Description) VALUES(@LeagueName, @Commissioner, @Description)";
                SqlCommand command = new SqlCommand(query, sqlcn);

                command.Parameters.Add("@LeagueName", newLeague.LeagueName);
                command.Parameters.Add("@Commissioner", newLeague.Commissioner);
                command.Parameters.Add("@Description", newLeague.Description);
                command.ExecuteNonQuery();
                sqlcn.Close();
            }
            catch { }
            finally
            {
                sqlcn.Close();
            }
        }

        public void CreateEvent(EventModel newEvent, string connectionStringName)
        {
            string connString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            SqlConnection sqlcn = new SqlConnection(connString);
            try
            {
                sqlcn.Open();

                String query = "INSERT INTO EventsTable (League, EventName, EventDate, StartTime, EndTime, Location, Description) VALUES (@League, @EventName, @EventDate, @StartTime, "
                    + "@EndTime, @Location, @Description)";
                SqlCommand command = new SqlCommand(query, sqlcn);

                command.Parameters.Add("@League", newEvent.LeagueRef);
                command.Parameters.Add("@EventName", newEvent.EventName);
                command.Parameters.Add("@EventDate", newEvent.Date);
                command.Parameters.Add("@StartTime", newEvent.StartTime);
                command.Parameters.Add("@EndTime", newEvent.EndTime);
                command.Parameters.Add("@Location", newEvent.Location);
                command.Parameters.Add("@Description", newEvent.Description);
                command.ExecuteNonQuery();
                sqlcn.Close();
            }
            catch { }
            finally
            {
                sqlcn.Close();
            }
        }

        public void EditMember(MembersModel newMember, string connectionStringName)
        {
            string connString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            SqlConnection sqlcn = new SqlConnection(connString);
            try
            {
                sqlcn.Open();
                string query = "Update Member SET FirstName=@FirstName, LastName=@LastName WHERE Email = @MemberEmail";
                SqlCommand command = new SqlCommand(query, sqlcn);
                
                command.Parameters.Add("@FirstName", newMember.FirstName);
                command.Parameters.Add("@LastName", newMember.LastName);
                command.Parameters.Add("@MemberEmail", newMember.MemberEmail);
                command.ExecuteNonQuery();
                sqlcn.Close();
            }
            catch { }
            finally
            {
                sqlcn.Close();
            }
        }
        
        public void EditEvent(EventModel newEvent, string connectionStringName)
        {
            string connString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            SqlConnection sqlcn = new SqlConnection(connString);
            try
            {
                sqlcn.Open();
                string query = "Update EventsTable " 
                +"SET League=@League, EventName=@EventName, EventDate=@EventDate, "
                +"StartTime=@StartTime, EndTime=@EndTime, Location=@Location, Description=@Description "
                +"WHERE EventName=@EventName OR EventDate=@EventDate";
                SqlCommand command = new SqlCommand(query, sqlcn);
                
                command.Parameters.Add("@League", newEvent.LeagueRef);
                command.Parameters.Add("@EventName", newEvent.EventName);
                command.Parameters.Add("@EventDate", newEvent.Date);
                command.Parameters.Add("@StartTime", newEvent.StartTime);
                command.Parameters.Add("@EndTime", newEvent.EndTime);
                command.Parameters.Add("@Location", newEvent.Location);
                command.Parameters.Add("@Description", newEvent.Description);
                command.ExecuteNonQuery();
                sqlcn.Close();
            }
            catch { }
            finally
            {
                sqlcn.Close();
            }
        }
        
        public void EditLeague(LeagueModel newLeague, string connectionStringName)
        {
            string connString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            SqlConnection sqlcn = new SqlConnection(connString);
            try
            {
                sqlcn.Open();
                string query = "Update League SET Description=@Description, Commissioner=@Commissioner WHERE LeagueName=@LeagueName";
                SqlCommand command = new SqlCommand(query, sqlcn);
                
                command.Parameters.AddWithValue("@LeagueName", newLeague.LeagueName);
                command.Parameters.AddWithValue("@Commissioner", newLeague.Commissioner);
                command.Parameters.AddWithValue("@Description", newLeague.Description);

                command.ExecuteNonQuery();
                sqlcn.Close();
            }
            catch { }
            finally
            {
                sqlcn.Close();
            }
        }
    }
}