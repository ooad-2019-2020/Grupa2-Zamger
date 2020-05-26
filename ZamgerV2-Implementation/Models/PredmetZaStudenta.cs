using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZamgerV2_Implementation.Models
{
    public class PredmetZaStudenta
    {
        private string naziv;
        private double ectsPoeni;
        private double bodovi;
        private int ocjena;
        private List<Aktivnost> aktivnosti;
        private int idPredmeta;
        private int idStudenta;
        private int studijskaGodina;


        public PredmetZaStudenta(string tNaziv, double tEctsPoeni, double tBodovi, int tOcjena, List<Aktivnost> tAktivnosti, int tIdPredmeta, int tIdStudenta, int tStudijskaGodina)
        {
            this.naziv = tNaziv;
            this.ectsPoeni = tEctsPoeni;
            this.bodovi = tBodovi;
            this.ocjena = tOcjena;
            this.aktivnosti = tAktivnosti;
            this.idPredmeta = tIdPredmeta;
            this.idStudenta = tIdStudenta;
            this.studijskaGodina = tStudijskaGodina;
        }

        public string Naziv { get => naziv; set => naziv = value; }
        public double EctsPoeni { get => ectsPoeni; set => ectsPoeni = value; }
        public double Bodovi { get => bodovi; set => bodovi = value; }
        public int Ocjena { get => ocjena; set => ocjena = value; }
        public List<Aktivnost> Aktivnosti { get => aktivnosti; set => aktivnosti = value; }
        public int IdPredmeta { get => idPredmeta; set => idPredmeta = value; }
        public int IdStudenta { get => idStudenta; set => idStudenta = value; }
        
        public int StudijskaGodina { get => studijskaGodina; set => studijskaGodina = value; }
    }
}
