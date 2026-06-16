using DOMAIN.Entities;

namespace DAL.Interfaces
{
	public interface ITournamentRepository
	{
		public Task CreateTournamentAsync(Tournament tournament);
		Task AddCategoriesToTournamentAsync(int tournamentId, List<int> categoryIds);
		public Task<List<Tournament>> GetLastTenTournamentInProgressAsync();
		public Task<Tournament?> GetByIdAsync(int tournamentId);
		public Task UpdateTournamentAsync(Tournament tournament);
		public Task UpdateDeadlineAsync(int tournamentId, DateTime newDeadline);
		public Task DeleteTournamentAsync(int tournamentId);
	}
}
