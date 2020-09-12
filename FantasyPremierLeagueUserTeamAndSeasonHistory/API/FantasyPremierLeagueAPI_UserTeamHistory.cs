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
    public class FantasyPremierLeagueAPIGameweekHistory
    {
        public static void GetUserTeamHistoryDataJson(int userTeamId, UserTeamSeasons userTeamSeasonsInsert, SqlConnection db)
        {
            try
            {
                string userTeamHistoryUrl = ConfigSettings.ReadSetting("userTeamHistoryURL");

                userTeamHistoryUrl = string.Format(userTeamHistoryUrl, userTeamId);

                HttpClient client = new HttpClient();
                JsonSerializer serializer = new JsonSerializer();
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                using (Stream s = client.GetStreamAsync(userTeamHistoryUrl).Result)
                using (StreamReader sr = new StreamReader(s))
                using (JsonReader reader = new JsonTextReader(sr))
                {

                    // read the json from a stream
                    // json size doesn't matter because only a small piece is read at a time from the HTTP request
                    var userTeamHistoryData = serializer.Deserialize<UserTeamGameweekHistoryData>(reader);

                    if (userTeamHistoryData.past != null)
                    {
                        GetUserTeamSeasonJson(userTeamId, userTeamHistoryData, userTeamSeasonsInsert, db);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("GetUserTeamHistoryDataJson data exception (UserTeamId: " + userTeamId.ToString() + "): " + ex.Message);
                throw new Exception("GetUserTeamHistoryDataJson data exception (UserTeamId: " + userTeamId.ToString() + "): " + ex.Message);
            }
        }

        public static void GetUserTeamSeasonJson(int userTeamId, UserTeamGameweekHistoryData userTeamHistoryData, UserTeamSeasons userTeamSeasonsInsert, SqlConnection db)
        {
            try
            {
                //Load UserTeamSeason data
                UserTeamSeasonRepository userTeamSeasonRepository = new UserTeamSeasonRepository();

                //List<string> UserTeamSeasonNames = userTeamSeasonRepository.GetAllUserTeamSeasonNamesForUserTeamId(userTeamId, db);

                foreach (UserTeamSeason userTeamSeason in userTeamHistoryData.past)
                {
                    //needed if want to assign value from parent to add into db table
                    userTeamSeason.userteamid = userTeamId;
                    //userTeamSeason.season = userTeamSeasonRepository.GetSeasonIdFromSeasonName(userTeamSeason.season_name);

                    //if (!UserTeamSeasonNames.Contains(userTeamSeason.season_name))
                    //{
                        userTeamSeasonsInsert.Add(userTeamSeason);
                    //}
                    //else
                    //{
                    //    userTeamSeasonRepository.UpdateUserTeamSeason(userTeamSeason);
                    //}
                }
            }
            catch (Exception ex)
            {
                Logger.Error("GetUserTeamSeasonJson data exception (UserTeamId: " + userTeamId.ToString() + "): " + ex.Message);
                throw new Exception("GetUserTeamSeasonJson data exception (UserTeamId: " + userTeamId.ToString() + "): " + ex.Message);
            }
        }
    }
}