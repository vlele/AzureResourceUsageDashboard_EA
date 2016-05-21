using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace AzureBilling.Data
{
    public class EAUsageDetailEntity : TableEntity
    {
        public EAUsageDetailEntity() { }

        public EAUsageDetailEntity(string month)
        {
            this.PartitionKey = month;
            this.RowKey = Guid.NewGuid().ToString();
        }

        public string RunId { get; set; }

        public string AccountOwnerLiveId { get; set; }

        public string AccountName { get; set; }

        public string ServiceAdministratorLiveId { get; set; }

        public string SubscriptionId { get; set; }

        public string MOCPSubscriptionGuid { get; set; }

        public string SubscriptionName { get; set; }

        public string Date { get; set; }

        public string Month { get; set; }

        public string Day { get; set; }

        public string Year { get; set; }

        public string BillableItemName { get; set; }

        public string ResourceGUID { get; set; }

        public string Service { get; set; }

        public string ServiceType { get; set; }

        public string ServiceRegion { get; set; }

        public string ServiceResource { get; set; }

        public string ResourceQtyConsumed { get; set; }

        public string ResourceRate { get; set; }

        public string ExtendedCost { get; set; }

        public string ServiceSubRegion { get; set; }

        public string ServiceInfo { get; set; }

        public string Component { get; set; }

        public string ServiceInfo1 { get; set; }

        public string ServiceInfo2 { get; set; }

        public string AdditionalInfo { get; set; }

        public string Tags { get; set; }

        public string OrderNumber { get; set; }

        public string DepartmentName { get; set; }

        public string UnitOfMeasure { get; set; }

        public string CostCenter { get; set; }

        public string ResourceGroup { get; set; }

        public string MeterId { get; set; }

        public string MeterCategory { get; set; }

        public string MeterSubCategory { get; set; }

        public string MeterName { get; set; }

        public string InstanceId { get; set; }
    }
}
