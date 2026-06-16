namespace API.DTO.RegistrationDTO
{
	public class ScoreDTO
	{
		public string Pseudo { get; set; }
		public int Wins { get; set; } 
		public int Losses { get; set; }
		public int Draws { get; set; }
		public decimal Score { get; set; }
		public int MatchesPlayed { get; set; }
	}
}
