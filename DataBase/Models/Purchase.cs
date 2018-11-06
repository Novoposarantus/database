using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DataBase.Models
{
	public class Purchase
	{
		public int Id { get; set; }
		public DateTime Date { get; set; }
		public List<Product> Products { get; set; }
	}
}
