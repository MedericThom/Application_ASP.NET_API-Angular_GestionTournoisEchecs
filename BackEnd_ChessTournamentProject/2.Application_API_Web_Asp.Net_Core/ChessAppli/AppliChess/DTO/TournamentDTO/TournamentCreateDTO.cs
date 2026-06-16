using System.ComponentModel.DataAnnotations;

namespace API.DTO.TournamentDTO
{
	public class TournamentCreateDTO
	{
		[Required]
		[MinLength(3)]
		[MaxLength(50)]
		public string NameTournament { get; set; }
		[Required]
		public string Place { get; set; }
		[Required]
		[Range(2,100)]
		public int MinNbPlayer { get; set; }
		[Required]
		[Range(2,100)]
		public int MaxNbPlayer { get; set; }
		[Range(0,3000)]
		public int? MinElo { get; set; }
		[Range(0,3000)]
		public int? MaxElo { get; set; }
		public bool WomenOnly { get; set; } = false;
		[Required]
		[Range(1, 100)]
		public int MaxRounds { get; set; }
		[Required]
		public DateTime RegistrationDeadline { get; set; }
		[Required]
		[MinLength(1, ErrorMessage = "Un tournoi doit avoir au moins une catégorie.")]
		public List<int> CategoryIds { get; set; }
	}
}
