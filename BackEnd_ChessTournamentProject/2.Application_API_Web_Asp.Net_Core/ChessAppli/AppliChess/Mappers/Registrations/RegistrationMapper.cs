using API.DTO.RegistrationDTO;
using DOMAIN.Entities;


namespace API.Mappers.Registrations
{
	public static class RegistrationMapper
	{
		public static Registration ToEntity(RegistrationCreateDTO dto)
		{
			return new Registration
			{
				Player_Id = dto.Player_Id,
				Tournament_Id = dto.Tournament_Id
	        };
		}

		public static RegistrationResponseDTO ToResponse(Registration registration)
		{
			return new RegistrationResponseDTO
			{
				Registration_Id = registration.Registration_Id,
				Player_Id = registration.Player_Id,
				Tournament_Id = registration.Tournament_Id,
				Wins = registration.Wins,
				Losses = registration.Losses,
				Draws = registration.Draws,
				Score = registration.Score,
				MatchesPlayed = registration.MatchesPlayed,
				RegistrationDate = registration.RegistrationDate
			};
		}

		public static ScoreDTO ToScore(Registration registration, Player player)
		{
			return new ScoreDTO
			{
			Pseudo = player.Pseudo,
			Wins = registration.Wins,
			Losses = registration.Losses,
			Draws = registration.Draws,
			Score = registration.Score,
			MatchesPlayed = registration.MatchesPlayed,
	};
		}
	}
}
