using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using KutuphaneOtomasyonu.VeriBaglantisi;
using KutuphaneOtomasyonu;

namespace KutuphaneOtomasyonu.Formlar
{
    public partial class FrmYoneticiPanel : Form
    {
        Baglanti bgl = new Baglanti();

        TabControl tabMenu;
        DataGridView dgwKitaplar, dgwKullanicilar, dgwRapor;

        TextBox txtKitapAd, txtYazar, txtKategori, txtSayfa, txtStok, txtYayinYili, txtRaf;
        RichTextBox rtbKitapOzet;

        TextBox txtKullaniciAd, txtKullaniciSoyad, txtEmail, txtSifre, txtOkulNo, txtTelefon;
        ComboBox cmbRol;

        Label lblToplamKitap, lblAktifOdunc, lblToplamUye;

        ComboBox cmbIstatistik;
        Label lblIstatistikSonuc, lblIstatistikBaslik;

        Button aktifButon;

        int secilenKitapID = -1;
        int secilenKullaniciID = -1;

        public FrmYoneticiPanel()
        {
            this.Text = "Yönetici Paneli - Admin Dashboard";
            this.Size = new Size(1600, 950);
            this.StartPosition = FormStartPosition.CenterScreen;

            this.Load += FrmYoneticiPanel_Load;

            ArayuzuOlustur();
        }

        private void ArayuzuOlustur()
        {
            Panel pnlSol = new Panel();
            pnlSol.Dock = DockStyle.Left;
            pnlSol.Width = 280;
            pnlSol.BackColor = Color.FromArgb(30, 30, 45);
            this.Controls.Add(pnlSol);

            Panel pnlLogo = new Panel();
            pnlLogo.Dock = DockStyle.Top;
            pnlLogo.Height = 120;
            pnlLogo.BackColor = Color.FromArgb(25, 25, 35);
            pnlSol.Controls.Add(pnlLogo);

            Label lblLogo = new Label();
            lblLogo.Text = "ADMIN\nDASHBOARD";
            lblLogo.Font = new Font("Segoe UI", 18, FontStyle.Bold);
            lblLogo.ForeColor = Color.White;
            lblLogo.Dock = DockStyle.Fill;
            lblLogo.TextAlign = ContentAlignment.MiddleCenter;
            pnlLogo.Controls.Add(lblLogo);

            Button btnCikis = new Button();
            btnCikis.Text = "  🚪  Çıkış Yap";
            btnCikis.Dock = DockStyle.Bottom;
            btnCikis.Height = 65;
            btnCikis.FlatStyle = FlatStyle.Flat;
            btnCikis.FlatAppearance.BorderSize = 0;
            btnCikis.BackColor = Color.IndianRed;
            btnCikis.ForeColor = Color.White;
            btnCikis.Font = new Font("Segoe UI", 13, FontStyle.Bold);
            btnCikis.TextAlign = ContentAlignment.MiddleLeft;
            btnCikis.Padding = new Padding(25, 0, 0, 0);
            btnCikis.Cursor = Cursors.Hand;
            btnCikis.Click += (s, e) =>
            {
                FrmGiris fr = new FrmGiris();
                fr.Show();
                this.Close();
            };
            pnlSol.Controls.Add(btnCikis);

            Panel pnlMenuButonlari = new Panel();
            pnlMenuButonlari.Dock = DockStyle.Fill;
            pnlMenuButonlari.Padding = new Padding(0, 30, 0, 0);
            pnlSol.Controls.Add(pnlMenuButonlari);
            pnlMenuButonlari.BringToFront();

            MenuButonuEkle(pnlMenuButonlari, "  📈  Raporlar & Analiz", 3);
            MenuButonuEkle(pnlMenuButonlari, "  👥  Kullanıcı Yönetimi", 2);
            MenuButonuEkle(pnlMenuButonlari, "  📚  Kitap Yönetimi", 1);
            MenuButonuEkle(pnlMenuButonlari, "  📊  Genel İstatistikler", 0);

            tabMenu = new TabControl();
            tabMenu.Dock = DockStyle.Fill;
            tabMenu.Appearance = TabAppearance.FlatButtons;
            tabMenu.ItemSize = new Size(0, 1);
            tabMenu.SizeMode = TabSizeMode.Fixed;
            this.Controls.Add(tabMenu);
            pnlSol.SendToBack();

            TabPage pageStat = new TabPage();
            pageStat.BackColor = Color.WhiteSmoke;
            tabMenu.TabPages.Add(pageStat);

            lblToplamKitap = KartEkle(pageStat, "Toplam Kitap", "0", Color.FromArgb(255, 159, 67), 50, 60);
            lblAktifOdunc = KartEkle(pageStat, "Emanetteki Kitaplar", "0", Color.FromArgb(10, 189, 227), 450, 60);
            lblToplamUye = KartEkle(pageStat, "Toplam Üye", "0", Color.FromArgb(238, 82, 83), 850, 60);

            TabPage pageKitap = new TabPage();
            pageKitap.BackColor = Color.White;
            tabMenu.TabPages.Add(pageKitap);
            KitapArayuzuOlustur(pageKitap);

            TabPage pageUser = new TabPage();
            pageUser.BackColor = Color.White;
            tabMenu.TabPages.Add(pageUser);
            KullaniciArayuzuOlustur(pageUser);

            TabPage pageRapor = new TabPage();
            pageRapor.BackColor = Color.White;
            tabMenu.TabPages.Add(pageRapor);
            RaporArayuzuOlustur(pageRapor);
        }

        private void MenuButonuEkle(Panel panel, string text, int pageIndex)
        {
            Button btn = new Button();
            btn.Text = text;
            btn.Dock = DockStyle.Top;
            btn.Height = 70;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.ForeColor = Color.LightGray;
            btn.BackColor = Color.FromArgb(30, 30, 45);
            btn.Font = new Font("Segoe UI", 12, FontStyle.Regular);
            btn.TextAlign = ContentAlignment.MiddleLeft;
            btn.Padding = new Padding(30, 0, 0, 0);
            btn.Cursor = Cursors.Hand;

            btn.MouseEnter += (s, e) => { if (aktifButon != btn) btn.BackColor = Color.FromArgb(40, 40, 60); };
            btn.MouseLeave += (s, e) => { if (aktifButon != btn) btn.BackColor = Color.FromArgb(30, 30, 45); };

            btn.Click += (s, e) =>
            {
                tabMenu.SelectedIndex = pageIndex;
                if (aktifButon != null)
                {
                    aktifButon.BackColor = Color.FromArgb(30, 30, 45);
                    aktifButon.ForeColor = Color.LightGray;
                    aktifButon.Font = new Font("Segoe UI", 12, FontStyle.Regular);
                }
                aktifButon = btn;
                aktifButon.BackColor = Color.Teal;
                aktifButon.ForeColor = Color.White;
                aktifButon.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            };

            if (pageIndex == 0)
            {
                aktifButon = btn;
                btn.BackColor = Color.Teal;
                btn.ForeColor = Color.White;
                btn.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            }
            panel.Controls.Add(btn);
        }

        private void KitapArayuzuOlustur(TabPage page)
        {
            GroupBox grp = new GroupBox();
            grp.Text = " Kitap İşlemleri ";
            grp.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            grp.Location = new Point(30, 20);
            grp.Size = new Size(1280, 280);
            page.Controls.Add(grp);

            txtKitapAd = InputEkle(grp, "Kitap Adı:", 30, 50, 250);
            txtYazar = InputEkle(grp, "Yazar:", 300, 50, 250);
            txtKategori = InputEkle(grp, "Kategori:", 570, 50, 200);

            txtYayinYili = InputEkle(grp, "Yayın Yılı:", 30, 120, 120);
            txtSayfa = InputEkle(grp, "Sayfa:", 170, 120, 100);
            txtStok = InputEkle(grp, "Stok:", 290, 120, 100);
            txtRaf = InputEkle(grp, "Raf Konumu:", 410, 120, 180);

            Label lblOzet = new Label { Text = "Kitap Özeti:", Location = new Point(30, 170), AutoSize = true, ForeColor = Color.Gray };
            grp.Controls.Add(lblOzet);

            rtbKitapOzet = new RichTextBox();
            rtbKitapOzet.Location = new Point(30, 190);
            rtbKitapOzet.Size = new Size(800, 75);
            rtbKitapOzet.BorderStyle = BorderStyle.FixedSingle;
            grp.Controls.Add(rtbKitapOzet);

            Button btnEkle = ButonOlustur("KAYDET", Color.SeaGreen, 1150, 45);
            btnEkle.Click += BtnKitapEkle_Click;
            grp.Controls.Add(btnEkle);

            Button btnGuncelle = ButonOlustur("GÜNCELLE", Color.DarkOrange, 1150, 95);
            btnGuncelle.Click += BtnKitapGuncelle_Click;
            grp.Controls.Add(btnGuncelle);

            Button btnSil = ButonOlustur("SİL", Color.IndianRed, 1150, 145);
            btnSil.Size = new Size(110, 35);
            btnSil.Click += BtnKitapSil_Click;
            grp.Controls.Add(btnSil);

            dgwKitaplar = new DataGridView();
            dgwKitaplar.Location = new Point(30, 320);
            dgwKitaplar.Size = new Size(1280, 550);
            dgwKitaplar.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgwKitaplar.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgwKitaplar.ReadOnly = true;
            dgwKitaplar.CellClick += DgwKitaplar_CellClick;
            GridTasarimUygula(dgwKitaplar);
            page.Controls.Add(dgwKitaplar);
        }

        private void KullaniciArayuzuOlustur(TabPage page)
        {
            GroupBox grp = new GroupBox();
            grp.Text = " Personel & Öğrenci Yönetimi ";
            grp.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            grp.Location = new Point(30, 20);
            grp.Size = new Size(1280, 200);
            page.Controls.Add(grp);

            txtKullaniciAd = InputEkle(grp, "Ad:", 30, 50, 180);
            txtKullaniciSoyad = InputEkle(grp, "Soyad:", 230, 50, 180);
            txtEmail = InputEkle(grp, "Email:", 430, 50, 250);
            txtSifre = InputEkle(grp, "Şifre:", 700, 50, 150);

            txtOkulNo = InputEkle(grp, "Okul No:", 30, 120, 180);
            txtTelefon = InputEkle(grp, "Telefon:", 230, 120, 180);

            Label lblRol = new Label { Text = "Rol:", Location = new Point(430, 100), AutoSize = true, Font = new Font("Segoe UI", 9), ForeColor = Color.Gray };
            grp.Controls.Add(lblRol);

            cmbRol = new ComboBox();
            cmbRol.Location = new Point(430, 120);
            cmbRol.Size = new Size(180, 25);
            cmbRol.Items.AddRange(new object[] { "1-Öğrenci", "2-Personel", "3-Yönetici" });
            cmbRol.DropDownStyle = ComboBoxStyle.DropDownList;
            grp.Controls.Add(cmbRol);

            Button btnEkle = ButonOlustur("EKLE", Color.SeaGreen, 1150, 40);
            btnEkle.Click += BtnKullaniciEkle_Click;
            grp.Controls.Add(btnEkle);

            Button btnGuncelle = ButonOlustur("GÜNCELLE", Color.DarkOrange, 1150, 90);
            btnGuncelle.Click += BtnKullaniciGuncelle_Click;
            grp.Controls.Add(btnGuncelle);

            Button btnSil = ButonOlustur("SİL", Color.IndianRed, 1150, 140);
            btnSil.Size = new Size(110, 35);
            btnSil.Click += BtnKullaniciSil_Click;
            grp.Controls.Add(btnSil);

            dgwKullanicilar = new DataGridView();
            dgwKullanicilar.Location = new Point(30, 240);
            dgwKullanicilar.Size = new Size(1280, 580);
            dgwKullanicilar.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgwKullanicilar.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgwKullanicilar.ReadOnly = true;
            dgwKullanicilar.CellClick += DgwKullanicilar_CellClick;
            GridTasarimUygula(dgwKullanicilar);
            page.Controls.Add(dgwKullanicilar);
        }

        private void RaporArayuzuOlustur(TabPage page)
        {
            page.Controls.Clear();
            page.BackColor = Color.FromArgb(240, 242, 245);

            Label lblSayfaBaslik = new Label();
            lblSayfaBaslik.Text = "📊 Raporlar ve Detaylı Analiz";
            lblSayfaBaslik.Font = new Font("Segoe UI", 18, FontStyle.Bold);
            lblSayfaBaslik.ForeColor = Color.FromArgb(44, 62, 80);
            lblSayfaBaslik.AutoSize = true;
            lblSayfaBaslik.Location = new Point(30, 20);
            page.Controls.Add(lblSayfaBaslik);

            Panel pnlSol = new Panel();
            pnlSol.Location = new Point(30, 70);
            pnlSol.Size = new Size(850, 500);
            pnlSol.BackColor = Color.White;
            page.Controls.Add(pnlSol);

            Label lblTabloBaslik = new Label();
            lblTabloBaslik.Text = "🔥 En Çok Ödünç Alınan Kitaplar (Top 5)";
            lblTabloBaslik.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblTabloBaslik.ForeColor = Color.White;
            lblTabloBaslik.BackColor = Color.FromArgb(52, 73, 94);
            lblTabloBaslik.Dock = DockStyle.Top;
            lblTabloBaslik.Height = 45;
            lblTabloBaslik.TextAlign = ContentAlignment.MiddleLeft;
            lblTabloBaslik.Padding = new Padding(15, 0, 0, 0);
            pnlSol.Controls.Add(lblTabloBaslik);

            dgwRapor = new DataGridView();
            dgwRapor.Dock = DockStyle.Fill;
            dgwRapor.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgwRapor.ReadOnly = true;
            dgwRapor.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            GridTasarimUygula(dgwRapor);
            pnlSol.Controls.Add(dgwRapor);
            dgwRapor.BringToFront();

            Panel pnlSag = new Panel();
            pnlSag.Location = new Point(900, 70);
            pnlSag.Size = new Size(350, 250);
            pnlSag.BackColor = Color.White;
            page.Controls.Add(pnlSag);

            Label lblAnalizBaslik = new Label();
            lblAnalizBaslik.Text = "📅 Zamanlı Analiz";
            lblAnalizBaslik.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblAnalizBaslik.ForeColor = Color.White;
            lblAnalizBaslik.BackColor = Color.FromArgb(52, 73, 94);
            lblAnalizBaslik.Dock = DockStyle.Top;
            lblAnalizBaslik.Height = 45;
            lblAnalizBaslik.TextAlign = ContentAlignment.MiddleLeft;
            lblAnalizBaslik.Padding = new Padding(15, 0, 0, 0);
            pnlSag.Controls.Add(lblAnalizBaslik);

            Panel pnlIcerik = new Panel();
            pnlIcerik.Dock = DockStyle.Fill;
            pnlIcerik.Padding = new Padding(20);
            pnlSag.Controls.Add(pnlIcerik);

            Label lblBilgi = new Label();
            lblBilgi.Text = "Dönem Seçiniz:";
            lblBilgi.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            lblBilgi.ForeColor = Color.Gray;
            lblBilgi.Location = new Point(20, 15);
            lblBilgi.AutoSize = true;
            pnlIcerik.Controls.Add(lblBilgi);

            cmbIstatistik = new ComboBox();
            cmbIstatistik.Items.AddRange(new object[] { "Günlük (Bugün)", "Haftalık (Son 7 Gün)", "Aylık (Son 30 Gün)" });
            cmbIstatistik.Location = new Point(20, 40);
            cmbIstatistik.Size = new Size(300, 30);
            cmbIstatistik.Font = new Font("Segoe UI", 11);
            cmbIstatistik.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbIstatistik.BackColor = Color.WhiteSmoke;
            cmbIstatistik.SelectedIndex = 0;
            cmbIstatistik.SelectedIndexChanged += CmbIstatistik_SelectedIndexChanged;
            pnlIcerik.Controls.Add(cmbIstatistik);

            lblIstatistikSonuc = new Label();
            lblIstatistikSonuc.Text = "0";
            lblIstatistikSonuc.Font = new Font("Segoe UI", 36, FontStyle.Bold);
            lblIstatistikSonuc.ForeColor = Color.Teal;
            lblIstatistikSonuc.AutoSize = false;
            lblIstatistikSonuc.Size = new Size(300, 70);
            lblIstatistikSonuc.TextAlign = ContentAlignment.MiddleCenter;
            lblIstatistikSonuc.Location = new Point(20, 80);
            pnlIcerik.Controls.Add(lblIstatistikSonuc);

            lblIstatistikBaslik = new Label();
            lblIstatistikBaslik.Text = "Adet Kitap Verildi";
            lblIstatistikBaslik.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lblIstatistikBaslik.ForeColor = Color.Gray;
            lblIstatistikBaslik.AutoSize = false;
            lblIstatistikBaslik.Size = new Size(300, 30);
            lblIstatistikBaslik.TextAlign = ContentAlignment.TopCenter;
            lblIstatistikBaslik.Location = new Point(20, 150);
            pnlIcerik.Controls.Add(lblIstatistikBaslik);
        }

        private void FrmYoneticiPanel_Load(object sender, EventArgs e)
        {
            VerileriTazele();
        }

        void VerileriTazele()
        {
            KitaplariListele();
            KullanicilariListele();
            RaporlariGetir();
            IstatistikleriGetir();
        }

        void KitapTemizle()
        {
            txtKitapAd.Text = ""; txtYazar.Text = ""; txtKategori.Text = "";
            txtSayfa.Text = ""; txtStok.Text = ""; txtYayinYili.Text = ""; txtRaf.Text = "";
            rtbKitapOzet.Text = "";
            secilenKitapID = -1;
        }

        void KullaniciTemizle()
        {
            txtKullaniciAd.Text = ""; txtKullaniciSoyad.Text = ""; txtEmail.Text = "";
            txtSifre.Text = ""; txtOkulNo.Text = ""; txtTelefon.Text = "";
            cmbRol.SelectedIndex = -1;
            secilenKullaniciID = -1;
        }

        void KitaplariListele()
        {
            string sql = @"Select KitapID, KitapAd, Yazar, Kategori, YayinYili, SayfaSayisi, StokAdeti, RafKonumu, Ozet, Durum 
                           From Tbl_Kitaplar Order By KitapID DESC";
            SqlDataAdapter da = new SqlDataAdapter(sql, bgl.baglanti());
            DataTable dt = new DataTable();
            da.Fill(dt);
            dgwKitaplar.DataSource = dt;
        }

        private void BtnKitapEkle_Click(object sender, EventArgs e)
        {
            try
            {
                string sql = @"INSERT INTO Tbl_Kitaplar (KitapAd, Yazar, Kategori, SayfaSayisi, StokAdeti, YayinYili, RafKonumu, Ozet, Durum) 
                               VALUES (@p1, @p2, @p3, @p4, @p5, @p6, @p7, @p8, 1)";

                SqlCommand komut = new SqlCommand(sql, bgl.baglanti());
                komut.Parameters.AddWithValue("@p1", txtKitapAd.Text);
                komut.Parameters.AddWithValue("@p2", txtYazar.Text);
                komut.Parameters.AddWithValue("@p3", txtKategori.Text);
                komut.Parameters.AddWithValue("@p4", txtSayfa.Text);
                komut.Parameters.AddWithValue("@p5", txtStok.Text);
                komut.Parameters.AddWithValue("@p6", string.IsNullOrEmpty(txtYayinYili.Text) ? DBNull.Value : (object)txtYayinYili.Text);
                komut.Parameters.AddWithValue("@p7", string.IsNullOrEmpty(txtRaf.Text) ? DBNull.Value : (object)txtRaf.Text);
                komut.Parameters.AddWithValue("@p8", string.IsNullOrEmpty(rtbKitapOzet.Text) ? DBNull.Value : (object)rtbKitapOzet.Text);

                komut.ExecuteNonQuery();
                MessageBox.Show("Kitap Eklendi!", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                KitapTemizle();
                VerileriTazele();
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627 || ex.Number == 2601) MessageBox.Show("Bu isimde bir kitap zaten mevcut!", "Mükerrer Kayıt", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else MessageBox.Show("Veritabanı Hatası: " + ex.Message);
            }
            catch (Exception ex) { MessageBox.Show("Hata: " + ex.Message); }
        }

        private void BtnKitapGuncelle_Click(object sender, EventArgs e)
        {
            if (secilenKitapID == -1) { MessageBox.Show("Güncellenecek kitabı seçiniz."); return; }
            try
            {
                string sql = @"UPDATE Tbl_Kitaplar SET KitapAd=@p1, Yazar=@p2, Kategori=@p3, SayfaSayisi=@p4, StokAdeti=@p5, 
                               YayinYili=@p6, RafKonumu=@p7, Ozet=@p8, Durum=1 WHERE KitapID=@id";

                SqlCommand komut = new SqlCommand(sql, bgl.baglanti());
                komut.Parameters.AddWithValue("@p1", txtKitapAd.Text);
                komut.Parameters.AddWithValue("@p2", txtYazar.Text);
                komut.Parameters.AddWithValue("@p3", txtKategori.Text);
                komut.Parameters.AddWithValue("@p4", txtSayfa.Text);
                komut.Parameters.AddWithValue("@p5", txtStok.Text);
                komut.Parameters.AddWithValue("@p6", string.IsNullOrEmpty(txtYayinYili.Text) ? DBNull.Value : (object)txtYayinYili.Text);
                komut.Parameters.AddWithValue("@p7", string.IsNullOrEmpty(txtRaf.Text) ? DBNull.Value : (object)txtRaf.Text);
                komut.Parameters.AddWithValue("@p8", string.IsNullOrEmpty(rtbKitapOzet.Text) ? DBNull.Value : (object)rtbKitapOzet.Text);
                komut.Parameters.AddWithValue("@id", secilenKitapID);

                komut.ExecuteNonQuery();
                MessageBox.Show("Kitap güncellendi ve AKTİF hale getirildi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                KitapTemizle();
                VerileriTazele();
            }
            catch (Exception ex) { MessageBox.Show("Hata: " + ex.Message); }
        }

        private void BtnKitapSil_Click(object sender, EventArgs e)
        {
            if (secilenKitapID == -1) { MessageBox.Show("Silinecek kitabı seçiniz."); return; }
            if (MessageBox.Show("Kitabı pasife almak istiyor musunuz?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                SqlCommand komut = new SqlCommand("Update Tbl_Kitaplar Set Durum=0, StokAdeti=0 Where KitapID=@p1", bgl.baglanti());
                komut.Parameters.AddWithValue("@p1", secilenKitapID);
                komut.ExecuteNonQuery();
                MessageBox.Show("Kitap pasife alındı.");
                KitapTemizle();
                VerileriTazele();
            }
        }

        private void DgwKitaplar_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                secilenKitapID = int.Parse(dgwKitaplar.Rows[e.RowIndex].Cells["KitapID"].Value.ToString());
                txtKitapAd.Text = dgwKitaplar.Rows[e.RowIndex].Cells["KitapAd"].Value.ToString();
                txtYazar.Text = dgwKitaplar.Rows[e.RowIndex].Cells["Yazar"].Value.ToString();
                txtKategori.Text = dgwKitaplar.Rows[e.RowIndex].Cells["Kategori"].Value.ToString();
                txtSayfa.Text = dgwKitaplar.Rows[e.RowIndex].Cells["SayfaSayisi"].Value.ToString();
                txtStok.Text = dgwKitaplar.Rows[e.RowIndex].Cells["StokAdeti"].Value.ToString();

                var yil = dgwKitaplar.Rows[e.RowIndex].Cells["YayinYili"].Value;
                txtYayinYili.Text = yil != DBNull.Value ? yil.ToString() : "";
                var raf = dgwKitaplar.Rows[e.RowIndex].Cells["RafKonumu"].Value;
                txtRaf.Text = raf != DBNull.Value ? raf.ToString() : "";
                var ozet = dgwKitaplar.Rows[e.RowIndex].Cells["Ozet"].Value;
                rtbKitapOzet.Text = ozet != DBNull.Value ? ozet.ToString() : "";
            }
        }

        void KullanicilariListele()
        {
            string sql = @"Select KullaniciID, Ad, Soyad, Email, OkulNo, Telefon, Sifre,
                           CASE Rol WHEN 1 THEN 'Öğrenci' WHEN 2 THEN 'Personel' WHEN 3 THEN 'Yönetici' END AS 'Rol' 
                           From Tbl_Kullanicilar";
            SqlDataAdapter da = new SqlDataAdapter(sql, bgl.baglanti());
            DataTable dt = new DataTable();
            da.Fill(dt);
            dgwKullanicilar.DataSource = dt;
        }

        private void BtnKullaniciEkle_Click(object sender, EventArgs e)
        {
            if (cmbRol.SelectedIndex == -1) { MessageBox.Show("Lütfen Rol seçiniz."); return; }
            try
            {
                int rol = int.Parse(cmbRol.Text.Substring(0, 1));
                string sql = "INSERT INTO Tbl_Kullanicilar (Ad, Soyad, Email, Sifre, OkulNo, Telefon, Rol) VALUES (@p1,@p2,@p3,@p4,@p5,@p6,@p7)";
                SqlCommand komut = new SqlCommand(sql, bgl.baglanti());
                komut.Parameters.AddWithValue("@p1", txtKullaniciAd.Text);
                komut.Parameters.AddWithValue("@p2", txtKullaniciSoyad.Text);
                komut.Parameters.AddWithValue("@p3", txtEmail.Text);
                komut.Parameters.AddWithValue("@p4", txtSifre.Text);
                komut.Parameters.AddWithValue("@p5", string.IsNullOrEmpty(txtOkulNo.Text) ? DBNull.Value : (object)txtOkulNo.Text);
                komut.Parameters.AddWithValue("@p6", string.IsNullOrEmpty(txtTelefon.Text) ? DBNull.Value : (object)txtTelefon.Text);
                komut.Parameters.AddWithValue("@p7", rol);
                komut.ExecuteNonQuery();

                MessageBox.Show("Kullanıcı Eklendi!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                KullaniciTemizle();
                VerileriTazele();
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627 || ex.Number == 2601) MessageBox.Show("Bu E-Posta veya Okul No zaten kayıtlı!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else MessageBox.Show("Veritabanı Hatası: " + ex.Message);
            }
            catch (Exception ex) { MessageBox.Show("Hata: " + ex.Message); }
        }

        private void BtnKullaniciGuncelle_Click(object sender, EventArgs e)
        {
            if (secilenKullaniciID == -1) return;
            try
            {
                int rol = int.Parse(cmbRol.Text.Substring(0, 1));
                string sql = "Update Tbl_Kullanicilar Set Ad=@p1, Soyad=@p2, Email=@p3, Sifre=@p4, OkulNo=@p5, Telefon=@p6, Rol=@p7 Where KullaniciID=@id";
                SqlCommand komut = new SqlCommand(sql, bgl.baglanti());
                komut.Parameters.AddWithValue("@p1", txtKullaniciAd.Text);
                komut.Parameters.AddWithValue("@p2", txtKullaniciSoyad.Text);
                komut.Parameters.AddWithValue("@p3", txtEmail.Text);
                komut.Parameters.AddWithValue("@p4", txtSifre.Text);
                komut.Parameters.AddWithValue("@p5", string.IsNullOrEmpty(txtOkulNo.Text) ? DBNull.Value : (object)txtOkulNo.Text);
                komut.Parameters.AddWithValue("@p6", string.IsNullOrEmpty(txtTelefon.Text) ? DBNull.Value : (object)txtTelefon.Text);
                komut.Parameters.AddWithValue("@p7", rol);
                komut.Parameters.AddWithValue("@id", secilenKullaniciID);
                komut.ExecuteNonQuery();

                MessageBox.Show("Kullanıcı Güncellendi!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                KullaniciTemizle();
                VerileriTazele();
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627 || ex.Number == 2601) MessageBox.Show("Bu bilgiler başka bir kullanıcıya ait!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else MessageBox.Show("Veritabanı Hatası: " + ex.Message);
            }
            catch (Exception ex) { MessageBox.Show("Hata: " + ex.Message); }
        }

        private void BtnKullaniciSil_Click(object sender, EventArgs e)
        {
            if (secilenKullaniciID == -1)
            {
                MessageBox.Show("Lütfen silinecek kullanıcıyı seçiniz.");
                return;
            }

            if (MessageBox.Show("Kullanıcıyı silmek istiyor musunuz?", "Kritik Uyarı", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                try
                {
                    SqlCommand komut = new SqlCommand("Delete From Tbl_Kullanicilar Where KullaniciID=@p1", bgl.baglanti());
                    komut.Parameters.AddWithValue("@p1", secilenKullaniciID);
                    komut.ExecuteNonQuery();

                    MessageBox.Show("Kullanıcı Silindi.", "İşlem Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    KullaniciTemizle();
                    VerileriTazele();
                }
                catch (SqlException ex)
                {
                    if (ex.Number == 547)
                    {
                        MessageBox.Show("Bu kullanıcının kütüphanede geçmiş kitap hareketleri (emanet/iade) bulunduğu için SİLEMEZSİNİZ!\n",
                            "İlişkili Kayıt Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        MessageBox.Show("Veritabanı Hatası: " + ex.Message);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata: " + ex.Message);
                }
            }
        }

        private void DgwKullanicilar_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                secilenKullaniciID = int.Parse(dgwKullanicilar.Rows[e.RowIndex].Cells["KullaniciID"].Value.ToString());
                txtKullaniciAd.Text = dgwKullanicilar.Rows[e.RowIndex].Cells["Ad"].Value.ToString();
                txtKullaniciSoyad.Text = dgwKullanicilar.Rows[e.RowIndex].Cells["Soyad"].Value.ToString();
                txtEmail.Text = dgwKullanicilar.Rows[e.RowIndex].Cells["Email"].Value.ToString();
                txtSifre.Text = dgwKullanicilar.Rows[e.RowIndex].Cells["Sifre"].Value.ToString();

                var okulNo = dgwKullanicilar.Rows[e.RowIndex].Cells["OkulNo"].Value;
                txtOkulNo.Text = okulNo != DBNull.Value ? okulNo.ToString() : "";
                var tel = dgwKullanicilar.Rows[e.RowIndex].Cells["Telefon"].Value;
                txtTelefon.Text = tel != DBNull.Value ? tel.ToString() : "";

                string rolAdi = dgwKullanicilar.Rows[e.RowIndex].Cells["Rol"].Value.ToString();
                if (rolAdi == "Öğrenci") cmbRol.SelectedIndex = 0;
                else if (rolAdi == "Personel") cmbRol.SelectedIndex = 1;
                else if (rolAdi == "Yönetici") cmbRol.SelectedIndex = 2;
            }
        }

        void RaporlariGetir()
        {
            string sql = @"SELECT TOP 5 k.KitapAd, COUNT(*) as 'Ödünç Sayısı' 
                           FROM Tbl_Hareketler h JOIN Tbl_Kitaplar k ON h.KitapID = k.KitapID 
                           GROUP BY k.KitapAd ORDER BY COUNT(*) DESC";
            SqlDataAdapter da = new SqlDataAdapter(sql, bgl.baglanti());
            DataTable dt = new DataTable();
            da.Fill(dt);
            dgwRapor.DataSource = dt;

            if (cmbIstatistik != null && cmbIstatistik.SelectedIndex == -1) cmbIstatistik.SelectedIndex = 0;
            else if (cmbIstatistik != null) CmbIstatistik_SelectedIndexChanged(null, null);
        }

        private void CmbIstatistik_SelectedIndexChanged(object sender, EventArgs e)
        {
            DateTime baslangicTarihi = DateTime.Now;
            switch (cmbIstatistik.SelectedIndex)
            {
                case 0: baslangicTarihi = DateTime.Today; break;
                case 1: baslangicTarihi = DateTime.Now.AddDays(-7); break;
                case 2: baslangicTarihi = DateTime.Now.AddDays(-30); break;
            }

            try
            {
                if (bgl.baglanti().State == ConnectionState.Closed) bgl.baglanti().Open();
                string sql = "Select Count(*) From Tbl_Hareketler Where TalepTarihi >= @tarih";
                SqlCommand komut = new SqlCommand(sql, bgl.baglanti());
                komut.Parameters.AddWithValue("@tarih", baslangicTarihi.ToString("yyyy-MM-dd HH:mm:ss"));
                object sonuc = komut.ExecuteScalar();
                lblIstatistikSonuc.Text = sonuc != null ? sonuc.ToString() : "0";
            }
            catch { lblIstatistikSonuc.Text = "Hata"; }
            finally { bgl.baglanti().Close(); }
        }

        void IstatistikleriGetir()
        {
            lblToplamKitap.Text = TekDegerGetir("Select Sum(StokAdeti) From Tbl_Kitaplar");
            lblAktifOdunc.Text = TekDegerGetir("Select Count(*) From Tbl_Hareketler Where Durum IN ('Beklemede', 'Onaylandı', 'Teslim Edildi')");
            lblToplamUye.Text = TekDegerGetir("Select Count(*) From Tbl_Kullanicilar");
        }

        string TekDegerGetir(string sql)
        {
            try
            {
                if (bgl.baglanti().State == ConnectionState.Closed) bgl.baglanti().Open();
                SqlCommand komut = new SqlCommand(sql, bgl.baglanti());
                object sonuc = komut.ExecuteScalar();
                bgl.baglanti().Close();
                return sonuc != null ? sonuc.ToString() : "0";
            }
            catch { return "0"; }
        }

        private TextBox InputEkle(GroupBox grp, string labelText, int x, int y, int width)
        {
            Label lbl = new Label { Text = labelText, Location = new Point(x, y - 20), AutoSize = true, ForeColor = Color.Gray };
            grp.Controls.Add(lbl);
            TextBox txt = new TextBox { Location = new Point(x, y), Size = new Size(width, 25) };
            grp.Controls.Add(txt);
            return txt;
        }

        private Button ButonOlustur(string text, Color renk, int x, int y)
        {
            Button btn = new Button { Text = text, Location = new Point(x, y), Size = new Size(110, 40), BackColor = renk, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            return btn;
        }

        private Label KartEkle(TabPage page, string baslik, string deger, Color renk, int x, int y)
        {
            Panel pnl = new Panel { Location = new Point(x, y), Size = new Size(350, 160), BackColor = renk };
            page.Controls.Add(pnl);
            Label lblVal = new Label { Text = deger, Location = new Point(20, 65), ForeColor = Color.White, Font = new Font("Segoe UI", 36, FontStyle.Bold), AutoSize = true };
            pnl.Controls.Add(lblVal);
            pnl.Controls.Add(new Label { Text = baslik, Location = new Point(20, 20), ForeColor = Color.White, Font = new Font("Segoe UI", 14), AutoSize = true });
            return lblVal;
        }

        private void GridTasarimUygula(DataGridView dgw)
        {
            dgw.AllowUserToAddRows = false;
            dgw.BackgroundColor = Color.White;
            dgw.BorderStyle = BorderStyle.None;
            dgw.EnableHeadersVisualStyles = false;
            dgw.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 73, 94);
            dgw.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgw.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            dgw.ColumnHeadersHeight = 45;
            dgw.DefaultCellStyle.Font = new Font("Segoe UI", 11);
            dgw.RowTemplate.Height = 35;
            dgw.DefaultCellStyle.SelectionBackColor = Color.FromArgb(44, 62, 80);
            dgw.RowHeadersVisible = false;
            dgw.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 246, 250);
        }
    }
}