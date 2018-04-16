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
using System.Text;

namespace Apptain.Functions
{
  public static class ProfileGet
  {
    [FunctionName("ProfileGet")]
    public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "profile")]HttpRequestMessage req,
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

      return new HttpResponseMessage(HttpStatusCode.OK)
      {
        Content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json")
      };

    }
  }
}
