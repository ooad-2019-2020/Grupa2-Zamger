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

        public Obavještenje(String tNaslov,String tSadržaj, DateTime tVrijemeObavještenja)
        {
            this.naslov = tNaslov;
            this.sadržaj = tSadržaj;
            this.vrijemeObavještenja = tVrijemeObavještenja;
        }
        public String Naslov { get=>naslov; set=>naslov=value; }
        public String Sadržaj { get=>sadržaj; set=>sadržaj=value; }
        public DateTime dateTime { get=>vrijemeObavještenja; set=>vrijemeObavještenja=value; }
    }
}
