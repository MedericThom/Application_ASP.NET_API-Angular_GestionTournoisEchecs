using System.ComponentModel.DataAnnotations;

namespace API.DTO.PlayerDTO
{
	public class PlayerCreateDTO
	{
		[Required]
		public string Pseudo { get; set; }
		[Required]
		[EmailAddress]
		public string Email { get; set; }
		[Required]
		public string Password { get; set; }
		[Required]
		public DateTime BirthDate { get; set; }
		[Required]
		public string Gender { get; set; }
		[Range(0, 3000)]
		public int Elo { get; set; } = 1200;
	}
}
