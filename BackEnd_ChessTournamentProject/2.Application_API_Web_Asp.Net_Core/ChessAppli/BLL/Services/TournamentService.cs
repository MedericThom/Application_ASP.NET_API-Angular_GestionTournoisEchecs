using BLL.Interfaces;
using DAL.Interfaces;
using DOMAIN.Entities;

namespace BLL.Services
{
	public class TournamentService : ITournamentService
	{

		#region ConnectionS DAL
		private readonly ITournamentRepository _tournamentRepository;
		private readonly IRegistrationRepository _registrationRepository;
		private readonly IPlayerRepository _playerRepository;
		private readonly IMatchRepository _matchRepository;

		public TournamentService(ITournamentRepository tournamentRepository,
								 IRegistrationRepository registrationRepository,
								 IPlayerRepository playerRepository,
								 IMatchRepository matchRepository)
		{
			_tournamentRepository = tournamentRepository;
			_registrationRepository = registrationRepository;
			_playerRepository = playerRepository;
			_matchRepository = matchRepository;
		}
		#endregion

		public async Task CreateTournamentAsync(Tournament tournament)
		{
			if (tournament.MinNbPlayer > tournament.MaxNbPlayer)
			{
				throw new ArgumentException("Le nombre mininum de joueur doit être plus petit ou égal que le nombre maximun de joueur");
			}

			//Hashvalue => Vérifie si la valeur est nulle (propriété des types nullable)
			if (tournament.MinElo.HasValue && tournament.MaxElo.HasValue && tournament.MinElo > tournament.MaxElo)
			{
				throw new ArgumentException("L'Elo minimum doit être inférieur ou égal Elo maximun");
			}

			if (tournament.RegistrationDeadline.Date < DateTime.Today)
				throw new ArgumentException("La date de fin des inscriptions doit être supérieure ou égale à aujourd'hui.");

			tournament.CurrentRound = 0;
			tournament.StatusTournament = "En attente de joueurs";
			tournament.CreationDate = DateTime.Now;
			tournament.UpdateDate = DateTime.Now;

			await _tournamentRepository.CreateTournamentAsync(tournament);
		}

		public async Task AddCategoriesAsync(int tournamentId, List<int> categoryIds)
		{
			if (categoryIds == null || !categoryIds.Any())
				throw new ArgumentException("Un tournoi doit avoir au moins une catégorie.");

			await _tournamentRepository.AddCategoriesToTournamentAsync(tournamentId, categoryIds);
		}

		public async Task<List<Tournament>> GetTournamentsAsync()
		{
			return await _tournamentRepository.GetLastTenTournamentInProgressAsync();
		}

		public async Task<Tournament?> GetTournamentByIdAsync(int tournamentId)
		{
			return await _tournamentRepository.GetByIdAsync(tournamentId);
		}

		public async Task StartTournamentAsync(int tournamentId)
		{
			Tournament tournament = await _tournamentRepository.GetByIdAsync(tournamentId);

			if (tournament == null)
				throw new KeyNotFoundException("Tournoi introuvable");

			if (tournament.StatusTournament != "En attente de joueurs")
				throw new ArgumentException("Le tournoi a déjà commencé !");

			if (tournament.RegistrationDeadline > DateTime.Now)
				throw new ArgumentException("La date de fin des inscriptions n'est pas encore dépassée !");

			int registeredPlayers = await _registrationRepository.GetRegisteredPlayersCountAsync(tournamentId);
			if (registeredPlayers < tournament.MinNbPlayer)
				throw new ArgumentException("Le nombre minimum de participants n'est pas atteint !");

			// Trier les joueurs par Elo décroissant pour le seeding initial
			List<Player> players = await _playerRepository.GetPlayersByTournamentIdAsync(tournamentId);
			List<Player> seeded = players.OrderByDescending(p => p.Elo).ToList();

			// Générer uniquement les matchs de la ronde 1 : 1er vs 2e, 3e vs 4e, ...
			List<Match_> matches = new List<Match_>();
			for (int i = 0; i + 1 < seeded.Count; i += 2)
			{
				matches.Add(new Match_
				{
					Tournament_Id  = tournamentId,
					WhitePlayer_Id = seeded[i].Player_Id,
					BlackPlayer_Id = seeded[i + 1].Player_Id,
					RoundNumber    = 1,
					Result         = null
				});
			}

			tournament.CurrentRound = 1;
			tournament.StatusTournament = "En cours";
			tournament.UpdateDate = DateTime.Now;

			await _matchRepository.CreateMatchesAsync(matches);
			await _tournamentRepository.UpdateTournamentAsync(tournament);
		}

		public async Task NextRoundAsync(int tournamentId)
		{
			Tournament tournament = await _tournamentRepository.GetByIdAsync(tournamentId);
			if (tournament == null)
				throw new KeyNotFoundException("Tournoi introuvable");

			// 1. Vérifier que tous les matchs de la ronde courante sont terminés
			List<Match_> currentMatches = await _matchRepository.GetMatchesByTournamentAndRoundAsync(tournamentId, tournament.CurrentRound);
			if (!currentMatches.All(m => m.Result != null))
				throw new ArgumentException("Tous les matchs de la ronde courante doivent être terminés avant de passer à la ronde suivante.");

			// 2. Incrémenter la ronde
			tournament.CurrentRound++;
			tournament.UpdateDate = DateTime.Now;

			// 3. Si toutes les rondes sont jouées, clôturer le tournoi
			if (tournament.CurrentRound > tournament.MaxRounds)
			{
				tournament.StatusTournament = "Terminé";
				await _tournamentRepository.UpdateTournamentAsync(tournament);
				return;
			}

			// 4. Générer les appariements suisses pour la nouvelle ronde
			List<Registration> scores = await _registrationRepository.GetScoresByTournamentAsync(
				tournamentId, tournament.CurrentRound - 1);

			List<Match_> allMatches = await _matchRepository.GetAllMatchesByTournamentAsync(tournamentId);

			// Toutes les confrontations déjà jouées dans les deux sens
			HashSet<(int, int)> dejaAffronte = new HashSet<(int, int)>(
				allMatches.SelectMany(m => new[] {
					(m.WhitePlayer_Id, m.BlackPlayer_Id),
					(m.BlackPlayer_Id, m.WhitePlayer_Id)
				}));

			// Directions (blanc, noir) déjà utilisées pour l'attribution des couleurs
			HashSet<(int, int)> dejaJoueEnBlanc = new HashSet<(int, int)>(
				allMatches.Select(m => (m.WhitePlayer_Id, m.BlackPlayer_Id)));

			// Liste triée par score décroissant
			List<int> nonAppariei = scores.Select(r => r.Player_Id).ToList();
			List<Match_> newMatches = new List<Match_>();

			while (nonAppariei.Count >= 2)
			{
				int joueurA = nonAppariei[0];
				nonAppariei.RemoveAt(0);

				// Chercher le premier adversaire non encore affronté (les deux sens comptent)
				int indexAdversaire = nonAppariei.FindIndex(p => !dejaAffronte.Contains((joueurA, p)));

				// Cas extrême : tous les adversaires ont déjà été rencontrés
				// → répétition autorisée, prendre le joueur de score le plus proche
				if (indexAdversaire == -1)
					indexAdversaire = 0;

				int joueurB = nonAppariei[indexAdversaire];
				nonAppariei.RemoveAt(indexAdversaire);

				// Attribution des couleurs : préférer la direction non encore jouée
				bool aDejaJoueEnBlanc = dejaJoueEnBlanc.Contains((joueurA, joueurB));
				bool bDejaJoueEnBlanc = dejaJoueEnBlanc.Contains((joueurB, joueurA));

				// Si A a déjà joué blanc contre B mais pas B → B joue blanc
				// Sinon (direction libre, ou les deux déjà jouées) → A joue blanc par défaut
				int white = (aDejaJoueEnBlanc && !bDejaJoueEnBlanc) ? joueurB : joueurA;
				int black = (white == joueurA) ? joueurB : joueurA;

				newMatches.Add(new Match_
				{
					Tournament_Id  = tournamentId,
					WhitePlayer_Id = white,
					BlackPlayer_Id = black,
					RoundNumber    = tournament.CurrentRound,
					Result         = null
				});
			}

			// 5. Insérer les matchs dans une transaction, puis mettre à jour le tournoi
			if (newMatches.Any())
				await _matchRepository.CreateMatchesAsync(newMatches);

			await _tournamentRepository.UpdateTournamentAsync(tournament);
		}

		public async Task<(string Pseudo, decimal Score)> GetWinnerAsync(int tournamentId)
		{
			Tournament tournament = await _tournamentRepository.GetByIdAsync(tournamentId);
			if (tournament == null)
				throw new KeyNotFoundException("Tournoi introuvable !");

			Registration winner = await _registrationRepository.GetWinnerByTournamentAsync(tournamentId);
			if (winner == null)
				throw new KeyNotFoundException("Aucun joueur inscrit dans ce tournoi !");

			Player player = await _playerRepository.GetByPlayerIdAsync(winner.Player_Id);
			return (player.Pseudo, winner.Score);
		}

		public async Task UpdateDeadlineAsync(int tournamentId, DateTime newDeadline)
		{
			Tournament tournament = await _tournamentRepository.GetByIdAsync(tournamentId);

			if (tournament == null)
				throw new KeyNotFoundException("Tournoi introuvable !");

			if (tournament.StatusTournament != "En attente de joueurs")
				throw new ArgumentException("Impossible de modifier la date d'un tournoi qui a déjà commencé !");

			if (newDeadline.Date < DateTime.Today)
				throw new ArgumentException("La nouvelle date doit être supérieure ou égale à aujourd'hui.");

			await _tournamentRepository.UpdateDeadlineAsync(tournamentId, newDeadline);
		}

		public async Task DeleteTournamentAsync(int tournamentId)
		{
			//Récupérer le tournoi
			Tournament tournament = await _tournamentRepository.GetByIdAsync(tournamentId);

			//Check si tournoi existe
			if (tournament == null)
			{
				throw new KeyNotFoundException("Tournoi introuvable");
			}

			//Check si tournoi n'a pas commencé
			if (tournament.StatusTournament != "En attente de joueurs" && tournament.StatusTournament != "Terminé")
			{
				throw new ArgumentException("Impossible de supprimer un tournoi qui a déjà commencé !");
			}

			//Delete tournoi
			await _tournamentRepository.DeleteTournamentAsync(tournamentId);
		}
	}
}
