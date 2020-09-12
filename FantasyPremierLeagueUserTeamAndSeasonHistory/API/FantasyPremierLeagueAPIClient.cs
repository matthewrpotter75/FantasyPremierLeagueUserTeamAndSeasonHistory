using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace FantasyPremierLeagueUserTeams
{
    public class FantasyPremierLeagueAPIClient
    {
        public static void GetUserTeamDataJson(int userTeamId, List<int> userTeamIds, string userTeamUrl, UserTeams userTeamInsert, UserTeamSeasons userTeamSeasonsInsert, int userTeamRetries, int userTeamRowsAdded, SqlConnection db)
        {
            try
            {
                var url = "";

                url = string.Format(userTeamUrl, userTeamId);

                FantasyPremierLeagueAPIUserTeam.GetUserTeamData(userTeamId, userTeamIds, userTeamUrl, userTeamInsert, userTeamSeasonsInsert, userTeamRowsAdded, db);
                //FantasyPremierLeagueAPIGameweekHistory.GetUserTeamHistoryDataJson(userTeamId, userTeamSeasonsInsert, db);

                //Logger.Out("");
            }
            catch (Exception ex)
            {
                Logger.Error("GetUserTeamDataJson data exception (starting retries) (UserTeamId: " + userTeamId.ToString() + "): " + ex.Message);
                
                if (userTeamRetries < 10)
                {
                    userTeamRetries += 1;
                    GetUserTeamDataJson(userTeamId, userTeamIds, userTeamUrl, userTeamInsert, userTeamSeasonsInsert, userTeamRetries, userTeamRowsAdded, db);
                }
                else
                {
                    Logger.Error("GetUserTeamDataJson data exception (UserTeamId: " + userTeamId.ToString() + "): " + ex.Message);
                    throw new Exception("GetUserTeamDataJson data exception (UserTeamId: " + userTeamId.ToString() + "): " + ex.Message);
                }
            }
        }
    }
}