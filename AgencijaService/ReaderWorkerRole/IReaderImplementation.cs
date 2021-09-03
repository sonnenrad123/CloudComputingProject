using AgencijaProject_Data;
using AgencijaWeb.Models;
using Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReaderWorkerRole
{
    class IReaderImplementation : IReader
    {
        public string prihvatiZahtevCitanjeJednog(int id)
        {
            string ret = "";
            AgencijaDataRepository repo = new AgencijaDataRepository();
            List<Agencija> agencijeTemp = repo.RetrieveAllAgencies().ToList();
            foreach (Agencija temp in agencijeTemp)
            {
                if (temp.id == id)
                {
                    ret = temp.adresa + ";" + temp.drzava + ";" + temp.grad + ";" + temp.id + ";" + temp.ime + ";"; //formatiramo entitete da bi mogli da ih rekreiramo u web roli
                }
            }
            return ret;
        }

        public string prihvatiZahtevCitanjeJednogKorisnik(int id)
        {
            //Trace.WriteLine("Invoked CitanjeJednogKorisnik");

            string ret = "";
            AgencijaDataRepository repo = new AgencijaDataRepository();
            List<Korisnik> korisniciTemp = repo.RetrieveAllKorisnik().ToList();
            foreach(Korisnik temp in korisniciTemp)
            {
                if(temp.id == id)
                {
                    ret = temp.username + ";" + temp.ime + ";" + temp.prezime + ";" + temp.id + ";";
                }
            }
            return ret;
        }

        public string prihvatiZahtevCitanjeJednogPonuda(int id)
        {
            string ret = "";
            AgencijaDataRepository repo = new AgencijaDataRepository();
            List<Ponuda> ponudeTemp = repo.RetrieveAllPonuda().ToList();
            foreach(Ponuda p in ponudeTemp)
            {
                if (p.id == id)
                {
                    ret = p.ime + ";" + p.opis + ";" + p.cena + ";" + p.id + ";";
                }
            }
            return ret;
        }

        public List<string> prihvatiZahtevCitanjeSve()
        {
            List<string> ret = new List<string>();
            AgencijaDataRepository repo = new AgencijaDataRepository();
            List<Agencija> agencijeTemp = repo.RetrieveAllAgencies().ToList();
            foreach(Agencija temp in agencijeTemp)
            {
                ret.Add(temp.adresa + ";" + temp.drzava + ";" + temp.grad + ";" + temp.id + ";" + temp.ime + ";"); //formatiramo entitete da bi mogli da ih rekreiramo u web roli
            }
            //Trace.WriteLine("Prihvacen zahtev za citanje svega");
            return ret;
        }

        public List<string> prihvatiZahtevCitanjeSveKorisnik()
        {
            //Trace.WriteLine("Invoked prihvatiZahtevCitanjeSveKorisnik");
            List<string> ret = new List<string>();
            AgencijaDataRepository repo = new AgencijaDataRepository();
            List<Korisnik> korisniciTemp = repo.RetrieveAllKorisnik().ToList();
            foreach(Korisnik temp in korisniciTemp)
            {
                ret.Add(temp.username + ";" + temp.ime + ";" + temp.prezime + ";" + temp.id + ";");
            }
            return ret;
        }

        public List<string> prihvatiZahtevCitanjeSvePonuda()
        {
            List<string> ret = new List<string>();
            AgencijaDataRepository repo = new AgencijaDataRepository();
            List<Ponuda> ponudeTemp = repo.RetrieveAllPonuda().ToList();
            foreach(Ponuda p in ponudeTemp)
            {
                ret.Add(p.ime + ";" + p.opis + ";" + p.cena + ";" + p.id + ";");
            }
            return ret;
        }

        public bool proveriDaLiPostojiDodavanje(string ime, string adresa, string drzava, string grad)
        {
            AgencijaDataRepository repo = new AgencijaDataRepository();
            List<Agencija> agencijeTemp = repo.RetrieveAllAgencies().ToList();
            //Trace.WriteLine("Prihvacen zahtev za proveru postojanja");
            foreach(Agencija temp in agencijeTemp)
            {
                if(temp.ime == ime && temp.adresa == adresa && temp.drzava==drzava && temp.grad == grad)//ako vec postoji vracamo true
                {
                    return true;
                }
            }
            return false;
        }

        public bool proveriDaLiPostojiDodavanjeKorisnik(string username, string ime, string prezime)
        {
            //Trace.WriteLine("Invoked proveriDaLiPostojiDodavanjeKorisnik");
            AgencijaDataRepository repo = new AgencijaDataRepository();
            List<Korisnik> korisniciTemp = repo.RetrieveAllKorisnik().ToList();
            foreach(Korisnik temp in korisniciTemp)
            {
                if(temp.username==username)
                {
                    return true;
                }
            }
            return false;
        }

        public bool proveriDaLiPostojiDodavanjePonuda(string ime_ponude, string opis_ponude, double cena)
        {
            AgencijaDataRepository repo = new AgencijaDataRepository();
            List<Ponuda> ponudeTemp = repo.RetrieveAllPonuda().ToList(); 
            foreach(Ponuda temp in ponudeTemp)
            {
                if(temp.ime==ime_ponude && temp.opis==opis_ponude&&temp.cena == cena)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
