using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ZamgerV2_Implementation.Helpers
{
    public class Autorizacija : TypeFilterAttribute
    {
        public Autorizacija(bool sviKorisnici, params TipKorisnika[] korisnickeUloge) : base(typeof(AsyncActionFilter))
        {
            Arguments = new object[]
            {
               sviKorisnici,
               korisnickeUloge
            };
        }

        public class AsyncActionFilter : IAsyncActionFilter
        {
            private readonly bool _sviKorisnici;
            private readonly TipKorisnika[] _korisnickeUloge;

            public AsyncActionFilter(bool sviKorisnici, params TipKorisnika[] korisnickeUloge)
            {
                _sviKorisnici = sviKorisnici;
                _korisnickeUloge = korisnickeUloge;
            }

            public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
            {
                var idKorisnika = Autentifikacija.GetIdKorisnika(context.HttpContext);
                var tipKorisnika = Autentifikacija.GetTipKorisnika(context.HttpContext);

                if (!idKorisnika.HasValue || !tipKorisnika.HasValue) // user nije logiran
                {
                    context.HttpContext.Response.Redirect("/Home");
                    return;
                }

                if (_sviKorisnici || _korisnickeUloge.Contains(tipKorisnika.Value))
                {
                    await next();
                    return;
                }

                context.Result = new RedirectToActionResult("pristupOdbijen", "Početni", new { id = 401 });
            }
        }

    }
}
