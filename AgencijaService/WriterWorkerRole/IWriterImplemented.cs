using AgencijaWeb.Models;
using Common;
using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace WriterWorkerRole
{
    class IWriterImplemented : IWriter
    {
        public void PrihvatiZahtevBrisanje(string ime, string adresa, string drzava, string grad, int id)
        {
            Agencija temp = new Agencija(ime, adresa, grad, drzava, id);
            AgencijaProject_Data.AgencijaDataRepository repo = new AgencijaProject_Data.AgencijaDataRepository();
            repo.DeleteAgencie(temp);
            LogujDogadjaj("["+DateTime.Now.ToString()+"]:" + String.Format("Izvrseno brisanje agencije sa sledecim podacima. Ime: {0}; Adresa:{1}; Drzava:{2}; Grad:{3}",ime,adresa,drzava,grad));
        }

        public void PrihvatiZahtevBrisanjeKorisnik(string username, string ime, string prezime, int id)
        {
            Korisnik temp = new Korisnik(username, ime, prezime, id);
            AgencijaProject_Data.AgencijaDataRepository repo = new AgencijaProject_Data.AgencijaDataRepository();
            repo.DeleteKorisnik(temp);
            //Trace.WriteLine("Invoked ZahtevBrisanje");
            LogujDogadjaj("[" + DateTime.Now.ToString() + "]:" + String.Format("Izvrseno brisanje korisnika sa sledecim podacima. Username: {0}; Ime: {1}; Prezime: {2}",username,ime,prezime));
        }

        public void PrihvatiZahtevBrisanjePonuda(string ime, string opis, double cena, int id)
        {
            Ponuda temp = new Ponuda(ime, opis, cena, id);
            AgencijaProject_Data.AgencijaDataRepository repo = new AgencijaProject_Data.AgencijaDataRepository();
            repo.DeletePonuda(temp);
            LogujDogadjaj("[" + DateTime.Now.ToString() + "]:" + String.Format("Izvrseno brisanje ponude sa sledecim podacima. Ime: {0}; Opis: {1}; Cena:{2}",ime,opis,cena));
        }

        public bool PrihvatiZahtevCreate(string ime, string adresa, string drzava, string grad,int id)
        {
            Agencija temp = new Agencija(ime, adresa, grad, drzava, id);         
            AgencijaProject_Data.AgencijaDataRepository repo = new AgencijaProject_Data.AgencijaDataRepository();
            repo.AddAgencie(temp);
            LogujDogadjaj("[" + DateTime.Now.ToString() + "]:" + String.Format("Izvrseno kreiranje novog entiteta Agencija sa sledecim podacima. Ime: {0}; Adresa:{1}; Drzava:{2}; Grad:{3}", ime, adresa, drzava, grad));
            return true;
        }

        public bool PrihvatiZahtevCreateKorisnik(string username, string ime, string prezime, int id)
        {
            Korisnik temp = new Korisnik(username, ime, prezime, id);
            AgencijaProject_Data.AgencijaDataRepository repo = new AgencijaProject_Data.AgencijaDataRepository();
            repo.AddKorisnik(temp);
            //Trace.WriteLine("Invoked CreateKorisnik");
            LogujDogadjaj("[" + DateTime.Now.ToString() + "]:" + String.Format("Izvrseno kreiranje novog entiteta Korisnik sa sledecim podacima. Username: {0}; Ime: {1}; Prezime: {2}", username, ime, prezime));
            return true;
        }

        public bool PrihvatiZahtevCreatePonuda(string ime, string opis, double cena, int id)
        {
            Ponuda temp = new Ponuda(ime, opis, cena, id);
            AgencijaProject_Data.AgencijaDataRepository repo = new AgencijaProject_Data.AgencijaDataRepository();
            repo.AddPonuda(temp);
            LogujDogadjaj("[" + DateTime.Now.ToString() + "]:" + String.Format("Izvrseno kreiranje novog entiteta Ponuda sa sledecim podacima. Ime: {0}; Opis: {1}; Cena:{2}", ime, opis, cena));
            return true;
        }

        public void PrihvatiZahtevModify(string ime, string adresa, string drzava, string grad, int id)
        {
            Agencija temp = new Agencija(ime, adresa, grad, drzava, id);
            AgencijaProject_Data.AgencijaDataRepository repo = new AgencijaProject_Data.AgencijaDataRepository();
            repo.ModifyAgencie(temp);
            LogujDogadjaj("[" + DateTime.Now.ToString() + "]:" + String.Format("Izvrsena modifikacija entiteta Agencija sa sledecim podacima. Ime: {0}; Adresa:{1}; Drzava:{2}; Grad:{3}", ime, adresa, drzava, grad));
        }

        public void PrihvatiZahtevModifyKorisnik(string username, string ime, string prezime, int id)
        {
            Korisnik temp = new Korisnik(username, ime, prezime, id);
            AgencijaProject_Data.AgencijaDataRepository repo = new AgencijaProject_Data.AgencijaDataRepository();
            repo.ModifyKorisnik(temp);
            //Trace.WriteLine("Invoked Modify korisnik");
            LogujDogadjaj("[" + DateTime.Now.ToString() + "]:" + String.Format("Izvrsena modifikacija entiteta Korisnik sa sledecim podacima. Username: {0}; Ime: {1}; Prezime: {2}", username, ime, prezime));
        }

        public void PrihvatiZahtevModifyPonuda(string ime, string opis, double cena, int id)
        {
            Ponuda temp = new Ponuda(ime, opis, cena, id);
            AgencijaProject_Data.AgencijaDataRepository repo = new AgencijaProject_Data.AgencijaDataRepository();
            repo.ModifyPonuda(temp);
            LogujDogadjaj("[" + DateTime.Now.ToString() + "]:" + String.Format("Izvrsena modifikacija entiteta Ponuda sa sledecim podacima. Ime: {0}; Opis: {1}; Cena:{2}", ime, opis, cena));
        }

        void LogujDogadjaj(string logMessage)
        {
            CloudQueue logQueue = AgencijaProject_Data.QueueHelper.GetQueueReference("logqueue");

            logQueue.AddMessage(new CloudQueueMessage(logMessage));
        }
    }
}
