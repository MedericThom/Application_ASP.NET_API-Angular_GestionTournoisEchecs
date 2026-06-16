using BLL.Interfaces;
using DAL.Interfaces;
using DOMAIN.Entities;

namespace BLL.Services
{
	public class PlayerService : IPlayerService
	{
		#region Connexion à la DAL
		private readonly IPlayerRepository _playerRepository;
		public PlayerService(IPlayerRepository PlayerRepository)
		{
			_playerRepository = PlayerRepository;
		}
		#endregion

		public async Task CreatePlayerAsync(Player player)
		{
			if (player.ChessClub_Id == 0) player.ChessClub_Id = null;

			Player existingEmail = await _playerRepository.GetByEmailAsync(player.Email);

			if (existingEmail != null)
			{
				throw new ArgumentException("Cette adresse e-mail est déjà utilisé !");
			}

			Player existingPseudo = await _playerRepository.GetByPseudoAsync(player.Pseudo);

			if (existingPseudo != null)
			{
				throw new ArgumentException("Ce pseudo est déjà utilisé !");
			}

			// Hacher le mot de passe
			player.Pwd = BCrypt.Net.BCrypt.HashPassword(player.Pwd);

			// Vérifier un mot de passe (pour le login plus tard)
			//bool isValid = BCrypt.Net.BCrypt.Verify(player.Pwd, hashedPassword);

			if (player.Elo == 0)
			{
				player.Elo = 1200;
			}

			await _playerRepository.CreatePlayerAsync(player);
		}

		public async Task<Player> GetByPlayerIdAsync(int playerId)
		{
			return await _playerRepository.GetByPlayerIdAsync(playerId);
		}
		public async Task<List<Player>> GetPlayersByTournamentIdAsync(int tournamentId)
		{
			return await _playerRepository.GetPlayersByTournamentIdAsync(tournamentId);
		}

		public async Task<List<Player>> GetAllAsync()
		{
			return await _playerRepository.GetAllAsync();
		}

		public async Task DeletePlayerAsync(int playerId)
		{
			Player player = await _playerRepository.GetByPlayerIdAsync(playerId);
			if (player is null)
				throw new KeyNotFoundException($"Joueur {playerId} introuvable.");

			await _playerRepository.DeletePlayerAsync(playerId);
		}

		public async Task<(int Trophies, int Victories, int Draws, int Defeats)> GetStatsByPlayerIdAsync(int playerId)
		{
			Player player = await _playerRepository.GetByPlayerIdAsync(playerId);
			if (player is null)
				throw new KeyNotFoundException($"Joueur {playerId} introuvable.");

			return await _playerRepository.GetStatsByPlayerIdAsync(playerId);
		}

		public async Task<Player> UpdateClubAsync(int playerId, int? chessClubId)
		{
			Player player = await _playerRepository.GetByPlayerIdAsync(playerId);
			if (player is null)
				throw new KeyNotFoundException($"Joueur {playerId} introuvable.");

			return await _playerRepository.UpdateClubAsync(playerId, chessClubId);
		}
	}
}
