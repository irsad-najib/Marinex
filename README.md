# Marinex

**Maritime Integrated Explorer**

Sebuah aplikasi desktop berbasis WPF yang dirancang untuk manajemen operasional maritim terpadu. Aplikasi ini mengintegrasikan ship tracking real-time, maintenance management, safety reporting, environmental monitoring, dan weather forecasting dalam satu platform.

### Services

- **SupabaseService** - Database operations
- **AISStreamService** - WebSocket connection untuk real-time ship tracking
- **WeatherService** - OpenWeatherMap API integration
- **ReportService** - CRUD operations untuk all report types
- **ShipDatabaseService, VoyageDatabaseService** - Specialized database services

## 🚀 Quick Start


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
   - Username : `testing`
   - Password: `testing123`

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
