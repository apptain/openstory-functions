using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Apptain.Functions.Providers;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Driver;

namespace Apptain.Functions
{
  public static class StoriesGet
  {
    [FunctionName("StoriesGet")]
    public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "stories")]HttpRequestMessage req,
        TraceWriter log)
    {
      var collection = MongoProvider.Collection("stories");
      var stories = await collection.Find(new BsonDocument()).ToListAsync();
      var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict }; // key part
      var storiesJson = stories.ToJson(jsonWriterSettings);
     
      return new HttpResponseMessage(HttpStatusCode.OK)
      {
        Content = new StringContent(storiesJson, Encoding.UTF8, "application/json")
      };
    }
  }
}
