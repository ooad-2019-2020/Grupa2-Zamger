using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZamgerV2_Implementation.Models
{
    public class Obavještenje
    {
        private String naslov;
        private String sadržaj;
        private DateTime vrijemeObavještenja;
        private int idObavještenja;

        public Obavještenje(String tNaslov,String tSadržaj, DateTime tVrijemeObavještenja, int tidObavještenja)
        {
            this.naslov = tNaslov;
            this.sadržaj = tSadržaj;
            this.vrijemeObavještenja = tVrijemeObavještenja;
            this.idObavještenja = tidObavještenja;
        }
        public String Naslov { get=>naslov; set=>naslov=value; }
        public String Sadržaj { get=>sadržaj; set=>sadržaj=value; }
        public DateTime dateTime { get=>vrijemeObavještenja; set=>vrijemeObavještenja=value; }
        public int IdObavještenja { get => idObavještenja; set => idObavještenja = value; }
    }
}
