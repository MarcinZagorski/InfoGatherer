using HtmlAgilityPack;
using InfoGatherer.api.Data.Entities.Scrapper;
using InfoGatherer.api.Data.Repositories.Interfaces;
using InfoGatherer.api.Services.Interfaces;
using System.Globalization;

namespace InfoGatherer.api.Services
{
    public class WiborMoneyScarpperService(IWiborRepository wrepo) : IWiborScrapperService
    {
        private readonly IWiborRepository _wrepo = wrepo;
        public async Task<string> ScrapeAndSaveDataAsync()
        {
            string response;
            var url = "https://wibor.money.pl";
            var httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(url);
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            var table = htmlDocument.DocumentNode.SelectSingleNode("//table[contains(@class, 'tabela big m0 tlo_biel')]");
            var wibor = new Wibor
            {
                Id = Guid.NewGuid(),
                Source = "https://wibor.money.pl"
            };
            if (table != null)
            {
                foreach (var row in table.SelectNodes(".//tr").Skip(1)) 
                {
                    var cells = row.SelectNodes("td").ToList();
                    if (cells.Count >= 6)
                    {
                        var name = cells[0].InnerText.Trim();
                        var valueWithPercentage = cells[1].InnerText.Trim();
                        var value = decimal.Parse(valueWithPercentage.Replace("%", "").Replace(",", "."), CultureInfo.InvariantCulture);
                        wibor.Date = DateTime.Parse(cells[5].InnerText.Trim());

                        switch (name)
                        {
                            case "WIBOR ON":
                                wibor.Overnight = value;
                                break;
                            case "WIBOR TN":
                                wibor.TomorrowNext = value;
                                break;
                            case "WIBOR 1W":
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
                            case "WIBOR 1Y":
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
