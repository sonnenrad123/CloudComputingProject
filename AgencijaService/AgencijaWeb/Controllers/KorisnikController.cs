using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;
using System.Web.Mvc;
using AgencijaWeb.Models;
using Common;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace AgencijaWeb.Controllers
{
    public class KorisnikController : Controller
    {
        // GET: Korisnik
        public ActionResult Index()
        {
            List<Korisnik> ret = new List<Korisnik>();
            //konektujemo se na reader
            NetTcpBinding readerbind = new NetTcpBinding();
            ChannelFactory<IReader> factory = new ChannelFactory<IReader>(readerbind, new
            EndpointAddress("net.tcp://localhost:10101/InputRequest"));
            IReader proxyreader = factory.CreateChannel();

            List<string> temp = proxyreader.prihvatiZahtevCitanjeSveKorisnik();

            foreach(string entitystring in temp)
            {
                string[] podaci = entitystring.Split(';');
                Korisnik k = new Korisnik() { username = podaci[0], ime = podaci[1], prezime = podaci[2], id = Int32.Parse(podaci[3]) };
                ret.Add(k);
            }

            return View(ret);
        }

        // GET: Korisnik/Details/5
        public ActionResult Details(int id)
        {
            //konektujemo se na reader i trazimo podatak
            NetTcpBinding readerbind = new NetTcpBinding();
            ChannelFactory<IReader> factory = new ChannelFactory<IReader>(readerbind, new
           EndpointAddress("net.tcp://localhost:10101/InputRequest"));
            IReader proxyreader = factory.CreateChannel();
            string temp = proxyreader.prihvatiZahtevCitanjeJednogKorisnik(id);
            string[] podaci = temp.Split(';');
            Korisnik k = new Korisnik() { username = podaci[0], ime = podaci[1], prezime = podaci[2], id = Int32.Parse(podaci[3]) };
            return View(k);
        }

        // GET: Korisnik/Create
        public ActionResult Create()
        {
            

            return View();
        }

        // POST: Korisnik/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            //prvo provera postojanja preko readera,ide input radi azure load balancera
            NetTcpBinding readerbind = new NetTcpBinding();
            ChannelFactory<IReader> factory = new ChannelFactory<IReader>(readerbind, new
           EndpointAddress("net.tcp://localhost:10101/InputRequest"));
            IReader proxyreader = factory.CreateChannel();
            if (proxyreader.proveriDaLiPostojiDodavanjeKorisnik(collection["username"], collection["ime"], collection["prezime"]))
            {
                ViewBag.Error = "Doslo je do greske! Postoji vec dati username.";
                return View();
            }
            else
            {
                NetTcpBinding binding = new NetTcpBinding();
                string internalEndpointName = "InternalRequest";
                List<RoleInstance> bratske_instance = RoleEnvironment.Roles["WriterWorkerRole"].Instances.ToList(); //uzmemo sve writer instance
                RoleInstance rola_dva = bratske_instance[1];//uzmemo samo drugu jer ona radi sa secondary entitetima
                try
                {
                    RoleInstanceEndpoint pristupna = rola_dva.InstanceEndpoints[internalEndpointName];
                    EndpointAddress endpoint = new EndpointAddress(String.Format("net.tcp://{0}/{1}", pristupna.IPEndpoint.ToString(), internalEndpointName));
                    IWriter proxy = new ChannelFactory<IWriter>(binding, endpoint).CreateChannel();
                    proxy.PrihvatiZahtevCreateKorisnik(collection["username"], collection["ime"], collection["prezime"], IDGenerator.idKorisnik);
                    IDGenerator.idKorisnik++;
                    return RedirectToAction("Index");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }

        // GET: Korisnik/Edit/5
        public ActionResult Edit(int id)
        {
            //Procitamo opet preko readera podatak da bi dobavili trenutne informacije
            NetTcpBinding readerbind = new NetTcpBinding();
            ChannelFactory<IReader> factory = new ChannelFactory<IReader>(readerbind, new
            EndpointAddress("net.tcp://localhost:10101/InputRequest"));
            IReader proxyreader = factory.CreateChannel();
            string temp = proxyreader.prihvatiZahtevCitanjeJednogKorisnik(id);
            string[] podaci = temp.Split(';');
            Korisnik k = new Korisnik() { username = podaci[0], ime = podaci[1], prezime = podaci[2], id = Int32.Parse(podaci[3]) };

            return View(k);
        }

        // POST: Korisnik/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                NetTcpBinding binding = new NetTcpBinding();
                string internalEndpointName = "InternalRequest";
                List<RoleInstance> bratske_instance = RoleEnvironment.Roles["WriterWorkerRole"].Instances.ToList(); //uzmemo sve writer instance
                RoleInstance drugaInstanca = bratske_instance[1];//samo druga jer je secondary entity type
                RoleInstanceEndpoint pristupna = drugaInstanca.InstanceEndpoints[internalEndpointName];
                EndpointAddress endpoint = new EndpointAddress(String.Format("net.tcp://{0}/{1}", pristupna.IPEndpoint.ToString(), internalEndpointName));
                IWriter proxy = new ChannelFactory<IWriter>(binding, endpoint).CreateChannel();
                proxy.PrihvatiZahtevModifyKorisnik(collection["username"], collection["ime"], collection["prezime"], id);

                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        // GET: Korisnik/Delete/5
        public ActionResult Delete(int id)
        {
            //Procitamo opet preko readera podatak da bi dobavili trenutne informacije
            NetTcpBinding readerbind = new NetTcpBinding();
            ChannelFactory<IReader> factory = new ChannelFactory<IReader>(readerbind, new
            EndpointAddress("net.tcp://localhost:10101/InputRequest"));
            IReader proxyreader = factory.CreateChannel();
            string temp = proxyreader.prihvatiZahtevCitanjeJednogKorisnik(id);
            string[] podaci = temp.Split(';');
            Korisnik k = new Korisnik() { username = podaci[0], ime = podaci[1], prezime = podaci[2], id = Int32.Parse(podaci[3]) };
            return View(k);
        }

        // POST: Korisnik/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                NetTcpBinding binding = new NetTcpBinding();
                string internalEndpointName = "InternalRequest";
                List<RoleInstance> bratske_instance = RoleEnvironment.Roles["WriterWorkerRole"].Instances.ToList(); //uzmemo sve writer instance
                RoleInstance druga_instanca = bratske_instance[1]; //treba nam samo druga jer je secondary type entity
                RoleInstanceEndpoint pristupna = druga_instanca.InstanceEndpoints[internalEndpointName];
                EndpointAddress endpoint = new EndpointAddress(String.Format("net.tcp://{0}/{1}", pristupna.IPEndpoint.ToString(), internalEndpointName));
                IWriter proxy = new ChannelFactory<IWriter>(binding, endpoint).CreateChannel();
                
                proxy.PrihvatiZahtevBrisanjeKorisnik(collection["username"], collection["ime"], collection["prezime"], id);

                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
