using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZamgerV2_Implementation.Models
{
    public class MasterStudent : Student
    {
        private Double prosjekNaBSC;
        public MasterStudent(String tIme, String tPrezime, String tDatumRođenja, String tMjestoPrebivališta, String tUsername, String tEmail, String tSpol, String tOdsjek, int? tBrojIndeksa, Double tProsjek) : base(tIme, tPrezime, tDatumRođenja, tMjestoPrebivališta, tUsername, tEmail, tSpol, tOdsjek, tBrojIndeksa)
        {
            this.prosjekNaBSC = tProsjek;
        }
        public Double ProsjekNaBSC { get => prosjekNaBSC; set => prosjekNaBSC = value; }
    }
}
