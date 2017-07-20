using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Scorpion.Data
{
    class Program
    {
        private const string EndpointUrl = "https://scorpion.documents.azure.com:443/";
        private const string PrimaryKey = "AjSOfpBcexL4OS41L4msvoptkdFpWxts4EEGEu3wv3Cq9OEsQm9JKk6grYaeRAiYxeOAzLsNko1iMo3PzwLfTA==";
        private const string DbName = "ScorpionDb";
        private const string CollectionName = "ScorpionCollection";
        private const string DocumentDirectory = @"C:\Users\dfvd\Desktop\Scorpion\DocumentDb";
        private static DocumentClient client;
        private static List<Model.Scorpion> Scorpions = new List<Model.Scorpion>();

        public static void Main(string[] args)
        {
            FileReader reader = new FileReader();
            Scorpions = reader.GetScorpionFromDirectory(DocumentDirectory);

            client = new DocumentClient(new Uri(EndpointUrl), PrimaryKey);
            Initialize(DbName).Wait();
            AddScorpionToDocumentDb().Wait();

            //ReadScorpion();
            //client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(DbName, CollectionName, "Scorpion.1")).Wait();
            //client.DeleteDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(DbName, CollectionName)).Wait();
            //client.DeleteDatabaseAsync(UriFactory.CreateDatabaseUri(DbName)).Wait();
        }

        private static void ReadScorpion()
        {
            ExecuteSimpleQuery(DbName, CollectionName);
        }

        private static void ExecuteSimpleQuery(string databaseName, string collectionName)
        {
            // Set some common query options
            FeedOptions queryOptions = new FeedOptions { MaxItemCount = -1 };

            IQueryable<Model.Scorpion> scorpionQuery = client.CreateDocumentQuery<Model.Scorpion>(
                UriFactory.CreateDocumentCollectionUri(databaseName, collectionName), queryOptions)
                .Where(s => s.TagId == "Scorpion.1");

            foreach (var scorpion in scorpionQuery)
            {
                Console.WriteLine("\tRead {0}", scorpion.TagId);
            }

            IQueryable<Model.Scorpion> scorpionQueryInSql = client.CreateDocumentQuery<Model.Scorpion>(
                UriFactory.CreateDocumentCollectionUri(databaseName, collectionName),
                "SELECT * FROM ScorpionCollection WHERE ScorpionCollection.id = 'Scorpion.2'",
                queryOptions);

            foreach (Model.Scorpion scorpion in scorpionQueryInSql)
            {
                Console.WriteLine("\tRead {0}", scorpion.TagId);
            }
        }

        private static async Task AddScorpionToDocumentDb()
        {
            foreach (var s in Scorpions)
            {
                await CreateScorpionDocumentIfNotExists(DbName, CollectionName, s);
            }
        }

        private static async Task Initialize(string dbId)
        {
            client = new DocumentClient(new Uri(EndpointUrl), PrimaryKey);
            await client.CreateDatabaseIfNotExistsAsync(new Database { Id = dbId });
            await CreateDocumentCollectionIfNotExists(DbName, CollectionName);
        }

        private static async Task CreateDocumentCollectionIfNotExists(string databaseName, string collectionName)
        {
            try
            {
                await client.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(databaseName, collectionName));
            }
            catch (DocumentClientException de)
            {
                // If the document collection does not exist, create a new collection
                if (de.StatusCode == HttpStatusCode.NotFound)
                {
                    DocumentCollection collectionInfo = new DocumentCollection();
                    collectionInfo.Id = collectionName;

                    // Optionally, you can configure the indexing policy of a collection. Here we configure collections for maximum query flexibility including string range queries. 
                    collectionInfo.IndexingPolicy = new IndexingPolicy(new RangeIndex(DataType.String) { Precision = -1 });

                    // DocumentDB collections can be reserved with throughput specified in request units/second. 1 RU is a normalized request equivalent to the read of a 1KB document.  Here we create a collection with 400 RU/s. 
                    await client.CreateDocumentCollectionAsync(
                        UriFactory.CreateDatabaseUri(databaseName),
                        new DocumentCollection { Id = collectionName },
                        new RequestOptions { OfferThroughput = 400 });
                }
                else
                {
                    throw;
                }
            }
        }

        private static async Task CreateScorpionDocumentIfNotExists(string databaseName, string collectionName, Model.Scorpion scorpion)
        {
            try
            {
                await client.ReadDocumentAsync(UriFactory.CreateDocumentUri(databaseName, collectionName, scorpion.TagId));
            }
            catch (DocumentClientException de)
            {
                if (de.StatusCode == HttpStatusCode.NotFound)
                {
                    await client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(databaseName, collectionName), scorpion);
                }
                else
                {
                    throw;
                }
            }
        }

        private static async Task ReplaceScorpion(string databaseName, string collectionName, string scorpionName, Model.Scorpion updatedScorpion)
        {
            var documentLink = UriFactory.CreateDocumentUri(databaseName, collectionName, scorpionName);
            await client.ReplaceDocumentAsync(documentLink, updatedScorpion);
        }
    }
}
