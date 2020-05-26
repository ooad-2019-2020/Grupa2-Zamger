using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZamgerV2_Implementation.Models
{
    public class Ispit : Aktivnost
    {
        private int idStudenta;
        private double bodovi;
        private int maxBrojBodova;
        private double brojBodovaZaProlaz;

        public Ispit(int idStudenta, int idPredmeta, string naziv, DateTime datum, double bodovi, int idIspita, int maxBrojBodova, double brojBodovaZaProlaz) : base(naziv, datum, idPredmeta, idIspita)
        {
            this.idStudenta = idStudenta;
            this.bodovi = bodovi;
            this.maxBrojBodova = maxBrojBodova;
            this.brojBodovaZaProlaz = brojBodovaZaProlaz;
        }

        public int IdStudenta { get => idStudenta; set => idStudenta = value; }
        public double Bodovi { get => bodovi; set => bodovi = value; }
        public int MaxBrojBodova { get => maxBrojBodova; set => maxBrojBodova = value; }
        public double BrojBodovaZaProlaz { get => brojBodovaZaProlaz; set => brojBodovaZaProlaz = value; }

        
    }
}
