using System.Collections.Generic;
using System.Net.Http;
using HtmlAgilityPack;

namespace DebateElo.Scrapers
{
    public class TournamentScraper : ITournamentScraper
    {
        private static readonly HttpClient client = new HttpClient();

        public List<(string title, string lead)> FetchMotions(string url)
        {
            var fullUrl = url.TrimEnd('/') + "/motions/";

            var request = new HttpRequestMessage(HttpMethod.Get, fullUrl);
            request.Headers.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 Chrome/90.0.4430.93 Safari/537.36");
            request.Headers.Accept.ParseAdd("text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
            request.Headers.AcceptLanguage.ParseAdd("en-US,en;q=0.9");
            request.Headers.Connection.ParseAdd("keep-alive");

            var response = client.SendAsync(request).Result;
            response.EnsureSuccessStatusCode();

            var pageContents = response.Content.ReadAsStringAsync().Result;

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(pageContents);
            var motions = new List<(string, string)>();

            var cards = htmlDoc.DocumentNode.SelectNodes("//div[contains(@class, 'card mt-3')]");
            if (cards != null)
            {
                foreach (var card in cards)
                {
                    var titleNode = card.SelectSingleNode(".//h4[contains(@class, 'card-title')]");
                    var leadNode = card.SelectSingleNode(".//div[contains(@class, 'mr-auto pr-3 lead')]");
                    string title = titleNode?.InnerText.Trim() ?? "No Title";
                    string lead = leadNode?.InnerText.Trim() ?? "No Lead";
                    motions.Add((title, lead));
                }
            }

            return motions;
        }

    }
}
