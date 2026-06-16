using DOMAIN.Entities;

namespace BLL.Interfaces
{
	public interface ICategoryService
	{
		public Task CreateCategoryAsync(Category category);
		Task<List<Category>> GetAllAsync();
		Task DeleteCategoryAsync(int categoryId);
	}
}
