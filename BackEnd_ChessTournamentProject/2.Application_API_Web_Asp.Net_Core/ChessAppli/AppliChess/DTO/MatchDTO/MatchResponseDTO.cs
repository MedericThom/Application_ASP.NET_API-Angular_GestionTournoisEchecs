namespace API.DTO.MatchDTO
{
	public class MatchResponseDTO
	{
		public int Match_Id { get; set; }
		public int RoundNumber { get; set; }
		public int? Result { get; set; }
		public int Tournament_Id { get; set; }
		public int WhitePlayer_Id { get; set; }
		public int BlackPlayer_Id { get; set; }
	}
}
