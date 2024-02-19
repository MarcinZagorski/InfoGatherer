using HtmlAgilityPack;
using InfoGatherer.api.Data.Entities.Scrapper;
using InfoGatherer.api.Data.Repositories.Interfaces;
using InfoGatherer.api.Services.Interfaces;
using System.Globalization;

namespace InfoGatherer.api.Services
{
    public class WiborBankierScarpperService(IWiborRepository wrepo) : IWiborScrapperService
    {
        private readonly IWiborRepository _wrepo = wrepo;

        public async Task<string> ScrapeAndSaveDataAsync()
        {
            string response;
            var url = "https://www.bankier.pl/mieszkaniowe/stopy-procentowe/wibor";
            var httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(url);
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);
            var table = htmlDocument.DocumentNode.SelectSingleNode("//table[@class='summaryTable']");
            var wibor = new Wibor
            {
                Id = Guid.NewGuid(),
                Source = "https://www.bankier.pl/mieszkaniowe/stopy-procentowe/wibor"
            };
            if (table != null)
            {
                foreach (var row in table.SelectNodes(".//tr"))
                {
                    var cells = row.SelectNodes("td").ToList();
                    if (cells.Count >= 2)
                    {
                        var name = cells[0].InnerText.Trim();
                        if (name == "Data")
                        {
                            var dateString = cells[1].InnerText.Trim();
                            if (DateTime.TryParse(dateString, out var date))
                            {
                                wibor.Date = date;
                            }
                            continue;
                        }
                        var valueWithPercentage = cells[1].InnerText.Split('\n')[1].Trim();
                        var value = decimal.Parse(valueWithPercentage.Replace("%", "").Replace(",", "."), CultureInfo.InvariantCulture);

                        switch (name)
                        {
                            case "WIBOR ON":
                                wibor.Overnight = value;
                                break;
                            case "WIBOR TN":
                                wibor.TomorrowNext = value;
                                break;
                            case "WIBOR SW":
                                wibor.SpotWeek = value;
                                break;
                            case "WIBOR 1M":
                                wibor.OneMonth = value;
                                break;
                            case "WIBOR 3M":
                                wibor.ThreeMonths = value;
                                break;
                            case "WIBOR 6M":
                                wibor.SixMonths = value;
                                break;
                            case "WIBOR 1R":
                                wibor.OneYear = value;
                                break;
                        }
                    }
                }
                response = await _wrepo.AddOrUpdate(wibor);
            }
            else
            {
                response = "Unable to scrap data";
                //TODO send email to me when unable to scarp
            }
            return response;
        }
    }
}
