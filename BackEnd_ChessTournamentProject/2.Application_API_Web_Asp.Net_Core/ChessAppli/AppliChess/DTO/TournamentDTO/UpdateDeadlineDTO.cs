using System.ComponentModel.DataAnnotations;

namespace API.DTO.TournamentDTO
{
	public class UpdateDeadlineDTO
	{
		[Required]
		public DateTime RegistrationDeadline { get; set; }
	}
}
