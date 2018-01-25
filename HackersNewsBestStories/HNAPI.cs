using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using HackersNewsBestStories.Models;
using System.Linq;
using System;
using System.Diagnostics;

namespace HackersNewsBestStories
{
	/// <summary>
	/// Class that handles connection and parsing of data from API.
	/// </summary>
	public class HNAPI
	{
		//The base address to be used for connection to the api.
		private const string baseAddress = "https://hacker-news.firebaseio.com/v0/";
		//End point to find the best stories from the api.
		private const string bestStoriesEndPoint = "beststories.json";
		//End point to parse items.
		private const string itemEndpoint = "item/{0}.json";
		//used to referesh cache after length of time.
		private static TimeSpan _cacheTimeSpan = TimeSpan.FromMinutes(15);
		//cache storage.
		private static Dictionary<HackerNewsDetailsViewModel, DateTime> _storyCache;

		/// <summary>
		/// Requests a list of ids for the best stories using the constants BaseAddress and BestStoriesEndPoint.
		/// </summary>
		/// <returns>List of ints of values corisponding to the best stories.</returns>
		public static async Task<List<int>> BestStories()
		{
			var content = await GetContent(baseAddress + bestStoriesEndPoint);
			return JsonConvert.DeserializeObject<List<int>>(content);
			
		}

		/// <summary>
		/// Retreives Json data from address and parses it to a details model with needed information and discards the rest.
		/// address is comprised of BaseAddress and ItemEndPoint
		/// Data is cashed and pulled from cash if it is younger than the _cacheTimeSpan.
		/// </summary>
		/// <param name="id">Id of the item to fetch</param>
		/// <returns>HackerNewsDetailsViewModel use to present data to views</returns>
		public static async Task<HackerNewsDetailsViewModel> Item(int id)
		{
			if (_storyCache != null && _storyCache.Keys.Any(s => s.id == id))
			{
				var story = _storyCache.Keys.First(s => s.id == id);
				Debug.WriteLine(string.Format("Story: {0} | CachedAt: {1}", story.id, _storyCache[story]));
				if (story != null && DateTime.Now < _storyCache[story] + _cacheTimeSpan)
				{
					story.cached = true;
					return story;
				}
			}
			
			var content = await GetContent(baseAddress + string.Format(itemEndpoint, id));
			var result = JsonConvert.DeserializeObject<HackerNewsDetailsViewModel>(content);

			if (_storyCache == null)
				_storyCache = new Dictionary<HackerNewsDetailsViewModel, DateTime>();

			if(_storyCache.Keys.Any(s=>s.id == id))
			{
				var story = _storyCache.Keys.First(s => s.id == id);
				_storyCache.Remove(story);
			}

			_storyCache.Add(result, DateTime.Now);
			return result;
		}

		//Get content from api as string and returns it for parsing.
		private static async Task<string> GetContent(string uri)
		{
			using (var client = new HttpClient())
			{
				using (var response = await client.GetAsync(uri))
				{
					response.EnsureSuccessStatusCode();

					return await response.Content.ReadAsStringAsync();
				}
			}
		}
	}
}