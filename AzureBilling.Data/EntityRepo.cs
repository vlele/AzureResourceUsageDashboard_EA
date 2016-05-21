using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace AzureBilling.Data
{
    /// <summary>
    /// Base class for all billing and usage related Azure Table Entity 
    /// </summary>
    /// <typeparam name="T">Any Azure Table Entity</typeparam>
    public class EntityRepo<T> where T : TableEntity, new()
    {
        private CloudTableClient tableClient = null;
        private CloudTable table = null;

        public EntityRepo()
        {
            //initialize storage account from given connection string
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);
            tableClient = storageAccount.CreateCloudTableClient();

            //azure table name is same as name of the entity
            table = tableClient.GetTableReference(typeof(T).Name);
            table.CreateIfNotExists();
        }

        /// <summary>
        /// Insert azure table entities
        /// </summary>
        /// <param name="enities"></param>
        public void Insert(List<T> enities)
        {
            foreach (var item in enities)
            {
                var entityItem = item as TableEntity;
                TableOperation insertOperation = TableOperation.InsertOrReplace(entityItem);
                table.Execute(insertOperation);
            }

        }


        /// <summary>
        /// Delete a given entity from azure table
        /// </summary>
        /// <param name="entity"></param>
        public void Delete(T entity)
        {
            var entityItem = entity as TableEntity;
            string partitionKeyFilter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, entityItem.PartitionKey);
            string rowKeyFilter = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, entityItem.RowKey);
            string aggregate = TableQuery.CombineFilters(partitionKeyFilter, "and", rowKeyFilter);
            var existingPartitionData = new TableQuery().Where(aggregate);
            var item = table.ExecuteQuery(existingPartitionData).FirstOrDefault();
            if(item != null)
            {
                TableOperation deleteOperation = TableOperation.Delete(item);
                table.Execute(deleteOperation);
            }

        }

        /// <summary>
        /// Get an table entity from given criteria
        /// </summary>
        /// <param name="partitionKey">partition key; required</param>
        /// <param name="keyValuePair">search criteria key value based, e.g. query such as Property1 eq 'hello world' can be sent as Tuple ("Property1","hello world")</string></param>
        /// <param name="rowKey">optional, if rowkey is known, search will be faster</param>
        /// <returns></returns>
        public IList<T> Get(string partitionKey,List<Tuple<string,string>> keyValuePair, string rowKey="")
        {
            string partitionKeyFilter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey);
            List<string> criteria = new List<string>();
            foreach(var item in keyValuePair)
            {

                string criterian = TableQuery.CombineFilters(item.Item1, QueryComparisons.Equal, "'"+item.Item2+ "'");
                criteria.Add(criterian);
            }
            string aggregate = partitionKeyFilter;
            if (criteria.Count > 0)
            {
                foreach (var criterian in criteria)
                {
                    aggregate = TableQuery.CombineFilters(partitionKeyFilter, "and", criterian);
                }
            }
            if (!string.IsNullOrEmpty(rowKey))
            {
                string rowKeyFilter = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, rowKey);
                aggregate = TableQuery.CombineFilters(aggregate, "and", rowKeyFilter);
            }
           // aggregate = TableQuery.CombineFilters(aggregate, "and", partitionKeyFilter);
            var existingPartitionData = new TableQuery<T>().Where(aggregate);
            return table.ExecuteQuery(existingPartitionData).ToList();
            
        }

    }
}
