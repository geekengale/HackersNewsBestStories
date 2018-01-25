using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HackersNewsBestStories.Models
{
	public class HackerNewsDetailsViewModel
	{
		public int id;
		public string by;
		public string title;
		public bool cached = false;
	}
}