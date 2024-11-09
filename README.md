# DataInsertProject

## Gereksinimler
- .NET 8.0 SDK
- SQL Server
- RabbitMQ

## Kurulum Adımları

1. Projeyi klonlayın:
    "git clone https://github.com/kullaniciadi/DataInsertProject.git"

2. Gerekli NuGet paketlerini yükleyin:
    "dotnet restore"

3. `appsettings.json` dosyasını düzenleyin ve Veritabanı/RabbitMQ bağlantı ayarlarını kendi ortamınıza göre yapılandırın.

4. Veritabanını oluşturun ve migrationları uygulayın:
    "dotnet ef database update"

5. Projeyi çalıştırın:
    "dotnet run"

