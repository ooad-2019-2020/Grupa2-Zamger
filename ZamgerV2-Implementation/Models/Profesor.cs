using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZamgerV2_Implementation.Models
{
    public class Profesor : NastavnoOsoblje, IPlataOsoblja
    {
        List<Anketa> anketeNaPredmetima;
        public Profesor(String tIme, String tPrezime, String tDatumRođenja, String tMjestoPrebivališta, String tUsername, String tEmail, String tSpol, String tTitula) : base(tIme, tPrezime, tDatumRođenja, tMjestoPrebivališta, tUsername, tEmail, tSpol, tTitula)
        {
            AnketeNaPredmetima = null;
        }

        public List<Anketa> AnketeNaPredmetima { get => anketeNaPredmetima; set => anketeNaPredmetima = value; }

        public double dajPlatu() 
        {
            return 505.6;
        }
    }
}
