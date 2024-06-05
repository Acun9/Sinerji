﻿using SinerjiCRM.Scripts;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace SinerjiCRM.Forms.Main.Menu
{
    public partial class gridTest2 : Form
    {
        private string _previousTeklifNo;

        public gridTest2()
        {
            InitializeComponent();
        }

        private void gridTest2_Load(object sender, EventArgs e)
        {
            LoadDataGridView();
        }

        private void LoadDataGridView()
        {
            using (SqlConnection connection = new SQLBaglantisi().baglanti())
            {
                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM dbo.TEKLIFTRA", connection);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                dataGridView1.DataSource = dataTable;
            }
        }

        private void dataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (dataGridView1.Columns[e.ColumnIndex].Name == "TEKLIF_NO")
            {
                _previousTeklifNo = dataGridView1[e.ColumnIndex, e.RowIndex].Value?.ToString();
            }
        }

        private void dataGridView1_RowValidated(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                DataGridViewRow newRow = dataGridView1.Rows[e.RowIndex];
                var teklifNo = newRow.Cells["TEKLIF_NO"].Value?.ToString();

                if (string.IsNullOrWhiteSpace(teklifNo))
                {
                    MessageBox.Show("Teklif No boş olamaz. Lütfen Teklif No girin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    newRow.Cells["TEKLIF_NO"].Value = _previousTeklifNo; // Eski değeri geri yükle
                    return;
                }

                var stokKodu = newRow.Cells["STOK_KODU"].Value?.ToString();
                var stokAdi = newRow.Cells["STOK_ADI"].Value?.ToString();
                var teslimTarihi = newRow.Cells["TESLIM_TARIHI"].Value?.ToString();
                var miktar = newRow.Cells["MIKTAR"].Value?.ToString();
                var miktarOlcuBirimi = newRow.Cells["MIKTAR_OLCU_BIRIMI"].Value?.ToString();
                var dovizKuru = newRow.Cells["DOVIZ_KURU"].Value?.ToString();
                var tlFiyati = newRow.Cells["TL_FIYATI"].Value?.ToString();
                var kdvOrani = newRow.Cells["KDV_ORANI"].Value?.ToString();
                var tutar = newRow.Cells["TUTAR"].Value?.ToString();
                var siraNo = newRow.Cells["SIRA_NO"].Value?.ToString();
                var projeKodu = newRow.Cells["PROJE_KODU"].Value?.ToString();
                var ekAlan1 = newRow.Cells["EK_ALAN_1"].Value?.ToString();
                var ekAlan2 = newRow.Cells["EK_ALAN_2"].Value?.ToString();

                using (SqlConnection connection = new SQLBaglantisi().baglanti())
                {

                    // TEKLIF_NO değerinin veritabanında olup olmadığını kontrol et
                    string checkQuery = "SELECT COUNT(*) FROM dbo.TEKLIFTRA WHERE TEKLIF_NO = @TeklifNo";
                    SqlCommand checkCommand = new SqlCommand(checkQuery, connection);
                    checkCommand.Parameters.AddWithValue("@TeklifNo", teklifNo);
                    int count = (int)checkCommand.ExecuteScalar();

                    string query;
                    if (count > 0) // TEKLIF_NO zaten varsa güncelle
                    {
                        query = "UPDATE dbo.TEKLIFTRA SET STOK_KODU = @StokKodu, STOK_ADI = @StokAdi, TESLIM_TARIHI = @TeslimTarihi, MIKTAR = @Miktar, MIKTAR_OLCU_BIRIMI = @MiktarOlcuBirimi, DOVIZ_KURU = @DovizKuru, TL_FIYATI = @TlFiyati, KDV_ORANI = @KdvOrani, TUTAR = @Tutar, SIRA_NO = @SiraNo, PROJE_KODU = @ProjeKodu, EK_ALAN_1 = @EkAlan1, EK_ALAN_2 = @EkAlan2 " +
                                "WHERE TEKLIF_NO = @TeklifNo";
                    }
                    else // TEKLIF_NO yoksa yeni kayıt ekle
                    {
                        query = "INSERT INTO dbo.TEKLIFTRA (TEKLIF_NO, STOK_KODU, STOK_ADI, TESLIM_TARIHI, MIKTAR, MIKTAR_OLCU_BIRIMI, DOVIZ_KURU, TL_FIYATI, KDV_ORANI, TUTAR, SIRA_NO, PROJE_KODU, EK_ALAN_1, EK_ALAN_2) " +
                                "VALUES (@TeklifNo, @StokKodu, @StokAdi, @TeslimTarihi, @Miktar, @MiktarOlcuBirimi, @DovizKuru, @TlFiyati, @KdvOrani, @Tutar, @SiraNo, @ProjeKodu, @EkAlan1, @EkAlan2)";
                    }

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@TeklifNo", teklifNo);
                        command.Parameters.AddWithValue("@StokKodu", stokKodu);
                        command.Parameters.AddWithValue("@StokAdi", stokAdi);
                        command.Parameters.AddWithValue("@TeslimTarihi", teslimTarihi);
                        command.Parameters.AddWithValue("@Miktar", miktar);
                        command.Parameters.AddWithValue("@MiktarOlcuBirimi", miktarOlcuBirimi);
                        command.Parameters.AddWithValue("@DovizKuru", dovizKuru);
                        command.Parameters.AddWithValue("@TlFiyati", tlFiyati);
                        command.Parameters.AddWithValue("@KdvOrani", kdvOrani);
                        command.Parameters.AddWithValue("@Tutar", tutar);
                        command.Parameters.AddWithValue("@SiraNo", siraNo);
                        command.Parameters.AddWithValue("@ProjeKodu", projeKodu);
                        command.Parameters.AddWithValue("@EkAlan1", ekAlan1);
                        command.Parameters.AddWithValue("@EkAlan2", ekAlan2);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veritabanına ekleme/güncelleme hatası: " + ex.Message);
            }
        }

        private void dataGridView1_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            DialogResult result = MessageBox.Show("Bu satırı silmek istediğinize emin misiniz?", "Silme Onayı", MessageBoxButtons.YesNo);
            if (result == DialogResult.No)
            {
                e.Cancel = true;
            }
            else
            {
                var teklifNo = e.Row.Cells["TEKLIF_NO"].Value?.ToString();

                if (teklifNo != null)
                {
                    try
                    {
                        using (SqlConnection connection = new SQLBaglantisi().baglanti())
                        {
                            string query = "DELETE FROM dbo.TEKLIFTRA WHERE TEKLIF_NO = @TeklifNo";
                            using (SqlCommand command = new SqlCommand(query, connection))
                            {
                                command.Parameters.AddWithValue("@TeklifNo", teklifNo);
                                command.ExecuteNonQuery();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Veritabanından silme hatası: " + ex.Message);
                        e.Cancel = true;
                    }
                }
                else
                {
                    MessageBox.Show("Silinecek satırın TEKLIF_NO değeri alınamadı.");
                    e.Cancel = true;
                }
            }
        }
    }
}