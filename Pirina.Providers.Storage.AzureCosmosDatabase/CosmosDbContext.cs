using System;
using System.Linq;
using Pirina.Kernel.Data;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace Pirina.Providers.Storage.AzureCosmosDatabase
{
    public class CosmosDbContext : ITransitDbContext
    {
        private readonly ICosmosDbConfiguration _cosmosDbConfiguration;
        private readonly DocumentClient _client;
        private readonly Uri _databaseUri;

        public CosmosDbContext(ICosmosDbConfiguration cosmosDbConfiguration)
        {
            _cosmosDbConfiguration = cosmosDbConfiguration;           

            _client = new DocumentClient(new Uri(_cosmosDbConfiguration.EndPointUri), _cosmosDbConfiguration.PrimaryKey);

            _client.CreateDatabaseIfNotExistsAsync(new Database{Id = _cosmosDbConfiguration.DatabaseId}).Wait();

            _databaseUri = UriFactory.CreateDatabaseUri(_cosmosDbConfiguration.DatabaseId);
        }

        public T Add<T>(T item) where T : BaseTransactionModel
        {
            var collectionName = GetCollectionName<T>();

            _client.CreateDocumentCollectionIfNotExistsAsync(_databaseUri, new DocumentCollection { Id = collectionName }).Wait();

            if (IsDocumentInInCollection<T>(item))
            {
                _client.ReplaceDocumentAsync(
                    UriFactory.CreateDocumentUri(_cosmosDbConfiguration.DatabaseId, collectionName, item.TransactionId.ToString()),
                    item);
            }
            else
            {
                _client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(_cosmosDbConfiguration.DatabaseId, collectionName), item).Wait();
            }

            return item;
        }

        public bool Remove<T>(T item) where T : BaseTransactionModel
        {
            var collectionName = GetCollectionName<T>();

            if (IsDocumentInInCollection<T>(item))
            {
                _client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(_cosmosDbConfiguration.DatabaseId, collectionName, item.TransactionId.ToString()));
            }

            return true;
        }

        public IQueryable<T> Set<T>() where T : BaseTransactionModel
        {
            var collectionName = GetCollectionName<T>();

            return _client.CreateDocumentQuery<T>(UriFactory.CreateDocumentCollectionUri(_cosmosDbConfiguration.DatabaseId, collectionName));
        }

        private static string GetCollectionName<T>() where T : BaseTransactionModel
        {
            var t = typeof(T);
            var collectionName = t.FullName;

            return collectionName;
        }

        private bool IsDocumentInInCollection<T>(T item) where T : BaseTransactionModel
        {
            var result = Set<T>().Where(txm => txm.TransactionId == item.TransactionId);
            var resultInList = result.ToList();
            return resultInList.Any();
        }
    }
}