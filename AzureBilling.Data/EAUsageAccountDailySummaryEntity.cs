using Microsoft.WindowsAzure.Storage.Table;

namespace AzureBilling.Data
{
    public class EAUsageAccountDailySummaryEntity : TableEntity
    {
        public EAUsageAccountDailySummaryEntity() { }

        public string AccountName { get; set; }

        public string RunId { get; set; }

        public string Day { get; set; }

        public double Amount { get; set; }
    }
}
