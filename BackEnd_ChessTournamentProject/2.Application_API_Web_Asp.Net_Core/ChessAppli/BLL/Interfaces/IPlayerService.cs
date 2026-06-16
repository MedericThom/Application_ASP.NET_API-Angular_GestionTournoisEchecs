using DOMAIN.Entities;

namespace BLL.Interfaces
{
	public interface IPlayerService
	{
		public Task CreatePlayerAsync(Player player);
		public Task<Player> GetByPlayerIdAsync(int playerId);
		public Task<List<Player>> GetPlayersByTournamentIdAsync(int tournamentId);
		Task<List<Player>> GetAllAsync();
		Task<Player> UpdateClubAsync(int playerId, int? chessClubId);
		Task<(int Trophies, int Victories, int Draws, int Defeats)> GetStatsByPlayerIdAsync(int playerId);
		Task DeletePlayerAsync(int playerId);
	}
}
