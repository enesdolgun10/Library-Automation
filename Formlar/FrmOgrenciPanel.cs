using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using KutuphaneOtomasyonu.VeriBaglantisi;
using KutuphaneOtomasyonu;

namespace KutuphaneOtomasyonu.Formlar
{
    public partial class FrmOgrenciPanel : Form
    {
        public string kullaniciMail;
        private int secilenKitapID = -1;

        Baglanti bgl = new Baglanti();
        DataTable dtTumKitaplar;

        TabControl tabControl;
        DataGridView dgwKitaplar, dgwGecmis;
        TextBox txtArama;
        Button btnTalepEt;

        GroupBox grpDetay;
        Label lblSecilenAd, lblSecilenYazar, lblSecilenStok, lblSecilenRaf;
        RichTextBox rtbOzet;

        public FrmOgrenciPanel()
        {
            this.Text = "Öğrenci Paneli";
            this.Size = new Size(1100, 680);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Load += FrmOgrenciPanel_Load;

            ArayuzuOlustur();
        }

        private void ArayuzuOlustur()
        {
            tabControl = new TabControl();
            tabControl.Dock = DockStyle.Fill;
            tabControl.Font = new Font("Segoe UI", 10);
            tabControl.ItemSize = new Size(100, 30);
            this.Controls.Add(tabControl);

            TabPage pageKitaplar = new TabPage("📚 Kitap Arama & Talep");
            pageKitaplar.BackColor = Color.WhiteSmoke;
            tabControl.TabPages.Add(pageKitaplar);

            Label lblAra = new Label();
            lblAra.Text = "🔍 Detaylı Arama (Ad, Yazar, Kategori):";
            lblAra.Location = new Point(20, 25);
            lblAra.AutoSize = true;
            lblAra.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblAra.ForeColor = Color.DarkSlateGray;
            pageKitaplar.Controls.Add(lblAra);

            txtArama = new TextBox();
            txtArama.Location = new Point(350, 22);
            txtArama.Size = new Size(250, 27);
            txtArama.Font = new Font("Segoe UI", 11);
            txtArama.TextChanged += TxtArama_TextChanged;
            pageKitaplar.Controls.Add(txtArama);

            dgwKitaplar = new DataGridView();
            dgwKitaplar.Location = new Point(20, 70);
            dgwKitaplar.Size = new Size(700, 500);
            dgwKitaplar.ReadOnly = true;
            dgwKitaplar.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgwKitaplar.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgwKitaplar.CellClick += DgwKitaplar_CellClick;
            GridTasarimUygula(dgwKitaplar);
            pageKitaplar.Controls.Add(dgwKitaplar);

            grpDetay = new GroupBox();
            grpDetay.Text = "  Seçili Kitap Detayları  ";
            grpDetay.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            grpDetay.ForeColor = Color.DarkSlateBlue;
            grpDetay.Location = new Point(740, 70);
            grpDetay.Size = new Size(320, 500);
            grpDetay.BackColor = Color.White;
            pageKitaplar.Controls.Add(grpDetay);

            int y = 40;
            lblSecilenAd = DetayLabelEkle(grpDetay, "Kitap Adı: -", y, 12, true); y += 40;
            lblSecilenYazar = DetayLabelEkle(grpDetay, "Yazar: -", y, 11, false); y += 40;

            lblSecilenRaf = DetayLabelEkle(grpDetay, "📍 Raf: -", y, 10, false);
            lblSecilenRaf.ForeColor = Color.Crimson;

            lblSecilenStok = DetayLabelEkle(grpDetay, "📦 Stok: -", y + 30, 10, false);
            lblSecilenStok.ForeColor = Color.Teal;

            y += 70;

            Label lblOzetBaslik = new Label();
            lblOzetBaslik.Text = "📖 Kitap Özeti:";
            lblOzetBaslik.Location = new Point(15, y);
            lblOzetBaslik.AutoSize = true;
            lblOzetBaslik.ForeColor = Color.Gray;
            grpDetay.Controls.Add(lblOzetBaslik);

            rtbOzet = new RichTextBox();
            rtbOzet.Location = new Point(15, y + 25);
            rtbOzet.Size = new Size(290, 200);
            rtbOzet.BorderStyle = BorderStyle.None;
            rtbOzet.BackColor = Color.WhiteSmoke;
            rtbOzet.ReadOnly = true;
            grpDetay.Controls.Add(rtbOzet);

            btnTalepEt = new Button();
            btnTalepEt.Text = "BU KİTABI TALEP ET";
            btnTalepEt.Dock = DockStyle.Bottom;
            btnTalepEt.Height = 50;
            btnTalepEt.BackColor = Color.Teal;
            btnTalepEt.ForeColor = Color.White;
            btnTalepEt.FlatStyle = FlatStyle.Flat;
            btnTalepEt.Cursor = Cursors.Hand;
            btnTalepEt.Click += BtnTalepEt_Click;
            grpDetay.Controls.Add(btnTalepEt);

            TabPage pageGecmis = new TabPage("🕒 Geçmiş Hareketlerim");
            pageGecmis.BackColor = Color.White;
            tabControl.TabPages.Add(pageGecmis);

            dgwGecmis = new DataGridView();
            dgwGecmis.Dock = DockStyle.Fill;
            dgwGecmis.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgwGecmis.ReadOnly = true;
            GridTasarimUygula(dgwGecmis);
            pageGecmis.Controls.Add(dgwGecmis);

            Button btnCikis = new Button();
            btnCikis.Text = "Oturumu Kapat";
            btnCikis.Size = new Size(140, 30);
            btnCikis.Location = new Point(this.ClientSize.Width - 150, 2);
            btnCikis.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnCikis.BackColor = Color.IndianRed;
            btnCikis.ForeColor = Color.White;
            btnCikis.FlatStyle = FlatStyle.Flat;
            btnCikis.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btnCikis.Cursor = Cursors.Hand;
            btnCikis.Click += (s, e) =>
            {
                FrmGiris fr = new FrmGiris();
                fr.Show();
                this.Close();
            };
            this.Controls.Add(btnCikis);
            btnCikis.BringToFront();
        }

        private Label DetayLabelEkle(GroupBox grp, string text, int y, int fontSize, bool bold)
        {
            Label lbl = new Label();
            lbl.Text = text;
            lbl.Location = new Point(15, y);
            lbl.AutoSize = true;
            lbl.MaximumSize = new Size(290, 0);
            lbl.Font = new Font("Segoe UI", fontSize, bold ? FontStyle.Bold : FontStyle.Regular);
            grp.Controls.Add(lbl);
            return lbl;
        }

        private void FrmOgrenciPanel_Load(object sender, EventArgs e)
        {
            try
            {
                SqlCommand komut = new SqlCommand("Select Ad, Soyad From Tbl_Kullanicilar Where Email=@p1", bgl.baglanti());
                komut.Parameters.AddWithValue("@p1", kullaniciMail);
                SqlDataReader dr = komut.ExecuteReader();
                if (dr.Read()) this.Text = "Hoşgeldin: " + dr["Ad"] + " " + dr["Soyad"];
                bgl.baglanti().Close();
            }
            catch { this.Text = "Hoşgeldin: " + kullaniciMail; }

            KitaplariHafizayaAl();
            GecmisiListele();
        }

        void KitaplariHafizayaAl()
        {
            string sql = "Select KitapID, KitapAd, Yazar, Kategori, SayfaSayisi, StokAdeti, RafKonumu, Ozet From Tbl_Kitaplar Where Durum=1";

            try
            {
                SqlDataAdapter da = new SqlDataAdapter(sql, bgl.baglanti());
                dtTumKitaplar = new DataTable();
                da.Fill(dtTumKitaplar);
                dgwKitaplar.DataSource = dtTumKitaplar;

                if (dgwKitaplar.Columns.Contains("Ozet")) dgwKitaplar.Columns["Ozet"].Visible = false;
                if (dgwKitaplar.Columns.Contains("RafKonumu")) dgwKitaplar.Columns["RafKonumu"].Visible = false;
                if (dgwKitaplar.Columns.Contains("KitapID")) dgwKitaplar.Columns["KitapID"].Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veritabanı sütun hatası! RafKonumu veya Ozet sütunları eksik olabilir.\n" + ex.Message);
            }
        }

        private void DgwKitaplar_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgwKitaplar.Rows[e.RowIndex];

                secilenKitapID = int.Parse(row.Cells["KitapID"].Value.ToString());

                lblSecilenAd.Text = row.Cells["KitapAd"].Value.ToString();
                lblSecilenYazar.Text = "Yazar: " + row.Cells["Yazar"].Value.ToString();
                lblSecilenStok.Text = "📦 Stok Adeti: " + row.Cells["StokAdeti"].Value.ToString();

                string raf = row.Cells["RafKonumu"].Value != DBNull.Value ? row.Cells["RafKonumu"].Value.ToString() : "Belirtilmemiş";
                lblSecilenRaf.Text = "📍 Raf: " + raf;

                string ozet = row.Cells["Ozet"].Value != DBNull.Value ? row.Cells["Ozet"].Value.ToString() : "Özet bilgisi bulunmamaktadır.";
                rtbOzet.Text = ozet;
            }
        }

        private void GridTasarimUygula(DataGridView dgw)
        {
            dgw.AllowUserToAddRows = false;
            dgw.BackgroundColor = Color.White;
            dgw.BorderStyle = BorderStyle.None;
            dgw.EnableHeadersVisualStyles = false;
            dgw.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(20, 25, 72);
            dgw.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgw.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgw.ColumnHeadersHeight = 35;
            dgw.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgw.DefaultCellStyle.SelectionBackColor = Color.SeaGreen;
            dgw.RowHeadersVisible = false;
            dgw.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(238, 239, 249);
        }

        void GecmisiListele()
        {
            string sorgu = @"
                Select h.HareketID, k.KitapAd, h.TalepTarihi, h.IadeTarihi AS 'Son Teslim Tarihi', h.Durum 
                From Tbl_Hareketler h
                Inner Join Tbl_Kitaplar k ON h.KitapID = k.KitapID
                Inner Join Tbl_Kullanicilar u ON h.KullaniciID = u.KullaniciID
                Where u.Email = @p1 Order By h.TalepTarihi DESC";

            SqlDataAdapter da = new SqlDataAdapter(sorgu, bgl.baglanti());
            da.SelectCommand.Parameters.AddWithValue("@p1", kullaniciMail);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dgwGecmis.DataSource = dt;

            if (dgwGecmis.Columns.Contains("HareketID"))
            {
                dgwGecmis.Columns["HareketID"].Visible = false;
            }

            GecikmeleriKontrolEt();
        }

        void GecikmeleriKontrolEt()
        {
            DateTime bugun = DateTime.Now;

            foreach (DataGridViewRow row in dgwGecmis.Rows)
            {
                if (row.IsNewRow) continue;

                if (row.Cells["Son Teslim Tarihi"].Value != null &&
                    row.Cells["Son Teslim Tarihi"].Value != DBNull.Value &&
                    row.Cells["Durum"].Value != null)
                {
                    DateTime iadeTarihi = Convert.ToDateTime(row.Cells["Son Teslim Tarihi"].Value);
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

        private void TxtArama_TextChanged(object sender, EventArgs e)
        {
            if (dtTumKitaplar != null)
            {
                string aranan = txtArama.Text;
                dtTumKitaplar.DefaultView.RowFilter = string.Format(
                    "KitapAd LIKE '%{0}%' OR Yazar LIKE '%{0}%' OR Kategori LIKE '%{0}%'",
                    aranan
                );
            }
        }

        private void BtnTalepEt_Click(object sender, EventArgs e)
        {
            if (secilenKitapID == -1)
            {
                MessageBox.Show("Lütfen listeden bir kitap seçiniz.");
                return;
            }
            try
            {
                SqlCommand kmtID = new SqlCommand("Select KullaniciID From Tbl_Kullanicilar where Email=@mail", bgl.baglanti());
                kmtID.Parameters.AddWithValue("@mail", kullaniciMail);
                int uID = (int)kmtID.ExecuteScalar();

                string kontrolSorgusu = "Select Count(*) From Tbl_Hareketler Where KullaniciID=@p1 AND KitapID=@p2 AND Durum IN ('Beklemede', 'Onaylandı', 'Teslim Edildi')";
                SqlCommand kmtKontrol = new SqlCommand(kontrolSorgusu, bgl.baglanti());
                kmtKontrol.Parameters.AddWithValue("@p1", uID);
                kmtKontrol.Parameters.AddWithValue("@p2", secilenKitapID);
                int kayitSayisi = (int)kmtKontrol.ExecuteScalar();

                if (kayitSayisi > 0)
                {
                    MessageBox.Show("Bu kitap için zaten işleminiz devam ediyor.", "İzin Verilmedi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                SqlCommand stokKomut = new SqlCommand("Select StokAdeti From Tbl_Kitaplar where KitapID=@k1", bgl.baglanti());
                stokKomut.Parameters.AddWithValue("@k1", secilenKitapID);
                int stok = (int)stokKomut.ExecuteScalar();

                if (stok <= 0)
                {
                    MessageBox.Show("Stok tükenmiş!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                SqlCommand komut = new SqlCommand("INSERT INTO Tbl_Hareketler (KullaniciID, KitapID, Durum) VALUES (@p1, @p2, 'Beklemede')", bgl.baglanti());
                komut.Parameters.AddWithValue("@p1", uID);
                komut.Parameters.AddWithValue("@p2", secilenKitapID);
                komut.ExecuteNonQuery();

                MessageBox.Show("Talep Gönderildi!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                GecmisiListele();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
        }
    }
}