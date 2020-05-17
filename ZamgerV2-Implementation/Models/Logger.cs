using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ZamgerV2_Implementation.Models
{
    public class Logger
    {
        private static SqlConnection conn = null;
        private static Logger instance = null;
        private Logger()
        {
            String connString = "server=DESKTOP-0G31M9N;database=zamgerDB-new;Trusted_Connection=true;MultipleActiveResultSets=true";
            try
            {
                conn = new SqlConnection(connString);
                conn.Open();
            }
            catch(Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.StackTrace+"Greška pri uspostavi konekcije");
                conn.Close();
            }
        }

        public static Logger GetInstance()
        {
            if (instance == null)
            {
                instance = new Logger();
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
                System.Diagnostics.Debug.WriteLine(e.StackTrace + "greška pri uništavanju Logger singleton-a");
            }
        }

        

        public int generišiKorisničkePodatke(Korisnik noviKorisnik)
        {
            int userID = dajNoviKorisničkiId();
            String username = generišiUsername(noviKorisnik.Ime, noviKorisnik.Prezime);
            String password;
            if (username != null && userID != -1)
            {
                int brojLjudiSaIstimUsernameom = dajBrojIstihUsernameova(username);
                if (brojLjudiSaIstimUsernameom != -1)
                {
                    noviKorisnik.Username = username + brojLjudiSaIstimUsernameom;
                    noviKorisnik.Email = noviKorisnik.Username + "@etf.unsa.ba";
                    if(noviKorisnik.GetType() == typeof(Student))
                    {
                        Student tempStd = (Student)noviKorisnik;
                        tempStd.BrojIndeksa = userID;
                        password = noviKorisnik.Username + "-pass";
                        spremiStudentaUBazu(tempStd, password);
                        registrujDefaultOcjene(tempStd, tempStd.BrojIndeksa);
                    }
                    else if(noviKorisnik.GetType() == typeof(MasterStudent))
                    {
                        MasterStudent tempMaster = (MasterStudent)noviKorisnik;
                        tempMaster.BrojIndeksa = userID;
                        password = tempMaster.Username + "-pass";
                        spremiStudentaUBazu(tempMaster, password);
                        registrujDefaultOcjene(tempMaster, tempMaster.BrojIndeksa);

                    }
                    else if(noviKorisnik.GetType() == typeof(Profesor)) //ovdje se desi weird bug npr ovo bude tačno ako je i noviKorisnik tipa NastavnoOsoblje, pa npr Jurke završi ko asistent umjesto ko profesor, moramo skontat što
                    {
                        Profesor tempProf = (Profesor)noviKorisnik;
                        tempProf.IdOsobe = userID;
                        password = tempProf.Username + "-pass";
                        spremiNastavnoOsobljeUBazu(tempProf, password);
                    }
                    else
                    {
                        NastavnoOsoblje tempNastavno = (NastavnoOsoblje)noviKorisnik;
                        tempNastavno.IdOsobe = userID;
                        password = tempNastavno.Username + "-pass";
                        spremiNastavnoOsobljeUBazu(tempNastavno, password);
                    }

                }
                else
                {
                    throw new Exception("greška prilikom generisanja broja u usernameu");
                }
                return userID;
            }
            else
            {
                throw new Exception("greška prilikom generisanja korisničih podataka");
            }
        }

        private void spremiNastavnoOsobljeUBazu(NastavnoOsoblje tempNastavno, string password)
        {
            string sqlKveri = "INSERT INTO KORISNICI VALUES(@user, @pass, @userID, @userTip)";
            var userParam = new SqlParameter("user", System.Data.SqlDbType.NVarChar);
            userParam.Value = tempNastavno.Username;
            var passParam = new SqlParameter("pass", System.Data.SqlDbType.NVarChar);
            passParam.Value = password;
            var userIDParam = new SqlParameter("userID", System.Data.SqlDbType.Int);
            userIDParam.Value = tempNastavno.IdOsobe;
            var userTipParam = new SqlParameter("userTip", System.Data.SqlDbType.Int);

            if (tempNastavno.GetType() == typeof(NastavnoOsoblje)) userTipParam.Value = 2; //nastavno osoblje nivo pristupa je 2 (WEIRD BUG NAVEDEN RANIJE, ne kontam što jer za masterstudent i student radi perfektno)
            else userTipParam.Value = 4; //profesor ima nivo pristupa 4

            string sqlKveri2 = "INSERT INTO NASTAVNO_OSOBLJE VALUES(@userID, @ime, @prezime, @datumRodjenja, @mjestoPrebivalista, @email, @spol, @titula)";
            var userIDParam2 = new SqlParameter("userID", System.Data.SqlDbType.Int);
            userIDParam2.Value = tempNastavno.IdOsobe;
            var imeParam = new SqlParameter("ime", System.Data.SqlDbType.NVarChar);
            imeParam.Value = tempNastavno.Ime;
            var prezimeParam = new SqlParameter("prezime", System.Data.SqlDbType.NVarChar);
            prezimeParam.Value = tempNastavno.Prezime;
            var spolParam = new SqlParameter("spol", System.Data.SqlDbType.NVarChar);
            spolParam.Value = tempNastavno.Spol;
            var rodjenjeParam = new SqlParameter("datumRodjenja", System.Data.SqlDbType.Date);
            DateTime oDate = Convert.ToDateTime(tempNastavno.DatumRođenja);
            rodjenjeParam.Value = oDate.Date;
            var prebivalisteParam = new SqlParameter("mjestoPrebivalista", System.Data.SqlDbType.NVarChar);
            prebivalisteParam.Value = tempNastavno.MjestoPrebivališta;
            var emailParam = new SqlParameter("email", System.Data.SqlDbType.NVarChar);
            emailParam.Value = tempNastavno.Email;
            var titulaParam = new SqlParameter("titula", System.Data.SqlDbType.NVarChar);
            titulaParam.Value = tempNastavno.Titula;

            SqlCommand command = new SqlCommand(sqlKveri, conn);
            command.Parameters.Add(userParam);
            command.Parameters.Add(passParam);
            command.Parameters.Add(userIDParam);
            command.Parameters.Add(userTipParam);

            SqlCommand command2 = new SqlCommand(sqlKveri2, conn);
            command2.Parameters.Add(userIDParam2);
            command2.Parameters.Add(imeParam);
            command2.Parameters.Add(prezimeParam);
            command2.Parameters.Add(spolParam);
            command2.Parameters.Add(rodjenjeParam);
            command2.Parameters.Add(prebivalisteParam);
            command2.Parameters.Add(titulaParam);
            command2.Parameters.Add(emailParam);

            try
            {
                command.ExecuteNonQuery();

                Thread.Sleep(100);

                command2.ExecuteNonQuery();

            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("nesto nije u redu");
                throw new Exception("Greška prilikom ubacivanja osobe u bazu");
            }
        }

        private void spremiStudentaUBazu(Student noviKorisnik, String password)
        {
            string sqlKveri = "INSERT INTO KORISNICI VALUES(@user, @pass, @userID, @userTip)";
            var userParam = new SqlParameter("user", System.Data.SqlDbType.NVarChar);
            userParam.Value = noviKorisnik.Username;
            var passParam = new SqlParameter("pass", System.Data.SqlDbType.NVarChar);
            passParam.Value = password;
            var userIDParam = new SqlParameter("userID", System.Data.SqlDbType.Int);
            userIDParam.Value = noviKorisnik.BrojIndeksa;
            var userTipParam = new SqlParameter("userTip", System.Data.SqlDbType.Int);
            userTipParam.Value = 1;

            string sqlKveri2 = "INSERT INTO STUDENTI VALUES(@userID, @ime, @prezime, @spol, @datumRodjenja, @mjestoPrebivalista, @odsjek, @email, 1, 0, NULL)";

            var userIDParam2 = new SqlParameter("userID", System.Data.SqlDbType.Int);
            userIDParam2.Value = noviKorisnik.BrojIndeksa;
            var imeParam = new SqlParameter("ime", System.Data.SqlDbType.NVarChar);
            imeParam.Value = noviKorisnik.Ime;
            var prezimeParam = new SqlParameter("prezime", System.Data.SqlDbType.NVarChar);
            prezimeParam.Value = noviKorisnik.Prezime;
            var spolParam = new SqlParameter("spol", System.Data.SqlDbType.NVarChar);
            spolParam.Value = noviKorisnik.Spol;
            var rodjenjeParam = new SqlParameter("datumRodjenja", System.Data.SqlDbType.Date);
            DateTime oDate = Convert.ToDateTime(noviKorisnik.DatumRođenja);
            rodjenjeParam.Value = oDate.Date;
            var prebivalisteParam = new SqlParameter("mjestoPrebivalista", System.Data.SqlDbType.NVarChar);
            prebivalisteParam.Value = noviKorisnik.MjestoPrebivališta;
            var odsjekParam = new SqlParameter("odsjek", System.Data.SqlDbType.NVarChar);
            odsjekParam.Value = noviKorisnik.Odsjek;
            var emailParam = new SqlParameter("email", System.Data.SqlDbType.NVarChar);
            emailParam.Value = noviKorisnik.Email;
            SqlParameter prosjekParam = null;
            SqlCommand command = new SqlCommand(sqlKveri, conn);
            command.Parameters.Add(userParam);
            command.Parameters.Add(passParam);
            command.Parameters.Add(userIDParam);
            command.Parameters.Add(userTipParam);


            if (noviKorisnik.GetType() == typeof(MasterStudent))
             {
                    MasterStudent tempKorisnik = (MasterStudent)noviKorisnik;
                    prosjekParam = new SqlParameter("prosjek", System.Data.SqlDbType.Real);
                    prosjekParam.Value = tempKorisnik.ProsjekNaBSC;
                    sqlKveri2 = "INSERT INTO STUDENTI VALUES(@userID, @ime, @prezime, @spol, @datumRodjenja, @mjestoPrebivalista, @odsjek, @email, 4, 1, @prosjek)";
             }

            SqlCommand command2 = new SqlCommand(sqlKveri2, conn);
            command2.Parameters.Add(userIDParam2);
            command2.Parameters.Add(imeParam);
            command2.Parameters.Add(prezimeParam);
            command2.Parameters.Add(spolParam);
            command2.Parameters.Add(rodjenjeParam);
            command2.Parameters.Add(prebivalisteParam);
            command2.Parameters.Add(odsjekParam);
            command2.Parameters.Add(emailParam);
            if(noviKorisnik is MasterStudent)
            {
                command2.Parameters.Add(prosjekParam);
            }
            try
                {
                    command.ExecuteNonQuery();

                    Thread.Sleep(100);

                    command2.ExecuteNonQuery();

                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("nesto nije u redu");
                    throw new Exception("Greška prilikom ubacivanja studenta u bazu");
                }
        }

        private void registrujDefaultOcjene(Korisnik noviKorisnik, int? userID)
        {
            string kveri;
            SqlCommand command = null;
            var odsjekParam = new SqlParameter("odsjek", System.Data.SqlDbType.NVarChar);

            if (noviKorisnik.GetType() == typeof(Student))
            {
                Student noviStd = (Student)noviKorisnik;
                kveri = "select idPredmeta from DOSTUPNOST_PREDMETA where godinaStudija=1 and odsjek like @odsjek";
                odsjekParam.Value = noviStd.Odsjek;
                command = new SqlCommand(kveri, conn);
                command.Parameters.Add(odsjekParam);
            }
            else
            {
                MasterStudent masterStd = (MasterStudent)noviKorisnik;
                kveri = "select idPredmeta from DOSTUPNOST_PREDMETA where godinaStudija=4 and odsjek like @odsjek";
                odsjekParam.Value = masterStd.Odsjek;
                command = new SqlCommand(kveri, conn);
                command.Parameters.Add(odsjekParam);
            }
            List<int> ideviPredmeta = new List<int>();
            try
            {
                var result = command.ExecuteReader();
                if(result.HasRows)
                {
                    while(result.Read())
                    {
                        ideviPredmeta.Add(result.GetInt32(0));
                    }
                    StringBuilder sb = new StringBuilder("insert into ocjene values");
                    for(int i = 0; i<ideviPredmeta.Count; i++)
                    {
                        if (i < ideviPredmeta.Count-1)
                        {
                            sb.Append("(").Append(userID).Append(",").Append(ideviPredmeta[i]).Append(", 5, 0,").Append(DateTime.Now.Year.ToString()).Append("), ");
                        }
                        else
                        {
                            sb.Append("(").Append(userID).Append(",").Append(ideviPredmeta[i]).Append(", 5, 0,").Append(DateTime.Now.Year.ToString()).Append(")");
                        }
                    }

                    SqlCommand command2 = new SqlCommand(sb.ToString(), conn);
                    command2.ExecuteNonQuery();
                }
                else
                {
                    throw new Exception("Ili nisu još dodani predmeti na toj godini il nešto drugo ne valja (mozda insert u ocjene nije uspio)");
                }
            }
            catch(Exception e)
            {
                throw e;
            }

        }

        public void zadužiKreiranogNaPredmetima(int userID, int idPrvogPredmeta = -1, int idDrugogPredmeta = -1)
        {
            var userIDParam = new SqlParameter("userID", System.Data.SqlDbType.Int);
            userIDParam.Value = userID;
            var prviPredmetParam = new SqlParameter("idPrvog", System.Data.SqlDbType.Int);
            prviPredmetParam.Value = idPrvogPredmeta;
            var drugiPredmetParam = new SqlParameter("idDrugog", System.Data.SqlDbType.Int);
            drugiPredmetParam.Value = idDrugogPredmeta;
            string kveri;
            SqlCommand command=null;

            if (idPrvogPredmeta!=-1 && idDrugogPredmeta!=-1 && idPrvogPredmeta!=idDrugogPredmeta) //osoba je zadužena na oba predmeta
            {
                kveri = "insert into ansambl values (@idPrvog, @userID), (@idDrugog, @userID)";
                command = new SqlCommand(kveri, conn);
                command.Parameters.Add(userIDParam);
                command.Parameters.Add(prviPredmetParam);
                command.Parameters.Add(drugiPredmetParam);
            }
            else if(idPrvogPredmeta==-1 && idDrugogPredmeta==-1) //osoba nije zadužena ni na jednom predmetu
            {
                throw new Exception("Nije odabran niti jedan predmet na kom će osoba biti zadužena");
            }
            else if(idPrvogPredmeta!=-1 && idDrugogPredmeta ==-1) //osoba zadužena na prvom predmetu
            {
                kveri = "insert into ansambl values (@idPrvog, @userID)";
                command = new SqlCommand(kveri, conn);
                command.Parameters.Add(userIDParam);
                command.Parameters.Add(prviPredmetParam);
            }
            else if(idPrvogPredmeta == -1 && idDrugogPredmeta != -1) //osoba zadužena na drugom predmetu
            {
                kveri = "insert into ansambl values (@idDrugog, @userID)";
                command = new SqlCommand(kveri, conn);
                command.Parameters.Add(userIDParam);
                command.Parameters.Add(drugiPredmetParam);
            }
            else //oba predmeta ista --- GREŠKA
            {
                throw new Exception("Dva odabrana predmeta su ista! --- GREŠKA");
            }
            try
            {
                command.ExecuteNonQuery();
            }
            catch(Exception e)
            {
                throw new Exception("Nešto nije u redu prilikom zaduživanja osobe na predmetu");
            }
        }

        private int dajBrojIstihUsernameova(String username)
        {
            username = username + "[0-9]";
            string sqlKveri = "SELECT Count(idKorisnika)+1 from korisnici where username like @user";
            var userParam = new SqlParameter("user", System.Data.SqlDbType.Text);
            userParam.Value = username;
            SqlCommand command = new SqlCommand(sqlKveri, conn);
            command.Parameters.Add(userParam);
            try
            {
                return (int)command.ExecuteScalar();
            }
            catch(Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.StackTrace + "error u generisanju ID");
                return -1;
            }
        }

        public int dajNoviKorisničkiId()
        {
            string sqlKveri = "SELECT isnull(max(idKorisnika),0)+1 from korisnici";
            SqlCommand command = new SqlCommand(sqlKveri, conn);
            try
            {
                int resultID = (int)command.ExecuteScalar();
                return resultID;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.StackTrace + "error u generisanju ID");
                return -1;
            }
        }

        public static int dajNoviPredmetId()
        {
            string sqlKveri = "SELECT isnull(max(idPredmeta),0)+1 from predmeti";
            SqlCommand command = new SqlCommand(sqlKveri, conn);
            try
            {
                int resultID = (int)command.ExecuteScalar();
                return resultID;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.StackTrace + "error u generisanju ID");
                return -1;
            }
        }

        public string generišiUsername(string ime, string prezime)
        {
            if (ime != null && prezime != null)
            {
                String prvoSlovo = ime.Substring(0, 1);
                String ostatak;
                prezime = Regex.Replace(prezime, " ", "");
                if (prezime.Length > 10)
                {
                    ostatak = prezime.Substring(0, 10);
                }
                else
                {
                    ostatak = prezime;
                }

                String tempUsername = prvoSlovo + ostatak;
                return tempUsername.ToLower();
            }
            return null;
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
                if(result.HasRows)
                {
                    result.Read();
                    if(result.GetInt32(11) == 1) //ako mu je vrijednost BSC kolone 1, to znači da je završio BSC i da je to MasterStudent
                    {
                        Student tempS =  new MasterStudent(result.GetValue(0).ToString(), result.GetValue(1).ToString(), result.GetValue(2).ToString(), result.GetValue(3).ToString(), result.GetValue(4).ToString(), result.GetValue(5).ToString(), result.GetValue(6).ToString(), result.GetValue(7).ToString(), result.GetInt32(8), result.GetFloat(9));
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
            catch(Exception e)
            {
                throw new Exception(e.StackTrace + "greška prilikom pretrage studenta po ID iz baze");
            }


        }

        public int unesiPredmetUBazu(String naziv, double brojECTSPoena, List<string> odsjeci, List<int> godine, int izborni)
        {
            try
            {
                string sqlKveri = "INSERT INTO PREDMETI VALUES(@predmetID, @name, @ECTSpoints)";
                var predmetIDParam = new SqlParameter("predmetID", System.Data.SqlDbType.Int);
                int pomocni = dajNoviPredmetId();
                predmetIDParam.Value = pomocni;
                var nameParam = new SqlParameter("name", System.Data.SqlDbType.NVarChar);
                nameParam.Value = naziv;
                var ECTSpointsParam = new SqlParameter("ECTSpoints", System.Data.SqlDbType.Real);
                ECTSpointsParam.Value = brojECTSPoena;
                SqlCommand command = new SqlCommand(sqlKveri, conn);
                command.Parameters.Add(predmetIDParam);
                command.Parameters.Add(nameParam);
                command.Parameters.Add(ECTSpointsParam);
                command.ExecuteNonQuery();
                Thread.Sleep(100);

                for (int i = 0; i < odsjeci.Count; i++)
                {
                    string odsjek = odsjeci[i];
                    for (int j = 0; j < godine.Count; j++)
                    {
                        int godina = godine[j];
                        var predmetID2 = new SqlParameter("predmetID", System.Data.SqlDbType.Int);
                        predmetID2.Value = pomocni;
                        string sqlKveri2 = "INSERT INTO DOSTUPNOST_PREDMETA VALUES(@predmetID, @course, @studyYear, @selective)";
                        var courseParam = new SqlParameter("course", System.Data.SqlDbType.NVarChar);
                        courseParam.Value = odsjek;
                        var studyYearParam = new SqlParameter("studyYear", System.Data.SqlDbType.Int);
                        studyYearParam.Value = godina;
                        var selectiveParam = new SqlParameter("selective", System.Data.SqlDbType.Int);
                        selectiveParam.Value = izborni;
                        SqlCommand command2 = new SqlCommand(sqlKveri2, conn);
                        command2.Parameters.Add(predmetID2);
                        command2.Parameters.Add(courseParam);
                        command2.Parameters.Add(studyYearParam);
                        command2.Parameters.Add(selectiveParam);
                        command2.ExecuteNonQuery();
                    }
                    Thread.Sleep(50);
                }
                return pomocni;
            }catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("nesto nije u redu");
                return -1;
                throw new Exception("Greška prilikom ubacivanja studenta u bazu");
            }
        }

        public KreiranPredmetViewModel dajKreiranPredmetPoID(int id)
        {
            KreiranPredmetViewModel model;
            List<int> godineDostupnosti = new List<int>();
            List<string> odsjeciDostupnosti = new List<string>();
            string sqlKveri = "select distinct p.naziv, p.ectsPoeni, dp.izborni from predmeti p, dostupnost_predmeta dp where p.idPredmeta = @predmetID and p.idPredmeta=dp.idPredmeta";
            var predmetIDParam = new SqlParameter("predmetID", System.Data.SqlDbType.Int);
            predmetIDParam.Value = id;

            SqlCommand command = new SqlCommand(sqlKveri, conn);
            command.Parameters.Add(predmetIDParam);
            try
            {
                var result = command.ExecuteReader();
                if(result.HasRows)
                {
                    result.Read();
                    model = new KreiranPredmetViewModel(result.GetValue(0).ToString(), result.GetFloat(1), null, null, result.GetInt32(2));
                    string sqlKveri2 = "select godinaStudija from DOSTUPNOST_PREDMETA where idPredmeta=@idPredmeta group by godinaStudija";
                    var predmetIDParam2 = new SqlParameter("idPredmeta", System.Data.SqlDbType.Int);
                    predmetIDParam2.Value = id;

                    SqlCommand command2 = new SqlCommand(sqlKveri2, conn);
                    command2.Parameters.Add(predmetIDParam2);
                    var result2 = command2.ExecuteReader();
                    if(result2.HasRows)
                    {
                        while(result2.Read())
                        {
                            godineDostupnosti.Add(result2.GetInt32(0));
                        }

                        string sqlKveri3 = "select odsjek from DOSTUPNOST_PREDMETA where idPredmeta=@idPredmeta group by odsjek";
                        var predmetIDParam3 = new SqlParameter("idPredmeta", System.Data.SqlDbType.Int);
                        predmetIDParam3.Value = id;

                        SqlCommand command3 = new SqlCommand(sqlKveri3, conn);
                        command3.Parameters.Add(predmetIDParam3);
                        var result3 = command3.ExecuteReader();

                        if(result3.HasRows)
                        {
                            while(result3.Read())
                            {
                                odsjeciDostupnosti.Add(result3.GetValue(0).ToString());
                            }

                            model.GodineDostupnosti = godineDostupnosti;
                            model.OdsjeciDostupnosti = odsjeciDostupnosti;
                            return model;
                        }
                        else
                        {
                            throw new Exception("neuspješno dohvaćanje odsjeka predmeta");
                        }
                    }
                    else
                    {
                        throw new Exception("neuspješno dohvaćanje godina dostupnosti");
                    }
                }
                else
                {
                    return null;
                }
            }
            catch(Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.StackTrace + "nije dobro dobvaljanje predmeta po ID");
                return null;
            }

        }

        public Dictionary<string, int> dajSvePredmeteImenaID()
        {
            var returnMapa = new Dictionary<string, int>();
            string kveri = "select naziv, idPredmeta from predmeti";
            SqlCommand command = new SqlCommand(kveri, conn);
            try
            {
                var result = command.ExecuteReader();
                if(result.HasRows)
                {
                    while(result.Read())
                    {
                        returnMapa.Add(result.GetString(0), result.GetInt32(1));
                    }
                    return returnMapa;
                }
                else
                {
                    return null;
                }
            }
            catch(Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Greška prilikom učitavanja svih predmeta iz baze");
                return null;
            }
        }


        public NastavnoOsoblje dajKreiranoNastavnoOsobljePoID(int id)
        {
            string kveri = "select no.ime, no.prezime, no.datumRođenja, no.mjestoPrebivališta, k.username, no.email, no.spol, titula from nastavno_osoblje no, korisnici k where no.idOsobe=k.idKorisnika and no.idOsobe=@userID";
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
                   return new NastavnoOsoblje(result.GetValue(0).ToString(), result.GetValue(1).ToString(), result.GetValue(2).ToString(), result.GetValue(3).ToString(), result.GetValue(4).ToString(), result.GetValue(5).ToString(), result.GetValue(6).ToString(), result.GetValue(7).ToString());
                }
                else
                {
                    return null;
                }
            }
            catch(Exception e)
            {
                throw new Exception("Greška prilikom dohvaćanja nastavnog osoblja po ID iz baze");
            }
        }

        public List<string> dajNazivePredmetaNaKojimPredajePoID(int id)
        {
            List<string> povratnaLista = new List<string>();
            string kveri = "select naziv from predmeti p, ansambl a where a.idNastavnogOsoblja=@userID and a.idPredmeta=p.idPredmeta";
            var userIDParam = new SqlParameter("userID", System.Data.SqlDbType.Int);
            userIDParam.Value = id;
            SqlCommand command = new SqlCommand(kveri, conn);
            command.Parameters.Add(userIDParam);
            try
            {
                var result = command.ExecuteReader();
                if (result.HasRows)
                {
                    while (result.Read())
                    {
                        povratnaLista.Add(result.GetValue(0).ToString());
                    }
                    return povratnaLista;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                throw new Exception("Greška prilikom dohvaćanja nastavnog osoblja po ID iz baze");
            }

        }

        public double dajProsjekStudentaPoID(int? id)
        {

            if(id!=null)
            {
                string kveri = "select avg(Cast(ocjena as float)) from ocjene where idStudenta=@userID";
                var userIDParam = new SqlParameter("userID", System.Data.SqlDbType.Int);
                userIDParam.Value = id;
                SqlCommand command = new SqlCommand(kveri, conn);
                command.Parameters.Add(userIDParam);
                try
                {
                    return (double)command.ExecuteScalar();
                }
                catch(Exception e)
                {
                    throw e;
                }
            }
            else
            {
                throw new Exception("Korisnik nema niti jednu ocjenu u sistemu");
            }

        }

        public int dajBrojPredmetaPoID(int? id)
        {
            if (id != null)
            {
                string kveri = "select count(ocjena) from ocjene where idStudenta=@userID";
                var userIDParam = new SqlParameter("userID", System.Data.SqlDbType.Int);
                userIDParam.Value = id;
                SqlCommand command = new SqlCommand(kveri, conn);
                command.Parameters.Add(userIDParam);
                try
                {
                    return (int)command.ExecuteScalar();
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
            else
            {
                throw new Exception("Korisnik nema niti jedan predmet u sistemu");
            }

        }

        public void kreirajObavještenje(String naslov, String sadržaj)
        {
            string kveri = "insert into obavještenja values(@naslov, @sadržaj, @vrijeme, @idObavještenja)";
            var naslovParam = new SqlParameter("naslov", System.Data.SqlDbType.NVarChar);
            naslovParam.Value = naslov;
            var sadržajParam = new SqlParameter("sadržaj", System.Data.SqlDbType.NVarChar);
            sadržajParam.Value = sadržaj;
            var vrijemeParam = new SqlParameter("vrijeme", System.Data.SqlDbType.DateTime);
            vrijemeParam.Value = DateTime.Now;
            var idParam = new SqlParameter("idObavještenja", System.Data.SqlDbType.Int);
            idParam.Value = dajIdObavještenja();
            SqlCommand command = new SqlCommand(kveri, conn);
            command.Parameters.Add(naslovParam);
            command.Parameters.Add(sadržajParam);
            command.Parameters.Add(vrijemeParam);
            command.Parameters.Add(idParam);
            try
            {
                command.ExecuteNonQuery();
            }
            catch(Exception e)
            {
                throw new Exception("neuspješan unos obavještenja u bazu");
            }
        }

        public int dajIdObavještenja()
        {
            string sqlKveri = "SELECT isnull(max(idObavjestenja),0)+1 from OBAVJEŠTENJA";
            SqlCommand command = new SqlCommand(sqlKveri, conn);
            try
            {
                int resultID = (int)command.ExecuteScalar();
                return resultID;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.StackTrace + "error u generisanju ID obavještenja");
                return -1;
            }
        }

        public List<Obavještenje> dajObavještenja()
        {
            List<Obavještenje> obavještenja = new List<Obavještenje>();
            string kveri = "select * from obavještenja";
            try { 
            SqlCommand command = new SqlCommand(kveri, conn);
            
            
                var result = command.ExecuteReader();
                if (result.HasRows)
                {
                    while (result.Read())
                    {
                        string ime = result.GetString(0);
                        string sadrzaj = result.GetString(1);
                        DateTime datum = Convert.ToDateTime(result.GetValue(2).ToString());
                        int idObavjestenja = result.GetInt32(3);
                        obavještenja.Add(new Obavještenje(ime, sadrzaj, datum, idObavjestenja));
                    }
                }
                else return null;
            }
            catch (Exception e)
            {
                //throw new Exception("belaj neki nemam pojma");
            }
            return obavještenja;
        }
        public Obavještenje dajObavještenjePoId(int id)
        {
            string kveri = "select naslov, sadržaj, vrijemeObavještenja from obavještenja where idObavjestenja = @id";
            var idParam = new SqlParameter("id", System.Data.SqlDbType.Int);
            idParam.Value = id;
            try
            {
                SqlCommand command = new SqlCommand(kveri, conn);
                command.Parameters.Add(idParam);
                var result = command.ExecuteReader();
                if(result.HasRows)
                {
                    result.Read();
                    return new Obavještenje(result.GetString(0), result.GetString(1), Convert.ToDateTime(result.GetValue(2).ToString()), id);
                }
                else
                {
                    return null;
                }
            }
            catch(Exception e)
            {
                throw new Exception("Nešto nije u redu prilikom dohvaćanja obavještenja po ID");
            }
        }

        public void editujObavještenje(Obavještenje o)
        {
            string kveri = "UPDATE obavještenja SET naslov = @naslov, sadržaj = @sadrzaj, vrijemeObavještenja=@vrijeme WHERE idObavjestenja=@id";
            var naslovParam = new SqlParameter("naslov", System.Data.SqlDbType.NVarChar);
            naslovParam.Value = o.Naslov;
            var sadrzajParam = new SqlParameter("sadrzaj", System.Data.SqlDbType.NVarChar);
            sadrzajParam.Value = o.Sadržaj;
            var vrijemeParam = new SqlParameter("vrijeme", System.Data.SqlDbType.DateTime);
            vrijemeParam.Value = o.dateTime;
            var idParam = new SqlParameter("id", System.Data.SqlDbType.Int);
            idParam.Value = o.IdObavještenja;
            SqlCommand command = new SqlCommand(kveri, conn);
            command.Parameters.Add(naslovParam);
            command.Parameters.Add(sadrzajParam);
            command.Parameters.Add(vrijemeParam);
            command.Parameters.Add(idParam);
            try
            {
                command.ExecuteNonQuery();
            }
            catch(Exception e)
            {
                throw new Exception("greška prilikom uređivanja predmeta po ID-u");
            }
        }

        public void izbrisiObavjestenje(int id)
        {
            string kveri = "delete from obavještenja where idObavjestenja = @id";
            var idParam = new SqlParameter("id", System.Data.SqlDbType.Int);
            idParam.Value = id;
            SqlCommand command = new SqlCommand(kveri, conn);
            command.Parameters.Add(idParam);
            try
            {
                command.ExecuteNonQuery();
            }
            catch(Exception e)
            {
                throw new Exception("greška prilikom brisanja ograničenja iz baze");
            }
        }

        public List<Student> pretražiStudenta(int brojIndeksa,string ime, string prezime, string odsjek)
        {
            List<Student> studenti = new List<Student>();
            int brojac = 0;
            string where = "";
            string kveri = "select * from studenti ";
            string indeksKveri = "@index = brojIndeksa ";
            string imeKveri = "@name = ime";
            string prezimeKveri = "@lastName = prezime ";
            string odsjekKveri = "@course = odsjek ";
            //if (brojIndeksa != -1 || ime != null || prezime != null || odsjek != null) where ="where ";
            bool provjeraIndeks = false;
            bool provjeraIme = false;
            bool provjeraPrezime = false;
            bool provjeraOdsjek = false;
            if (brojIndeksa != -1) provjeraIndeks = true;
            if (ime != null) provjeraIme = true;
            if (prezime != null) provjeraPrezime = true;
            if (odsjek != null) provjeraOdsjek = true;
            var indexParam = new SqlParameter("index", System.Data.SqlDbType.Int);
            var nameParam = new SqlParameter("name", System.Data.SqlDbType.NVarChar);
            var lastNameParam = new SqlParameter("lastName", System.Data.SqlDbType.NVarChar);
            var courseParam = new SqlParameter("course", System.Data.SqlDbType.NVarChar);
            if (provjeraIme || provjeraPrezime || provjeraOdsjek || provjeraIndeks) kveri = kveri + "where ";
            bool prethodni = false;
            if (provjeraIndeks)
            {
                indexParam.Value = brojIndeksa;
                kveri = kveri + indeksKveri;
                prethodni = true;
            }

            if (provjeraIme)
            {
                if (prethodni) kveri += "and ";
                nameParam.Value = ime;
                kveri += imeKveri;
                prethodni = true;
            }
            if (provjeraPrezime)
            {
                if (prethodni) kveri += "and ";
                lastNameParam.Value = prezime;
                kveri += prezimeKveri;
                prethodni = true;
            }

            if(provjeraOdsjek)
            {
                if (prethodni) kveri += "and ";
                courseParam.Value = odsjek;
                kveri += odsjekKveri;
                //prethodni = true;
            }

            try
            {

                SqlCommand command = new SqlCommand(kveri, conn);
                if (provjeraIndeks) command.Parameters.Add(indexParam);
                if (provjeraIme) command.Parameters.Add(nameParam);
                if (provjeraPrezime) command.Parameters.Add(lastNameParam);
                if (provjeraOdsjek) command.Parameters.Add(courseParam);

                var result = command.ExecuteReader();
                if (result.HasRows)
                {
                    while (result.Read())
                    {
                        //studenti.Add(new Student())
                        string tIme = result.GetString(0);
                        string tPrezime = result.GetString(1);
                        int tBrojIndeksa = result.GetInt32(2);
                        string tDatumRođenja = result.GetString(3);
                        //DateTime tDate = Convert.ToDateTime(tDateString);
                        string tMjestoPrebivališta = result.GetString(4);
                        string tOdsjek = result.GetString(5);
                        string tEmail = result.GetString(6);
                        bool bsc = result.GetBoolean(7);
                        string tSpol = result.GetString(8);
                        if (bsc)
                        {
                            studenti.Add(new Student(tIme, tPrezime, tDatumRođenja, tMjestoPrebivališta, generišiUsername(tIme, tPrezime), tEmail, tSpol, tOdsjek, tBrojIndeksa));
                            
                        }else
                        {
                            studenti.Add(new MasterStudent(tIme, tPrezime, tDatumRođenja, tMjestoPrebivališta, generišiUsername(tIme, tPrezime), tEmail, tSpol, tOdsjek, tBrojIndeksa, 6));
                            
                        }
                    }

                }
                else return null;

            }catch(Exception e)
            {

            }
                
            return studenti;

        }


    }


}
