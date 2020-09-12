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
    public class FantasyPremierLeagueAPIUserTeam
    {
        //public static void GetUserTeamJson(int userTeamId, List<int> userTeamIds, UserTeam userTeam)
        public static void GetUserTeamData(int userTeamId, List<int> userTeamIds, string userTeamUrl, UserTeams userTeamInsert, UserTeamSeasons userTeamSeasonsInsert, int userTeamRowsAdded, SqlConnection db)
        {
            try
            {
                userTeamUrl = string.Format(userTeamUrl, userTeamId);

                HttpClient client = new HttpClient();
                JsonSerializer serializer = new JsonSerializer();
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                using (Stream s = client.GetStreamAsync(userTeamUrl).Result)
                using (StreamReader sr = new StreamReader(s))
                using (JsonReader reader = new JsonTextReader(sr))
                {

                    // read the json from a stream
                    // json size doesn't matter because only a small piece is read at a time from the HTTP request
                    var userTeamData = serializer.Deserialize<UserTeam>(reader);

                    UserTeam userTeam = userTeamData;

                    //Load UserTeam data
                    //string userTeamNameDB = "";
                    string userTeamName = "";

                    userTeamName = userTeam.name;
                    Logger.Out(userTeamName);
                    //Logger.Out("");

                    UserTeamRepository userTeamRepository = new UserTeamRepository();

                    if (!userTeamIds.Contains(userTeam.id))
                    {
                        userTeamInsert.Add(userTeam);
                        FantasyPremierLeagueAPIGameweekHistory.GetUserTeamHistoryDataJson(userTeamId, userTeamSeasonsInsert, db);
                    }
                    //else
                    //{
                    //    userTeamNameDB = userTeamRepository.GetUserTeamName(userTeamId, db);
                    //    if (userTeamName != userTeamNameDB)
                    //    {
                    //        userTeamRepository.UpdateUserTeam(userTeam, db);
                    //    }
                    //}
                }
            }
            catch (Exception ex)
            {
                Logger.Error("GetUserTeamData data exception (starting retries) (UserTeamId: " + userTeamId.ToString() + "): " + ex.Message);
                throw new Exception("GetUserTeamData data exception (UserTeamId: " + userTeamId.ToString() + "): " + ex.Message);                
            }
        }
    }
}
