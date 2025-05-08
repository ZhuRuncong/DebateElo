using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net;
using HtmlAgilityPack;

namespace DebateElo.Scrapers
{
    public class MotionScraper : IMotionScraper
    {
        private static readonly HttpClient client = new HttpClient();

        public List<(string title, string lead)> FetchMotions(string url)
        {
            try
            {
                return TryFetchFromMotions(url.TrimEnd('/') + "/motions/");
            }
            catch (HttpRequestException)
            {
                Console.WriteLine("403 or failed /motions, falling back to /motions/statistics...");
                return TryFetchFromStatistics(url.TrimEnd('/') + "/motions/statistics/");
            }
        }

        private List<(string, string)> TryFetchFromMotions(string motionsUrl)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, motionsUrl);
            request.Headers.UserAgent.ParseAdd("Mozilla/5.0");
            request.Headers.Accept.ParseAdd("text/html");
            request.Headers.AcceptLanguage.ParseAdd("en-US,en;q=0.9");

            var response = client.SendAsync(request).Result;
            response.EnsureSuccessStatusCode();

            var html = response.Content.ReadAsStringAsync().Result;
            return ParseFromMotionsHtml(html);
        }

        private List<(string, string)> TryFetchFromStatistics(string statisticsUrl)
        {
            var response = client.GetAsync(statisticsUrl).Result;
            response.EnsureSuccessStatusCode();

            var html = response.Content.ReadAsStringAsync().Result;
            return ParseFromStatisticsHtml(html);
        }

        private List<(string, string)> ParseFromMotionsHtml(string html)
        {
            var motions = new List<(string, string)>();
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            var cards = htmlDoc.DocumentNode.SelectNodes("//div[contains(@class, 'card mt-3')]");
            if (cards == null) return motions;

            foreach (var card in cards)
            {
                var roundTitleNode = card.SelectSingleNode(".//h4[contains(@class, 'card-title')]");
                string roundLabel = NormalizeText(WebUtility.HtmlDecode(roundTitleNode?.InnerText ?? "Unknown Round"));

                var motionItems = card.SelectNodes(".//li[contains(@class, 'list-group-item')]");
                if (motionItems == null) continue;

                foreach (var motionNode in motionItems)
                {
                    var leadNode = motionNode.SelectSingleNode(".//div[contains(@class, 'lead')]");
                    string motionText = WebUtility.HtmlDecode(leadNode?.InnerText ?? "No Lead").Trim();
                    motionText = NormalizeText(motionText);

                    string infoText = "";
                    string? modalId = motionNode.SelectSingleNode(".//*[@data-target and contains(@data-target, '#info_')]")
                        ?.GetAttributeValue("data-target", string.Empty);

                    if (!string.IsNullOrEmpty(modalId))
                    {
                        var infoId = modalId.TrimStart('#');
                        var modalNode = htmlDoc.DocumentNode.SelectSingleNode($"//div[@id='{infoId}']//div[@class='modal-body lead']");
                        if (modalNode != null)
                        {
                            var rawInfo = WebUtility.HtmlDecode(modalNode.InnerText.Trim());
                            infoText = " " + NormalizeText(rawInfo);
                        }
                    }

                    if (!motionText.EndsWith(".")) motionText += ".";
                    motions.Add((roundLabel, motionText + infoText));
                }
            }

            return motions;
        }

        private List<(string, string)> ParseFromStatisticsHtml(string html)
        {
            var motions = new List<(string, string)>();
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            var sections = htmlDoc.DocumentNode.SelectNodes("//div[contains(@class, 'list-group mt-3')]");
            if (sections == null) return motions;

            foreach (var section in sections)
            {
                var roundLabelNode = section.SelectSingleNode(".//div[contains(@class, 'list-group-item disabled')]");
                string roundLabel = NormalizeText(WebUtility.HtmlDecode(roundLabelNode?.InnerText ?? "Unknown Round"));

                var motionCards = section.SelectNodes(".//div[contains(@class, 'list-group-item px-1')]");
                if (motionCards == null) continue;

                foreach (var card in motionCards)
                {
                    var titleNode = card.SelectSingleNode(".//h4");
                    var leadNode = card.SelectSingleNode(".//div[contains(@class, 'lead')]");

                    string motionText = WebUtility.HtmlDecode(titleNode?.InnerText ?? "No Title").Trim();
                    string lead = WebUtility.HtmlDecode(leadNode?.InnerText ?? "").Trim();

                    string combinedMotion = !string.IsNullOrWhiteSpace(lead) ? lead : motionText;
                    combinedMotion = NormalizeText(combinedMotion);

                    string infoText = "";
                    string? modalId = card.SelectSingleNode(".//*[@data-target and contains(@data-target, '#info_')]")
                        ?.GetAttributeValue("data-target", string.Empty);

                    if (!string.IsNullOrEmpty(modalId))
                    {
                        var infoId = modalId.TrimStart('#');
                        var modalNode = htmlDoc.DocumentNode.SelectSingleNode($"//div[@id='{infoId}']//div[@class='modal-body lead']");
                        if (modalNode != null)
                        {
                            var rawInfo = WebUtility.HtmlDecode(modalNode.InnerText.Trim());
                            infoText = " " + NormalizeText(rawInfo);
                        }
                    }

                    if (!combinedMotion.EndsWith(".")) combinedMotion += ".";
                    motions.Add((roundLabel, combinedMotion + infoText));
                }
            }

            return motions;
        }

        private static string NormalizeText(string text)
        {
            return string.Join(" ", text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)).Trim();
        }
    }
}
