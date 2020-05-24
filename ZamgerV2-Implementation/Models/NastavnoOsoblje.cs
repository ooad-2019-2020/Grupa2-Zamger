using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZamgerV2_Implementation.Models
{
    public class NastavnoOsoblje : Korisnik, IPlataOsoblja
    {


        private String titula;
        private int? idOsobe;
        List<PredmetZaNastavnoOsoblje> predmetiNaKojimPredaje;
        List<Aktivnost> aktivnosti;

        public NastavnoOsoblje(String tIme, String tPrezime, String tDatumRođenja, String tMjestoPrebivališta, String tUsername, String tEmail, String tSpol, String tTitula): base(tIme, tPrezime, tDatumRođenja, tMjestoPrebivališta, tUsername, tEmail, tSpol)
        {
            this.Titula = tTitula;
        }

        public string Titula { get => titula; set => titula = value; }
        public int? IdOsobe { get => idOsobe; set => idOsobe = value; }
        public List<PredmetZaNastavnoOsoblje> PredmetiNaKojimPredaje { get => predmetiNaKojimPredaje; set => predmetiNaKojimPredaje = value; }
        public List<Aktivnost> Aktivnosti { get => aktivnosti; set => aktivnosti = value; }

        public double dajPlatu()
        {
            return 203.4;
        }
    }
}
