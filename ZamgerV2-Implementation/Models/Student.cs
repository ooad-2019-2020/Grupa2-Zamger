using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZamgerV2_Implementation.Models
{
    public class Student : Korisnik
    {

        private String odsjek;
        private int? brojIndeksa;
        private int godinaStudija;
        private List<PredmetZaStudenta> predmeti;

        public Student(String tIme, String tPrezime, String tDatumRođenja, String tMjestoPrebivališta, String tUsername, String tEmail, String tSpol, String tOdsjek, int? tBrojIndeksa): base(tIme, tPrezime, tDatumRođenja, tMjestoPrebivališta, tUsername, tEmail, tSpol)
        {
            this.odsjek = tOdsjek;
            this.brojIndeksa = tBrojIndeksa;
        }

        public string Odsjek { get => odsjek; set => odsjek = value; }
        public int? BrojIndeksa { get => brojIndeksa; set => brojIndeksa = value; }
        public int GodinaStudija { get => godinaStudija; set => godinaStudija = value; }
        public List<PredmetZaStudenta> Predmeti { get => predmeti; set => predmeti = value; }
    }
}
