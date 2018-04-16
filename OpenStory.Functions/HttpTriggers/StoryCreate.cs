using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Apptain.Functions.Domain;
using Newtonsoft.Json;
using Apptain.Functions.Providers;
using MongoDB.Bson;
using System.Text;
using System.Security.Cryptography;

namespace Apptain.Functions
{
  public static class StoryCreate
  {
    public static byte[] GetHash(string inputString)
    {
      HashAlgorithm algorithm = MD5.Create();  //or use SHA256.Create();
      return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
    }

    public static string GetHashString(string inputString)
    {
      StringBuilder sb = new StringBuilder();
      foreach (byte b in GetHash(inputString))
        sb.Append(b.ToString("X2"));

      return sb.ToString();
    }

    [FunctionName("StoryCreate")]
      public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "story")]HttpRequestMessage req,
        TraceWriter log)
      {

      string jwt = req.Headers.Authorization.ToString().Replace("Bearer ", "");

      if (String.IsNullOrEmpty(jwt))
      {
        throw new UnauthorizedAccessException();
      }
      User user = JwtProvider.ClaimsUser(jwt);
      if (user == null)
      {
        throw new UnauthorizedAccessException();
      }

      Task<string> content = req.Content.ReadAsStringAsync();
      string body = content.Result;
      if (body != "")
      {
        Story story = JsonConvert.DeserializeObject<Story>(body);
        story.Id = Guid.NewGuid();
        story.Date = DateTime.Now;

        string imageName = $"{story.Id}/{story.Title}.jpg";

        story.BlobImagePath = AzureBlobProvider.UploadJpeg(story.Base64ImageString, imageName);
        story.Base64ImageString = "";

        story.AuthorId = user.Id;

        var document = story.ToBsonDocument();
          await MongoProvider.Collection("stories").InsertOneAsync(document);

        //TODO really assuming success here, need checks and should return db updated doc
        var storyJson = JsonConvert.SerializeObject(story);

        return new HttpResponseMessage(HttpStatusCode.OK)
        {
          Content = new StringContent(storyJson, Encoding.UTF8, "application/json")
        };
      }
        else
        {
          return req.CreateResponse(HttpStatusCode.BadRequest);
        }
      }
    }
}
