using BLL.Interfaces;
using DAL.Interfaces;
using DOMAIN.Entities;

namespace BLL.Services
{
	public class CategoryService : ICategoryService
	{
		#region Connexion à la DAL
		private readonly ICategoryRepository _categoryRepository;

		public CategoryService(ICategoryRepository categoryRepository)
		{
			_categoryRepository = categoryRepository;
		} 
		#endregion

		public async Task CreateCategoryAsync(Category category)
		{
			if (string.IsNullOrEmpty(category.NameCategory))
			{
				throw new ArgumentException("Indiquez un nom de categorie");
			}

			if (category.MinAge < 0)
			{
				throw new ArgumentException("L'âge minimum doit être supérieur ou égal à 0");
			}

			if (category.MaxAge > 110)
			{
				throw new ArgumentException("L'âge maximum doit être inférieur ou égal à 110");

			}

			if (category.MinAge > category.MaxAge)
			{
				throw new ArgumentException("L'âge minimum doit être inférieur ou égal à l'âge maximum");

			}

			await _categoryRepository.CreateCategoryAsync(category);
		}
		public async Task<List<Category>> GetAllAsync()
		{
			return await _categoryRepository.GetAllAsync();
		}

		public async Task DeleteCategoryAsync(int categoryId)
		{
			bool isUsed = await _categoryRepository.IsUsedByTournamentsAsync(categoryId);
			if (isUsed)
				throw new InvalidOperationException("Impossible de supprimer cette catégorie : elle est utilisée par un ou plusieurs tournois non terminés.");

			await _categoryRepository.DeleteCategoryAsync(categoryId);
		}
	}
}
