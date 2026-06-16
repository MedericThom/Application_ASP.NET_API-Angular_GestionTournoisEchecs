using BLL.Interfaces;
using DAL.Interfaces;
using DOMAIN.Entities;

namespace BLL.Services
{
	public class MatchService : IMatchService
	{
		#region Connexion à la DAL
		private readonly IMatchRepository _matchRepository;
		private readonly ITournamentRepository _tournamentRepository;

		public MatchService(IMatchRepository matchRepository,
			                ITournamentRepository tournamentRepository)
		{
			_matchRepository = matchRepository;
			_tournamentRepository = tournamentRepository;
		}
		#endregion

		public async Task<List<Match_>> GetMatchesByTournamentAndRoundAsync(int tournamentId, int round)
		{
			return await _matchRepository.GetMatchesByTournamentAndRoundAsync(tournamentId, round);
		}

		public async Task<List<Match_>> GetAllMatchesByTournamentAsync(int tournamentId)
		{
			return await _matchRepository.GetAllMatchesByTournamentAsync(tournamentId);
		}

		public async Task UpdateMatchAsync(int matchId, int result)
		{
			//1.Récupérer le match et checker s'il existe
			Match_ match = await _matchRepository.GetMatchByIdAsync(matchId);

			if (match == null)
			{
				throw new KeyNotFoundException("Match introuvable !");
			}

			//2.Récupérer le tournoi associé
			Tournament tournament = await _tournamentRepository.GetByIdAsync(match.Tournament_Id);

			//3.Vérifier que le match appartient à la ronde courante
			if (match.RoundNumber != tournament.CurrentRound)
				throw new ArgumentException("Seuls les matchs de la ronde courante peuvent être modifiés !");

			//Mettre match à jour
			match.Result = result;

			//Send to DAL
			await _matchRepository.UpdateMatchAsync(match);
		}
	}
}
