# Debra - Event Management System (Backend)

## Description
**Debra** is the backend for an event management system built using **.NET 8.0.0** and **MS SQL Server**. This API allows for managing events, registrations, schedules, and more. It is designed as part of an educational project.

## Features
- Event creation and management
- User registration for events
- Schedule management
- Database integration using MS SQL Server

## Technologies Used
- **.NET 8.0.0**
- **Entity Framework Core**
- **MS SQL Server**
- **Swagger** (for API documentation)

## Prerequisites
To run this project locally, ensure you have the following:
- [.NET 8.0.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)

## Setup and Installation

### 1. Clone the Repository:
```bash
git clone https://github.com/C-Nisshan/debra-event-management-backen.git
cd debra-backend
```

### 2. Restore NuGet Packages:
```bash
dotnet restore
```

### 3. Configure the Database:
#### 1. Update the appsettings.json file to set your SQL Server connection string:
```bash
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=your_server;Database=your_db;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```
#### 2. Apply database migrations:
```bash
dotnet ef database update
```

### 3. Run the Application:
```bash
dotnet run
```

### Access the API at http://localhost:5000/swagger to explore the endpoints via Swagger.

### 4. Running Tests (Optional)
```bash
dotnet test
```


