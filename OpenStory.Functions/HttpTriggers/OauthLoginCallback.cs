using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Apptain.Functions.Domain;
using Apptain.Functions.Providers;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace Apptain.Functions.HttpTriggers.Auth.Login
{
  public static class OauthLoginCallback
  {
    [FunctionName("OauthLoginCallback")]
    public static async Task<HttpResponseMessage> Run(
      [HttpTrigger(AuthorizationLevel.Anonymous, "get",  Route = "oauth/login/callback")]HttpRequestMessage req,
      TraceWriter log
    )
    {
      try
      {
        //only works in cloud, not local, so logging may be needed to get wired up on azure cloud
        log.Info($"OAuth Login Callbacack");

        //Initializes users from mongo
        //TODO pass database with dependency injection in params azure functions style
        var collection = MongoProvider.Collection<User>("users");
        var users = await collection.Find(new BsonDocument()).ToListAsync();

        //let's log out user in into to JSON so we can take a look at him
        //TODO possible sensitive logging
        var headersJson = JsonConvert.SerializeObject(req.Headers);
        log.Info($"OAuth Mobile Log In Request Recieved With Headers {headersJson}");

        Dictionary<string, string> headersDict = req.Headers.HeadersToDictionary();

        //Get details from from headers
        string oAuthProvider = headersDict.FirstOrDefault(x => x.Key == "X-MS-CLIENT-PRINCIPAL-IDP").Value;       
        string oAuthProvidedUserId = headersDict.FirstOrDefault(x => x.Key == "X-MS-CLIENT-PRINCIPAL-ID").Value;   
        string oAuthProvidedUserHandle = headersDict.FirstOrDefault(x => x.Key == "X-MS-CLIENT-PRINCIPAL-NAME").Value;

        User user;
        if (oAuthProvider == "facebook")
        {
          user = users.FirstOrDefault(x => x.FacebookId == oAuthProvidedUserId);       
          if (user == null)
          {
            user = new User()
            {
              FacebookId = oAuthProvidedUserId,
              Handle = oAuthProvidedUserHandle,
              FacebookHandle = oAuthProvidedUserHandle
            };

            //TODO possible sensitive logging
            log.Info($"User Created :{JsonConvert.SerializeObject(user)}");
          }
        }
        else if (oAuthProvider == "twitter")
        {
          user = users.FirstOrDefault(x => x.TwitterId == oAuthProvidedUserId);
          if (user == null)
          {
            user = new User()
            {
              TwitterId = oAuthProvidedUserId,
              Handle = oAuthProvidedUserHandle,
              TwitterHandle = oAuthProvidedUserHandle
            };
          }
        }
        else
        {
          throw new Exception("Not Authenticated");
        }

        string jwToken = JwtProvider.GenerateToken(user);

        var response = req.CreateResponse(HttpStatusCode.Moved);
        response.Headers.Add("Authorization", "Bearer " + jwToken);

        string mobileRedirectUri = "openstory://login?token=" + jwToken;
        response.Headers.Location = new Uri(mobileRedirectUri);
        return response;
      }
      catch(Exception ex)
      {
        var exceptionJson = JsonConvert.SerializeObject(ex);
        log.Error($"Error Occured {exceptionJson}");
        throw ex;
      }
    }
  }
}
