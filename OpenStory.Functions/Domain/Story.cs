using Newtonsoft.Json;
using System;

namespace Apptain.Functions.Domain
{
  public class Story
  {
    [JsonProperty(PropertyName = "id")]
    public Guid Id { get; set; }

    [JsonProperty(PropertyName = "date")]
    public DateTime Date { get; set; }

    [JsonProperty(PropertyName = "title")]
    public string Title { get; set; }

    [JsonProperty(PropertyName = "content")]
    public string Content { get; set; }
    
    [JsonProperty(PropertyName = "base64ImageString")]
    public string Base64ImageString { get; set; }

    [JsonProperty(PropertyName = "blobImagePath")]
    public string BlobImagePath { get; set; }

    [JsonProperty(PropertyName = "authorId")]
    public Guid AuthorId { get; set; }
  }
}
