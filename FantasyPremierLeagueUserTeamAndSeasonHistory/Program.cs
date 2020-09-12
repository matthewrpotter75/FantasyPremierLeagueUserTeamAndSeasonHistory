using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
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

            int userTeamRowsInserted = 0;
            int userTeamSeasonRowsInserted = 0;

            UserTeamRepository userTeamRepository = new UserTeamRepository();
            UserTeamSeasonRepository userTeamSeasonRepository = new UserTeamSeasonRepository();

            int userTeamRetries = 0;
            int userTeamRowsAdded = 0;

            UserTeams userTeamInsert = new UserTeams();
            UserTeamSeasons userTeamSeasonsInsert = new UserTeamSeasons();

            SqlConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["FantasyPremierLeagueUserTeam202021"].ConnectionString);

            try
            {
                Logger.Out("Starting...");
                Logger.Out("");

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

                string userTeamUrl = ConfigSettings.ReadSetting("userTeamURL");
                //string userTeamLeaguesUrl = ConfigSettings.ReadSetting("userTeamLeaguesURL");

                List<int> userTeamIds = Enumerable.Range(startingUserTeamId, 8000000 - startingUserTeamId).ToList();

                using (db)
                {
                    db.Open();

                    List<int> completedUserTeamIds = userTeamRepository.GetAllUserTeamIds(db);

                    List<int> toDoUserTeamIds = userTeamIds.Except(completedUserTeamIds).ToList();

                    Logger.Out(startingUserTeamId.ToString());

                    foreach (int userTeamId in toDoUserTeamIds)
                    {
                        userTeamRowsAdded += 1;
                        userTeamRetries = 0;

                        // Get the fantasyPremierLeaguePl1ayerData using JSON.NET
                        FantasyPremierLeagueAPIClient.GetUserTeamDataJson(userTeamId, completedUserTeamIds, userTeamUrl, userTeamInsert, userTeamSeasonsInsert, userTeamRetries, userTeamRowsAdded, db);

                        if (userTeamRowsAdded >= 5000)
                        //if (userTeamRowsAdded >= 10)
                        {
                            WriteToDB(userTeamRowsInserted, userTeamSeasonRowsInserted, userTeamInsert, userTeamSeasonsInsert, db);

                            userTeamRowsAdded = 0;
                            userTeamRowsInserted = 0;
                            userTeamSeasonRowsInserted = 0;

                            userTeamInsert.Clear();
                            userTeamSeasonsInsert.Clear();

                            Logger.Out(userTeamId.ToString());
                            Logger.Out("");
                        }

                        completedUserTeamIds.Add(userTeamId);
                    }

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
                Logger.Error(ex.Message);
                //Logger.Error(userTeamName + " caused error!!!");

                WriteToDB(userTeamRowsInserted, userTeamSeasonRowsInserted, userTeamInsert, userTeamSeasonsInsert, db);

                throw ex;
            }
        }

        public static void WriteToDB(int userTeamRowsInserted, int userTeamSeasonRowsInserted, UserTeams userTeamInsert, UserTeamSeasons userTeamSeasonsInsert, SqlConnection db)
        {
            UserTeamRepository userTeamRepository = new UserTeamRepository();
            UserTeamSeasonRepository userTeamSeasonRepository = new UserTeamSeasonRepository();

            Logger.Out("");

            //Write UserTeamSeason to the db
            userTeamRowsInserted = userTeamRepository.InsertUserTeam(userTeamInsert, db);
            Logger.Out("UserTeam bulk insert complete (UserTeam rows inserted: " + Convert.ToString(userTeamRowsInserted) + ")");

            //Write UserTeamSeason to the db
            userTeamSeasonRowsInserted = userTeamSeasonRepository.InsertUserTeamSeason(userTeamSeasonsInsert, db);
            Logger.Out("UserTeamSeason bulk insert complete (UserTeamSeason rows inserted: " + Convert.ToString(userTeamSeasonRowsInserted) + ")");

            Logger.Out("");
        }
    }
}