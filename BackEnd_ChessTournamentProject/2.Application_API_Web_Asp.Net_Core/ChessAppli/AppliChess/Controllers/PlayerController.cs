using API.DTO.PlayerDTO;
using API.Mappers.Players;
using BLL.Interfaces;
using DOMAIN.Entities;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class PlayerController : ControllerBase
	{
		#region Connexion à la BLL
		private readonly IPlayerService _playerService;

		public PlayerController(IPlayerService playerService)
		{
			_playerService = playerService;
		}
		#endregion

		[HttpGet]
		public async Task<IActionResult> GetAllPlayers()
		{
			List<Player> players = await _playerService.GetAllAsync();
			List<PlayerResponseDTO> response = players
				.Select(p => PlayerMapper.ToResponse(p))
				.ToList();
			return Ok(response);
		}

		[HttpPost]
		public async Task<IActionResult> CreatePlayer(PlayerCreateDTO dto)
		{
			Player player = PlayerMapper.ToEntity(dto);
			await _playerService.CreatePlayerAsync(player);
			return CreatedAtAction(nameof(CreatePlayer), PlayerMapper.ToResponse(player));
		}

		[HttpGet("{id}/stats")]
		public async Task<IActionResult> GetStats(int id)
		{
			var (trophies, victories, draws, defeats) = await _playerService.GetStatsByPlayerIdAsync(id);
			return Ok(new PlayerStatsDTO
			{
				Trophies  = trophies,
				Victories = victories,
				Draws     = draws,
				Defeats   = defeats
			});
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeletePlayer(int id)
		{
			await _playerService.DeletePlayerAsync(id);
			return NoContent();
		}

		[HttpPatch("{id}/club")]
		public async Task<IActionResult> UpdateClub(int id, UpdateClubDTO dto)
		{
			Player updated = await _playerService.UpdateClubAsync(id, dto.ChessClub_Id);
			return Ok(PlayerMapper.ToResponse(updated));
		}
	}
}
