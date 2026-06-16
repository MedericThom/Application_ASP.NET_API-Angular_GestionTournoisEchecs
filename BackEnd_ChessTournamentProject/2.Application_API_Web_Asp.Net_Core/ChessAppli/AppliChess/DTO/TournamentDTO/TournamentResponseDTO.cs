namespace API.DTO.TournamentDTO
{
	public class TournamentResponseDTO
	{
		public int Tournament_Id { get; set; }
		public string NameTournament { get; set; }
		public string Place { get; set; }
		public int MinNbPlayer { get; set; }
		public int MaxNbPlayer { get; set; }
		public int? MinElo { get; set; }
		public int? MaxElo { get; set; }
		public string StatusTournament { get; set; }
		public int CurrentRound { get; set; } = 0;
		public bool WomenOnly { get; set; } = false;
		public int MaxRounds { get; set; }
		public DateTime RegistrationDeadline { get; set; }
		public DateTime CreationDate { get; set; }
		public DateTime UpdateDate { get; set; }
		public int PlayerCount { get; set; }
	}
}
