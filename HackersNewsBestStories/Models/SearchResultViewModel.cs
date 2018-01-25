using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HackersNewsBestStories.Models
{
	public class SearchResultViewModel
	{
		public string SearchText;
		public int SearchOption;
		public List<HackerNewsDetailsViewModel> Results;
	}
}