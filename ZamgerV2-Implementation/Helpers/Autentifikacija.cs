using Microsoft.AspNetCore.Http;
using ZamgerV2_Implementation.Models;

namespace ZamgerV2_Implementation.Helpers
{
    public enum TipKorisnika
    {
        Student = 1,
        NastavnoOsoblje,
        StudentskaSluzba,
        Profesor
    }

    public class Autentifikacija
    {
        private const string _logiraniKorisnik = "logirani_korisnik";
        private const string _tipKorisnika = "tip_korisnika";

        private IHttpContextAccessor httpContextAccessor;

        public Autentifikacija(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public static void PokreniNovuSesiju(int idKorisnika, HttpContext context, TipKorisnika tipKorisnika)
        {
            context.Session.SetJson(_logiraniKorisnik, idKorisnika);
            context.Session.SetJson(_tipKorisnika, tipKorisnika);
        }

        public static void OcistiSesiju(HttpContext httpContext) => httpContext.Session.SetJson(_logiraniKorisnik, null);

        public static int? GetIdKorisnika(HttpContext context)
        {
            return context.Session.GetJson<int?>(_logiraniKorisnik);
        }

        public static TipKorisnika? GetTipKorisnika(HttpContext context)
        {
            return context.Session.GetJson<TipKorisnika?>(_tipKorisnika);
        }

        public static Student GetLogiraniStudent(HttpContext context)
        {
            var idKorisnika = GetIdKorisnika(context);

            if (idKorisnika == null)
                return null;

            KreatorKorisnika creator = new KreatorKorisnika();
            Korisnik tempK = creator.FactoryMethod(idKorisnika.Value);

            if (tempK.GetType() == typeof(Student))
            {
                return (Student)tempK;
            }
            else
            {
                return (MasterStudent)tempK;
            }
        }

        public static NastavnoOsoblje GetNastavnoOsoblje(HttpContext context)
        {
            var idKorisnika = GetIdKorisnika(context);

            if (idKorisnika == null)
                return null;

            KreatorKorisnika creator = new KreatorKorisnika();
            Korisnik tempK = creator.FactoryMethod(idKorisnika.Value);

            if (tempK.GetType() == typeof(NastavnoOsoblje))
            {
                return (NastavnoOsoblje)tempK;
            }
            else
            {
                return (Profesor)tempK;
            }
        }
    }
}
