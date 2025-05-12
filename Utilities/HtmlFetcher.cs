using System.Net.Http;
using System.Threading.Tasks;

namespace DebateElo.Utilities
{
    public static class HtmlFetcher
    {
        private static readonly HttpClient client = new();

        static HtmlFetcher()
        {
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0");
        }

        public static string FetchSync(string url)
        {
            return client.GetStringAsync(url).Result;
        }

        public static async Task<string> FetchAsync(string url)
        {
            return await client.GetStringAsync(url);
        }
    }
}
