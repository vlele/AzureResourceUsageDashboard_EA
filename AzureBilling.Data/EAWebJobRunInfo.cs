using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureBilling.Data
{
    public class EAWebJobRunInfo : TableEntity
    {
        public EAWebJobRunInfo() { }

        /// <summary>
        /// 
        /// </summary>
        public string RunId { get; set; }

        public DateTime StartTimeUTC { get; set; }

        public DateTime EndTimeUTC { get; set; }
    }
}
