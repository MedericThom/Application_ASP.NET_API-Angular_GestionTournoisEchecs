using DOMAIN.Entities;

namespace DAL.Interfaces
{
	public interface ICategoryRepository
	{
		public Task CreateCategoryAsync(Category category);
		public Task<List<Category>> GetCategoriesByTournamentIdAsync(int tournamentId);
		Task<List<Category>> GetAllAsync();
		Task<bool> IsUsedByTournamentsAsync(int categoryId);
		Task DeleteCategoryAsync(int categoryId);
	}
}
