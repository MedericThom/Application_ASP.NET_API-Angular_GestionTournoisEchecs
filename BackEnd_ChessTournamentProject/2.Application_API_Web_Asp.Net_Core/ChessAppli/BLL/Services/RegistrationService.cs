using BLL.Interfaces;
using DAL.Interfaces;
using DOMAIN.Entities;

namespace BLL.Services
{
	public class RegistrationService : IRegistrationService
	{
		#region Connexion à la DAL
		private readonly IRegistrationRepository _registrationRepository;
		private readonly ITournamentRepository _tournamentRepository;
		private readonly IPlayerRepository _playerRepository;
		private readonly ICategoryRepository _categoryRepository;
		public RegistrationService(IRegistrationRepository registrationRepository,
			                       ITournamentRepository tournamentRepository,
								   IPlayerRepository playerRepository,
								   ICategoryRepository categoryRepository)
		{
			_registrationRepository = registrationRepository;
			_tournamentRepository = tournamentRepository;
			_playerRepository = playerRepository;
			_categoryRepository = categoryRepository;
		}
		#endregion

		public async Task RegisterPlayerAsync(Registration registration)
		{
			//1.Récupérer tournoi
			Tournament tournament = await _tournamentRepository.GetByIdAsync(registration.Tournament_Id);
			if (tournament == null)
			{
				throw new KeyNotFoundException("Tournoi introuvable !");
			}

			//2.Récupérer joueur
			Player player = await _playerRepository.GetByPlayerIdAsync(registration.Player_Id);
			if (player == null)
			{
				throw new KeyNotFoundException("Joueur introuvable !");
			}

			//3.Vérifier que le tournoi n'a pas encore commencé
			if (tournament.StatusTournament != "En attente de joueurs")
			{
				throw new ArgumentException("Le tournoi a déjà commencé");
			}

			//4.Vérifier que la date de fin des inscriptions n'est pas dépassée
			if (DateTime.Now > tournament.RegistrationDeadline)
			{
				throw new ArgumentException("La date de fin des inscriptions est dépassée");
			}

			//5.Vérifier que le joueur n'est pas déjà inscrit
			if (await _registrationRepository.IsPlayerRegisteredAsync(registration.Player_Id, registration.Tournament_Id))
			{
				throw new ArgumentException("Le joueur est déjà inscrit !");
			}

			//6.Vérifier que le tournoi n'a pas atteint son nombre maximum
			int registeredPlayers = await _registrationRepository.GetRegisteredPlayersCountAsync(registration.Tournament_Id);

			if (registeredPlayers >= tournament.MaxNbPlayer)
			{
				throw new ArgumentException("Le tournoi a atteint son nombre maximum de joueurs !");
			}

			//7.Vérifier que l'âge du joueur correspond à une catégorie autorisée
			int age = tournament.RegistrationDeadline.Year - player.BirthDate.Year;
			//Correction si l'anniversaire n'est pas encore passé
			if (player.BirthDate.Date > tournament.RegistrationDeadline.AddYears(-age))
			{
				age--;
			}

			//Récupérer les catégories
			List<Category> categories = await _categoryRepository.GetCategoriesByTournamentIdAsync(registration.Tournament_Id);

			if (!categories.Any())
				throw new ArgumentException("Ce tournoi n'a aucune catégorie configurée.");

			//Vérifier l'âge
			if (!categories.Any(c => age >= c.MinAge && age <= c.MaxAge))
				throw new ArgumentException("L'âge du joueur ne correspond à aucune catégorie autorisée !");

			//8. Vérifier que l'ELO du joueur respecte les bornes
			if (tournament.MinElo.HasValue && player.Elo < tournament.MinElo)
			{
				throw new ArgumentException("L'ELO du joueur est inférieur à l'ELO minimum du tournoi !");
			}

			if (tournament.MaxElo.HasValue && player.Elo > tournament.MaxElo)
			{
				throw new ArgumentException("L'ELO du joueur est supérieur à l'ELO maximum du tournoi !");
			}

			// 9. Vérifier le genre
			if (tournament.WomenOnly && player.Gender != "Female")
			{
				throw new ArgumentException("Ce tournoi est réservé aux femmes !");
			}

			//10. Inscrire le joueur
			registration.RegistrationDate = DateTime.Now;
			await _registrationRepository.CreateRegistrationAsync(registration);
		}

		public async Task<List<Registration>> GetScoresAsync(int tournamentId, int round)
		{
			return await _registrationRepository.GetScoresByTournamentAsync(tournamentId, round);
		}

		public async Task UnregisterPlayerAsync(int playerId, int tournamentId)
		{
			//1.Récupérer tournoi et vérifier s'il existe
			Tournament tournament = await _tournamentRepository.GetByIdAsync(tournamentId);

			if (tournament == null)
			{
				throw new KeyNotFoundException("Tournoi introuvable !");
			}

			//3.Vérifier que le tournoi n'a pas encore commencé
			if (tournament.StatusTournament != "En attente de joueurs")
			{
				throw new ArgumentException("Le tournoi a déjà commencé");
			}

			//2.Vérifier que le joueur est inscrit
			if (!await _registrationRepository.IsPlayerRegisteredAsync(playerId, tournamentId))
			{
				throw new ArgumentException("Le joueur n'est pas inscrit !");
			}

			await _registrationRepository.DeleteRegistrationAsync(playerId, tournamentId);
		}
	}
}
