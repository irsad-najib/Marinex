# Marinex

**Maritime Integrated Explorer**

Sebuah aplikasi desktop berbasis WPF yang dirancang untuk hybrid offline dan online guna manajemen rute, kapal dan kerusakan, serta memprediksi perbaikan dan cuaca dan memasukkan unsur lingkungan dengan konsentrasi pemantauan sampah lautan.

## 🚢 Features

- **Dashboard Hero** - Landing page dengan informasi maritime intelligence
- **Authentication** - Login & Register dengan BCrypt password hashing
- **Single Page Application** - Navigation tanpa reload window (seperti web app)
- **Ship Management** - Kelola data kapal (coming soon)
- **Voyage Tracking** - Pantau dan kelola perjalanan (coming soon)
- **Maintenance System** - Jadwal dan tracking maintenance (coming soon)
- **Database Integration** - Terhubung dengan Supabase PostgreSQL

## 🛠️ Tech Stack

- **Framework**: .NET 9.0 (WPF - Windows Presentation Foundation)
- **Database**: Supabase (PostgreSQL)
- **Authentication**: BCrypt.Net password hashing
- **Language**: C# 12

## 📦 Package Dependencies

```xml
<PackageReference Include="Npgsql" Version="8.0.5" />
<PackageReference Include="BCrypt.Net-Next" Version="4.0.3" />
<PackageReference Include="System.Configuration.ConfigurationManager" Version="8.0.1" />
```

## 🚀 Quick Start

### Prerequisites

- .NET SDK 9.0 or newer
- Windows OS (untuk WPF)
- Supabase account (untuk database)

### Installation

1. Clone repository:

```bash
git clone https://github.com/irsad-najib/Marinex.git
cd Marinex/Marinex
```

2. Setup Database:

   - Buka [SUPABASE_SETUP.md](Marinex/SUPABASE_SETUP.md)
   - Jalankan SQL schema di Supabase SQL Editor
   - Copy connection string

3. Update Connection String:

   - Edit `Services/SupabaseService.cs`
   - Ganti `[YOUR-PASSWORD]` dengan password Supabase Anda

4. Build & Run:

```bash
dotnet restore
dotnet build
dotnet run
```

## 📁 Project Structure

```
Marinex/
├── Models/              # Data models (User, Ship, Voyage, Maintenance, Weather)
├── Views/               # User Controls (Dashboard, Login, Register)
├── Services/            # Business logic (SupabaseService)
├── Assets/              # Images and resources
├── MainWindow.xaml      # Main application shell
├── App.xaml            # Application resources & styles
└── SUPABASE_SETUP.md   # Database setup guide
```

## 🔐 Security Features

- ✅ BCrypt password hashing (work factor: 11)
- ✅ Parameterized SQL queries (SQL injection prevention)
- ✅ Secure authentication flow
- ⚠️ TODO: Environment variables for connection strings
- ⚠️ TODO: Rate limiting & session management

## 💻 Usage

### First Run

1. App akan membuka Dashboard view
2. Klik **"Sign In"** di navigation bar
3. Untuk test, gunakan kredensial:
   - Email: `admin@marinex.com`
   - Password: `admin123`

### Navigation

- **Dashboard** - Hero landing page (public)
- **Ships** - Ship management (requires login)
- **Voyages** - Voyage tracking (requires login)
- **Maintenance** - Maintenance system (requires login)
- **Sign In/Out** - Authentication

### Register New User

1. Klik **"Sign In"** → **"Sign up"** link
2. Isi form (Username, Email, Company, Password)
3. Setelah sukses → auto redirect ke login
4. Login dengan akun baru

## 🎨 Screenshots

![Dashboard](Assests/90621a57-7518-4808-bc24-6db100602557.jpg)

## 👥 IRVINGO TEAM

- **Irsad Najib Eka Putra** (23/518119/TK/57005) - Konsolidasi dan koordinasi teams serta fullstack [KETUA]
- **Anggita Salsabilla** (23/516001/TK/56775) - UI/UX Design
- **Melvino Rizky Putra Wahyudi** (23/515981/TK/56770) - Front End
- **Abdul Halim Edi Rahmansyah** (23/516603/TK/56796) - Data dan AI analyst

## 📝 License

© 2025 IRVINGO TEAM - All Rights Reserved
