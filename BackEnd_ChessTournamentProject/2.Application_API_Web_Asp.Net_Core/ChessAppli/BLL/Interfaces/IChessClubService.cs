using DOMAIN.Entities;

namespace BLL.Interfaces
{
	public interface IChessClubService
	{
		public Task CreateChessClubAsync(ChessClub chessclub);
		Task<List<ChessClub>> GetAllAsync();
		Task DeleteChessClubAsync(int chessClubId);
	}
}
