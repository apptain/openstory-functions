using MongoDB.Bson;
using MongoDB.Driver;
using System;

using static System.Environment;

namespace Apptain.Functions.Providers
{
  public static class MongoProvider
  {
    private static string connectionString = GetEnvironmentVariable("MongoConnection", EnvironmentVariableTarget.Process);
    private static string dbName = GetEnvironmentVariable("DBName", EnvironmentVariableTarget.Process);

    private static IMongoClient client = new MongoClient(connectionString);
    private static IMongoDatabase db = client.GetDatabase(dbName);

    /// <summary>
    /// BsonDocument IMongoCollection from configured mongodb
    /// </summary>
    /// <param name="collectionName"></param>
    /// <returns>Bson Document Collection</returns>
    public static IMongoCollection<BsonDocument> Collection(string collectionName)
    {
      return db.GetCollection<BsonDocument>(collectionName);
    }

    /// <summary>
    /// Generic Typed IMongoCollection from configured mongodb
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="collectionName"></param>
    /// <returns>Generic type T</returns>
    public static IMongoCollection<T> Collection<T>(string collectionName)
    {
      return db.GetCollection<T>(collectionName);
    }
  }
}
