GameQuest - Oyun Takımı ve Oyuncu Bulma Platformu
GameQuest, çok oyunculu (multiplayer) oyunlarda benzer yetenek seviyelerine sahip oyuncuları ve takımları bir araya getirmek için tasarlanmış, C# ASP.NET Core MVC tabanlı bir web uygulamasıdır.

🚀 Projenin Amacı
Bu proje, oyuncuların oyun içi rolleri, rankları (seviyeleri) ve deneyimlerine göre ilan oluşturmalarını; takımların ise eksik oyuncularını bu kriterlere göre bulmalarını hedefler.

🛠️ Kullanılan Teknolojiler
Backend: C#, .NET 8 / ASP.NET Core MVC

Veritabanı: SQL Server / MS SQL

Frontend: Bootstrap, CSS, HTML

Kütüphaneler: jQuery, Validation Unobtrusive

📂 Proje Yapısı ve Özellikler
İlan Sistemi: Oyuncular ve takımlar için özelleştirilebilir ilanlar.

Kategori Yönetimi: Farklı oyun türlerine göre filtreleme.

Başvuru Takibi: İlanlara yapılan başvuruların durum yönetimi.

Veritabanı Mimarisi: database.sql dosyası ile sunulan ilişkisel tablo yapısı.

🔧 Kurulum
Projeyi klonlayın: git clone [https://github.com/arcadiacommunity36-dev/Web-Final-Proje.git](https://github.com/arcadiacommunity36-dev/Web-Final-Proje.git)

database.sql dosyasını SQL Server üzerinde çalıştırarak veritabanını oluşturun.

appsettings.json dosyasındaki Connection String ayarlarını kendi yerel SQL Server bilgilerinizle güncelleyin.

Visual Studio üzerinden projeyi derleyin ve çalıştırın.

Dosyayı Geri Yükleme İpucu:
Terminalde hala hata alıyorsan, bu metni kopyalayıp bir not defterine yapıştır, adını README.md yap ve proje klasörüne at. Sonra şu 3 komutu yaz:

git add README.md

git commit -m "README dosyası geri yüklendi"

git push origin main
