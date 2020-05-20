using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZamgerV2_Implementation.Models
{
    public class OdgovorNaAnketu
    {
        private int idAnkete;
        private int idStudenta;
        private List<String> odgovori;
        private String komentar;
        private int ocjenaPredmeta;

        public OdgovorNaAnketu(int idAnkete, int idStudenta, List<string> odgovori, string komentar, int ocjenaPredmeta)
        {
            this.idAnkete = idAnkete;
            this.idStudenta = idStudenta;
            this.odgovori = odgovori;
            this.komentar = komentar;
            this.ocjenaPredmeta = ocjenaPredmeta;
        }

        public int IdAnkete { get => idAnkete; set => idAnkete = value; }
        public int IdStudenta { get => idStudenta; set => idStudenta = value; }
        public List<string> Odgovori { get => odgovori; set => odgovori = value; }
        public string Komentar { get => komentar; set => komentar = value; }
        public int OcjenaPredmeta { get => ocjenaPredmeta; set => ocjenaPredmeta = value; }
    }
}
