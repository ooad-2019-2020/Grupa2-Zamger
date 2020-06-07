using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using ZamgerV2_Implementation.Helpers;
using ZamgerV2_Implementation.Models;

namespace ZamgerV2_Implementation.Controllers
{
    [Route("/zamger-api")]
    [ApiController]
    public class ZamgerApiController : ControllerBase
    {
        private Logger logg;
        private ZamgerDbContext zmgr;
        public ZamgerApiController()
        {
            this.logg = Logger.GetInstance();
            this.zmgr = ZamgerDbContext.GetInstance();
        }

        [Route("/zamger-api/sva-obavjestenja")]
        [HttpGet]
        public List<Obavještenje> dajObavjestenja()
        {
            return logg.dajObavještenja();
        }

        [Autorizacija(false, TipKorisnika.Student, TipKorisnika.NastavnoOsoblje, TipKorisnika.Profesor)]
        [Route("/zamger-api/inbox/{idOsobe}")]
        [HttpGet]
        public List<Poruka> dajInbox(int idOsobe)
        {
            if (Autentifikacija.GetIdKorisnika(HttpContext).Value == idOsobe)
            {
                return zmgr.dajInbox(idOsobe);
            }
            return null;
        }

        [Autorizacija(false, TipKorisnika.Student, TipKorisnika.NastavnoOsoblje, TipKorisnika.Profesor)]
        [Route("/zamger-api/outbox/{idOsobe}")]
        [HttpGet]
        public List<Poruka> dajOutbox(int idOsobe)
        {
            if (Autentifikacija.GetIdKorisnika(HttpContext).Value == idOsobe)
            {
                return zmgr.dajOutbox(idOsobe);
            }
            return null;
        }



        [Autorizacija(false, TipKorisnika.NastavnoOsoblje, TipKorisnika.Profesor, TipKorisnika.StudentskaSluzba)]
        [Route("/zamger-api/studenti-na-predmetu/{idPredmeta}")]
        [HttpGet]
        public List<Student> dajStudentaPoId(int idPredmeta)
        {
            var trenutniKorisnik = Autentifikacija.GetNastavnoOsoblje(HttpContext);
            foreach(PredmetZaNastavnoOsoblje p in trenutniKorisnik.PredmetiNaKojimPredaje)
            {
                if(p.IdPredmeta==idPredmeta)
                {
                    return zmgr.formirajStudenteNaPredmetuPoId(idPredmeta);
                }
            }
            return null;
        }


        [Autorizacija(false, TipKorisnika.Profesor, TipKorisnika.StudentskaSluzba)]
        [Route("/zamger-api/odgovori-na-anketu/{idAnkete}")]
        [HttpGet]
        public List<OdgovorNaAnketu> dajOdgovoreNaAnketu(int idAnkete)
        {
            var trenutniKorisnik =(Profesor)Autentifikacija.GetNastavnoOsoblje(HttpContext);
           
            foreach (Anketa an in trenutniKorisnik.AnketeNaPredmetima)
            {
                if (an.IdAnkete == idAnkete)
                {
                    return zmgr.dajOdgovoreNaAnketu(idAnkete);
                }
            }
            return null;
        }

        [Autorizacija(false, TipKorisnika.Student)]
        [Route("/zamger-api/ispiti-za-studenta-na-predmetu/{idPredmeta}")]
        [HttpGet]
        public List<Ispit> dajStudentoveIspiteNaPredmetu(int idPredmeta)
        {
            var trenutniKorisnik = Autentifikacija.GetLogiraniStudent(HttpContext);

            foreach (PredmetZaStudenta an in trenutniKorisnik)
            {
                if (an.IdPredmeta == idPredmeta)
                {
                    return zmgr.dajStudentoveIspite(trenutniKorisnik.BrojIndeksa.Value, idPredmeta);
                }
            }
            return null;
        }



    }
}
