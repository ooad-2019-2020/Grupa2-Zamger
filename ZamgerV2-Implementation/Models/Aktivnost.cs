using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZamgerV2_Implementation.Models
{
    public abstract class Aktivnost
    {
        private string naziv;
        private DateTime krajnjiDatum;
        private int idPredmeta;
        private int idAktivnosti;

        public Aktivnost(string tNaziv, DateTime tKrajnjiDatum, int tIdPredmeta, int tIdAktivnosti)
        {
            this.naziv = tNaziv;
            this.krajnjiDatum = tKrajnjiDatum;
            this.idPredmeta = tIdPredmeta;
            this.idAktivnosti = tIdAktivnosti;
        }

        public string Naziv { get => naziv; set => naziv = value; }
        public DateTime KrajnjiDatum { get => krajnjiDatum; set => krajnjiDatum = value; }
        public int IdPredmeta { get => idPredmeta; set => idPredmeta = value; }
        public int IdAktivnosti { get => idAktivnosti; set => idAktivnosti = value; }
    }
}
