using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using System.Configuration;
using log4net.Config;

namespace FantasyPremierLeagueUserTeams
{
    class Program
    {
        static void Main(string[] args)
        {
            XmlConfigurator.Configure();

            //string LogName = GetType().Assembly.GetName().Name + ".log";
            //log4net.GlobalContext.Properties["LogName"] = LogName;

            UserTeamRepository userTeamRepository = new UserTeamRepository();
            UserTeamSeasonRepository userTeamSeasonRepository = new UserTeamSeasonRepository();
            UserTeamClassicLeagueRepository userTeamClassicLeagueRepository = new UserTeamClassicLeagueRepository();

            UserTeams userTeamInsert = new UserTeams();
            UserTeamSeasons userTeamSeasonsInsert = new UserTeamSeasons();
            UserTeamGameweekHistories userTeamGameweekHistoriesInsert = new UserTeamGameweekHistories();
            UserTeamChips userTeamChipsInsert = new UserTeamChips();
            UserTeamClassicLeagues userTeamClassicLeaguesInsert = new UserTeamClassicLeagues();
            UserTeamH2hLeagues userTeamH2hLeaguesInsert = new UserTeamH2hLeagues();

            SqlConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["FantasyPremierLeagueUserTeam202021"].ConnectionString);

            try
            {
                Logger.Out("Starting...");
                Logger.Out("");

                int userTeamRetries = 0;

                int startingUserTeamId = 1;
                bool test = true;

                if (args.Length == 0)
                {
                    System.Console.WriteLine("No arguments passed into program");
                }
                else
                {
                    test = int.TryParse(args[0], out startingUserTeamId);
                    if (test == false)
                    {
                        System.Console.WriteLine("Arguments passed into program were not integer");
                        return;
                    }
                }

                Logger.Out("Starting UserTeams data load");
                Logger.Out("");

                FantasyPremierLeagueAPI_Bootstrap.GetPlayerBootstrapDataJson();

                string userTeamUrl = ConfigSettings.ReadSetting("userTeamURL");
                //string userTeamLeaguesUrl = ConfigSettings.ReadSetting("userTeamLeaguesURL");

                using (db)
                {
                    db.Open();

                    SetLatestGameweek("UserTeamGameweekHistory");
                    SetActualGameweek("UserTeamGameweekPick");

                    //List<string> userTeamSeasonNames;

                    Logger.Out("Starting GetAllUserTeamIds call");
                    List<int> existingUserTeamIds = userTeamRepository.GetAllUserTeamIds(startingUserTeamId, db);
                    Logger.Out("Starting GetAllUserTeamIdsWithLeagues call");
                    List<int> userTeamIdsWithLeagues = userTeamClassicLeagueRepository.GetAllUserTeamIdsWithLeagues(startingUserTeamId, db);
                    Logger.Out("Starting GetAllUserTeamIdsWithSeasons call");
                    List<int> userTeamIdsWithSeasons = userTeamSeasonRepository.GetAllUserTeamIdsWithSeasons(startingUserTeamId, db);

                    int userTeamsToProcess = Globals.bootstrapUserTeamCount - startingUserTeamId;

                    if (userTeamsToProcess < 0)
                        userTeamsToProcess = 0;

                    List<int> userTeamIds = Enumerable.Range(startingUserTeamId, userTeamsToProcess).ToList();

                    //List<int> toDoUserTeamIds = userTeamIds.Except(userTeamIdsWithLeagues).ToList();
                    List<int> toDoUserTeamIds = userTeamIds.Except(existingUserTeamIds).ToList();

                    Logger.Out("");
                    Logger.Out("Starting UserTeamId" + startingUserTeamId.ToString());
                    Logger.Out("");

                    Globals.apiCalls = 0;
                    Globals.apiUserTeamCalls = 0;
                    Globals.apiUserTeamHistoryCalls = 0;
                    Globals.userTeamInsertCount = 0;

                    FantasyPremierLeagueAPIClient.GetUserTeamDataJson(startingUserTeamId, toDoUserTeamIds, existingUserTeamIds, userTeamIdsWithSeasons, userTeamUrl, userTeamInsert, userTeamGameweekHistoriesInsert, userTeamChipsInsert, userTeamSeasonsInsert, userTeamClassicLeaguesInsert, userTeamH2hLeaguesInsert, userTeamRetries, db);
                    db.Close();
                }

                Logger.Out("UserTeams data load complete");
                Logger.Out("");

                Logger.Out("Finished!!!");

                //// Wait for user input - keep the program running
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Logger.Error("Program error: " + ex.Message);
                throw ex;
            }
        }

        private static void SetLatestGameweek(string entity)
        {
            int latestGameweek;
            UserTeamRepository userTeamRepository = new UserTeamRepository();

            using (SqlConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["FantasyPremierLeague"].ConnectionString))
            {
                db.Open();

                latestGameweek = userTeamRepository.GetLatestGameweekId(db, entity);

                Globals.latestGameweek = latestGameweek;

                db.Close();
            }
        }

        private static void SetActualGameweek(string entity)
        {
            int latestGameweek;
            UserTeamRepository userTeamRepository = new UserTeamRepository();

            using (SqlConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["FantasyPremierLeague"].ConnectionString))
            {
                db.Open();

                latestGameweek = userTeamRepository.GetLatestGameweekId(db, entity);

                Globals.actualGameweek = latestGameweek;

                db.Close();
            }
        }
    }
}