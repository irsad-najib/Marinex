-- =============================================
-- MARINEX DATABASE SCHEMA
-- Maritime Integrated Explorer - Complete Schema
-- =============================================
-- Base Schema + Enhancements untuk:
-- 1. Ship Tracking dengan AIS
-- 2. Pollution/Waste Reporting  
-- 3. Real-time Weather Data
-- 4. Enhanced Maintenance & Safety Reports
-- =============================================

-- =============================================
-- 1. CORE TABLES (Base Schema)
-- =============================================

-- User Table
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

-- Ship Table (Enhanced dengan AIS tracking)
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
    MMSI VARCHAR(9), -- Maritime Mobile Service Identity
    AISEnabled BOOLEAN DEFAULT FALSE,
    LastPositionUpdate TIMESTAMP,
    CurrentLatitude DOUBLE PRECISION,
    CurrentLongitude DOUBLE PRECISION,
    CurrentSpeed DOUBLE PRECISION,
    CurrentCourse DOUBLE PRECISION,
    CurrentHeading DOUBLE PRECISION
);

-- Voyage Table
CREATE TABLE Voyage (
    VoyageID SERIAL PRIMARY KEY,
    "From" VARCHAR(100),
    Destination VARCHAR(100),
    EstimatedDuration INTERVAL,
    ShipID INT REFERENCES Ship(ShipID) ON DELETE CASCADE,
    UserID INT REFERENCES "User"(UserID) ON DELETE SET NULL
);

-- Weather Table
CREATE TABLE Weather (
    WeatherID SERIAL PRIMARY KEY,
    Location VARCHAR(100),
    Temperature DECIMAL(5,2),
    Wind VARCHAR(50),
    SeaCondition VARCHAR(100),
    VoyageID INT REFERENCES Voyage(VoyageID) ON DELETE CASCADE
);

-- WasteReport Table (Base - untuk user-submitted reports)
CREATE TABLE WasteReport (
    ReportID SERIAL PRIMARY KEY,
    Reporter VARCHAR(100),
    Location VARCHAR(100),
    Category VARCHAR(50),
    Severity VARCHAR(50),
    Description TEXT,
    UserID INT REFERENCES "User"(UserID) ON DELETE SET NULL
);

-- Maintenance Table
CREATE TABLE Maintenance (
    MaintenanceID SERIAL PRIMARY KEY,
    Date DATE,
    Type VARCHAR(100),
    Status VARCHAR(50),
    UserID INT REFERENCES "User"(UserID) ON DELETE SET NULL,
    ShipID INT REFERENCES Ship(ShipID) ON DELETE CASCADE
);

-- UserShip Table (Many-to-Many relationship)
CREATE TABLE UserShip (
    UserShipID SERIAL PRIMARY KEY,
    UserID INT REFERENCES "User"(UserID) ON DELETE CASCADE,
    ShipID INT REFERENCES Ship(ShipID) ON DELETE CASCADE,
    JoinDate DATE,
    Status VARCHAR(50)
);

-- =============================================
-- 2. ENHANCEMENT TABLES
-- =============================================

-- Ship Positions History (untuk AIS tracking)
CREATE TABLE ShipPositionHistory (
    PositionID SERIAL PRIMARY KEY,
    ShipID INT NOT NULL REFERENCES Ship(ShipID) ON DELETE CASCADE,
    MMSI VARCHAR(9),
    Latitude DOUBLE PRECISION NOT NULL,
    Longitude DOUBLE PRECISION NOT NULL,
    Speed DOUBLE PRECISION, -- Speed over ground (knots)
    Course DOUBLE PRECISION, -- Course over ground (degrees)
    Heading DOUBLE PRECISION, -- True heading (degrees)
    Timestamp TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    Destination VARCHAR(200),
    ETA TIMESTAMP, -- Estimated Time of Arrival
    NavigationalStatus VARCHAR(50) -- Under way, At anchor, Moored, etc.
);

-- Pollution Reports (Enhanced dari WasteReport)
CREATE TABLE PollutionReport (
    ReportID SERIAL PRIMARY KEY,
    UserID INT NOT NULL REFERENCES "User"(UserID) ON DELETE CASCADE,
    ShipID INT REFERENCES Ship(ShipID) ON DELETE SET NULL, -- Optional: kapal yang observe
    
    -- Location data
    Location VARCHAR(200) NOT NULL,
    Latitude DOUBLE PRECISION NOT NULL,
    Longitude DOUBLE PRECISION NOT NULL,
    
    -- Pollution details
    WasteType VARCHAR(100) NOT NULL, -- Plastic, Oil Spill, Chemical, Marine Debris, etc.
    Quantity VARCHAR(50), -- Small, Medium, Large, Massive
    Severity VARCHAR(20) NOT NULL, -- Low, Medium, High, Critical
    Description TEXT NOT NULL,
    EnvironmentalImpact TEXT,
    
    -- Status tracking
    Status VARCHAR(50) DEFAULT 'Reported', -- Reported, Under Investigation, Cleanup Initiated, Resolved
    ActionTaken TEXT,
    
    -- Photos/Evidence
    PhotoPaths TEXT, -- Multiple paths separated by semicolon
    
    -- Timestamps
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    ResolvedAt TIMESTAMP
);

-- Weather Data (Historical weather records)
CREATE TABLE WeatherData (
    WeatherDataID SERIAL PRIMARY KEY,
    ShipID INT REFERENCES Ship(ShipID) ON DELETE CASCADE,
    VoyageID INT REFERENCES Voyage(VoyageID) ON DELETE CASCADE,
    
    -- Location
    Location VARCHAR(200) NOT NULL,
    Latitude DOUBLE PRECISION NOT NULL,
    Longitude DOUBLE PRECISION NOT NULL,
    
    -- Weather conditions
    Temperature DOUBLE PRECISION, -- Celsius
    FeelsLike DOUBLE PRECISION,
    TempMin DOUBLE PRECISION,
    TempMax DOUBLE PRECISION,
    Pressure INT, -- hPa
    Humidity INT, -- %
    
    -- Wind
    WindSpeed DOUBLE PRECISION, -- m/s
    WindDegree INT, -- degrees
    WindGust DOUBLE PRECISION,
    
    -- General conditions
    Condition VARCHAR(50), -- Clear, Clouds, Rain, Storm, etc.
    ConditionDescription TEXT,
    Clouds INT, -- %
    Visibility INT, -- meters
    
    -- Sea state
    SeaCondition VARCHAR(100),
    WarningLevel VARCHAR(50),
    SafeForSailing BOOLEAN,
    
    -- Timestamps
    ObservationTime TIMESTAMP NOT NULL,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Maintenance Reports (Detailed maintenance logs)
CREATE TABLE MaintenanceReport (
    ReportID SERIAL PRIMARY KEY,
    MaintenanceID INT NOT NULL REFERENCES Maintenance(MaintenanceID) ON DELETE CASCADE,
    UserID INT NOT NULL REFERENCES "User"(UserID) ON DELETE CASCADE,
    ShipID INT NOT NULL REFERENCES Ship(ShipID) ON DELETE CASCADE,
    
    -- Location
    Location VARCHAR(200) NOT NULL,
    
    -- Maintenance details
    EquipmentName VARCHAR(200) NOT NULL,
    IssueDescription TEXT NOT NULL,
    Priority VARCHAR(20) NOT NULL, -- Low, Medium, High, Critical, Urgent
    
    -- Parts & costs
    PartsNeeded TEXT,
    EstimatedDuration VARCHAR(50),
    EstimatedCost DECIMAL(10,2),
    ActualCost DECIMAL(10,2),
    
    -- Timestamps
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Safety Reports (Incident reporting)
CREATE TABLE SafetyReport (
    ReportID SERIAL PRIMARY KEY,
    UserID INT NOT NULL REFERENCES "User"(UserID) ON DELETE CASCADE,
    ShipID INT REFERENCES Ship(ShipID) ON DELETE SET NULL,
    MaintenanceID INT REFERENCES Maintenance(MaintenanceID) ON DELETE SET NULL,
    
    -- Location
    Location VARCHAR(200) NOT NULL,
    
    -- Incident details
    IncidentType VARCHAR(100) NOT NULL, -- Injury, Near Miss, Equipment Failure, etc.
    Severity VARCHAR(20) NOT NULL, -- Low, Medium, High, Critical
    Description TEXT NOT NULL,
    
    -- People involved
    PeopleInvolved TEXT,
    Injuries TEXT,
    
    -- Response
    ImmediateActionTaken TEXT,
    PreventiveMeasures TEXT,
    
    -- Status
    Status VARCHAR(50) DEFAULT 'Reported', -- Reported, Under Investigation, Resolved
    
    -- Timestamps
    IncidentDate TIMESTAMP NOT NULL,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    ResolvedAt TIMESTAMP
);

-- Voyage Updates (Progress tracking during voyage)
CREATE TABLE VoyageUpdate (
    UpdateID SERIAL PRIMARY KEY,
    VoyageID INT NOT NULL REFERENCES Voyage(VoyageID) ON DELETE CASCADE,
    UserID INT NOT NULL REFERENCES "User"(UserID) ON DELETE CASCADE,
    
    -- Position
    Latitude DOUBLE PRECISION,
    Longitude DOUBLE PRECISION,
    
    -- Status update
    Status VARCHAR(50), -- Departed, En Route, Arrived, Delayed, etc.
    Notes TEXT,
    
    -- Weather at time of update
    WeatherCondition VARCHAR(100),
    
    -- Timestamp
    UpdateTime TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- =============================================
-- 3. INDEXES FOR PERFORMANCE
-- =============================================

-- User indexes
CREATE INDEX idx_user_username ON "User"(UserName);
CREATE INDEX idx_user_company ON "User"(Company);

-- Ship indexes
CREATE INDEX idx_ship_name ON Ship(ShipName);
CREATE INDEX idx_ship_mmsi ON Ship(MMSI);
CREATE INDEX idx_ship_ais_enabled ON Ship(AISEnabled) WHERE AISEnabled = TRUE;
CREATE INDEX idx_ship_status ON Ship(Status);

-- ShipPositionHistory indexes
CREATE INDEX idx_position_ship_id ON ShipPositionHistory(ShipID);
CREATE INDEX idx_position_timestamp ON ShipPositionHistory(Timestamp);
CREATE INDEX idx_position_mmsi ON ShipPositionHistory(MMSI);
CREATE INDEX idx_position_location ON ShipPositionHistory(Latitude, Longitude);

-- PollutionReport indexes
CREATE INDEX idx_pollution_user_id ON PollutionReport(UserID);
CREATE INDEX idx_pollution_ship_id ON PollutionReport(ShipID);
CREATE INDEX idx_pollution_status ON PollutionReport(Status);
CREATE INDEX idx_pollution_severity ON PollutionReport(Severity);
CREATE INDEX idx_pollution_created ON PollutionReport(CreatedAt);
CREATE INDEX idx_pollution_location ON PollutionReport(Latitude, Longitude);

-- WeatherData indexes
CREATE INDEX idx_weather_ship_id ON WeatherData(ShipID);
CREATE INDEX idx_weather_voyage_id ON WeatherData(VoyageID);
CREATE INDEX idx_weather_observation ON WeatherData(ObservationTime);
CREATE INDEX idx_weather_location ON WeatherData(Latitude, Longitude);

-- MaintenanceReport indexes
CREATE INDEX idx_maintenance_report_maintenance_id ON MaintenanceReport(MaintenanceID);
CREATE INDEX idx_maintenance_report_user_id ON MaintenanceReport(UserID);
CREATE INDEX idx_maintenance_report_ship_id ON MaintenanceReport(ShipID);
CREATE INDEX idx_maintenance_report_priority ON MaintenanceReport(Priority);

-- SafetyReport indexes
CREATE INDEX idx_safety_user_id ON SafetyReport(UserID);
CREATE INDEX idx_safety_ship_id ON SafetyReport(ShipID);
CREATE INDEX idx_safety_severity ON SafetyReport(Severity);
CREATE INDEX idx_safety_status ON SafetyReport(Status);
CREATE INDEX idx_safety_incident_date ON SafetyReport(IncidentDate);

-- Voyage indexes
CREATE INDEX idx_voyage_ship_id ON Voyage(ShipID);
CREATE INDEX idx_voyage_user_id ON Voyage(UserID);

-- VoyageUpdate indexes
CREATE INDEX idx_voyage_update_voyage_id ON VoyageUpdate(VoyageID);
CREATE INDEX idx_voyage_update_time ON VoyageUpdate(UpdateTime);

-- Maintenance indexes
CREATE INDEX idx_maintenance_ship_id ON Maintenance(ShipID);
CREATE INDEX idx_maintenance_user_id ON Maintenance(UserID);
CREATE INDEX idx_maintenance_status ON Maintenance(Status);
CREATE INDEX idx_maintenance_date ON Maintenance(Date);

-- UserShip indexes
CREATE INDEX idx_usership_user_id ON UserShip(UserID);
CREATE INDEX idx_usership_ship_id ON UserShip(ShipID);

-- =============================================
-- 4. TRIGGERS FOR UPDATED_AT
-- =============================================

-- Trigger function untuk update timestamp
CREATE OR REPLACE FUNCTION update_updated_at_column()
RETURNS TRIGGER AS $$
BEGIN
    NEW.UpdatedAt = CURRENT_TIMESTAMP;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- Apply trigger ke PollutionReport
CREATE TRIGGER pollution_report_updated_at
    BEFORE UPDATE ON PollutionReport
    FOR EACH ROW
    EXECUTE FUNCTION update_updated_at_column();

-- Apply trigger ke MaintenanceReport
CREATE TRIGGER maintenance_report_updated_at
    BEFORE UPDATE ON MaintenanceReport
    FOR EACH ROW
    EXECUTE FUNCTION update_updated_at_column();

-- Apply trigger ke SafetyReport
CREATE TRIGGER safety_report_updated_at
    BEFORE UPDATE ON SafetyReport
    FOR EACH ROW
    EXECUTE FUNCTION update_updated_at_column();

-- =============================================
-- 5. USEFUL VIEWS
-- =============================================

-- View untuk active pollution reports
CREATE OR REPLACE VIEW ActivePollutionReports AS
SELECT 
    pr.*,
    u.UserName as ReporterName,
    s.ShipName,
    CASE 
        WHEN pr.CreatedAt < CURRENT_TIMESTAMP - INTERVAL '7 days' THEN 'Overdue'
        WHEN pr.Severity IN ('Critical', 'High') THEN 'Urgent'
        ELSE 'Normal'
    END as ActionPriority
FROM PollutionReport pr
LEFT JOIN "User" u ON pr.UserID = u.UserID
LEFT JOIN Ship s ON pr.ShipID = s.ShipID
WHERE pr.Status != 'Resolved'
ORDER BY pr.Severity DESC, pr.CreatedAt DESC;

-- View untuk ship tracking summary
CREATE OR REPLACE VIEW ShipTrackingSummary AS
SELECT 
    s.ShipID,
    s.ShipName,
    s.MMSI,
    s.CurrentLatitude,
    s.CurrentLongitude,
    s.CurrentSpeed,
    s.CurrentCourse,
    s.LastPositionUpdate,
    COUNT(sph.PositionID) as TotalPositionsTracked,
    MAX(sph.Timestamp) as LastTrackedPosition
FROM Ship s
LEFT JOIN ShipPositionHistory sph ON s.ShipID = sph.ShipID
WHERE s.AISEnabled = TRUE
GROUP BY s.ShipID;

-- View untuk maintenance overview
CREATE OR REPLACE VIEW MaintenanceOverview AS
SELECT 
    m.*,
    s.ShipName,
    u.UserName as AssignedTo,
    mr.Priority as ReportPriority,
    mr.EquipmentName,
    mr.IssueDescription,
    mr.EstimatedCost,
    mr.ActualCost
FROM Maintenance m
LEFT JOIN Ship s ON m.ShipID = s.ShipID
LEFT JOIN "User" u ON m.UserID = u.UserID
LEFT JOIN MaintenanceReport mr ON m.MaintenanceID = mr.MaintenanceID
ORDER BY m.Date DESC;

-- View untuk safety incidents summary
CREATE OR REPLACE VIEW SafetyIncidentsSummary AS
SELECT 
    sr.*,
    u.UserName as ReporterName,
    s.ShipName,
    CASE 
        WHEN sr.Status = 'Resolved' THEN 'Closed'
        WHEN sr.Severity IN ('Critical', 'High') THEN 'Urgent'
        ELSE 'Open'
    END as Priority
FROM SafetyReport sr
LEFT JOIN "User" u ON sr.UserID = u.UserID
LEFT JOIN Ship s ON sr.ShipID = s.ShipID
ORDER BY sr.IncidentDate DESC;

-- =============================================
-- 6. SAMPLE DATA
-- =============================================

-- Insert sample users
INSERT INTO "User" (UserName, Password, Role, Company, SubmitReport) VALUES
('admin', '$2a$11$YourHashedPasswordHere', 'Admin', 'Marinex Corp', TRUE),
('captain_ahmad', '$2a$11$YourHashedPasswordHere', 'Captain', 'Marinex Corp', TRUE),
('engineer_budi', '$2a$11$YourHashedPasswordHere', 'Engineer', 'Marinex Corp', TRUE);

-- Insert sample ships
INSERT INTO Ship (ShipName, ShipType, Owner, Capacity, Status, MMSI, AISEnabled) VALUES
('MV NUSANTARA', 'Cargo', 'Marinex Corp', 5000, 'Active', '525012345', TRUE),
('MV INDONESIA JAYA', 'Container', 'Marinex Corp', 8000, 'Active', '525012346', TRUE),
('MV SAMUDRA', 'Tanker', 'Marinex Corp', 10000, 'Maintenance', '525012347', FALSE);

-- Insert sample UserShip relationships
INSERT INTO UserShip (UserID, ShipID, JoinDate, Status) VALUES
(2, 1, '2025-01-01', 'Active'), -- Captain Ahmad on MV NUSANTARA
(3, 1, '2025-01-01', 'Active'), -- Engineer Budi on MV NUSANTARA
(2, 2, '2025-01-15', 'Active'); -- Captain Ahmad on MV INDONESIA JAYA

-- Insert sample voyage
INSERT INTO Voyage ("From", Destination, EstimatedDuration, ShipID, UserID) VALUES
('Jakarta', 'Singapore', '2 days', 1, 2);

-- Insert sample pollution report
INSERT INTO PollutionReport (
    UserID, ShipID, Location, Latitude, Longitude,
    WasteType, Quantity, Severity, Description, Status
) VALUES (
    2, 1, 'Java Sea', -6.2088, 106.8456,
    'Plastic Debris', 'Large', 'High',
    'Large patch of plastic waste observed during voyage. Estimated size: 50-100 square meters.',
    'Reported'
);

-- Insert sample maintenance
INSERT INTO Maintenance (Date, Type, Status, UserID, ShipID) VALUES
('2025-11-20', 'Engine Overhaul', 'Scheduled', 3, 1);

-- Insert sample maintenance report
INSERT INTO MaintenanceReport (
    MaintenanceID, UserID, ShipID, Location,
    EquipmentName, IssueDescription, Priority, EstimatedCost
) VALUES (
    1, 3, 1, 'Engine Room',
    'Main Engine Cylinder #3', 'Low compression, needs overhaul', 'High', 5000.00
);

-- =============================================
-- 7. COMPOSITE INDEXES FOR COMMON QUERIES
-- =============================================

-- Ships dengan AIS tracking yang aktif
CREATE INDEX idx_ships_ais_active ON Ship(AISEnabled, LastPositionUpdate) 
WHERE AISEnabled = TRUE;

-- Active pollution reports by severity
CREATE INDEX idx_pollution_active_severity ON PollutionReport(Status, Severity, CreatedAt) 
WHERE Status != 'Resolved';

-- Active maintenance by status
CREATE INDEX idx_maintenance_active ON Maintenance(Status, Date) 
WHERE Status != 'Done';

-- Safety reports yang belum resolved
CREATE INDEX idx_safety_open ON SafetyReport(Status, Severity, IncidentDate)
WHERE Status != 'Resolved';

-- =============================================
-- END OF SCHEMA
-- =============================================

-- Notes:
-- 1. Semua password harus di-hash dengan BCrypt sebelum insert
-- 2. MMSI harus 9 digit angka (Maritime Mobile Service Identity)
-- 3. Coordinates: Latitude (-90 to 90), Longitude (-180 to 180)
-- 4. Timestamps menggunakan UTC
-- 5. PhotoPaths di PollutionReport dipisahkan dengan semicolon (;)
