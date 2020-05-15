using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZamgerV2_Implementation.Models
{
    public class KreiranPredmetViewModel
    {
        private String naziv;
        private double ECTSPoeni;
        private List<int> godineDostupnosti;
        private List<string> odsjeciDostupnosti;
        private bool izborni;
        public KreiranPredmetViewModel(String tNaziv, double tECTS, List<int> tGodineDostupnosti, List<string> tOdsjeciDostupnosti, int tIzborni)
        {

            this.Naziv = tNaziv;
            this.ECTSPoeni = tECTS;
            this.GodineDostupnosti = tGodineDostupnosti;
            this.OdsjeciDostupnosti = tOdsjeciDostupnosti;
            if(tIzborni==1)
            {
                Izborni = true;
            }
            else
            {
                Izborni = false;
            }
        }

        public string Naziv { get => naziv; set => naziv = value; }
        public double EctsPoeni { get => ECTSPoeni; set => ECTSPoeni = value; }
        public List<int> GodineDostupnosti { get => godineDostupnosti; set => godineDostupnosti = value; }
        public List<string> OdsjeciDostupnosti { get => odsjeciDostupnosti; set => odsjeciDostupnosti = value; }
        public bool Izborni { get => izborni; set => izborni = value; }
    }
}
