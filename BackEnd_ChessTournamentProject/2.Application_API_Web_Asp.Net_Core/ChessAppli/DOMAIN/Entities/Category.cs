using System;
using System.Collections.Generic;
using System.Text;

namespace DOMAIN.Entities
{
	public class Category
	{
		public int Category_Id { get; set; }
		public string NameCategory { get; set; }	
		public int MinAge { get; set; }	
		public int MaxAge { get; set; }	
	}
}
