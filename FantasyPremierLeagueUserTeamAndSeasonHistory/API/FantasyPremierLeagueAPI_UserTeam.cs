﻿using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net;
using System.Data.SqlClient;

namespace FantasyPremierLeagueUserTeams
{
    public class FantasyPremierLeagueAPIUserTeam
    {
        public static void GetUserTeamData(int userTeamId, List<int> userTeamIds, string userTeamUrl, UserTeams userTeamInsert, UserTeamClassicLeagues userTeamClassicLeaguesInsert, UserTeamH2hLeagues userTeamH2hLeaguesInsert, SqlConnection db)
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
                    Globals.apiCalls += 1;
                    Globals.apiUserTeamCalls += 1;

                    // read the json from a stream
                    // json size doesn't matter because only a small piece is read at a time from the HTTP request
                    var userTeamData = serializer.Deserialize<UserTeam>(reader);

                    UserTeam userTeam = userTeamData;

                    //Load UserTeam data
                    //string userTeamNameDB = "";
                    string userTeamName = "";

                    userTeamName = userTeam.name;
                    Logger.Out(userTeamName + " (" + Convert.ToString(userTeamId) + ")");
                    //Logger.Out("");

                    UserTeamRepository userTeamRepository = new UserTeamRepository();

                    if (!userTeamIds.Contains(userTeam.id) && !userTeamInsert.Contains(userTeam))
                    {
                        userTeamInsert.Add(userTeam);
                    }
                    //else
                    //{
                    //    userTeamNameDB = userTeamRepository.GetUserTeamName(userTeamId, db);
                    //    if (userTeamName != userTeamNameDB)
                    //    {
                    //        userTeamRepository.UpdateUserTeam(userTeam, db);
                    //    }
                    //}

                    GetUserTeamClassicLeagueJson(userTeamId, userTeamData, userTeamClassicLeaguesInsert, db);

                    if (userTeam.leagues.h2h != null)
                    {
                        GetUserTeamH2hLeagueJson(userTeamId, userTeamData, userTeamH2hLeaguesInsert, db);
                    }

                    //if (userTeam.leagues.cup != null)
                    //{
                    //    GetUserTeamCupJson(userTeamId, userTeamData, db);
                    //}
                }
            }
            catch (Exception ex)
            {
                Logger.Error("GetUserTeamData data exception (UserTeamId: " + userTeamId.ToString() + "): " + ex.Message);
                throw new Exception("GetUserTeamData data exception (UserTeamId: " + userTeamId.ToString() + "): " + ex.Message);                
            }
        }

        public static void GetUserTeamCupJson(int userTeamId, UserTeam userTeam, SqlConnection db)
        {
            try
            {
                //Load Cup data
                UserTeamCupRepository cupRepository = new UserTeamCupRepository();

                List<int> CupIds = cupRepository.GetAllCupIdsForUserId(userTeamId, db);

                int cupid, gameweekid;

                foreach (UserTeamCupMatch match in userTeam.leagues.cup.matches)
                {
                    match.fromuserteamid = userTeamId;

                    if (!CupIds.Contains(match.id))
                    {
                        cupRepository.InsertUserTeamCup(match, db);
                    }
                    else
                    {
                        cupRepository.UpdateUserTeamCup(match, db);
                    }

                    if (match.tiebreak != null)
                    {
                        cupid = match.id;
                        gameweekid = match.@event;
                        //GetUserTeamCupTiebreakJson(userTeamId, cupid, gameweekid, cup);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("GetUserTeamCupJson data exception (UserTeamId: " + userTeamId.ToString() + "): " + ex.Message);
                throw new Exception("GetUserTeamCupJson data exception (UserTeamId: " + userTeamId.ToString() + "): " + ex.Message);
                //GetUserTeamCupJson(userTeamId, userTeamHistoryData);
            }
        }

        //public static void GetUserTeamCupTiebreakJson(int userTeamId, int cupid, int gameweekid, UserTeamCup cup, SqlConnection db)
        //{
        //    try
        //    {
        //        //Load Cup tiebreak data
        //        UserTeamCupTiebreakRepository cupTiebreakRepository = new UserTeamCupTiebreakRepository();

        //        List<string> CupTiebreakNames = cupTiebreakRepository.GetAllCupNamesForUserId(userTeamId);

        //        List<UserTeamCupMatch> matches = cup.matches;

        //        foreach (UserTeamCupMatch match in matches)
        //        {
        //            foreach (UserTeamCupTiebreak cupTiebreak in match.tiebreak)
        //            {

        //                //UserTeamCupTiebreak cupTiebreak = new UserTeamCupTiebreak();

        //                //cupTiebreak = cup.tiebreak;

        //                //needed if want to assign value from parent to add into db table
        //                cupTiebreak.userteamcupid = cupid;
        //                cupTiebreak.gameweekid = gameweekid;
        //                cupTiebreak.userteamid = userTeamId;

        //                if (!CupTiebreakNames.Contains(cupTiebreak.name))
        //                {
        //                    cupTiebreakRepository.InsertUserTeamCupTiebreak(cupTiebreak);
        //                }
        //                else
        //                {
        //                    cupTiebreakRepository.UpdateUserTeamCupTiebreak(cupTiebreak);
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Error("GetUserTeamCupTiebreakJson data exception (UserTeamId: " + userTeamId.ToString() + "): " + ex.Message);
        //        throw new Exception("GetUserTeamCupTiebreakJson data exception (UserTeamId: " + userTeamId.ToString() + "): " + ex.Message);
        //        //GetUserTeamCupTiebreakJson(userTeamId, cupid, gameweekid, cup);
        //    }
        //}

        public static void GetUserTeamClassicLeagueJson(int userTeamId, UserTeam userTeam, UserTeamClassicLeagues userTeamClassicLeaguesInsert, SqlConnection db)
        {
            try
            {
                //Load ClassicLeague data
                UserTeamClassicLeagueRepository classicLeagueRepository = new UserTeamClassicLeagueRepository();

                List<int> classicLeagueIds = classicLeagueRepository.GetAllClassicLeagueIdsForUserTeamId(userTeamId, db);

                foreach (UserTeamClassicLeague classicLeague in userTeam.leagues.classic)
                {
                    //needed if want to assign value from parent to add into db table
                    classicLeague.userteamid = userTeamId;

                    if (!classicLeagueIds.Contains(classicLeague.id) && !userTeamClassicLeaguesInsert.Contains(classicLeague))
                    {
                        //classicLeagueRepository.InsertUserTeamClassicLeague(classicLeague, db);
                        userTeamClassicLeaguesInsert.Add(classicLeague);
                    }
                    //NEED TO ADD BACK IN WHEN RERUNNING OR WILL NOT UPDATE LEAGUES
                    //else
                    //{
                    //    classicLeagueRepository.UpdateUserTeamClassicLeague(classicLeague);
                    //}
                }
            }
            catch (Exception ex)
            {
                Logger.Error("GetUserTeamClassicLeagueJson data exception (UserTeamId: " + userTeamId.ToString() + "): " + ex.Message);
                throw new Exception("GetUserTeamClassicLeagueJson data exception (UserTeamId: " + userTeamId.ToString() + "): " + ex.Message);
            }
        }

        public static void GetUserTeamH2hLeagueJson(int userTeamId, UserTeam userTeam, UserTeamH2hLeagues userTeamH2hLeaguesInsert, SqlConnection db)
        {
            try
            {
                //Load H2hLeague data
                UserTeamH2hLeagueRepository h2hLeagueRepository = new UserTeamH2hLeagueRepository();

                List<int> h2hLeagueIds = h2hLeagueRepository.GetAllH2hLeagueIdsForUserTeamId(userTeamId, db);

                foreach (UserTeamH2hLeague h2hLeague in userTeam.leagues.h2h)
                {
                    //needed if want to assign value from parent to add into db table
                    h2hLeague.userteamid = userTeamId;

                    if (!h2hLeagueIds.Contains(h2hLeague.id) && !userTeamH2hLeaguesInsert.Contains(h2hLeague))
                    {
                        //h2hLeagueRepository.InsertUserTeamH2hLeague(h2hLeague, db);
                        userTeamH2hLeaguesInsert.Add(h2hLeague);
                    }
                    //NEED TO ADD BACK IN WHEN RERUNNING OR WILL NOT UPDATE LEAGUES
                    //else
                    //{
                    //    h2hLeagueRepository.UpdateUserTeamH2hLeague(h2hLeague);
                    //}
                }
            }
            catch (Exception ex)
            {
                Logger.Error("GetUserTeamH2hLeagueJson data exception (UserTeamId: " + userTeamId.ToString() + "): " + ex.Message);
                throw new Exception("GetUserTeamH2hLeagueJson data exception (UserTeamId: " + userTeamId.ToString() + "): " + ex.Message);
            }
        }
    }
}
