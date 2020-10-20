using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace FantasyPremierLeagueUserTeams
{
    public class FantasyPremierLeagueAPIClient
    {
        public static void GetUserTeamDataJson(int startingUserTeamId, List<int> toDoUserTeamIds, List<int> existingUserTeamIds, List<int> userTeamIdsWithSeasons, string userTeamUrl, UserTeams userTeamInsert, UserTeamGameweekHistories userTeamGameweekHistoriesInsert, UserTeamChips userTeamChipsInsert, UserTeamSeasons userTeamSeasonsInsert, UserTeamClassicLeagues userTeamClassicLeaguesInsert, UserTeamH2hLeagues userTeamH2hLeaguesInsert, int userTeamRetries, SqlConnection db)
        {
            var url = "";
            int userTeamId = 0;
            int userTeamRowsProcessed = 0;

            List<int> processedUserTeamIds = new List<int>();

            try
            {
                if (db.State == ConnectionState.Closed)
                {
                    db.ConnectionString = ConfigurationManager.ConnectionStrings["FantasyPremierLeagueUserTeam202021"].ConnectionString;
                    db.Open();
                }

                foreach (int loopUserTeamId in toDoUserTeamIds)
                {
                    userTeamId = loopUserTeamId;

                    url = string.Format(userTeamUrl, userTeamId);

                    //userTeamSeasonNames = userTeamSeasonRepository.GetAllUserTeamSeasonNamesForUserTeamId(userTeamId, db);

                    //if (userTeamSeasonNames.Count == 0)
                    //{
                    // Get the fantasyPremierLeaguePl1ayerData using JSON.NET
                    //FantasyPremierLeagueAPIClient.GetUserTeamDataJson(userTeamId, existingUserTeamIds, userTeamIdsWithSeasons, userTeamUrl, userTeamInsert, userTeamGameweekHistoriesInsert, userTeamChipsInsert, userTeamSeasonsInsert, userTeamClassicLeaguesInsert, userTeamH2hLeaguesInsert, userTeamRetries, userTeamRowsAdded, db);

                    //Process UserTeam, UserTeamClassicLeague, UserTeamH2hLeague, UserTeamCup
                    FantasyPremierLeagueAPIUserTeam.GetUserTeamData(userTeamId, existingUserTeamIds, userTeamUrl, userTeamInsert, userTeamClassicLeaguesInsert, userTeamH2hLeaguesInsert, userTeamRowsProcessed, db);

                    //Process UserTeamSeasonHistory
                    if (!userTeamIdsWithSeasons.Contains(userTeamId))
                    {
                        FantasyPremierLeagueAPIGameweekHistory.GetUserTeamHistoryDataJson(userTeamId, userTeamIdsWithSeasons, userTeamGameweekHistoriesInsert, userTeamChipsInsert, userTeamSeasonsInsert, db);
                    }
                    //}

                    if (userTeamRowsProcessed >= 500)
                    {
                        WriteToDB(userTeamInsert, userTeamGameweekHistoriesInsert, userTeamChipsInsert, userTeamSeasonsInsert, userTeamClassicLeaguesInsert, userTeamH2hLeaguesInsert, db);

                        userTeamRowsProcessed = 0;

                        Logger.Out(userTeamId.ToString());
                        Logger.Out("");
                    }
                    else
                    {
                        userTeamRowsProcessed += 1;
                    }

                    existingUserTeamIds.Add(userTeamId);
                    processedUserTeamIds.Add(userTeamId);

                    //var itemToRemove = toDoUserTeamIds.SingleOrDefault(r => r == userTeamId);
                    //toDoUserTeamIds.Remove(itemToRemove);
                }

                //Logger.Out("");
            }
            catch (Exception ex)
            {
                Logger.Error("GetUserTeamDataJson data exception (UserTeamId: " + userTeamId.ToString() + "): " + ex.Message);

                //userTeamRetries = 0;
                toDoUserTeamIds.Remove(userTeamId);
                toDoUserTeamIds = toDoUserTeamIds.Except(processedUserTeamIds).ToList();
                GetUserTeamDataJson(startingUserTeamId, toDoUserTeamIds, existingUserTeamIds, userTeamIdsWithSeasons, userTeamUrl, userTeamInsert, userTeamGameweekHistoriesInsert, userTeamChipsInsert, userTeamSeasonsInsert, userTeamClassicLeaguesInsert, userTeamH2hLeaguesInsert, userTeamRetries, db);
            }
            //catch (Exception ex)
            //{
            //    Logger.Error("GetUserTeamDataJson data exception (UserTeamId: " + userTeamId.ToString() + "): " + ex.Message);

            //    if (userTeamRetries < 10)
            //    {
            //        if (userTeamRetries == 1)
            //        {
            //            Logger.Error("GetUserTeamDataJson data exception (starting retries) (UserTeamId: " + userTeamId.ToString() + "): " + ex.Message);
            //            Logger.Error("GetUserTeamDataJson data exception (retry:" + Convert.ToString(userTeamRetries) + ") (UserTeamId: " + userTeamId.ToString() + "): " + ex.Message);
            //        }
            //        else
            //        {
            //            Logger.Error("GetUserTeamDataJson data exception (retry:" + Convert.ToString(userTeamRetries) + ") (UserTeamId: " + userTeamId.ToString() + "): " + ex.Message);
            //        }
            //        userTeamRetries += 1;
            //        GetUserTeamDataJson(startingUserTeamId, toDoUserTeamIds, existingUserTeamIds, userTeamIdsWithSeasons, userTeamUrl, userTeamInsert, userTeamGameweekHistoriesInsert, userTeamChipsInsert, userTeamSeasonsInsert, userTeamClassicLeaguesInsert, userTeamH2hLeaguesInsert, userTeamRetries, db);
            //    }
            //    else
            //    {
            //        Logger.Error("GetUserTeamDataJson data exception (UserTeamId: " + userTeamId.ToString() + "): " + ex.Message);
            //        //throw new Exception("GetUserTeamDataJson data exception (UserTeamId: " + userTeamId.ToString() + "): " + ex.Message);

            //        if (db.State == ConnectionState.Closed)
            //        {
            //            db.ConnectionString = ConfigurationManager.ConnectionStrings["FantasyPremierLeagueUserTeam202021"].ConnectionString;
            //            db.Open();
            //        }

            //        //If error is thrown from sub program write existing records to the DB
            //        WriteToDB(userTeamInsert, userTeamGameweekHistoriesInsert, userTeamChipsInsert, userTeamSeasonsInsert, userTeamClassicLeaguesInsert, userTeamH2hLeaguesInsert, db);

            //        userTeamRetries = 0;
            //        toDoUserTeamIds.Remove(userTeamId);
            //        toDoUserTeamIds = toDoUserTeamIds.Except(processedUserTeamIds).ToList();
            //        GetUserTeamDataJson(startingUserTeamId, toDoUserTeamIds, existingUserTeamIds, userTeamIdsWithSeasons, userTeamUrl, userTeamInsert, userTeamGameweekHistoriesInsert, userTeamChipsInsert, userTeamSeasonsInsert, userTeamClassicLeaguesInsert, userTeamH2hLeaguesInsert, userTeamRetries, db);
            //    }
            //}
        }

        public static void WriteToDB(UserTeams userTeamInsert, UserTeamGameweekHistories userTeamGameweekHistoriesInsert, UserTeamChips userTeamChipsInsert, UserTeamSeasons userTeamSeasonsInsert, UserTeamClassicLeagues userTeamClassicLeaguesInsert, UserTeamH2hLeagues userTeamH2hLeaguesInsert, SqlConnection db)
        {
            try
            {
                int userTeamRowsInserted = 0;
                int userTeamGameweekHistoryRowsInserted = 0;
                int userTeamChipRowsInserted = 0;
                int userTeamSeasonRowsInserted = 0;
                int userTeamClassicLeagueRowsInserted = 0;
                int userTeamH2hLeagueRowsInserted = 0;

                //Write UserTeam to the db
                userTeamRowsInserted = WriteUserTeamToDB(userTeamInsert, db);
                //userTeamRowsInserted = userTeamRepository.InsertUserTeam(userTeamInsert, db);

                //Write UserTeamChip to the db
                userTeamChipRowsInserted = WriteUserTeamChipToDB(userTeamChipsInsert, db);
                //userTeamChipRowsInserted = userTeamChipRepository.InsertUserTeamChip(userTeamChipsInsert, db);

                //Write UserTeamGameweekHistory to the db
                userTeamGameweekHistoryRowsInserted = WriteUserTeamGameweekHistoryToDB(userTeamGameweekHistoriesInsert, db);
                //userTeamGameweekHistoryRowsInserted = userTeamGameweekHistoryRepository.InsertUserTeamGameweekHistories(userTeamGameweekHistoriesInsert, db);

                //Write UserTeamSeason to the db
                userTeamSeasonRowsInserted = WriteUserTeamSeasonToDB(userTeamSeasonsInsert, db);
                //userTeamSeasonRowsInserted = userTeamSeasonRepository.InsertUserTeamSeason(userTeamSeasonsInsert, db);

                //Write UserTeamClassicLeague to the db
                userTeamClassicLeagueRowsInserted = WriteUserTeamClassicLeagueToDB(userTeamClassicLeaguesInsert, db);
                //userTeamClassicLeagueRowsInserted = userTeamClassicLeagueRepository.InsertUserTeamClassicLeague(userTeamClassicLeaguesInsert, db);

                //Write UserTeamH2hLeague to the db
                userTeamH2hLeagueRowsInserted = WriteUserTeamH2hLeagueToDB(userTeamH2hLeaguesInsert, db);
                //userTeamH2hLeagueRowsInserted = userTeamH2hLeagueRepository.InsertUserTeamH2hLeague(userTeamH2hLeaguesInsert, db);

                Logger.Out("");
                Logger.Out("UserTeam bulk insert complete (UserTeam rows inserted: " + Convert.ToString(userTeamRowsInserted) + ")");
                Logger.Out("UserTeamGameweekHistory bulk insert complete (UserTeamGameweekHistory rows inserted: " + Convert.ToString(userTeamGameweekHistoryRowsInserted) + ")");
                Logger.Out("UserTeamChip bulk insert complete (UserTeamChip rows inserted: " + Convert.ToString(userTeamChipRowsInserted) + ")");
                Logger.Out("UserTeamSeason bulk insert complete (UserTeamSeason rows inserted: " + Convert.ToString(userTeamSeasonRowsInserted) + ")");
                Logger.Out("UserTeamClassicLeague bulk insert complete (UserTeamClassicLeague rows inserted: " + Convert.ToString(userTeamClassicLeagueRowsInserted) + ")");
                Logger.Out("UserTeamH2hLeague bulk insert complete (UserTeamH2hLeague rows inserted: " + Convert.ToString(userTeamH2hLeagueRowsInserted) + ")");
                Logger.Out("");

                Logger.Out("API UserTeam calls: " + Convert.ToString(Globals.apiUserTeamCalls));
                Logger.Out("API UserTeamHistory calls: " + Convert.ToString(Globals.apiUserTeamHistoryCalls));
                Logger.Out("API calls: " + Convert.ToString(Globals.apiCalls));
                Logger.Out("");

                userTeamInsert.Clear();
                userTeamGameweekHistoriesInsert.Clear();
                userTeamChipsInsert.Clear();
                userTeamSeasonsInsert.Clear();
                userTeamClassicLeaguesInsert.Clear();
                userTeamH2hLeaguesInsert.Clear();

                Globals.apiCalls = 0;
                Globals.apiUserTeamCalls = 0;
                Globals.apiUserTeamHistoryCalls = 0;
                Globals.userTeamInsertCount = 0;
            }
            catch (Exception ex)
            {
                Logger.Error("Program WriteToDB error: " + ex.Message);
                //WriteToDB(userTeamRowsInserted, userTeamSeasonRowsInserted, userTeamInsert, userTeamSeasonsInsert, db);
            }
        }

        private static int WriteUserTeamToDB(UserTeams userTeamInsert, SqlConnection db)
        {
            try
            {
                UserTeamRepository userTeamRepository = new UserTeamRepository();
                int userTeamRowsInserted = userTeamRepository.InsertUserTeam(userTeamInsert, db);
                return userTeamRowsInserted;
            }
            catch (Exception ex)
            {
                Logger.Error("Program WriteUserTeamToDB error: " + ex.Message);
                return 0;
            }
        }

        private static int WriteUserTeamGameweekHistoryToDB(UserTeamGameweekHistories userTeamGameweekHistoryInsert, SqlConnection db)
        {
            try
            {
                UserTeamGameweekHistoryRepository userTeamGameweekHistoryRepository = new UserTeamGameweekHistoryRepository();
                int userTeamGameweekHistoryRowsInserted = userTeamGameweekHistoryRepository.InsertUserTeamGameweekHistories(userTeamGameweekHistoryInsert, db);
                return userTeamGameweekHistoryRowsInserted;
            }
            catch (Exception ex)
            {
                Logger.Error("Program WriteUserTeamGameweekHistoryToDB error: " + ex.Message);
                return 0;
            }
        }

        private static int WriteUserTeamChipToDB(UserTeamChips userTeamChipInsert, SqlConnection db)
        {
            try
            {
                UserTeamChipRepository userTeamChipRepository = new UserTeamChipRepository();
                int userTeamChipRowsInserted = userTeamChipRepository.InsertUserTeamChip(userTeamChipInsert, db);
                return userTeamChipRowsInserted;
            }
            catch (Exception ex)
            {
                Logger.Error("Program WriteUserTeamChipToDB error: " + ex.Message);
                return 0;
            }
        }

        private static int WriteUserTeamSeasonToDB(UserTeamSeasons userTeamSeasonsInsert, SqlConnection db)
        {
            try
            {
                UserTeamSeasonRepository userTeamSeasonRepository = new UserTeamSeasonRepository();
                int userTeamSeasonRowsInserted = userTeamSeasonRepository.InsertUserTeamSeason(userTeamSeasonsInsert, db);
                return userTeamSeasonRowsInserted;
            }
            catch (Exception ex)
            {
                Logger.Error("Program WriteUserTeamSeasonToDB error: " + ex.Message);
                return 0;
            }
        }

        private static int WriteUserTeamClassicLeagueToDB(UserTeamClassicLeagues userTeamClassicLeaguesInsert, SqlConnection db)
        {
            try
            {
                UserTeamClassicLeagueRepository userTeamClassicLeagueRepository = new UserTeamClassicLeagueRepository();
                int userTeamClassicLeagueRowsInserted = userTeamClassicLeagueRepository.InsertUserTeamClassicLeague(userTeamClassicLeaguesInsert, db);
                return userTeamClassicLeagueRowsInserted;
            }
            catch (Exception ex)
            {
                Logger.Error("Program WriteUserTeamClassicLeagueToDB error: " + ex.Message);
                return 0;
            }
        }

        private static int WriteUserTeamH2hLeagueToDB(UserTeamH2hLeagues userTeamH2hLeaguesInsert, SqlConnection db)
        {
            try
            {
                UserTeamH2hLeagueRepository userTeamH2hLeagueRepository = new UserTeamH2hLeagueRepository();
                int userTeamH2hLeagueRowsInserted = userTeamH2hLeagueRepository.InsertUserTeamH2hLeague(userTeamH2hLeaguesInsert, db);
                return userTeamH2hLeagueRowsInserted;
            }
            catch (Exception ex)
            {
                Logger.Error("Program WriteUserTeamH2hLeagueToDB error: " + ex.Message);
                return 0;
            }
        }
    }
}