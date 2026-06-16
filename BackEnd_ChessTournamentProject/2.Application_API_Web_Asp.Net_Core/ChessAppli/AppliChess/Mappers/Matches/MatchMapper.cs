using API.DTO.MatchDTO;
using DOMAIN.Entities;


namespace API.Mappers.Matches
{
	public static class MatchMapper
	{
		public static Match_ ToEntity(MatchUpdateDTO dto)
		{
			return new Match_
			{
				Result = dto.Result
			};
		}
		public static MatchResponseDTO ToResponse(Match_ match)
		{
			return new MatchResponseDTO
			{
				Match_Id = match.Match_Id,
				RoundNumber = match.RoundNumber,
				Result = match.Result,
				Tournament_Id = match.Tournament_Id,
				WhitePlayer_Id = match.WhitePlayer_Id,
				BlackPlayer_Id = match.BlackPlayer_Id
			};
		}
	}
}
