namespace API.DTO.PlayerDTO
{
	public class PlayerResponseDTO
	{
		public int Player_Id { get; set; }
		public string Pseudo { get; set; }
		public string Email { get; set; }
		public DateTime BirthDate { get; set; }
		public string Gender { get; set; }
		public int Elo { get; set; } = 1200;
		public int? ChessClub_Id { get; set; }
	}
}
