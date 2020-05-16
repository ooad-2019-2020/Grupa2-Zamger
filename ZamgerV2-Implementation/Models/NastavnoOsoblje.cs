using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZamgerV2_Implementation.Models
{
    public class NastavnoOsoblje : Korisnik
    {


        private String titula;
        private int? idOsobe;

        public NastavnoOsoblje(String tIme, String tPrezime, String tDatumRođenja, String tMjestoPrebivališta, String tUsername, String tEmail, String tSpol, String tTitula): base(tIme, tPrezime, tDatumRođenja, tMjestoPrebivališta, tUsername, tEmail, tSpol)
        {
            this.Titula = tTitula;
        }

        public string Titula { get => titula; set => titula = value; }
        public int? IdOsobe { get => idOsobe; set => idOsobe = value; }
    }
}
