using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting.Internal;
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
            ViewBag.listaObavjestenja = zmgr.dajSvaObavještenja();
            return View(trenutniKorisnik);
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
            ViewBag.poruka = zmgr.dajPoruku(idPoruke);
            zmgr.oznaciProcitanu(idPoruke);
            return View(trenutniKorisnik);
        }

        [Route("/student/poruke/moj-outbox/{idPoruke}")]
        public IActionResult detaljiPorukeOutbox(int idPoruke)
        {
            var trenutniKorisnik = Autentifikacija.GetLogiraniStudent(HttpContext);
            ViewBag.poruka = zmgr.dajPoruku(idPoruke);
            return View(trenutniKorisnik);
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
            List<Korisnik> korisnici = zmgr.pretražiKorisnike(forma["Ime"], forma["Prezime"]);
            ViewBag.korisnici = korisnici;
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
            var trenutniKorisnik = Autentifikacija.GetLogiraniStudent(HttpContext);
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
            return View();
        }

        [Route("/student/zadaca-info/{idZadaće}/{idPredmeta}")]
        [HttpGet]
        public IActionResult infoOZadaći(int idZadaće, int idPredmeta)
        {
            var trenutniKorisnik = Autentifikacija.GetLogiraniStudent(HttpContext);
            foreach(PredmetZaStudenta p in trenutniKorisnik.Predmeti)
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
                    foreach (PredmetZaStudenta p in trenutniKorisnik.Predmeti)
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


    }
}