using AgencijaWeb.Models;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencijaProject_Data
{
    public class AgencijaDataRepository
    {
        private CloudStorageAccount _storageAccount;
        private CloudTable _table;

        public AgencijaDataRepository()
        {
            _storageAccount =
           CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
            CloudTableClient tableClient = new CloudTableClient(new
           Uri(_storageAccount.TableEndpoint.AbsoluteUri), _storageAccount.Credentials);
            _table = tableClient.GetTableReference("AgencijaProjectTable");
            _table.CreateIfNotExists();

        }
        public IQueryable<Agencija> RetrieveAllAgencies()
        {
            var results = from g in _table.CreateQuery<Agencija>()
                          where g.PartitionKey == "Agencija"
                          select g;
            return results;
        }

        public IQueryable<Korisnik> RetrieveAllKorisnik()
        {
            var results = from g in _table.CreateQuery<Korisnik>()
                          where g.PartitionKey == "Korisnik"
                          select g;
            return results;
        }

        public IQueryable<Ponuda> RetrieveAllPonuda()
        {
            var results = from g in _table.CreateQuery<Ponuda>()
                          where g.PartitionKey == "Ponuda"
                          select g;
            return results;
        }

        public void AddAgencie(Agencija newAgencie)
        {
            TableOperation insertOperation = TableOperation.Insert(newAgencie);
            _table.Execute(insertOperation);
        }

        public void AddKorisnik(Korisnik newKorisnik)
        {
            TableOperation insertOperation = TableOperation.Insert(newKorisnik);
            _table.Execute(insertOperation);
        }
        public void AddPonuda(Ponuda newPonuda)
        {
            TableOperation insertOperation = TableOperation.Insert(newPonuda);
            _table.Execute(insertOperation);
        }

        public void ModifyAgencie(Agencija newInfo)
        {
            TableOperation retrieveOperation =
            TableOperation.Retrieve<Agencija>(newInfo.PartitionKey, newInfo.RowKey);
            var existingItem = _table.Execute(retrieveOperation);
            newInfo.ETag = existingItem.Etag;
            if (existingItem != null)
            {
                TableOperation replaceOperation =
                TableOperation.Replace(newInfo);
                _table.Execute(replaceOperation);
            }
        }
        public void ModifyKorisnik(Korisnik newInfo)
        {
            TableOperation retrieveOperation =
            TableOperation.Retrieve<Korisnik>(newInfo.PartitionKey, newInfo.RowKey);
            var existingItem = _table.Execute(retrieveOperation);
            newInfo.ETag = existingItem.Etag;
            if (existingItem != null)
            {
                TableOperation replaceOperation =
                TableOperation.Replace(newInfo);
                _table.Execute(replaceOperation);
            }
        }
        public void ModifyPonuda(Ponuda newInfo)
        {
            TableOperation retrieveOperation =
            TableOperation.Retrieve<Ponuda>(newInfo.PartitionKey, newInfo.RowKey);
            var existingItem = _table.Execute(retrieveOperation);
            newInfo.ETag = existingItem.Etag;
            if (existingItem != null)
            {
                TableOperation replaceOperation =
                TableOperation.Replace(newInfo);
                _table.Execute(replaceOperation);
            }
        }

        public void DeleteAgencie(Agencija deleteAgencie)
        {
            TableOperation retrieveOperation =
            TableOperation.Retrieve<Agencija>(deleteAgencie.PartitionKey, deleteAgencie.RowKey);
            var existingItem = _table.Execute(retrieveOperation);
            deleteAgencie.ETag = existingItem.Etag;
            TableOperation deleteOp = TableOperation.Delete(deleteAgencie);
            _table.Execute(deleteOp);

        }

        public void DeleteKorisnik(Korisnik deleteKorisnik)
        {
            TableOperation retrieveOperation =
            TableOperation.Retrieve<Korisnik>(deleteKorisnik.PartitionKey, deleteKorisnik.RowKey);
            var existingItem = _table.Execute(retrieveOperation);
            deleteKorisnik.ETag = existingItem.Etag;
            TableOperation deleteOp = TableOperation.Delete(deleteKorisnik);
            _table.Execute(deleteOp);

        }
        public void DeletePonuda(Ponuda deletePonuda)
        {
            TableOperation retrieveOperation =
            TableOperation.Retrieve<Ponuda>(deletePonuda.PartitionKey, deletePonuda.RowKey);
            var existingItem = _table.Execute(retrieveOperation);
            deletePonuda.ETag = existingItem.Etag;
            TableOperation deleteOp = TableOperation.Delete(deletePonuda);
            _table.Execute(deleteOp);

        }
    }
}
