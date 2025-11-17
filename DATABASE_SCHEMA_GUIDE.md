# 📊 DATABASE SCHEMA - QUICK REFERENCE

## Sesuai dengan Requirement Original + Enhancements

---

## ✅ BASE TABLES (dari requirement)

### 1. **User Table**

```sql
CREATE TABLE "User" (
    UserID SERIAL PRIMARY KEY,
    UserName VARCHAR(100) NOT NULL,
    Password VARCHAR(255) NOT NULL,
    Role VARCHAR(50),
    Company VARCHAR(100),
    LogIn TIMESTAMP,
    LogOut TIMESTAMP,
    SubmitReport BOOLEAN
);
```

**Gunanya:** Menyimpan user accounts (Captain, Engineer, Admin, Crew)

---

### 2. **Ship Table** (Enhanced)

```sql
CREATE TABLE Ship (
    ShipID SERIAL PRIMARY KEY,
    ShipName VARCHAR(100) NOT NULL,
    ShipType VARCHAR(50),
    Owner VARCHAR(100),
    Capacity INT,
    Status VARCHAR(50),
    StartVoyage DATE,
    EndVoyage DATE,
    -- AIS Enhancement
    MMSI VARCHAR(9),
    AISEnabled BOOLEAN DEFAULT FALSE,
    LastPositionUpdate TIMESTAMP,
    CurrentLatitude DOUBLE PRECISION,
    CurrentLongitude DOUBLE PRECISION,
    CurrentSpeed DOUBLE PRECISION,
    CurrentCourse DOUBLE PRECISION,
    CurrentHeading DOUBLE PRECISION
);
```

**Gunanya:** Registry kapal + real-time position tracking

**Enhancement:** Tambah AIS fields untuk real-time tracking

---

### 3. **Voyage Table**

```sql
CREATE TABLE Voyage (
    VoyageID SERIAL PRIMARY KEY,
    "From" VARCHAR(100),
    Destination VARCHAR(100),
    EstimatedDuration INTERVAL,
    ShipID INT REFERENCES Ship(ShipID) ON DELETE CASCADE,
    UserID INT REFERENCES "User"(UserID) ON DELETE SET NULL
);
```

**Gunanya:** Track perjalanan kapal

---

### 4. **Weather Table**

```sql
CREATE TABLE Weather (
    WeatherID SERIAL PRIMARY KEY,
    Location VARCHAR(100),
    Temperature DECIMAL(5,2),
    Wind VARCHAR(50),
    SeaCondition VARCHAR(100),
    VoyageID INT REFERENCES Voyage(VoyageID) ON DELETE CASCADE
);
```

**Gunanya:** Weather observation terkait voyage

---

### 5. **WasteReport Table**

```sql
CREATE TABLE WasteReport (
    ReportID SERIAL PRIMARY KEY,
    Reporter VARCHAR(100),
    Location VARCHAR(100),
    Category VARCHAR(50),
    Severity VARCHAR(50),
    Description TEXT,
    UserID INT REFERENCES "User"(UserID) ON DELETE SET NULL
);
```

**Gunanya:** User-submitted waste reports (simple)

---

### 6. **Maintenance Table**

```sql
CREATE TABLE Maintenance (
    MaintenanceID SERIAL PRIMARY KEY,
    Date DATE,
    Type VARCHAR(100),
    Status VARCHAR(50),
    UserID INT REFERENCES "User"(UserID) ON DELETE SET NULL,
    ShipID INT REFERENCES Ship(ShipID) ON DELETE CASCADE
);
```

**Gunanya:** Maintenance schedules

---

### 7. **UserShip Table**

```sql
CREATE TABLE UserShip (
    UserShipID SERIAL PRIMARY KEY,
    UserID INT REFERENCES "User"(UserID) ON DELETE CASCADE,
    ShipID INT REFERENCES Ship(ShipID) ON DELETE CASCADE,
    JoinDate DATE,
    Status VARCHAR(50)
);
```

**Gunanya:** Many-to-many relationship User ↔ Ship

---

## 🚀 ENHANCEMENT TABLES (tambahan untuk fitur baru)

### 8. **ShipPositionHistory**

```sql
CREATE TABLE ShipPositionHistory (
    PositionID SERIAL PRIMARY KEY,
    ShipID INT REFERENCES Ship(ShipID),
    MMSI VARCHAR(9),
    Latitude DOUBLE PRECISION NOT NULL,
    Longitude DOUBLE PRECISION NOT NULL,
    Speed DOUBLE PRECISION,
    Course DOUBLE PRECISION,
    Heading DOUBLE PRECISION,
    Timestamp TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    Destination VARCHAR(200),
    ETA TIMESTAMP,
    NavigationalStatus VARCHAR(50)
);
```

**Gunanya:** Historical AIS tracking data (breadcrumb trail)

---

### 9. **PollutionReport** (Enhanced dari WasteReport)

```sql
CREATE TABLE PollutionReport (
    ReportID SERIAL PRIMARY KEY,
    UserID INT REFERENCES "User"(UserID),
    ShipID INT REFERENCES Ship(ShipID),
    Location VARCHAR(200) NOT NULL,
    Latitude DOUBLE PRECISION NOT NULL,
    Longitude DOUBLE PRECISION NOT NULL,
    WasteType VARCHAR(100) NOT NULL,
    Quantity VARCHAR(50),
    Severity VARCHAR(20) NOT NULL,
    Description TEXT NOT NULL,
    EnvironmentalImpact TEXT,
    Status VARCHAR(50) DEFAULT 'Reported',
    ActionTaken TEXT,
    PhotoPaths TEXT,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP,
    ResolvedAt TIMESTAMP
);
```

**Gunanya:** Advanced pollution reporting dengan GPS, photos, tracking

---

### 10. **WeatherData** (Historical)

```sql
CREATE TABLE WeatherData (
    WeatherDataID SERIAL PRIMARY KEY,
    ShipID INT REFERENCES Ship(ShipID),
    VoyageID INT REFERENCES Voyage(VoyageID),
    Location VARCHAR(200),
    Latitude DOUBLE PRECISION,
    Longitude DOUBLE PRECISION,
    Temperature DOUBLE PRECISION,
    FeelsLike DOUBLE PRECISION,
    Pressure INT,
    Humidity INT,
    WindSpeed DOUBLE PRECISION,
    WindDegree INT,
    Condition VARCHAR(50),
    SeaCondition VARCHAR(100),
    SafeForSailing BOOLEAN,
    ObservationTime TIMESTAMP,
    CreatedAt TIMESTAMP
);
```

**Gunanya:** Store historical weather data dari API

---

### 11. **MaintenanceReport** (Detailed)

```sql
CREATE TABLE MaintenanceReport (
    ReportID SERIAL PRIMARY KEY,
    MaintenanceID INT REFERENCES Maintenance(MaintenanceID),
    UserID INT REFERENCES "User"(UserID),
    ShipID INT REFERENCES Ship(ShipID),
    Location VARCHAR(200),
    EquipmentName VARCHAR(200),
    IssueDescription TEXT,
    Priority VARCHAR(20),
    PartsNeeded TEXT,
    EstimatedCost DECIMAL(10,2),
    ActualCost DECIMAL(10,2),
    CreatedAt TIMESTAMP,
    UpdatedAt TIMESTAMP
);
```

**Gunanya:** Detailed maintenance logs (equipment, costs, parts)

---

### 12. **SafetyReport**

```sql
CREATE TABLE SafetyReport (
    ReportID SERIAL PRIMARY KEY,
    UserID INT REFERENCES "User"(UserID),
    ShipID INT REFERENCES Ship(ShipID),
    Location VARCHAR(200),
    IncidentType VARCHAR(100),
    Severity VARCHAR(20),
    Description TEXT,
    PeopleInvolved TEXT,
    ImmediateActionTaken TEXT,
    PreventiveMeasures TEXT,
    Status VARCHAR(50) DEFAULT 'Reported',
    IncidentDate TIMESTAMP,
    CreatedAt TIMESTAMP,
    ResolvedAt TIMESTAMP
);
```

**Gunanya:** Safety incident reporting & investigation

---

### 13. **VoyageUpdate**

```sql
CREATE TABLE VoyageUpdate (
    UpdateID SERIAL PRIMARY KEY,
    VoyageID INT REFERENCES Voyage(VoyageID),
    UserID INT REFERENCES "User"(UserID),
    Latitude DOUBLE PRECISION,
    Longitude DOUBLE PRECISION,
    Status VARCHAR(50),
    Notes TEXT,
    WeatherCondition VARCHAR(100),
    UpdateTime TIMESTAMP
);
```

**Gunanya:** Progress updates during voyage

---

## 🔍 VIEWS (Query Helpers)

### ActivePollutionReports

```sql
SELECT pr.*, u.UserName, s.ShipName,
       CASE WHEN CreatedAt < NOW() - INTERVAL '7 days' THEN 'Overdue'
            WHEN Severity IN ('Critical','High') THEN 'Urgent'
            ELSE 'Normal' END as ActionPriority
FROM PollutionReport pr
WHERE Status != 'Resolved';
```

### ShipTrackingSummary

```sql
SELECT s.ShipID, s.ShipName, s.MMSI,
       s.CurrentLatitude, s.CurrentLongitude,
       COUNT(sph.PositionID) as TotalPositions
FROM Ship s
LEFT JOIN ShipPositionHistory sph ON s.ShipID = sph.ShipID
WHERE s.AISEnabled = TRUE
GROUP BY s.ShipID;
```

### MaintenanceOverview

```sql
SELECT m.*, s.ShipName, u.UserName,
       mr.Priority, mr.EquipmentName, mr.EstimatedCost
FROM Maintenance m
LEFT JOIN Ship s ON m.ShipID = s.ShipID
LEFT JOIN MaintenanceReport mr ON m.MaintenanceID = mr.MaintenanceID;
```

---

## 📈 RELATIONSHIPS

```
User ─┬─< UserShip >─ Ship ─┬─< Voyage
      │                     ├─< Maintenance ─< MaintenanceReport
      │                     ├─< ShipPositionHistory
      │                     └─< WeatherData
      │
      └─< PollutionReport
      └─< SafetyReport
      └─< WasteReport
```

---

## 🔑 KEY POINTS

### Primary Keys

- Semua table pakai `SERIAL PRIMARY KEY` (auto-increment)

### Foreign Keys

- **CASCADE**: Kalau parent dihapus, child ikut dihapus
- **SET NULL**: Kalau parent dihapus, foreign key jadi NULL

### Timestamps

- `CURRENT_TIMESTAMP` untuk auto-fill
- `UpdatedAt` auto-update via trigger

### Indexes

- 30+ indexes untuk fast queries
- Composite indexes untuk common filters

---

## 💡 USAGE TIPS

### Insert User

```sql
INSERT INTO "User" (UserName, Password, Role, Company, SubmitReport)
VALUES ('captain_ahmad', '$2a$11$hashed...', 'Captain', 'Marinex Corp', TRUE);
```

### Insert Ship dengan AIS

```sql
INSERT INTO Ship (ShipName, ShipType, Owner, MMSI, AISEnabled)
VALUES ('MV NUSANTARA', 'Cargo', 'Marinex Corp', '525012345', TRUE);
```

### Query Ship Positions

```sql
SELECT * FROM ShipPositionHistory
WHERE ShipID = 1
ORDER BY Timestamp DESC
LIMIT 100;
```

### Query Active Pollution Reports

```sql
SELECT * FROM ActivePollutionReports
WHERE Severity IN ('High', 'Critical')
ORDER BY CreatedAt DESC;
```

---

## 🎯 COMPLIANCE

✅ Sesuai 100% dengan requirement original
✅ Enhanced dengan fitur-fitur tambahan
✅ Normalized (3NF)
✅ Proper indexes
✅ Foreign key constraints
✅ Triggers untuk automation

---

**File lokasi:** `/Marinex/Marinex/DATABASE_SCHEMA.sql`

**Cara pakai:**

1. Login ke Supabase Dashboard
2. Go to SQL Editor
3. Copy-paste isi file
4. Execute
5. Done! ✅
