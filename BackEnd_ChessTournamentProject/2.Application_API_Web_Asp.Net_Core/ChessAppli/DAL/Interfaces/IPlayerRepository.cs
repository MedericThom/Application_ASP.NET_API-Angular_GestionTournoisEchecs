using DOMAIN.Entities;

namespace DAL.Interfaces
{
	public interface IPlayerRepository
	{
		public Task CreatePlayerAsync(Player player);
		public Task<Player> GetByPlayerIdAsync(int playerId);
		public Task<List<Player>> GetPlayersByTournamentIdAsync(int tournamentId);
		public Task<Player> GetByEmailAsync(string email);
		public Task<Player> GetByPseudoAsync(string pseudo);
		Task<List<Player>> GetAllAsync();
		Task<Player> UpdateClubAsync(int playerId, int? chessClubId);
		Task<(int Trophies, int Victories, int Draws, int Defeats)> GetStatsByPlayerIdAsync(int playerId);
		Task DeletePlayerAsync(int playerId);
	}
}
