using GIVE_Admin.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace GIVE_Admin.Models
{
	public class NGOModel
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string YearFound { get; set; }
		public string Motto { get; set; }
		public string Email { get; set; }
		public string Phone { get; set; }
		public string Address { get; set; }
		public string About { get; set; }
		public string Projects { get; set; }
		public string Bank { get; set; }
		public string AccountNumber { get; set; }
		public string Category { get; set; }
		public string ImagePath { get; set; }
		public IList<SelectListItem> Categories { get; set; }
		public IList<SelectListItem> AllProjects { get; set; }
		public IList<SelectListItem> AllBanks { get; set; }
		public string[] SelectedProjects { get; set; }
	}
}