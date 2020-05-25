using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZamgerV2_Implementation.Models
{
    public class Poruka
    {
        private int idPosiljaoca;
        private int idPrimaoca;
        private string naslov;
        private string sadrzaj;
        DateTime vrijemePoruke;
        private int procitana;
        private int idPoruke;
        private string userPrimaoca;
        private string userPosiljaoca;


        public Poruka(int idPosiljaoca, int idPrimaoca, string naslov, string sadrzaj, DateTime vrijemePoruke, int procitana, int idPoruke)
        {
            this.IdPosiljaoca = idPosiljaoca;
            this.IdPrimaoca = idPrimaoca;
            this.Naslov = naslov;
            this.Sadrzaj = sadrzaj;
            this.VrijemePoruke = vrijemePoruke;
            this.Procitana = procitana;
            this.IdPoruke = idPoruke;
        }

        public int IdPosiljaoca { get => idPosiljaoca; set => idPosiljaoca = value; }
        public int IdPrimaoca { get => idPrimaoca; set => idPrimaoca = value; }
        public string Naslov { get => naslov; set => naslov = value; }
        public string Sadrzaj { get => sadrzaj; set => sadrzaj = value; }
        public DateTime VrijemePoruke { get => vrijemePoruke; set => vrijemePoruke = value; }
        public int Procitana { get => procitana; set => procitana = value; }
        public int IdPoruke { get => idPoruke; set => idPoruke = value; }
        public string UserPrimaoca { get => userPrimaoca; set => userPrimaoca = value; }
        public string UserPosiljaoca { get => userPosiljaoca; set => userPosiljaoca = value; }
    }
}
