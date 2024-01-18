using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace EvoLvl1._3
{
    internal class Program
    {
        public SqlConnection DbConnection()
        {
            SqlConnection connection = new SqlConnection("Server=DESKTOP-35T25HC\\SQLEXPRESS;Database=Evo13;Trusted_Connection=True");
            connection.Open();
            return connection;
        }

        public SqlCommand DbCommand(string query)
        {
            SqlCommand command = new SqlCommand(query, DbConnection());
            return command;
        }


        static void Main(string[] args)
        {
            Program program = new Program();

            while (true)
            {
                Console.Clear();
                Console.WriteLine("------- Sözlük Uygulaması -------");
                Console.WriteLine("1. Kelime Ekle");
                Console.WriteLine("2. Kelime Ara");
                Console.WriteLine("3. Kelime Güncelle");
                Console.WriteLine("4. Kelime Sil");
                Console.WriteLine("5. Sözlüğü Listele");
                Console.WriteLine("0. Çıkış");
                Console.Write("Seçiminiz: ");

                string secim = Console.ReadLine();

                switch (secim)
                {
                    case "1":
                        Console.Write("Türkçe Kelime: ");
                        string turkce = Console.ReadLine();

                        Console.Write("İngilizce Karşılığı: ");
                        string ingilizce = Console.ReadLine();

                        program.KelimeEkle(turkce, ingilizce);
                        break;
                    case "2":
                        Console.Write("Aranan Kelimeyi Giriniz: ");
                        string arananKelime = Console.ReadLine();
                        program.KelimeAra(arananKelime);
                        break;
                    case "3":
                        Console.Write("Güncellenecek Türkçe Kelime: ");
                        string eskiTurkce = Console.ReadLine();

                        Console.Write("Yeni Türkçe Kelime: ");
                        string yeniTurkce = Console.ReadLine();

                        Console.Write("Yeni İngilizce Kelime: ");
                        string yeniIngilizce = Console.ReadLine();

                        program.KelimeGuncelle(eskiTurkce, yeniTurkce, yeniIngilizce);
                        break;
                    case "4":
                        Console.Write("Silinecek Türkçe Kelime: ");
                        string silinecekTurkce = Console.ReadLine();
                        program.KelimeSil(silinecekTurkce);
                        break;
                    case "5":
                        program.SozluguListele();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Geçersiz seçenek. Lütfen tekrar deneyin.");
                        break;
                }

                Console.WriteLine("\nDevam etmek için bir tuşa basın...");
                Console.ReadKey();
            }
        }


        public void KelimeEkle(string turkce, string ingilizce)
        {
            using (SqlCommand cmd = DbCommand("INSERT INTO kelimeler (turkce_kelime, ingilizce_kelime) VALUES (@turkce, @ingilizce)"))
            {
                cmd.Parameters.AddWithValue("@turkce", turkce);
                cmd.Parameters.AddWithValue("@ingilizce", ingilizce);
                cmd.ExecuteNonQuery();
            }

            Console.WriteLine("Kelime başarıyla eklendi.");
        }

        public void KelimeAra(string arananKelime)
        {
            using (SqlCommand cmd = DbCommand("SELECT * FROM kelimeler WHERE turkce_kelime LIKE @kelime OR ingilizce_kelime LIKE @kelime"))
            {
                cmd.Parameters.AddWithValue("@kelime", "%" + arananKelime + "%");

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        Console.WriteLine("Bulunan Sonuçlar:");
                        while (reader.Read())
                        {
                            Console.WriteLine($"Türkçe: {reader["turkce_kelime"]}, İngilizce: {reader["ingilizce_kelime"]}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"{arananKelime} kelimesi sözlükte bulunamadı.");
                    }
                }
            }
        }


        public void KelimeGuncelle(string eskiTurkce, string yeniTurkce, string yeniIngilizce)
        {
            using (SqlCommand cmd = DbCommand("UPDATE kelimeler SET turkce_kelime = @yeniTurkce, ingilizce_kelime = @yeniIngilizce WHERE turkce_kelime = @eskiTurkce"))
            {
                cmd.Parameters.AddWithValue("@yeniTurkce", yeniTurkce);
                cmd.Parameters.AddWithValue("@yeniIngilizce", yeniIngilizce);
                cmd.Parameters.AddWithValue("@eskiTurkce", eskiTurkce);
                cmd.ExecuteNonQuery();
            }

            Console.WriteLine("Kelime başarıyla güncellendi.");
        }

        public void KelimeSil(string silinecekTurkce)
        {
            using (SqlCommand cmd = DbCommand("DELETE FROM kelimeler WHERE turkce_kelime = @turkce"))
            {
                cmd.Parameters.AddWithValue("@turkce", silinecekTurkce);
                int affectedRows = cmd.ExecuteNonQuery();

                if (affectedRows > 0)
                {
                    Console.WriteLine("Kelime başarıyla silindi.");
                }
                else
                {
                    Console.WriteLine($"{silinecekTurkce} kelimesi sözlükte bulunamadı.");
                }
            }
        }

        public void SozluguListele()
        {
            using (SqlCommand cmd = DbCommand("SELECT * FROM kelimeler"))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    Console.WriteLine("------- Sözlük -------");
                    while (reader.Read())
                    {
                        Console.WriteLine($"Türkçe: {reader["turkce_kelime"]}, İngilizce: {reader["ingilizce_kelime"]}");
                    }
                }
            }
        }
    }
}