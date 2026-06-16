using DOMAIN.Entities;

namespace DAL.Interfaces
{
	public interface IRegistrationRepository
	{
		public Task CreateRegistrationAsync(Registration registration); //J'ai besoin de TOUTES les données de l'inscription d'OU le registration
		public Task<bool> IsPlayerRegisteredAsync(int playerId, int tournamentId); //QuestionASePoser:"Pour vérifier si un joueur est inscrit, de quoi ai-je besoin?" réponse :"Je dois savoir QUEL joueur ET DANS QUEL tournoi ! ET ce dont j'ai besoin c'est quoi ? => int playerId ET int tournamentId"
		public Task<int> GetRegisteredPlayersCountAsync(int tournamentId); //QuestionASePoser:"Pour compter les joueurs inscrits, de quoi ai-je besoin?" réponse :"Je dois savoir DANS QUEL tournoi je compte ! ET ce dont j'ai besoin c'est quoi ? => int tournamentId"
		public Task<List<Registration>> GetScoresByTournamentAsync(int tournamentId, int round);
		public Task<Registration?> GetWinnerByTournamentAsync(int tournamentId);
		public Task DeleteRegistrationAsync(int playerId, int tournamentId); //QuestionASePoser:"Pour supprimer une inscription, de quoi ai-je besoin?" réponse :"Je dois savoir QUEL joueur ET DANS QUEL tournoi ! ET ce dont j'ai besoin c'est quoi ? => int playerId ET int tournamentId"
	}
}

//COMMENT trouver les paramètres d'une méthode => "De quoi ma méthode a besoin pour faire son travail ?"
//Les paramètres = Les info dont la méthode a besoin pour ne pas travailler dans le vide !
