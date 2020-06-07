using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZamgerV2_Implementation.Models
{
    public interface ISortiranje
    {
        public bool uporedi(Korisnik k1, Korisnik k2);
    }
    public class Sortiranje
    {
        private List<Korisnik> korisnici;

        public Sortiranje(List<Korisnik> kolekcija)
        {
            Korisnici = kolekcija;
        }

        public List<Korisnik> Korisnici { get => korisnici; set => korisnici = value; }

        public void sortiraj()
        {
            Korisnici.Sort();
        }
    }
    public class StudentiSort : Sortiranje, ISortiranje
    {
        public StudentiSort(List<Korisnik> studenti) : base(studenti)
        {

        }
        public bool uporedi(Korisnik k1, Korisnik k2)
        {
            int brojPolozenihk1 = 0;
            int brojPolozenihk2 = 0;
            List<PredmetZaStudenta> p1 = ((Student)k1).Predmeti;
            List<PredmetZaStudenta> p2 = ((Student)k2).Predmeti;
            foreach (PredmetZaStudenta p in p1)
            {
                if (p.Ocjena > 5)
                {
                    brojPolozenihk1++;
                }
            }
            foreach (PredmetZaStudenta p in p2)
            {
                if (p.Ocjena > 5)
                {
                    brojPolozenihk2++;
                }
            }
            if (brojPolozenihk1 > brojPolozenihk2)
            {
                return true;
            }
            else
            {
                return (((Student)k1).dajTrenutniProsjek() > ((Student)k2).dajTrenutniProsjek());
            }
        }
    }
    public class NastavnoOsobljeSort : Sortiranje, ISortiranje
    {
        public NastavnoOsobljeSort(List<Korisnik> k) : base(k)
        {

        }

        public bool uporedi(Korisnik k1, Korisnik k2)
        {
            return ((NastavnoOsoblje)k1).PredmetiNaKojimPredaje.Count > ((NastavnoOsoblje)k2).PredmetiNaKojimPredaje.Count;
        }
    }
}