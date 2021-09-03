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
    public class AgencijaController : Controller
    {
        public static uint cnt = 0;
        // GET: Agencija
        public ActionResult Index()//iscitavanje agencija
        {
            List<AgencijaWeb.Models.Agencija> ret = new List<Models.Agencija>();
            //konektujemo se na reader
            NetTcpBinding readerbind = new NetTcpBinding();
            ChannelFactory<IReader> factory = new ChannelFactory<IReader>(readerbind, new
           EndpointAddress("net.tcp://localhost:10101/InputRequest"));
            IReader proxyreader = factory.CreateChannel();

            List<string> temp = proxyreader.prihvatiZahtevCitanjeSve(); //pokupimo sve stringove za rekreaciju

            foreach(string entitystring in temp)
            {
                string[] podaci = entitystring.Split(';');
                Agencija ag = new Agencija() { adresa = podaci[0], drzava = podaci[1], grad = podaci[2], id = Int32.Parse(podaci[3]), ime = podaci[4] };
                ret.Add(ag);
            }

            return View(ret);
        }

        // GET: Agencija/Details/5
        public ActionResult Details(int id)
        {
            //konektujemo se na reader i trazimo podatak
            NetTcpBinding readerbind = new NetTcpBinding();
            ChannelFactory<IReader> factory = new ChannelFactory<IReader>(readerbind, new
           EndpointAddress("net.tcp://localhost:10101/InputRequest"));
            IReader proxyreader = factory.CreateChannel();
            string temp = proxyreader.prihvatiZahtevCitanjeJednog(id);

            string[] podaci = temp.Split(';');
            Agencija ag = new Agencija() { adresa = podaci[0], drzava = podaci[1], grad = podaci[2], id = Int32.Parse(podaci[3]), ime = podaci[4] };


            return View(ag);
        }

        // GET: Agencija/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Agencija/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            //prvo provera postojanja preko readera,ide input radi azure load balancera
            NetTcpBinding readerbind = new NetTcpBinding();
            ChannelFactory<IReader> factory = new ChannelFactory<IReader>(readerbind, new
           EndpointAddress("net.tcp://localhost:10101/InputRequest"));
            IReader proxyreader = factory.CreateChannel();
            if (proxyreader.proveriDaLiPostojiDodavanje(collection["ime"], collection["adresa"], collection["drzava"], collection["grad"]))//ako postoji sa istim podacima ne moze se dodati
            {
                ViewBag.Error = "Doslo je do greske! Postoji vec isti entitet.";
                return View();
            }
            else//sve je uredu
            {
             
                NetTcpBinding binding = new NetTcpBinding();
                string internalEndpointName = "InternalRequest";
                List<RoleInstance> bratske_instance = RoleEnvironment.Roles["WriterWorkerRole"].Instances.ToList(); //uzmemo sve writer instance
                try
                {
                    // Konektuje se na prvi ili treci writer po principu ostatka pri deljenju dvojkom
                    if (cnt % 2 == 0)//prva instanca
                    {
                        RoleInstanceEndpoint pristupna = bratske_instance[0].InstanceEndpoints[internalEndpointName];
                        EndpointAddress endpoint = new EndpointAddress(String.Format("net.tcp://{0}/{1}", pristupna.IPEndpoint.ToString(), internalEndpointName));
                        IWriter proxy = new ChannelFactory<IWriter>(binding, endpoint).CreateChannel();
                        //saljemo zahtev i dobijamo rezultat uspesnosti
                        bool zahtev = proxy.PrihvatiZahtevCreate(collection["ime"], collection["adresa"], collection["drzava"], collection["grad"],IDGenerator.idAgencija);
                        IDGenerator.idAgencija++;
                        cnt++;
                    }
                    else//treca instanca
                    {
                        RoleInstanceEndpoint pristupna = bratske_instance[2].InstanceEndpoints[internalEndpointName];
                        EndpointAddress endpoint = new EndpointAddress(String.Format("net.tcp://{0}/{1}", pristupna.IPEndpoint.ToString(), internalEndpointName));
                        IWriter proxy = new ChannelFactory<IWriter>(binding, endpoint).CreateChannel();
                        bool zahtev = proxy.PrihvatiZahtevCreate(collection["ime"], collection["adresa"], collection["drzava"], collection["grad"],IDGenerator.idAgencija);
                        IDGenerator.idAgencija++;
                        cnt++;
                    }





                    return RedirectToAction("Index");
                }
                catch(Exception e)
                {
                    throw e;
                }
            }
        }

        // GET: Agencija/Edit/5
        public ActionResult Edit(int id)
        {
            //Procitamo opet preko readera podatak da bi dobavili trenutne informacije
            NetTcpBinding readerbind = new NetTcpBinding();
            ChannelFactory<IReader> factory = new ChannelFactory<IReader>(readerbind, new
            EndpointAddress("net.tcp://localhost:10101/InputRequest"));
            IReader proxyreader = factory.CreateChannel();
            string temp = proxyreader.prihvatiZahtevCitanjeJednog(id);

            string[] podaci = temp.Split(';');
            Agencija ag = new Agencija() { adresa = podaci[0], drzava = podaci[1], grad = podaci[2], id = Int32.Parse(podaci[3]), ime = podaci[4] };

            return View(ag);
        }




        // POST: Agencija/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                NetTcpBinding binding = new NetTcpBinding();
                string internalEndpointName = "InternalRequest";
                List<RoleInstance> bratske_instance = RoleEnvironment.Roles["WriterWorkerRole"].Instances.ToList(); //uzmemo sve writer instance

                if (cnt % 2 == 0)//prva instanca
                {
                    RoleInstanceEndpoint pristupna = bratske_instance[0].InstanceEndpoints[internalEndpointName];
                    EndpointAddress endpoint = new EndpointAddress(String.Format("net.tcp://{0}/{1}", pristupna.IPEndpoint.ToString(), internalEndpointName));
                    IWriter proxy = new ChannelFactory<IWriter>(binding, endpoint).CreateChannel();
                    
                    proxy.PrihvatiZahtevModify(collection["ime"], collection["adresa"], collection["drzava"], collection["grad"], id);
                    
                    cnt++;
                }
                else//treca instanca
                {
                    RoleInstanceEndpoint pristupna = bratske_instance[2].InstanceEndpoints[internalEndpointName];
                    EndpointAddress endpoint = new EndpointAddress(String.Format("net.tcp://{0}/{1}", pristupna.IPEndpoint.ToString(), internalEndpointName));
                    IWriter proxy = new ChannelFactory<IWriter>(binding, endpoint).CreateChannel();
                    proxy.PrihvatiZahtevModify(collection["ime"], collection["adresa"], collection["drzava"], collection["grad"], id);

                    cnt++;
                }




                return RedirectToAction("Index");
            }
            catch(Exception e)
            {
                throw e;
            }
        }





        // GET: Agencija/Delete/5
        public ActionResult Delete(int id)
        {
            //Procitamo opet preko readera podatak da bi dobavili trenutne informacije
            NetTcpBinding readerbind = new NetTcpBinding();
            ChannelFactory<IReader> factory = new ChannelFactory<IReader>(readerbind, new
            EndpointAddress("net.tcp://localhost:10101/InputRequest"));
            IReader proxyreader = factory.CreateChannel();
            string temp = proxyreader.prihvatiZahtevCitanjeJednog(id);

            string[] podaci = temp.Split(';');
            Agencija ag = new Agencija() { adresa = podaci[0], drzava = podaci[1], grad = podaci[2], id = Int32.Parse(podaci[3]), ime = podaci[4] };

            return View(ag);
        }

        // POST: Agencija/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                NetTcpBinding binding = new NetTcpBinding();
                string internalEndpointName = "InternalRequest";
                List<RoleInstance> bratske_instance = RoleEnvironment.Roles["WriterWorkerRole"].Instances.ToList(); //uzmemo sve writer instance

                if (cnt % 2 == 0)//prva instanca konektovanje
                {
                    RoleInstanceEndpoint pristupna = bratske_instance[0].InstanceEndpoints[internalEndpointName];
                    EndpointAddress endpoint = new EndpointAddress(String.Format("net.tcp://{0}/{1}", pristupna.IPEndpoint.ToString(), internalEndpointName));
                    IWriter proxy = new ChannelFactory<IWriter>(binding, endpoint).CreateChannel();
                    

                    proxy.PrihvatiZahtevBrisanje(collection["ime"], collection["adresa"], collection["drzava"], collection["grad"], id);

                    cnt++;
                }
                else//treca instanca konektovanje
                {
                    RoleInstanceEndpoint pristupna = bratske_instance[2].InstanceEndpoints[internalEndpointName];
                    EndpointAddress endpoint = new EndpointAddress(String.Format("net.tcp://{0}/{1}", pristupna.IPEndpoint.ToString(), internalEndpointName));
                    IWriter proxy = new ChannelFactory<IWriter>(binding, endpoint).CreateChannel();
                    proxy.PrihvatiZahtevBrisanje(collection["ime"], collection["adresa"], collection["drzava"], collection["grad"], id);

                    cnt++;
                }




                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
