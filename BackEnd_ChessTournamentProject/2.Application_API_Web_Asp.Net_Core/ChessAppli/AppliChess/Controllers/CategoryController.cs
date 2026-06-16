using API.DTO.CategoryDTO;
using API.Mappers.Categories;
using BLL.Interfaces;
using DOMAIN.Entities;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class CategoryController : ControllerBase
	{
		#region Connexion à la DAL
		private readonly ICategoryService _categoryService;
		public CategoryController(ICategoryService categoryService)
		{
			_categoryService = categoryService;
		}
		#endregion

		[HttpGet]
		public async Task<IActionResult> GetAllCategories()
		{
			List<Category> categories = await _categoryService.GetAllAsync();
			return Ok(categories.Select(c => CategoryMapper.ToResponse(c)).ToList());
		}

		[HttpPost]
		public async Task<IActionResult> CreateCategory(CategoryCreateDTO dto)
		{
			Category category = CategoryMapper.ToEntity(dto);
			await _categoryService.CreateCategoryAsync(category);
			return CreatedAtAction(nameof(CreateCategory), CategoryMapper.ToResponse(category));
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteCategory(int id)
		{
			await _categoryService.DeleteCategoryAsync(id);
			return NoContent();
		}
	}

	#region Comments
	//=> return CreatedAtAction(nameof(CreateCategory), category); <=
	//Retoune => un HTTP 201 Created (via ControllerBase)
	//AVEC    => les données créées
	//ET      => l'URL où trouver la ressource

	//CreatedAtAction
	//→ Méthode de ControllerBase
	//→ Retourne HTTP 201 Created automatiquement !

	//nameof(CreateCategory)
	//→ Le nom de l'action qui permet de retrouver la ressource
	//→ nameof() = retourne le nom en string
	//→ nameof(CreateCategory) = "CreateCategory"
	//→ C'est plus safe que d'écrire "CreateCategory" en dur !

	//category
	//→ L'objet créé qu'on retourne au client
	//→ Il sera sérialisé en JSON automatiquement !

	//Résumé
	//CreatedAtAction  →  HTTP 201 + objet créé en JSON
	//nameof()         →  nom de l'action en string (safe)
	//category         →  l'objet retourné au client
	#endregion
}
