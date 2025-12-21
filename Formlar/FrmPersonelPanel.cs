using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using KutuphaneOtomasyonu.VeriBaglantisi;
using KutuphaneOtomasyonu;

namespace KutuphaneOtomasyonu.Formlar
{
    public partial class FrmPersonelPanel : Form
    {
        Baglanti bgl = new Baglanti();

        DataGridView dgwTalepler;
        ContextMenuStrip sagTikMenu;
        Label lblBugunVerilen, lblToplamIade, lblGeciken, lblBekleyen;

        int secilenHareketID = -1;
        int secilenKitapID = -1;
        string secilenDurum = "";

        public FrmPersonelPanel()
        {
            this.Text = "Personel Paneli - Günlük Ödünç Özeti ve Talep Yönetimi";
            this.Size = new Size(1250, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Load += FrmPersonelPanel_Load;

            ArayuzuOlustur();
        }

        private void ArayuzuOlustur()
        {
            Panel pnlHeader = new Panel();
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Height = 50;
            pnlHeader.BackColor = Color.FromArgb(40, 40, 60);
            this.Controls.Add(pnlHeader);

            Label lblBaslik = new Label();
            lblBaslik.Text = "PERSONEL KONTROL PANELİ";
            lblBaslik.ForeColor = Color.White;
            lblBaslik.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblBaslik.AutoSize = true;
            lblBaslik.Location = new Point(20, 10);
            pnlHeader.Controls.Add(lblBaslik);

            Button btnCikis = new Button();
            btnCikis.Text = "🚪 Çıkış";
            btnCikis.Size = new Size(100, 30);
            btnCikis.Location = new Point(this.ClientSize.Width - 120, 10);
            btnCikis.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnCikis.BackColor = Color.IndianRed;
            btnCikis.ForeColor = Color.White;
            btnCikis.FlatStyle = FlatStyle.Flat;
            btnCikis.Click += (s, e) => { new FrmGiris().Show(); this.Close(); };
            pnlHeader.Controls.Add(btnCikis);

            Panel pnlOzet = new Panel();
            pnlOzet.Dock = DockStyle.Top;
            pnlOzet.Height = 110;
            pnlOzet.BackColor = Color.WhiteSmoke;
            this.Controls.Add(pnlOzet);

            Label lblOzetBaslik = new Label();
            lblOzetBaslik.Text = "📊 GÜNLÜK ÖDÜNÇ ÖZETİ";
            lblOzetBaslik.ForeColor = Color.DarkSlateGray;
            lblOzetBaslik.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblOzetBaslik.Location = new Point(20, 5);
            lblOzetBaslik.AutoSize = true;
            pnlOzet.Controls.Add(lblOzetBaslik);

            lblBugunVerilen = OzetKartiOlustur(pnlOzet, "Bugün Verilen", "0", Color.Teal, 20);
            Button btnVerilenGoster = OzetButonu(pnlOzet, "Listele", 20, 70, Color.Teal);
            btnVerilenGoster.Click += BtnBugunVerilen_Click;

            lblToplamIade = OzetKartiOlustur(pnlOzet, "Toplam İade", "0", Color.SeaGreen, 250);
            Button btnIadeGoster = OzetButonu(pnlOzet, "Listele", 250, 70, Color.SeaGreen);
            btnIadeGoster.Click += BtnIadeler_Click;

            lblGeciken = OzetKartiOlustur(pnlOzet, "Geciken Kitaplar", "0", Color.Crimson, 480);
            Button btnGecikenGoster = OzetButonu(pnlOzet, "Listele", 480, 70, Color.Crimson);
            btnGecikenGoster.Click += BtnGecikenler_Click;

            lblBekleyen = OzetKartiOlustur(pnlOzet, "Onay Bekleyen", "0", Color.DarkOrange, 710);
            Button btnBekleyenGoster = OzetButonu(pnlOzet, "Talepleri Gör", 710, 70, Color.DarkOrange);
            btnBekleyenGoster.Click += (s, e) => { TalepleriListele("h.Durum = 'Beklemede'"); };

            Button btnTumu = new Button();
            btnTumu.Text = "📋 TÜM İŞLEMLERİ LİSTELE";
            btnTumu.Location = new Point(940, 30);
            btnTumu.Size = new Size(200, 65);
            btnTumu.BackColor = Color.SlateGray;
            btnTumu.ForeColor = Color.White;
            btnTumu.FlatStyle = FlatStyle.Flat;
            btnTumu.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnTumu.Cursor = Cursors.Hand;
            btnTumu.Click += (s, e) =>
            {
                TalepleriListele();
            };
            pnlOzet.Controls.Add(btnTumu);

            dgwTalepler = new DataGridView();
            dgwTalepler.Dock = DockStyle.Fill;
            dgwTalepler.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgwTalepler.ReadOnly = true;
            dgwTalepler.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            dgwTalepler.CellClick += DgwTalepler_CellClick;
            dgwTalepler.CellMouseDown += DgwTalepler_CellMouseDown;

            GridTasarimUygula(dgwTalepler);

            this.Controls.Add(dgwTalepler);
            dgwTalepler.BringToFront();
            pnlOzet.SendToBack();
            pnlHeader.SendToBack();

            sagTikMenu = new ContextMenuStrip();
            ToolStripMenuItem itemOnayla = new ToolStripMenuItem("✅ Talebi Onayla");
            itemOnayla.Click += IslemiOnayla_Click;
            ToolStripMenuItem itemTeslim = new ToolStripMenuItem("📦 Kitabı Teslim Et (15 Gün)");
            itemTeslim.Click += TeslimEt_Click;
            ToolStripMenuItem itemIade = new ToolStripMenuItem("🔙 Kitabı İade Al");
            itemIade.Click += IadeAl_Click;
            sagTikMenu.Items.AddRange(new ToolStripItem[] { itemOnayla, itemTeslim, itemIade });
            dgwTalepler.ContextMenuStrip = sagTikMenu;
        }

        private Label OzetKartiOlustur(Panel pnl, string baslik, string deger, Color renk, int x)
        {
            Panel box = new Panel();
            box.Location = new Point(x, 30);
            box.Size = new Size(200, 40);
            box.BackColor = Color.White;
            box.BorderStyle = BorderStyle.FixedSingle;
            pnl.Controls.Add(box);

            Label lblBaslik = new Label();
            lblBaslik.Text = baslik + ":";
            lblBaslik.Location = new Point(5, 10);
            lblBaslik.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            lblBaslik.ForeColor = Color.Gray;
            lblBaslik.AutoSize = true;
            box.Controls.Add(lblBaslik);

            Label lblDeger = new Label();
            lblDeger.Text = deger;
            lblDeger.Location = new Point(130, 8);
            lblDeger.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblDeger.ForeColor = renk;
            lblDeger.AutoSize = true;
            box.Controls.Add(lblDeger);

            return lblDeger;
        }

        private Button OzetButonu(Panel pnl, string text, int x, int y, Color renk)
        {
            Button btn = new Button();
            btn.Text = text;
            btn.Location = new Point(x, y);
            btn.Size = new Size(200, 25);
            btn.FlatStyle = FlatStyle.Flat;
            btn.BackColor = renk;
            btn.ForeColor = Color.White;
            btn.Font = new Font("Segoe UI", 8, FontStyle.Bold);
            btn.Cursor = Cursors.Hand;
            pnl.Controls.Add(btn);
            return btn;
        }

        private void GridTasarimUygula(DataGridView dgw)
        {
            dgw.AllowUserToAddRows = false;
            dgw.BackgroundColor = Color.White;
            dgw.BorderStyle = BorderStyle.None;
            dgw.EnableHeadersVisualStyles = false;
            dgw.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(40, 40, 60);
            dgw.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgw.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgw.ColumnHeadersHeight = 35;
            dgw.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgw.DefaultCellStyle.SelectionBackColor = Color.SeaGreen;
            dgw.RowHeadersVisible = false;
            dgw.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 245);
        }

        private void FrmPersonelPanel_Load(object sender, EventArgs e)
        {
            TalepleriListele();
            OzetBilgileriGuncelle();
        }

        void TalepleriListele(string filtreSql = "")
        {
            string sql = @"
                Select 
                    h.HareketID,
                    (u.Ad + ' ' + u.Soyad) AS 'Öğrenci',
                    k.KitapAd AS 'Kitap',
                    h.TalepTarihi,
                    h.IadeTarihi AS 'Son Teslim',
                    h.Durum,
                    h.KitapID
                From Tbl_Hareketler h
                Inner Join Tbl_Kullanicilar u ON h.KullaniciID = u.KullaniciID
                Inner Join Tbl_Kitaplar k ON h.KitapID = k.KitapID ";

            if (!string.IsNullOrEmpty(filtreSql))
            {
                sql += " WHERE " + filtreSql;
            }

            sql += " Order By h.TalepTarihi DESC";

            SqlDataAdapter da = new SqlDataAdapter(sql, bgl.baglanti());
            DataTable dt = new DataTable();
            da.Fill(dt);
            dgwTalepler.DataSource = dt;

            if (dgwTalepler.Columns.Contains("KitapID")) dgwTalepler.Columns["KitapID"].Visible = false;
            if (dgwTalepler.Columns.Contains("HareketID")) dgwTalepler.Columns["HareketID"].Visible = false;

            GecikmeleriKontrolEt();
        }

        void OzetBilgileriGuncelle()
        {
            try
            {
                if (bgl.baglanti().State == ConnectionState.Closed) bgl.baglanti().Open();

                string bugun = DateTime.Now.ToString("yyyy-MM-dd");

                SqlCommand cmd1 = new SqlCommand($"Select Count(*) From Tbl_Hareketler Where TalepTarihi >= '{bugun}' AND Durum != 'Beklemede'", bgl.baglanti());
                lblBugunVerilen.Text = cmd1.ExecuteScalar().ToString();

                SqlCommand cmd2 = new SqlCommand("Select Count(*) From Tbl_Hareketler Where Durum='İade Edildi'", bgl.baglanti());
                lblToplamIade.Text = cmd2.ExecuteScalar().ToString();

                SqlCommand cmd3 = new SqlCommand("Select Count(*) From Tbl_Hareketler Where Durum='Teslim Edildi' AND IadeTarihi < GETDATE()", bgl.baglanti());
                lblGeciken.Text = cmd3.ExecuteScalar().ToString();

                SqlCommand cmd4 = new SqlCommand("Select Count(*) From Tbl_Hareketler Where Durum='Beklemede'", bgl.baglanti());
                lblBekleyen.Text = cmd4.ExecuteScalar().ToString();
            }
            catch { }
            finally { bgl.baglanti().Close(); }
        }

        private void BtnBugunVerilen_Click(object sender, EventArgs e)
        {
            string bugun = DateTime.Now.ToString("yyyy-MM-dd");
            TalepleriListele($"h.TalepTarihi >= '{bugun}' AND h.Durum != 'Beklemede'");
        }

        private void BtnIadeler_Click(object sender, EventArgs e)
        {
            TalepleriListele("h.Durum = 'İade Edildi'");
        }

        private void BtnGecikenler_Click(object sender, EventArgs e)
        {
            TalepleriListele("h.Durum = 'Teslim Edildi' AND h.IadeTarihi < GETDATE()");
        }

        void GecikmeleriKontrolEt()
        {
            DateTime bugun = DateTime.Now;
            foreach (DataGridViewRow row in dgwTalepler.Rows)
            {
                if (row.IsNewRow) continue;

                if (row.Cells["Son Teslim"].Value != null &&
                    row.Cells["Son Teslim"].Value != DBNull.Value &&
                    row.Cells["Durum"].Value != null)
                {
                    DateTime iadeTarihi = Convert.ToDateTime(row.Cells["Son Teslim"].Value);
                    string durum = row.Cells["Durum"].Value.ToString();

                    if (durum == "Teslim Edildi" && bugun > iadeTarihi)
                    {
                        row.DefaultCellStyle.BackColor = Color.DarkRed;
                        row.DefaultCellStyle.ForeColor = Color.White;
                        row.Cells["Durum"].Value = "GECİKTİ!";
                    }
                }
            }
        }

        private void IslemiOnayla_Click(object sender, EventArgs e)
        {
            if (secilenDurum != "Beklemede") { MessageBox.Show("Sadece 'Beklemede' olanlar onaylanabilir."); return; }
            DurumGuncelle("Onaylandı", null);
        }

        private void TeslimEt_Click(object sender, EventArgs e)
        {
            if (secilenDurum != "Onaylandı") { MessageBox.Show("Önce talebi onaylayın."); return; }

            DateTime sonTarih = DateTime.Now.AddDays(15);
            StokGuncelle(secilenKitapID, -1);
            DurumGuncelle("Teslim Edildi", sonTarih);
            MessageBox.Show($"Kitap teslim edildi. Son tarih: {sonTarih.ToShortDateString()}");
        }

        private void IadeAl_Click(object sender, EventArgs e)
        {
            if (secilenDurum != "Teslim Edildi" && secilenDurum != "GECİKTİ!") { MessageBox.Show("Sadece teslim edilenler iade alınabilir."); return; }

            StokGuncelle(secilenKitapID, 1);
            DurumGuncelle("İade Edildi", null);
        }

        void DurumGuncelle(string yeniDurum, DateTime? tarih)
        {
            try
            {
                string sql = "Update Tbl_Hareketler Set Durum=@p1 Where HareketID=@p2";
                if (tarih != null) sql = "Update Tbl_Hareketler Set Durum=@p1, IadeTarihi=@p3 Where HareketID=@p2";

                if (bgl.baglanti().State == ConnectionState.Closed) bgl.baglanti().Open();
                SqlCommand komut = new SqlCommand(sql, bgl.baglanti());
                komut.Parameters.AddWithValue("@p1", yeniDurum);
                komut.Parameters.AddWithValue("@p2", secilenHareketID);
                if (tarih != null) komut.Parameters.AddWithValue("@p3", tarih);

                komut.ExecuteNonQuery();
                TalepleriListele();
                OzetBilgileriGuncelle();
            }
            catch (Exception ex) { MessageBox.Show("Hata: " + ex.Message); }
            finally { bgl.baglanti().Close(); }
        }

        void StokGuncelle(int kitapId, int miktar)
        {
            try
            {
                if (bgl.baglanti().State == ConnectionState.Closed) bgl.baglanti().Open();
                SqlCommand komut = new SqlCommand("Update Tbl_Kitaplar Set StokAdeti = StokAdeti + @adet Where KitapID=@id", bgl.baglanti());
                komut.Parameters.AddWithValue("@adet", miktar);
                komut.Parameters.AddWithValue("@id", kitapId);
                komut.ExecuteNonQuery();
            }
            catch { }
            finally { bgl.baglanti().Close(); }
        }

        private void DgwTalepler_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            SatirSec(e.RowIndex);
        }

        private void DgwTalepler_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex >= 0)
            {
                dgwTalepler.CurrentCell = dgwTalepler.Rows[e.RowIndex].Cells[e.ColumnIndex];
                dgwTalepler.Rows[e.RowIndex].Selected = true;
                dgwTalepler.Focus();
                SatirSec(e.RowIndex);
            }
        }

        void SatirSec(int rowIndex)
        {
            if (rowIndex >= 0)
            {
                secilenHareketID = int.Parse(dgwTalepler.Rows[rowIndex].Cells["HareketID"].Value.ToString());
                secilenKitapID = int.Parse(dgwTalepler.Rows[rowIndex].Cells["KitapID"].Value.ToString());
                secilenDurum = dgwTalepler.Rows[rowIndex].Cells["Durum"].Value.ToString();
            }
        }
    }
}