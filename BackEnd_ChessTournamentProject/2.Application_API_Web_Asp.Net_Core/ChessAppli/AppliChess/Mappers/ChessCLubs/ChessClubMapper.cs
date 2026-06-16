using API.DTO.ChessClubDTO;
using DOMAIN.Entities;

namespace API.Mappers.ChessClubs
{
	public static class ChessClubMapper
	{
		public static ChessClub ToEntity(ChessClubCreateDTO dto)
		{
			return new ChessClub
			{
				NameChessClub = dto.NameChessClub
			};
		}

		public static ChessClubResponseDTO ToResponse(ChessClub chessClub)
		{
			return new ChessClubResponseDTO
			{
				ChessClub_Id = chessClub.ChessClub_Id,
				NameChessClub = chessClub.NameChessClub
			};
		}
	}
}
