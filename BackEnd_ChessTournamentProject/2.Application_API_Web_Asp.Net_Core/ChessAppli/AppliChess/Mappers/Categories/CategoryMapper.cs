using API.DTO.CategoryDTO;
using DOMAIN.Entities;

namespace API.Mappers.Categories
{
	public static class CategoryMapper
	{
		public static Category ToEntity(CategoryCreateDTO dto)
		{
			return new Category
			{
				NameCategory = dto.NameCategory,
				MinAge = dto.MinAge,
				MaxAge = dto.MaxAge
			};
		}
		public static CategoryResponseDTO ToResponse(Category category)
		{
			return new CategoryResponseDTO
			{
				Category_Id = category.Category_Id,
				NameCategory = category.NameCategory,
				MinAge = category.MinAge,
				MaxAge = category.MaxAge
			};
		}
	}
}
