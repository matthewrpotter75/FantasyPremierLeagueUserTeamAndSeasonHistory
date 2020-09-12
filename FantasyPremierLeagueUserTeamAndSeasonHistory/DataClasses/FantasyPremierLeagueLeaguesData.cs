using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FantasyPremierLeagueUserTeams
{

    public class LeagueStandingsData
    {
        public NewEntries new_entries { get; set; }
        public League league { get; set; }
        public Standings standings { get; set; }
        public int update_status { get; set; }
    }

    public class NewEntries
    {
        public bool has_next { get; set; }
        public int number { get; set; }
        public List<object> results { get; set; }
    }

    public class League
    {
        public int id { get; set; }
        public List<object> leagueban_set { get; set; }
        public string name { get; set; }
        public string short_name { get; set; }
        public DateTime created { get; set; }
        public bool closed { get; set; }
        public bool forum_disabled { get; set; }
        public bool make_code_public { get; set; }
        public object rank { get; set; }
        public object size { get; set; }
        public string league_type { get; set; }
        public string _scoring { get; set; }
        public bool reprocess_standings { get; set; }
        public object admin_entry { get; set; }
        public int start_event { get; set; }
    }

    public class Standings
    {
        public bool has_next { get; set; }
        public int number { get; set; }
        public List<TeamLeaguePosition> results { get; set; }
    }

    public class TeamLeaguePosition
    {
        public int id { get; set; }
        public string entry_name { get; set; }
        public int event_total { get; set; }
        public string player_name { get; set; }
        public string movement { get; set; }
        public bool own_entry { get; set; }
        public int rank { get; set; }
        public int last_rank { get; set; }
        public int rank_sort { get; set; }
        public int total { get; set; }
        public int entry { get; set; }
        public int league { get; set; }
        public int start_event { get; set; }
        public int stop_event { get; set; }
    }
}