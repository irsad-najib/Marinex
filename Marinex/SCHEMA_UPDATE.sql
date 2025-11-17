-- =============================================
-- MARINEX DATABASE SCHEMA UPDATE
-- Maritime Integrated Explorer - Complete Schema
-- =============================================
-- Update untuk support:
-- 1. Ship Tracking dengan AIS
-- 2. Pollution/Waste Reporting
-- 3. Real-time Weather Data
-- 4. Enhanced Maintenance & Safety Reports
-- =============================================

-- =============================================
-- 1. UPDATE EXISTING TABLES
-- =============================================

-- Add MMSI untuk AIS tracking ke ships table
ALTER TABLE ships 
ADD COLUMN mmsi VARCHAR(9), -- Maritime Mobile Service Identity (9 digits)
ADD COLUMN ais_enabled BOOLEAN DEFAULT FALSE,
ADD COLUMN last_position_update TIMESTAMP,
ADD COLUMN current_latitude DOUBLE PRECISION,
ADD COLUMN current_longitude DOUBLE PRECISION,
ADD COLUMN current_speed DOUBLE PRECISION,
ADD COLUMN current_course DOUBLE PRECISION,
ADD COLUMN current_heading DOUBLE PRECISION;

-- Update maintenance table untuk better tracking
ALTER TABLE maintenance
ADD COLUMN description TEXT,
ADD COLUMN priority VARCHAR(20) DEFAULT 'Medium', -- Low, Medium, High, Critical
ADD COLUMN estimated_cost DECIMAL(10,2),
ADD COLUMN actual_cost DECIMAL(10,2),
ADD COLUMN scheduled_date TIMESTAMP,
ADD COLUMN completed_date TIMESTAMP;

-- =============================================
-- 2. SHIP POSITIONS HISTORY (untuk AIS tracking)
-- =============================================
CREATE TABLE ship_positions_history (
    position_id SERIAL PRIMARY KEY,
    ship_id INTEGER NOT NULL REFERENCES ships(ship_id) ON DELETE CASCADE,
    mmsi VARCHAR(9),
    latitude DOUBLE PRECISION NOT NULL,
    longitude DOUBLE PRECISION NOT NULL,
    speed DOUBLE PRECISION, -- Speed over ground (knots)
    course DOUBLE PRECISION, -- Course over ground (degrees)
    heading DOUBLE PRECISION, -- True heading (degrees)
    timestamp TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    destination VARCHAR(200),
    eta TIMESTAMP, -- Estimated Time of Arrival
    navigational_status VARCHAR(50), -- Under way, At anchor, Moored, etc.
    
    -- Indexes untuk query performance
    INDEX idx_ship_positions_ship_id (ship_id),
    INDEX idx_ship_positions_timestamp (timestamp),
    INDEX idx_ship_positions_mmsi (mmsi)
);

-- =============================================
-- 3. POLLUTION REPORTS TABLE
-- =============================================
CREATE TABLE pollution_reports (
    report_id SERIAL PRIMARY KEY,
    user_id INTEGER NOT NULL REFERENCES users(user_id) ON DELETE CASCADE,
    ship_id INTEGER REFERENCES ships(ship_id) ON DELETE SET NULL, -- Optional: kapal yang observe
    
    -- Location data
    location VARCHAR(200) NOT NULL,
    latitude DOUBLE PRECISION NOT NULL,
    longitude DOUBLE PRECISION NOT NULL,
    
    -- Pollution details
    waste_type VARCHAR(100) NOT NULL, -- Plastic, Oil Spill, Chemical, Marine Debris, etc.
    quantity VARCHAR(50), -- Small, Medium, Large, Massive
    severity VARCHAR(20) NOT NULL, -- Low, Medium, High, Critical
    description TEXT NOT NULL,
    environmental_impact TEXT,
    
    -- Status tracking
    status VARCHAR(50) DEFAULT 'Reported', -- Reported, Under Investigation, Cleanup Initiated, Resolved
    action_taken TEXT,
    
    -- Photos/Evidence
    photo_paths TEXT, -- Multiple paths separated by semicolon
    
    -- Timestamps
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    resolved_at TIMESTAMP,
    
    -- Report metadata
    report_status VARCHAR(20) DEFAULT 'Active',
    version INTEGER DEFAULT 1,
    
    -- Indexes
    INDEX idx_pollution_reports_user_id (user_id),
    INDEX idx_pollution_reports_ship_id (ship_id),
    INDEX idx_pollution_reports_status (status),
    INDEX idx_pollution_reports_severity (severity),
    INDEX idx_pollution_reports_created_at (created_at),
    INDEX idx_pollution_reports_location (latitude, longitude)
);

-- =============================================
-- 4. WEATHER DATA TABLE (untuk historical weather)
-- =============================================
CREATE TABLE weather_data (
    weather_id SERIAL PRIMARY KEY,
    ship_id INTEGER REFERENCES ships(ship_id) ON DELETE CASCADE,
    voyage_id INTEGER REFERENCES voyages(voyage_id) ON DELETE CASCADE,
    
    -- Location
    location VARCHAR(200) NOT NULL,
    latitude DOUBLE PRECISION NOT NULL,
    longitude DOUBLE PRECISION NOT NULL,
    
    -- Weather conditions
    temperature DOUBLE PRECISION, -- Celsius
    feels_like DOUBLE PRECISION,
    temp_min DOUBLE PRECISION,
    temp_max DOUBLE PRECISION,
    pressure INTEGER, -- hPa
    humidity INTEGER, -- %
    
    -- Wind
    wind_speed DOUBLE PRECISION, -- m/s
    wind_degree INTEGER, -- degrees
    wind_gust DOUBLE PRECISION,
    
    -- General conditions
    condition VARCHAR(50), -- Clear, Clouds, Rain, Storm, etc.
    description TEXT,
    clouds INTEGER, -- %
    visibility INTEGER, -- meters
    
    -- Sea state
    sea_condition VARCHAR(100),
    warning_level VARCHAR(50),
    safe_for_sailing BOOLEAN,
    
    -- Timestamps
    observation_time TIMESTAMP NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    
    -- Indexes
    INDEX idx_weather_ship_id (ship_id),
    INDEX idx_weather_voyage_id (voyage_id),
    INDEX idx_weather_observation_time (observation_time),
    INDEX idx_weather_location (latitude, longitude)
);

-- =============================================
-- 5. MAINTENANCE REPORTS TABLE (enhanced)
-- =============================================
CREATE TABLE maintenance_reports (
    report_id SERIAL PRIMARY KEY,
    maintenance_id INTEGER NOT NULL REFERENCES maintenance(maintenance_id) ON DELETE CASCADE,
    user_id INTEGER NOT NULL REFERENCES users(user_id) ON DELETE CASCADE,
    ship_id INTEGER NOT NULL REFERENCES ships(ship_id) ON DELETE CASCADE,
    
    -- Location
    location VARCHAR(200) NOT NULL,
    
    -- Maintenance details
    equipment_name VARCHAR(200) NOT NULL,
    issue_description TEXT NOT NULL,
    priority VARCHAR(20) NOT NULL, -- Low, Medium, High, Critical, Urgent
    
    -- Parts & costs
    parts_needed TEXT,
    estimated_duration VARCHAR(50),
    
    -- Timestamps
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    
    -- Report metadata
    report_status VARCHAR(20) DEFAULT 'Active',
    version INTEGER DEFAULT 1,
    
    -- Indexes
    INDEX idx_maintenance_reports_maintenance_id (maintenance_id),
    INDEX idx_maintenance_reports_user_id (user_id),
    INDEX idx_maintenance_reports_ship_id (ship_id),
    INDEX idx_maintenance_reports_priority (priority)
);

-- =============================================
-- 6. SAFETY REPORTS TABLE (enhanced)
-- =============================================
CREATE TABLE safety_reports (
    report_id SERIAL PRIMARY KEY,
    user_id INTEGER NOT NULL REFERENCES users(user_id) ON DELETE CASCADE,
    ship_id INTEGER REFERENCES ships(ship_id) ON DELETE SET NULL,
    maintenance_id INTEGER REFERENCES maintenance(maintenance_id) ON DELETE SET NULL,
    
    -- Location
    location VARCHAR(200) NOT NULL,
    
    -- Incident details
    incident_type VARCHAR(100) NOT NULL, -- Injury, Near Miss, Equipment Failure, etc.
    severity VARCHAR(20) NOT NULL, -- Low, Medium, High, Critical
    description TEXT NOT NULL,
    
    -- People involved
    people_involved TEXT,
    injuries TEXT,
    
    -- Response
    immediate_action_taken TEXT,
    preventive_measures TEXT,
    
    -- Status
    status VARCHAR(50) DEFAULT 'Reported', -- Reported, Under Investigation, Resolved
    
    -- Timestamps
    incident_date TIMESTAMP NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    resolved_at TIMESTAMP,
    
    -- Report metadata
    report_status VARCHAR(20) DEFAULT 'Active',
    version INTEGER DEFAULT 1,
    
    -- Indexes
    INDEX idx_safety_reports_user_id (user_id),
    INDEX idx_safety_reports_ship_id (ship_id),
    INDEX idx_safety_reports_severity (severity),
    INDEX idx_safety_reports_status (status),
    INDEX idx_safety_reports_incident_date (incident_date)
);

-- =============================================
-- 7. WEATHER REPORTS TABLE (user-generated)
-- =============================================
CREATE TABLE weather_reports (
    report_id SERIAL PRIMARY KEY,
    user_id INTEGER NOT NULL REFERENCES users(user_id) ON DELETE CASCADE,
    ship_id INTEGER REFERENCES ships(ship_id) ON DELETE SET NULL,
    
    -- Location
    location VARCHAR(200) NOT NULL,
    
    -- Reporter info
    reporter VARCHAR(200) NOT NULL,
    category VARCHAR(50), -- Observation, Warning, Alert, etc.
    severity VARCHAR(20), -- Low, Medium, High, Critical
    description TEXT NOT NULL,
    
    -- Weather observations
    temperature DOUBLE PRECISION,
    wind_speed VARCHAR(50),
    sea_condition VARCHAR(100),
    
    -- Timestamps
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    
    -- Report metadata
    report_status VARCHAR(20) DEFAULT 'Active',
    version INTEGER DEFAULT 1,
    
    -- Indexes
    INDEX idx_weather_reports_user_id (user_id),
    INDEX idx_weather_reports_ship_id (ship_id),
    INDEX idx_weather_reports_severity (severity)
);

-- =============================================
-- 8. VOYAGE UPDATES (untuk tracking voyage progress)
-- =============================================
CREATE TABLE voyage_updates (
    update_id SERIAL PRIMARY KEY,
    voyage_id INTEGER NOT NULL REFERENCES voyages(voyage_id) ON DELETE CASCADE,
    user_id INTEGER NOT NULL REFERENCES users(user_id) ON DELETE CASCADE,
    
    -- Position
    latitude DOUBLE PRECISION,
    longitude DOUBLE PRECISION,
    
    -- Status update
    status VARCHAR(50), -- Departed, En Route, Arrived, Delayed, etc.
    notes TEXT,
    
    -- Weather at time of update
    weather_condition VARCHAR(100),
    
    -- Timestamp
    update_time TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    
    -- Indexes
    INDEX idx_voyage_updates_voyage_id (voyage_id),
    INDEX idx_voyage_updates_update_time (update_time)
);

-- =============================================
-- 9. TRIGGERS FOR UPDATED_AT
-- =============================================

-- Pollution Reports
CREATE OR REPLACE FUNCTION update_pollution_reports_timestamp()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_at = CURRENT_TIMESTAMP;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER pollution_reports_updated_at
    BEFORE UPDATE ON pollution_reports
    FOR EACH ROW
    EXECUTE FUNCTION update_pollution_reports_timestamp();

-- Maintenance Reports
CREATE OR REPLACE FUNCTION update_maintenance_reports_timestamp()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_at = CURRENT_TIMESTAMP;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER maintenance_reports_updated_at
    BEFORE UPDATE ON maintenance_reports
    FOR EACH ROW
    EXECUTE FUNCTION update_maintenance_reports_timestamp();

-- Safety Reports
CREATE OR REPLACE FUNCTION update_safety_reports_timestamp()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_at = CURRENT_TIMESTAMP;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER safety_reports_updated_at
    BEFORE UPDATE ON safety_reports
    FOR EACH ROW
    EXECUTE FUNCTION update_safety_reports_timestamp();

-- =============================================
-- 10. SAMPLE DATA (untuk testing)
-- =============================================

-- Update existing ship dengan MMSI
UPDATE ships 
SET mmsi = '123456789', 
    ais_enabled = TRUE 
WHERE ship_id = 1;

-- Insert sample pollution report
INSERT INTO pollution_reports (
    user_id, location, latitude, longitude, 
    waste_type, quantity, severity, description, status
) VALUES (
    1, 'Pacific Ocean, near Hawaii', 21.3099, -157.8581,
    'Plastic Debris', 'Large', 'High', 
    'Large patch of plastic waste observed during voyage. Estimated size: 50-100 square meters.',
    'Reported'
);

-- Insert sample weather data
INSERT INTO weather_data (
    ship_id, location, latitude, longitude,
    temperature, humidity, wind_speed, condition,
    sea_condition, safe_for_sailing, observation_time
) VALUES (
    1, 'Java Sea', -6.2088, 106.8456,
    28.5, 75, 8.5, 'Partly Cloudy',
    'Moderate Breeze - Small waves', TRUE, CURRENT_TIMESTAMP
);

-- =============================================
-- 11. USEFUL VIEWS
-- =============================================

-- View untuk active pollution reports yang butuh action
CREATE OR REPLACE VIEW critical_pollution_reports AS
SELECT 
    pr.*,
    u.username as reporter_name,
    s.ship_name,
    CASE 
        WHEN pr.created_at < CURRENT_TIMESTAMP - INTERVAL '7 days' THEN 'Overdue'
        WHEN pr.severity IN ('Critical', 'High') THEN 'Urgent'
        ELSE 'Normal'
    END as action_priority
FROM pollution_reports pr
LEFT JOIN users u ON pr.user_id = u.user_id
LEFT JOIN ships s ON pr.ship_id = s.ship_id
WHERE pr.status != 'Resolved'
ORDER BY pr.severity DESC, pr.created_at DESC;

-- View untuk ship tracking summary
CREATE OR REPLACE VIEW ship_tracking_summary AS
SELECT 
    s.ship_id,
    s.ship_name,
    s.mmsi,
    s.current_latitude,
    s.current_longitude,
    s.current_speed,
    s.current_course,
    s.last_position_update,
    COUNT(sph.position_id) as total_positions_tracked,
    MAX(sph.timestamp) as last_tracked_position
FROM ships s
LEFT JOIN ship_positions_history sph ON s.ship_id = sph.ship_id
WHERE s.ais_enabled = TRUE
GROUP BY s.ship_id;

-- View untuk maintenance overview
CREATE OR REPLACE VIEW maintenance_overview AS
SELECT 
    m.*,
    s.ship_name,
    u.username as assigned_to,
    mr.priority as report_priority,
    mr.equipment_name,
    mr.issue_description
FROM maintenance m
LEFT JOIN ships s ON m.ship_id = s.ship_id
LEFT JOIN users u ON m.user_id = u.user_id
LEFT JOIN maintenance_reports mr ON m.maintenance_id = mr.maintenance_id
ORDER BY m.date DESC;

-- =============================================
-- 12. INDEXES FOR PERFORMANCE
-- =============================================

-- Composite indexes untuk common queries
CREATE INDEX idx_ships_ais_tracking ON ships(ais_enabled, last_position_update) 
WHERE ais_enabled = TRUE;

CREATE INDEX idx_pollution_active ON pollution_reports(status, severity, created_at) 
WHERE status != 'Resolved';

CREATE INDEX idx_maintenance_active ON maintenance(status, date) 
WHERE status != 'Done';

-- =============================================
-- END OF SCHEMA UPDATE
-- =============================================
