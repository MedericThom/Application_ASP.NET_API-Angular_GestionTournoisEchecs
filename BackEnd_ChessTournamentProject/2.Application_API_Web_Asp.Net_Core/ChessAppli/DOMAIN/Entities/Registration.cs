using System;
using System.Collections.Generic;
using System.Text;

namespace DOMAIN.Entities
{
	public class Registration
	{
		public int Registration_Id { get; set; }
		public int Player_Id { get; set; }
		public int Tournament_Id { get; set; }
		public int Wins { get; set; } = 0;
		public int Losses { get; set; } = 0;
		public int Draws { get; set; } = 0;
		public decimal Score { get; set; } = 0;
		public int MatchesPlayed { get; set; } = 0;
		public DateTime RegistrationDate { get; set; } = DateTime.Now;
	}
}
