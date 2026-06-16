using System.ComponentModel.DataAnnotations;

namespace API.DTO.CategoryDTO
{
	public class CategoryCreateDTO
	{
		[Required]
		public string NameCategory { get; set; }

		[Required]
		[Range(0, 110)]
		public int MinAge { get; set; }

		[Required]
		[Range(0, 110)]
		public int MaxAge { get; set; }
	}
}
