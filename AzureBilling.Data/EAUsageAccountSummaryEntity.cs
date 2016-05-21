using Microsoft.WindowsAzure.Storage.Table;

namespace AzureBilling.Data
{
    public class EAUsageAccountSummaryEntity : TableEntity
    {
        public EAUsageAccountSummaryEntity() { }

        public string AccountName { get; set; }

        public string RunId { get; set; }

        public double Amount { get; set; }
    }
}
