using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ZamgerV2_Implementation.Models
{
    public class Zadaća : Aktivnost
    {
        private int idStudenta;
        private double bodovi;
        private Document rješenjeZadaće;
        private double maxBrojBodova;
        private string putanjaDoZadaće;

        public Zadaća(int redniBroj, int idStudenta, int idPredmeta, string nazivZadaće, double bodovi, DateTime rokIsteka, Document rješenjeZadaće, double maxBrojBodova, string putanjaDoZadaće) : base(nazivZadaće, rokIsteka, idPredmeta, redniBroj)
        {

            this.idStudenta = idStudenta;
            this.bodovi = bodovi;
            this.rješenjeZadaće = rješenjeZadaće;
            this.maxBrojBodova = maxBrojBodova;
            this.putanjaDoZadaće = putanjaDoZadaće;
        }

        public int IdStudenta { get => idStudenta; set => idStudenta = value; }
        public double Bodovi { get => bodovi; set => bodovi = value; }
        public Document RješenjeZadaće { get => rješenjeZadaće; set => rješenjeZadaće = value; }
        public double MaxBrojBodova { get => maxBrojBodova; set => maxBrojBodova = value; }
        public string PutanjaDoZadaće { get => putanjaDoZadaće; set => putanjaDoZadaće = value; }
    }
}
