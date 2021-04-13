using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using EmailTestApp.Models;

namespace EmailTestApp.DBA
{
    public class LeagueDB
    {
        private static string select, insert, delete, update;
        private static SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);

        public static LeagueModel getLeague(string leagueRef)
        {
            select = "SELECT * FROM League WHERE LeagueName = @leagueRef";
            LeagueModel resultLeague = new LeagueModel();

            SqlCommand selectCommand = new SqlCommand(select, con);
            selectCommand.Parameters.AddWithValue("@leagueRef", leagueRef);
            try
            {
                con.Open();
                SqlDataReader reader = selectCommand.ExecuteReader();
                while (reader.Read())
                {
                    resultLeague = new LeagueModel(reader["LeagueName"].ToString(),
                        reader["Commissioner"].ToString(), reader["Description"].ToString());
                }
            }
            catch { }
            finally
            {
                con.Close();
            }
            resultLeague.Members = MemberDB.getLeagueMembers(leagueRef);
            resultLeague.Events = EventDB.getLeagueEvents(leagueRef);
            return resultLeague;
        }

        public static bool isLeagueMember(string league, string email)
        {
            select = "SELECT * FROM League_Members where League = @league and MembEmail = @email";
            bool result = false;

            SqlCommand selectCommand = new SqlCommand(select, con);
            selectCommand.Parameters.AddWithValue("@league", league);
            selectCommand.Parameters.AddWithValue("@email", email);

            try
            {
                con.Open();
                SqlDataReader reader = selectCommand.ExecuteReader();
                result = reader.HasRows;
            }
            finally
            {
                con.Close();
            }

            return result;
        }

        public static List<LeagueModel> getJoinedLeagues(string memberEmail)
        {
            select = "SELECT LeagueName, Commissioner, Description " +
                "FROM League join League_Members on League.LeagueName = League_Members.League " +
                "WHERE League.Commissioner != @email AND League_Members.MembEmail = @email";

            SqlCommand selectCommand = new SqlCommand(select, con);
            selectCommand.Parameters.AddWithValue("@email", memberEmail);
            List<LeagueModel> leagues = new List<LeagueModel>();
            LeagueModel l = new LeagueModel();

            try
            {
                con.Open();
                SqlDataReader reader = selectCommand.ExecuteReader();
                while (reader.Read())
                {
                    l = new LeagueModel(reader["LeagueName"].ToString(),
                        reader["Commissioner"].ToString(), reader["Description"].ToString());
                    leagues.Add(l);
                }
            }
            catch { }
            finally
            {
                con.Close();
            }

            return leagues;
        }

        public static List<LeagueModel> getLeaguesImNotIn(string memberEmail)
        {
            select = "SELECT * FROM League where LeagueName NOT IN " +
                "(Select League FROM League_Members where MembEmail = @email)";

            SqlCommand selectCommand = new SqlCommand(select, con);
            selectCommand.Parameters.AddWithValue("@email", memberEmail);
            List<LeagueModel> leagues = new List<LeagueModel>();
            try
            {
                con.Open();
                SqlDataReader reader = selectCommand.ExecuteReader();
                while (reader.Read())
                {
                    leagues.Add(new LeagueModel(reader["LeagueName"].ToString(), reader["Commissioner"].ToString(), reader["Description"].ToString()));
                }
            }
            catch { }
            finally { con.Close(); }

            return leagues;
        }

        public static List<LeagueModel> getCreatedLeagues(string memberEmail)
        {
            select = "SELECT LeagueName, Commissioner, Description " +
                "FROM League, Member " +
                "WHERE League.Commissioner = @email AND Member.Email = @email";

            SqlCommand selectCommand = new SqlCommand(select, con);
            selectCommand.Parameters.AddWithValue("@email", memberEmail);
            List<LeagueModel> leagues = new List<LeagueModel>();
            LeagueModel l = new LeagueModel();

            try
            {
                con.Open();
                SqlDataReader reader = selectCommand.ExecuteReader();
                while (reader.Read())
                {
                    l = new LeagueModel(reader["LeagueName"].ToString(),
                        reader["Commissioner"].ToString(), reader["Description"].ToString());
                    leagues.Add(l);
                }
            }
            catch { }
            finally
            {
                con.Close();
            }
            return leagues;
        }

        public static bool addMemberToLeague(MembersModel me, string league)
        {
            return addMemberToLeague(me.MemberEmail, league);
        }

        public static bool addMemberToLeague(string email, string league)
        {
            insert = "INSERT INTO League_Members VALUES (@league, @email, 0)";

            SqlCommand insertCommand = new SqlCommand(insert, con);
            insertCommand.Parameters.AddWithValue("@league", league);
            insertCommand.Parameters.AddWithValue("@email", email);

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

        public static List<LeagueModel> getLeaguesICommish(string comEmail)
        {
            select = "Select * from League where Commissioner = @email";

            SqlCommand selectCommand = new SqlCommand(select, con);
            selectCommand.Parameters.AddWithValue("@email", comEmail);
            List<LeagueModel> leagues = new List<LeagueModel>();

            try
            {
                con.Open();
                SqlDataReader reader = selectCommand.ExecuteReader();
                while (reader.Read())
                {
                    leagues.Add(new LeagueModel(reader["LeagueName"].ToString(),
                        reader["Commissioner"].ToString(), reader["Description"].ToString()));
                }
            }
            catch { }
            finally
            {
                con.Close();
            }

            return leagues;
        }

        public static List<String> ReturnLeagueNames()
        {
            List<string> leagues = new List<string>();
            try
            {
                con.Open();

                SqlCommand sqlcmd = new SqlCommand("SELECT * FROM League", con);
                SqlDataReader sqlReader = sqlcmd.ExecuteReader();
                while (sqlReader.Read())
                {
                    leagues.Add(sqlReader["LeagueName"].ToString());
                }
                sqlReader.Close();
            }
            catch { }
            finally
            {
                con.Close();
            }
            return leagues;
        }

        public static LeagueModel getGroupLeague(int gID)
        {
            select = "select * from League where LeagueName = "+
                "(select League from EventsTable E, Groups G "+
                "where E.EventName = G.EventName and E.EventDate = G.EventDate and G.Group_ID = @group)";
            SqlCommand selectCommand = new SqlCommand(select, con);
            selectCommand.Parameters.AddWithValue("@group", gID);
            LeagueModel league = new LeagueModel();

            try
            {
                con.Open();
                SqlDataReader reader = selectCommand.ExecuteReader();
                while (reader.Read())
                {
                    league = new LeagueModel(reader["LeagueName"].ToString(),
                        reader["Commissioner"].ToString(), reader["Description"].ToString());
                }
            }
            catch { }
            finally
            {
                con.Close();
            }

            league.Members = MemberDB.getLeagueMembers(league.LeagueName);
            league.Events = EventDB.getLeagueEvents(league.LeagueName);
            return league;
        }

        public static string getGroupLeagueName(int gID)
        {
            select = "select League from EventsTable E, Groups G "+
                "where E.EventName = G.EventName and E.EventDate = G.EventDate and G.Group_ID = @group";

            SqlCommand selectCommand = new SqlCommand(select, con);
            selectCommand.Parameters.AddWithValue("@group", gID);
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
            finally
            {
                con.Close();
            }

            return league;
        }

        public static bool isCommissioner(string email, string league)
        {
            select = "SELECT * FROM League WHERE Commissioner = @email AND LeagueName = @league";
            bool result = false;
            SqlCommand selectCommand = new SqlCommand(select, con);
            selectCommand.Parameters.AddWithValue("@email", email);
            selectCommand.Parameters.AddWithValue("@league", league);

            try{
                con.Open();
                result = selectCommand.ExecuteReader().HasRows;
            }
            catch (Exception ex) { throw ex; }
            finally { con.Close(); }

            return result;
        }

        public static bool deleteLeague(LeagueModel l)
        {
            delete = "DELETE League WHERE LeagueName = @name and Commissioner=@email";

            SqlCommand deleteCommand = new SqlCommand(delete, con);
            deleteCommand.Parameters.AddWithValue("@name", l.LeagueName);
            deleteCommand.Parameters.AddWithValue("@email", l.Commissioner);

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

        public static bool removeLeagueMember(string email, string league)
        {
            delete = "DELETE League_Members WHERE MembEmail = @email AND League = @league";

            SqlCommand deleteCommand = new SqlCommand(delete, con);
            deleteCommand.Parameters.AddWithValue("@email", email);
            deleteCommand.Parameters.AddWithValue("@league", league);

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
    }
}