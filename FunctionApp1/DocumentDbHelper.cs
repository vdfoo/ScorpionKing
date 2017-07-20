using Microsoft.Azure.Documents.Client;
using System;
using System.Linq;

namespace Scorpion.Function
{
    public class DocumentDbHelper
    {
        private const string EndpointUrl = "https://scorpion.documents.azure.com:443/";
        private const string PrimaryKey = "AjSOfpBcexL4OS41L4msvoptkdFpWxts4EEGEu3wv3Cq9OEsQm9JKk6grYaeRAiYxeOAzLsNko1iMo3PzwLfTA==";
        private const string DbName = "ScorpionDb";
        private const string CollectionName = "ScorpionCollection";
        private static DocumentClient client;

        public DocumentDbHelper()
        {
            client = new DocumentClient(new Uri(EndpointUrl), PrimaryKey);
        }

        public Model.Scorpion SearchByTag(string tag)
        {
            Model.Scorpion result = new Model.Scorpion();

            FeedOptions queryOptions = new FeedOptions { MaxItemCount = -1 };

            // Query using SQL
            IQueryable<Model.Scorpion> scorpionQueryInSql = client.CreateDocumentQuery<Model.Scorpion>(
                UriFactory.CreateDocumentCollectionUri(DbName, CollectionName),
                $"SELECT * FROM ScorpionCollection WHERE ScorpionCollection.id = '{tag}'",
                queryOptions);

            foreach (var s in scorpionQueryInSql)
            {
                result = s;
                break;
            }

            // Query using LINQ
            /*
            IQueryable<Model.Scorpion> scorpionFromLinq = client.CreateDocumentQuery<Model.Scorpion>(
                UriFactory.CreateDocumentCollectionUri(DbName, CollectionName), queryOptions)
                .Where(s => s.TagId == tag);
                //.Where(s => s.Deadly.StartsWith("Yes"));

            foreach (var s in scorpionFromLinq)
            {
                result = s;
                break;
            }*/

            return result;
        }

    }
}
