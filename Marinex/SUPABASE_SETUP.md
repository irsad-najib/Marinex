# Setup Supabase untuk Marinex

## 1. Setup Database Schema

Jalankan SQL berikut di Supabase SQL Editor:

```sql
-- Create users table
CREATE TABLE IF NOT EXISTS users (
    userid SERIAL PRIMARY KEY,
    username VARCHAR(100) NOT NULL,
    email VARCHAR(255) UNIQUE NOT NULL,
    password VARCHAR(255) NOT NULL,  -- Stores BCrypt hashed passwords
    role VARCHAR(50) DEFAULT 'User',
    company VARCHAR(255),
    created_at TIMESTAMP DEFAULT NOW()
);

-- Create ships table (from your existing Models)
CREATE TABLE IF NOT EXISTS ships (
    shipid SERIAL PRIMARY KEY,
    shipname VARCHAR(255) NOT NULL,
    shiptype VARCHAR(100),
    owner VARCHAR(255),
    capacity INTEGER,
    status VARCHAR(50) DEFAULT 'Docked',
    created_at TIMESTAMP DEFAULT NOW()
);

-- Create voyages table
CREATE TABLE IF NOT EXISTS voyages (
    voyageid SERIAL PRIMARY KEY,
    shipid INTEGER REFERENCES ships(shipid),
    userid INTEGER REFERENCES users(userid),
    from_location VARCHAR(255),
    destination VARCHAR(255),
    estimated_duration INTERVAL,
    created_at TIMESTAMP DEFAULT NOW()
);

-- Create maintenance table
CREATE TABLE IF NOT EXISTS maintenances (
    maintenanceid SERIAL PRIMARY KEY,
    shipid INTEGER REFERENCES ships(shipid),
    userid INTEGER REFERENCES users(userid),
    maintenance_date TIMESTAMP,
    type VARCHAR(100),
    status VARCHAR(50) DEFAULT 'Pending',
    created_at TIMESTAMP DEFAULT NOW()
);

-- Insert sample user for testing
-- Password 'admin123' hashed with BCrypt
INSERT INTO users (username, email, password, role, company)
VALUES ('admin', 'admin@marinex.com', '$2a$11$XKvvz8YJVvPz3.oE5wN8JePSvvqL8KxR5YqGqxQJZxO8VQZ0yQvKG', 'Admin', 'Marinex Corp');

-- Note: The hashed password above is for 'admin123'
-- When registering new users through the app, passwords will be automatically hashed
```

## 2. Update Connection String

Edit file `Services/SupabaseService.cs` dan ganti connection string:

```csharp
private const string ConnectionString = "postgresql://postgres:[YOUR-PASSWORD]@db.zbwkuuzmhdaathgrsmff.supabase.co:5432/postgres";
```

Ganti `[YOUR-PASSWORD]` dengan password Supabase Anda.

## 3. Connection Strings yang Tersedia

Anda punya 2 connection string:

### Direct Connection (Recommended untuk development):

```
postgresql://postgres:[YOUR-PASSWORD]@db.zbwkuuzmhdaathgrsmff.supabase.co:5432/postgres
```

### Pooled Connection (Recommended untuk production):

```
postgresql://postgres.zbwkuuzmhdaathgrsmff:[YOUR-PASSWORD]@aws-1-ap-southeast-1.pooler.supabase.com:6543/postgres
```

## 4. Test Login

Setelah setup, gunakan kredensial ini untuk test:

- Email: `admin@marinex.com`
- Password: `admin123`

## 5. Build & Run

```cmd
dotnet restore
dotnet build
dotnet run
```

## ⚠️ Security Notes

**IMPLEMENTED**:

- ✅ Password hashing using BCrypt.Net (work factor: 11)
- ✅ SQL injection prevention via Npgsql parameterized queries

**TODO for production**:

1. Use environment variables atau Azure Key Vault untuk connection strings
2. Add rate limiting untuk login attempts
3. Implement JWT atau session tokens untuk persistent authentication
4. Add password strength requirements
5. Implement "Forgot Password" functionality dengan email verification
