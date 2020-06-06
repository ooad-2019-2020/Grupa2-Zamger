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
        public List<Poruka> dajOutbox(int idOsobe)
        {
            if (Autentifikacija.GetIdKorisnika(HttpContext).Value == idOsobe)
            {
                return zmgr.dajOutbox(idOsobe);
            }
            return null;
        }
    }
}
