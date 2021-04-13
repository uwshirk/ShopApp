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
    public static class MemberDB
    {
        private static SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        private static string select, delete, update, insert;

        #region Create
        public static bool addMember(MembersModel m)
        {
            insert = "INSERT INTO Member " +
                            "(FirstName, LastName, Email, PasswordHash)" +
                            "VALUES (@FName, @LName, @Email, @PasswordHash) ";

            SqlCommand insertCommand = new SqlCommand(insert, con);
            insertCommand.Parameters.AddWithValue("@FName", m.FirstName);
            insertCommand.Parameters.AddWithValue("@LName", m.LastName);
            insertCommand.Parameters.AddWithValue("@Email", m.MemberEmail);
            insertCommand.Parameters.AddWithValue("@PasswordHash", m.PasswordHash);

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

        public static bool addLeagueMember(string email, string league)
        {
            insert = "INSERT League_Members VALUES (@league, @email, 0)";

            SqlCommand insertCommand = new SqlCommand(insert, con);
            insertCommand.Parameters.AddWithValue("@league", league);
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
        #endregion

        #region Retrieve

        public static bool memberExists(string email)
        {
            select = "Select Email From Member where Email = @email";

            SqlCommand selectCommand = new SqlCommand(select, con);
            selectCommand.Parameters.AddWithValue("@email", email);

            bool valid = false;
            try
            {
                con.Open();
                valid = selectCommand.ExecuteReader().HasRows;
            }
            catch (Exception ex) { throw ex; }
            finally { con.Close(); }

            return valid;
        }

        public static bool memberExists(string email, string passwordHash)
        {
            select = "Select * From Member where Email = @email AND " +
                "PasswordHash = @passwordHash";

            SqlCommand selectCommand = new SqlCommand(select, con);
            selectCommand.Parameters.AddWithValue("@email", email);
            selectCommand.Parameters.AddWithValue("@passwordHash", passwordHash);

            bool valid = false;
            try
            {
                con.Open();
                valid = selectCommand.ExecuteReader().HasRows;
            }
            catch (Exception ex) { throw ex; }
            finally { con.Close(); }

            return valid;
        }

        public static bool isAdmin(string email)
        {
            select = "Select * FROM Member WHERE Email = @email and Admin = 1";
            bool isAdmin = false;

            SqlCommand selectCommand = new SqlCommand(select, con);
            selectCommand.Parameters.AddWithValue("@email", email);
            try
            {
                con.Open();
                List<MembersModel> admins = new List<MembersModel>();
                SqlDataReader reader = selectCommand.ExecuteReader();
                isAdmin = reader.HasRows;
            }
            catch { }
            finally
            {
                con.Close();
            }
            
            return isAdmin;
        }

        public static bool memberInGroup(string email, int gID)
        {
            select = "Select * from Group_Members where MyGroup = @ID and MembEmail = @email";
            bool result = false;

            SqlCommand selectCommand = new SqlCommand(select, con);
            selectCommand.Parameters.AddWithValue("@ID", gID);
            selectCommand.Parameters.AddWithValue("@email", email);

            try
            {
                con.Open();
                SqlDataReader reader = selectCommand.ExecuteReader();
                result = reader.HasRows;
            }
            catch { }
            finally
            {
                con.Close();
            }

            return result;
        }

        public static bool memberInEventGroup(string email, EventModel eve)
        {
            select = "SELECT * FROM Group_Members GM " +
                "Where MembEmail = @email and GM.MyGroup IN " +
                "(SELECT Group_ID FROM Groups where EventName = @name AND EventDate = @date)";
            bool result= false;
            SqlCommand selectCommand = new SqlCommand(select, con);
            selectCommand.Parameters.AddWithValue("@email", email);
            selectCommand.Parameters.AddWithValue("@name", eve.EventName);
            selectCommand.Parameters.AddWithValue("@date", eve.Date);

            try
            {
                con.Open();
                SqlDataReader reader = selectCommand.ExecuteReader();
                result = reader.HasRows;
            }
            catch { }
            finally
            {
                con.Close();
            }

            return result;
        }

        public static MembersModel getMember(string email)
        {
            select = "SELECT * FROM Member WHERE Email = @email";

            SqlCommand selectCommand = new SqlCommand(select, con);
            selectCommand.Parameters.AddWithValue("@email", email);
            MembersModel m = new MembersModel();

            try
            {
                con.Open();
                SqlDataReader reader = selectCommand.ExecuteReader();
                while (reader.Read())
                {
                    m = new MembersModel(reader["FirstName"].ToString(),
                        reader["LastName"].ToString(), email, reader["PasswordHash"].ToString());
                    m.ConfirmPassword = m.PasswordHash;
                    m.joinedLeagues = LeagueDB.getJoinedLeagues(email);
                    m.createdLeagues = LeagueDB.getCreatedLeagues(email);
                }
            }
            catch { }
            finally
            {
                con.Close();
            }
            return m;
        }

        public static MembersModel getMember(string email, string passwordHash)
        {
            select = "SELECT * FROM Member WHERE Email = @email AND PasswordHash = @passwordHash";

            SqlCommand selectCommand = new SqlCommand(select, con);
            
            selectCommand.Parameters.AddWithValue("@email", email);
            selectCommand.Parameters.AddWithValue("@passwordHash", passwordHash);
                
            MembersModel m = new MembersModel();

            try
            {
                con.Open();
                SqlDataReader reader = selectCommand.ExecuteReader();

                while (reader.Read())
                {
                    m = new MembersModel(reader["FirstName"].ToString(), reader["LastName"].ToString(), email, passwordHash);
                }
            }
            catch { }
            finally
            {
                con.Close();
            }
            
            return m;
        }

        public static MembersModel getLeagueMember(string email, string league)
        {
            select = "SELECT M.FirstName, M.LastName, M.Email, M.PasswordHash" +
            "FROM Member M, League_Members L WHERE M.Email = @email and L.MembEmail = @email and L.League = @league";

            SqlCommand selectCommand = new SqlCommand(select, con);
            selectCommand.Parameters.AddWithValue("@email", email);
            MembersModel m = new MembersModel();

            try
            {
                con.Open();
                SqlDataReader reader = selectCommand.ExecuteReader();
                while (reader.Read())
                {
                    m = new MembersModel(reader["FirstName"].ToString(), reader["LastName"].ToString(), email, reader["PasswordHash"].ToString());
                    m.ConfirmPassword = m.PasswordHash;
                }
            }
            catch { }
            finally
            {
                con.Close();
            }

            return m;
        }

        public static List<MembersModel> getAllMembers()
        {
            select = "SELECT * FROM Member";

            SqlCommand selectCommand = new SqlCommand(select, con);
            List<MembersModel> members = new List<MembersModel>();

            try
            {
                con.Open();
                SqlDataReader reader = selectCommand.ExecuteReader();
                while (reader.Read())
                {
                    MembersModel m = new MembersModel(reader["FirstName"].ToString(),
                        reader["LastName"].ToString(), reader["Email"].ToString(), reader["PasswordHash"].ToString());
                    m.ConfirmPassword = m.PasswordHash;
                    members.Add(m);
                }
            }
            catch { }
            finally
            {
                con.Close();
            }

            return members;
        }

        public static List<MembersModel> getNonLeagueMembers(string league)
        {
            select = "SELECT FirstName, LastName, Email, PasswordHash " +
                            "FROM Member right join League_Members on Email = MembEmail " +
                            "WHERE League <> @league ORDER BY Email";

            SqlCommand selectCommand = new SqlCommand(select, con);
            selectCommand.Parameters.AddWithValue("@league", league);
            List<MembersModel> members = new List<MembersModel>();

            try
            {
                con.Open();
                SqlDataReader reader = selectCommand.ExecuteReader();
                while (reader.Read())
                {
                    MembersModel m = new MembersModel(reader["FirstName"].ToString(),
                        reader["LastName"].ToString(), reader["Email"].ToString(), reader["PasswordHash"].ToString());
                    m.ConfirmPassword = m.PasswordHash;
                    members.Add(m);
                }
            }
            catch { }
            finally
            {
                con.Close();
            }

            return members;
        }

        public static List<MembersModel> getLeagueMembers(string league)
        {
            select = "SELECT FirstName, LastName, Email, PasswordHash " +
                            "FROM Member right join League_Members on Email = MembEmail " +
                            "WHERE League = @league ORDER BY Email";

            SqlCommand selectCommand = new SqlCommand(select, con);
            selectCommand.Parameters.AddWithValue("@league", league);
            List<MembersModel> members = new List<MembersModel>();

            try
            {
                con.Open();
                SqlDataReader reader = selectCommand.ExecuteReader();
                while (reader.Read())
                {
                    MembersModel m = new MembersModel(reader["FirstName"].ToString(),
                        reader["LastName"].ToString(), reader["Email"].ToString(), reader["PasswordHash"].ToString());
                    m.ConfirmPassword = m.PasswordHash;
                    members.Add(m);
                }
            }
            catch { }
            finally
            {
                con.Close();
            }

            return members;
        }

        public static List<string> getLeagueMembersEmail(string league)
        {
            select = "SELECT FirstName, LastName, Email, PasswordHash " +
                            "FROM Member right join League_Members on Email = MembEmail " +
                            "WHERE League = @league ORDER BY Email";

            SqlCommand selectCommand = new SqlCommand(select, con);
            selectCommand.Parameters.AddWithValue("@league", league);
            List<string> memberEmails = new List<string>();

            try
            {
                con.Open();
                SqlDataReader reader = selectCommand.ExecuteReader();
                while (reader.Read())
                {
                    string memberEmail = reader["Email"].ToString();
                    memberEmails.Add(memberEmail);
                }
            }
            catch { }
            finally
            {
                con.Close();
            }

            return memberEmails;
        }

        public static List<MembersModel> getGroupMembers(int id)
        {
            select = "select M.FirstName, M.LastName, M.Email, M.PasswordHash " +
                            "from Member M right join Group_Members on M.Email = Group_Members.MembEmail " +
                            "where Group_Members.MyGroup = @group";

            SqlCommand selectCommand = new SqlCommand(select, con);
            selectCommand.Parameters.AddWithValue("@group", id);
            List<MembersModel> members = new List<MembersModel>();

            try
            {
                con.Open();
                SqlDataReader reader = selectCommand.ExecuteReader();
                while (reader.Read())
                {
                    MembersModel m = new MembersModel(reader["FirstName"].ToString(),
                        reader["LastName"].ToString(), reader["Email"].ToString(),
                        reader["PasswordHash"].ToString());
                    m.ConfirmPassword = m.PasswordHash;
                    members.Add(m);
                }
            }
            catch { }
            finally
            {
                con.Close();
            }

            return members;
        }
        #endregion

        #region Delete
        public static void deleteMember(string email)
        {
            List<LeagueModel> leagues = LeagueDB.getLeaguesICommish(email);
            if (leagues.Count > 0)
            {
                foreach (LeagueModel l in leagues)
                {
                    LeagueDB.deleteLeague(l);
                }
            }
            delete = "DELETE Member WHERE Email = @email";

            SqlCommand deleteCommand = new SqlCommand(delete, con);
            deleteCommand.Parameters.AddWithValue("@email", email);

            try
            {
                con.Open();
                deleteCommand.ExecuteNonQuery();
            }
            catch (Exception ex) { throw ex; }
            finally { con.Close(); }
        }
        #endregion
    }
}