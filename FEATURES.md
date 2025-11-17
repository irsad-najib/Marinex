# 🚢 MARINEX - Maritime Integrated Explorer

## **Aplikasi Manajemen Operasional Maritim Terpadu**

<Sistem tracking dan monitoring kapal dengan fitur lengkap untuk operasional, maintenance, safety, dan environmental monitoring>

---

## 📋 DAFTAR ISI

1. [Konsep Aplikasi](#konsep-aplikasi)
2. [Fitur Utama](#fitur-utama)
3. [Teknologi & Integrasi](#teknologi--integrasi)
4. [User Flow](#user-flow)
5. [API & Services](#api--services)
6. [Database Schema](#database-schema)
7. [Setup & Installation](#setup--installation)

---

## 🎯 KONSEP APLIKASI

### **Problem Statement**

Perusahaan pelayaran membutuhkan sistem terpadu untuk:

- ✅ **Tracking kapal** secara real-time
- ✅ **Monitoring maintenance** untuk mencegah breakdown
- ✅ **Pelaporan keselamatan** saat operasi/perbaikan
- ✅ **Environmental monitoring** - deteksi dan lapor polusi/sampah laut
- ✅ **Cuaca real-time** untuk keputusan operasional

### **Solution: MARINEX**

Platform desktop (WPF) yang mengintegrasikan:

1. **AIS Stream** untuk tracking posisi kapal real-time
2. **Weather API** untuk cuaca dan kondisi laut terkini
3. **Database Supabase** untuk data persistence
4. **Reporting System** untuk semua jenis laporan

### **Use Case Utama**

```
SCENARIO: Operasional Kapal Cargo "MV NUSANTARA"

1. TRACKING
   - Kapal dilengkapi AIS transponder (MMSI: 525012345)
   - Aplikasi track posisi real-time via AIS Stream
   - Captain bisa lihat kapal lain di sekitarnya

2. MAINTENANCE
   - Engine butuh maintenance rutin setiap 500 jam
   - System alert kalau sudah waktunya
   - Engineer create maintenance report + schedule

3. WEATHER CHECK
   - Before departure, check cuaca di route
   - Real-time weather update selama voyage
   - Warning jika cuaca berbahaya

4. SAFETY INCIDENT
   - Minor injury saat maintenance
   - Crew immediately create safety report
   - Management bisa track dan investigate

5. POLLUTION DETECTION
   - Crew spot plastic debris di laut
   - Take photo, log coordinates
   - Submit pollution report
   - Bisa share ke coast guard/authorities
```

---

## 🚀 FITUR UTAMA

### 1. **🗺️ SHIP TRACKING (AIS Integration)**

#### Konsep:

- Setiap kapal punya **MMSI** (Maritime Mobile Service Identity) yang unik
- AIS transponder broadcast posisi, speed, course setiap beberapa detik
- AISStream.io menyediakan WebSocket stream untuk tracking real-time

#### Fitur:

- ✅ **Real-time Position** - Lat/Long, Speed, Course, Heading
- ✅ **Historical Tracking** - Lihat rute perjalanan kapal
- ✅ **Multi-ship View** - Track semua kapal milik company
- ✅ **Destination & ETA** - Info tujuan dan estimasi arrival
- ✅ **Map Visualization** - Show kapal di map (future: OpenStreetMap)

#### Technical:

```csharp
// AISStreamService.cs
- WebSocket connection ke wss://stream.aisstream.io/v0/stream
- Subscribe dengan API Key + Bounding Box
- Filter by MMSI untuk kapal-kapal milik user
- Save position history ke database
- Emit events untuk UI update
```

#### Data Flow:

```
AIS Transponder (on ship)
  → AIS Station (ground/satellite)
    → AISStream.io API
      → Marinex App (WebSocket)
        → Database (history)
          → UI (map/list view)
```

---

### 2. **🔧 MAINTENANCE MANAGEMENT**

#### Konsep:

- Kapal butuh maintenance rutin (engine, hull, equipment)
- Preventive maintenance lebih murah daripada emergency repair
- Tracking jadwal, status, cost, dan reports

#### Fitur:

- ✅ **Maintenance Schedule** - Jadwal maintenance berdasarkan jam operasi/tanggal
- ✅ **Maintenance Reports** - Detail equipment, issue, priority
- ✅ **Status Tracking** - Scheduled → In Progress → Done
- ✅ **Cost Tracking** - Estimated vs Actual cost
- ✅ **Parts Management** - List spare parts yang dibutuhkan
- ✅ **Priority Levels** - Low, Medium, High, Critical, Urgent

#### Maintenance Types:

- **Routine** - Regular inspection dan service
- **Corrective** - Fix masalah yang ditemukan
- **Preventive** - Cegah masalah sebelum terjadi
- **Emergency** - Urgent repair

#### User Flow:

```
1. System alert: "Engine maintenance due in 50 hours"
2. Chief Engineer create maintenance schedule
3. Assign to engineer team
4. Engineer create detailed maintenance report
   - Equipment: Main Engine Cylinder #3
   - Issue: Low compression, need overhaul
   - Priority: High
   - Parts needed: Piston rings, gaskets
   - Estimated duration: 8 hours
5. Mark as "In Progress" saat mulai
6. After complete, mark "Done" + update actual cost
7. System log completion date untuk next schedule
```

---

### 3. **🛡️ SAFETY REPORTS**

#### Konsep:

- Safety first! Setiap incident harus dilaporkan
- Analysis untuk prevent kejadian serupa
- Compliance dengan maritime safety regulations

#### Fitur:

- ✅ **Incident Reporting** - Log semua safety incidents
- ✅ **Incident Types**:
  - Personal Injury
  - Near Miss
  - Equipment Failure
  - Fire/Explosion
  - Man Overboard
  - Collision/Grounding
  - Environmental Incident
- ✅ **Severity Levels** - Low, Medium, High, Critical
- ✅ **Investigation** - Track status: Reported → Under Investigation → Resolved
- ✅ **Preventive Measures** - Document actions untuk prevent reoccurrence
- ✅ **People Involved** - Track semua yang terlibat
- ✅ **Immediate Actions** - Document response yang sudah diambil

#### Especially Important:

- **During Maintenance** - Risk tinggi, banyak equipment exposure
- **Bad Weather** - Deck operation jadi dangerous
- **Night Operation** - Reduced visibility

#### Example Report:

```
SAFETY INCIDENT REPORT

Date/Time: 2025-11-17 14:30 UTC
Location: Pacific Ocean, 10.5°N 125.3°E

Incident Type: Personal Injury
Severity: Medium

Description:
Engineer sustained minor burn injury while performing
maintenance on auxiliary boiler. Hot water spray from
pressure relief valve that was not properly isolated.

People Involved:
- John Doe (2nd Engineer) - injured
- Jane Smith (Chief Engineer) - supervisor

Immediate Action:
- First aid administered
- Engineer stood down from duty
- Boiler system fully isolated

Preventive Measures:
- Review and update boiler maintenance procedures
- Additional safety briefing for all engineers
- Install additional warning signs
- Require two-person team for boiler maintenance

Status: Under Investigation
```

---

### 4. **🗑️ POLLUTION/WASTE REPORTING**

#### Konsep:

- Ocean pollution adalah masalah global
- Ships adalah "eyes on the water" - bisa detect dan report
- Data collection penting untuk environmental protection

#### Fitur:

- ✅ **Waste Detection** - Report sampah/polusi yang ditemukan
- ✅ **Waste Types**:
  - Plastic Debris (bottles, bags, fishing nets)
  - Oil Spill
  - Chemical Discharge
  - Marine Debris (wood, metal)
  - Dead Marine Life
  - Other Pollutants
- ✅ **Quantity Estimation** - Small, Medium, Large, Massive
- ✅ **Coordinates** - Exact location (Lat/Long)
- ✅ **Photo Evidence** - Upload multiple photos
- ✅ **Severity Levels** - Impact assessment
- ✅ **Environmental Impact** - Assessment of damage
- ✅ **Action Tracking** - Status: Reported → Investigation → Cleanup → Resolved

#### Why Important:

- **Data Collection** - Build database of pollution hotspots
- **Alert Authorities** - Notify coast guard, environmental agencies
- **Company Responsibility** - Show environmental commitment
- **Trend Analysis** - Identify pollution patterns

#### Integration Possibilities:

- Share data dengan marine conservation organizations
- Integration dengan cleanup programs
- Contribute to scientific research
- Public awareness campaigns

#### Example Report:

```
POLLUTION REPORT

Location: Sulu Sea, Philippines
Coordinates: 8.0°N, 119.5°E
Date: 2025-11-17 09:15 UTC

Waste Type: Plastic Debris
Quantity: Large
Severity: High

Description:
Large patch of mixed plastic waste observed during
transit. Estimated coverage: 100-150 square meters.
Predominantly plastic bottles, bags, and styrofoam.
Multiple ghost fishing nets visible.

Environmental Impact:
High density area with potential harm to marine life.
Several seabirds observed caught in debris.

Photos: [3 photos attached]

Observer: MV NUSANTARA (MMSI: 525012345)
Reporter: Captain Ahmad Rizki

Status: Reported
Shared with: Philippine Coast Guard, Ocean Cleanup Initiative
```

---

### 5. **🌦️ REAL-TIME WEATHER**

#### Konsep:

- Weather adalah critical factor untuk maritime operations
- Real-time data untuk decision making
- Integration dengan voyage planning

#### Fitur:

- ✅ **Current Weather** - Temperature, humidity, pressure
- ✅ **Wind Data** - Speed, direction, gusts
- ✅ **Sea Conditions** - Based on Beaufort scale
- ✅ **Visibility** - Critical untuk navigation
- ✅ **Weather Warnings** - Storms, high winds, etc.
- ✅ **5-Day Forecast** - Plan ahead untuk voyage
- ✅ **Safety Assessment** - Is it safe to sail?
- ✅ **Location-based** - Weather di posisi kapal saat ini

#### Weather API Integration:

```csharp
// WeatherService.cs
- OpenWeatherMap API integration
- Get weather by coordinates (ship position)
- 5-day forecast untuk route planning
- Maritime-specific features:
  * Sea condition calculation (Beaufort scale)
  * Sailing safety assessment
  * Warning levels
```

#### Beaufort Scale (Sea Conditions):

```
Wind Speed  | Sea Condition
------------|--------------------------------------------------
0-0.5 m/s   | Calm - Mirror-like sea
0.5-2 m/s   | Light Air - Ripples
2-3.5 m/s   | Light Breeze - Small wavelets
3.5-5.5 m/s | Gentle Breeze - Large wavelets
5.5-8 m/s   | Moderate Breeze - Small waves
8-11 m/s    | Fresh Breeze - Moderate waves (CAUTION)
11-14 m/s   | Strong Breeze - Large waves (WARNING)
14-17 m/s   | Near Gale - Sea heaps up (DANGEROUS)
17-21 m/s   | Gale - High waves (DO NOT SAIL)
21-24.5 m/s | Strong Gale - Very high waves (STORM)
>24.5 m/s   | Storm/Hurricane (EXTREME DANGER)
```

#### Safety Decision Logic:

```
SAFE FOR SAILING if:
✅ Wind speed < 15 m/s
✅ Visibility > 1 km
✅ No thunderstorm/squall
✅ No extreme temperature

WARNING LEVELS:
- LOW: Safe conditions
- MODERATE: Caution advised (rain, moderate wind)
- HIGH: Not recommended (strong wind, low visibility)
- CRITICAL: Do not sail (storm, extreme conditions)
```

#### Use Cases:

1. **Before Departure**

   - Check weather di departure point dan route
   - Check forecast untuk voyage duration
   - Decide: Go or delay?

2. **During Voyage**

   - Monitor weather changes real-time
   - Get warnings untuk storms ahead
   - Plan untuk shelter jika perlu

3. **Arrival Planning**

   - Check weather di destination port
   - Plan berthing based on wind/tide

4. **Maintenance Planning**
   - Deck maintenance needs good weather
   - Hull inspection needs calm sea

---

## 🔧 TEKNOLOGI & INTEGRASI

### **Tech Stack**

#### Frontend:

- **WPF** (Windows Presentation Foundation)
- **.NET 9.0** - Latest framework
- **XAML** - UI markup
- **C# 12** - Programming language

#### Backend/Services:

- **Supabase** - PostgreSQL database (cloud)
- **Npgsql** - PostgreSQL connector
- **BCrypt.Net** - Password hashing

#### External APIs:

- **AISStream.io** - Real-time ship tracking
  - WebSocket connection
  - Global AIS data coverage
  - Filter by MMSI, bounding box
- **OpenWeatherMap** - Weather data
  - Current weather API
  - 5-day forecast API
  - Maritime data (wind, sea state)

#### Libraries & Packages:

```xml
<PackageReference Include="Npgsql" Version="8.0.5" />
<PackageReference Include="BCrypt.Net-Next" Version="4.0.3" />
<PackageReference Include="System.Net.WebSockets.Client" />
<PackageReference Include="System.Text.Json" />
```

### **Architecture**

```
┌─────────────────────────────────────────────────────────┐
│                    MARINEX CLIENT                        │
│                   (WPF .NET 9.0)                        │
├─────────────────────────────────────────────────────────┤
│  VIEWS (XAML + Code-behind)                             │
│  - Dashboard                                             │
│  - My Ships                                              │
│  - Ship Tracking (with map)                              │
│  - Maintenance                                           │
│  - Safety Reports                                        │
│  - Pollution Reports                                     │
│  - Weather Dashboard                                     │
│  - Login/Register                                        │
├─────────────────────────────────────────────────────────┤
│  MODELS                                                  │
│  - Ship, Voyage, User, UserShip                         │
│  - BaseReport (abstract)                                 │
│    ├─ MaintenanceReport                                 │
│    ├─ SafetyReport                                      │
│    ├─ WeatherReport                                     │
│    └─ PollutionReport (NEW)                             │
│  - ShipPosition                                          │
│  - Weather, Maintenance                                  │
├─────────────────────────────────────────────────────────┤
│  SERVICES                                                │
│  - SupabaseService (database operations)                │
│  - AISStreamService (ship tracking)                     │
│  - WeatherService (weather data)                        │
│  - ReportService (all reports CRUD)                     │
│  - ShipDatabaseService                                   │
│  - VoyageDatabaseService                                 │
└─────────────────────────────────────────────────────────┘
                         │
                         │ HTTP/WebSocket
                         ▼
┌─────────────────────────────────────────────────────────┐
│              EXTERNAL SERVICES                           │
├─────────────────────────────────────────────────────────┤
│  SUPABASE (PostgreSQL)                                  │
│  - All data persistence                                  │
│  - Tables: ships, users, reports, positions, etc.       │
├─────────────────────────────────────────────────────────┤
│  AISSTREAM.IO                                            │
│  - WebSocket: wss://stream.aisstream.io/v0/stream      │
│  - Real-time AIS data                                    │
├─────────────────────────────────────────────────────────┤
│  OPENWEATHERMAP                                          │
│  - REST API: api.openweathermap.org/data/2.5           │
│  - Weather & forecast data                               │
└─────────────────────────────────────────────────────────┘
```

---

## 👥 USER FLOW

### **1. First Time User**

```
1. Launch Marinex application
2. See Landing/Dashboard page
3. Click "Sign Up"
4. Fill registration form:
   - Username
   - Email
   - Company name
   - Password (hashed dengan BCrypt)
5. Submit → Account created
6. Redirect to Login page
7. Login dengan credentials baru
8. Welcome to main dashboard
```

### **2. Company Admin - Setup Ships**

```
1. Login as admin
2. Navigate to "My Ships"
3. Click "Add New Ship"
4. Fill ship details:
   - Ship Name: "MV NUSANTARA"
   - Ship Type: "Cargo"
   - MMSI: "525012345" (untuk AIS tracking)
   - Capacity, Owner, etc.
   - Enable AIS Tracking: ✓
5. Save ship
6. System starts tracking via AIS
7. Ship appears on tracking map
```

### **3. Captain - Track Ship**

```
1. Login sebagai captain
2. Navigate to "Ship Tracking"
3. See map dengan semua ships
4. Click on own ship icon
5. See details:
   - Current position (Lat/Long)
   - Speed: 12.5 knots
   - Course: 085° (heading east)
   - Destination: Singapore Port
   - ETA: 2025-11-18 06:00 UTC
6. Check nearby ships
7. View historical track (breadcrumb trail)
```

### **4. Engineer - Create Maintenance**

```
1. Login sebagai engineer
2. Navigate to "Maintenance"
3. Click "New Maintenance"
4. Fill form:
   - Ship: MV NUSANTARA
   - Type: Engine Overhaul
   - Priority: High
   - Scheduled Date: 2025-11-20
   - Description: Main engine cylinder #3 needs overhaul
5. Create detailed maintenance report:
   - Equipment: Main Engine
   - Issue: Low compression in cylinder
   - Parts needed: Piston rings, gaskets, oil
   - Estimated cost: $5,000
   - Duration: 8 hours
6. Submit report
7. System notifies Chief Engineer
8. Schedule approved
```

### **5. Crew - Report Safety Incident**

```
1. Incident terjadi: Minor burn injury during maintenance
2. Crew immediately open Marinex
3. Navigate to "Safety Reports"
4. Click "New Report"
5. Fill incident form:
   - Location: Engine Room
   - Incident Type: Personal Injury
   - Severity: Medium
   - Description: [detail kejadian]
   - People involved: John Doe
   - Immediate action: First aid applied
   - Preventive measures: [rencana]
6. Upload photos (if any)
7. Submit report
8. System alert Management
9. Investigation started
```

### **6. Observer - Report Pollution**

```
1. Crew spot plastic debris di laut
2. Open Marinex
3. Navigate to "Pollution Reports"
4. Click "Report Pollution"
5. Fill form:
   - Location: Auto-fill dari ship position
   - Coordinates: Auto-detect (or manual input)
   - Waste Type: Plastic Debris
   - Quantity: Large
   - Severity: High
   - Description: [detail observation]
6. Take photos dengan phone
7. Upload photos ke report
8. Submit
9. Report saved dan bisa di-share ke authorities
```

### **7. Before Departure - Check Weather**

```
1. Captain planning voyage
2. Navigate to "Weather Dashboard"
3. Enter route coordinates (or select from ships)
4. View current weather:
   - Temperature: 28°C
   - Wind: 8 m/s (Fresh Breeze)
   - Sea Condition: "Moderate waves"
   - Visibility: Good (10 km)
   - Safety: ✓ SAFE FOR SAILING
5. View 5-day forecast
6. Check weather along route
7. Decision: Safe to depart
8. Create voyage dengan weather data recorded
```

---

## 📊 DATABASE SCHEMA

### **Core Tables (Existing)**

- `users` - User accounts
- `ships` - Ship registry
- `user_ships` - User-Ship relationship
- `voyages` - Voyage records
- `maintenance` - Maintenance schedules
- `weather` - Weather observations

### **New Tables (Enhanced)**

#### 1. **ship_positions_history**

```sql
- position_id (PK)
- ship_id (FK to ships)
- mmsi
- latitude, longitude
- speed, course, heading
- timestamp
- destination, eta
- navigational_status
```

Stores all AIS position updates untuk historical tracking

#### 2. **pollution_reports**

```sql
- report_id (PK)
- user_id, ship_id (FKs)
- location, latitude, longitude
- waste_type, quantity, severity
- description, environmental_impact
- status, action_taken
- photo_paths
- created_at, updated_at, resolved_at
```

All pollution/waste reports

#### 3. **weather_data**

```sql
- weather_id (PK)
- ship_id, voyage_id (FKs)
- location, latitude, longitude
- temperature, pressure, humidity
- wind_speed, wind_degree, wind_gust
- condition, description, clouds, visibility
- sea_condition, warning_level, safe_for_sailing
- observation_time, created_at
```

Historical weather data

#### 4. **maintenance_reports**

```sql
- report_id (PK)
- maintenance_id, user_id, ship_id (FKs)
- location
- equipment_name, issue_description, priority
- parts_needed, estimated_duration
- created_at, updated_at
```

Detailed maintenance reports

#### 5. **safety_reports**

```sql
- report_id (PK)
- user_id, ship_id, maintenance_id (FKs)
- location
- incident_type, severity, description
- people_involved, injuries
- immediate_action_taken, preventive_measures
- status
- incident_date, created_at, updated_at, resolved_at
```

Safety incident reports

#### 6. **weather_reports**

```sql
- report_id (PK)
- user_id, ship_id (FKs)
- location, reporter, category, severity
- description
- temperature, wind_speed, sea_condition
- created_at
```

User-submitted weather observations

#### 7. **voyage_updates**

```sql
- update_id (PK)
- voyage_id, user_id (FKs)
- latitude, longitude
- status, notes, weather_condition
- update_time
```

Progress updates during voyage

### **Relationships**

```
users ─┬─< user_ships >─ ships ─┬─< voyages
       │                        ├─< maintenance ─< maintenance_reports
       │                        ├─< ship_positions_history
       │                        ├─< pollution_reports
       │                        ├─< safety_reports
       │                        ├─< weather_reports
       │                        └─< weather_data
       │
       └─< (all reports author)
```

---

## 🚀 SETUP & INSTALLATION

### **Prerequisites**

1. **.NET SDK 9.0** - Download dari Microsoft
2. **Windows OS** - Untuk WPF
3. **Supabase Account** - Free tier available
4. **AISStream API Key** - Register di aisstream.io
5. **OpenWeatherMap API Key** - Free tier: 1000 calls/day

### **Step-by-Step**

#### 1. Clone Repository

```bash
git clone https://github.com/irsad-najib/Marinex.git
cd Marinex/Marinex
```

#### 2. Setup Supabase Database

```bash
# Login ke Supabase dashboard
# Create new project
# Go to SQL Editor
# Run SCHEMA_UPDATE.sql
```

#### 3. Get API Keys

**AISStream.io:**

```
1. Register di https://aisstream.io
2. Generate API key
3. Free tier: sufficient untuk development
```

**OpenWeatherMap:**

```
1. Register di https://openweathermap.org
2. Get API key
3. Free tier: 1000 calls/day (enough!)
```

#### 4. Configure Connections

Edit `Services/SupabaseService.cs`:

```csharp
private const string CONNECTION_STRING =
    "Host=xxx.supabase.co;" +
    "Port=5432;" +
    "Database=postgres;" +
    "Username=postgres;" +
    "Password=YOUR_PASSWORD";
```

Create `Config.cs`:

```csharp
public static class Config
{
    public const string AIS_API_KEY = "your-aisstream-api-key";
    public const string WEATHER_API_KEY = "your-openweather-api-key";
}
```

#### 5. Build & Run

```bash
dotnet restore
dotnet build
dotnet run
```

#### 6. First Login

```
Default admin account (create via SQL):
Email: admin@marinex.com
Password: admin123
```

---

## 📚 USAGE EXAMPLES

### **Code Example: Track Ship via AIS**

```csharp
// Initialize AIS service
var aisService = new AISStreamService(Config.AIS_API_KEY);

// Subscribe to ship position events
aisService.OnShipPositionReceived += (sender, position) => {
    Console.WriteLine($"Ship: {position.ShipName}");
    Console.WriteLine($"MMSI: {position.Mmsi}");
    Console.WriteLine($"Position: {position.Latitude}, {position.Longitude}");
    Console.WriteLine($"Speed: {position.Speed} knots");
    Console.WriteLine($"Course: {position.Course}°");

    // Save to database
    SavePositionToDatabase(position);

    // Update map UI
    UpdateMapMarker(position);
};

// Start streaming
await aisService.StartStreamAsync();
```

### **Code Example: Get Weather**

```csharp
// Initialize weather service
var weatherService = new WeatherService(Config.WEATHER_API_KEY);

// Get weather at ship's current position
double lat = -6.2088;  // Jakarta
double lon = 106.8456;
var weather = await weatherService.GetCurrentWeatherAsync(lat, lon);

Console.WriteLine($"Location: {weather.Location}");
Console.WriteLine($"Temperature: {weather.Temperature}°C");
Console.WriteLine($"Wind: {weather.WindSpeed} m/s");
Console.WriteLine($"Sea Condition: {weather.GetSeaCondition()}");
Console.WriteLine($"Safe for Sailing: {weather.IsSafeForSailing()}");
Console.WriteLine($"Warning Level: {weather.GetWarningLevel()}");
```

### **Code Example: Create Pollution Report**

```csharp
var pollutionReport = new PollutionReport
{
    UserID = currentUser.UserID,
    ShipID = currentShip.ShipID,
    Location = "Sulu Sea, Philippines",
    Latitude = 8.0,
    Longitude = 119.5,
    WasteType = "Plastic Debris",
    Quantity = "Large",
    Severity = "High",
    Description = "Large patch of plastic waste observed...",
    PhotoPaths = "photo1.jpg;photo2.jpg;photo3.jpg",
    Status = "Reported",
    CreatedAt = DateTime.Now
};

// Validate
if (pollutionReport.Validate())
{
    // Save to database
    await reportService.SavePollutionReportAsync(pollutionReport);

    // Generate formatted report
    string report = pollutionReport.GenerateReport();
    Console.WriteLine(report);

    // Check if needs immediate action
    if (pollutionReport.RequiresImmediateAction())
    {
        AlertAuthorities(pollutionReport);
    }
}
```

---

## 🎓 KONSEP OOP & DESIGN PATTERNS

### **1. Inheritance**

```csharp
// Base class
public abstract class BaseReport
{
    public int ReportID { get; set; }
    public string Location { get; set; }
    public DateTime CreatedAt { get; set; }

    public abstract string GenerateReport();
}

// Child classes
public class PollutionReport : BaseReport { }
public class SafetyReport : BaseReport { }
public class MaintenanceReport : BaseReport { }
public class WeatherReport : BaseReport { }
```

### **2. Polymorphism**

```csharp
List<BaseReport> allReports = new List<BaseReport>
{
    new PollutionReport(),
    new SafetyReport(),
    new MaintenanceReport()
};

foreach (var report in allReports)
{
    // Calls different implementation based on actual type
    string output = report.GenerateReport();
    Console.WriteLine(output);
}
```

### **3. Encapsulation**

```csharp
public class Ship
{
    // Private fields
    private double _latitude;
    private double _longitude;

    // Public properties with validation
    public double Latitude
    {
        get => _latitude;
        set
        {
            if (value < -90 || value > 90)
                throw new ArgumentException("Invalid latitude");
            _latitude = value;
        }
    }
}
```

### **4. Abstraction**

```csharp
public interface IDatabaseService
{
    Task<List<Ship>> GetAllShipsAsync();
    Task<Ship> GetShipByIdAsync(int id);
    Task<bool> SaveShipAsync(Ship ship);
}

// Implementation hidden from user
public class ShipDatabaseService : IDatabaseService
{
    // Actual database logic
}
```

---

## 📱 FUTURE ENHANCEMENTS

### **Phase 2 (Planned)**

- 🗺️ **Interactive Map** - OpenStreetMap/Mapbox integration
- 📱 **Mobile App** - Cross-platform dengan MAUI
- 🔔 **Push Notifications** - Real-time alerts
- 📊 **Analytics Dashboard** - Charts dan statistics
- 🤖 **AI Predictions** - Maintenance prediction, route optimization
- 🌐 **Multi-language** - English, Indonesian, etc.
- 👥 **Team Collaboration** - Chat, task assignment
- 📄 **PDF Export** - Professional report generation
- 🔐 **Role-based Access** - Captain, Engineer, Admin permissions
- ☁️ **Cloud Sync** - Multi-device synchronization

### **Phase 3 (Advanced)**

- 🛰️ **Satellite Integration** - Satellite AIS, weather satellite
- 🐟 **Fishing Zones** - Integration dengan fishing data
- ⚓ **Port Information** - Port schedules, berth availability
- 💰 **Cost Management** - Fuel, maintenance budget tracking
- 📈 **Predictive Analytics** - ML untuk predict failures
- 🌊 **Ocean Currents** - Route optimization dengan current data
- 🐋 **Marine Life Tracking** - Whale migration, protected areas
- 🏢 **Fleet Management** - Manage multiple ships centrally

---

## 👥 TEAM IRVINGO

### Development Team:

- **Irsad Najib Eka Putra** (23/518119/TK/57005)
  - Role: Team Lead, Fullstack Developer
  - Responsibilities: Architecture, Backend, Integration
- **Anggita Salsabilla** (23/516001/TK/56775)
  - Role: UI/UX Designer
  - Responsibilities: Interface design, User experience
- **Melvino Rizky Putra Wahyudi** (23/515981/TK/56770)
  - Role: Frontend Developer
  - Responsibilities: WPF Views, XAML, UI implementation
- **Abdul Halim Edi Rahmansyah** (23/516603/TK/56796)
  - Role: Data & AI Analyst
  - Responsibilities: Database design, Data analysis, AI features

---

## 📞 SUPPORT & CONTACT

### Issues & Bugs

GitHub Issues: https://github.com/irsad-najib/Marinex/issues

### Documentation

- API Docs: `/docs/API.md`
- Database Schema: `/SCHEMA_UPDATE.sql`
- Setup Guide: `/SUPABASE_SETUP.md`

### External Resources

- AISStream Documentation: https://aisstream.io/documentation
- OpenWeatherMap API: https://openweathermap.org/api
- Supabase Docs: https://supabase.com/docs

---

## 📄 LICENSE

© 2025 IRVINGO TEAM - All Rights Reserved

This project is created for educational purposes as part of university coursework.

---

## 🙏 ACKNOWLEDGMENTS

- **AISStream.io** - For providing free AIS data access
- **OpenWeatherMap** - For weather API services
- **Supabase** - For database hosting
- **Microsoft** - For .NET and WPF framework
- **Universitas Gadjah Mada** - For academic support

---

**Built with ❤️ by IRVINGO TEAM**

_Making maritime operations safer, smarter, and more sustainable._
