using API.DTO.ChessClubDTO;
using API.Mappers.ChessClubs;
using BLL.Interfaces;
using DOMAIN.Entities;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class ChessClubController : ControllerBase
	{
		#region Connexion à la BLL
		private readonly IChessClubService _chessClubService;
		public ChessClubController(IChessClubService chessClubService)
		{
			_chessClubService = chessClubService;
		}
		#endregion

		[HttpGet]
		public async Task<IActionResult> GetAllChessClubs()
		{
			List<ChessClub> chessClubs = await _chessClubService.GetAllAsync();
			return Ok(chessClubs.Select(c => ChessClubMapper.ToResponse(c)).ToList());
		}

		[HttpPost]
		public async Task<IActionResult> CreateChessClub(ChessClubCreateDTO dto)
		{
			ChessClub chessClub = ChessClubMapper.ToEntity(dto);
			await _chessClubService.CreateChessClubAsync(chessClub);
			return CreatedAtAction(nameof(CreateChessClub), ChessClubMapper.ToResponse(chessClub));
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteChessClub(int id)
		{
			await _chessClubService.DeleteChessClubAsync(id);
			return NoContent();
		}
	}
}

#region Comments
//Tu CRÉES quelque chose        →  POST
//Tu RÉCUPÈRES quelque chose    →  GET
//Tu MODIFIES tout un objet     →  PUT
//Tu MODIFIES une partie        →  PATCH
//Tu SUPPRIMES quelque chose    →  DELETE
#endregion
