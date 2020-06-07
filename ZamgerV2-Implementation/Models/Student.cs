using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZamgerV2_Implementation.Models
{
    public class Student : Korisnik, IEnumerable
    {

        private String odsjek;
        private int? brojIndeksa;
        private int godinaStudija;
        private List<PredmetZaStudenta> predmeti;

        public Student(String tIme, String tPrezime, String tDatumRođenja, String tMjestoPrebivališta, String tUsername, String tEmail, String tSpol, String tOdsjek, int? tBrojIndeksa) : base(tIme, tPrezime, tDatumRođenja, tMjestoPrebivališta, tUsername, tEmail, tSpol)
        {
            this.odsjek = tOdsjek;
            this.brojIndeksa = tBrojIndeksa;
        }

        public string Odsjek { get => odsjek; set => odsjek = value; }
        public int? BrojIndeksa { get => brojIndeksa; set => brojIndeksa = value; }
        public int GodinaStudija { get => godinaStudija; set => godinaStudija = value; }
        public List<PredmetZaStudenta> Predmeti { get => predmeti; set => predmeti = value; }

        public object Current => throw new NotImplementedException();

        public IEnumerator GetEnumerator()
        {
            if (Predmeti != null)
            {
                foreach (var predmet in Predmeti)
                {
                    yield return predmet;
                }
            }
        }
        public double dajBrojBodovaNaPredmetu(int idPredmeta)
        {
            double brojBodova = 0;
            PredmetZaStudenta prdmt = null;
            foreach (PredmetZaStudenta p in predmeti)
            {
                if (p.IdPredmeta == idPredmeta)
                {
                    prdmt = p;
                    break;
                }
            }

            foreach (Aktivnost akt in prdmt.Aktivnosti)
            {
                if (akt.GetType() == typeof(Ispit))
                {
                    brojBodova += ((Ispit)akt).Bodovi;
                }
                else
                {
                    brojBodova += ((Zadaća)akt).Bodovi;
                }
            }

            return brojBodova;

        }

        public int dajOcjenuNaPredmetu(int idPredmeta)
        {
            PredmetZaStudenta prdmt = null;
            foreach (PredmetZaStudenta p in predmeti)
            {
                if (p.IdPredmeta == idPredmeta)
                {
                    prdmt = p;
                    break;
                }
            }
            return prdmt.Ocjena;
        }

        public double dajTrenutniProsjek()
        {
            int brPredmeta = 0;
            int sumaOcjena = 0;
            foreach (PredmetZaStudenta p in predmeti)
            {
                if (p.Ocjena > 5)
                {
                    brPredmeta++;
                    sumaOcjena += p.Ocjena;
                }
            }
            if (sumaOcjena == 0) return 5;
            return (double)sumaOcjena / brPredmeta;
        }

        public bool MoveNext()
        {
            throw new NotImplementedException();
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
    }
}