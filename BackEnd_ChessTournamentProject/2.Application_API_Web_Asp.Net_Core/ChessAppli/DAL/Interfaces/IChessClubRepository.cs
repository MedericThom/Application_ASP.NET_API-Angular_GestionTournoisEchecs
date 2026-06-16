using DOMAIN.Entities;

namespace DAL.Interfaces
{
	public interface IChessClubRepository
	{
		public Task CreateChessClubAsync(ChessClub chessclub);
		Task<List<ChessClub>> GetAllAsync();
		Task<ChessClub> GetByIdAsync(int chessClubId);
		Task DeleteChessClubAsync(int chessClubId);
	}
}
