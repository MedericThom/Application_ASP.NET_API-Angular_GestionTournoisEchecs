using DOMAIN.Entities;

namespace DAL.Interfaces
{
	public interface IMatchRepository
	{
		public Task CreateMatchesAsync(List<Match_> matches);
		public Task<Match_?> GetMatchByIdAsync(int matchId);
		public Task<List<Match_>> GetMatchesByTournamentAndRoundAsync(int tournamentId, int round);
		public Task<List<Match_>> GetAllMatchesByTournamentAsync(int tournamentId);
		public Task UpdateMatchAsync(Match_ match);
	}
}
