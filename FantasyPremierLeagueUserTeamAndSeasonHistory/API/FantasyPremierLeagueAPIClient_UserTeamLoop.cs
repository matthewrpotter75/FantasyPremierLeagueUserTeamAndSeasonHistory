using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace FantasyPremierLeagueUserTeams
{
    public class FantasyPremierLeagueAPIClientLoop
    {
        public static void GetUserTeamDataJsonLoop(int startingUserTeamId, int userTeamId, List<int> toDoUserTeamIds, List<int> existingUserTeamIds, List<int> userTeamIdsWithSeasons, string userTeamUrl, UserTeams userTeamInsert, UserTeamGameweekHistories userTeamGameweekHistoriesInsert, UserTeamChips userTeamChipsInsert, UserTeamSeasons userTeamSeasonsInsert, UserTeamClassicLeagues userTeamClassicLeaguesInsert, UserTeamH2hLeagues userTeamH2hLeaguesInsert, int userTeamRetries, SqlConnection db)
        {
            //int userTeamRowsProcessed = 0;

            try
            {
                if (db.State == ConnectionState.Closed)
                {
                    db.ConnectionString = ConfigurationManager.ConnectionStrings["FantasyPremierLeagueUserTeam202021"].ConnectionString;
                    db.Open();
                }

                //userTeamSeasonNames = userTeamSeasonRepository.GetAllUserTeamSeasonNamesForUserTeamId(userTeamId, db);

                //if (userTeamSeasonNames.Count == 0)
                //{
                // Get the fantasyPremierLeaguePl1ayerData using JSON.NET
                //FantasyPremierLeagueAPIClient.GetUserTeamDataJson(userTeamId, existingUserTeamIds, userTeamIdsWithSeasons, userTeamUrl, userTeamInsert, userTeamGameweekHistoriesInsert, userTeamChipsInsert, userTeamSeasonsInsert, userTeamClassicLeaguesInsert, userTeamH2hLeaguesInsert, userTeamRetries, userTeamRowsAdded, db);

                //Process UserTeam, UserTeamClassicLeague, UserTeamH2hLeague, UserTeamCup
                FantasyPremierLeagueAPIUserTeam.GetUserTeamData(userTeamId, existingUserTeamIds, userTeamUrl, userTeamInsert, userTeamClassicLeaguesInsert, userTeamH2hLeaguesInsert, db);

                //Process UserTeamSeasonHistory
                if (!userTeamIdsWithSeasons.Contains(userTeamId))
                {
                    FantasyPremierLeagueAPIGameweekHistory.GetUserTeamHistoryDataJson(userTeamId, userTeamIdsWithSeasons, userTeamGameweekHistoriesInsert, userTeamChipsInsert, userTeamSeasonsInsert, db);
                }
                //}
            }
            catch (Exception ex)
            {
                Logger.Error("GetUserTeamDataJsonLoop data exception (UserTeamId: " + userTeamId.ToString() + "): " + ex.Message);
            }
        }
    } 
}