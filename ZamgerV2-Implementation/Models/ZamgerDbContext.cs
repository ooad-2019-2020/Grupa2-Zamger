using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Razor.Language.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ZamgerV2_Implementation.Models
{
    public class ZamgerDbContext 
    {
        private static SqlConnection conn = null;
        private static ZamgerDbContext instance = null;

        private ZamgerDbContext()
        {

            String connString = "server=DESKTOP-0G31M9N;database=zamgerDB-new;Trusted_Connection=true;MultipleActiveResultSets=true";

            try
            {
                conn = new SqlConnection(connString);
                conn.Open();
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.StackTrace + "Greška pri uspostavi konekcije");
                conn.Close();
            }
        }

        public static ZamgerDbContext GetInstance()
        {
            if (instance == null)
            {
                instance = new ZamgerDbContext();
            }
            return instance;
        }

        public static void removeInstance()
        {
            if (instance == null) return;
            instance.close();
            instance = null;
        }

        private void close()
        {
            try
            {
                conn.Close();
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.StackTrace + "greška pri uništavanju ZamgerDbContext singletonea");
            }
        }

        public List<Poruka> dajOutbox(int id)
        {
            List<Poruka> lista = new List<Poruka>();
            string kveri = "select pošiljalac, primalac, naslov, sadržaj, vrijemePoruke, pročitana, idPoruke, k1.username, k2.username from DOPISIVANJE, KORISNICI k1, KORISNICI k2 Where pošiljalac = k1.idKorisnika AND primalac = k2.idKorisnika AND pošiljalac = @pId ORDER BY vrijemePoruke DESC";
            var idParam = new SqlParameter("pId", System.Data.SqlDbType.Int);
            SqlCommand komanda = new SqlCommand(kveri, conn);
            idParam.Value = id;
            komanda.Parameters.Add(idParam);
            try
            {
                var result = komanda.ExecuteReader();
                if(result.HasRows)
                {
                    while(result.Read())
                    {
                        Poruka privremenaPoruka = new Poruka(result.GetInt32(0), result.GetInt32(1), result.GetString(2), result.GetString(3), result.GetDateTime(4), result.GetInt32(5), result.GetInt32(6));
                        privremenaPoruka.UserPosiljaoca = result.GetString(7);
                        privremenaPoruka.UserPrimaoca = result.GetString(8);
                        lista.Add(privremenaPoruka);
                    }
                    return lista;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.StackTrace + "-nije ok");
            }

        }
        
        public Poruka dajPoruku(int id)
        {
            Poruka p;
            string kveri = "select pošiljalac, primalac, naslov, sadržaj, vrijemePoruke, pročitana, idPoruke, k1.username, k2.username from DOPISIVANJE, KORISNICI k1, KORISNICI k2 Where pošiljalac = k1.idKorisnika AND primalac = k2.idKorisnika AND idPoruke = @porukaId ";
            var porukaIdParam = new SqlParameter("porukaId", System.Data.SqlDbType.Int);
            SqlCommand komanda = new SqlCommand(kveri, conn);
            porukaIdParam.Value = id;
            komanda.Parameters.Add(porukaIdParam);
            try
            {
                var result = komanda.ExecuteReader();
                if(result.Read())
                {
                    Poruka privremenaPoruka = new Poruka(result.GetInt32(0), result.GetInt32(1), result.GetString(2), result.GetString(3), result.GetDateTime(4), result.GetInt32(5), result.GetInt32(6));
                    privremenaPoruka.UserPosiljaoca = result.GetString(7);
                    privremenaPoruka.UserPrimaoca = result.GetString(8);
                    return privremenaPoruka;
                }
                else
                {
                    return null;
                }
                
            }
            catch(Exception e)
            {
                throw new Exception(e.StackTrace + "-nije ok");
            }
        }
        public List<Poruka> dajInbox(int id)
        {
            List<Poruka> lista = new List<Poruka>();
            string kveri = "select pošiljalac, primalac, naslov, sadržaj, vrijemePoruke, pročitana, idPoruke, k1.username, k2.username from DOPISIVANJE, KORISNICI k1, KORISNICI k2 Where pošiljalac = k1.idKorisnika AND primalac = k2.idKorisnika AND primalac = @pId ORDER BY vrijemePoruke DESC";
            var idParam = new SqlParameter("pId", System.Data.SqlDbType.Int);
            SqlCommand komanda = new SqlCommand(kveri, conn);
            idParam.Value = id;
            komanda.Parameters.Add(idParam);
            try
            {
                var result = komanda.ExecuteReader();
                if(result.HasRows)
                {
                    while(result.Read())
                    {
                        Poruka privremenaPoruka = new Poruka(result.GetInt32(0), result.GetInt32(1), result.GetString(2), result.GetString(3), result.GetDateTime(4), result.GetInt32(5), result.GetInt32(6));
                        privremenaPoruka.UserPosiljaoca = result.GetString(7);
                        privremenaPoruka.UserPrimaoca = result.GetString(8);
                        lista.Add(privremenaPoruka);
                    }
                    return lista;
                }
                else
                {
                    return null;
                }
            }
            catch(Exception e)
            {
                throw new Exception(e.StackTrace + "-nije ok");
            }
        }

        public bool posaljiPoruku(Poruka poruka)
        {
            string kveri = "INSERT INTO DOPISIVANJE VALUES(@posId, @primId , @naslov, @sadržaj, @vrijemePoruke, @pročitana, @idPoruke)";
            SqlParameter posIdParam = new SqlParameter("posId", System.Data.SqlDbType.Int);
            SqlParameter primIdParam = new SqlParameter("primId", System.Data.SqlDbType.Int);
            SqlParameter naslovParam = new SqlParameter("naslov", System.Data.SqlDbType.NVarChar);
            SqlParameter sadrzajParam = new SqlParameter("sadržaj", System.Data.SqlDbType.NVarChar);
            SqlParameter vrijemeParam = new SqlParameter("vrijemePoruke", System.Data.SqlDbType.DateTime);
            SqlParameter procitanaParam = new SqlParameter("pročitana", System.Data.SqlDbType.Int);
            SqlParameter idPorukeParam = new SqlParameter("idPoruke", System.Data.SqlDbType.Int);
            posIdParam.Value = poruka.IdPosiljaoca;
            primIdParam.Value = poruka.IdPrimaoca;
            naslovParam.Value = poruka.Naslov;
            sadrzajParam.Value = poruka.Sadrzaj;
            vrijemeParam.Value = poruka.VrijemePoruke;
            procitanaParam.Value = poruka.Procitana;
            idPorukeParam.Value = poruka.IdPoruke;
            SqlCommand komanda = new SqlCommand(kveri, conn);
            komanda.Parameters.Add(posIdParam);
            komanda.Parameters.Add(primIdParam);
            komanda.Parameters.Add(naslovParam);
            komanda.Parameters.Add(sadrzajParam);
            komanda.Parameters.Add(vrijemeParam);
            komanda.Parameters.Add(procitanaParam);
            komanda.Parameters.Add(idPorukeParam);
            try
            {
                komanda.ExecuteNonQuery();
                return true;
            }
            catch(Exception e)
            {
                
                throw new Exception(e.StackTrace+"Slanje poruke neuspješno");
            }
        }

        public bool oznaciProcitanu(int idPoruke)
        {
            string kveri = "UPDATE DOPISIVANJE SET pročitana = 1 WHERE idPoruke = @id";
            SqlParameter idParam = new SqlParameter("id", System.Data.SqlDbType.Int);
            idParam.Value = idPoruke;
            SqlCommand komanda = new SqlCommand(kveri, conn);
            komanda.Parameters.Add(idParam);
            try
            {
                komanda.ExecuteNonQuery();
                return true;
            }
            catch(Exception e)
            {
                throw new Exception("Greška pri označavanju poruke pročitanom");
            }
        }

        public int dajNoviPorukaId()
        {
            string kveri = "SELECT isnull(max(idPoruke), 0)+1 FROM DOPISIVANJE";
            SqlCommand komanda = new SqlCommand(kveri, conn);
            try
            {
                int resultID = (int)komanda.ExecuteScalar();
                return resultID;
            }
            catch(Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.StackTrace + "error u generisanju ID");
                return -1;
            }
        }

        public Student dajStudentaPoID(int? idStudenta)
        {
            Student s;
            string sqlKveri = "select ime, prezime, datumRođenja, mjestoPrebivališta, username, email, spol, odsjek, brojIndeksa, prosjekNaBSC, godinaStudija, BSC from korisnici k, studenti s where s.brojIndeksa=k.idKorisnika and s.brojIndeksa=@userID";
            var userIDParam = new SqlParameter("userID", System.Data.SqlDbType.Int);
            userIDParam.Value = idStudenta;
            SqlCommand command = new SqlCommand(sqlKveri, conn);
            command.Parameters.Add(userIDParam);

            try
            {
                var result = command.ExecuteReader();
                if (result.HasRows)
                {
                    result.Read();
                    if (result.GetInt32(11) == 1) //ako mu je vrijednost BSC kolone 1, to znači da je završio BSC i da je to MasterStudent
                    {
                        Student tempS = new MasterStudent(result.GetValue(0).ToString(), result.GetValue(1).ToString(), result.GetValue(2).ToString(), result.GetValue(3).ToString(), result.GetValue(4).ToString(), result.GetValue(5).ToString(), result.GetValue(6).ToString(), result.GetValue(7).ToString(), result.GetInt32(8), result.GetFloat(9));
                        tempS.GodinaStudija = result.GetInt32(10);
                        return tempS;
                    }
                    else
                    {
                        Student tempS = new Student(result.GetValue(0).ToString(), result.GetValue(1).ToString(), result.GetValue(2).ToString(), result.GetValue(3).ToString(), result.GetValue(4).ToString(), result.GetValue(5).ToString(), result.GetValue(6).ToString(), result.GetValue(7).ToString(), result.GetInt32(8));
                        tempS.GodinaStudija = result.GetInt32(10);
                        return tempS;
                    }

                }
                return null;
            }
            catch (Exception e)
            {
                throw new Exception(e.StackTrace + "greška prilikom pretrage studenta po ID iz baze");
            }
        }

        internal void uploadujZadacu(int idPredmeta, int idZadaće, int? brojIndeksa, string nazivFajla)
        {
            string kveri = "update zadaće set putanjaDoZadaće = @nazivFajla where idPredmeta = @predmetID and redniBroj=@zadacaID and idStudenta=@studentID";
            SqlCommand cmd = new SqlCommand(kveri, conn);
            cmd.Parameters.AddWithValue("nazivFajla", nazivFajla);
            cmd.Parameters.AddWithValue("zadacaID", idZadaće);
            cmd.Parameters.AddWithValue("studentID", brojIndeksa.Value);
            cmd.Parameters.AddWithValue("predmetID", idPredmeta);
            try
            {
                cmd.ExecuteNonQuery();
            }catch(Exception e) { throw new Exception(e.StackTrace + " greška prilikom uploadanja zadaće!"); }

        }

        public List<PredmetZaStudenta> formirajPredmeteZaStudentaPoId(int id)
        {
            List<PredmetZaStudenta> lista = new List<PredmetZaStudenta>();

            string kveri = "select p.naziv, p.ectsPoeni, o.bodovi, o.ocjena, o.idPredmeta, o.idStudenta, o.studijskaGodina from PREDMETI p, OCJENE o where p.idPredmeta=o.idPredmeta and o.idStudenta=@idStudenta";
            
            var userIDParam = new SqlParameter("idStudenta", System.Data.SqlDbType.Int);
            userIDParam.Value = id;
            SqlCommand command = new SqlCommand(kveri, conn);
            command.Parameters.Add(userIDParam);

            try
            {
                var result = command.ExecuteReader();
                if(result.HasRows)
                {
                    while(result.Read())
                    {
                        PredmetZaStudenta predmet = new PredmetZaStudenta(result.GetString(0), result.GetFloat(1), result.GetFloat(2), result.GetInt32(3), null, result.GetInt32(4), result.GetInt32(5), result.GetInt32(6));
                        predmet.Aktivnosti = this.dajAktivnostiZaStudentovPredmet(predmet.IdPredmeta, id);
                        lista.Add(predmet);
                    }
                    return lista;
                }
                else
                {
                    throw new Exception("Nešto nije u redu prilikom stvaranja predmeta za studenta");
                }
            }
            catch(Exception e)
            {
                throw new Exception(e.StackTrace + "-nije ok");
            }
        }

        public List<Aktivnost> dajAktivnostiZaStudentovPredmet(int idPredmet, int idStudenta)
        {
            List<Aktivnost> aktivnosti = new List<Aktivnost>();
            //prvo sve ispite
            string kveri = "select idStudenta, idPredmeta, naziv, datum, bodovi, idIspita, maxBrojBodova, brojBodovaZaProlaz from ispiti where idPredmeta = @predmetID and idStudenta = @studentID";
            var userIDParam = new SqlParameter("studentID", System.Data.SqlDbType.Int);
            userIDParam.Value = idStudenta;
            var predmetIDParam = new SqlParameter("predmetID", System.Data.SqlDbType.Int);
            predmetIDParam.Value = idPredmet;

            SqlCommand command = new SqlCommand(kveri, conn);
            command.Parameters.Add(userIDParam);
            command.Parameters.Add(predmetIDParam);

            //pa onda sve zadaće
            string kveri2 = "select redniBroj, idStudenta, idPredmeta, nazivZadaće, bodovi, rokIsteka, rješenjeZadaće, maxBrojBodova, putanjaDoZadaće from zadaće where idPredmeta = @IDpredmet and idStudenta = @IDStudent";
            var userIDParam2 = new SqlParameter("IDStudent", System.Data.SqlDbType.Int);
            userIDParam2.Value = idStudenta;
            var predmetIDParam2 = new SqlParameter("IDPredmet", System.Data.SqlDbType.Int);
            predmetIDParam2.Value = idPredmet;

            SqlCommand command2 = new SqlCommand(kveri2, conn);
            command2.Parameters.Add(userIDParam2);
            command2.Parameters.Add(predmetIDParam2);

            try
            {
                var result = command.ExecuteReader();
                var result2 = command2.ExecuteReader();

                if(result.HasRows)
                {
                    while(result.Read())
                    {
                        aktivnosti.Add(new Ispit(result.GetInt32(0), result.GetInt32(1), result.GetString(2), result.GetDateTime(3), result.GetFloat(4), result.GetInt32(5), result.GetInt32(6), result.GetFloat(7)));
                    }
                }
                if(result2.HasRows)
                {
                    while(result2.Read())
                    {
                        if (!result2.IsDBNull(8))
                        {
                            aktivnosti.Add(new Zadaća(result2.GetInt32(0), result2.GetInt32(1), result2.GetInt32(2), result2.GetString(3), result2.GetFloat(4), result2.GetDateTime(5), null, result2.GetFloat(7), result2.GetString(8)));
                        }
                        else
                        {
                            aktivnosti.Add(new Zadaća(result2.GetInt32(0), result2.GetInt32(1), result2.GetInt32(2), result2.GetString(3), result2.GetFloat(4), result2.GetDateTime(5), null, result2.GetFloat(7), null));
                        }
                    }
                }

                return aktivnosti;
            }
            catch(Exception e)
            {
                throw new Exception(e.StackTrace + "nešto ne valja prilikom učitavanja zadaća i ispita za neki studentov predmet");
            }
        }


        public NastavnoOsoblje dajNastavnoOsobljePoId(int id)
        {
            NastavnoOsoblje no;
            int tipKorisnika = this.dajTipKorisnikaPoId(id);
                string kveri = "select no.ime, no.prezime, no.datumRođenja, no.mjestoPrebivališta, k.username, no.email, no.spol, no.titula from NASTAVNO_OSOBLJE no, KORISNICI k where k.idKorisnika=no.idOsobe and no.idOsobe=@userID";
                var userIDParam = new SqlParameter("userID", System.Data.SqlDbType.Int);
                userIDParam.Value = id;
                SqlCommand command = new SqlCommand(kveri, conn);
                command.Parameters.Add(userIDParam);
                try
                {
                    var result = command.ExecuteReader();
                    if(result.HasRows)
                    {
                        result.Read();
                        if (tipKorisnika == 2)
                        {
                            return new NastavnoOsoblje(result.GetString(0), result.GetString(1), result.GetDateTime(2).ToString(), result.GetString(3), result.GetString(4), result.GetString(5), result.GetString(6), result.GetString(7));
                        }
                        else if(tipKorisnika==4)
                        {
                            return new Profesor(result.GetString(0), result.GetString(1), result.GetDateTime(2).ToString(), result.GetString(3), result.GetString(4), result.GetString(5), result.GetString(6), result.GetString(7));
                        }
                        else
                        {
                            return null; //nešto ne valja, pozvala se ova metoda a osoba nije nit profesor nit nastavnik
                        }
                    }
                    else
                    {
                    throw new Exception("greška prilikom dobavljanja podataka o nastavnom osoblju");
                    }
                }
                catch(Exception e)
                {
                    throw new Exception(e.StackTrace);
                }
        }


        public int dajTipKorisnikaPoId(int id)
        {
            string kveri = "select tipKorisnika from korisnici where idKorisnika = @userID";
            var userIDParam = new SqlParameter("userID", System.Data.SqlDbType.Int);
            userIDParam.Value = id;
            SqlCommand command = new SqlCommand(kveri, conn);
            command.Parameters.Add(userIDParam);
            try
            {
                return (int)command.ExecuteScalar(); //možda će trebat executeReader ako šta bude bacalo
            }
            catch(Exception e)
            {
                
                throw new Exception(e.StackTrace + "greška prilikom dobavljanja tipa korisnika iz baze");
            }
        }

        public List<PredmetZaNastavnoOsoblje> formirajPredmeteZaNastavnoOsobljePoId(int id)
        {
            List<PredmetZaNastavnoOsoblje> lista = new List<PredmetZaNastavnoOsoblje>();
            string kveri = "select p.naziv, p.idPredmeta, p.ectsPoeni from predmeti p, ANSAMBL a, NASTAVNO_OSOBLJE no where a.idNastavnogOsoblja=no.idOsobe and a.idPredmeta=p.idPredmeta and no.idOsobe=@userID";
            var userIDParam = new SqlParameter("userID", System.Data.SqlDbType.Int);
            userIDParam.Value = id;
            SqlCommand command = new SqlCommand(kveri, conn);
            command.Parameters.Add(userIDParam);
            try
            {
                var result = command.ExecuteReader();
                if(result.HasRows)
                {
                    while(result.Read())
                    {
                        lista.Add(new PredmetZaNastavnoOsoblje(result.GetString(0), result.GetInt32(1), result.GetFloat(2), null, null));
                    }
                    return lista;
                }
                else
                {
                    return null;
                }
            }
            catch(Exception e)
            {
                throw new Exception(e.StackTrace + "nešto nije uredu prilikom formiranja predmeta za nastavno osoblje");
            }
        }


        public List<int> dajIdeveStudenataNaPredmetu(int id)
        {
            List<int> idevi = new List<int>();
            string kveri = "select idStudenta from ocjene where idPredmeta = @predmetID group by idStudenta";
            var predmetIDParam = new SqlParameter("predmetID", System.Data.SqlDbType.Int);
            predmetIDParam.Value = id;
            SqlCommand command = new SqlCommand(kveri, conn);
            command.Parameters.Add(predmetIDParam);
            try
            {
                var result = command.ExecuteReader();
                if(result.HasRows)
                {
                    while(result.Read())
                    {
                        idevi.Add(result.GetInt32(0));
                    }
                    return idevi;
                }
                else
                {
                    return null;
                }
            }
            catch(Exception e)
            {
                throw new Exception(e.StackTrace + "greška prilikom dobavljana ideva studenata za predmet");
            }
        }

        public List<Student> formirajStudenteNaPredmetuPoId(int id)
        {
            List<Student> studenti = new List<Student>();
            var idevi = this.dajIdeveStudenataNaPredmetu(id);
            try
            {
                foreach (int studentID in idevi)
                {
                    Student tempStudent = this.dajStudentaPoID(studentID);
                    tempStudent.Predmeti = this.formirajPredmeteZaStudentaPoId(studentID);
                    foreach (PredmetZaStudenta prdmt in tempStudent.Predmeti)
                    {
                        prdmt.Aktivnosti = this.dajAktivnostiZaStudentovPredmet(prdmt.IdPredmeta, prdmt.IdStudenta);
                    }
                    studenti.Add(tempStudent);
                }
                return studenti;
            }
            catch(Exception e)
            {
                throw new Exception(e.StackTrace + "greška prilikom formiranja studenata na predmetu");
            }
        }

        public List<Aktivnost> formirajAktivnostiZaNastavnoOsobljePoIdOsobe(int id)
        {
            List<Aktivnost> aktivnosti = new List<Aktivnost>();
            string kveri = "select a.idAktivnosti , a.idPredmeta, a.naziv, a.rok, a.vrsta, a.maxBrojBodova from AKTIVNOSTI a, PREDMETI p, ANSAMBL an, NASTAVNO_OSOBLJE no where a.idPredmeta=p.idPredmeta and p.idPredmeta=an.idPredmeta and an.idNastavnogOsoblja=no.idOsobe and no.idOsobe=@userID order by a.rok asc";
            var userIDParam = new SqlParameter("userID", System.Data.SqlDbType.Int);
            userIDParam.Value = id;
            SqlCommand command = new SqlCommand(kveri, conn);
            command.Parameters.Add(userIDParam);
            try
            {
                var result = command.ExecuteReader();
                if(result.HasRows)
                {
                    while(result.Read())
                    {
                        if(result.GetString(4).Equals("Ispit"))
                        {
                            aktivnosti.Add(new Ispit(-1,result.GetInt32(1), result.GetString(2), result.GetDateTime(3),0,result.GetInt32(0), result.GetInt32(5),0));
                        }
                        else
                        {
                            aktivnosti.Add(new Zadaća(result.GetInt32(0), -1, result.GetInt32(1), result.GetString(2), 0, result.GetDateTime(3), null, result.GetInt32(5), null));
                        }
                    }
                    return aktivnosti;
                }
                else
                {
                    return null;
                }
            }
            catch(Exception e)
            {
                throw new Exception(e.StackTrace + "greška prilikom dobijanja svih aktivnosti za nastavno osoblje po id");
            }
        }

        public List<OdgovorNaAnketu> dajOdgovoreNaAnketuPoIdAnkete(int id)
        {
            List<OdgovorNaAnketu> odgovori = new List<OdgovorNaAnketu>();
            string kveri = "select idAnkete, idStudenta, odgovor1, odgovor2, odgovor3, odgovor4, odgovor5, komentarStudenta, ocjenaPredmeta from ODGOVORI_NA_ANKETU where idAnkete = @anketaID";
            var anketaIDParam = new SqlParameter("anketaID", System.Data.SqlDbType.Int);
            anketaIDParam.Value = id;
            SqlCommand command = new SqlCommand(kveri, conn);
            command.Parameters.Add(anketaIDParam);
            try
            {
                var result = command.ExecuteReader();
                if(result.HasRows)
                {
                    while(result.Read())
                    {
                        OdgovorNaAnketu tempOdg = new OdgovorNaAnketu(result.GetInt32(0), result.GetInt32(1), null, result.GetString(7), result.GetInt32(8));
                        tempOdg.Odgovori = new List<string>();
                        for(int i = 2; i<7; i++)
                        {
                            tempOdg.Odgovori.Add(result.GetString(i));
                        }
                        odgovori.Add(tempOdg);
                    }
                    return odgovori;
                }
                else
                {
                    return null;
                }
            }
            catch(Exception e)
            {
                throw new Exception(e.StackTrace + "greška prilikom dobijanja odgovora na neku anketu");
            }
        }

        public List<Anketa> dajAnketeZaPredmetPoId(int id)
        {
            List<Anketa> ankete = new List<Anketa>();
            string kveri = "select naziv, datum, pitanje1, pitanje2, pitanje3, pitanje4, pitanje5, idAnkete, idPredmeta from ANKETE_NA_PREDMETIMA where idPredmeta = @predmetID order by datum asc";
            var predmetIDParam = new SqlParameter("predmetID", System.Data.SqlDbType.Int);
            predmetIDParam.Value = id;
            SqlCommand command = new SqlCommand(kveri, conn);
            command.Parameters.Add(predmetIDParam);
            try
            {
                var result = command.ExecuteReader();
                if(result.HasRows)
                {
                    while (result.Read())
                    {
                        Anketa tempAnketa = new Anketa(result.GetString(0), result.GetDateTime(1), null, result.GetInt32(7), null, result.GetInt32(8));
                        tempAnketa.Pitanja = new List<string>();
                        for (int i = 2; i < 7; i++)
                        {
                            tempAnketa.Pitanja.Add(result.GetString(i));
                        }
                        tempAnketa.Odgovori = this.dajOdgovoreNaAnketuPoIdAnkete(tempAnketa.IdAnkete);
                        ankete.Add(tempAnketa);
                    }
                    return ankete;
                }
                else
                {
                    return null;
                }
            }
            catch(Exception e)
            {
                throw new Exception(e.StackTrace + "greška prilikom učitavanja svih anketa za pojedini predmet");
            }
        }

        public bool spremiZahtjev(Zahtjev z)
        {
            string kveri = "insert into zahtjevi values(@userID, @vrsta, @datum, 0, @zahtjevID)";
            var userIDParam = new SqlParameter("userID", SqlDbType.Int);
            userIDParam.Value = z.IdStudenta;
            var vrstaIDParam = new SqlParameter("vrsta", SqlDbType.NVarChar);
            vrstaIDParam.Value = z.Vrsta;
            var datumIDParam = new SqlParameter("datum", SqlDbType.DateTime);
            datumIDParam.Value = z.Datum;
            var zahtjevIDParam = new SqlParameter("zahtjevID", SqlDbType.Int);
            zahtjevIDParam.Value = z.IdZahtjeva;
            SqlCommand command = new SqlCommand(kveri, conn);
            command.Parameters.Add(userIDParam);
            command.Parameters.Add(vrstaIDParam);
            command.Parameters.Add(datumIDParam);
            command.Parameters.Add(zahtjevIDParam);
            try
            {
                command.ExecuteNonQuery();
                return true;
            }
            catch(Exception e)
            {
                throw new Exception(e.StackTrace + "nešto nije u redu prilikom kreiranja novog zahtjeva");
            }
        }

        public List<Zahtjev> dajZahtjeveZaStudenta(int id)
        {
            List<Zahtjev> zahtjevi = new List<Zahtjev>();
            string kveri = "select idStudenta, vrsta, datum, odobren, idZahtjeva from ZAHTJEVI where idStudenta = @userID";
            var userIDParam = new SqlParameter("userID", SqlDbType.Int);
            userIDParam.Value = id;
            SqlCommand command = new SqlCommand(kveri, conn);
            command.Parameters.Add(userIDParam);
            try
            {
                var result = command.ExecuteReader();
                if(result.HasRows)
                {
                    while(result.Read())
                    {
                        zahtjevi.Add(new Zahtjev(result.GetInt32(0), result.GetString(1), result.GetDateTime(2), result.GetInt32(3), result.GetInt32(4)));
                    }
                    return zahtjevi;
                }
                else
                {
                    return null;
                }
            }
            catch(Exception e)
            {
                throw new Exception(e.StackTrace + "greška prilikom dobavljanja zahtjeva za studenta");
            }
        }

        public int generišiIdZahtjeva()
        {
            string kveri = "select isnull(Max(idZahtjeva),0)+1 from ZAHTJEVI";
            SqlCommand cmd = new SqlCommand(kveri, conn);
            try
            {
                return (int)cmd.ExecuteScalar();
            }
            catch(Exception e)
            {
                throw new Exception(e.StackTrace + "greška prilikom generisanja id za zahtjev");
            }
        }



        public int dajBrojPoloženihPredmeta(int? indeks)
        {
            string kveri = "select Count(idPredmeta) from ocjene where ocjena>5 and idStudenta=@StudentID";
            var studentIDParam = new SqlParameter("StudentID", SqlDbType.Int);
            studentIDParam.Value = indeks;
            SqlCommand command = new SqlCommand(kveri, conn);
            command.Parameters.Add(studentIDParam);
            try
            {
                return (int)command.ExecuteScalar();
            }
            catch(Exception e)
            {

            }
            return 0;
        }

        public int dajBrojNepoloženihPredmeta(int? indeks)
        {
            int brojPoloženih = dajBrojPoloženihPredmeta(indeks);

            string kveri = "select Count(distinct idPredmeta) from ocjene where idStudenta=@StudentID";
            var studentIDParam = new SqlParameter("StudentID", SqlDbType.Int);
            studentIDParam.Value = indeks;
            SqlCommand command = new SqlCommand(kveri, conn);
            command.Parameters.Add(studentIDParam);
            try
            {
                int ukupnoPredmeta = (int) command.ExecuteScalar();
                return ukupnoPredmeta - brojPoloženih;
            }
            catch(Exception e)
            {

            }
            return 0;

        }
        public double dajProsjekPoID(int? indeks)
        {
            string kveri = "select isnull(sum(ocjena),0),count(ocjena) from ocjene where ocjena>5 and idStudenta = @StudentID";
            
            var studentIDParam = new SqlParameter("StudentID", SqlDbType.Int);
            studentIDParam.Value = indeks;
            SqlCommand command = new SqlCommand(kveri, conn);
            command.Parameters.Add(studentIDParam);
            try
            {
                var result = command.ExecuteReader();
                if (result.HasRows)
                {
                    result.Read();
                    int suma = result.GetInt32(0);
                    int count = result.GetInt32(1);
                    if (count == 0) return 5;
                    return ((double)suma)/ count;
                }
            }catch(Exception e)
            {
                return 5;
            }
            return 5;
        }

        public List<Obavještenje> dajSvaObavještenja()
        {
            Logger logg = Logger.GetInstance();
            List<Obavještenje> obavještenja = logg.dajObavještenja();
            Logger.removeInstance();
            return obavještenja;
        }


        public Obavještenje dajObavještenjePoId(int id)
        {
            string kveri = "select naslov, sadržaj, vrijemeObavještenja, idObavjestenja from OBAVJEŠTENJA where idObavjestenja = @obID";
            var idParam = new SqlParameter("obID", SqlDbType.Int);
            idParam.Value = id;
            SqlCommand cmd = new SqlCommand(kveri, conn);
            cmd.Parameters.Add(idParam);
            try
            {
                var result = cmd.ExecuteReader();
                if(result.HasRows)
                {
                    result.Read();
                    return new Obavještenje(result.GetString(0), result.GetString(1), result.GetDateTime(2), result.GetInt32(3));
                }
                else
                {
                    return null;
                }
            }
            catch(Exception e)
            {
                throw new Exception(e.StackTrace + " greška prilikom dohvaćanja obavještenja po ID iz baze");
            }

        }

        public List<Tuple<string,int,int>> dajMojePredmete(int? indeks)
        {
            List<Tuple<string, int, int>> mojiPredmeti = new List<Tuple<string, int, int>>();

            string kveri = "select p.naziv, p.ectsPoeni p.idPredmeta, o.studijskaGodina from ocjene o, predmeti p where o.idStudenta = @StudentID and o.idPredmeta = p.idPredmeta order by o.studijskaGodina asc";
            var studentIDParam = new SqlParameter("StudentID", SqlDbType.Int);
            studentIDParam.Value = indeks;
            SqlCommand command = new SqlCommand(kveri, conn);
            command.Parameters.Add(studentIDParam);
            try
            {
                var result = command.ExecuteReader();
                if (result.HasRows)
                {
                    while (result.Read())
                    {
                        mojiPredmeti.Add(new Tuple<string, int, int>(result.GetString(0), result.GetInt32(2), result.GetInt32(3)));
                    }
                    return mojiPredmeti;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.StackTrace + " greška prilikom dohvaćanja informacija o predmetu iz baze");
            }
        }

        public List<Zadaća> dajStudentoveZadaće(int indeks,int idPredmeta)
        {
            List<Zadaća> zadaće = new List<Zadaća>();
            string kveri = "select z.redniBroj, z.nazivZadaće, z.bodovi, z.rokIsteka, z.rješenjeZadaće, z.maxBrojBodova, z.putanjaDoZadaće from zadaće z,aktivnosti a where z.redniBroj = a.idAktivnosti and z.idStudenta = @studentID and z.idPredmeta = @subjectID";
            var studentIDParam = new SqlParameter("studentID", SqlDbType.Int);
            studentIDParam.Value = indeks;
            var subjectIDParam = new SqlParameter("subjectID", SqlDbType.Int);
            subjectIDParam.Value = idPredmeta;
            SqlCommand command = new SqlCommand(kveri, conn);
            command.Parameters.Add(studentIDParam);
            command.Parameters.Add(subjectIDParam);
            try
            {
                var result = command.ExecuteReader();
                if (result.HasRows)
                {
                    while (result.Read())
                    {
                        if(!result.IsDBNull(6))
                        {
                            zadaće.Add(new Zadaća(result.GetInt32(0), indeks, idPredmeta, result.GetString(1), result.GetFloat(2), result.GetDateTime(3), null, result.GetFloat(5), result.GetString(6)));
                        }
                        else
                        {
                            zadaće.Add(new Zadaća(result.GetInt32(0), indeks, idPredmeta, result.GetString(1), result.GetFloat(2), result.GetDateTime(3), null, result.GetFloat(5), null));
                        }
                      
                    }
                    return zadaće;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.StackTrace + " greška prilikom dohvaćanja zadaće za studenta iz baze");
            }

        }

        public List<Ispit> dajStudentoveIspite(int indeks, int idPredmeta)
        {
            List<Ispit> ispiti = new List<Ispit>();
            string kveri = "select i.naziv,i.datum,i.bodovi,i.idIspita,i.maxBrojBodova,i.brojBodovaZaProlaz from ispiti i, aktivnosti a where i.idIspita = a.idAktivnosti and i.idStudenta = @studentID and i.idPredmeta = @subjectID";
            var studentIDParam = new SqlParameter("studentID", SqlDbType.Int);
            studentIDParam.Value = indeks;
            var subjectIDParam = new SqlParameter("subjectID", SqlDbType.Int);
            subjectIDParam.Value = idPredmeta;
            SqlCommand command = new SqlCommand(kveri, conn);
            command.Parameters.Add(studentIDParam);
            command.Parameters.Add(subjectIDParam);

            try
            {
                var result = command.ExecuteReader();
                if (result.HasRows)
                {
                    while (result.Read())
                    {
                        ispiti.Add(new Ispit(indeks, idPredmeta, result.GetString(0), result.GetDateTime(1), result.GetFloat(2), result.GetInt32(3), result.GetInt32(4), result.GetFloat(5)));
                    }
                    return ispiti;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.StackTrace + " greška prilikom dohvaćanja obavještenja po ID iz baze");
            }



        }
        
        public PredmetZaStudenta dajPredmetZaStudentaPoID(int indeks, int idPredmeta, int studijskaGodina)
        {
            
            List<Zadaća> zadaće = dajStudentoveZadaće(indeks, idPredmeta);
            List<Ispit> ispiti = dajStudentoveIspite(indeks, idPredmeta);
            List<Aktivnost> aktivnosti = new List<Aktivnost>();
            if(zadaće != null) foreach (Zadaća zadaća in zadaće) aktivnosti.Add(zadaća);
            if(ispiti != null) foreach (Ispit ispit in ispiti) aktivnosti.Add(ispit);
            string kveri = "select p.naziv, p.ectsPoeni, o.bodovi, o.ocjena from ocjene o, predmeti p where o.idStudenta = @studentID and o.idPredmeta = @subjectID and o.studijskaGodina = @studyYear and p.idPredmeta = o.idPredmeta";

            var studentIDParam = new SqlParameter("studentID", SqlDbType.Int);
            studentIDParam.Value = indeks;
            var subjectIDParam = new SqlParameter("subjectID", SqlDbType.Int);
            subjectIDParam.Value = idPredmeta;
            var studyYearParam = new SqlParameter("studyYear", SqlDbType.Int);
            studyYearParam.Value = studijskaGodina;
            SqlCommand command = new SqlCommand(kveri, conn);
            command.Parameters.Add(studentIDParam);
            command.Parameters.Add(studyYearParam);
            command.Parameters.Add(subjectIDParam);
            try
            {
                var result = command.ExecuteReader();
                if (result.HasRows)
                {
                    result.Read();
                    return new PredmetZaStudenta(result.GetString(0), result.GetFloat(1), result.GetFloat(2), result.GetInt32(3), aktivnosti,
                        idPredmeta, indeks, studijskaGodina);
                }
                else
                {
                    return null;
                }
            }
            catch(Exception e)
            {
                throw new Exception(e.StackTrace);
            }

        }

        public List<Korisnik> pretražiKorisnike(string ime, string prezime)
        {
            List<Korisnik> korisnici = new List<Korisnik>();
            Logger logg = Logger.GetInstance();
            List<Student> studenti = logg.pretražiStudenta(null, ime, prezime, null);
            if(studenti!=null) foreach (Student student in studenti) korisnici.Add(student);

            List<Tuple<int, NastavnoOsoblje>> nastavnoOsoblje = logg.pretražiNastavnoOsoblje(ime, prezime, "Izaberite");
            if(nastavnoOsoblje!=null) foreach (Tuple<int, NastavnoOsoblje> tapl in nastavnoOsoblje) korisnici.Add(tapl.Item2);

            Logger.removeInstance();
            return korisnici;
        }


        public List<NastavnoOsoblje> dajAnsamblNaPredmetu(int idPredmeta)
        {
            List<NastavnoOsoblje> ansambl = new List<NastavnoOsoblje>();
            Logger logg = Logger.GetInstance();
            string kveri1 = "select idNastavnogOsoblja from ansambl where idPredmeta = @subjectID";
            var subjectIDParam = new SqlParameter("subjectID", SqlDbType.Int);
            subjectIDParam.Value = idPredmeta;
            SqlCommand command = new SqlCommand(kveri1, conn);
            command.Parameters.Add(subjectIDParam);
            try
            {
                var result = command.ExecuteReader();
                if (result.HasRows)
                {
                    while (result.Read())
                    {
                        int id = result.GetInt32(0);
                        ansambl.Add(logg.dajKreiranoNastavnoOsobljePoID(id));

                    }
                }
                else
                {
                    return ansambl;
                }
            }
            catch(Exception e)
            {
                throw new Exception(e.StackTrace);
            }

            return ansambl;
        }


        public int generišiIdAktivnosti()
        {
            string kveri = "select isnull(Max(idAktivnosti),0)+1 from AKTIVNOSTI";
            SqlCommand cmd = new SqlCommand(kveri, conn);
            try
            {
                return (int)cmd.ExecuteScalar();
            }
            catch (Exception e)
            {
                throw new Exception(e.StackTrace + "greška prilikom generisanja id za aktivnost");
            }
        }


        public int kreirajAktivnost(int idPredmeta, String naziv, DateTime vrijeme, String vrsta, double maxBrojBodova)
        {
            int idAktivnosti = this.generišiIdAktivnosti();
            string kveri = "insert into AKTIVNOSTI values (@aktivnostID, @predmetID, @vrsta, @naziv, @rok, @maxbrojbodova)";
            SqlCommand cmd = new SqlCommand(kveri, conn);
            cmd.Parameters.AddWithValue("aktivnostID", idAktivnosti);
            cmd.Parameters.AddWithValue("predmetID", idPredmeta);
            cmd.Parameters.AddWithValue("naziv", naziv);
            cmd.Parameters.AddWithValue("rok", vrijeme);
            cmd.Parameters.AddWithValue("vrsta", vrsta);
            cmd.Parameters.AddWithValue("maxbrojbodova", maxBrojBodova);
            try
            {
                cmd.ExecuteNonQuery();
                return idAktivnosti;
            }
            catch(Exception e)
            {
                throw new Exception(e.StackTrace + " greška prilikom kreiranja aktivnosti");
            }
        }


        public void ubaciDefaultPodatkeZaZadaću(int idZadaće, PredmetZaNastavnoOsoblje prdmt, String nazivZadaće, double maxBrojBodova, DateTime vrijeme)
        {
            StringBuilder sb = new StringBuilder("insert into zadaće values");
            for (int i = 0; i < prdmt.Studenti.Count; i++)
            {
                if (i < prdmt.Studenti.Count-1)
                {
                    sb.Append("(").Append(idZadaće).Append(",").Append(prdmt.Studenti[i].BrojIndeksa).Append(",").Append(prdmt.IdPredmeta+", '").Append(nazivZadaće).Append("', 0, ").Append("@vrijeme").Append(", null, "+maxBrojBodova+", null), ");
                }
                else
                {
                    sb.Append("(").Append(idZadaće).Append(",").Append(prdmt.Studenti[i].BrojIndeksa).Append(",").Append(prdmt.IdPredmeta + ", '").Append(nazivZadaće).Append("', 0, ").Append("@vrijeme").Append(", null, " + maxBrojBodova + ", null)");
                }
            }
            string kveri = sb.ToString();
            SqlParameter vrijemeParam = new SqlParameter("vrijeme", SqlDbType.DateTime);
            vrijemeParam.Value = vrijeme;
            SqlCommand cmd = new SqlCommand(kveri, conn);
            cmd.Parameters.Add(vrijemeParam);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch(Exception e)
            {
                throw new Exception(e.StackTrace + "greška prilikom unosa default podataka za zadaću");
            }
        }

        public List<Tuple<string,Aktivnost>> dajIspiteNaKojeSeStudentNijePrijavio(int idStudenta)
        {
            List<Tuple<string, Aktivnost>> lista = new List<Tuple<string, Aktivnost>>();
            string kveri = "select p.naziv, a.idAktivnosti, a.idPredmeta, a.vrsta, a.naziv, a.rok, a.maxBrojBodova from aktivnosti a, predmeti p, ocjene o where vrsta = 'Ispit' and a.idPredmeta=p.idPredmeta and o.idPredmeta=p.idPredmeta and o.idStudenta=@studentID";
            SqlCommand cmd = new SqlCommand(kveri, conn);
            cmd.Parameters.AddWithValue("studentID", idStudenta);
            try
            {
                var result = cmd.ExecuteReader();
                if(result.HasRows)
                {
                    while(result.Read())
                    {
                        lista.Add(new Tuple<string, Aktivnost>(result.GetString(0), new Ispit(idStudenta, result.GetInt32(2), result.GetString(4), result.GetDateTime(5), 0, result.GetInt32(1), result.GetInt32(6), 10)));
                    }
                    return lista;
                }
                else
                {
                    return null;
                }
            }
            catch(Exception e)
            {
                throw new Exception(e.StackTrace + " greška prilikom učitavanja ispita na koje se student nije prijavio!");
            }
        }

        public Aktivnost dajAktivnostPoId(int idAktivnosti)
        {
            string kveri = "select idAktivnosti, idPredmeta, vrsta, naziv, rok, maxBrojBodova from aktivnosti where idAktivnosti=@aktivnostID";
            SqlCommand cmd = new SqlCommand(kveri, conn);
            cmd.Parameters.AddWithValue("aktivnostID", idAktivnosti);
            try
            {
                var result = cmd.ExecuteReader();
                if(result.HasRows)
                {
                    result.Read();
                    if(result.GetString(2).Equals("Ispit"))
                    {
                        return new Ispit(-1, result.GetInt32(1), result.GetString(3), result.GetDateTime(4), 0, result.GetInt32(0), result.GetInt32(5), 10);
                    }
                    else
                    {
                        return new Zadaća(result.GetInt32(0), -1, result.GetInt32(1), result.GetString(3), 0, result.GetDateTime(4), null, result.GetInt32(5), null);
                    }
                }
                return null;
            }
            catch(Exception e)
            {
                throw new Exception(e.StackTrace + " greška prilikom dobijanja aktivnosti po ID");
            }
        }

        public void editujAktivnost(int idAktivnosti,string naziv, DateTime datumVrijeme, int maxBrojBodova)
        {
            string kveri = "update aktivnosti set naziv=@naziv, rok=@datumVrijeme, maxBrojBodova = @max where idAktivnosti = @aktivnostID";
            SqlCommand cmd = new SqlCommand(kveri, conn);
            cmd.Parameters.AddWithValue("datumVrijeme", datumVrijeme);
            cmd.Parameters.AddWithValue("max", maxBrojBodova);
            cmd.Parameters.AddWithValue("aktivnostID", idAktivnosti);
            cmd.Parameters.AddWithValue("naziv", naziv);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch(Exception e)
            {
                throw new Exception(e.StackTrace + " greška prilikom izmjene aktivnosti po ID");
            }
        }

        public void updateBodoveZadaćeZaStudenta(int idZadaće, int idStudenta, double bodovi)
        {
            string kveri = "update zadaće set bodovi = @bodovi where redniBroj = @zadaćaID and idStudenta= @studentID";
            SqlCommand cmd = new SqlCommand(kveri, conn);
            cmd.Parameters.AddWithValue("bodovi", bodovi);
            cmd.Parameters.AddWithValue("zadaćaID", idZadaće);
            cmd.Parameters.AddWithValue("studentID", idStudenta);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch(Exception e)
            {
                throw new Exception(e.StackTrace + " greška prilikom izmjene bodova studentove zadaće");
            }

        }

        public void updateOrInsertOcjenuZaStudenta(int idPredmeta, int idStudenta, int ocjena)
        {
            string kveri = "update ocjene set ocjena = @ocjena where idPredmeta=@predmetID and idStudenta=@studentID";
            SqlCommand cmd = new SqlCommand(kveri, conn);
            cmd.Parameters.AddWithValue("ocjena", ocjena);
            cmd.Parameters.AddWithValue("predmetID", idPredmeta);
            cmd.Parameters.AddWithValue("studentID", idStudenta);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception e) { throw new Exception(e.StackTrace + " greška prilikom upisa ocjene za studenta"); }
        }

        public void prijaviStudentaNaIspit(int idStudenta, Ispit tempIspit)
        {
            string kveri = "insert into ODAZVANI_STUDENTI values (@aktivnostID, @studentID)";
            SqlCommand cmd = new SqlCommand(kveri, conn);
            cmd.Parameters.AddWithValue("aktivnostID", tempIspit.IdAktivnosti);
            cmd.Parameters.AddWithValue("studentID", idStudenta);

            string kveri2 = "insert into ISPITI values(@studID, @predID, @naziv, @datum, 0, @ispitID, @maxBrBod, 10)";
            SqlCommand cmd2 = new SqlCommand(kveri2, conn);
            cmd2.Parameters.AddWithValue("studID", idStudenta);
            cmd2.Parameters.AddWithValue("predID", tempIspit.IdPredmeta);
            cmd2.Parameters.AddWithValue("naziv", tempIspit.Naziv);
            cmd2.Parameters.AddWithValue("datum", tempIspit.KrajnjiDatum);
            cmd2.Parameters.AddWithValue("ispitID", tempIspit.IdAktivnosti);
            cmd2.Parameters.AddWithValue("maxBrBod", tempIspit.MaxBrojBodova);
            try
            {
                cmd.ExecuteNonQuery();
                cmd2.ExecuteNonQuery();
            }
            catch (Exception e) { throw new Exception(e.StackTrace + " greška prilikom prijavljivanja studenta na ispit"); }
        }

        public bool daLiJePrijavljenNaIspit(int idStudenta, int idIspita)
        {
            string kveri = "select * from ODAZVANI_STUDENTI where idAktivnosti=@aktID and idStudenta = @studID";
            SqlCommand cmd = new SqlCommand(kveri, conn);
            cmd.Parameters.AddWithValue("aktID", idIspita);
            cmd.Parameters.AddWithValue("studID", idStudenta);
            try
            {
                var result = cmd.ExecuteReader();
                return result.HasRows;
            }catch(Exception e) { throw new Exception(e.StackTrace + " neka greška prilikom provjere da li je student prijavljen na ispit"); }
        }

        public void odjaviStudentaSaIspita(int idStudenta, int idIspita)
        {
            string kveri = "delete from odazvani_studenti where idAktivnosti = @ispitID and idStudenta = @studID";
            SqlCommand cmd = new SqlCommand(kveri, conn);
            cmd.Parameters.AddWithValue("ispitID", idIspita);
            cmd.Parameters.AddWithValue("studID", idStudenta);

            string kveri2 = "delete from ISPITI where idIspita = @IDIspit and idStudenta = @IDStud";
            SqlCommand cmd2 = new SqlCommand(kveri2, conn);
            cmd2.Parameters.AddWithValue("IDIspit", idIspita);
            cmd2.Parameters.AddWithValue("IDStud", idStudenta);
            try
            {
                cmd.ExecuteNonQuery();
                cmd2.ExecuteNonQuery();
            }catch(Exception e){ throw new Exception(e.StackTrace + " greška prilikom odjavljivanja studenta sa ispita"); }
        }

        public int dajBrojPrijavljenihNaIspit(int idIspita)
        {
            string kveri = "select ISNULL(Count(idAktivnosti),0) from odazvani_studenti where idAktivnosti = @ispitID";
            SqlCommand cmd = new SqlCommand(kveri, conn);
            cmd.Parameters.AddWithValue("ispitID", idIspita);
            try
            {
                return (int)cmd.ExecuteScalar();
            }catch(Exception e) { throw new Exception(e.StackTrace + " greška prilikom dobavljanja broja prijavljenih studenata"); }
        }


        public void updateBodoveIspitaZaStudenta(int idIspita, int idStudenta, double bodovi)
        {
            string kveri = "update ispiti set bodovi=@bodovi where idIspita=@ispitID and idStudenta=@studentID";
            SqlCommand cmd = new SqlCommand(kveri, conn);
            cmd.Parameters.AddWithValue("ispitID", idIspita);
            cmd.Parameters.AddWithValue("studentID", idStudenta);
            cmd.Parameters.AddWithValue("bodovi", bodovi);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception e) { throw new Exception(e.StackTrace + " greška prilikom bodovanja studenta na ispitu"); }
        }

        public int generišiIdAnkete()
        {
            string kveri = "select isnull(Max(idAnkete),0)+1 from ANKETE_NA_PREDMETIMA";
            SqlCommand komanda = new SqlCommand(kveri, conn);
            try
            {
                return (int)komanda.ExecuteScalar();
            }
            catch (Exception e)
            {
                throw new Exception(e.StackTrace + "greška prilikom generisanja id za novu anketu");
            }
        }




        public int kreirajAnketu(int idPredmeta, string naziv, DateTime datum, string pitanje1, string pitanje2, string pitanje3, string pitanje4, string pitanje5, int ocjena)
        {
            int idAnkete = this.generišiIdAnkete();
            string kveri = "insert into ANKETE_NA_PREDMETIMA values (@predmetID, @naziv, @datum, @anketaID, @pitanje1, @pitanje2, @pitanje3, @pitanje4, @pitanje5, @ocjena)";
            SqlCommand cmd = new SqlCommand(kveri, conn);

            cmd.Parameters.AddWithValue("predmetID", idPredmeta);
            cmd.Parameters.AddWithValue("naziv", naziv);
            cmd.Parameters.AddWithValue("datum", datum);
            cmd.Parameters.AddWithValue("anketaID", idAnkete);
            cmd.Parameters.AddWithValue("pitanje1", pitanje1);
            cmd.Parameters.AddWithValue("pitanje2", pitanje2);
            cmd.Parameters.AddWithValue("pitanje3", pitanje3);
            cmd.Parameters.AddWithValue("pitanje4", pitanje4);
            cmd.Parameters.AddWithValue("pitanje5", pitanje5);
            cmd.Parameters.AddWithValue("ocjena", ocjena);
            try
            {
                cmd.ExecuteNonQuery();
                return idAnkete;
            }
            catch (Exception e)
            {
                throw new Exception(e.StackTrace + " greška prilikom kreiranja ankete");
            }
        }

        public List<OdgovorNaAnketu> dajOdgovoreNaAnketu(int id)
        {
            string kveri = "select idStudenta, idAnkete, odgovor1, odgovor2, odgovor3, odgovor4, odgovor5, komentarStudenta, ocjenaPredmeta from ODGOVORI_NA_ANKETU where idAnkete=@anketaId";
            SqlCommand komanda = new SqlCommand(kveri, conn);
            komanda.Parameters.AddWithValue("anketaId", id);
            try
            {
                var rezultat = komanda.ExecuteReader();
                if (rezultat.HasRows)
                {
                    List<OdgovorNaAnketu> o = new List<OdgovorNaAnketu>();
                    while (rezultat.Read())
                    {
                        List<string> odgovori = new List<string>();
                        odgovori.Add(rezultat.GetString(2));
                        odgovori.Add(rezultat.GetString(3));
                        odgovori.Add(rezultat.GetString(4));
                        odgovori.Add(rezultat.GetString(5));
                        odgovori.Add(rezultat.GetString(6));
                        o.Add(new OdgovorNaAnketu(rezultat.GetInt32(1), rezultat.GetInt32(0), odgovori, rezultat.GetString(7), rezultat.GetInt32(8)));
                    }
                    return o;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.StackTrace + " greška prilikom vraćanja odgovora za anketu po id-u");
            }
        }
        public Anketa dajAnketu(int id)
        {
            string kveri = "select idPredmeta, naziv, datum, idAnkete, pitanje1, pitanje2, pitanje3, pitanje4, pitanje5, ocjenaPredmeta from ANKETE_NA_PREDMETIMA where idAnkete=@anketaId";
            SqlCommand komanda = new SqlCommand(kveri, conn);
            komanda.Parameters.AddWithValue("anketaId", id);
            try
            {
                var rezultat = komanda.ExecuteReader();
                if (rezultat.HasRows)
                {
                    rezultat.Read();
                    List<String> pitanja = new List<string>();
                    pitanja.Add(rezultat.GetString(4));
                    pitanja.Add(rezultat.GetString(5));
                    pitanja.Add(rezultat.GetString(6));
                    pitanja.Add(rezultat.GetString(7));
                    pitanja.Add(rezultat.GetString(8));
                    List<OdgovorNaAnketu> odgovori = dajOdgovoreNaAnketu(id);
                    Anketa a = new Anketa(rezultat.GetString(1), rezultat.GetDateTime(2), pitanja, rezultat.GetInt32(3), odgovori, rezultat.GetInt32(0));
                    return a;
                }
                else
                {
                    throw new Exception("Greška, nema ankete pod datim id-em");
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.StackTrace + " greška prilikom pretrage ankete po id-u");
            }
        }

        public string dajNazivPredmetaPoId(int idPredmeta)
        {
            string kveri = "select naziv from predmeti where idPredmeta = @predmetID";
            SqlCommand cmd = new SqlCommand(kveri, conn);
            cmd.Parameters.AddWithValue("predmetID", idPredmeta);
            try
            {
                return (string)cmd.ExecuteScalar();
            }
            catch(Exception e) { throw new Exception(e.StackTrace + " greška prilikom dobavljanja naziva predmeta po id"); }
        }

        public List<int> dajIdeveAktivnihAnketaZaPredmet(int idPredmeta)
        {
            List<int> idevi = new List<int>();
            string kveri = "select idAnkete from ANKETE_NA_PREDMETIMA where idPredmeta = @predmetID and datum>@trenutniDatum";
            SqlCommand cmd = new SqlCommand(kveri, conn);
            cmd.Parameters.AddWithValue("predmetID", idPredmeta);
            cmd.Parameters.AddWithValue("trenutniDatum", DateTime.Now);
            try
            {
                var result = cmd.ExecuteReader();
                if(result.HasRows)
                {
                    while(result.Read())
                    {
                        idevi.Add(result.GetInt32(0));
                    }
                    return idevi;
                }
                else
                {
                    return null;
                }
            }catch(Exception e) { throw new Exception(e.StackTrace + " greška prilikom dobavljanja ideva za ankete po idu predmeta"); }
        }


        public bool popuniAnketu(int idAnkete, int idStudenta, List<String> odgovori, String komentar, int ocjena)
        {
            string kveri = "insert into ODGOVORI_NA_ANKETU VALUES (@idStudenta, @idAnkete, @odgovor1, @odgovor2, @odgovor3, @odgovor4, @odgovor5, @komentarStudenta, @ocjenaPredmeta)";
            SqlCommand komanda = new SqlCommand(kveri, conn);
            komanda.Parameters.AddWithValue("idStudenta", idStudenta);
            komanda.Parameters.AddWithValue("idAnkete", idAnkete);
            komanda.Parameters.AddWithValue("odgovor1", odgovori.ElementAt(0));
            komanda.Parameters.AddWithValue("odgovor2", odgovori.ElementAt(1));
            komanda.Parameters.AddWithValue("odgovor3", odgovori.ElementAt(2));
            komanda.Parameters.AddWithValue("odgovor4", odgovori.ElementAt(3));
            komanda.Parameters.AddWithValue("odgovor5", odgovori.ElementAt(4));
            komanda.Parameters.AddWithValue("komentarStudenta", komentar);
            komanda.Parameters.AddWithValue("ocjenaPredmeta", ocjena);
            try
            {
                komanda.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }

        public bool daLiJeAnketaVecPopunjena(int idStudenta, int idAnkete)
        {
            string kveri = "select * from odgovori_na_anketu where idStudenta=@StudentID and idAnkete=@AnketaID";
            SqlCommand command = new SqlCommand(kveri, conn);
            command.Parameters.AddWithValue("StudentID", idStudenta);
            command.Parameters.AddWithValue("AnketaID", idAnkete);
            try
            {
                var result = command.ExecuteReader();
                if (result.HasRows) return true;
                return false;
            }
            catch (Exception e)
            {
                throw new Exception(e.StackTrace + "greška prilikom pretrage odgovora na anketu");
            }


        }





    }


}
