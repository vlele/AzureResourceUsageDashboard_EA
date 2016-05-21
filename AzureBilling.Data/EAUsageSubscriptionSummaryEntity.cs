using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureBilling.Data
{
    public class EAUsageSubscriptionSummaryEntity : TableEntity
    {
        public EAUsageSubscriptionSummaryEntity() { }

        public string SubscriptionName { get; set; }
        public string SubscriptionId { get; set; }
        public string RunId { get; set; }
        public double Amount { get; set; }
    }
}
