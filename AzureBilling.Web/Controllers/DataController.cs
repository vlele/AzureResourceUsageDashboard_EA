using AzureBilling.Data;
using AzureBilling.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace AzureBilling.Web.Controllers
{
    /// <summary>
    /// Returns persisted data for UI
    /// </summary>
    public class DataController : Controller
    {
        /// <summary>
        /// Get Azure expense by subscription (summary)
        /// </summary>
        /// <param name="monthId">month in YYYY-mm format</param>
        /// <returns>expenses for a given month in JSON format</returns>
        public JsonResult SpendingBySubscription(string monthId="")
        {

            if (string.IsNullOrEmpty(monthId))
            {
                monthId = GetMonthId();
            }

            var repo = new EntityRepo<EAUsageSubscriptionSummaryEntity>();
            var data = repo.Get(monthId, new List<Tuple<string, string>> { });
            var array =  data.Select(p => new { name = p.SubscriptionName, y = p.Amount });
            return Json(array.ToList(), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get Azure expense by account (summary)
        /// </summary>
        /// <param name="monthId">month in YYYY-mm format</param>
        /// <returns>expenses for a given month in JSON format</returns>
        public JsonResult SpendingByAccount(string monthId="")
        {
            if (string.IsNullOrEmpty(monthId))
            {
                monthId = GetMonthId();
            }
            var repo = new EntityRepo<EAUsageAccountSummaryEntity>();
            var data = repo.Get(monthId, new List<Tuple<string, string>> { });
            var array = data.Select(p => new { name = p.AccountName, y = p.Amount  });
            return Json(array.ToList(), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get Azure expense by azure services (summary)
        /// </summary>
        /// <param name="monthId">month in YYYY-mm format</param>
        /// <returns>expenses for a given month in JSON format</returns>
        public JsonResult SpendingByService(string monthId = "")
        {
            if (string.IsNullOrEmpty(monthId))
            {
                monthId = GetMonthId();
            }
            var repo = new EntityRepo<EAUsageMeterSummaryEntity>();
            var data = repo.Get(monthId, new List<Tuple<string, string>> { });

            var aggregateUsage = from us in data
                                 group us by new
                                 {
                                     MeterCategory = us.MeterCategory,
                                 }
                            into fus
                                 select new
                                 {
                                     y = fus.Sum(x => x.Amount),
                                     name = fus.Key.MeterCategory,
                                 };
            return Json(aggregateUsage.ToList(), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get Azure expense by azure services (daily)
        /// </summary>
        /// <param name="monthId">month in YYYY-mm format</param>
        /// <returns>expenses for a given month in JSON format</returns>
        public JsonResult SpendingByServiceDaily(string monthId = "")
        {
            var dictionary = new Dictionary<string, Dictionary<string,double>>();
            var uniqueDates = new Dictionary<string,string>();
            if (string.IsNullOrEmpty(monthId))
            {
                monthId = GetMonthId();
            }
            var repo = new EntityRepo<EAUsageMeterDailySummaryEntity>();
            var data = repo.Get(monthId, new List<Tuple<string, string>> { });

            var aggregateUsage = data.OrderBy(x=>x.Day).Select(p => new DailyBillInfo{ Amount = p.Amount, Name = p.MeterCategory, Date = p.Day });
            return GetDailyBillSeries(aggregateUsage);
        }

        /// <summary>
        /// Get Azure expense by subscription (daily)
        /// </summary>
        /// <param name="monthId">month in YYYY-mm format</param>
        /// <returns>expenses for a given month in JSON format</returns>
        public JsonResult SpendingBySubscriptionDaily(string monthId = "")
        {
            if (string.IsNullOrEmpty(monthId))
            {
                monthId = GetMonthId();
            }
            var repo = new EntityRepo<EAUsageSubscriptionDailySummaryEntity>();
            var data = repo.Get(monthId, new List<Tuple<string, string>> { });

            var aggregateUsage = data.OrderBy(x => x.Day).Select(p => new DailyBillInfo{ Amount = p.Amount, Name = p.SubscriptionName, Date = p.Day });
            return GetDailyBillSeries(aggregateUsage);
        }

        /// <summary>
        /// Get Azure expense by account (daily)
        /// </summary>
        /// <param name="monthId">month in YYYY-mm format</param>
        /// <returns>expenses for a given month in JSON format</returns>
        public JsonResult SpendingByAccountDaily(string monthId = "")
        {

            if (string.IsNullOrEmpty(monthId))
            {
                monthId = GetMonthId();
            }
            var repo = new EntityRepo<EAUsageAccountDailySummaryEntity>();
            var data = repo.Get(monthId, new List<Tuple<string, string>> { });

            var aggregateUsage = data.OrderBy(x => x.Day).Select(p => new DailyBillInfo { Amount = p.Amount, Name = p.AccountName, Date = p.Day });
            return GetDailyBillSeries(aggregateUsage);
        }

        private JsonResult GetDailyBillSeries(IEnumerable<DailyBillInfo> aggregateUsage)
        {
            // dictionary to hold daily expense for a given category
            // e.g. 'Subscription1' => [{'2016-03-01',18.00},{'2016-01-02',18.00}]
            //      'Subscription2' => [{'2016-03-01',12.00},{'2016-01-02',9.00}]
            var dictionary = new Dictionary<string, Dictionary<string, double>>();

            // dictionary to hold uniques dates for a given set of data
            var uniqueDates = new Dictionary<string, string>();
            foreach (var item in aggregateUsage)
            {
                var dateKey = GetDateKey(item.Date);

                //if date is not in dictionary add it in uniqueDates dictionary
                if (!uniqueDates.ContainsKey(dateKey))
                {
                    uniqueDates.Add(dateKey, dateKey);
                }

                // if the dictionary already has the given category
                if (dictionary.ContainsKey(item.Name))
                {
                    // get the day wise list
                    var doubleList = dictionary[item.Name];

                    // if the day wise list contain the given date, update the amount
                    if (doubleList.ContainsKey(dateKey))
                    {
                        doubleList[dateKey] = item.Amount;
                    }
                    else
                    {
                        // date is not present, add the amount against the given date
                        doubleList.Add(dateKey, item.Amount);
                    }
                }
                else // if this is the first time the category data is obtained
                {
                    // create a date-wise list
                    var doubleList = new Dictionary<string, double>();

                    // add entry to the date-wise list with given date
                    doubleList.Add(dateKey, item.Amount);

                    // add date-wise list against the category
                    dictionary.Add(item.Name, doubleList);
                }
            }

            // refined list of datewise list
            // if there are some dates for which we do not have data
            // we add an amount 0, for the consistency
            var finalDictionary = new Dictionary<string, List<double>>();
            
            //populate the entries with date with no data to '0'
            foreach (var categories in dictionary.Keys)
            {
                var dateWiseDictionary = dictionary[categories];
                foreach (var date in uniqueDates.Keys)
                {
                    if (!dateWiseDictionary.ContainsKey(date))
                    {
                        dateWiseDictionary.Add(date, 0.0);
                    }
                }

                // once data is populated sort it based on the date.
                finalDictionary.Add(categories, dateWiseDictionary.OrderBy(p => p.Key).Select(p => p.Value).ToList());
            }
            return Json(new { date = uniqueDates.Keys.OrderBy(p => p), series = finalDictionary.Select(p => new { name = p.Key, data = p.Value }).ToList() }, JsonRequestBehavior.AllowGet);
        }

        private string GetDateKey(string date)
        {
            return date; // can create custom key if needed
        }

        private static string GetMonthId()
        {
            string monthId;
            string month = DateTime.UtcNow.Month < 10 ? "0" + DateTime.UtcNow.Month.ToString() : DateTime.UtcNow.Month.ToString();
            string year = DateTime.UtcNow.Year.ToString();
            monthId = string.Format("{0}-{1}", year, month);
            return monthId;
        }
    }
}