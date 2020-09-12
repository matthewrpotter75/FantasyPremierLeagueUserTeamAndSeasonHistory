using System.Data.SqlClient;

namespace FantasyPremierLeagueUserTeams
{
    public interface IUserTeam
    {
        int InsertUserTeam(UserTeams userTeams, SqlConnection db);
        bool UpdateUserTeam(UserTeam userTeam, SqlConnection db);
        bool DeleteUserTeam(int userTeamId, SqlConnection db);
    }
}
