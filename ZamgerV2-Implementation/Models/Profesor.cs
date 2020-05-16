using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZamgerV2_Implementation.Models
{
    public class Profesor : NastavnoOsoblje
    {

        private int? nekkiBezzeID;
        public Profesor(String tIme, String tPrezime, String tDatumRođenja, String tMjestoPrebivališta, String tUsername, String tEmail, String tSpol, String tTitula, int? tNekiID) : base(tIme, tPrezime, tDatumRođenja, tMjestoPrebivališta, tUsername, tEmail, tSpol, tTitula)
        {
            nekkiBezzeID = tNekiID;
        }




    }
}
