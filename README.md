# Marinex

**Maritime Integrated Explorer**

Sebuah aplikasi desktop berbasis WPF yang dirancang untuk manajemen operasional maritim terpadu. Aplikasi ini mengintegrasikan ship tracking real-time, maintenance management, safety reporting, environmental monitoring, dan weather forecasting dalam satu platform.

## 🚢 Core Features

### 🗺️ Ship Tracking (AIS Integration)

- **Real-time Position Tracking** via AISStream.io WebSocket
- Track kapal berdasarkan MMSI (Maritime Mobile Service Identity)
- Historical position tracking dan route visualization
- Multi-ship monitoring untuk fleet management
- Destination, ETA, dan navigational status

### � Maintenance Management

- Scheduled maintenance tracking dengan priority levels
- Detailed maintenance reports (equipment, issues, parts needed)
- Cost tracking (estimated vs actual)
- Status workflow: Scheduled → In Progress → Done
- Maintenance history dan analytics

### 🛡️ Safety Reporting

- Comprehensive incident reporting system
- Multiple incident types: Injury, Near Miss, Equipment Failure, etc.
- Severity assessment dan investigation tracking
- Preventive measures documentation
- Compliance dengan maritime safety regulations

### 🗑️ Environmental Monitoring - Pollution Reports

- Report sampah dan polusi di laut (plastic, oil spills, chemical discharge)
- Koordinat GPS dan photo evidence
- Quantity estimation dan environmental impact assessment
- Status tracking: Reported → Investigation → Cleanup → Resolved
- Data sharing untuk authorities dan conservation organizations

### 🌦️ Real-time Weather Integration

- Current weather data via OpenWeatherMap API
- 5-day forecast untuk voyage planning
- Maritime-specific data: wind, sea conditions (Beaufort scale), visibility
- Safety assessment: Is it safe to sail?
- Warning levels untuk storm alerts

### 🔐 Authentication & User Management

- Secure login dengan BCrypt password hashing
- Role-based access (Admin, Captain, Engineer, Crew)
- Company-based ship assignment
- User activity logging

### 📊 Dashboard & Reporting

- Comprehensive dashboard dengan real-time data
- Report generation untuk semua jenis reports
- Data visualization (maps, charts, statistics)
- Export capabilities untuk documentation

## 🛠️ Tech Stack

- **Framework**: .NET 9.0 (WPF - Windows Presentation Foundation)
- **Database**: Supabase (PostgreSQL)
- **Authentication**: BCrypt.Net password hashing
- **Language**: C# 12
- **External APIs**:
  - **AISStream.io** - Real-time AIS ship tracking
  - **OpenWeatherMap** - Weather data and forecasts

## 📦 Package Dependencies

```xml
<PackageReference Include="Npgsql" Version="8.0.5" />
<PackageReference Include="BCrypt.Net-Next" Version="4.0.3" />
<PackageReference Include="System.Configuration.ConfigurationManager" Version="8.0.1" />
<PackageReference Include="System.Net.WebSockets.Client" />
<PackageReference Include="System.Text.Json" />
```

## 🏗️ Architecture

### Models

- **Ship, Voyage, User, UserShip** - Core maritime entities
- **BaseReport** (Abstract) - Parent class untuk semua reports
  - MaintenanceReport - Equipment maintenance dan repairs
  - SafetyReport - Safety incidents dan investigations
  - WeatherReport - User-submitted weather observations
  - **PollutionReport** - Environmental pollution detection
- **ShipPosition** - AIS position data
- **Weather, Maintenance** - Supporting models

### Services

- **SupabaseService** - Database operations
- **AISStreamService** - WebSocket connection untuk real-time ship tracking
- **WeatherService** - OpenWeatherMap API integration
- **ReportService** - CRUD operations untuk all report types
- **ShipDatabaseService, VoyageDatabaseService** - Specialized database services

### Views (WPF/XAML)

- Dashboard, Login, Register
- My Ships Management
- Ship Tracking (dengan map)
- Maintenance Management
- Safety Reports
- Pollution Reports
- Weather Dashboard

## 🚀 Quick Start

### Prerequisites

- .NET SDK 9.0 or newer
- Windows OS (untuk WPF)
- Supabase account (free tier available)
- AISStream.io API key (register di https://aisstream.io)
- OpenWeatherMap API key (free tier: 1000 calls/day)

### Installation

1. Clone repository:

```bash
git clone https://github.com/irsad-najib/Marinex.git
cd Marinex/Marinex
```

2. Setup Database:

   - Buka [SUPABASE_SETUP.md](Marinex/SUPABASE_SETUP.md)
   - Jalankan SQL schema di Supabase SQL Editor
   - Run `SCHEMA_UPDATE.sql` untuk tables baru (pollution_reports, ship_positions_history, weather_data, dll)
   - Copy connection string

3. Get API Keys:

   **AISStream.io:**

   - Register di https://aisstream.io
   - Generate API key (free tier available)

   **OpenWeatherMap:**

   - Register di https://openweathermap.org/api
   - Get API key (free tier: 1000 calls/day)

4. Configure Connection Strings:

   - Edit `Services/SupabaseService.cs` - Ganti `[YOUR-PASSWORD]` dengan password Supabase Anda
   - Create `Config.cs`:

   ```csharp
   public static class Config
   {
       public const string AIS_API_KEY = "your-aisstream-api-key";
       public const string WEATHER_API_KEY = "your-openweather-api-key";
   }
   ```

5. Build & Run:

```bash
dotnet restore
dotnet build
dotnet run
```

## 📘 Documentation

Untuk dokumentasi lengkap tentang semua fitur, use cases, dan technical details, lihat:

**[📖 FEATURES.md](FEATURES.md)** - Comprehensive feature documentation

Dokumentasi ini menjelaskan:

- ✅ Konsep dan use case setiap fitur
- ✅ Technical architecture dan data flow
- ✅ Code examples dan best practices
- ✅ Database schema lengkap
- ✅ API integration guides
- ✅ Future enhancement plans

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

### Main Workflows

#### 1. Setup Your Ship

```
My Ships → Add New Ship
- Enter ship details (Name, Type, MMSI for AIS)
- Enable AIS Tracking
- Save → Ship appears on tracking map
```

#### 2. Track Ships Real-time

```
Ship Tracking → View Map
- See all ships with AIS enabled
- Real-time position updates via WebSocket
- View speed, course, destination, ETA
- Historical track visualization
```

#### 3. Check Weather

```
Weather Dashboard → Enter Location/Coordinates
- Current weather conditions
- Wind speed & sea state (Beaufort scale)
- Safety assessment
- 5-day forecast
- Warning levels
```

#### 4. Create Maintenance Schedule

```
Maintenance → New Maintenance
- Select ship
- Enter equipment details
- Set priority (Low/Medium/High/Critical)
- Schedule date
- Create detailed report
```

#### 5. Report Safety Incident

```
Safety Reports → New Report
- Select incident type
- Document details
- Add people involved
- Immediate actions taken
- Preventive measures
- Submit for investigation
```

#### 6. Report Pollution

```
Pollution Reports → Report Pollution
- Location auto-filled from ship position
- Select waste type (Plastic, Oil, Chemical, etc.)
- Estimate quantity
- Upload photos
- Submit → Authorities notified
```

### Navigation

- **Dashboard** - Main overview dengan statistics
- **My Ships** - Manage fleet
- **Ship Tracking** - Real-time AIS tracking (map view)
- **Maintenance** - Schedule & reports
- **Safety Reports** - Incident reporting & investigation
- **Pollution Reports** - Environmental monitoring
- **Weather** - Current & forecast weather data
- **Sign In/Out** - Authentication

## 🎨 Screenshots

![Dashboard](Assests/90621a57-7518-4808-bc24-6db100602557.jpg)

## 🔄 Development Status

### ✅ Completed

- Authentication system (Login/Register)
- Database integration (Supabase PostgreSQL)
- Ship, Voyage, User models
- AIS Stream service (WebSocket real-time tracking)
- Weather service (OpenWeatherMap integration)
- All Report models (Maintenance, Safety, Weather, Pollution)
- Logging system untuk debugging
- Database schema untuk all features

### 🚧 In Progress

- UI Views (WPF/XAML) untuk semua fitur
- Map visualization untuk ship tracking
- Photo upload untuk pollution reports
- Report CRUD operations
- Dashboard dengan real-time statistics

### 📋 Planned (Future Enhancements)

- Mobile app (cross-platform dengan MAUI)
- Interactive map dengan route planning
- AI-powered predictive maintenance
- Push notifications untuk alerts
- PDF report export
- Analytics dashboard dengan charts
- Multi-language support

## 🔬 Technical Highlights

### OOP Concepts Demonstrated

- **Inheritance** - BaseReport → MaintenanceReport, SafetyReport, WeatherReport, PollutionReport
- **Polymorphism** - Different GenerateReport() implementations
- **Encapsulation** - Private fields dengan public properties
- **Abstraction** - IDatabaseService interface

### Design Patterns

- **Repository Pattern** - Database services
- **Observer Pattern** - Event-driven AIS updates
- **Singleton Pattern** - Service instances
- **Factory Pattern** - Report creation

### Best Practices

- ✅ Async/await untuk all I/O operations
- ✅ Exception handling dengan try-catch
- ✅ Input validation
- ✅ SQL injection prevention (parameterized queries)
- ✅ Password hashing (BCrypt)
- ✅ Comprehensive logging
- ✅ Thread-safe operations
- ✅ Resource disposal (using statements)

## 👥 IRVINGO TEAM

- **Irsad Najib Eka Putra** (23/518119/TK/57005) - Konsolidasi dan koordinasi teams serta fullstack [KETUA]
- **Anggita Salsabilla** (23/516001/TK/56775) - UI/UX Design
- **Melvino Rizky Putra Wahyudi** (23/515981/TK/56770) - Front End
- **Abdul Halim Edi Rahmansyah** (23/516603/TK/56796) - Data dan AI analyst

## 📝 License

© 2025 IRVINGO TEAM - All Rights Reserved
