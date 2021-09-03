using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AgencijaProject_Data;
using Microsoft.Azure;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;

namespace Logger
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

        public override void Run()
        {
            Trace.TraceInformation("Logger is running");
            CloudQueue queue = QueueHelper.GetQueueReference("logqueue");

            while (true)
            {
                CloudQueueMessage message = queue.GetMessage();
                if (message != null)
                {
                    sacuvajUBlob(message);
                    queue.DeleteMessage(message);
                }
                Thread.Sleep(1000);
            }

        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at https://go.microsoft.com/fwlink/?LinkId=166357.

            bool result = base.OnStart();
            LoggerServer ls = new LoggerServer();
            ls.Open();
            Trace.TraceInformation("Logger has been started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("Logger is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("Logger has stopped");
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace the following with your own logic.
            while (!cancellationToken.IsCancellationRequested)
            {
                //Trace.TraceInformation("Working");
                await Task.Delay(1000);
            }
        }

        void sacuvajUBlob(CloudQueueMessage message)
        {
            var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
            CloudBlobClient blobStorage = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container =
            blobStorage.GetContainerReference("loggerblob");
            CloudBlockBlob blob = container.GetBlockBlobReference("log");
            string content = "";
            if (blob.Exists())
            {
                content = blob.DownloadText() + "|";
            }
            content = content + message.AsString;
            blob.UploadText(content);

        }


    }
}
