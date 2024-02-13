namespace InfoGatherer.api.DTOs.Scrappers.Wibor
{
    public class WiborDto
    {
        public DateTime Date { get; set; }
        public string Source { get; set; }

        public decimal Overnight { get; set; } // "ON"
        public decimal TomorrowNext { get; set; } // "TN"
        public decimal SpotWeek { get; set; } // "SW"
        public decimal OneMonth { get; set; } // "1M"
        public decimal ThreeMonths { get; set; } // "3M"
        public decimal SixMonths { get; set; } // "6M"
        public decimal OneYear { get; set; } // "1R"
    }
}
