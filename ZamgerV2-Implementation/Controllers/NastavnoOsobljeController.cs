using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using ZamgerV2_Implementation.Helpers;
using ZamgerV2_Implementation.Models;

namespace ZamgerV2_Implementation.Controllers
{
    /*OGROMNA NAPOMENA 
     
        Problem je autorizacije sto se u svakoj metodi mora provjerit npr kad se ide /poruke/moj-outbox/9 mora se provjerit da li idPoruke se zaista
        nalazi u outboxu trenutno logovane osobe
        ako se nalazi onda tek prikazat full poruku a a ko se ne nalazi ide redirectto 401 error u početni kontroler odnoso pristupOdbijen!
            
     */
    [Autorizacija(false, TipKorisnika.NastavnoOsoblje, TipKorisnika.Profesor)]
    public class NastavnoOsobljeController : Controller
    {
        private NastavnoOsoblje trenutniKorisnik;
        private ZamgerDbContext zmgr;

        public NastavnoOsobljeController()
        {
            zmgr = ZamgerDbContext.GetInstance();
        }

        [Route("/nastavno-osoblje/dashboard")]
        public IActionResult Dashboard()
        {
            trenutniKorisnik = Autentifikacija.GetNastavnoOsoblje(HttpContext);

            ViewBag.listaObavjestenja = zmgr.dajSvaObavještenja();
            return View(trenutniKorisnik);
        }


        [Route("/nastavno-osoblje/kreiraj-aktivnost")]
        [HttpGet]
        public IActionResult KreirajAktivnost()
        {
            var trenutniKorisnik = Autentifikacija.GetNastavnoOsoblje(HttpContext);
            return View(trenutniKorisnik);
        }

        [Route("/nastavno-osoblje/kreiraj-aktivnost")]
        [HttpPost]
        public IActionResult KreirajAktivnost(int id, IFormCollection forma)
        {
            var trenutniKorisnik = Autentifikacija.GetNastavnoOsoblje(HttpContext);
            DateTime oDate = Convert.ToDateTime(forma["datum"] + " " + forma["vrijeme"]);

            int idAktivnosti = zmgr.kreirajAktivnost(int.Parse(forma["izabraniPredmet"]), forma["naziv"], oDate, forma["vrstaAktivnosti"], double.Parse(forma["maxBrojBodova"]));
            if (forma["vrstaAktivnosti"].Equals("Zadaća"))
            {
                for (int i = 0; i < trenutniKorisnik.PredmetiNaKojimPredaje.Count; i++)
                {
                    if (trenutniKorisnik.PredmetiNaKojimPredaje[i].IdPredmeta == int.Parse(forma["izabraniPredmet"]))
                    {
                        zmgr.ubaciDefaultPodatkeZaZadaću(idAktivnosti, trenutniKorisnik.PredmetiNaKojimPredaje[i], forma["naziv"], double.Parse(forma["maxBrojBodova"]), oDate);
                        break;
                    }
                }
            }
            return RedirectToAction("MojeAktivnosti");
        }


        [Route("/nastavno-osoblje/moje-aktivnosti")]
        public IActionResult MojeAktivnosti()
        {
            var trenutniKorisnik = Autentifikacija.GetNastavnoOsoblje(HttpContext);
            return View(trenutniKorisnik);
        }

        [Route("/nastavno-osoblje/osobe-list")]
        public IActionResult searchUsersForMessage()
        {
            var trenutniKorisnik = Autentifikacija.GetNastavnoOsoblje(HttpContext);
            return View(trenutniKorisnik);
        }

        [Route("/nastavno-osoblje/osobe-list/pretraga")]
        [HttpPost]
        public IActionResult searchUsersForMessageForm(IFormCollection forma)
        {
            var trenutniKorisnik = Autentifikacija.GetNastavnoOsoblje(HttpContext);
            List<Korisnik> korisnici = zmgr.pretražiKorisnike(forma["Ime"], forma["Prezime"]);
            ViewBag.korisnici = korisnici;
            return View(trenutniKorisnik);
        }


        [Route("/nastavno-osoblje/osobe-list/pretraga/{idPrimaoca}")]
        [HttpGet]
        public IActionResult teacherSendMessage(int idPrimaoca)
        {
            var trenutniKorisnik = Autentifikacija.GetNastavnoOsoblje(HttpContext);
            return View(trenutniKorisnik);
        }



        [Route("/nastavno-osoblje/osobe-list/pretraga/{idPrimaoca}")]
        [HttpPost]
        public IActionResult sendMessage(IFormCollection forma, int idPrimaoca)
        {
            var trenutniKorisnik = Autentifikacija.GetNastavnoOsoblje(HttpContext);
            if (forma != null)
            {
                string sadržaj = forma["sadržaj"];
                string naslov = forma["naslov"];
                Poruka poruka = new Poruka(trenutniKorisnik.IdOsobe.Value, idPrimaoca, naslov, sadržaj, DateTime.Now, 0, zmgr.dajNoviPorukaId());
                zmgr.posaljiPoruku(poruka);
                return RedirectToAction("mojOutbox");
            }
            return RedirectToAction("prikaziGresku", new { lokacija = "posalji-poruku", idPoruke = 2 });
        }

        [Route("/nastavno-osoblje/poruke/moj-inbox")]
        public IActionResult mojInbox(int id)
        {
            var trenutniKorisnik = Autentifikacija.GetNastavnoOsoblje(HttpContext);
            return View(trenutniKorisnik);
        }

        [Route("/nastavno-osoblje/poruke/moj-outbox")]
        public IActionResult mojOutbox(int id)
        {
            var trenutniKorisnik = Autentifikacija.GetNastavnoOsoblje(HttpContext);
            return View(trenutniKorisnik);
        }

        [Route("/nastavno-osoblje/poruke/moj-inbox/{idPoruke}")]
        public IActionResult detaljiPorukeInbox(int idPoruke)
        {
            var trenutniKorisnik = Autentifikacija.GetNastavnoOsoblje(HttpContext);
            ViewBag.poruka = zmgr.dajPoruku(idPoruke);
            zmgr.oznaciProcitanu(idPoruke);
            return View(trenutniKorisnik);
        }

        [Route("/nastavno-osoblje/poruke/moj-outbox/{idPoruke}")]
        public IActionResult detaljiPorukeOutbox(int idPoruke)
        {
            var trenutniKorisnik = Autentifikacija.GetNastavnoOsoblje(HttpContext);
            ViewBag.poruka = zmgr.dajPoruku(idPoruke);
            return View(trenutniKorisnik);
        }

        [Route("/nastavno-osoblje/edituj-aktivnost/{idAktivnosti}")]
        [HttpGet]
        public IActionResult editujAktivnost(int idAktivnosti)
        {
            var trenutniKorisnik = Autentifikacija.GetNastavnoOsoblje(HttpContext);
            Aktivnost aktivnost = zmgr.dajAktivnostPoId(idAktivnosti);
            if (aktivnost.GetType() == typeof(Ispit))
            {
                ViewBag.aktivnost = (Ispit)aktivnost;
            }
            else
            {
                ViewBag.aktivnost = (Zadaća)aktivnost;
            }
            return View(trenutniKorisnik);
        }

        [Route("/nastavno-osoblje/edituj-aktivnost/{idAktivnosti}")]
        [HttpPost]
        public IActionResult editujAktivnost(int idAktivnosti, IFormCollection forma)
        {
            if (forma != null)
            {
                DateTime oDate = Convert.ToDateTime(forma["datum"] + " " + forma["vrijeme"]);
                zmgr.editujAktivnost(idAktivnosti, forma["naziv"], oDate, int.Parse(forma["maxBrojBodova"]));
                return RedirectToAction("Dashboard");
            }
            return RedirectToAction("prikaziGresku", new { lokacija = "edituj-aktivnost", idPoruke = 3 });
        }

        [Route("/nastavno-osoblje/{lokacija}/greska/{idPoruke}")]
        public IActionResult prikaziGresku(string lokacija, int idPoruke)
        {
            if (idPoruke == 1)
            {
                ViewBag.poruka = "Greška pri kreiranju zahtjeva";
            }
            else if (idPoruke == 2)
            {
                ViewBag.poruka = "Greška pri slanju poruke";
            }
            else if (idPoruke == 3)
            {
                ViewBag.poruka = "Greška pri editovanju zahtjeva";
            }
            else if (idPoruke == 4)
            {
                ViewBag.poruka = "Nemate pravo pristupa ovoj stranici jer ne pripadate ansamblu traženog predmeta!";
            }
            else if (idPoruke == 5)
            {
                ViewBag.poruka = "Greška prilikom upisa ocjene za studenta!";
            }
            else if (idPoruke == 6)
            {
                ViewBag.poruka = "Tražena zadaća za traženog studenta na odabranom predmetu ne postoji!";
            }
            else if (idPoruke == 7)
            {
                ViewBag.poruka = "Morate popuniti formu!";
            }
            return View();
        }

        [Route("/nastavno-osoblje/moji-predmeti")]
        public IActionResult mojiPredmeti()
        {
            var trenutniKorisnik = Autentifikacija.GetNastavnoOsoblje(HttpContext);
            return View(trenutniKorisnik);
        }

        [Route("/nastavno-osoblje/detalji-o-predmetu/{idPredmeta}")]
        public IActionResult detaljiOPredmetu(int idPredmeta)
        {
            var trenutniKorisnik = Autentifikacija.GetNastavnoOsoblje(HttpContext);
            ViewBag.ansambl = zmgr.dajAnsamblNaPredmetu(idPredmeta);
            ViewBag.trazeniPredmet = idPredmeta;
            return View(trenutniKorisnik);
        }

        [Route("/nastavno-osoblje/detalji-o-zadaci/{idPredmeta}/{idZadaće}")]
        [HttpGet]
        public IActionResult detaljiOZadaći(int idPredmeta, int idZadaće)
        {
            var trenutniKorisnik = Autentifikacija.GetNastavnoOsoblje(HttpContext);
            ViewBag.idZadaće = idZadaće;
            ViewBag.idPredmeta = idPredmeta;
            return View(trenutniKorisnik);
        }

        [Route("/nastavno-osoblje/ocjeni-zadaću/{idZadaće}")]
        [HttpPost]
        public IActionResult OcjeniZadaću(int idZadaće, IFormCollection forma)
        {
            NumberFormatInfo format = new NumberFormatInfo();
            format.NumberDecimalSeparator = ".";
            foreach (String key in forma.Keys)
            {
                if (!String.IsNullOrEmpty(forma[key]))
                {
                    zmgr.updateBodoveZadaćeZaStudenta(idZadaće, int.Parse(key), float.Parse(forma[key], format));
                }
            }
            return RedirectToAction("mojiPredmeti");
        }


        [Route("/nastavno-osoblje/predmet/{idPredmeta}/svi-studenti")]
        public IActionResult studentiNaPredmetu(int idPredmeta)
        {
            ViewBag.trazeniPredmet = idPredmeta;
            var trenutniKorisnik = Autentifikacija.GetNastavnoOsoblje(HttpContext);
            return View(trenutniKorisnik);
        }

        [Route("/nastavno-osoblje/predmet/detalji-o-studentu/{idPredmeta}/{idStudenta}")]
        public IActionResult detaljiOStudentuNaPredmetu(int idPredmeta, int idStudenta)
        {
            var trenutniKorisnik = Autentifikacija.GetNastavnoOsoblje(HttpContext);
            foreach (PredmetZaNastavnoOsoblje p in trenutniKorisnik.PredmetiNaKojimPredaje)
            {
                if (p.IdPredmeta == idPredmeta)
                {
                    ViewBag.trazeniPredmet = p;
                    ViewBag.ansambl = zmgr.dajAnsamblNaPredmetu(idPredmeta);
                    ViewBag.predmet = zmgr.dajPredmetZaStudentaPoID(idStudenta, idPredmeta, DateTime.Now.Year);
                    KreatorKorisnika creator = new KreatorKorisnika();
                    Korisnik tempK = creator.FactoryMethod(idStudenta);

                    if (tempK.GetType() == typeof(Student))
                    {
                        ViewBag.trazeniStudent = (Student)tempK;
                    }
                    else
                    {
                        ViewBag.trazeniStudent = (MasterStudent)tempK;
                    }
                    return View(trenutniKorisnik);
                }
            }
            return RedirectToAction("prikaziGresku", new { lokacija = "nastavno-osoblje/predmet/detalji-o-studentu", idPoruke = 4 });
        }

        [Route("/nastavno-osoblje/predmet/{idPredmeta}/ocjeni/{idStudenta}")]
        [HttpPost]
        public IActionResult ocjeniStudenta(int idPredmeta, int idStudenta, IFormCollection forma)
        {
            if (!String.IsNullOrEmpty(forma["ocjena"]))
            {
                zmgr.updateOrInsertOcjenuZaStudenta(idPredmeta, idStudenta, int.Parse(forma["ocjena"]));
                return RedirectToAction("detaljiOStudentuNaPredmetu", new { idPredmeta = idPredmeta, idStudenta = idStudenta });
            }

            return RedirectToAction("prikaziGresku", new { lokacija = "predmet/upisi-ocjenu", idPoruke = 5 });

        }

        [Route("/nastavno-osoblje/student-rjesenje-zadace/{idPredmeta}/{idZadaće}/{idStudenta}")]
        public IActionResult studentovaZadaćaInfo(int idPredmeta, int idZadaće, int idStudenta)
        {
            var trenutniKorisnik = Autentifikacija.GetNastavnoOsoblje(HttpContext);
            KreatorKorisnika creator = new KreatorKorisnika();
            Korisnik tempK = creator.FactoryMethod(idStudenta);

            if (tempK.GetType() == typeof(Student))
            {
                ViewBag.trazeniStudent = (Student)tempK;
            }
            else
            {
                ViewBag.trazeniStudent = (MasterStudent)tempK;
            }
            foreach (PredmetZaStudenta p in ((Student)tempK).Predmeti)
            {
                if (p.IdPredmeta == idPredmeta)
                {
                    ViewBag.trazeniPredmet = p;
                    foreach (Aktivnost akt in p.Aktivnosti)
                    {
                        if (akt.IdAktivnosti == idZadaće)
                        {
                            ViewBag.trazenaZadaca = (Zadaća)akt;
                            return View(trenutniKorisnik);
                        }
                    }
                }
            }

            return RedirectToAction("prikaziGresku", new { lokacija = "zadaca-za-studenta", idPoruke = 6 });
        }


        [Route("/nastavno-osoblje/zadaca/boduj-zadacu-za-studenta/{idPredmeta}/{idZadaće}/{idStudenta}")]
        [HttpPost]
        public IActionResult bodujZadaćuStudentu(int idPredmeta, int idZadaće, int idStudenta, IFormCollection forma)
        {
            if (!String.IsNullOrEmpty(forma["bodovi"]))
            {
                NumberFormatInfo format = new NumberFormatInfo();
                format.NumberDecimalSeparator = ".";
                zmgr.updateBodoveZadaćeZaStudenta(idZadaće, idStudenta, float.Parse(forma["bodovi"], format));
                return RedirectToAction("detaljiOStudentuNaPredmetu", new { idPredmeta = idPredmeta, idStudenta = idStudenta });
            }
            return RedirectToAction("prikaziGresku", new { lokacija = "boduj-zadacu-za-studenta", idPoruke = 7 });
        }


        [Route("/nastavno-osoblje/detalji-o-ispitu/{idPredmeta}/{idIspita}")]
        [HttpGet]
        public IActionResult detaljiOIspitu(int idPredmeta, int idIspita)
        {
            var trenutniKorisnik = Autentifikacija.GetNastavnoOsoblje(HttpContext);
            foreach(PredmetZaNastavnoOsoblje p in trenutniKorisnik.PredmetiNaKojimPredaje)
            {
                if(p.IdPredmeta==idPredmeta)
                {
                    ViewBag.idIspita = idIspita;
                    ViewBag.idPredmeta = idPredmeta;
                    ViewBag.brojPrijavljenih = zmgr.dajBrojPrijavljenihNaIspit(idIspita);
                    return View(trenutniKorisnik);
                }
            }

            return RedirectToAction("pristupOdbijen", new RouteValueDictionary(new { controller = "Početni", action = "pristupOdbijen" }));

        }

        [Route("/nastavno-osoblje/ocjeni-ispit/{idIspita}")]
        [HttpPost]
        public IActionResult ocjeniIspit(int idIspita, IFormCollection forma)
        {
            NumberFormatInfo format = new NumberFormatInfo();
            format.NumberDecimalSeparator = ".";
            foreach (String key in forma.Keys)
            {
                if (!String.IsNullOrEmpty(forma[key]))
                {
                    zmgr.updateBodoveIspitaZaStudenta(idIspita, int.Parse(key), float.Parse(forma[key], format));
                }

            }
            return RedirectToAction("mojiPredmeti");
        }



        [Route("/nastavno-osoblje/student-detalji-o-ispitu/{idPredmeta}/{idStudenta}/{idIspita}")]
        [HttpGet]
        public IActionResult infoOStudentovomIspitu(int idPredmeta, int idStudenta, int idIspita)
        {
            var trenutniKorisnik = Autentifikacija.GetNastavnoOsoblje(HttpContext);
            foreach(PredmetZaNastavnoOsoblje p in trenutniKorisnik.PredmetiNaKojimPredaje)
            {
                if(p.IdPredmeta==idPredmeta)
                {
                    foreach(Student s in p.Studenti)
                    {
                        if(s.BrojIndeksa.Value==idStudenta)
                        {
                            ViewBag.trazeniStudent = s;
                            foreach(PredmetZaStudenta prdmt in s.Predmeti)
                            {
                                if(prdmt.IdPredmeta==idPredmeta)
                                {
                                    ViewBag.trazeniPredmet = prdmt;
                                    foreach(Aktivnost akt in prdmt.Aktivnosti)
                                    {
                                        if(akt.IdAktivnosti==idIspita)
                                        {
                                            ViewBag.trazeniIspit = (Ispit)akt;
                                            return View(trenutniKorisnik);
                                        }
                                    }
                                }     
                            }
                        }
                    }
                }
            }
            return RedirectToAction("pristupOdbijen", new RouteValueDictionary(new { controller = "Početni", action = "pristupOdbijen" }));
        }


        [Route("/nastavno-osoblje/ispit/boduj-ispit-za-studenta/{idPredmeta}/{idIspita}/{idStudenta}")]
        public IActionResult bodujIspitZaStudenta(int idPredmeta, int idIspita, int idStudenta, IFormCollection forma)
        {
            if (!String.IsNullOrEmpty(forma["bodovi"]))
            {
                NumberFormatInfo format = new NumberFormatInfo();
                format.NumberDecimalSeparator = ".";
                zmgr.updateBodoveIspitaZaStudenta(idIspita, idStudenta, float.Parse(forma["bodovi"], format));
                return RedirectToAction("detaljiOStudentuNaPredmetu", new { idPredmeta = idPredmeta, idStudenta = idStudenta });
            }
            return RedirectToAction("prikaziGresku", new { lokacija = "boduj-ispit-za-studenta", idPoruke = 7 });
        }
        [Route("/nastavno-osoblje/moj-profil")]
        public IActionResult mojProfil()
        {
            var trenutniKorisnik = Autentifikacija.GetNastavnoOsoblje(HttpContext);

            ViewBag.listaObavjestenja = zmgr.dajSvaObavještenja();

            return View(trenutniKorisnik);
        }

        [Route("/nastavno-osoblje/moj-profil")]
        [HttpPost]
        public IActionResult mojProfil(IFormCollection forma)
        {
            var trenutniKorisnik = Autentifikacija.GetNastavnoOsoblje(HttpContext); 

            ViewBag.listaObavjestenja = zmgr.dajSvaObavještenja();
            Logger logg = Logger.GetInstance();
            try
            {
                logg.promijeniPasswordKorisniku((int)trenutniKorisnik.IdOsobe, forma["password"]);
            }
            catch (Exception e)
            {

            }

            return View(trenutniKorisnik);
        }

        [Route("/nastavno-osoblje/moje-ankete")]
        [HttpGet]
        public IActionResult mojeAnkete()
        {
            var trenutniKorisnik = Autentifikacija.GetNastavnoOsoblje(HttpContext);
            if(trenutniKorisnik.GetType() != typeof(Profesor))
            {
                return RedirectToAction("pristupOdbijen", new RouteValueDictionary(new { controller = "Početni", action = "pristupOdbijen" }));
            }
            return View(trenutniKorisnik);
        }


        [Route("/nastavno-osoblje/kreiraj-anketu")]
        [HttpGet]
        public IActionResult kreirajAnketu()
        {
            var trenutniKorisnik = Autentifikacija.GetNastavnoOsoblje(HttpContext);
            if (trenutniKorisnik.GetType() != typeof(Profesor))
            {
                return RedirectToAction("pristupOdbijen", new RouteValueDictionary(new { controller = "Početni", action = "pristupOdbijen" }));
            }
            return View(trenutniKorisnik);
        }


        [Route("/nastavno-osoblje/napravi-anketu")]
        public IActionResult napraviAnketu(IFormCollection forma)
        {
            DateTime oDate = Convert.ToDateTime(forma["datum"] + " " + forma["vrijeme"]);
            zmgr.kreirajAnketu(int.Parse(forma["izabraniPredmet"]), forma["nazivAnkete"], oDate, forma["pitanje1"], forma["pitanje2"], forma["pitanje3"], forma["pitanje4"], forma["pitanje5"], 5);
            return RedirectToAction("mojeAnkete");
        }

        [Route("/nastavno-osoblje/rezultati-ankete/{idAnkete}")]
        public IActionResult rezultatiAnkete(int idAnkete)
        {
            var trenutniKorisnik = Autentifikacija.GetNastavnoOsoblje(HttpContext);
            if(trenutniKorisnik.GetType() == typeof(Profesor))
            {
                if (((Profesor)trenutniKorisnik).AnketeNaPredmetima != null)
                {
                    foreach (Anketa an in ((Profesor)trenutniKorisnik).AnketeNaPredmetima)
                    {
                        if(an.IdAnkete==idAnkete)
                        {
                            ViewBag.nazivPredmeta = zmgr.dajNazivPredmetaPoId(an.IdPredmeta);
                            ViewBag.trazenaAnketa = zmgr.dajAnketu(idAnkete);
                            return View(((Profesor)trenutniKorisnik));
                        }
                    }
                }
              
            }

            return RedirectToAction("pristupOdbijen", new RouteValueDictionary(new { controller = "Početni", action = "pristupOdbijen" }));

        }




    }
}