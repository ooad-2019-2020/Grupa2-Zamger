using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZamgerV2_Implementation.Models;

namespace ZamgerV2_Implementation.Controllers
{
    public class StudentController : Controller
    {

        private Student trenutniKorisnik;
        private ZamgerDbContext zmgr;

        public StudentController()
        {
            zmgr = ZamgerDbContext.GetInstance();
        }
        [Route("/student/dashboard/{id}")]
        public IActionResult Dashboard(int id)
        {
            KreatorKorisnika creator = new KreatorKorisnika();
            Korisnik tempK = creator.FactoryMethod(id);
            if (tempK.GetType() == typeof(Student))
            {
                trenutniKorisnik = (Student)tempK;
            }
            else
            {
                trenutniKorisnik = (MasterStudent)tempK;
            }
            ViewBag.polozeni = zmgr.dajBrojPoloženihPredmeta(trenutniKorisnik.BrojIndeksa);
            ViewBag.nepolozeni = zmgr.dajBrojNepoloženihPredmeta(trenutniKorisnik.BrojIndeksa);
            ViewBag.prosjek = zmgr.dajProsjekPoID(trenutniKorisnik.BrojIndeksa);
            ViewBag.listaObavjestenja = zmgr.dajSvaObavještenja();
           
            return View(trenutniKorisnik);
        }

        [Route("/student/kreiraj-zahtjev/{id}")]
        [HttpGet]
        public IActionResult KreirajZahtjev(int id)
        {
            KreatorKorisnika creator = new KreatorKorisnika();
            Korisnik tempK = creator.FactoryMethod(id);
            if (tempK.GetType() == typeof(Student))
            {
                trenutniKorisnik = (Student)tempK;
            }
            else
            {
                trenutniKorisnik = (MasterStudent)tempK;
            }
            return View(trenutniKorisnik);
        }

        [Route("/student/kreiraj-zahtjev/{id}")]
        [HttpPost]
        public IActionResult KreirajZahtjev(int id, IFormCollection forma)
        {
            int idZaht = zmgr.generišiIdZahtjeva();
            if (zmgr.spremiZahtjev(new Zahtjev(id, forma["VrstaZahtjeva"].ToString(), DateTime.Now, 0, idZaht)))
            {
                return RedirectToAction("UspješnoKreiranZahtjev", new { id=id });
            }
            else
            {
                //ovdje treba neki error view vratit 404 il nešta
                return RedirectToAction("prikaziGresku", new { idStudenta = id, lokacija = "kreiraj-zahtjev" + id, idPoruke = 1 });
            }
        }


        [Route("/student/uspjesno-poslan-zahtjev/{id}")]
        [HttpGet]
        public IActionResult UspješnoKreiranZahtjev(int id)
        {
            KreatorKorisnika creator = new KreatorKorisnika();
            Korisnik tempK = creator.FactoryMethod(id);
            if (tempK.GetType() == typeof(Student))
            {
                trenutniKorisnik = (Student)tempK;
            }
            else
            {
                trenutniKorisnik = (MasterStudent)tempK;
            }
            return View(trenutniKorisnik);
        }

        [Route("/student/moji-zahtjevi/{id}")]
        [HttpGet]
        public IActionResult MojiZahtjevi(int id)
        {
            KreatorKorisnika creator = new KreatorKorisnika();
            Korisnik tempK = creator.FactoryMethod(id);
            if (tempK.GetType() == typeof(Student))
            {
                trenutniKorisnik = (Student)tempK;
            }
            else
            {
                trenutniKorisnik = (MasterStudent)tempK;
            }
            ViewBag.mojiZahtjevi = zmgr.dajZahtjeveZaStudenta(id);
            return View(trenutniKorisnik);
        }

        [Route("/student/sva-obavještenja-list/{id}")]
        public IActionResult AllStudentAnnouncementsList(int id)
        {
            KreatorKorisnika creator = new KreatorKorisnika();
            Korisnik tempK = creator.FactoryMethod(id);
            if (tempK.GetType() == typeof(Student))
            {
                trenutniKorisnik = (Student)tempK;
            }
            else
            {
                trenutniKorisnik = (MasterStudent)tempK;
            }

            ViewBag.listaObavjestenja = zmgr.dajSvaObavještenja();
            return View(trenutniKorisnik);
        }


        [Route("/student/obavještenje/{idObavještenja}/{id}")]
        public IActionResult AnnouncementStudentInfo(int idObavještenja, int id)
        {
            KreatorKorisnika creator = new KreatorKorisnika();
            Korisnik tempK = creator.FactoryMethod(id);
            if (tempK.GetType() == typeof(Student))
            {
                trenutniKorisnik = (Student)tempK;
            }
            else
            {
                trenutniKorisnik = (MasterStudent)tempK;
            }
            ViewBag.obavještenje = zmgr.dajObavještenjePoId(idObavještenja);
            return View(trenutniKorisnik);
        }

        [Route("/student/predmet/{id}/{idPredmeta}/{studijskaGodina}")]
        public IActionResult StudentSubjectInfo(int id, int idPredmeta, int studijskaGodina)
        {
            KreatorKorisnika creator = new KreatorKorisnika();
            Korisnik tempK = creator.FactoryMethod(id);
            if (tempK.GetType() == typeof(Student))
            {
                trenutniKorisnik = (Student)tempK;
            }
            else
            {
                trenutniKorisnik = (MasterStudent)tempK;
            }
            ViewBag.predmet = zmgr.dajPredmetZaStudentaPoID(id, idPredmeta, studijskaGodina);
            ViewBag.ansambl = zmgr.dajAnsamblNaPredmetu(idPredmeta);
            //ViewBag.listaPredmeta = zmgr.dajMojePredmete(id);

            return View(trenutniKorisnik);
        }


        [Route("/student/predmeti-list/{id}")]
        public IActionResult MySubjects(int id)
        {
            KreatorKorisnika creator = new KreatorKorisnika();
            Korisnik tempK = creator.FactoryMethod(id);
            if (tempK.GetType() == typeof(Student))
            {
                trenutniKorisnik = (Student)tempK;
            }
            else
            {
                trenutniKorisnik = (MasterStudent)tempK;
            }
            ViewBag.listaPredmeta = trenutniKorisnik.Predmeti;

            return View(trenutniKorisnik);
        }

        [Route("/student/poruke/moj-inbox/{idStudenta}")]
        public IActionResult mojInbox(int idStudenta)
        {
            KreatorKorisnika creator = new KreatorKorisnika();
            Korisnik tempK = creator.FactoryMethod(idStudenta);
            if (tempK.GetType() == typeof(Student))
            {
                trenutniKorisnik = (Student)tempK;
            }
            else
            {
                trenutniKorisnik = (MasterStudent)tempK;
            }
            return View(trenutniKorisnik);
        }
        [Route("/student/poruke/moj-outbox/{idStudenta}")]
        public IActionResult mojOutbox(int idStudenta)
        {
            KreatorKorisnika creator = new KreatorKorisnika();
            Korisnik tempK = creator.FactoryMethod(idStudenta);
            if (tempK.GetType() == typeof(Student))
            {
                trenutniKorisnik = (Student)tempK;
            }
            else
            {
                trenutniKorisnik = (MasterStudent)tempK;
            }
            return View(trenutniKorisnik);
        }

        [Route("/student/poruke/moj-inbox/{idPoruke}/{idStudenta}")]
        public IActionResult detaljiPorukeInbox(int idPoruke, int idStudenta)
        {
            KreatorKorisnika creator = new KreatorKorisnika();
            Korisnik tempK = creator.FactoryMethod(idStudenta);
            if (tempK.GetType() == typeof(Student))
            {
                trenutniKorisnik = (Student)tempK;
            }
            else
            {
                trenutniKorisnik = (MasterStudent)tempK;
            }
            ViewBag.poruka = zmgr.dajPoruku(idPoruke);
            zmgr.oznaciProcitanu(idPoruke);
            return View(trenutniKorisnik);
        }

        [Route("/student/poruke/moj-outbox/{idPoruke}/{idStudenta}")]
        public IActionResult detaljiPorukeOutbox(int idPoruke, int idStudenta)
        {
            KreatorKorisnika creator = new KreatorKorisnika();
            Korisnik tempK = creator.FactoryMethod(idStudenta);
            if (tempK.GetType() == typeof(Student))
            {
                trenutniKorisnik = (Student)tempK;
            }
            else
            {
                trenutniKorisnik = (MasterStudent)tempK;
            }
            ViewBag.poruka = zmgr.dajPoruku(idPoruke);
            return View(trenutniKorisnik);
        }

        [Route("/student/studenti-list/{idStudenta}")]
        public IActionResult searchStudentsForMessage(int idStudenta)
        {
            KreatorKorisnika creator = new KreatorKorisnika();
            Korisnik tempK = creator.FactoryMethod(idStudenta);
            if (tempK.GetType() == typeof(Student))
            {
                trenutniKorisnik = (Student)tempK;
            }
            else
            {
                trenutniKorisnik = (MasterStudent)tempK;
            }
            return View(trenutniKorisnik);
        }

        [Route("/student/studenti-list/{idStudenta}/pretraga")]
        [HttpPost]
        public IActionResult searchStudentsForMessageForm(IFormCollection forma, int idStudenta)
        {
            KreatorKorisnika creator = new KreatorKorisnika();
            Korisnik tempK = creator.FactoryMethod(idStudenta);
            if (tempK.GetType() == typeof(Student))
            {
                trenutniKorisnik = (Student)tempK;
            }
            else
            {
                trenutniKorisnik = (MasterStudent)tempK;
            }

            //Logger logg = Logger.GetInstance();

            List<Korisnik> korisnici = zmgr.pretražiKorisnike(forma["Ime"], forma["Prezime"]);

            ViewBag.korisnici = korisnici;

            return View(trenutniKorisnik);
        }


        [Route("/student/studenti-list/{idStudenta}/pretraga/{idPrimaoca}")]
        [HttpGet]
        public IActionResult sendMessage(int idStudenta, int idPrimaoca)
        {
            KreatorKorisnika creator = new KreatorKorisnika();
            Korisnik tempK = creator.FactoryMethod(idStudenta);
            if (tempK.GetType() == typeof(Student))
            {
                trenutniKorisnik = (Student)tempK;
            }
            else
            {
                trenutniKorisnik = (MasterStudent)tempK;
            }


            return View(trenutniKorisnik);
        }



        [Route("/student/studenti-list/{idStudenta}/pretraga/{idPrimaoca}")]
        [HttpPost]
        public IActionResult sendMessage(IFormCollection forma,int idStudenta, int idPrimaoca)
        {
            KreatorKorisnika creator = new KreatorKorisnika();
            Korisnik tempK = creator.FactoryMethod(idStudenta);
            if (tempK.GetType() == typeof(Student))
            {
                trenutniKorisnik = (Student)tempK;
            }
            else
            {
                trenutniKorisnik = (MasterStudent)tempK;
            }

            if(forma!=null)
            {
                string sadržaj = forma["sadržaj"];
                string naslov = forma["naslov"];
                Poruka poruka = new Poruka(idStudenta, idPrimaoca, naslov, sadržaj, DateTime.Now, 0, zmgr.dajNoviPorukaId());
                zmgr.posaljiPoruku(poruka);
                //Response.WriteAsync("Poruka je poslana: " + zmgr.posaljiPoruku(poruka));
                return RedirectToAction("prikaziGresku", new { idStudenta = idStudenta, lokacija = "studenti-list/"+idStudenta+"/pretraga/"+idPrimaoca, idPoruke=2}); 
            }

            return View(trenutniKorisnik);
        }

        [Route("/student/{idStudenta}/{lokacija}/greska/{idPoruke}")]
        public IActionResult prikaziGresku(int idStudenta, string lokacija, int idPoruke)
        {
            KreatorKorisnika creator = new KreatorKorisnika();
            Korisnik tempK = creator.FactoryMethod(idStudenta);
            if (tempK.GetType() == typeof(Student))
            {
                trenutniKorisnik = (Student)tempK;
            }
            else
            {
                trenutniKorisnik = (MasterStudent)tempK;
            }
            if (idPoruke == 1)
            {
                ViewBag.poruka = "Greška pri kreiranju zahtjeva";
            }
            else if (idPoruke == 2)
            {
                ViewBag.poruka = "Greška pri slanju poruke";
            }
            
            return View(trenutniKorisnik);
        }


    }
}