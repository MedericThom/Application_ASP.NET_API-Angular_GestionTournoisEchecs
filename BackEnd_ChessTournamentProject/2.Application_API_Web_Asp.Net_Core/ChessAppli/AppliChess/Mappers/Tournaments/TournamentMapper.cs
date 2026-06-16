using API.DTO.MatchDTO;
using API.DTO.PlayerDTO;
using API.DTO.TournamentDTO;
using DOMAIN.Entities;

namespace API.Mappers.Tournaments
{
	public static class TournamentMapper
	{
		public static Tournament ToEntity(TournamentCreateDTO dto)
		{
			return new Tournament
			{
				NameTournament = dto.NameTournament,
				Place = dto.Place,
				MinNbPlayer = dto.MinNbPlayer,
				MaxNbPlayer = dto.MaxNbPlayer,
				MinElo = dto.MinElo,
				MaxElo  = dto.MaxElo,
				WomenOnly = dto.WomenOnly,
				MaxRounds = dto.MaxRounds,
				RegistrationDeadline = dto.RegistrationDeadline,
	        };
		}

		public static TournamentDetailDTO ToDetail(Tournament tournament, List<PlayerResponseDTO> players, List<MatchResponseDTO> matches)
		{
			return new TournamentDetailDTO
			{
				Tournament_Id = tournament.Tournament_Id,
				NameTournament = tournament.NameTournament,
				Place = tournament.Place,
				MinNbPlayer = tournament.MinNbPlayer,
				MaxNbPlayer = tournament.MaxNbPlayer,
				MinElo = tournament.MinElo,
				MaxElo = tournament.MaxElo,
				StatusTournament = tournament.StatusTournament,
				WomenOnly = tournament.WomenOnly,
				MaxRounds = tournament.MaxRounds,
				CurrentRound = tournament.CurrentRound,
				RegistrationDeadline = tournament.RegistrationDeadline,
				CreationDate = tournament.CreationDate,
				UpdateDate = tournament.UpdateDate,
				Players = players,
				Matches = matches
			};
		}

		public static TournamentResponseDTO ToResponse(Tournament tournament)
		{
			return new TournamentResponseDTO
			{
				Tournament_Id = tournament.Tournament_Id,
				NameTournament = tournament.NameTournament,
				Place = tournament.Place,
				MinNbPlayer = tournament.MinNbPlayer,
				MaxNbPlayer = tournament.MaxNbPlayer,
				MinElo = tournament.MinElo,
				MaxElo = tournament.MaxElo,
				StatusTournament = tournament.StatusTournament,
				WomenOnly = tournament.WomenOnly,
				MaxRounds = tournament.MaxRounds,
				CurrentRound = tournament.CurrentRound,
				RegistrationDeadline = tournament.RegistrationDeadline,
				CreationDate = tournament.CreationDate,
				UpdateDate = tournament.UpdateDate,
				PlayerCount = tournament.PlayerCount
			};
		}
	}
}
