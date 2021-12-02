using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CosmosDbHelper
{
    public class CosmosDbClient<T>
    {
        private Container _container;

        public CosmosDbClient(CosmosClient dbClient, string databaseName, string containerName)
        {
            this._container = dbClient.GetContainer(databaseName, containerName);
        }

        public async Task AddItemAsync(T item)
        {
            try
            {
                Type myType = item.GetType();
                PropertyInfo propertyInfo = myType.GetProperty("id");
                await this._container.CreateItemAsync<T>(item, new PartitionKey(propertyInfo.GetValue(item).ToString()));
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task DeleteItemAsync(string id)
        {
            await this._container.DeleteItemAsync<T>(id, new PartitionKey(id));
        }

        public async Task<double> GetItemAsync(string id)
        {
            try
            {
                ItemResponse<T> response = await this._container.ReadItemAsync<T>("3a35937e-ce78-4eb1-95fa-689fb29eac6d", new PartitionKey("Building 30"));
                return response.RequestCharge;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return default(double);
            }

        }

        public async Task UpdateItemAsync(string id, T item)
        {
            await this._container.UpsertItemAsync<T>(item, new PartitionKey(id));
        }
    }
}

