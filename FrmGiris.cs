using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using KutuphaneOtomasyonu.VeriBaglantisi;
using KutuphaneOtomasyonu.Formlar;

namespace KutuphaneOtomasyonu
{
    public partial class FrmGiris : Form
    {
        Baglanti bgl = new Baglanti();

        TextBox txtEmail, txtSifre;
        Button btnGiris;
        LinkLabel lnkKayit;

        public FrmGiris()
        {
            InitializeComponent();
            this.Text = "Kütüphane Yönetim Sistemi - Giriþ";
            this.Size = new Size(450, 550);
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
            mainPanel.RowCount = 6;
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 30F));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 70F));
            mainPanel.Padding = new Padding(50);
            this.Controls.Add(mainPanel);

            Label lblBaslik = new Label();
            lblBaslik.Text = "KÜTÜPHANE GÝRÝÞÝ";
            lblBaslik.Font = new Font("Segoe UI", 20, FontStyle.Bold);
            lblBaslik.ForeColor = Color.DarkSlateBlue;
            lblBaslik.TextAlign = ContentAlignment.MiddleCenter;
            lblBaslik.Dock = DockStyle.Fill;
            lblBaslik.AutoSize = true;
            mainPanel.Controls.Add(lblBaslik, 0, 0);

            Label lblEmail = new Label();
            lblEmail.Text = "E-Posta Adresi:";
            lblEmail.Font = new Font("Segoe UI", 11, FontStyle.Regular);
            lblEmail.ForeColor = Color.Gray;
            lblEmail.TextAlign = ContentAlignment.BottomLeft;
            lblEmail.Dock = DockStyle.Fill;
            lblEmail.Margin = new Padding(0, 10, 0, 5);
            mainPanel.Controls.Add(lblEmail, 0, 1);

            txtEmail = new TextBox();
            txtEmail.Font = new Font("Segoe UI", 12);
            txtEmail.Dock = DockStyle.Fill;
            txtEmail.BorderStyle = BorderStyle.FixedSingle;
            mainPanel.Controls.Add(txtEmail, 0, 2);

            Label lblSifre = new Label();
            lblSifre.Text = "Þifre:";
            lblSifre.Font = new Font("Segoe UI", 11, FontStyle.Regular);
            lblSifre.ForeColor = Color.Gray;
            lblSifre.TextAlign = ContentAlignment.BottomLeft;
            lblSifre.Dock = DockStyle.Fill;
            lblSifre.Margin = new Padding(0, 15, 0, 5);
            mainPanel.Controls.Add(lblSifre, 0, 3);

            txtSifre = new TextBox();
            txtSifre.Font = new Font("Segoe UI", 12);
            txtSifre.UseSystemPasswordChar = true;
            txtSifre.Dock = DockStyle.Fill;
            txtSifre.BorderStyle = BorderStyle.FixedSingle;
            mainPanel.Controls.Add(txtSifre, 0, 4);

            Panel pnlAlt = new Panel();
            pnlAlt.Dock = DockStyle.Top;
            pnlAlt.Height = 100;
            pnlAlt.Margin = new Padding(0, 25, 0, 0);
            mainPanel.Controls.Add(pnlAlt, 0, 5);

            btnGiris = new Button();
            btnGiris.Text = "GÝRÝÞ YAP";
            btnGiris.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            btnGiris.BackColor = Color.DarkSlateBlue;
            btnGiris.ForeColor = Color.White;
            btnGiris.FlatStyle = FlatStyle.Flat;
            btnGiris.FlatAppearance.BorderSize = 0;
            btnGiris.Cursor = Cursors.Hand;
            btnGiris.Dock = DockStyle.Top;
            btnGiris.Height = 45;
            btnGiris.Click += BtnGiris_Click;
            pnlAlt.Controls.Add(btnGiris);

            lnkKayit = new LinkLabel();
            lnkKayit.Text = "Hesabýn yok mu? Kayýt Ol";
            lnkKayit.Font = new Font("Segoe UI", 10);
            lnkKayit.LinkColor = Color.DarkSlateBlue;
            lnkKayit.ActiveLinkColor = Color.MediumSlateBlue;
            lnkKayit.TextAlign = ContentAlignment.MiddleCenter;
            lnkKayit.Dock = DockStyle.Bottom;
            lnkKayit.Margin = new Padding(0, 10, 0, 0);
            lnkKayit.Cursor = Cursors.Hand;
            lnkKayit.LinkClicked += LnkKayit_LinkClicked;
            pnlAlt.Controls.Add(lnkKayit);
        }

        private void BtnGiris_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text;
            string sifre = txtSifre.Text;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(sifre))
            {
                MessageBox.Show("Lütfen tüm alanlarý doldurunuz.", "Uyarý", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                SqlCommand komut = new SqlCommand("Select Rol From Tbl_Kullanicilar Where Email=@p1 AND Sifre=@p2", bgl.baglanti());
                komut.Parameters.AddWithValue("@p1", email);
                komut.Parameters.AddWithValue("@p2", sifre);

                SqlDataReader dr = komut.ExecuteReader();

                if (dr.Read())
                {
                    int rol = Convert.ToInt32(dr["Rol"]);

                    if (rol == 1)
                    {
                        FrmOgrenciPanel fr = new FrmOgrenciPanel();
                        fr.kullaniciMail = email;
                        fr.Show();
                    }
                    else if (rol == 2)
                    {
                        FrmPersonelPanel fr = new FrmPersonelPanel();
                        fr.Show();
                    }
                    else if (rol == 3)
                    {
                        FrmYoneticiPanel fr = new FrmYoneticiPanel();
                        fr.Show();
                    }

                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Hatalý E-Posta veya Þifre!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                bgl.baglanti().Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Bir hata oluþtu: " + ex.Message);
            }
        }

        private void LnkKayit_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            FrmKayitOl fr = new FrmKayitOl();
            fr.Show();
            this.Hide();
        }
    }
}