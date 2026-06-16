using API.DTO.RegistrationDTO;
using API.Mappers.Registrations;
using BLL.Interfaces;
using DOMAIN.Entities;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class RegistrationController : ControllerBase
	{
		#region Connexion à la BLL
		private readonly IRegistrationService _registrationService;
		private readonly IPlayerService _playerService;
		public RegistrationController(IRegistrationService registrationService, IPlayerService playerService)
		{
			_registrationService = registrationService;
			_playerService = playerService;
		}
		#endregion

		[HttpPost]
		public async Task<IActionResult> RegisterPlayer(RegistrationCreateDTO dto)
		{
			Registration registration = RegistrationMapper.ToEntity(dto);
			await _registrationService.RegisterPlayerAsync(registration);
			return CreatedAtAction(nameof(RegisterPlayer), RegistrationMapper.ToResponse(registration));
		}

		[HttpGet("{tournamentId}/{round}")]
		public async Task<IActionResult> GetScores(int tournamentId, int round)
		{
			List<Registration> scoresTournament = await _registrationService.GetScoresAsync(tournamentId, round);

			List<ScoreDTO> response = new List<ScoreDTO>();
			foreach (Registration st in scoresTournament)
			{
				Player player = await _playerService.GetByPlayerIdAsync(st.Player_Id);
				response.Add(RegistrationMapper.ToScore(st, player));
			}

			return Ok(response);
		}

		[HttpDelete("{playerId}/{tournamentId}")]
		public async Task<IActionResult> UnregisterPlayer(int playerId, int tournamentId)
		{
			await _registrationService.UnregisterPlayerAsync(playerId, tournamentId);
			return NoContent();
		}
	}
}
