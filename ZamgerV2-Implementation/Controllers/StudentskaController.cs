using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ZamgerV2_Implementation.Helpers;
using ZamgerV2_Implementation.Models;



namespace ZamgerV2_Implementation.Controllers
{
    [Autorizacija(false, TipKorisnika.StudentskaSluzba)]
    public class StudentskaController : Controller
    {
        private Logger logg;
        public StudentskaController()
        {
            logg = Logger.GetInstance();
        }

        [Route("/studentska/dashboard")]
        public IActionResult Dashboard()
        {
            ViewBag.zahtjevi = logg.dajSveNeobrađeneZahtjeve();
            ViewBag.ukupanBrojOsoba = logg.dajUkupanBrojOsobaNaSistemu();
            ViewBag.ukupanBrojStudenata = logg.dajUkupanBrojStudenataNaSistemu();
            ViewBag.ukupanBrojNastavnogOsoblja = logg.dajUkupanBrojNastavnogOsobljaNaSistemu();
            ViewBag.uposlenici = logg.pretražiNastavnoOsoblje(null, null, "Izaberite");
            ViewBag.zahtjevi = logg.dajSveNeobrađeneZahtjeve();
            ViewBag.nadolazeciIspiti = logg.dajInfoONadolazecimIspitima();
            return View();
        }

        [Route("/studentska/svi-studenti")]
        public IActionResult AllStudents()
        {
            ViewBag.zahtjevi = logg.dajSveNeobrađeneZahtjeve();
            return View();
        }

        [Route("/studentska/svi-studenti/kreiraj-studenta")]
        [Route("/studentska/kreiraj-studenta")]
        [HttpGet]
        public IActionResult KreirajStudenta()
        {
            ViewBag.zahtjevi = logg.dajSveNeobrađeneZahtjeve();
            return View();
        }


        [Route("/studentska/svi-studenti/kreiraj-studenta")]
        [Route("/studentska/kreiraj-studenta")]
        [HttpPost]
        public IActionResult KreirajStudenta(IFormCollection forma)
        {
            NumberFormatInfo format = new NumberFormatInfo();
            format.NumberDecimalSeparator = ".";
            if (forma["BScInfo"].Equals("3")) //ako osoba nije završila BSc tada se upisuje kao običan a ne MasterStudent -> nema potrebe za ekvivalentiranjem
            {
                if (!forma["izabraniSmjer"].Equals("Izaberite smjer")) 
                { 
                    Student noviStudent = new Student(forma["ime"], forma["prezime"], forma["datumRodjenja"], forma["prebivaliste"], null, null, forma["izabraniSpol"], forma["izabraniSmjer"], null);
                    if (validirajStudenta(noviStudent))
                    {
                        logg.generišiKorisničkePodatke(noviStudent);
                        return RedirectToAction("UspješnoKreiranStudent", new { id = noviStudent.BrojIndeksa });
                    }
                    else
                    {
                        return RedirectToAction("prikaziGresku", new { lokacija = "kreiraj-studenta", idPoruke=1 });
                    }
                }

                else
                {
                    return RedirectToAction("prikaziGresku", new { lokacija = "kreiraj-studenta", idPoruke = 1 });
                }
            }
            else if (forma["BScInfo"].Equals("1")) // osoba je završila BSc studij na ovom fakultetu
            {
                if (!forma["izabraniSmjer"].Equals("Izaberite smjer"))
                {
                    MasterStudent noviStudent = new MasterStudent(forma["ime"], forma["prezime"], forma["datumRodjenja"], forma["prebivaliste"], null, null, forma["izabraniSpol"], forma["izabraniSmjer"], null, double.Parse(forma["prosjekBSC"], format));
                    if (validirajStudenta(noviStudent))
                    {
                        logg.generišiKorisničkePodatke(noviStudent);
                        return RedirectToAction("UspješnoKreiranStudent", new { id = noviStudent.BrojIndeksa });
                    }
                    else
                    {
                        return RedirectToAction("prikaziGresku", new { lokacija = "kreiraj-studenta", idPoruke = 1 });
                    }
                }

            }
            else if(forma["BScInfo"].Equals("2")) //osoba je završila BSc ali na nekom drugom fakultetu pa će ovdje trebati implementirati onaj adapter pattern --- ovo ranije sve radi!
            {
                MasterStudentAdapter adapter = new MasterStudentAdapter();
                MasterStudent noviStudent = new MasterStudent(forma["ime"], forma["prezime"], forma["datumRodjenja"], forma["prebivaliste"], null, null, forma["izabraniSpol"], forma["izabraniSmjer"], null, double.Parse(forma["prosjekBSC"], format));
                if (validirajStudenta(noviStudent))
                {
                    logg.generišiKorisničkePodatke(adapter.ekvivalentirajStudenta(noviStudent, int.Parse(forma["drzavaBSC"])));
                    return RedirectToAction("UspješnoKreiranStudent", new { id = noviStudent.BrojIndeksa });
                }
                else
                {
                    return RedirectToAction("prikaziGresku", new { lokacija = "kreiraj-studenta", idPoruke = 1 });
                }
                
            }
            return null;
        }


        private bool validirajStudenta(Student tempStudent)
        {
            if(tempStudent is MasterStudent)
            {
                MasterStudent noviStudent = (MasterStudent)tempStudent;
                return !(noviStudent.Ime == null || noviStudent.Prezime == null || noviStudent.DatumRođenja == null || noviStudent.MjestoPrebivališta == null || noviStudent.Odsjek == null || noviStudent.Odsjek.Equals("Izaberite smjer") || noviStudent.ProsjekNaBSC < 0 || noviStudent.ProsjekNaBSC > 10);
                
            }
            return  !(tempStudent.Ime == null || tempStudent.Prezime == null || tempStudent.DatumRođenja == null || tempStudent.MjestoPrebivališta == null || tempStudent.Odsjek == null || tempStudent.Odsjek.Equals("Izaberite smjer"));
        }

        [Route("/studentska/svo-nastavno-osoblje")]
        public IActionResult AllTeachers()
        {
            ViewBag.zahtjevi = logg.dajSveNeobrađeneZahtjeve();
            var mapaPredmetiImenaID = logg.dajSvePredmeteImenaID();
            if (mapaPredmetiImenaID != null)
            {
                ViewBag.Predmeti = mapaPredmetiImenaID;
                return View();
            }
            return null;

        }

        [Route("/studentska/svo-nastavno-osoblje/kreiraj-nastavno-osoblje")]
        [Route("/studentska/kreiraj-nastavno-osoblje")]
        [HttpGet]
        public IActionResult KreirajNastavnoOsoblje()
        {
            ViewBag.zahtjevi = logg.dajSveNeobrađeneZahtjeve();
            var mapaPredmetiImenaID = logg.dajSvePredmeteImenaID();
            if(mapaPredmetiImenaID!=null)
            {
                ViewBag.Predmeti = mapaPredmetiImenaID;
                return View();
            }
            return RedirectToAction("prikaziGresku", new { lokacija = "kreiraj-nastavno-osoblje", idPoruke = 2 });

        }

        [Route("/studentska/svo-nastavno-osoblje/kreiraj-nastavno-osoblje")]
        [Route("/studentska/kreiraj-nastavno-osoblje")]
        [HttpPost]
        public IActionResult KreirajNastavnoOsoblje(IFormCollection forma)
        {
            if(forma!=null)
            {
                if(forma["titulaOdabir"].Equals("Red. prof. dr") || forma["titulaOdabir"].Equals("Doc. dr") || forma["titulaOdabir"].Equals("Van. prof. dr"))
                {
                    Profesor tempOsoba = new Profesor(forma["ime"], forma["prezime"], forma["datumRodjenja"], forma["prebivaliste"], null, null, forma["spol"], forma["titulaOdabir"]);
                    int userID = logg.generišiKorisničkePodatke(tempOsoba);
                    logg.zadužiKreiranogNaPredmetima(userID, Int32.Parse(forma["prviPredmetOdabir"]), Int32.Parse(forma["drugiPredmetOdabir"]));
                    return RedirectToAction("UspješnoKreiranoNastavnoOsoblje", new { id = userID });
                }
                else
                {
                    NastavnoOsoblje tempOsoba = new NastavnoOsoblje(forma["ime"], forma["prezime"], forma["datumRodjenja"], forma["prebivaliste"], null, null, forma["spol"], forma["titulaOdabir"]);
                    int userID = logg.generišiKorisničkePodatke(tempOsoba);
                    logg.zadužiKreiranogNaPredmetima(userID, Int32.Parse(forma["prviPredmetOdabir"]), Int32.Parse(forma["drugiPredmetOdabir"]));
                    return RedirectToAction("UspješnoKreiranoNastavnoOsoblje", new { id = userID });
                }
            }
            return View();
        }

        [Route("/studentska/svi-studenti-list")]
        public IActionResult AllStudentsList()
        {
            ViewBag.zahtjevi = logg.dajSveNeobrađeneZahtjeve();
            return View();
        }

        [Route("/studentska/svo-nastavno-osoblje-list")]
        public IActionResult AllTeachersList()
        {
            ViewBag.zahtjevi = logg.dajSveNeobrađeneZahtjeve();
            var mapaPredmetiImenaID = logg.dajSvePredmeteImenaID();
            if (mapaPredmetiImenaID != null)
            {
                ViewBag.Predmeti = mapaPredmetiImenaID;
                return View();
            }
            else
            {
                return RedirectToAction("prikaziGresku", new { lokacija = "svo-nastavno-osoblje", idPoruke = 12 });
            }
        }


        [Route("/studentska/sva-obavještenja-list")]
        public IActionResult AllAnnouncementsList()
        {
            ViewBag.zahtjevi = logg.dajSveNeobrađeneZahtjeve();
            var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:64580/zamger-api/");
            var responseTask = client.GetAsync("sva-obavjestenja");
            responseTask.Wait();

            var resultDisplay = responseTask.Result;
            if(resultDisplay.IsSuccessStatusCode)
            {
                var odgovor = resultDisplay.Content.ReadAsStringAsync();
                odgovor.Wait();
                ViewBag.listaObavjestenja = JsonConvert.DeserializeObject<List<Obavještenje>>(odgovor.Result);
                return View();
            }

            return RedirectToAction("prikaziGresku", new { lokacija = "sva-obavjestenja", idPoruke = 11 });

        }


        [Route("/studentska/svi-predmeti-list")]
        public IActionResult SviPredmetiList()
        {
            return View();
        }





        [Route("/studentska/kreiraj-predmet")]
        [HttpGet]
        public IActionResult KreirajPredmet()
        {
            ViewBag.zahtjevi = logg.dajSveNeobrađeneZahtjeve();
            return View();
        }

    
        [Route("/studentska/svi-predmeti-list/pretraga")]
        [HttpPost]
        public IActionResult PretražiPredmet(IFormCollection forma)
        {
            List<Tuple<int, string, double, int, int>> lista;
            try
            {
                lista = logg.pretražiPredmeteBasic(forma["nazivPredmeta"], forma["godinaStudija"].ToString(), forma["izborni"]);
            }
            catch
            {
                return View(new List<Tuple<int, string, double, int, int>>());
            }
            return View(lista);
        }



        [Route("/studentska/kreiraj-predmet")]
        [HttpPost]
        public IActionResult KreirajPredmet(IFormCollection forma)
        {
            NumberFormatInfo format = new NumberFormatInfo();
            format.NumberDecimalSeparator = ".";
            List<string> listaOdsjeciZaDodati = new List<string>();
            List<int> listaGodineZaDodati = new List<int>();

            var mapOdsjeci = new Dictionary<string, string>()
            {
                {"checkboxRI", "Računarstvo i informatika"},
                { "checkboxAiE", "Automatika i elektronika"},
                { "checkboxTK", "Telekomunikacije" },
                {"checkboxEE", "Elektroenergetika" }
            };

            var mapGodine = new Dictionary<string, int>()
            {
                {"checkboxPrva", 1},
                {"checkboxDruga", 2},
                {"checkboxTreca", 3},
                {"checkboxCetvrta", 4},
                {"checkboxPeta", 5}
            };


            foreach (KeyValuePair<string, string> entry in mapOdsjeci)
            {
                String tempst = forma[entry.Key];
                if (tempst != null)
                {
                    listaOdsjeciZaDodati.Add(entry.Value);
                }
            }

            foreach (KeyValuePair<string, int> entry in mapGodine)
            {
                String temps = forma[entry.Key];
                if (temps != null)
                {
                    listaGodineZaDodati.Add(entry.Value);
                }
            }

            int izborni = 0;
            String temp = forma["checkboxIzborni"];
            if(temp != null)
            {
                izborni = 1;
            }

            //Ove validacije nadam se da kontaš Paša, provjerava ispravnost forme, da neko nije nešto prazno ostavio i tako
            if(listaOdsjeciZaDodati.Any() && listaGodineZaDodati.Any()) //validacija u slucaju da neko nije pritisnuo nijedan checkbox
            {
                String tNaziv = forma["nazivPredmeta"];
                String tEcts = forma["ectsPoeni"];
                if(tNaziv!=null && tEcts!=null)
                {
                    int idPredmeta = logg.unesiPredmetUBazu(tNaziv, double.Parse(forma["ectsPoeni"], format), listaOdsjeciZaDodati, listaGodineZaDodati, izborni);
                    if (idPredmeta!=-1)
                    {
                        return RedirectToAction("UspješnoKreiranPredmet", new { id = idPredmeta });
                    }
                    else
                    {
                        return RedirectToAction("prikaziGresku", new { lokacija = "kreiraj-predmet", idPoruke = 3 });
                    }
                }
            }
            else
            {
                return RedirectToAction("prikaziGresku", new { lokacija = "kreiraj-predmet", idPoruke = 3 });
            }

            return View();
        }


        [Route("/studentska/student-uspjesno-kreiran/{id}")]
        public IActionResult UspješnoKreiranStudent(int? id)
        {
            if(id!=null)
            {
                ViewBag.zahtjevi = logg.dajSveNeobrađeneZahtjeve();
                Student tempStudent = logg.dajStudentaPoID(id);
                ViewBag.prosjek = logg.dajProsjekStudentaPoID(id);
                ViewBag.brojPredmeta = logg.dajBrojPredmetaPoID(id);
                return View(tempStudent);
            }
            return RedirectToAction("prikaziGresku", new { lokacija = "student-uspjesno-kreiran", idPoruke = 4 });
        }

        [Route("/studentska/predmet-uspjesno-kreiran/{id}")]
        public IActionResult UspješnoKreiranPredmet(int id)
        {
            ViewBag.zahtjevi = logg.dajSveNeobrađeneZahtjeve();
            KreiranPredmetViewModel predmet = logg.dajKreiranPredmetPoID(id);
            return View(predmet);
        }


        [Route("/studentska/nastavno-osoblje-uspjesno-kreirano/{id}")]
        public IActionResult UspješnoKreiranoNastavnoOsoblje(int id)
        {
            ViewBag.zahtjevi = logg.dajSveNeobrađeneZahtjeve();
            NastavnoOsoblje osoba = logg.dajKreiranoNastavnoOsobljePoID(id);
            List<string> lista = logg.dajNazivePredmetaNaKojimPredajePoID(id);
            KreiranoNastavnoOsobljeViewModel model;
            if(lista!=null && osoba!=null)
            {
                model = new KreiranoNastavnoOsobljeViewModel(osoba, lista);
                return View(model);
            }
            else
            {
                return RedirectToAction("prikaziGresku", new { lokacija = "nastavno-osoblje-uspjesno-kreirano", idPoruke = 5 });

            }
        }

        [Route("/studentska/kreiraj-obavjestenje")]
        [HttpGet]
        public IActionResult KreirajObavjestenje()
        {
            ViewBag.zahtjevi = logg.dajSveNeobrađeneZahtjeve();
            return View();
        }

        [Route("/studentska/kreiraj-obavjestenje")]
        [HttpPost]
        public IActionResult KreirajObavjestenje(IFormCollection forma)
        {
            if(forma!=null)
            {
                logg.kreirajObavještenje(forma["naslovObavještenja"], forma["sadržajObavještenja"]);
                return RedirectToAction("UspješnoKreiranoObavještenje");
            }
            return View();
        }


        [Route("/studentska/uspješno-kreirano-obavjestenje")]
        public IActionResult UspješnoKreiranoObavještenje()
        {
            ViewBag.zahtjevi = logg.dajSveNeobrađeneZahtjeve();
            return View();
        }

        [Route("/studentska/uredi-obavjestenje/{id}")]
        [HttpGet]
        public IActionResult UrediObavještenje(int id)
        {
            ViewBag.zahtjevi = logg.dajSveNeobrađeneZahtjeve();
            Obavještenje o = logg.dajObavještenjePoId(id);
            return View(o);
        }

        [Route("/studentska/uredi-obavjestenje/{id}")]
        [HttpPost]
        public IActionResult UrediObavještenje(int id, IFormCollection forma)
        {
            if(forma!=null)
            {
                Obavještenje o = new Obavještenje(forma["naslovObavještenja"], forma["sadržajObavještenja"], DateTime.Now, id);
                logg.editujObavještenje(o);
                return RedirectToAction("AllAnnouncementsList");
            }
            else
            {
                return RedirectToAction("prikaziGresku", new { lokacija = "uredi-obavjestenje", idPoruke = 6 });
            }
        }


        [Route("/studentska/izbrisi-obavjestenje/{id}")]
        public IActionResult IzbrišiObavještenje(int id)
        {
            ViewBag.zahtjevi = logg.dajSveNeobrađeneZahtjeve();
            logg.izbrisiObavjestenje(id);
            return RedirectToAction("AllAnnouncementsList");
        }


        [Route("/studentska/svi-studenti/pretraga")]
        [HttpPost]
        public IActionResult PretražiStudente(IFormCollection forma)
        {
            try
            {


                int? brIndeksa;
                if (String.IsNullOrEmpty(forma["brojIndeksa"].ToString())) brIndeksa = null;
                else
                {
                    brIndeksa = int.Parse(forma["brojIndeksa"].ToString());
                }
                string pom = forma["odsjek"];
                if (pom == "Svi odsjeci") pom = null;
                List<Student> studenti = logg.pretražiStudenta(brIndeksa, forma["ime"], forma["Prezime"], pom);
                if (studenti != null)
                {
                    return View(studenti);
                }
                return View(new List<Student>());
            }
            catch (Exception e)
            {
                return View(new List<Student>());
            }
        }

        [Route("/studentska/svi-studenti/pretraga-list")]
        [HttpPost]
        public IActionResult PretražiStudenteList(IFormCollection forma)
        {
            try
            {


                int? brIndeksa;
                if (String.IsNullOrEmpty(forma["brojIndeksa"].ToString())) brIndeksa = null;
                else
                {
                    brIndeksa = int.Parse(forma["brojIndeksa"].ToString());
                }
                string pom = forma["odsjek"];
                if (pom == "Svi odsjeci") pom = null;
                List<Student> studenti = logg.pretražiStudenta(brIndeksa, forma["ime"], forma["Prezime"], pom);
                if (studenti != null)
                {
                    return View(studenti);
                }
                return View(new List<Student>());
            }
            catch(Exception e)
            {
                return View(new List<Student>());
            }
        }


        [Route("/studentska/svo-nastavno-osoblje/pretraga")]
        [HttpPost]
        public IActionResult PretražiNastavnoOsoblje(IFormCollection forma)
        {
            try
            {
                var mapaPredmetiImenaID = logg.dajSvePredmeteImenaID();
                if (mapaPredmetiImenaID != null)
                {
                    ViewBag.Predmeti = mapaPredmetiImenaID;
                    List<Tuple<int, NastavnoOsoblje>> ltpl = logg.pretražiNastavnoOsoblje(forma["imeOsobe"], forma["prezimeOsobe"], forma["predmeti"]);
                    return View(ltpl);
                }
                else
                {
                    return View(new List<Tuple<int, NastavnoOsoblje>>());
                }
            }
            catch(Exception e)
            {
                return View(new List<Tuple<int, NastavnoOsoblje>>());
            }
        }

        [Route("/studentska/svo-nastavno-osoblje-list/pretraga")]
        [HttpPost]
        public IActionResult PretražiNastavnoOsobljeList(IFormCollection forma)
        {
            try
            {
                var mapaPredmetiImenaID = logg.dajSvePredmeteImenaID();
                if (mapaPredmetiImenaID != null)
                {
                    ViewBag.Predmeti = mapaPredmetiImenaID;
                    List<Tuple<int, NastavnoOsoblje>> ltpl = logg.pretražiNastavnoOsoblje(forma["imeOsobe"], forma["prezimeOsobe"], forma["predmeti"]);
                    return View(ltpl);
                }
                else
                {
                    return View(new List<Tuple<int, NastavnoOsoblje>>());
                }
            }
            catch (Exception e)
            {
                return View(new List<Tuple<int, NastavnoOsoblje>>());
            }
        }


        [Route("/studentska/uredi-predmet/{id}")]
        [HttpGet]
        public IActionResult UrediPredmet(int id)
        {
            KreiranPredmetViewModel predmet = logg.dajKreiranPredmetPoID(id);
            ViewBag.zahtjevi = logg.dajSveNeobrađeneZahtjeve();
            return View(predmet);
        }

        [Route("/studentska/uredi-predmet/{id}")]
        [HttpPost]
        public IActionResult UrediPredmet(int id, IFormCollection forma)
        {
            NumberFormatInfo format = new NumberFormatInfo();
            format.NumberDecimalSeparator = ".";
            List<string> listaOdsjeciZaDodati = new List<string>();
            List<int> listaGodineZaDodati = new List<int>();

            var mapOdsjeci = new Dictionary<string, string>()
            {
                {"checkboxRI", "Računarstvo i informatika"},
                { "checkboxAiE", "Automatika i elektronika"},
                { "checkboxTK", "Telekomunikacije" },
                {"checkboxEE", "Elektroenergetika" }
            };

            var mapGodine = new Dictionary<string, int>()
            {
                {"checkboxPrva", 1},
                {"checkboxDruga", 2},
                {"checkboxTreca", 3},
                {"checkboxCetvrta", 4},
                {"checkboxPeta", 5}
            };


            foreach (KeyValuePair<string, string> entry in mapOdsjeci)
            {
                String tempst = forma[entry.Key];
                if (tempst != null)
                {
                    listaOdsjeciZaDodati.Add(entry.Value);
                }
            }

            foreach (KeyValuePair<string, int> entry in mapGodine)
            {
                String temps = forma[entry.Key];
                if (temps != null)
                {
                    listaGodineZaDodati.Add(entry.Value);
                }
            }

            int izborni = 0;
            String temp = forma["checkboxIzborni"];
            if (temp != null)
            {
                izborni = 1;
            }


            KreiranPredmetViewModel prdmt = logg.dajKreiranPredmetPoID(id);
            if(!String.IsNullOrEmpty(forma["ectsPoeni"]) && !forma["ectsPoeni"].Equals(prdmt.EctsPoeni.ToString()))
            {
                logg.promijeniEctsPredmetu(id, float.Parse(forma["ectsPoeni"]));
            }

            if(listaGodineZaDodati.Any() && listaOdsjeciZaDodati.Any())
            {
                logg.promijeniDostupnostPredmet(id, listaOdsjeciZaDodati, listaGodineZaDodati, izborni);
                return RedirectToAction("SviPredmetiList");
            }
            return RedirectToAction("prikaziGresku", new { lokacija = "uredi-predmet/"+id, idPoruke = 8 });
        }





        [Route("/studentska/uredi-studenta/{id}")]
        [HttpGet]
        public IActionResult UrediStudenta(int id)
        {
            ViewBag.pass = logg.dajPasswordPoId(id);
            ViewBag.zahtjevi = logg.dajSveNeobrađeneZahtjeve();
            Student tempStudent = logg.dajStudentaPoID(id);
            return View(tempStudent);
        }

        [Route("/studentska/uredi-studenta/{id}")]
        [HttpPost]
        public IActionResult UrediStudenta(int id, IFormCollection forma)
        {
            Student tempStudent = logg.dajStudentaPoID(id);
            int prebUpdate = 0, passUpdate = 0, godUpdate = 0;
            if (!String.IsNullOrEmpty(forma["prebivaliste"]) && !forma["prebivaliste"].Equals(tempStudent.MjestoPrebivališta)) prebUpdate = 1;
            if (!String.IsNullOrEmpty(forma["passwordUser"]) && !forma["passwordUser"].Equals(logg.dajPasswordPoId(id))) passUpdate = 1;
            if (!forma["godinaStudija"].Equals(tempStudent.GodinaStudija.ToString())) godUpdate = 1;
            if (prebUpdate != 0 || passUpdate != 0 || godUpdate != 0)
            {
                try
                {
                    logg.izmijeniStudentskePodatke(prebUpdate, passUpdate, godUpdate, forma["prebivaliste"], forma["passwordUser"], forma["godinaStudija"], id);
                    return RedirectToAction("AllStudentsList");
                }
                catch(Exception)
                {
                    return RedirectToAction("prikaziGresku", new { lokacija = "uredi-studenta/"+id, idPoruke = 9 });
                }
            }
            return RedirectToAction("AllStudentsList");
        }




        [Route("/studentska/neobradjeni-zahtjevi")]
        public IActionResult NeobrađeniZahtjevi()
        {
            ViewBag.zahtjevi = logg.dajSveNeobrađeneZahtjeve();
            return View();
        }


        [Route("/studentska/obradi-zahtjev/{id}")]
        [HttpGet]
        public IActionResult ObradiZahtjev(int id)
        {
            ViewBag.zahtjevi = logg.dajSveNeobrađeneZahtjeve();
            ViewBag.zahtjev = logg.dajZahjtevPoId(id);
            return View();
        }

        [Route("/studentska/obradi-zahtjev/{id}")]
        [HttpPost]
        public IActionResult ObradiZahtjev(int id, IFormCollection forma)
        {
            ViewBag.zahtjevi = logg.dajSveNeobrađeneZahtjeve();
            if (logg.odobriZahtjev(id))
            {
                return RedirectToAction("ZahtjevUspješnoObrađen");
            }
            return RedirectToAction("prikaziGresku", new { lokacija = "uredi-zahtjev/"+id, idPoruke = 10 });
        }

        [Route("/studentska/zahtjev-uspjesno-obradjen")]
        public IActionResult ZahtjevUspješnoObrađen()
        {
            ViewBag.zahtjevi = logg.dajSveNeobrađeneZahtjeve();
            return View();
        }


        [Route("/studentska/plate-svih-uposlenih")]
        public IActionResult PlateSvihUposlenih()
        {
            ViewBag.zahtjevi = logg.dajSveNeobrađeneZahtjeve();
            ViewBag.uposlenici = logg.pretražiNastavnoOsoblje(null, null, "Izaberite");
            return View();
        }


        [Route("/studentska/izbrisi-studenta/{id}")]
        public IActionResult IzbrišiStudenta(int id)
        {
            logg.izbrisiStudentaPoId(id);
            return RedirectToAction("StudentUspješnoIzbrisan");
        }

        [Route("/studentska/student-uspjesno-izbrisan")]
        public IActionResult StudentUspješnoIzbrisan()
        {
            ViewBag.zahtjevi = logg.dajSveNeobrađeneZahtjeve();
            return View();
        }

        [Route("/studentska/izbrisi-nastavno-osoblje/{id}")]
        public IActionResult IzbrišiNastavnoOsoblje(int id)
        {
            logg.izbrisiNastavnoOsobljePoId(id);
            return RedirectToAction("NastavnoOsobljeUspješnoIzbrisano");
        }

        [Route("/studentska/nastavno-osoblje-uspjesno-izbrisano")]
        public IActionResult NastavnoOsobljeUspješnoIzbrisano()
        {
            ViewBag.zahtjevi = logg.dajSveNeobrađeneZahtjeve();
            return View();
        }


        [Route("/studentska/izbrisi-predmet/{id}")]
        public IActionResult IzbrišiPredmet(int id)
        {
            logg.izbrisiPredmetPoId(id);
            return RedirectToAction("UspješnoIzbrisanPredmet");
        }

        [Route("/studentska/predmet-uspjesno-izbrisan")]
        public IActionResult UspješnoIzbrisanPredmet()
        {
            ViewBag.zahtjevi = logg.dajSveNeobrađeneZahtjeve();
            return View();
        }

        [Route("/studentska/uredi-nastavno-osoblje/{id}")]
        [HttpGet]
        public IActionResult UrediNastavnoOsoblje(int id)
        {
            ViewBag.zahtjevi = logg.dajSveNeobrađeneZahtjeve();
            NastavnoOsoblje no = logg.dajKreiranoNastavnoOsobljePoID(id);
            ViewBag.predmetiNaKojimPredaje = logg.dajNazivePredmetaNaKojimPredajePoID(id);
            ViewBag.sviPredmeti = logg.dajSvePredmeteImenaID();
            ViewBag.pass = logg.dajPasswordPoId(id);
            ViewBag.titule = new List<string>() { "Red. prof. dr", "Doc. dr", "Van. prof. dr", "Mr. dipl. ing", "BSc. ing" };
            return View(no);
        }

        [Route("/studentska/uredi-nastavno-osoblje/{id}")]
        [HttpPost]
        public IActionResult UrediNastavnoOsoblje(int id, IFormCollection forma)
        {
            logg.izmijeniNastavnikaPoId(id, forma["prebivaliste"], forma["trenutnaTitula"], forma["passwordUser"], forma["predmet1"], forma["predmet2"]);
            return RedirectToAction("NastavnoOsobljeUspješnoUređeno");
        }

        [Route("/student/osoba-uspjesno-editovana")]
        public IActionResult NastavnoOsobljeUspješnoUređeno()
        {
            return View();
        }

        [Route("/studentska/{lokacija}/greska/{idPoruke}")]
        public IActionResult prikaziGresku(string lokacija, int idPoruke)
        {
            
            if(idPoruke==1)
            {
                ViewBag.poruka = "Greška pri kreiranju studenta";
            }
            else if(idPoruke==2)
            {
                ViewBag.poruka = "Greška pri kreiranju nastavnog osoblja";
            }
            else if(idPoruke==3)
            {
                ViewBag.poruka = "Greška pri kreiranju predmeta";
            }
            else if(idPoruke==4)
            {
                ViewBag.poruka = "Greška prilikom log in-a";
            }
            else if(idPoruke==5)
            {
                ViewBag.poruka = "Greška prilikom dodavanja nastavnog osoblja u sistem";
            }
            else if(idPoruke==6)
            {
                ViewBag.poruka = "Greška prilikom uređivanja obavještenja";
            }
            else if (idPoruke==7)
            {
                ViewBag.poruka = "Greška prilikom pretrage";
            }
            else if(idPoruke==8)
            {
                ViewBag.poruka = "Greška pri uređivanju predmeta";
            }
            else if(idPoruke==9)
            {
                ViewBag.poruka = "Greška pri uređivanju podataka o studentu";
            }
            else if(idPoruke==10)
            {
                ViewBag.poruka = "Greška pri obradi zahtjeva";
            }
            else if (idPoruke == 11)
            {
                ViewBag.poruka = "Greška prilikom parsiranja JSON file-a liste obavještenja";
            }
            else if (idPoruke == 12)
            {
                ViewBag.poruka = "Ne postoji niti jedan predmet u bazi!";
            }
            return View();
        }


        [Route("/studentska/moj-profil")]
        [HttpGet]
        public IActionResult mojProfil()
        {
            ViewBag.zahtjevi = logg.dajSveNeobrađeneZahtjeve();
            ViewBag.ukupanBrojOsoba = logg.dajUkupanBrojOsobaNaSistemu();
            ViewBag.ukupanBrojStudenata = logg.dajUkupanBrojStudenataNaSistemu();
            ViewBag.ukupanBrojNastavnogOsoblja = logg.dajUkupanBrojNastavnogOsobljaNaSistemu();
            ViewBag.uposlenici = logg.pretražiNastavnoOsoblje(null, null, "Izaberite");
            ViewBag.zahtjevi = logg.dajSveNeobrađeneZahtjeve();
            ViewBag.sifra = logg.dajPasswordPoId(1);


            return View();
        }

        [Route("/studentska/moj-profil")]
        [HttpPost]
        public IActionResult mojProfil(IFormCollection forma)
        {

            ViewBag.zahtjevi = logg.dajSveNeobrađeneZahtjeve();
            ViewBag.ukupanBrojOsoba = logg.dajUkupanBrojOsobaNaSistemu();
            ViewBag.ukupanBrojStudenata = logg.dajUkupanBrojStudenataNaSistemu();
            ViewBag.ukupanBrojNastavnogOsoblja = logg.dajUkupanBrojNastavnogOsobljaNaSistemu();
            ViewBag.uposlenici = logg.pretražiNastavnoOsoblje(null, null, "Izaberite");
            ViewBag.zahtjevi = logg.dajSveNeobrađeneZahtjeve();
            
            

            try
            {
                logg.promijeniPasswordKorisniku(1, forma["password"]);
                ViewBag.sifra = logg.dajPasswordPoId(1);
            }
            catch (Exception e)
            {

            }

            return View();
        }

        [Route("/studentska/nadolazeci-ispiti")]
        public IActionResult nadolazeciIspiti()
        {
            ViewBag.zahtjevi = logg.dajSveNeobrađeneZahtjeve();
            ViewBag.ukupanBrojOsoba = logg.dajUkupanBrojOsobaNaSistemu();
            ViewBag.ukupanBrojStudenata = logg.dajUkupanBrojStudenataNaSistemu();
            ViewBag.ukupanBrojNastavnogOsoblja = logg.dajUkupanBrojNastavnogOsobljaNaSistemu();
            ViewBag.uposlenici = logg.pretražiNastavnoOsoblje(null, null, "Izaberite");
            ViewBag.zahtjevi = logg.dajSveNeobrađeneZahtjeve();
            ViewBag.nadolazeciIspiti = logg.dajInfoONadolazecimIspitima();

            return View();
        }


        [Route("/studentska/info-o-studentu/{idStudenta}")]
        public IActionResult infoOStudentu(int idStudenta)
        {
            KreatorKorisnika creator = new KreatorKorisnika();
            ViewBag.trazeniStudent = creator.FactoryMethod(idStudenta);
            return View();
        }



    }
}