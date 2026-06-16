using System;
using System.Collections.Generic;
using System.Text;

namespace DOMAIN.Entities
{
	public class Player
	{
		public int Player_Id { get; set; }
		public string Pseudo { get; set; }
		public string Email { get; set; }
		public string Pwd { get; set; }
		public DateTime BirthDate { get; set; }
		public string Gender { get; set; }
		public int Elo { get; set; } = 1200;
		public int? ChessClub_Id { get; set; }
	}
}
