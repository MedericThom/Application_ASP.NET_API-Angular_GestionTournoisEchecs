using DOMAIN.Entities;

namespace BLL.Interfaces
{
	public interface ITournamentService
	{
		public Task CreateTournamentAsync(Tournament tournament);
		public Task AddCategoriesAsync(int tournamentId, List<int> categoryIds);
		public Task<List<Tournament>> GetTournamentsAsync();
		public Task<Tournament?> GetTournamentByIdAsync(int tournamentId);
		public Task StartTournamentAsync(int tournamentId);
		public Task NextRoundAsync(int tournamentId);
		public Task UpdateDeadlineAsync(int tournamentId, DateTime newDeadline);
		public Task<(string Pseudo, decimal Score)> GetWinnerAsync(int tournamentId);
		public Task DeleteTournamentAsync(int tournamentId);
	}
}

// CONSTRUCTION méthodes BLL :
// 1. "Qu'est ce que cette méthode FAIT ?"
// 2. "De quoi a-t-elle BESOIN ?"
// 3. "Est ce qu'elle RETOURNE quelque chose ?"


// QUESTIONS A EVITER [👉 Construis tes interfaces BLL comme si la DAL n'existait pas ! ] :
//❌ "Est ce que la DAL a cette méthode ?"
//❌ "Comment la DAL va récupérer ça ?"
//❌ "Est ce que la DAL retourne le bon type ?"

// MAIS PLUTOT
//✅ "Qu'est ce que mon Controller a besoin ?"
//✅ "Quelles règles métier dois-je appliquer ?"
//✅ "Qu'est ce que je retourne au Controller ?"
