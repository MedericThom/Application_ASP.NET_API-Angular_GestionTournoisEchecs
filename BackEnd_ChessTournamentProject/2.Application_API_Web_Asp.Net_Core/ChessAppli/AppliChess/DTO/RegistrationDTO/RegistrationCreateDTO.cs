using System.ComponentModel.DataAnnotations;

namespace API.DTO.RegistrationDTO
{
	public class RegistrationCreateDTO
	{
		[Required]
		public int Player_Id{ get; set; }
		[Required]
		public int Tournament_Id { get; set; }
	}
}
