using API.DTO.PlayerDTO;
using DOMAIN.Entities;

namespace API.Mappers.Players
{
	public static class PlayerMapper
	{
		public static Player ToEntity(PlayerCreateDTO dto)
		{
			return new Player
			{
				Pseudo = dto.Pseudo,
				Email = dto.Email,
				Pwd = dto.Password,
				BirthDate = dto.BirthDate,
				Gender = dto.Gender,
				Elo = dto.Elo
			};
		}

		public static PlayerResponseDTO ToResponse(Player player)
		{
			return new PlayerResponseDTO
			{
				Player_Id = player.Player_Id,
				Pseudo = player.Pseudo,
				Email = player.Email,
				BirthDate = player.BirthDate,
				Gender = player.Gender, 
				Elo = player.Elo,
				ChessClub_Id = player.ChessClub_Id
			};
		}
	}	
}
