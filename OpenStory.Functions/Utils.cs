using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;

namespace Apptain.Functions
{
  public static class Utils
  {

    /// <summary>
    /// Converts http request headers to dictionary of headerName/headerValue
    /// </summary>
    /// <param name="requestHeaders"></param>
    /// <returns></returns>
    public static Dictionary<string, string> HeadersToDictionary(this HttpRequestHeaders requestHeaders)
    {
      Dictionary<string, string> dictionary = new Dictionary<string, string>() { };
      foreach (var header in requestHeaders)
      {
        dictionary.Add(header.Key, header.Value.FirstOrDefault());
      }
      return dictionary;
    }
  }
}
