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
        public static void GetUserTeamHistoryDataJson(int userTeamId, List<int> userTeamIdsWithSeasons, UserTeamGameweekHistories userTeamGameweekHistoriesInsert, UserTeamChips userTeamChipsInsert, UserTeamSeasons userTeamSeasonsInsert, SqlConnection db)
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
                    Globals.apiCalls += 1;
                    Globals.apiUserTeamHistoryCalls += 1;

                    // read the json from a stream
                    // json size doesn't matter because only a small piece is read at a time from the HTTP request
                    var userTeamHistoryData = serializer.Deserialize<UserTeamGameweekHistoryData>(reader);

                    //Process UserTeamSeasonHistory
                    if (userTeamHistoryData.past != null && !userTeamIdsWithSeasons.Contains(userTeamId))
                    {
                        GetUserTeamSeasonJson(userTeamId, userTeamHistoryData, userTeamSeasonsInsert, db);
                    }

                    //Process UserTeamGameweekHistory
                    if (userTeamHistoryData.current != null && !userTeamIdsWithSeasons.Contains(userTeamId))
                    {
                        GetUserTeamGameweekHistoryJson(userTeamId, userTeamHistoryData, userTeamGameweekHistoriesInsert, db);
                    }

                    //Process UserTeamChip
                    if (userTeamHistoryData.chips != null && !userTeamIdsWithSeasons.Contains(userTeamId))
                    {
                        GetUserTeamChipJson(userTeamId, userTeamChipsInsert, userTeamHistoryData, db);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("GetUserTeamHistoryDataJson data exception (UserTeamId: " + userTeamId.ToString() + "): " + ex.Message);
                throw new Exception("GetUserTeamHistoryDataJson data exception (UserTeamId: " + userTeamId.ToString() + "): " + ex.Message);
            }
        }

        public static void GetUserTeamGameweekHistoryJson(int userTeamId, UserTeamGameweekHistoryData userTeamHistoryData, UserTeamGameweekHistories userTeamGameweekHistoriesInsert, SqlConnection db)
        {
            //Logger.Out("GetUserTeamGameweekHistoryJson starting");
            //int gameweekId;

            try
            {
                //Load UserTeamGameweekHistory data
                UserTeamGameweekHistoryRepository userTeamGameweekHistoryRepository = new UserTeamGameweekHistoryRepository();

                List<int> userTeamGameweekHistoryIds = new List<int>();

                //List<int> UserTeamGameweekHistoryIds = userTeamGameweekHistoryRepository.GetAllUserTeamGameweekHistoryIdsForUserTeamId(userTeamId, db);
                userTeamGameweekHistoryIds = userTeamGameweekHistoryRepository.GetAllUserTeamGameweekHistoryIdsForUserTeamId(userTeamId, db);

                //string userTeamPicksUrl = ConfigSettings.ReadSetting("userTeamPicksUrl");

                foreach (UserTeamGameweekHistory userTeamGameweekHistory in userTeamHistoryData.current)
                {
                    if (userTeamGameweekHistory.@event <= Globals.latestGameweek)
                    {
                        userTeamGameweekHistory.userteamid = userTeamId;

                        if (!userTeamGameweekHistoryIds.Contains(userTeamGameweekHistory.@event) && !userTeamGameweekHistoriesInsert.Contains(userTeamGameweekHistory))
                        {
                            userTeamGameweekHistoriesInsert.Add(userTeamGameweekHistory);
                        }
                        //else
                        //{
                        //    userTeamGameweekHistoryRepository.UpdateUserTeamGameweekHistory(userTeamGameweekHistory);
                        //}
                    }
                }
                //Logger.Out("GetUserTeamGameweekHistoryJson completed");
            }
            catch (Exception ex)
            {
                Logger.Error("GetUserTeamGameweekHistoryJson data exception (UserTeamId: " + userTeamId.ToString() + "): " + ex.Message);
                throw new Exception("GetUserTeamGameweekHistoryJson data exception (UserTeamId: " + userTeamId.ToString() + "): " + ex.Message);
            }
        }

        public static void GetUserTeamChipJson(int userTeamId, UserTeamChips userTeamChipsInsert, UserTeamGameweekHistoryData userTeamHistoryData, SqlConnection db)
        {
            try
            {
                //Logger.Out("GetUserTeamChipJson: Gameweek " + Convert.ToString(gameweekId) + " - starting");

                //Load UserTeamChip data
                UserTeamChipRepository userTeamChipRepository = new UserTeamChipRepository();

                List<UserTeamChipId> userTeamChipIds = new List<UserTeamChipId>();

                //List<UserTeamChipId> userTeamChipIds = userTeamChipRepository.GetAllUserTeamChipIdsForUserTeamId(userTeamId, db);
                userTeamChipIds = userTeamChipRepository.GetAllUserTeamChipIdsForUserTeamId(userTeamId, db);

                foreach (UserTeamChip userTeamChip in userTeamHistoryData.chips)
                {
                    userTeamChip.userteamid = userTeamId;
                    userTeamChip.chip = userTeamChipRepository.GetChipIdFromName(userTeamChip.name, db);

                    UserTeamChipId userTeamChipId = new UserTeamChipId
                    (
                        userTeamId,
                        userTeamChip.@event,
                        userTeamChip.chip
                    );

                    //UserTeamChipIds = userTeamChipRepository.GetAllUserTeamPickIdsForUserTeamIdAndGameweekIdAndChipId(userTeamChipId);

                    //needed if want to assign value from parent to add into db table
                    //userTeamChip.entry = userTeamId;

                    userTeamChipId.gameweekid = userTeamChip.@event;
                    userTeamChipId.chipid = userTeamChip.chip;

                    if (!userTeamChipIds.Contains(userTeamChipId) && !userTeamChipsInsert.Contains(userTeamChip))
                    {
                        userTeamChipsInsert.Add(userTeamChip);
                    }
                    //else
                    //{
                    //    userTeamChipRepository.UpdateUserTeamChip(userTeamChip);
                    //}
                }
                //Logger.Out("GetUserTeamChipJson: Gameweek " + Convert.ToString(gameweekId) + " - completed");
            }
            catch (Exception ex)
            {
                Logger.Error("GetUserTeamChipJson data exception (UserTeamId: " + userTeamId.ToString() + "): " + ex.Message);
                throw new Exception("GetUserTeamChipJson data exception (UserTeamId: " + userTeamId.ToString() + "): " + ex.Message);
                //GetUserTeamChipJson(userTeamId, userTeamChipsInsert, userTeamHistoryData);
            }
        }

        public static void GetUserTeamSeasonJson(int userTeamId, UserTeamGameweekHistoryData userTeamHistoryData, UserTeamSeasons userTeamSeasonsInsert, SqlConnection db)
        {
            try
            {
                //Load UserTeamSeason data
                UserTeamSeasonRepository userTeamSeasonRepository = new UserTeamSeasonRepository();

                List<string> UserTeamSeasonNames = userTeamSeasonRepository.GetAllUserTeamSeasonNamesForUserTeamId(userTeamId, db);

                foreach (UserTeamSeason userTeamSeason in userTeamHistoryData.past)
                {
                    //needed if want to assign value from parent to add into db table
                    userTeamSeason.userteamid = userTeamId;
                    //userTeamSeason.season = userTeamSeasonRepository.GetSeasonIdFromSeasonName(userTeamSeason.season_name);

                    if (!UserTeamSeasonNames.Contains(userTeamSeason.season_name))
                    {
                        userTeamSeasonsInsert.Add(userTeamSeason);
                    }
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