
using API.DTO.TournamentDTO;
using API.Mappers.Tournaments;
using API.DTO.PlayerDTO;
using API.DTO.MatchDTO;
using API.Mappers.Players;
using API.Mappers.Matches;
using BLL.Interfaces;
using BLL.Services;
using DOMAIN.Entities;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class TournamentController : ControllerBase
	{
		#region Connexion à la BLL
		private readonly ITournamentService _tournamentService;
		private readonly IPlayerService _playerService;
		private readonly IMatchService _matchService;
		public TournamentController(
			ITournamentService tournamentService,
			IPlayerService playerService,
			IMatchService matchService)
		{
			_tournamentService = tournamentService;
			_playerService = playerService;
			_matchService = matchService;
		}
		#endregion

		[HttpPost]
		public async Task<IActionResult> CreateTournament(TournamentCreateDTO dto)
		{
			Tournament tournament = TournamentMapper.ToEntity(dto);
			await _tournamentService.CreateTournamentAsync(tournament);
			await _tournamentService.AddCategoriesAsync(tournament.Tournament_Id, dto.CategoryIds);
			return CreatedAtAction(nameof(CreateTournament), TournamentMapper.ToResponse(tournament));
		}

		[HttpGet]
		public async Task<IActionResult> GetTournaments()
		{
			List<Tournament> getLastTentournaments = await _tournamentService.GetTournamentsAsync();
			List<TournamentResponseDTO> response = getLastTentournaments
				.Select(t => TournamentMapper.ToResponse(t))
				.ToList();
			return Ok(response);
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetTournamentById(int id)
		{
			Tournament tournament = await _tournamentService.GetTournamentByIdAsync(id);
			if (tournament == null) return NotFound();

			List<PlayerResponseDTO> players = (await _playerService.GetPlayersByTournamentIdAsync(id))
				.Select(p => PlayerMapper.ToResponse(p))
				.ToList();

			List<MatchResponseDTO> matches = (await _matchService.GetMatchesByTournamentAndRoundAsync(id, tournament.CurrentRound))
				.Select(m => MatchMapper.ToResponse(m))
				.ToList();

			return Ok(TournamentMapper.ToDetail(tournament, players, matches));
		}

		[HttpPatch("{id}/deadline")]
		public async Task<IActionResult> UpdateDeadline(int id, UpdateDeadlineDTO dto)
		{
			await _tournamentService.UpdateDeadlineAsync(id, dto.RegistrationDeadline);
			return NoContent();
		}

		[HttpGet("{id}/winner")]
		public async Task<IActionResult> GetWinner(int id)
		{
			var (pseudo, score) = await _tournamentService.GetWinnerAsync(id);
			return Ok(new WinnerDTO { Pseudo = pseudo, Score = score });
		}

		[HttpPatch("{id}/start")]
		public async Task<IActionResult> StartTournament(int id)
		{
			await _tournamentService.StartTournamentAsync(id);
			return NoContent();
		}

		[HttpPatch("{id}/next-round")]
		public async Task<IActionResult> NextRound(int id)
		{
			await _tournamentService.NextRoundAsync(id);
			return NoContent();
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteTournament(int id)
		{
			await _tournamentService.DeleteTournamentAsync(id);
			return NoContent();
		}
	}
}
