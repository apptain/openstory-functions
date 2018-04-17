using System;

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using static System.Environment;

namespace Apptain.Functions.Providers
{
  public static class AzureBlobProvider
  {
    private static string _connectionString = GetEnvironmentVariable("AzureWebJobsStorage", EnvironmentVariableTarget.Process);

    private static CloudStorageAccount GetConnection()
    {
      CloudStorageAccount storageAccount = CloudStorageAccount.Parse(_connectionString);
      return storageAccount;
    }

    private static CloudBlobContainer GetContainer(string containerName)
    {
      CloudStorageAccount storageAccount = GetConnection();
      CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
      CloudBlobContainer container = blobClient.GetContainerReference(containerName);
      container.CreateIfNotExists();

      return container;
    }

    /// <summary>
    /// Uploaded Jpeg to configured Azure Storage
    /// </summary>
    /// <param name="imageData"></param>
    /// <param name="fileName"></param>
    /// <returns>Azure Storage Uri</returns>
    public static string UploadJpeg(string imageData, string fileName)
    {
      CloudBlobContainer container = GetContainer("media");
      byte[] imageBytes = Convert.FromBase64String(imageData);
      CloudBlockBlob blob = container.GetBlockBlobReference(fileName);
      blob.Properties.ContentType = "Jpeg";
      blob.UploadFromByteArray(imageBytes, 0, imageBytes.Length);
      return blob.StorageUri.PrimaryUri.AbsoluteUri;
    }
  }
}
