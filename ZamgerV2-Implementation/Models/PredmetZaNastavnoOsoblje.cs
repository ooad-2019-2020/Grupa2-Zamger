using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZamgerV2_Implementation.Models
{
    public class PredmetZaNastavnoOsoblje
    {
        private string naziv;
        private int idPredmeta;
        private double ectsPoeni;
        List<Student> studenti;
        List<Aktivnost> aktivnosti;

        public PredmetZaNastavnoOsoblje(string naziv, int idPredmeta, double ectsPoeni, List<Student> studenti, List<Aktivnost> aktivnosti)
        {
            this.naziv = naziv;
            this.idPredmeta = idPredmeta;
            this.ectsPoeni = ectsPoeni;
            this.studenti = studenti;
            this.aktivnosti = aktivnosti;
        }

        public string Naziv { get => naziv; set => naziv = value; }
        public int IdPredmeta { get => idPredmeta; set => idPredmeta = value; }
        public double EctsPoeni { get => ectsPoeni; set => ectsPoeni = value; }
        public List<Student> Studenti { get => studenti; set => studenti = value; }
        public List<Aktivnost> Aktivnosti { get => aktivnosti; set => aktivnosti = value; }
    }
}
