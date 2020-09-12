using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using Dapper;
using DapperExtensions;
using System.Linq;
using DataStreams.ETL;

namespace FantasyPremierLeagueUserTeams
{
    public class UserTeamRepository : IUserTeam
    {
        public int InsertUserTeam(UserTeams userTeams, SqlConnection db)
        {
            try
            {
                int rowsAffected = 0;

                using (IDataReader reader = userTeams.GetDataReader())
                //using (var db = new SqlConnection(ConfigurationManager.ConnectionStrings["FantasyPremierLeagueUserTeam"].ConnectionString))
                {
                    //db.Open();

                    using (var bulkCopy = new SqlBulkCopy(db))
                    {
                        bulkCopy.BulkCopyTimeout = 1000;
                        bulkCopy.BatchSize = 500;
                        bulkCopy.DestinationTableName = "UserTeam";
                        bulkCopy.EnableStreaming = true;

                        // Add your column mappings here
                        bulkCopy.ColumnMappings.Add("id", "id");
                        bulkCopy.ColumnMappings.Add("player_first_name", "player_first_name");
                        bulkCopy.ColumnMappings.Add("player_last_name", "player_last_name");
                        bulkCopy.ColumnMappings.Add("player_region_id", "player_region_id");
                        bulkCopy.ColumnMappings.Add("player_region_name", "player_region_name");
                        bulkCopy.ColumnMappings.Add("player_region_iso_code_long", "player_region_iso_code");
                        bulkCopy.ColumnMappings.Add("summary_overall_points", "summary_overall_points");
                        bulkCopy.ColumnMappings.Add("summary_overall_rank", "summary_overall_rank");
                        bulkCopy.ColumnMappings.Add("summary_event_points", "summary_gameweek_points");
                        bulkCopy.ColumnMappings.Add("summary_event_rank", "summary_gameweek_rank");
                        bulkCopy.ColumnMappings.Add("current_event", "current_gameweekId");
                        bulkCopy.ColumnMappings.Add("joined_time", "joined_time");
                        bulkCopy.ColumnMappings.Add("name", "team_name");                        
                        bulkCopy.ColumnMappings.Add("last_deadline_bank","team_bank");
                        bulkCopy.ColumnMappings.Add("last_deadline_value","team_value");
                        bulkCopy.ColumnMappings.Add("last_deadline_total_transfers","team_transfers");
                        bulkCopy.ColumnMappings.Add("kit", "kit");
                        bulkCopy.ColumnMappings.Add("favourite_team","favourite_teamid");
                        bulkCopy.ColumnMappings.Add("started_event","started_gameweekid");

                        //using (var dataReader = userTeamSeasons.ToDataReader())
                        //{
                        bulkCopy.WriteToServer(reader);
                        rowsAffected = SqlBulkCopyExtension.RowsCopiedCount(bulkCopy);
                        //}
                    }
                }

                return rowsAffected;
            }
            catch (Exception ex)
            {
                Logger.Error("UserTeam Repository (insert) error: " + ex.Message);
                throw ex;
            }
        }

        public bool UpdateUserTeam(UserTeam userTeam, SqlConnection db)
        {
            try
            {
                bool rowsUpdated = false;

                //using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["FantasyPremierLeagueUserTeam"].ConnectionString))
                //{
                    rowsUpdated = db.Update(userTeam, commandTimeout: 300);
                //}

                if (rowsUpdated == true)
                {
                    //Console.WriteLine("UserTeam " + userTeam.name + "(" + Convert.ToString(userTeam.id) + ") - updated");
                    Logger.Out("UserTeam: " + userTeam.name + " (" + Convert.ToString(userTeam.id) + ") - updated");
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Logger.Error("UserTeam Repository (update) error: " + ex.Message);
                throw ex;
            }
        }

        public bool DeleteUserTeam(int userTeamId, SqlConnection db)
        {
            try
            {
                bool rowsDeleted = false;
                string userTeamName;

                //using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["FantasyPremierLeagueUserTeam"].ConnectionString))
                //{
                    var userTeam = db.Get<UserTeam>(userTeamId);
                    userTeamName = userTeam.name;

                    rowsDeleted = db.Delete(new UserTeam() { id = userTeamId });
                //}

                if (rowsDeleted == true)
                {
                    //Console.WriteLine("UserTeam " + Convert.ToString(userTeamId) + " - deleted");
                    Logger.Out("UserTeam: " + userTeamName + " (" + Convert.ToString(userTeamId) + ") - deleted");
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Logger.Error("UserTeam Repository (DeleteUserTeam) error: " + ex.Message);
                throw ex;
            }
        }

        public bool DeleteUserTeamId(int userTeamId, SqlConnection db)
        {
            try
            {
                string userTeamName;
                int rowsDeleted;

                //using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["FantasyPremierLeagueUserTeam"].ConnectionString))
                //{
                    var userTeam = db.Get<UserTeam>(userTeamId);
                    userTeamName = userTeam.name;

                    //rowsDeleted = db.Delete(new UserTeam() { element = userTeamId });
                    string deleteQuery = "DELETE FROM dbo.UserTeam WHERE userTeamId = @UserTeamId;";
                    rowsDeleted = db.Execute(deleteQuery, new { UserTeamId = userTeamId });
                //}

                if (rowsDeleted > 0)
                {
                    Logger.Out("UserTeam: " + userTeamName + " (" + Convert.ToString(userTeamId) + ") - deleted");
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Logger.Error("UserTeam Repository (DeleteUserTeamId) error: " + ex.Message);
                throw ex;
            }
        }

        public List<int> GetAllUserTeamIds(SqlConnection db)
        {
            try
            {
                //using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["FantasyPremierLeagueUserTeam"].ConnectionString))
                //{
                    string selectQuery = @"SELECT id FROM dbo.UserTeam ORDER BY id;";

                    IDataReader reader = db.ExecuteReader(selectQuery, commandTimeout: 300);

                    List<int> result = ReadList(reader);

                    reader.Close();

                    return result;
                //}
            }
            catch (Exception ex)
            {
                Logger.Error("UserTeam Repository (GetAllUserTeamIds) error: " + ex.Message);
                throw ex;
            }
        }

        public string GetUserTeamName(int userTeamId, SqlConnection db)
        {
            //using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["FantasyPremierLeagueUserTeam"].ConnectionString))
            //{
                //string selectQuery = @"SELECT team_name FROM dbo.UserTeam WHERE id = " + userTeamId.ToString();

                var userTeam = db.Get<UserTeam>(userTeamId, commandTimeout: 300);

                string userTeamName = userTeam.name;

                return userTeamName;
            //}
        }

        public List<int> GetCompetedUserTeamIds(SqlConnection db)
        {
            //using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["FantasyPremierLeagueUserTeam"].ConnectionString))
            //{
                string selectQuery = @"SELECT ut.id FROM dbo.UserTeam ut INNER JOIN dbo.Gameweeks g ON ut.current_gameweekId = g.id WHERE g.id = (SELECT TOP 1 id FROM dbo.Gameweeks WHERE deadline_time < GETDATE() ORDER BY deadline_time DESC)";

                IDataReader reader = db.ExecuteReader(selectQuery, commandTimeout: 300);

                List<int> result = ReadList(reader);

                reader.Close();

                return result;
            //}
        }

        List<int> ReadList(IDataReader reader)
        {
            List<int> list = new List<int>();
            int column = reader.GetOrdinal("id");

            while (reader.Read())
            {
                //check for the null value and than add 
                if (!reader.IsDBNull(column))
                    list.Add(reader.GetInt32(column));
            }

            return list;
        }

    }
}