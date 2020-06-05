using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZamgerV2_Implementation.Models
{
    public class Anketa
    {
        private string naziv;
        private DateTime datumIsteka;
        private List<String> pitanja;
        private int idAnkete;
        private List<OdgovorNaAnketu> odgovori;
        private int idPredmeta;

        public Anketa(string naziv, DateTime datumIsteka, List<string> pitanja, int idAnkete, List<OdgovorNaAnketu> odgovori, int idPredmeta)
        {
            this.naziv = naziv;
            this.datumIsteka = datumIsteka;
            this.pitanja = pitanja;
            this.idAnkete = idAnkete;
            this.odgovori = odgovori;
            this.IdPredmeta = idPredmeta;
        }

        public string Naziv { get => naziv; set => naziv = value; }
        public DateTime DatumIsteka { get => datumIsteka; set => datumIsteka = value; }
        public List<string> Pitanja { get => pitanja; set => pitanja = value; }
        public int IdAnkete { get => idAnkete; set => idAnkete = value; }
        public List<OdgovorNaAnketu> Odgovori { get => odgovori; set => odgovori = value; }
        public int IdPredmeta { get => idPredmeta; set => idPredmeta = value; }
    }
}
