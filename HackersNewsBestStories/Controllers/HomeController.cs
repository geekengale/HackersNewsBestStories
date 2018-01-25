using HackersNewsBestStories.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Threading.Tasks;
using System;

namespace HackersNewsBestStories.Controllers
{
	public class HomeController : Controller
	{
		/// <summary>
		/// Creates a list of best stories' titles and authors and returns it to the view to be displayed.
		/// </summary>
		public async Task<ActionResult> Index(int page = 1)
		{
			var bestStoryIds = await HNAPI.BestStories();
			ViewData["pages"] = bestStoryIds.Count / 10;

			var storyItems = new List<HackerNewsDetailsViewModel>();

			foreach (var id in bestStoryIds.Skip(10 * (page - 1)).Take(10))
			{
				storyItems.Add(await HNAPI.Item(id));
			}
			
			return View(storyItems);
		}

		/// <summary>
		/// Creates a list of stories based on the supplied data and simple searches.
		/// </summary>
		/// <param name="searchText">The text to preform search with.</param>
		/// <param name="searchOption">The data to preform the search on.</param>
		[HttpPost]
		public async Task<ActionResult> SearchResults(string searchText, int searchOption)
		{
			var bestStoryIds = await HNAPI.BestStories();
			var storyItems = new List<HackerNewsDetailsViewModel>();
			var result = new SearchResultViewModel()
			{
				SearchOption = searchOption,
				SearchText = searchText
			};

			if (string.IsNullOrWhiteSpace(searchText))
			{
				ViewData["SearchMessage"] = "Please enter a term to search.";
				return View(result);
			}

			//Search on the title, author, or ID.
			switch (searchOption)
			{
				default:
					foreach (var id in bestStoryIds)
					{
						var storyItem = await HNAPI.Item(id);
						if (storyItem.title.ToLower().Contains(searchText.ToLower()))
							storyItems.Add(storyItem);
					}
					break;
				case 1:
					foreach (var id in bestStoryIds)
					{
						var storyItem = await HNAPI.Item(id);
						if(storyItem.by.ToLower().Contains(searchText.ToLower()))
							storyItems.Add(storyItem);
					}
					break;
				case 2:
					var storyID = 0;
					if (Int32.TryParse(searchText, out storyID) && bestStoryIds.Contains(storyID))
						storyItems.Add(await HNAPI.Item(storyID));
					else
					{
						ViewData["SearchMessage"] = "No Results found. Please be sure to enter a number when searching by ID.";
						return View(result);
					}
					break;
			}

			if(storyItems == null || storyItems.Count < 1)
			{
				ViewData["SearchMessage"] = "No Results found";
			}

			result.Results = storyItems;
			return View(result);
		}
	}
}
