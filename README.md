# 🚀 Project Name

Your Ultimate Parking Assistant helps drivers find available parking spots in Sofia, Bulgaria.

GitHub version:
![.NET Version](https://img.shields.io/badge/.NET-10.0-purple)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-10.0-blue)
![License](https://img.shields.io/badge/license-MIT-green)

Azure version:
![.NET Version](https://img.shields.io/badge/.NET-8.0-purple)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-8.0-blue)
![License](https://img.shields.io/badge/license-MIT-green)

---

## 📋 Table of Contents

- [About the Project](#about-the-project)
- [Technologies Used](#technologies-used)
- [Prerequisites](#prerequisites)
- [Deployment](#deployment)
- [Database Setup](#database-setup)

---

## 📖 About the Project

**YOUR ULTIMATE PARKING ASSISTANT** is a full-stack ASP.NET Core MVC application designed to help drivers find available parking spots in Sofia, Bulgaria, while enabling garage owners to manage their parking spaces and generate revenue. The application leverages spatial queries via NetTopologySuite to calculate distances and find nearby garages efficiently.

The system provides:
- Real-time search for nearby garages using geolocation
- Dynamic pricing policies with configurable rules
- Full parking session lifecycle (entry, billing, exit)
- Revenue tracking for garage owners
- Role-based access (Admin, Owner, Driver)

The project follows a clean architecture approach with:
- Separation of concerns (Controllers → Services → Repositories)
- Unit of Work and Repository patterns
- DTO/ViewModel mapping via AutoMapper
- Entity Framework Core with Code-First approach

It is built as part of the **ASP.NET Advanced** course at SoftUni and demonstrates production-ready practices including CI/CD and cloud deployment.


## ⚙️ What tasks we manage?

The application helps drivers find a parking spot with ease while giving property owners an accessible way to earn extra cash. It matches a driver’s location or destination with nearby available parking spots and handles everything from fee management to automated billing, providing a one-click revenue report to owners. It’s designed to also be suitable to private driveway owners who can now rent out their space 24/7, whether they’re at work, traveling, or just out shopping.

---

## 🛠️ Technologies Used

| Technology                     | Version | Purpose                              |
|------------------------------|--------|--------------------------------------|
| ASP.NET Core MVC             | 10.0   | Web framework                        |
| Entity Framework Core        | 10.0   | ORM / Database access                |
| SQL Server                   | -      | Primary database (local & Docker)    |
| NetTopologySuite             | 10.0   | Spatial data (geolocation support)   |
| AutoMapper                   | 12.x   | Object mapping                       |
| ASP.NET Identity             | -      | Authentication & authorization       |
| Bootstrap                    | 5.3    | Frontend styling                     |
| xUnit                        | -      | Unit testing                         |
| Docker                       | -      | Local database containerization      |
| GitHub Actions               | -      | CI/CD pipeline                       |
| Microsoft Azure              | -      | Cloud hosting (production deployment)|

---

## ✅ Prerequisites

To run the project locally (GitHub version), you need:

- .NET SDK 10.0+
- Visual Studio 2022 or VS Code
- Docker
- SQL Server Management Studio (SSMS)
- Git

This setup is required for the project to function correctly in a local environment.


### ⚠️ Important Note

The GitHub version of the project uses **SQL Server running in a Docker container**.
Due to spatial data support (NetTopologySuite), Spatial index is applied through a custom migration.
Database is initialized via Entity Framework migrations.

---

## 🚀 Getting Started

### 1. Clone the repository

```bash
git clone https://github.com/sensurl/CarsInsideGarage.git
cd CarsInsideGarage
```

### 2. Restore dependencies

```bash
dotnet restore
```

### 3. Apply database migrations
The application applies migrations automatically on startup in the configured environment.

Alternatively:
```bash
dotnet ef database update
```

### 4. Run the application
```bash
dotnet build
```

The app will be available at:

https://localhost:7212
and
http://localhost:5190


---


## ☁️ Deployment

The application is successfully deployed in the cloud using **Microsoft Azure**:

🌐 **Live URL:**  
https://park-in-sofia-bdb8hteuh2bwbxak.westeurope-01.azurewebsites.net/

### Azure Architecture

The production environment includes:

- **Azure Web App Service** – hosts the ASP.NET Core application
- **Azure Database for MySQL (Flexible Server)** – production database
- **Custom Dockerfile** – used for containerized deployment

### Important Clarification

There are **two versions of the project**:

#### 1. GitHub Version (Exam Version, Staging)
- Uses **SQL Server (Docker)**
- Requires manual setup (described in Prerequisites)
- Uses NetTopologySuite with SQL Server

#### 2. Azure Deployment Version (Production)
- Uses **MySQL (Azure Flexible Server)**
- Uses **Pomelo.EntityFrameworkCore.MySql**
- Adjusted DbContext and configuration for MySQL compatibility
- Fully automated deployment via Docker

⚠️ Due to environment differences, the Azure-specific configuration is **not included in the GitHub repository**.

---

### CI/CD
Unit tests are implemented using xUnit and Mock. These cover the Services layer business logic including GarageService, CarService, ParkingSessionService, and PricingCalculator. 
The project includes a working **GitHub Actions pipeline**:

- Restores dependencies
- Builds the project
- Runs all unit tests

Workflow file:
.github/workflows/dotnet.yml

---

## 📁 Project Structure

```
CarsInsideGarage/
	│
	├──CarsInsideGarage/
	│	│
	│	├── Areas/					 # Admin Area
	│	│	│
	│	│	├── Controllers/
	│	│	├── Models/
	│	│	├── Services/
	│	│	└── Views/
	│	│
	│	├── Controllers/             # MVC Controllers
	│	├── Data/                    # DbContext and Entities
	│	├── docs/ 					 # Video walk through the project
	│	├── Interfaces/				 # IRepository & IUnitOfWork
	│	├── Mappings/                # AutoMapper MappingProfile
	│	├── Migrations/              # Database migrations
	│	├── Models/                  # Domain models and ViewModels
	│	├── Repositories/ 			 # Contracts repositories
	│	├── Services/                # Business logic / service layer
	│	├── Views/                   # Razor Views (.cshtml)
	│	├── wwwroot/                 # Static files (CSS, JS, images)
	│	└── Program.cs               # App entry point and middleware setup
	│
	└──CarsInsideGarage.Tests/

```

---

## ✨ Features

- [x] User registration and login (ASP.NET Identity)
- [x] CRUD operations for Cars, Garages, Parking Sessions
- [x] Role-based access (Admin / Owner / Driver)
- [x] Geolocation-based garage search 
- [x] Pagination on the find nearest parking results 
- [x] Dynamic pricing system
- [x] Responsive UI with Bootstrap

---

## 💻 Usage

To access the features of the app after launching:

```
[All Users]
1. Navigate to /Register to create an account.
2. Log in at /Login and choose Role [Owner or Driver].
3. Use the dashboard to manage your [Car] or [Garage].

[Owners]
4. Create a parking fee.
5. Create a parking lot/spot.
6. Your Parking Revenue shows the income accumulated in real time.

[Drivers]
4. Search for available parking spots near your location or destination. 
5. Choose a parking spot and check available spots and fees.
6. Register your car's plate number. /in v.2 car plate shall be read automatically/
7. Go to the same parking spot and click "Park My Car Here" to use a spot.
8. Click "Pay" to end the session.
9. Click "Exit" to exit the parking lot. 

```

> 💡 Steps shown in the video: (docs/)

---

## 🗄️ Database Setup

The project uses **Entity Framework Core (Code-First)** with support for spatial data.

### Local Development (GitHub Version)

Database setup requires:

1. Start SQL Server in Docker:

```bash
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=Your_password123" \
-p 1433:1433 --name sqlserver -d mcr.microsoft.com/mssql/server:2022-latest
```

2. Update connection string in appsettings.Staging.json

3. Apply migrations:
```bash
dotnet ef database update
```

Notes
Spatial support is implemented using NetTopologySuite
A custom migration creates a spatial index for performance
Database is seeded automatically on application startup

### Production (Azure) 
Database is hosted on Azure MySQL Flexible Server
Migrations are applied automatically on startup
Connection string is managed via Azure configuration

---

## 🤝 Contributing 

Contributions are welcome! To contribute:

1. Fork the repository
2. Create a new branch: `git checkout -b feature/your-feature-name`
3. Commit your changes: `git commit -m "Add some feature"`
4. Push to the branch: `git push origin feature/your-feature-name`
5. Open a Pull Request

---

## 📄 License

This project is licensed under the **MIT License**. See the [LICENSE](LICENSE) file for details.

---

## 📬 Contact

[@sensurl](https://github.com/sensurl)

Project Link: https://github.com/sensurl/CarsInsideGarage

---

*Built as part of the **ASP.NET Advanced** course of SoftUni - February 2026.*
