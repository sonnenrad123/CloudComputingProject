using AgencijaWeb.Models;
using Common;
using Microsoft.WindowsAzure.ServiceRuntime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;
using System.Web.Mvc;

namespace AgencijaWeb.Controllers
{
    public class PonudaController : Controller
    {
        // GET: Ponuda
        public ActionResult Index()
        {
            List<Ponuda> ret = new List<Ponuda>();
            NetTcpBinding readerbind = new NetTcpBinding();
            ChannelFactory<IReader> factory = new ChannelFactory<IReader>(readerbind, new
            EndpointAddress("net.tcp://localhost:10101/InputRequest"));
            IReader proxyreader = factory.CreateChannel();
            List<string> temp = proxyreader.prihvatiZahtevCitanjeSvePonuda();

            foreach (string entitystring in temp)
            {
                string[] podaci = entitystring.Split(';');
                Ponuda p = new Ponuda() { ime = podaci[0], opis = podaci[1], cena = Double.Parse(podaci[2]), id = int.Parse(podaci[3]) };
                ret.Add(p);
            }

            return View(ret);
        }

        // GET: Ponuda/Details/5
        public ActionResult Details(int id)
        {
            //konektujemo se na reader i trazimo podatak
            NetTcpBinding readerbind = new NetTcpBinding();
            ChannelFactory<IReader> factory = new ChannelFactory<IReader>(readerbind, new
           EndpointAddress("net.tcp://localhost:10101/InputRequest"));
            IReader proxyreader = factory.CreateChannel();
            string temp = proxyreader.prihvatiZahtevCitanjeJednogPonuda(id);
            string[] podaci = temp.Split(';');
            Ponuda p = new Ponuda() { ime = podaci[0], opis = podaci[1], cena = Double.Parse(podaci[2]), id = int.Parse(podaci[3]) };

            return View(p);
        }

        // GET: Ponuda/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Ponuda/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            //prvo provera postojanja preko readera,ide input radi azure load balancera
            NetTcpBinding readerbind = new NetTcpBinding();
            ChannelFactory<IReader> factory = new ChannelFactory<IReader>(readerbind, new
           EndpointAddress("net.tcp://localhost:10101/InputRequest"));
            IReader proxyreader = factory.CreateChannel();
            if (proxyreader.proveriDaLiPostojiDodavanjePonuda(collection["ime"], collection["opis"], Double.Parse(collection["cena"])))
            {
                ViewBag.Error = "Doslo je do greske! Postoji vec data ponuda.";
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
                    proxy.PrihvatiZahtevCreatePonuda(collection["ime"], collection["opis"], Double.Parse(collection["cena"]), IDGenerator.idPonuda);
                    IDGenerator.idPonuda++;

                    return RedirectToAction("Index");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }

        // GET: Ponuda/Edit/5
        public ActionResult Edit(int id)
        {
            //Procitamo opet preko readera podatak da bi dobavili trenutne informacije
            NetTcpBinding readerbind = new NetTcpBinding();
            ChannelFactory<IReader> factory = new ChannelFactory<IReader>(readerbind, new
            EndpointAddress("net.tcp://localhost:10101/InputRequest"));
            IReader proxyreader = factory.CreateChannel();
            string temp = proxyreader.prihvatiZahtevCitanjeJednogPonuda(id);
            string[] podaci = temp.Split(';');
            Ponuda p = new Ponuda() { ime = podaci[0], opis = podaci[1], cena = Double.Parse(podaci[2]), id = int.Parse(podaci[3]) };

            return View(p);
        }

        // POST: Ponuda/Edit/5
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
                string opis = collection["opis"];
                double cena = Double.Parse(collection["cena"]);
                proxy.PrihvatiZahtevModifyPonuda(collection["ime"], collection["opis"], Double.Parse(collection["cena"]), id);

                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        // GET: Ponuda/Delete/5
        public ActionResult Delete(int id)
        {
            //Procitamo opet preko readera podatak da bi dobavili trenutne informacije
            NetTcpBinding readerbind = new NetTcpBinding();
            ChannelFactory<IReader> factory = new ChannelFactory<IReader>(readerbind, new
            EndpointAddress("net.tcp://localhost:10101/InputRequest"));
            IReader proxyreader = factory.CreateChannel();
            string temp = proxyreader.prihvatiZahtevCitanjeJednogPonuda(id);
            string[] podaci = temp.Split(';');
            Ponuda p = new Ponuda() { ime = podaci[0], opis = podaci[1], cena = Double.Parse(podaci[2]), id = int.Parse(podaci[3]) };
            return View(p);
        }

        // POST: Ponuda/Delete/5
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

                double cena = Double.Parse(collection["cena"]);

                proxy.PrihvatiZahtevBrisanjePonuda(collection["ime"], collection["opis"], cena, id);

                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
