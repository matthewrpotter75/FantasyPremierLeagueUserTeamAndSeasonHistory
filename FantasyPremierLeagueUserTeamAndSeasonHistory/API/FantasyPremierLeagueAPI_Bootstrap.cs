using System;
using System.IO;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net;


namespace FantasyPremierLeagueUserTeams
{
    public class FantasyPremierLeagueAPI_Bootstrap
    {
        public static void GetPlayerBootstrapDataJson()
        {
            try
            {
                string baseBootstrapUrl = ConfigSettings.ReadSetting("bootstrapURL");

                HttpClient client = new HttpClient();
                JsonSerializer serializer = new JsonSerializer();
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                using (Stream s = client.GetStreamAsync(baseBootstrapUrl).Result)
                using (StreamReader sr = new StreamReader(s))
                using (JsonReader reader = new JsonTextReader(sr))
                {
                    // read the json from a stream
                    // json size doesn't matter because only a small piece is read at a time from the HTTP request
                    var fantasyPremierLeagueBootstrapData = serializer.Deserialize<FantasyPremierLeagueBootstrapData>(reader);

                    Globals.bootstrapUserTeamCount = fantasyPremierLeagueBootstrapData.total_players;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("GetPlayerBootstrapDataJson data exception: " + ex.Message);
                throw new Exception("GetPlayerBootstrapDataJson data exception: " + ex.Message);
            }
        }
    }
}
