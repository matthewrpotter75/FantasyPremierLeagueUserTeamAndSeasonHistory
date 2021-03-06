﻿using System;
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
    public class UserTeamClassicLeagueRepository : IUserTeamClassicLeague
    {
        public int InsertUserTeamClassicLeague(UserTeamClassicLeagues classicleagues, SqlConnection db)
        {
            try
            {
                int rowsAffected = 0;

                using (IDataReader reader = classicleagues.GetDataReader())
                //using (var db = new SqlConnection(ConfigurationManager.ConnectionStrings["FantasyPremierLeagueUserTeam"].ConnectionString))
                {
                    //db.Open();

                    using (var bulkCopy = new SqlBulkCopy(db))
                    {
                        bulkCopy.BulkCopyTimeout = 1000;
                        bulkCopy.BatchSize = 500;
                        bulkCopy.DestinationTableName = "UserTeamClassicLeague";
                        bulkCopy.EnableStreaming = true;

                        // Add your column mappings here
                        bulkCopy.ColumnMappings.Add("id", "leagueid");
                        bulkCopy.ColumnMappings.Add("entry_rank", "entry_rank");
                        bulkCopy.ColumnMappings.Add("entry_last_rank", "entry_last_rank");
                        bulkCopy.ColumnMappings.Add("entry_can_leave", "entry_can_leave");
                        bulkCopy.ColumnMappings.Add("entry_can_admin", "entry_can_admin");
                        bulkCopy.ColumnMappings.Add("entry_can_invite", "entry_can_invite");
                        bulkCopy.ColumnMappings.Add("userteamid", "userteamid");

                        //using (var dataReader = userTeamChips.ToDataReader())
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
                Logger.Error("UserTeamClassicLeague Repository (insert) error: " + ex.Message);
                throw ex;
            }
        }

        //public bool InsertUserTeamClassicLeague(UserTeamClassicLeague classicleague, SqlConnection db)
        //{
        //    try
        //    {
        //        long rowsAffected = 0;

        //        //using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["FantasyPremierLeagueUserTeam"].ConnectionString))
        //        //{
        //            rowsAffected = db.Insert(classicleague, commandTimeout: 300);
        //        //}

        //        if (rowsAffected > 0)
        //        {
        //            Logger.Out("ClassicLeague: " + classicleague.name + " - inserted");
        //            return true;
        //        }
        //        return false;
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Error("ClassicLeague Repository (insert) error: " + ex.Message);
        //        throw ex;
        //    }
        //}

        public bool UpdateUserTeamClassicLeague(UserTeamClassicLeague classicleague, SqlConnection db)
        {
            try
            {
                bool rowsUpdated = false;

                //using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["FantasyPremierLeagueUserTeam"].ConnectionString))
                //{
                    rowsUpdated = db.Update(classicleague, commandTimeout: 300);
                //}

                if (rowsUpdated == true)
                {
                    Logger.Out("ClassicLeague: " + classicleague.name + " - updated");
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Logger.Error("UserTeamClassicLeague Repository (update) error: " + ex.Message);
                throw ex;
            }
        }

        public bool DeleteUserTeamClassicLeague(int userTeamid, int classicleagueid, SqlConnection db)
        {
            try
            {
                bool rowsDeleted = false;
                var classicleague = new UserTeamClassicLeague();

                //using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["FantasyPremierLeagueUserTeam"].ConnectionString))
                //{
                    classicleague = db.Get<UserTeamClassicLeague>(classicleagueid);
                    rowsDeleted = db.Delete(new UserTeamClassicLeague() { userteamid = userTeamid, id = classicleagueid });
                //}

                if (rowsDeleted == true)
                {
                    Logger.Out("ClassicLeague: " + classicleague.name + " (" + Convert.ToString(classicleagueid) + ") - deleted");
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Logger.Error("UserTeamClassicLeague Repository (DeleteClassicLeague) error: " + ex.Message);
                throw ex;
            }
        }

        public bool DeleteClassicLeagueId(int userTeamid, int classicleagueid, SqlConnection db)
        {
            try
            {
                string classicleagueName;
                int rowsDeleted;

                //using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["FantasyPremierLeagueUserTeam"].ConnectionString))
                //{
                    var classicleague = db.Get<UserTeamClassicLeague>(classicleagueid);
                    classicleagueName = classicleague.name;

                    string deleteQuery = "DELETE FROM dbo.UserTeamClassicLeague WHERE userTeamid = @UserTeamId AND id = @ClassicLeagueId;";
                    rowsDeleted = db.Execute(deleteQuery, new { UserTeamId = userTeamid, ClassicLeagueId = classicleagueid });
                //}

                if (rowsDeleted > 0)
                {
                    Logger.Out("ClassicLeague: " + classicleagueName + " (" + Convert.ToString(classicleagueid) + ") - deleted");
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Logger.Error("UserTeamClassicLeague Repository (DeleteClassicLeagueId) error: " + ex.Message);
                throw ex;
            }
        }

        public List<int> GetAllClassicLeagueIdsForUserTeamId(int userTeamId, SqlConnection db)
        {
            try
            {
                //using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["FantasyPremierLeagueUserTeam"].ConnectionString))
                //{
                    string selectQuery = @"SELECT leagueid AS id FROM dbo.UserTeamClassicLeague WHERE UserTeamId = @UserTeamId;";

                    IDataReader reader = db.ExecuteReader(selectQuery, new { UserTeamId = userTeamId }, commandTimeout: 300);

                    List<int> result = ReadList(reader);

                    reader.Close();

                    return result;
                //}
            }
            catch (Exception ex)
            {
                Logger.Error("UserTeamClassicLeague Repository (GetAllClassicLeagueIds) error: " + ex.Message);
                throw ex;
            }
        }

        public List<int> GetAllUserTeamIdsWithLeagues(int startingUserTeamId, SqlConnection db)
        {
            try
            {
                string selectQuery = @"SELECT DISTINCT userteamid AS id FROM dbo.UserTeamClassicLeague WITH (NOLOCK) WHERE userteamid >= @StartingUserTeamId;";

                IDataReader reader = db.ExecuteReader(selectQuery, new { StartingUserTeamId = startingUserTeamId }, commandTimeout: 300);

                List<int> result = ReadList(reader);

                reader.Close();

                return result;
            }
            catch (Exception ex)
            {
                Logger.Error("UserTeamClassicLeague Repository (GetAllUserTeamIdsWithLeagues) error: " + ex.Message);
                throw ex;
            }
        }

        public string GetClassicLeagueName(int classicleagueId, SqlConnection db)
        {
            //using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["FantasyPremierLeagueUserTeam"].ConnectionString))
            //{
                var classicleague = db.Get<UserTeamClassicLeague>(classicleagueId, commandTimeout: 300);

                string classicleagueName = classicleague.name;

                return classicleagueName;
            //}
        }

        //public List<int> GetCompetedClassicLeagueIds()
        //{
        //    using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["FantasyPremierLeagueUserTeam"].ConnectionString))
        //    {
        //        string selectQuery = @"SELECT c.id FROM dbo.UserTeamClassicLeague c INNER JOIN dbo.Gameweeks g ON c.gameweekId = g.id WHERE g.id = (SELECT TOP 1 id FROM dbo.Gameweeks WHERE deadline_time < GETDATE() ORDER BY deadline_time DESC)";

        //        IDataReader reader = db.ExecuteReader(selectQuery);

        //        List<int> result = ReadList(reader);

        //        return result;
        //    }
        //}

        public DataTable GetClassicLeagueCountFromUserTeamClassicLeagueForUserTeamIds(List<int> userTeamIds, SqlConnection db)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();

                List<string> sqlParams = new List<string>();

                int i = 0;
                foreach (var value in userTeamIds)
                {
                    var name = "@p" + i++;
                    cmd.Parameters.AddWithValue(name, value);
                    sqlParams.Add(name);
                }

                string paramNames = string.Join(",", sqlParams);

                string selectQuery = @"SELECT userteamid,COUNT(*) AS leagueCount FROM dbo.UserTeamClassicLeague WHERE userteamid IN (" + paramNames + ") GROUP BY userteamid;";

                cmd.Connection = db;
                cmd.CommandTimeout = 100;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = selectQuery;

                //Create SqlDataAdapter and DataTable
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable result = new DataTable();

                // this will query your database and return the result to your datatable
                da.Fill(result);
                da.Dispose();

                return result;
            }
            catch (Exception ex)
            {
                Logger.Error("UserTeamClassicLeague Repository (GetClassicLeagueCountFromUserTeamClassicLeagueForUserTeamIds) error: " + ex.Message);
                throw ex;
                //throw;
            }
        }

        List<int> ReadList(IDataReader reader)
        {
            try
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
            catch (Exception ex)
            {
                Logger.Error("UserTeamClassicLeague Repository (ReadList) error: " + ex.Message);
                throw ex;
            }
        }

    }
}