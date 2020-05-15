using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ZamgerV2_Implementation.Models
{
    public class Logger
    {
        private static SqlConnection conn = null;
        private static Logger instance = null;
        private Logger()
        {
            String connString = "server=DESKTOP-0G31M9N;database=zamgerDB-new;Trusted_Connection=true";
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

        

        public void generišiKorisničkePodatke(Student noviKorisnik)
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
                    noviKorisnik.BrojIndeksa = userID;
                    password = noviKorisnik.Username + "-pass";
                    spremiStudentaUBazu(noviKorisnik, password);
                }
                else
                {
                    throw new Exception("greška prilikom generisanja broja u usernameu");
                }

            }
            else
            {
                throw new Exception("greška prilikom generisanja korisničih podataka");
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


            if (noviKorisnik is MasterStudent)
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
            string sqlKveri = "SELECT max(idKorisnika)+1 from korisnici";
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
            string sqlKveri = "SELECT max(idPredmeta)+1 from predmeti";
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
                //tempUsername = Regex.Replace(tempUsername, " ", "");
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


        public bool unesiPredmetUBazu(String naziv, double brojECTSPoena, List<string> odsjeci, List<int> godine, int izborni)
        {
            try
            {

                string sqlKveri = "INSERT INTO PREDMETI VALUES(@predmetID, @name, @ECTSpoints)";
                var predmetIDParam = new SqlParameter("predmetID", System.Data.SqlDbType.Int);
                predmetIDParam.Value = dajNoviPredmetId();
                var nameParam = new SqlParameter("name", System.Data.SqlDbType.NVarChar);
                nameParam.Value = naziv;
                var ECTSpointsParam = new SqlParameter("ECTSpoints", System.Data.SqlDbType.Float);
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
                    int godina = godine[i];
                    string sqlKveri2 = "INSERT INTO DOSTUPNOST_PREDMETA VALUES(@predmetID, @course, @studyYear, @selective)";
                    var courseParam = new SqlParameter("course", System.Data.SqlDbType.NVarChar);
                    courseParam.Value = odsjek;
                    var studyYearParam = new SqlParameter("studyYear", System.Data.SqlDbType.Int);
                    studyYearParam.Value = godina;
                    var selectiveParam = new SqlParameter("selective", System.Data.SqlDbType.Int);
                    selectiveParam.Value = izborni;
                    SqlCommand command2 = new SqlCommand(sqlKveri2, conn);
                    command2.Parameters.Add(predmetIDParam);
                    command2.Parameters.Add(courseParam);
                    command2.Parameters.Add(studyYearParam);
                    command2.Parameters.Add(selectiveParam);
                    command2.ExecuteNonQuery();
                    Thread.Sleep(100);
                }
            }catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("nesto nije u redu");
                throw new Exception("Greška prilikom ubacivanja studenta u bazu");
            }

            return true;
            /* sad ovdje treba prvo dobiti valjan ID za predmet, onaj fazon sa Max()+1 al za tabelu PREDMETI
               nakon toga treba napraviti unos u tabelu PREDMETI
               kada se predmet unese u tabelu PREDMETI i to sve prođe kako treba
               onda je potrebno petljom proći kroz listu odsjeci i listu godine i respektivno to 
               dodavati u međutabelu DOSTUPNOST_PREDMETA

                Kad sve ovo prođe kako treba, predmet je registrovan u sistemu te prilikom kreiranja
                nastavnog osoblja, a i studenata moramo im u startu dodati npr da student koji upisuje BSc
                mu se po defaultu u međutabelu OCJENE ubace ti predmeti analogno na koji su se smjer upisali
                znaci pravit ce se upit u tabelu predmeti i dostupnost predmeta kako bi se dobio id predmeta
                koji je studentu dostupan na prvoj godini u zavisnosti od smjera na koji se upisuje
                analogno je i za MasterStudenta 
             */

        }




    }
}
