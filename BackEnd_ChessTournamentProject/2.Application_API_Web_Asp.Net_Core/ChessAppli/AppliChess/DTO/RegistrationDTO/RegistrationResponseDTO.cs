namespace API.DTO.RegistrationDTO
{
	public class RegistrationResponseDTO
	{
		public int Registration_Id { get; set; }
		public int Player_Id { get; set; }
		public int Tournament_Id { get; set; }
		public int Wins { get; set; }
		public int Losses { get; set; }
		public int Draws { get; set; }
		public decimal Score { get; set; }
		public int MatchesPlayed { get; set; }
		public DateTime RegistrationDate { get; set; }
	}
}
