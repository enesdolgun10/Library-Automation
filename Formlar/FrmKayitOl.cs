using KutuphaneOtomasyonu.VeriBaglantisi;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace KutuphaneOtomasyonu.Formlar
{
    public partial class FrmKayitOl : Form
    {
        Baglanti bgl = new Baglanti();

        TextBox txtAd, txtSoyad, txtEmail, txtSifre, txtOkulNo, txtTelefon;
        Button btnKayit;
        LinkLabel lnkGiris;

        public FrmKayitOl()
        {
            InitializeComponent();
            this.Text = "Kütüphane - Yeni Kayıt";
            this.Size = new Size(500, 750);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.WhiteSmoke;

            ArayuzuOlustur();
        }

        private void ArayuzuOlustur()
        {
            TableLayoutPanel mainPanel = new TableLayoutPanel();
            mainPanel.Dock = DockStyle.Fill;
            mainPanel.ColumnCount = 1;
            mainPanel.RowCount = 10;
            mainPanel.Padding = new Padding(60, 20, 60, 20);

            for (int i = 0; i < 15; i++) mainPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            this.Controls.Add(mainPanel);

            Label lblBaslik = new Label();
            lblBaslik.Text = "ARAMIZA KATIL";
            lblBaslik.Font = new Font("Segoe UI", 22, FontStyle.Bold);
            lblBaslik.ForeColor = Color.DarkSlateBlue;
            lblBaslik.TextAlign = ContentAlignment.MiddleCenter;
            lblBaslik.Dock = DockStyle.Top;
            lblBaslik.Margin = new Padding(0, 0, 0, 20);
            lblBaslik.AutoSize = true;
            mainPanel.Controls.Add(lblBaslik, 0, 0);

            txtAd = InputAlaniEkle(mainPanel, "Adınız:", 1);
            txtSoyad = InputAlaniEkle(mainPanel, "Soyadınız:", 2);

            txtOkulNo = InputAlaniEkle(mainPanel, "Okul Numaranız:", 3);
            txtOkulNo.KeyPress += SadeceSayi_KeyPress;
            txtOkulNo.MaxLength = 10;

            txtEmail = InputAlaniEkle(mainPanel, "E-Posta Adresi:", 4);

            txtTelefon = InputAlaniEkle(mainPanel, "Telefon Numaranız (Başında 0 ile):", 5);
            txtTelefon.MaxLength = 11;
            txtTelefon.KeyPress += SadeceSayi_KeyPress;

            txtSifre = InputAlaniEkle(mainPanel, "Şifre Belirle (En az 8 krktr, 1 Büyük, 1 Sayı):", 6, true);

            btnKayit = new Button();
            btnKayit.Text = "KAYDI TAMAMLA";
            btnKayit.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            btnKayit.BackColor = Color.SeaGreen;
            btnKayit.ForeColor = Color.White;
            btnKayit.FlatStyle = FlatStyle.Flat;
            btnKayit.FlatAppearance.BorderSize = 0;
            btnKayit.Height = 50;
            btnKayit.Dock = DockStyle.Top;
            btnKayit.Cursor = Cursors.Hand;
            btnKayit.Margin = new Padding(0, 30, 0, 0);
            btnKayit.Click += BtnKayit_Click;
            mainPanel.Controls.Add(btnKayit, 0, 13);

            lnkGiris = new LinkLabel();
            lnkGiris.Text = "Zaten hesabın var mı? Giriş Yap";
            lnkGiris.Font = new Font("Segoe UI", 10);
            lnkGiris.LinkColor = Color.DarkSlateBlue;
            lnkGiris.ActiveLinkColor = Color.MediumSlateBlue;
            lnkGiris.TextAlign = ContentAlignment.MiddleCenter;
            lnkGiris.Dock = DockStyle.Top;
            lnkGiris.Margin = new Padding(0, 15, 0, 0);
            lnkGiris.Cursor = Cursors.Hand;
            lnkGiris.AutoSize = true;
            lnkGiris.LinkClicked += (s, e) =>
            {
                new FrmGiris().Show();
                this.Close();
            };
            mainPanel.Controls.Add(lnkGiris, 0, 14);
        }

        private void SadeceSayi_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private TextBox InputAlaniEkle(TableLayoutPanel panel, string baslik, int sira, bool isPassword = false)
        {
            Label lbl = new Label();
            lbl.Text = baslik;
            lbl.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lbl.ForeColor = Color.Gray;
            lbl.Dock = DockStyle.Top;
            lbl.Margin = new Padding(0, 10, 0, 3);
            lbl.AutoSize = true;

            TextBox txt = new TextBox();
            txt.Font = new Font("Segoe UI", 12);
            txt.BorderStyle = BorderStyle.FixedSingle;
            txt.Dock = DockStyle.Top;
            if (isPassword) txt.UseSystemPasswordChar = true;

            panel.Controls.Add(lbl, 0, sira * 2 - 1);
            panel.Controls.Add(txt, 0, sira * 2);

            return txt;
        }

        private void BtnKayit_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtAd.Text) || string.IsNullOrEmpty(txtSoyad.Text) ||
                string.IsNullOrEmpty(txtEmail.Text) || string.IsNullOrEmpty(txtSifre.Text) ||
                string.IsNullOrEmpty(txtTelefon.Text) || string.IsNullOrEmpty(txtOkulNo.Text))
            {
                MessageBox.Show("Lütfen tüm zorunlu alanları doldurunuz.", "Eksik Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (txtTelefon.Text.Length != 11)
            {
                MessageBox.Show("Telefon numarası 11 haneli olmalıdır.", "Hatalı Telefon", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(txtEmail.Text, emailPattern))
            {
                MessageBox.Show("Lütfen geçerli bir E-Posta adresi giriniz.", "Hatalı Format", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string sifreDeseni = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$";
            if (!Regex.IsMatch(txtSifre.Text, sifreDeseni))
            {
                MessageBox.Show("Parola en az 8 karakter olmalı ve bir büyük harf, bir küçük harf ile bir sayı içermelidir.",
                    "Zayıf Şifre", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                if (bgl.baglanti().State == ConnectionState.Closed) bgl.baglanti().Open();

                SqlCommand kmtKontrol = new SqlCommand("Select Count(*) From Tbl_Kullanicilar Where Email=@mail", bgl.baglanti());
                kmtKontrol.Parameters.AddWithValue("@mail", txtEmail.Text);
                int varMi = (int)kmtKontrol.ExecuteScalar();

                if (varMi > 0)
                {
                    MessageBox.Show("Bu E-Posta adresi zaten sistemde kayıtlı!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    bgl.baglanti().Close();
                    return;
                }

                string sql = "INSERT INTO Tbl_Kullanicilar (Ad, Soyad, OkulNo, Email, Telefon, Sifre, Rol) VALUES (@p1, @p2, @p3, @p4, @p5, @p6, 1)";
                SqlCommand komut = new SqlCommand(sql, bgl.baglanti());
                komut.Parameters.AddWithValue("@p1", txtAd.Text);
                komut.Parameters.AddWithValue("@p2", txtSoyad.Text);
                komut.Parameters.AddWithValue("@p3", txtOkulNo.Text);
                komut.Parameters.AddWithValue("@p4", txtEmail.Text);
                komut.Parameters.AddWithValue("@p5", txtTelefon.Text);
                komut.Parameters.AddWithValue("@p6", txtSifre.Text);

                komut.ExecuteNonQuery();
                bgl.baglanti().Close();

                MessageBox.Show("Kayıt başarıyla oluşturuldu! Şimdi giriş yapabilirsiniz.", "Hoşgeldiniz", MessageBoxButtons.OK, MessageBoxIcon.Information);

                new FrmGiris().Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
        }
    }
}