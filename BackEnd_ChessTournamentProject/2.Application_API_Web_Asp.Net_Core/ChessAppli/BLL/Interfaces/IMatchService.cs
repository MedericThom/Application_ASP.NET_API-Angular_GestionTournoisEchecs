using DOMAIN.Entities;

namespace BLL.Interfaces
{
	public interface IMatchService
	{
		public Task UpdateMatchAsync(int matchId, int result);
		public Task<List<Match_>> GetMatchesByTournamentAndRoundAsync(int tournamentId, int round);
		public Task<List<Match_>> GetAllMatchesByTournamentAsync(int tournamentId);
	}
}
