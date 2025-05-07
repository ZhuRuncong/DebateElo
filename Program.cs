using DebateElo.Scrapers;
using Newtonsoft.Json.Linq;

namespace DebateElo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            var vueScraper = new VueDataScraper();
            string url = "https://hhhs2025.calicotab.com/hhhs2025/tab/team/";
            JObject vueData = vueScraper.ExtractVueData(url);
            Console.WriteLine(vueData.ToString());
        }
    }
}
