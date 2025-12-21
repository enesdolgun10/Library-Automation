# 📚 Kütüphane Otomasyon Sistemi

Bu proje, kütüphane süreçlerini dijitalleştirmek, kitap takibini kolaylaştırmak ve kullanıcı etkileşimini yönetmek amacıyla **C# Windows Forms** ve **.NET 8.0** kullanılarak geliştirilmiş kapsamlı bir masaüstü uygulamasıdır.

## 🚀 Proje Hakkında

Kütüphane Otomasyon Sistemi; yöneticiler, kütüphane personeli ve öğrenciler (üyeler) için özelleştirilmiş üç farklı panel sunar. Kitap stok takibi, ödünç/iade işlemleri, gecikme takibi ve detaylı istatistiksel raporlama gibi özellikleri barındırır.

## ⚡ Özellikler

### 👤 1. Öğrenci Paneli

- **Detaylı Kitap Arama:** Kitap adı, yazar veya kategoriye göre anlık filtreleme.
- **Kitap Talep Etme:** Stokta olan kitaplar için ödünç talebi oluşturma.
- **Geçmiş Hareketler:** Daha önce alınan kitapların ve iade durumlarının listelenmesi.
- **Gecikme Uyarısı:** Teslim tarihi geçen kitaplar için görsel uyarı sistemi.
- **Kitap Detayları:** Seçilen kitabın özetini, raf konumunu ve stok durumunu görüntüleme.

### 💼 2. Personel Paneli

- **Talep Yönetimi:** Öğrencilerden gelen kitap taleplerini onaylama veya reddetme.
- **Ödünç/İade İşlemleri:** Kitap teslim etme (15 gün süreli) ve iade alma süreçleri.
- **Günlük Özet:** O gün verilen kitaplar, iadeler ve gecikenlerin tek ekranda özeti.
- **Gecikme Takibi:** Teslim tarihi geçen kitapların sistem tarafından otomatik kırmızı ile işaretlenmesi.
- **Stok Entegrasyonu:** İade ve teslim işlemlerinde stok sayılarının otomatik güncellenmesi.

### 🛡️ 3. Yönetici (Admin) Paneli

- **Dashboard (Kontrol Paneli):** Toplam kitap, aktif ödünç, toplam üye sayısı gibi kritik verilerin görsel kartlarla sunumu.
- **Kitap Yönetimi (CRUD):** Yeni kitap ekleme, güncelleme, pasife alma (silme) işlemleri.
- **Kullanıcı Yönetimi:** Öğrenci, Personel veya Yönetici rolleriyle kullanıcı ekleme/düzenleme.
- **Raporlama:** En çok okunan kitaplar (Top 5) ve zaman bazlı (günlük/haftalık/aylık) işlem grafikleri.
- **Gelişmiş Validasyonlar:** Mükerrer kayıt engelleme ve veri bütünlüğü kontrolleri.

### 🔐 Genel Özellikler

- **Güvenli Giriş:** E-posta ve şifre ile rol tabanlı (Role-Based) yönlendirme.
- **Kayıt Ol Ekranı:** Regex kontrolü ile güvenli parola (Büyük harf, sayı zorunluluğu) ve formatlı telefon girişi.

## 🛠️ Teknolojiler

- **Dil:** C#
- **Framework:** .NET 8.0 (Windows Forms)
- **Veritabanı:** Microsoft SQL Server
- **Veri Erişimi:** ADO.NET (`Microsoft.Data.SqlClient`)
- **IDE:** Visual Studio 2022

## ⚙️ Kurulum

Projeyi kendi bilgisayarınızda çalıştırmak için aşağıdaki adımları izleyin:

1.  **Projeyi Klonlayın:**

    ```bash
    git clone [https://github.com/enesdolgun10/library-automation.git](https://github.com/enesdolgun33/library-automation.git)
    ```

2.  **Veritabanını Oluşturun:**
    SQL Server'da `KutuphaneDB` adında bir veritabanı oluşturun ve aşağıdaki tablo yapısını çalıştırın:

    ```sql
    CREATE TABLE Tbl_Kullanicilar (
        KullaniciID INT IDENTITY(1,1) PRIMARY KEY,
        Ad NVARCHAR(50),
        Soyad NVARCHAR(50),
        Email NVARCHAR(100) UNIQUE,
        Sifre NVARCHAR(50),
        OkulNo NVARCHAR(20),
        Telefon NVARCHAR(15),
        Rol TINYINT -- 1:Öğrenci, 2:Personel, 3:Yönetici
    );

    CREATE TABLE Tbl_Kitaplar (
        KitapID INT IDENTITY(1,1) PRIMARY KEY,
        KitapAd NVARCHAR(100),
        Yazar NVARCHAR(100),
        Kategori NVARCHAR(50),
        SayfaSayisi NVARCHAR(10),
        StokAdeti INT,
        YayinYili NVARCHAR(4),
        RafKonumu NVARCHAR(20),
        Ozet NVARCHAR(MAX),
        Durum BIT -- 1:Aktif, 0:Pasif
    );

    CREATE TABLE Tbl_Hareketler (
        HareketID INT IDENTITY(1,1) PRIMARY KEY,
        KullaniciID INT,
        KitapID INT,
        TalepTarihi DATETIME DEFAULT GETDATE(),
        IadeTarihi DATETIME,
        Durum NVARCHAR(20) -- 'Beklemede', 'Onaylandı', 'Teslim Edildi', 'İade Edildi'
    );
    ```

3.  **Bağlantı Adresini Güncelleyin:**
    `VeriBaglantisi/Baglanti.cs` dosyasını açın ve `Data Source` kısmını kendi SQL Server adınızla değiştirin:

    ```csharp
    public SqlConnection baglanti()
    {
        // "ENES\SQLEXPRESS" kısmını kendi sunucu adınızla değiştirin
        SqlConnection baglan = new SqlConnection(@"Data Source=YOUR_SERVER_NAME;Initial Catalog=KutuphaneDB;Integrated Security=True;TrustServerCertificate=True");
        baglan.Open();
        return baglan;
    }
    ```

4.  **Çalıştırın:**
    Projeyi Visual Studio ile açın ve `F5` tuşuna basarak başlatın.

## 📸 Ekran Görüntüleri

|        Giriş Ekranı         |       Yönetici Paneli       |
| :-------------------------: | :-------------------------: |
| ![Giriş](/images/giris.png) | ![Admin](/images/admin.png) |

|          Personel Paneli          |          Öğrenci Paneli           |
| :-------------------------------: | :-------------------------------: |
| ![Personel](/images/personel.png) | ![Öğrenci](/images/kullanici.png) |

## 🤝 Katkıda Bulunma

1.  Bu projeyi forklayın.
2.  Yeni bir özellik dalı (feature branch) oluşturun (`git checkout -b yeni-ozellik`).
3.  Değişikliklerinizi commit yapın (`git commit -m 'Yeni özellik eklendi'`).
4.  Dalınızı pushlayın (`git push origin yeni-ozellik`).
5.  Bir Pull Request oluşturun.

## 📄 Lisans

Bu proje [MIT License](LICENSE) altında lisanslanmıştır.

---

### 👨‍💻 Geliştirici

**Enes Dolgun** - [GitHub Profiliniz](https://github.com/enesdolgun33)
