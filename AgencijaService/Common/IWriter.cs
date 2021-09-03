using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    [ServiceContract]
    public interface IWriter
    {
        [OperationContract]
        bool PrihvatiZahtevCreate(string ime, string adresa, string drzava, string grad,int id);
        [OperationContract]
        void PrihvatiZahtevModify(string ime, string adresa, string drzava, string grad, int id);
        [OperationContract]
        void PrihvatiZahtevBrisanje(string ime, string adresa, string drzava, string grad, int id);
        [OperationContract]
        bool PrihvatiZahtevCreateKorisnik(string username, string ime, string prezime, int id);
        [OperationContract]
        void PrihvatiZahtevModifyKorisnik(string username, string ime, string prezime, int id);
        [OperationContract]
        void PrihvatiZahtevBrisanjeKorisnik(string username, string ime, string prezime, int id);
        [OperationContract]
        bool PrihvatiZahtevCreatePonuda(string ime, string opis, double cena, int id);
        [OperationContract]
        void PrihvatiZahtevModifyPonuda(string ime, string opis, double cena, int id);
        [OperationContract]
        void PrihvatiZahtevBrisanjePonuda(string ime, string opis, double cena, int id);
    }
}
