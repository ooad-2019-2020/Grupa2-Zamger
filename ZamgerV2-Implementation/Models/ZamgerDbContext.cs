using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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




    }
}
