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

namespace ZamgerV2_Implementation.Controllers
{
    public class StudentskaController : Controller
    {
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
            Logger logg = Logger.GetInstance();
            if (forma["BScInfo"].Equals("3")) //ako osoba nije završila BSc tada se upisuje kao običan a ne MasterStudent -> nema potrebe za ekvivalentiranjem
            {
                if (!forma["izabraniSmjer"].Equals("Izaberite smjer")) 
                { 
                    Student noviStudent = new Student(forma["ime"], forma["prezime"], forma["datumRodjenja"], forma["prebivaliste"], null, null, forma["izabraniSpol"], forma["izabraniSmjer"], null);
                    if (validirajStudenta(noviStudent))
                    {
                        logg.generišiKorisničkePodatke(noviStudent);
                        Logger.removeInstance();
                        Response.WriteAsync("Sve je OK");
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
                        Logger.removeInstance();
                        Response.WriteAsync("Sve je OK");
                    }
                    else
                    {
                        Response.WriteAsync("Nije OK - Master student -> ima nepravilne podatke");
                    }
                }

            }
            else //osoba je završila BSc ali na nekom drugom fakultetu pa će ovdje trebati implementirati onaj adapter pattern --- ovo ranije sve radi!
            {
                Logger.removeInstance();
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
    }
}