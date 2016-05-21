using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage;
//using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table.DataServices;
using System.Data.SqlClient;
using Newtonsoft.Json.Linq;
using AzureBilling.Data;

namespace AzureBilling.WebJob
{
    /// <summary>
    /// This executable calls EAUsage Controller to update month to date usage and billing data for Enterprise Agreement (EA) subscriptions
    /// It's execution is scheduled as a web job (in AzureBilling.Web project) but can be executed standalone nevertheless.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            string month = DateTime.UtcNow.Month < 10 ? "0" + DateTime.UtcNow.Month.ToString() : DateTime.UtcNow.Month.ToString();
            string year = DateTime.UtcNow.Year.ToString();

            //EA uses YYYY-MM format
            string baseUrl = System.Configuration.ConfigurationManager.AppSettings["API-URL"];
            string requesturl = String.Format("{0}/EaUsage/GetUsageData?yyyy={1}&mm={2}",
               baseUrl, year, month);

            //Build Request
            DateTime startTime = DateTime.UtcNow;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requesturl);

            // Read Response
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Console.WriteLine(response.StatusDescription);
            StreamReader reader = new StreamReader(response.GetResponseStream());
            var jsonString = reader.ReadToEnd();
            DateTime endTime = DateTime.UtcNow;

            //Get Run Id
            JToken outer = JToken.Parse(jsonString);
            string runId = (string)outer["RunId"];

            // Insert RunId after each successful run
            EntityRepo<EAWebJobRunInfo> repo = new EntityRepo<EAWebJobRunInfo>();
            repo.Insert(new List<EAWebJobRunInfo>() {
                new EAWebJobRunInfo {
                    PartitionKey = year+"-"+month,
                    RowKey = runId,
                    RunId = runId,
                    StartTimeUTC = startTime,
                    EndTimeUTC = endTime
                }
            });
        }
    }
}
