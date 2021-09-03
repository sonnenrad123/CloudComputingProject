using Common;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logger
{
    class ILoggerImplemented : ILogger
    {
        public string IzvuciLogoveIzBloba()
        {
            var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
            CloudBlobClient blobStorage = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container =
            blobStorage.GetContainerReference("loggerblob");
            CloudBlockBlob blob = container.GetBlockBlobReference("log");
            string content = "";
            if (blob.Exists())
            {
                content = blob.DownloadText(); //preuzmemo celu log istoriju
            }
            string novi_logovi = "";
            novi_logovi = content.Split('*')[content.Split('*').Count() - 1]; // uzmemo samo sve iza poslednje zvezdice jer to su jedini novi podaci
            if (novi_logovi != "")
            {
                content = content + "*";//obelezimo da smo sve do zvezdice poslali
            }
            blob.UploadText(content);
            return novi_logovi;
        }
    }
}
