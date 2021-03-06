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
    public class UserTeamH2hLeagueRepository : IUserTeamH2hLeague
    {
        public int InsertUserTeamH2hLeague(UserTeamH2hLeagues h2hleagues, SqlConnection db)
        {
            try
            {
                int rowsAffected = 0;

                using (IDataReader reader = h2hleagues.GetDataReader())
                //using (var db = new SqlConnection(ConfigurationManager.ConnectionStrings["FantasyPremierLeagueUserTeam"].ConnectionString))
                {
                    //db.Open();

                    using (var bulkCopy = new SqlBulkCopy(db))
                    {
                        bulkCopy.BulkCopyTimeout = 1000;
                        bulkCopy.BatchSize = 500;
                        bulkCopy.DestinationTableName = "UserTeamH2hLeague";
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
                Logger.Error("UserTeamH2hLeague Repository (insert) error: " + ex.Message);
                throw ex;
            }
        }

        //public bool InsertUserTeamH2hLeague(UserTeamH2hLeague h2hLeague, SqlConnection db)
        //{
        //    try
        //    {
        //        long rowsAffected = 0;

        //        //using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["FantasyPremierLeagueUserTeam"].ConnectionString))
        //        //{
        //            rowsAffected = db.Insert(h2hLeague, commandTimeout: 300);
        //        //}

        //        if (rowsAffected > 0)
        //        {
        //            Logger.Out("H2hLeague: " + h2hLeague.name + " - inserted");
        //            return true;
        //        }
        //        return false;
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Error("H2hLeague Repository (insert) error: " + ex.Message);
        //        throw ex;
        //    }
        //}

        public bool UpdateUserTeamH2hLeague(UserTeamH2hLeague h2hLeague, SqlConnection db)
        {
            try
            {
                bool rowsUpdated = false;

                //using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["FantasyPremierLeagueUserTeam"].ConnectionString))
                //{
                    rowsUpdated = db.Update(h2hLeague, commandTimeout: 300);
                //}

                if (rowsUpdated == true)
                {
                    Logger.Out("H2hLeague: " + h2hLeague.name + " - updated");
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Logger.Error("H2hLeague Repository (update) error: " + ex.Message);
                throw ex;
            }
        }

        public bool DeleteUserTeamH2hLeague(int userTeamid, int h2hLeagueid, SqlConnection db)
        {
            try
            {
                bool rowsDeleted = false;
                var h2hLeague = new UserTeamH2hLeague();

                //using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["FantasyPremierLeagueUserTeam"].ConnectionString))
                //{
                    h2hLeague = db.Get<UserTeamH2hLeague>(h2hLeagueid);
                    rowsDeleted = db.Delete(new UserTeamH2hLeague() { userteamid = userTeamid, id = h2hLeagueid });
                //}

                if (rowsDeleted == true)
                {
                    Logger.Out("H2hLeague: " + h2hLeague.name + " (" + Convert.ToString(h2hLeagueid) + ") - deleted");
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Logger.Error("H2hLeague Repository (DeleteH2hLeague) error: " + ex.Message);
                throw ex;
            }
        }

        public bool DeleteH2hLeagueId(int userTeamid, int h2hLeagueid, SqlConnection db)
        {
            try
            {
                string h2hLeagueName;
                int rowsDeleted;

                //using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["FantasyPremierLeagueUserTeam"].ConnectionString))
                //{
                    var h2hLeague = db.Get<UserTeamH2hLeague>(h2hLeagueid);
                    h2hLeagueName = h2hLeague.name;

                    string deleteQuery = "DELETE FROM dbo.UserTeamH2hLeague WHERE userTeamid = @UserTeamId AND leagueid = @H2hLeagueId;";
                    rowsDeleted = db.Execute(deleteQuery, new { UserTeamId = userTeamid, H2hLeagueId = h2hLeagueid });
                //}

                if (rowsDeleted > 0)
                {
                    Logger.Out("H2hLeague: " + h2hLeagueName + " (" + Convert.ToString(h2hLeagueid) + ") - deleted");
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Logger.Error("H2hLeague Repository (DeleteH2hLeagueId) error: " + ex.Message);
                throw ex;
            }
        }

        public List<int> GetAllH2hLeagueIdsForUserTeamId(int userTeamId, SqlConnection db)
        {
            try
            {
                //using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["FantasyPremierLeagueUserTeam"].ConnectionString))
                //{
                    string selectQuery = @"SELECT leagueid AS id FROM dbo.UserTeamH2hLeague WHERE UserTeamId = @UserTeamId;";

                    IDataReader reader = db.ExecuteReader(selectQuery, new { UserTeamId = userTeamId }, commandTimeout: 300);

                    List<int> result = ReadList(reader);

                    reader.Close();

                return result;
                //}
            }
            catch (Exception ex)
            {
                Logger.Error("H2hLeague Repository (GetAllH2hLeagueIds) error: " + ex.Message);
                throw ex;
            }
        }

        public string GetH2hLeagueName(int h2hLeagueId, SqlConnection db)
        {
            //using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["FantasyPremierLeagueUserTeam"].ConnectionString))
            //{
                var h2hLeague = db.Get<UserTeamH2hLeague>(h2hLeagueId, commandTimeout: 300);

                string h2hLeagueName = h2hLeague.name;

                return h2hLeagueName;
            //}
        }

        //public List<int> GetCompetedH2hLeagueIds(SqlConnection db)
        //{
        //    //using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["FantasyPremierLeagueUserTeam"].ConnectionString))
        //    //{
        //        string selectQuery = @"SELECT h.id FROM dbo.UserTeamH2hLeague h INNER JOIN dbo.Gameweeks g ON h.gameweekId = g.id WHERE g.id = (SELECT TOP 1 id FROM dbo.Gameweeks WHERE deadline_time < GETDATE() ORDER BY deadline_time DESC)";

        //        IDataReader reader = db.ExecuteReader(selectQuery);

        //        List<int> result = ReadList(reader);

        //        reader.Close();

        //        return result;
        //    //}
        //}

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