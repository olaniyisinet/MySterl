

using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace GIVE_WebAPI.Models
{
	public class ProjectsModel
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public string Category { get; set; }
		public string TargetAmount { get; set; }
		public string Duration { get; set; }
		public string Image { get; set; }
		public HttpPostedFileBase File { get; set; }
		public string Status { get; set; }
		public string EndDate { get; set; }
	public	IList<SelectListItem> Categories { get; set; }
	}
}