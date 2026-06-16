using API.DTO.MatchDTO;
using API.Mappers.Matches;
using BLL.Interfaces;
using DOMAIN.Entities;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class MatchController : ControllerBase
	{
		#region Connexion à la BLL
		private readonly IMatchService _matchService;
		public MatchController(IMatchService matchService)
		{
			_matchService = matchService;
		}
		#endregion

		[HttpGet]
		public async Task<IActionResult> GetMatches([FromQuery] int tournamentId, [FromQuery] int round)
		{
			List<Match_> matches = await _matchService.GetMatchesByTournamentAndRoundAsync(tournamentId, round);
			return Ok(matches.Select(MatchMapper.ToResponse));
		}

		[HttpGet("tournament/{tournamentId}")]
		public async Task<IActionResult> GetByTournament(int tournamentId)
		{
			List<Match_> matches = await _matchService.GetAllMatchesByTournamentAsync(tournamentId);
			return Ok(matches.Select(MatchMapper.ToResponse));
		}

		[HttpPatch("{id}")]
		public async Task<IActionResult> UpdateMatch(int id, MatchUpdateDTO dto)
		{
			Match_ match = MatchMapper.ToEntity(dto);
			await _matchService.UpdateMatchAsync(id, match.Result.Value);
			return NoContent();
		}
	}
}
