using System.Collections.Generic;
using System.Net.Http;
using HtmlAgilityPack;

namespace DebateElo
{
    public class TournamentScraper : ITournamentScraper
    {
        private static readonly HttpClient client = new HttpClient();

        public List<string> FetchMotions(string url)
        {
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0");
            var pageContents = client.GetStringAsync(url).Result;
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(pageContents);
            var motions = new List<string>();
            var cards = htmlDoc.DocumentNode.SelectNodes("//div[contains(@class, 'card mt-3')]");

            if (cards != null)
            {
                foreach (var card in cards)
                {
                    var titleNode = card.SelectSingleNode(".//h4[contains(@class, 'card-title')]");
                    var leadNode = card.SelectSingleNode(".//div[contains(@class, 'mr-auto pr-3 lead')]");
                    string title = titleNode?.InnerText.Trim() ?? "No Title";
                    string lead = leadNode?.InnerText.Trim() ?? "No Lead";
                    motions.Add($"{title}: {lead}");
                }
            }

            return motions;
        }
    }
}