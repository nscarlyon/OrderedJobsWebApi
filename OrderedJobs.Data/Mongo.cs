using MongoDB.Driver;
using System.Collections.Generic;
using MongoDB.Bson;
using System;
using MongoDB.Bson.Serialization.Attributes;

namespace OrderedJobs.Data
{
    public interface IMongoDatabase
    {
        IEnumerable<TestCase> GetAllJobs();
        void DeleteTestCases();
        void InsertTestCase(TestCase value);
    }

    public class MongoDatabase : IMongoDatabase
    {
         IMongoCollection<TestCase> collection;

        public MongoDatabase()
        {
            IMongoClient client = new MongoClient("mongodb://127.0.0.1:27017");
            collection = client.GetDatabase("OrderedJobs").GetCollection<TestCase>("Tests");
        }

        public IEnumerable<TestCase> GetAllJobs()
        {
            return collection.Find(x => true).ToListAsync().Result;
        }

        public void DeleteTestCases()
        {
            collection.DeleteMany(Test => true);
        }

        public void InsertTestCase(TestCase value)
        {
            collection.InsertOne(value);
        }
    }

    public class TestCase
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Dependency { get; set; }
    }
}

