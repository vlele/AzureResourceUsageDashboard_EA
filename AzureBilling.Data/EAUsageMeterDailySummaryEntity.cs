using Microsoft.WindowsAzure.Storage.Table;

namespace AzureBilling.Data
{
    public class EAUsageMeterDailySummaryEntity : TableEntity
    {
        public EAUsageMeterDailySummaryEntity() { }

        public string MeterId { get; set; }

        public string MeterName { get; set; }

        public string MeterCategory { get; set; }

        public string RunId { get; set; }

        public string Day { get; set; }

        public double Amount { get; set; }
    }
}
