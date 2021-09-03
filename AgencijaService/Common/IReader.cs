using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    [ServiceContract]
    public interface IReader
    {
        [OperationContract]
        List<string> prihvatiZahtevCitanjeSve();
        [OperationContract]
        string prihvatiZahtevCitanjeJednog(int id);
        [OperationContract]
        bool proveriDaLiPostojiDodavanje(string ime, string adresa, string drzava, string grad);
        [OperationContract]
        List<string> prihvatiZahtevCitanjeSveKorisnik();
        [OperationContract]
        string prihvatiZahtevCitanjeJednogKorisnik(int id);
        [OperationContract]
        bool proveriDaLiPostojiDodavanjeKorisnik(string username, string ime, string prezime);
        [OperationContract]
        List<string> prihvatiZahtevCitanjeSvePonuda();
        [OperationContract]
        string prihvatiZahtevCitanjeJednogPonuda(int id);
        [OperationContract]
        bool proveriDaLiPostojiDodavanjePonuda(string ime_ponude,string opis_ponude,double cena);


    }
}
