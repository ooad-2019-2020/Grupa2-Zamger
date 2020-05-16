using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZamgerV2_Implementation.Models
{
    public class KreiranoNastavnoOsobljeViewModel
    {
        private NastavnoOsoblje nastavno;
        List<string> imenaPredmeta;

        public KreiranoNastavnoOsobljeViewModel(NastavnoOsoblje no, List<string> predmeti)
        {
            this.Nastavno = no;
            ImenaPredmeta = predmeti;
        }

        public NastavnoOsoblje Nastavno { get => nastavno; set => nastavno = value; }
        public List<string> ImenaPredmeta { get => imenaPredmeta; set => imenaPredmeta = value; }
    }
}
