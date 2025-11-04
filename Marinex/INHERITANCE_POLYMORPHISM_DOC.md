# Dokumentasi Inheritance dan Polymorphism

## 📋 Overview

File ini menjelaskan implementasi **Inheritance** dan **Polymorphism** yang sudah ditambahkan ke proyek Marinex untuk memenuhi tugas kelompok.

---

## 🏗️ 1. INHERITANCE (Pewarisan)

### Base Class: `BaseReport.cs`

**Lokasi**: `Marinex/Models/BaseReport.cs`

Base class abstract yang menjadi parent class untuk semua jenis report. Menggunakan **Encapsulation** dengan private fields dan public properties.

**Fitur Inheritance**:
- Abstract class `BaseReport` sebagai base class
- Abstract method `GenerateReport()` yang harus diimplementasikan child class
- Virtual method `GetSummary()` yang bisa di-override oleh child class
- Encapsulation dengan private fields dan public properties

### Child Class 1: `SafetyReport.cs`

**Lokasi**: `Marinex/Models/SafetyReport.cs`

Child class yang inherit dari `BaseReport` dengan implementasi khusus untuk Safety Report.

**Implementasi Inheritance**:
```csharp
public class SafetyReport : BaseReport
{
    // Properties khusus SafetyReport
    public string IncidentType { get; set; }
    public string Severity { get; set; }
    
    // Override abstract method dari base class
    public override string GenerateReport() { ... }
    
    // Override virtual method dari base class
    public override string GetSummary() { ... }
}
```

### Child Class 2: `MaintenanceReport.cs`

**Lokasi**: `Marinex/Models/MaintenanceReport.cs`

Child class yang inherit dari `BaseReport` dengan implementasi berbeda dari `SafetyReport`.

**Implementasi Inheritance**:
```csharp
public class MaintenanceReport : BaseReport
{
    // Properties khusus MaintenanceReport (berbeda dengan SafetyReport)
    public string EquipmentName { get; set; }
    public string IssueDescription { get; set; }
    
    // Override abstract method dengan implementasi berbeda
    public override string GenerateReport() { ... }
    
    // Override virtual method dengan behavior berbeda
    public override string GetSummary() { ... }
}
```

**Konsep Inheritance yang diterapkan**:
- ✅ Single Inheritance (SafetyReport dan MaintenanceReport inherit dari BaseReport)
- ✅ Method Overriding (override abstract dan virtual methods)
- ✅ Encapsulation (private fields dengan public properties)

---

## 🎭 2. POLYMORPHISM (Polimorfisme)

### Interface: `IDatabaseService.cs`

**Lokasi**: `Marinex/Services/IDatabaseService.cs`

Interface yang mendefinisikan kontrak untuk semua database service implementations.

**Method yang harus diimplementasikan**:
- `ConnectAsync()`
- `DisconnectAsync()`
- `IsConnectedAsync()`
- `GetByIdAsync<T>(int id)`
- `SaveAsync<T>(T entity)`
- `DeleteAsync<T>(int id)`

### Implementation 1: `ShipDatabaseService.cs`

**Lokasi**: `Marinex/Services/ShipDatabaseService.cs`

Implementasi `IDatabaseService` khusus untuk operasi Ship database.

**Polymorphism Implementation**:
```csharp
public class ShipDatabaseService : IDatabaseService
{
    // Implementasi semua method dari interface
    public async Task<bool> ConnectAsync() { ... }
    public async Task<T?> GetByIdAsync<T>(int id) where T : class { ... }
    // Behavior khusus untuk Ship operations
}
```

### Implementation 2: `VoyageDatabaseService.cs`

**Lokasi**: `Marinex/Services/VoyageDatabaseService.cs`

Implementasi `IDatabaseService` khusus untuk operasi Voyage database dengan behavior berbeda dari `ShipDatabaseService`.

**Polymorphism Implementation**:
```csharp
public class VoyageDatabaseService : IDatabaseService
{
    // Implementasi semua method dari interface dengan behavior berbeda
    public async Task<bool> ConnectAsync() { ... }
    public async Task<T?> GetByIdAsync<T>(int id) where T : class { ... }
    // Behavior khusus untuk Voyage operations (berbeda dengan ShipDatabaseService)
    
    // Method tambahan yang spesifik untuk Voyage
    public async Task<List<Voyage>> GetVoyagesByShipAsync(int shipId) { ... }
}
```

### Service Helper: `ReportService.cs`

**Lokasi**: `Marinex/Services/ReportService.cs`

Service yang menggunakan Inheritance dan Polymorphism untuk memproses berbagai jenis report.

**Polymorphism yang diterapkan**:
```csharp
// Method yang menerima base class tapi bisa bekerja dengan berbagai child class
public string ProcessReport(BaseReport report)
{
    // Polymorphism: GenerateReport() akan dipanggil sesuai tipe sebenarnya
    return report.GenerateReport();
}

// Polymorphism dengan collection
public List<string> ProcessMultipleReports(List<BaseReport> reports)
{
    foreach (var report in reports)
    {
        // Polymorphism: Setiap report akan memanggil GenerateReport() sesuai tipenya
        results.Add(report.GenerateReport());
    }
}
```

**Konsep Polymorphism yang diterapkan**:
- ✅ Interface-based Polymorphism (IDatabaseService dengan implementasi berbeda)
- ✅ Inheritance-based Polymorphism (BaseReport dengan child classes yang berbeda)
- ✅ Method Overriding (GenerateReport() dengan implementasi berbeda per child class)
- ✅ Runtime Polymorphism (method dipanggil berdasarkan tipe objek sebenarnya)

---

## 📝 Contoh Penggunaan

### Inheritance Example:

```csharp
// Membuat SafetyReport (inherit dari BaseReport)
var safetyReport = new SafetyReport
{
    ReportID = 1,
    Location = "Java Sea",
    IncidentType = "Fire",
    Severity = "Critical",
    Description = "Engine room fire detected",
    CreatedAt = DateTime.Now
};

// Membuat MaintenanceReport (inherit dari BaseReport)
var maintenanceReport = new MaintenanceReport
{
    ReportID = 2,
    Location = "Surabaya Port",
    EquipmentName = "Main Engine",
    IssueDescription = "Overheating issue",
    Priority = "Urgent",
    CreatedAt = DateTime.Now
};

// Polymorphism: ProcessReport bisa menerima kedua jenis report
var reportService = new ReportService();
string safetyResult = reportService.ProcessReport(safetyReport);      // Memanggil SafetyReport.GenerateReport()
string maintenanceResult = reportService.ProcessReport(maintenanceReport); // Memanggil MaintenanceReport.GenerateReport()
```

### Polymorphism Example (Interface):

```csharp
// Membuat instance dari berbagai implementasi interface
IDatabaseService shipService = new ShipDatabaseService();
IDatabaseService voyageService = new VoyageDatabaseService();

// Polymorphism: Kedua service bisa dipanggil dengan cara yang sama
await shipService.ConnectAsync();
await voyageService.ConnectAsync();

// Tapi behavior-nya berbeda sesuai implementasinya
Ship? ship = await shipService.GetByIdAsync<Ship>(1);        // Query dari ships table
Voyage? voyage = await voyageService.GetByIdAsync<Voyage>(1); // Query dari voyages table

// Polymorphism dengan list
List<IDatabaseService> services = new List<IDatabaseService>
{
    new ShipDatabaseService(),
    new VoyageDatabaseService()
};

foreach (var service in services)
{
    // Polymorphism: Setiap service akan memanggil ConnectAsync() sesuai implementasinya
    await service.ConnectAsync();
}
```

---

## ✅ Checklist Tugas

### Tugas 1: Inheritance, Encapsulation, atau Polymorphism

- [x] **Inheritance**: ✅ Implemented
  - Base class: `BaseReport` (abstract class)
  - Child classes: `SafetyReport`, `MaintenanceReport`
  - Method overriding: `GenerateReport()`, `GetSummary()`
  
- [x] **Encapsulation**: ✅ Already exists + Enhanced
  - Private fields dengan public properties di `BaseReport`
  - Property validation di `BaseReport.Location`
  
- [x] **Polymorphism**: ✅ Implemented
  - Interface: `IDatabaseService`
  - Implementations: `ShipDatabaseService`, `VoyageDatabaseService`
  - Inheritance-based polymorphism: `BaseReport` → `SafetyReport`/`MaintenanceReport`
  - Method overriding dengan behavior berbeda

### Tugas 2: Koneksi PostgreSQL

- [x] **PostgreSQL Connection**: ✅ Already exists
  - Location: `Marinex/Services/SupabaseService.cs`
  - Menggunakan `Npgsql` package
  - Methods: `AuthenticateUser()`, `RegisterUser()`, `TestConnection()`
  - **NEW**: `ShipDatabaseService` dan `VoyageDatabaseService` juga menggunakan PostgreSQL connection

---

## 📂 File Structure

```
Marinex/
├── Models/
│   ├── BaseReport.cs              ← NEW: Base class untuk Inheritance
│   ├── SafetyReport.cs            ← NEW: Child class (Inheritance)
│   ├── MaintenanceReport.cs       ← NEW: Child class (Inheritance)
│   ├── Ship.cs                    ← Existing (tidak diubah)
│   ├── User.cs                    ← Existing (tidak diubah)
│   └── ...
├── Services/
│   ├── IDatabaseService.cs        ← NEW: Interface untuk Polymorphism
│   ├── ShipDatabaseService.cs     ← NEW: Implementation 1 (Polymorphism)
│   ├── VoyageDatabaseService.cs   ← NEW: Implementation 2 (Polymorphism)
│   ├── ReportService.cs           ← NEW: Service helper (Polymorphism)
│   └── SupabaseService.cs         ← Existing (tidak diubah)
└── INHERITANCE_POLYMORPHISM_DOC.md ← NEW: Dokumentasi ini
```

---

## 🎯 Kesimpulan

✅ **Inheritance**: Diimplementasikan dengan `BaseReport` sebagai base class dan `SafetyReport`/`MaintenanceReport` sebagai child classes.

✅ **Polymorphism**: Diimplementasikan dengan:
- Interface `IDatabaseService` dengan implementasi berbeda (`ShipDatabaseService`, `VoyageDatabaseService`)
- Inheritance-based polymorphism dengan `BaseReport` dan child classes
- Method overriding dengan behavior berbeda per implementasi

✅ **Encapsulation**: Sudah ada di semua Models, ditambah enhanced di `BaseReport`.

✅ **PostgreSQL Connection**: Sudah ada di `SupabaseService`, ditambah implementasi baru di `ShipDatabaseService` dan `VoyageDatabaseService`.

**Catatan**: Semua struktur yang sudah ada **TIDAK DIUBAH**. File baru ditambahkan untuk demonstrasi Inheritance dan Polymorphism tanpa conflict.

