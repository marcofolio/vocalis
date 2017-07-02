using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace VocalisBot.Cognitive
{
    public static class BingImageService
    {
       public static async Task<BingImageResult> GetBestImageAsync(string query)
        {
            var client = new HttpClient();

            // Request headers  
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", Constants.BING_API_KEY);

            // Request parameters  
            string count = "1";
            string offset = "0";
            string market = "en-us";

            // Execute request
            var result = await client.GetAsync(string.Format("{0}q={1}&count={2}&offset={3}&mkt={4}", Constants.BING_IMAGE_API_ROOT, WebUtility.UrlEncode(query), count, offset, market));
            result.EnsureSuccessStatusCode();

            // Parse and read result
            var json = await result.Content.ReadAsStringAsync();
            dynamic data = JObject.Parse(json);

            return new BingImageResult()
            {
                Name = data.value[0].name,
                ContentUrl = data.value[0].contentUrl,
                WebSearchUrl = data.value[0].webSearchUrl
            };
        }
    }
}