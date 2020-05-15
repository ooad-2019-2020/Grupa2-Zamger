using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using ZamgerV2_Implementation.Models;


/*      
 *      NAPOMENA ZA OSTALE ---
 *      Ovi Response.WriteAsync služe samo za debugganje i pretežno su stavljani na mjesto gdje se očekuje odnosno gdje
 *      se desila neka greška kako bi se to signaliziralo.
 *      
 *      Potrebno je sve ove ResponseAsync-e zamijeniti sa odgovarajućim Error stranicama, tipa napraviti custom error 404 page i slično
 */

namespace ZamgerV2_Implementation.Controllers
{
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
            return View();
        }

        [Route("/studentska/svi-studenti")]
        public IActionResult AllStudents()
        {
            return View();
        }

        [Route("/studentska/svi-studenti/kreiraj-studenta")]
        [Route("/studentska/kreiraj-studenta")]
        [HttpGet]
        public IActionResult KreirajStudenta()
        {
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
                        Response.WriteAsync("Nije OK - student -> ima nepravilne podatke");
                    }
                }

                else
                {
                    Response.WriteAsync("Nije OK - taj smjer ne postoji");
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
                        Response.WriteAsync("Nije OK - Master student -> ima nepravilne podatke");
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
                    Response.WriteAsync("Nije OK - Master student -> ima nepravilne podatke");
                }
                Response.WriteAsync("Nije OK");
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

            //trebat će napravit bolju validaciju, sama forma ne da da se submita sve dok sva polja koja su oznacena kao tražena ne ispune, no to ne onemogućava npr da neko unese npr prosjek 100
        }

        [Route("/studentska/svo-nastavno-osoblje")]
        public IActionResult AllTeachers()
        {
            return View();

        }

        [Route("/studentska/svo-nastavno-osoblje/kreiraj-nastavno-osoblje")]
        [Route("/studentska/kreiraj-nastavno-osoblje")]
        [HttpGet]
        public IActionResult KreirajNastavnoOsoblje()
        {
            return View();
        }

        [Route("/studentska/svo-nastavno-osoblje/kreiraj-nastavno-osoblje")]
        [Route("/studentska/kreiraj-nastavno-osoblje")]
        [HttpPost]
        public IActionResult KreirajNastavnoOsoblje(IFormCollection forma)
        {
            foreach(String kljuc in forma.Keys){
                Response.WriteAsync("kljuc: " + kljuc + "  -   vrijednost:   " + forma[kljuc]+" --- ");
            }
            return View();
        }

        [Route("/studentska/svi-studenti-list")]
        public IActionResult AllStudentsList()
        {
            return View();
        }

        [Route("/studentska/svo-nastavno-osoblje-list")]
        public IActionResult AllTeachersList()
        {
            return View();
        }


        [Route("/studentska/kreiraj-predmet")]
        [HttpGet]
        public IActionResult KreirajPredmet()
        {
            return View();
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
                {"checkboxCetvrta", 4 },
                {"checkboxPeta", 5 }
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
                    if (logg.unesiPredmetUBazu(tNaziv, double.Parse(forma["ectsPoeni"], format), listaOdsjeciZaDodati, listaGodineZaDodati, izborni))
                    {
                        Response.WriteAsync("Sve je Ok");
                        //Sad ovdje treba redirectto na uspjesno kreiran predmet na sistemu View(taj view treba napraviti)
                    }
                }
            }
            else
            {
                Response.WriteAsync("Nešto nije uredu, checkboxovi najvjerovatnije prazni");
            }

            return View();
        }


        [Route("/studentska/student-uspjesno-kreiran/{id}")]
        public IActionResult UspješnoKreiranStudent(int? id)
        {
            if(id!=null)
            {
                Student tempStudent = logg.dajStudentaPoID(id);
                return View(tempStudent);
            }
            Response.WriteAsync("neka greška prilikom prikazivanja uspješnog logina");
            return null;
        }


    }
}