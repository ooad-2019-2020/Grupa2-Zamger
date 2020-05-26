using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Razor.Language.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
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
                throw new Exception("Slanje poruke neuspješno");
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
            string kveri = "SELECT isnull(max(idPoruke), 0) FROM DOPISIVANJE";
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
                        lista.Add(new PredmetZaStudenta(result.GetString(0), result.GetFloat(1), result.GetFloat(2), result.GetInt32(3), null, result.GetInt32(4), result.GetInt32(5), result.GetInt32(6)));
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
            predmetIDParam.Value = idStudenta;

            SqlCommand command = new SqlCommand(kveri, conn);
            command.Parameters.Add(userIDParam);
            command.Parameters.Add(predmetIDParam);

            //pa onda sve zadaće
            string kveri2 = "select redniBroj, idStudenta, idPredmeta, nazivZadaće, bodovi, rokIsteka, rješenjeZadaće, maxBrojBodova, putanjaDoZadaće from zadaće where idPredmeta = @IDpredmet and idStudenta = @IDStudent";
            var userIDParam2 = new SqlParameter("IDStudent", System.Data.SqlDbType.Int);
            userIDParam2.Value = idStudenta;
            var predmetIDParam2 = new SqlParameter("IDPredmet", System.Data.SqlDbType.Int);
            predmetIDParam2.Value = idStudenta;

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
                        aktivnosti.Add(new Zadaća(result2.GetInt32(0), result2.GetInt32(1), result2.GetInt32(2), result2.GetString(3), result2.GetFloat(4), result2.GetDateTime(5), null, result2.GetFloat(7), null)); //ovo oko zadaće ne znam koji tip pa sam stavio null
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
            string kveri = "select a.idAktivnosti , a.idPredmeta, a.naziv, a.rok, a.vrsta from AKTIVNOSTI a, PREDMETI p, ANSAMBL an, NASTAVNO_OSOBLJE no where a.idPredmeta=p.idPredmeta and p.idPredmeta=an.idPredmeta and an.idNastavnogOsoblja=no.idOsobe and no.idOsobe=@userID";
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
                            aktivnosti.Add(new Ispit(-1,result.GetInt32(1), result.GetString(2), result.GetDateTime(3),0,result.GetInt32(0), 0,0));
                        }
                        else
                        {
                            aktivnosti.Add(new Zadaća(result.GetInt32(0), -1, result.GetInt32(1), result.GetString(2), 0, result.GetDateTime(3), null, 0, null));
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
            string kveri = "select naziv, datum, pitanje1, pitanje2, pitanje3, pitanje4, pitanje5, idAnkete from ANKETE_NA_PREDMETIMA where idPredmeta = @predmetID";
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
                        Anketa tempAnketa = new Anketa(result.GetString(0), result.GetDateTime(1), null, result.GetInt32(7), null);
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
            string kveri = "select isnull(Count(idZahtjeva)+1,0) from ZAHTJEVI";
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
            string kveri = "select isnull(avg(ocjena),5) from ocjene where ocjena>5 and idStudenta = @StudentID";
            var studentIDParam = new SqlParameter("StudentID", SqlDbType.Int);
            studentIDParam.Value = indeks;
            SqlCommand command = new SqlCommand(kveri, conn);
            command.Parameters.Add(studentIDParam);
            try
            {
                return (double)command.ExecuteScalar();
            }catch(Exception e)
            {
                return 5;
            }
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

            string kveri = "select p.naziv, p.idPredmeta, o.studijskaGodina from ocjene o, predmeti p where o.idStudenta = @StudentID and o.idPredmeta = p.idPredmeta order by o.studijskaGodina asc";
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
                        mojiPredmeti.Add(new Tuple<string, int, int>(result.GetString(0), result.GetInt32(1), result.GetInt32(2)));
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

        public List<Zadaća> dajStudentoveZadaće(int indeks,int idPredmeta,int studijskaGodina)
        {
            List<Zadaća> zadaće = new List<Zadaća>();
            string kveri = "select z.redniBroj, z.nazivZadaće, z.bodovi, z.rokIsteka, z.rješenjeZadaće, z.maxBrojBodova, z.putanjaDoZadaće from zadaće z,aktivnosti a where z.redniBroj = a.idAktivnosti and a.godinaStudija = @studyYear and z.idStudenta = @studentID and z.idPredmeta = @subjectID";
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
                    while (result.Read())
                    {
                        zadaće.Add(new Zadaća(result.GetInt32(0), result.GetInt32(1), result.GetInt32(2), result.GetString(3), result.GetFloat(4),
                            result.GetDateTime(5), null, result.GetFloat(7), result.GetString(8)));
                        //ne znam kako preuzet document iz baze, pa je za sad to null
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
                throw new Exception(e.StackTrace + " greška prilikom dohvaćanja obavještenja po ID iz baze");
            }

        }

        public List<Ispit> dajStudentoveIspite(int indeks, int idPredmeta, int studijskaGodina)
        {
            List<Ispit> ispiti = new List<Ispit>();
            string kveri = "select i.naziv,i.datum,i.bodovi,i.idIspita,i.maxBrojBodova,i.brojBodovaZaProlaz from ispiti i, aktivnosti a where i.idIspita = a.idAktivnosti and a.godinaStudija = @studyYear and i.idStudenta = @studentID and i.idPredmeta = @subjectID";
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
                    while (result.Read())
                    {
                        ispiti.Add(new Ispit(indeks, idPredmeta, result.GetString(0), result.GetDateTime(1), result.GetFloat(2),
                            result.GetInt32(3), result.GetInt32(4), result.GetFloat(5)));
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
            
            List<Zadaća> zadaće = dajStudentoveZadaće(indeks, idPredmeta, studijskaGodina);
            List<Ispit> ispiti = dajStudentoveIspite(indeks, idPredmeta, studijskaGodina);
            List<Aktivnost> aktivnosti = new List<Aktivnost>();
            if(zadaće != null) foreach (Zadaća zadaća in zadaće) aktivnosti.Add(zadaća);
            if(ispiti != null) foreach (Ispit ispit in ispiti) aktivnosti.Add(ispit);
            string kveri = "select p.naziv, p.ectsPoeni, o.bodovi, o.ocjena from ocjene o, predmeti p where idStudenta = @studentID and idPredmeta = @subjectID and studijskaGodina = @studyYear";
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

            }
            return null;
        }

    }


}
