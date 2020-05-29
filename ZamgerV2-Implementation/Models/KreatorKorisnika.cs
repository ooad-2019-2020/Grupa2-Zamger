using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZamgerV2_Implementation.Models
{
    public class KreatorKorisnika
    {
        private ZamgerDbContext zmgr;
        public KreatorKorisnika()
        {
            zmgr = ZamgerDbContext.GetInstance();
        }
        public Korisnik FactoryMethod(int id)
        {

            Korisnik trenutniKorisnik;
            int tipKorisnika = zmgr.dajTipKorisnikaPoId(id);
            if (tipKorisnika == 1)
            {
                trenutniKorisnik = zmgr.dajStudentaPoID(id);
                if (trenutniKorisnik.GetType() == typeof(Student))
                {
                    Student temps = (Student)trenutniKorisnik;
                    temps.Predmeti = zmgr.formirajPredmeteZaStudentaPoId(id);
                    foreach (PredmetZaStudenta prdmt in temps.Predmeti)
                    {
                        prdmt.Aktivnosti = zmgr.dajAktivnostiZaStudentovPredmet(prdmt.IdPredmeta, prdmt.IdStudenta);
                    }

                    temps.Inbox = zmgr.dajInbox(id);
                    temps.Outbox = zmgr.dajOutbox(id);
                    return temps;
                }
                else
                {
                    MasterStudent temps = (MasterStudent)trenutniKorisnik;
                    temps.Predmeti = zmgr.formirajPredmeteZaStudentaPoId(id);
                    foreach (PredmetZaStudenta prdmt in temps.Predmeti)
                    {
                        prdmt.Aktivnosti = zmgr.dajAktivnostiZaStudentovPredmet(prdmt.IdPredmeta, prdmt.IdStudenta);
                    }

                    temps.Inbox = zmgr.dajInbox(id);
                    temps.Outbox = zmgr.dajOutbox(id);
                    return temps;
                }
            }
            else if (tipKorisnika == 2 || tipKorisnika == 4)
            {
                trenutniKorisnik = zmgr.dajNastavnoOsobljePoId(id);
                if (trenutniKorisnik.GetType() == typeof(NastavnoOsoblje))
                {
                    NastavnoOsoblje tempOsoba = (NastavnoOsoblje)trenutniKorisnik;
                    tempOsoba.IdOsobe = id;
                    tempOsoba.PredmetiNaKojimPredaje = zmgr.formirajPredmeteZaNastavnoOsobljePoId(id);
                    foreach (PredmetZaNastavnoOsoblje prdmt in tempOsoba.PredmetiNaKojimPredaje)
                    {
                        prdmt.Studenti = zmgr.formirajStudenteNaPredmetuPoId(prdmt.IdPredmeta);
                    }
                    tempOsoba.Aktivnosti = zmgr.formirajAktivnostiZaNastavnoOsobljePoIdOsobe(id);
                    tempOsoba.Inbox = zmgr.dajInbox(id);
                    tempOsoba.Outbox = zmgr.dajOutbox(id);
                    return tempOsoba;
                }
                else
                {
                    Profesor tempOsoba = (Profesor)trenutniKorisnik;
                    tempOsoba.IdOsobe = id;
                    List<Anketa> anketice = new List<Anketa>();
                    tempOsoba.PredmetiNaKojimPredaje = zmgr.formirajPredmeteZaNastavnoOsobljePoId(id);
                    foreach (PredmetZaNastavnoOsoblje prdmt in tempOsoba.PredmetiNaKojimPredaje)
                    {
                        prdmt.Studenti = zmgr.formirajStudenteNaPredmetuPoId(prdmt.IdPredmeta);
                        List<Anketa> tempAnkete = zmgr.dajAnketeZaPredmetPoId(prdmt.IdPredmeta);
                        if (tempAnkete != null)
                        {
                            anketice.AddRange(tempAnkete);
                        }
                    }
                    tempOsoba.Aktivnosti = zmgr.formirajAktivnostiZaNastavnoOsobljePoIdOsobe(id);
                    tempOsoba.AnketeNaPredmetima = anketice;
                    tempOsoba.Inbox = zmgr.dajInbox(id);
                    tempOsoba.Outbox = zmgr.dajOutbox(id);
                    return tempOsoba;
                }
            }
            else
            {
                return null;
            }
        }
    }
}
