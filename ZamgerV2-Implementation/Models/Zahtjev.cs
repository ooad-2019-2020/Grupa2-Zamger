using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZamgerV2_Implementation.Models
{
    public class Zahtjev
    {
        private int idStudenta;
        private string vrsta;
        private DateTime datum;
        private int odobren;

        public Zahtjev(int idStudenta, string vrsta, DateTime datum, int odobren)
        {
            this.idStudenta = idStudenta;
            this.vrsta = vrsta;
            this.datum = datum;
            this.odobren = odobren;
        }

        public int IdStudenta { get => idStudenta; set => idStudenta = value; }
        public string Vrsta { get => vrsta; set => vrsta = value; }
        public DateTime Datum { get => datum; set => datum = value; }
        public int Odobren { get => odobren; set => odobren = value; }
    }
}
