using System.ComponentModel.DataAnnotations;

namespace API.DTO.MatchDTO
{
	public class MatchUpdateDTO
	{
		[Required]
		[Range(0,2)]
		public int Result { get; set; }
	}
}
