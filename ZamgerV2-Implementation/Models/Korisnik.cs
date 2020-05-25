using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZamgerV2_Implementation.Models
{
    public abstract class Korisnik
    {
        private String ime;
        private String prezime;
        private String datumRođenja;
        private String mjestoPrebivališta;
        private String username;
        private String email;
        private String spol;
        private List<Poruka> inbox;
        private List<Poruka> outbox;

        public Korisnik(String tIme, String tPrezime, String tDatumRođenja, String tMjestoPrebivališta, String tUsername, String tEmail, String tSpol)
        {
            this.ime = tIme;
            this.prezime = tPrezime;
            this.datumRođenja = tDatumRođenja;
            this.mjestoPrebivališta = tMjestoPrebivališta;
            this.username = tUsername;
            this.email = tEmail;
            this.Spol = tSpol;
        }

        public string Ime { get => ime; set => ime = value; }
        public string Prezime { get => prezime; set => prezime = value; }
        public string DatumRođenja { get => datumRođenja; set => datumRođenja = value; }
        public string MjestoPrebivališta { get => mjestoPrebivališta; set => mjestoPrebivališta = value; }
        public string Username { get => username; set => username = value; }
        public string Email { get => email; set => email = value; }
        public string Spol { get => spol; set => spol = value; }
        public List<Poruka> Inbox { get => inbox; set => inbox = value; }
        public List<Poruka> Outbox { get => outbox; set => outbox = value; }
    }
}
