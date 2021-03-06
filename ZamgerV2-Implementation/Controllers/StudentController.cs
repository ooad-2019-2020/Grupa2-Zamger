﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Hosting.Internal;
using Newtonsoft.Json;
using ZamgerV2_Implementation.Helpers;
using ZamgerV2_Implementation.Models;

namespace ZamgerV2_Implementation.Controllers
{
    [Autorizacija(false, TipKorisnika.Student)]
    public class StudentController : Controller
    {
        private ZamgerDbContext zmgr;
        private readonly IWebHostEnvironment hostingEnvironment;

        public StudentController(IWebHostEnvironment hostingEnvironment)
        {
            zmgr = ZamgerDbContext.GetInstance();
            this.hostingEnvironment = hostingEnvironment;
        }

        [Route("/student/dashboard")]
        public IActionResult Dashboard()
        {
            var trenutniKorisnik = Autentifikacija.GetLogiraniStudent(HttpContext);

            ViewBag.polozeni = zmgr.dajBrojPoloženihPredmeta(trenutniKorisnik.BrojIndeksa);
            ViewBag.nepolozeni = zmgr.dajBrojNepoloženihPredmeta(trenutniKorisnik.BrojIndeksa);
            ViewBag.listaObavjestenja = zmgr.dajSvaObavještenja();
            ViewBag.listaNePrijavljenihIspita = zmgr.dajIspiteNaKojeSeStudentNijePrijavio(trenutniKorisnik.BrojIndeksa.Value);
            ViewBag.prosjek = zmgr.dajProsjekPoID(trenutniKorisnik.BrojIndeksa.Value);

            return View(trenutniKorisnik);
        }

        [Route("/student/kreiraj-zahtjev/")]
        [HttpGet]
        public IActionResult KreirajZahtjev()
        {
            var trenutniKorisnik = Autentifikacija.GetLogiraniStudent(HttpContext);
            return View(trenutniKorisnik);
        }

        [Route("/student/kreiraj-zahtjev/")]
        [HttpPost]
        public IActionResult KreirajZahtjev(IFormCollection forma)
        {
            int idZaht = zmgr.generišiIdZahtjeva();
            if (zmgr.spremiZahtjev(new Zahtjev(Autentifikacija.GetIdKorisnika(HttpContext).Value, forma["VrstaZahtjeva"].ToString(), DateTime.Now, 0, idZaht)))
            {
                return RedirectToAction("UspješnoKreiranZahtjev");
            }
            else
            {
                //ovdje treba neki error view vratit 404 il nešta
                return RedirectToAction("prikaziGresku", new {lokacija = "kreiraj-zahtjev", idPoruke = 1 });
            }
        }


        [Route("/student/uspjesno-poslan-zahtjev")]
        [HttpGet]
        public IActionResult UspješnoKreiranZahtjev()
        {
            var trenutniKorisnik = Autentifikacija.GetLogiraniStudent(HttpContext);
            return View(trenutniKorisnik);
        }

        [Route("/student/moji-zahtjevi")]
        [HttpGet]
        public IActionResult MojiZahtjevi(int id)
        {
            var trenutniKorisnik = Autentifikacija.GetLogiraniStudent(HttpContext);
            ViewBag.mojiZahtjevi = zmgr.dajZahtjeveZaStudenta(trenutniKorisnik.BrojIndeksa.Value);
            return View(trenutniKorisnik);
        }

        [Route("/student/sva-obavještenja-list")]
        public IActionResult AllStudentAnnouncementsList(int id)
        {
            var trenutniKorisnik = Autentifikacija.GetLogiraniStudent(HttpContext);
            var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:64580/zamger-api/");
            var responseTask = client.GetAsync("sva-obavjestenja");
            responseTask.Wait();

            var resultDisplay = responseTask.Result;
            if (resultDisplay.IsSuccessStatusCode)
            {
                var odgovor = resultDisplay.Content.ReadAsStringAsync();
                odgovor.Wait();
                ViewBag.listaObavjestenja = JsonConvert.DeserializeObject<List<Obavještenje>>(odgovor.Result);
                return View(trenutniKorisnik);
            }

            return RedirectToAction("prikaziGresku", new { lokacija = "sva-obavjestenja", idPoruke = 7 });

 
        }


        [Route("/student/obavještenje/{idObavještenja}")]
        public IActionResult AnnouncementStudentInfo(int idObavještenja)
        {
            var trenutniKorisnik = Autentifikacija.GetLogiraniStudent(HttpContext);
            ViewBag.obavještenje = zmgr.dajObavještenjePoId(idObavještenja);
            return View(trenutniKorisnik);
        }

        [Route("/student/predmet/{idPredmeta}/{studijskaGodina}")]
        public IActionResult StudentSubjectInfo(int idPredmeta, int studijskaGodina)
        {
            var trenutniKorisnik = Autentifikacija.GetLogiraniStudent(HttpContext);
            ViewBag.predmet = zmgr.dajPredmetZaStudentaPoID(trenutniKorisnik.BrojIndeksa.Value, idPredmeta, studijskaGodina);
            ViewBag.ansambl = zmgr.dajAnsamblNaPredmetu(idPredmeta);
            return View(trenutniKorisnik);
        }


        [Route("/student/predmeti-list")]
        public IActionResult MySubjects()
        {
            var trenutniKorisnik = Autentifikacija.GetLogiraniStudent(HttpContext);
            ViewBag.listaPredmeta = trenutniKorisnik.Predmeti;
            return View(trenutniKorisnik);
        }

        [Route("/student/poruke/moj-inbox")]
        public IActionResult mojInbox()
        {
            var trenutniKorisnik = Autentifikacija.GetLogiraniStudent(HttpContext);
            return View(trenutniKorisnik);
        }
        [Route("/student/poruke/moj-outbox")]
        public IActionResult mojOutbox()
        {
            var trenutniKorisnik = Autentifikacija.GetLogiraniStudent(HttpContext);
            return View(trenutniKorisnik);
        }

        [Route("/student/poruke/moj-inbox/{idPoruke}")]
        public IActionResult detaljiPorukeInbox(int idPoruke)
        {
            var trenutniKorisnik = Autentifikacija.GetLogiraniStudent(HttpContext);
            foreach(Poruka p in trenutniKorisnik.Inbox)
            {
                if(p.IdPoruke==idPoruke)
                {
                    ViewBag.poruka = zmgr.dajPoruku(idPoruke);
                    zmgr.oznaciProcitanu(idPoruke);
                    return View(trenutniKorisnik);
                }
            }
            return RedirectToAction("pristupOdbijen", new RouteValueDictionary(new { controller = "Početni", action = "pristupOdbijen" }));

        }

        [Route("/student/poruke/moj-outbox/{idPoruke}")]
        public IActionResult detaljiPorukeOutbox(int idPoruke)
        {
            var trenutniKorisnik = Autentifikacija.GetLogiraniStudent(HttpContext);
            foreach(Poruka p in trenutniKorisnik.Outbox)
            {
                ViewBag.poruka = zmgr.dajPoruku(idPoruke);
                return View(trenutniKorisnik);
            }
            return RedirectToAction("pristupOdbijen", new RouteValueDictionary(new { controller = "Početni", action = "pristupOdbijen" }));
        }

        [Route("/student/studenti-list")]
        public IActionResult searchStudentsForMessage()
        {
            var trenutniKorisnik = Autentifikacija.GetLogiraniStudent(HttpContext);
            return View(trenutniKorisnik);
        }

        [Route("/student/studenti-list/pretraga")]
        [HttpPost]
        public IActionResult searchStudentsForMessageForm(IFormCollection forma)
        {
            var trenutniKorisnik = Autentifikacija.GetLogiraniStudent(HttpContext);
            List<Korisnik> korisnici;
            try
            {
                korisnici = zmgr.pretražiKorisnike(forma["Ime"], forma["Prezime"]);
                ViewBag.korisnici = korisnici;
            }
            catch
            {
                ViewBag.korisnici = new List < Korisnik >();
            }
            return View(trenutniKorisnik);
        }


        [Route("/student/studenti-list/pretraga/{idPrimaoca}")]
        [HttpGet]
        public IActionResult sendMessage(int idPrimaoca)
        {
            var trenutniKorisnik = Autentifikacija.GetLogiraniStudent(HttpContext);
            return View(trenutniKorisnik);
        }



        [Route("/student/studenti-list/pretraga/{idPrimaoca}")]
        [HttpPost]
        public IActionResult sendMessage(IFormCollection forma, int idPrimaoca)
        {
            var trenutniKorisnik = Autentifikacija.GetLogiraniStudent(HttpContext);
            if (forma != null)
            {
                string sadržaj = forma["sadržaj"];
                string naslov = forma["naslov"];
                Poruka poruka = new Poruka(trenutniKorisnik.BrojIndeksa.Value, idPrimaoca, naslov, sadržaj, DateTime.Now, 0, zmgr.dajNoviPorukaId());
                zmgr.posaljiPoruku(poruka);
                return RedirectToAction("mojOutbox");
            }

            return RedirectToAction("prikaziGresku", new { lokacija = "studenti-list/pretraga/" + idPrimaoca, idPoruke = 2 });
        }

        [Route("/student/{lokacija}/greska/{idPoruke}")]
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
                ViewBag.poruka = "Ne pohađate kurs za koji zahtjevate zadaću ili tražena zadaća ne postoji!";
            }
            else if (idPoruke == 4)
            {
                ViewBag.poruka = "Zadaća nije u .pdf formatu!";
            }
            else if (idPoruke == 5)
            {
                ViewBag.poruka = "Morate odabrati fajl za upload!";
            }
            else if (idPoruke == 6)
            {
                ViewBag.poruka = "Ne mozete se prijaviti na ovaj ispit";
            }
            else if (idPoruke == 7)
            {
                ViewBag.poruka = "greška prilikom pozivanja API za obavještenja";
            }
            return View();
        }

        [Route("/student/zadaca-info/{idZadaće}/{idPredmeta}")]
        [HttpGet]
        public IActionResult infoOZadaći(int idZadaće, int idPredmeta)
        {
            var trenutniKorisnik = Autentifikacija.GetLogiraniStudent(HttpContext);
            foreach(PredmetZaStudenta p in trenutniKorisnik) //iterator pattern
            {
                if(p.IdPredmeta==idPredmeta)
                {
                    ViewBag.trazeniPredmet = p;
                    foreach(Aktivnost akt in p.Aktivnosti)
                    {
                        if(akt.IdAktivnosti==idZadaće)
                        {
                            ViewBag.trazenaZadaca =(Zadaća)akt;
                            return View(trenutniKorisnik);
                        }
                    }
                }
            }
            return RedirectToAction("prikaziGresku", new { lokacija = "zadaca-ne-postoji", idPoruke = 3});
        }

        [Route("/posalji-zadacu/{idPredmeta}/{idZadaće}")]
        [HttpPost]
        public IActionResult pošaljiZadaću(int idPredmeta, int idZadaće, IFormFile rjesenje)
        {
            var trenutniKorisnik = Autentifikacija.GetLogiraniStudent(HttpContext);
          
            var aktivnost = zmgr.dajAktivnostPoId(idZadaće);
            if (rjesenje!=null)
            {
                if(rjesenje.ContentType.Equals("application/pdf"))
                {
                    foreach (PredmetZaStudenta p in trenutniKorisnik) //iterator pattern
                    {
                        if (p.IdPredmeta == idPredmeta)
                        {
                            foreach (Aktivnost akt in p.Aktivnosti)
                            {
                                if (akt.IdAktivnosti == idZadaće && !String.IsNullOrEmpty(((Zadaća)akt).PutanjaDoZadaće))
                                {
                                    string putanjaZaBrisanje = Path.Combine(hostingEnvironment.WebRootPath, "zadace", ((Zadaća)akt).PutanjaDoZadaće);
                                    FileInfo fi = new FileInfo(putanjaZaBrisanje);
                                    if(fi!=null)
                                    {
                                        System.IO.File.Delete(putanjaZaBrisanje);
                                    }
                                    break;
                                }
                            }
                            break;
                        }
                    }
                    string nazivFajla = trenutniKorisnik.Prezime + "-" + trenutniKorisnik.Ime + "-" + trenutniKorisnik.BrojIndeksa + "-" + idZadaće+"-"+aktivnost.Naziv+"-"+Guid.NewGuid().ToString()+".pdf";
                    string uploadsFolderPath = Path.Combine(hostingEnvironment.WebRootPath, "zadace");
                    string fullPutanja = Path.Combine(uploadsFolderPath, nazivFajla);
                    rjesenje.CopyToAsync(new FileStream(fullPutanja, FileMode.Create));
                    zmgr.uploadujZadacu(idPredmeta, idZadaće, trenutniKorisnik.BrojIndeksa, nazivFajla);
                    return RedirectToAction("infoOZadaći", new { idZadaće=idZadaće, idPredmeta=idPredmeta});
                }
                else
                {
                    return RedirectToAction("prikaziGresku", new { lokacija = "posalji-zadacu", idPoruke = 4 });
                }
                
            }
            return RedirectToAction("prikaziGresku", new { lokacija = "posalji-zadacu", idPoruke = 5 });
        }


        [Route("/student/ispit-info/{idIspita}/{idPredmeta}")]
        [HttpGet]
        public IActionResult infoOIspitu(int idIspita, int idPredmeta)
        {
            var trenutniKorisnik = Autentifikacija.GetLogiraniStudent(HttpContext); 

                foreach(PredmetZaStudenta p in trenutniKorisnik) //iterator pattern
                {
                    if(p.IdPredmeta == idPredmeta)
                    {
                        ViewBag.trazeniPredmet = p;
                        if (zmgr.daLiJePrijavljenNaIspit(trenutniKorisnik.BrojIndeksa.Value, idIspita))
                        {
                            foreach (Aktivnost akt in p.Aktivnosti)
                            {
                                if (akt.IdAktivnosti == idIspita)
                                {
                                    ViewBag.trazeniIspit = (Ispit)akt;
                                    ViewBag.daLiJePrijavljen = true;
                                    return View(trenutniKorisnik);
                                }
                            }
                           break;
                        }
                        else
                        {

                            ViewBag.trazeniIspit = (Ispit)zmgr.dajAktivnostPoId(idIspita);
                            ViewBag.daLiJePrijavljen = false;
                            return View(trenutniKorisnik);

                        }
                    }
                }
            return RedirectToAction("pristupOdbijen", new RouteValueDictionary(new { controller = "Početni", action = "pristupOdbijen" }));
        }



        [Route("/student/ispit/{idIspita}/prijavi-se")]
        [HttpPost]
        public IActionResult prijaviSeNaIspit(int idIspita)
        {
            var trenutniKorisnik = Autentifikacija.GetLogiraniStudent(HttpContext);
            var tempIspit =(Ispit)zmgr.dajAktivnostPoId(idIspita);
            foreach(PredmetZaStudenta p in trenutniKorisnik.Predmeti)
            {
                if(p.IdPredmeta==tempIspit.IdPredmeta)
                {
                    zmgr.prijaviStudentaNaIspit(trenutniKorisnik.BrojIndeksa.Value, tempIspit);
                    return RedirectToAction("Dashboard");
                }
            }

            return RedirectToAction("prikaziGresku", new { lokacija = "prijavi-ispit", idPoruke = 6 });

        }


        [Route("/student/ispit/{idIspita}/odjavi-se")]
        [HttpPost]
        public IActionResult odjaviSeSaIspita(int idIspita)
        {
            var trenutniKorisnik = Autentifikacija.GetLogiraniStudent(HttpContext);
            var tempIspit = zmgr.dajAktivnostPoId(idIspita);
            foreach (PredmetZaStudenta p in trenutniKorisnik) //iterator pattern
            {
                if (p.IdPredmeta == tempIspit.IdPredmeta)
                {
                    foreach(Aktivnost akt in p.Aktivnosti)
                    {
                        if(akt.IdAktivnosti == idIspita)
                        {
                            zmgr.odjaviStudentaSaIspita(trenutniKorisnik.BrojIndeksa.Value, idIspita);
                            return RedirectToAction("Dashboard");
                        }
                    }
                }
            }

            return RedirectToAction("prikaziGresku", new { lokacija = "prijavi-ispit", idPoruke = 6 });

        }


        [Route("/student/nadolazece-aktivnosti")]
        public IActionResult nadolazećiIspiti()
        {
            var trenutniKorisnik = Autentifikacija.GetLogiraniStudent(HttpContext);
            return View(trenutniKorisnik);
        }


        [Route("/student/moj-profil")]
        public IActionResult mojProfil()
        {
            var trenutniKorisnik = Autentifikacija.GetLogiraniStudent(HttpContext);

            ViewBag.polozeni = zmgr.dajBrojPoloženihPredmeta(trenutniKorisnik.BrojIndeksa);
            ViewBag.nepolozeni = zmgr.dajBrojNepoloženihPredmeta(trenutniKorisnik.BrojIndeksa);
            ViewBag.prosjek = zmgr.dajProsjekPoID(trenutniKorisnik.BrojIndeksa.Value);
            Logger logg = Logger.GetInstance();
            ViewBag.sifra = logg.dajPasswordPoId((int)trenutniKorisnik.BrojIndeksa);
            Logger.removeInstance();

            return View(trenutniKorisnik);
        }

        [Route("/student/moj-profil")]
        [HttpPost]
        public IActionResult mojProfil(IFormCollection forma)
        {
            var trenutniKorisnik = Autentifikacija.GetLogiraniStudent(HttpContext);

            ViewBag.polozeni = zmgr.dajBrojPoloženihPredmeta(trenutniKorisnik.BrojIndeksa);
            ViewBag.nepolozeni = zmgr.dajBrojNepoloženihPredmeta(trenutniKorisnik.BrojIndeksa);
            ViewBag.prosjek = zmgr.dajProsjekPoID(trenutniKorisnik.BrojIndeksa.Value);
            Logger logg = Logger.GetInstance();
            
            try
            {
                ViewBag.sifra = logg.dajPasswordPoId((int)trenutniKorisnik.BrojIndeksa);
                logg.promijeniPasswordKorisniku((int)trenutniKorisnik.BrojIndeksa, forma["password"]);
                ViewBag.sifra = logg.dajPasswordPoId((int)trenutniKorisnik.BrojIndeksa);

            }
            catch (Exception e)
            {

            }
            Logger.removeInstance();
            return View(trenutniKorisnik);
        }

        [Route("/student/aktivne-ankete")]
        [HttpGet]
        public IActionResult aktivneAnkete()
        {
            var trenutniKorisnik = Autentifikacija.GetLogiraniStudent(HttpContext);
            List<Anketa> aktivneAnkete = new List<Anketa>();
            foreach(PredmetZaStudenta p in trenutniKorisnik) //iterator pattern
            {
                List<int> ideviAnketa = zmgr.dajIdeveAktivnihAnketaZaPredmet(p.IdPredmeta);
                if(ideviAnketa!=null)
                {
                    foreach (int br in ideviAnketa)
                    {
                        if (!zmgr.daLiJeAnketaVecPopunjena(trenutniKorisnik.BrojIndeksa.Value, br))
                        {
                            aktivneAnkete.Add(zmgr.dajAnketu(br));
                        }
                    }
                }

            }
            ViewBag.ankete = aktivneAnkete;
            return View(trenutniKorisnik);
        }

        [Route("/student/popuni-anketu/{idPredmeta}/{idAnkete}")]
        [HttpGet]
        public IActionResult popuniAnketu(int idPredmeta, int idAnkete)
        {
            var trenutniKorisnik = Autentifikacija.GetLogiraniStudent(HttpContext);
            foreach(PredmetZaStudenta p in trenutniKorisnik) //iterator pattern
            {
                if(p.IdPredmeta == idPredmeta)
                {
                    ViewBag.trenutnaAnketa = zmgr.dajAnketu(idAnkete);
                    return View(trenutniKorisnik);
                }
            }

            return RedirectToAction("pristupOdbijen", new RouteValueDictionary(new { controller = "Početni", action = "pristupOdbijen" }));

        }

        [Route("/student/popuni-anketu/{idPredmeta}/{idAnkete}")]
        [HttpPost]
        public IActionResult popuniAnketu(int idPredmeta, int idAnkete, IFormCollection forma)
        {
            var trenutniKorisnik = Autentifikacija.GetLogiraniStudent(HttpContext);
            List<string> odgovori = new List<string>();
            odgovori.Add(forma["odg1"]); odgovori.Add(forma["odg2"]); odgovori.Add(forma["odg3"]); odgovori.Add(forma["odg4"]); odgovori.Add(forma["odg5"]);
            zmgr.popuniAnketu(idAnkete, trenutniKorisnik.BrojIndeksa.Value, odgovori, forma["komentar"], int.Parse(forma["ocjena"]));
            return RedirectToAction("Dashboard");
        }

    }
}