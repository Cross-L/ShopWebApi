# Shop Web API

A simple ASP.NET Core Web API for managing shop data, built with .NET 9, Entity Framework Core, and PostgreSQL. It provides endpoints to retrieve customers by birthday, recent customers, and purchased product categories. Swagger UI is used for testing.

## Prerequisites

- **.NET 9 SDK**: [Download](https://dotnet.microsoft.com/download/dotnet/9.0)
- **PostgreSQL 13+**: [Download](https://www.postgresql.org/download/)
- **Git**: To clone the repository

## Setup

### 1. Clone the Repository
```bash
git clone https://github.com/Cross-L/ShopWebApi.git
cd your-repo-name
```

### 2. Restore the Database
The database backup (`ShopDbBackup.sql`) is a PostgreSQL custom-format dump.

1. **Ensure PostgreSQL is running**:
   ```bash
   pg_ctl status
   ```

2. **Create the `ShopDb` database**:
   ```bash
   psql -U postgres -c "CREATE DATABASE ShopDb;"
   ```

3. **Restore the backup**:
   ```bash
   pg_restore -U postgres -d ShopDb --verbose ShopDbBackup.sql
   ```

4. **Verify the data**:
   ```bash
   psql -U postgres -d ShopDb -c "SELECT * FROM Customers;"
   ```
   Expected tables: `Customers`, `Products`, `Orders`, `OrderItems`.

### 3. Configure the Connection String
Set the PostgreSQL connection string using User Secrets:
```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Database=ShopDb;Username=postgres;Password=your_password"
```

### 4. Run the API
```bash
dotnet restore
dotnet run
```
The API will be available at:
- `http://localhost:5182/swagger` (Swagger UI)
- `https://localhost:7095/swagger` (if HTTPS is configured)

## Testing with Swagger
Open `http://localhost:5182/swagger` in your browser. Test the following endpoints (use test date: July 27, 2025):

1. **GET /api/Customers/birthdays**
   - **Parameter**: `date=2025-07-26`
   - **Expected Response** (HTTP 200):
     ```json
     [{"id":1,"fullName":"Іван Петров"}]
     ```

2. **GET /api/Customers/recent**
   - **Parameter**: `days=7`
   - **Expected Response** (HTTP 200):
     ```json
     [
       {"id":1,"fullName":"Іван Петров","lastPurchase":"2025-07-24T00:00:00Z"},
       {"id":2,"fullName":"Марія Сидоренко","lastPurchase":"2025-07-22T00:00:00Z"},
       {"id":3,"fullName":"Олексій Коваленко","lastPurchase":"2025-07-25T00:00:00Z"}
     ]
     ```

3. **GET /api/Customers/{customerId}/categories**
   - **Parameter**: `customerId=1`
   - **Expected Response** (HTTP 200):
     ```json
     [
       {"category":"Молочні продукти","totalQuantity":2},
       {"category":"Хлібобулочні вироби","totalQuantity":1}
     ]
     ```

## Troubleshooting
- **API not responding**:
  - Check if ports `5182` or `7095` are free:
    ```bash
    netstat -aon | findstr :5182
    ```
  - Disable HTTPS redirection in `Program.cs`:
    ```csharp
    // app.UseHttpsRedirection();
    ```

- **Database issues**:
  - Verify the connection string and PostgreSQL service.
  - Ensure the backup is restored correctly.

- **HTTPS issues**:
  - Regenerate the development certificate:
    ```bash
    dotnet dev-certs https --clean
    dotnet dev-certs https --trust
    ```
