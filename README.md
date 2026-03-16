# 🚀 Project Name

YOUR ULTIMATE PARKING ASSISTANT is made to assist car drivers find a parking spot in Sofia, Bulgaria. 

![.NET Version](https://img.shields.io/badge/.NET-8.0-purple)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-8.0-blue)
![License](https://img.shields.io/badge/license-MIT-green)

---

## 📋 Table of Contents

- [About the Project](#about-the-project)
- [Technologies Used](#technologies-used)
- [Prerequisites](#prerequisites)
- [Getting Started](#getting-started)
- [Project Structure](#project-structure)
- [Features](#features)
- [Usage](#usage)
- [Database Setup](#database-setup)
- [Configuration](#configuration)
- [Contributing](#contributing)
- [License](#license)
- [Contact](#contact)

---

## 📖 About the Project

This is a task management web application built as part of the *ASP.NET Fundamentals* course of SoftUni. It demonstrates core concepts like MVC architecture, Entity Framework Core, and RESTful API design.


## ⚙️ What tasks we manage?

The application helps drivers find a parking spot with ease while giving property owners an accessible way to earn extra cash. It matches a driver’s location or destination with nearby available parking spots and handles everything from fee management to automated billing, providing a one-click revenue report to owners. It’s designed to also be suitable to private driveway owners who can now rent out their space 24/7, whether they’re at work, traveling, or just out shopping.

---

## 🛠️ Technologies Used

| Technology            | Version  | Purpose                          |
|-----------------------|----------|----------------------------------|
| ASP.NET Core MVC      | 10.0     | Web framework                    |
| Entity Framework Core | 10.0     | ORM / Database access            |
| SQL Server / SQLite   | -        | Database                         |
| SqlServer.NetTopologySuite |10.0 | Geography Point                  |
| Bootstrap             | 5.3      | Frontend styling                 |
| Razor Pages / Views   | -        | Server-side HTML rendering       |


---

## ✅ Prerequisites

Make sure you have the following installed before running the project:

- [.NET SDK 8.0+](https://dotnet.microsoft.com/download)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)
- [SQL Server](https://www.microsoft.com/en-us/sql-server)
- [Git](https://git-scm.com/)

---

🚀 Getting Started
Follow these steps to get the project running locally.

1. Clone the repository

Bash
git clone https://github.com/sensurl/CarsInsideGarage.git
cd CarsInsideGarage

3. Restore dependencies

Bash
dotnet restore

4. Set Environment and Update Database

The project logic relies on the Staging environment. 
Run the command that matches your terminal:

For Windows (Command Prompt - CMD):

DOS
set ASPNETCORE_ENVIRONMENT=Staging
dotnet ef database update

For Windows (PowerShell):

PowerShell
$env:ASPNETCORE_ENVIRONMENT='Staging'
dotnet ef database update

For macOS / Linux / Git Bash:

Bash
export ASPNETCORE_ENVIRONMENT=Staging
dotnet ef database update

4. Run the application

When running the app, ensure the environment flag is passed to maintain the "Staging" logic:
Bash
dotnet run --environment Staging

The app is available at https://localhost:7212 or http://localhost:5190.

## 📁 Project Structure

```
CarsInsideGarage/
│
├── Controllers/             # MVC Controllers
├── Data/                    # DbContext and Entities
├── Mappings/                # AutoMapper MappingProfile
├── Migrations/              # Database migrations
├── Models/                  # Domain models and ViewModels
├── Services/                # Business logic / service layer
├── Views/                   # Razor Views (.cshtml)
├── wwwroot/                 # Static files (CSS, JS, images)
├── appsettings.Staging.json # App configuration
└── Program.cs               # App entry point and middleware setup
```

---

## ✨ Features

- [ ] User registration and login (ASP.NET Identity)
- [ ] CRUD operations for [Car, Garage, Fee, Location]
- [ ] RESTful API endpoints
- [ ] Input validation (server-side & client-side)
- [ ] Responsive UI with Bootstrap

---

## 💻 Usage

To access the features of the app after launching:

```
1. Navigate to /Register to create an account.
2. Log in at /Login and choose Role [Owner or Driver].
3. Use the dashboard to manage your [Car or Garage].
[Owners]
1. Create a parking fee.
2. Create a parking lot/spot.
[Drivers]
1. Search for available parking spots near your location or destination. /ToDo
2. Choose a parking spot and click "Park My Car Here" to access it
3. Register your car's plate number. /in v.2 car plate shall be read automatically/
4. Go to the same parking spot and again click "Park My Car Here" to use a spot.
5. When leaving the parking lot, click "Pay" to end the session.
6. "Exit" to leave the parking lot.
[Owners]
3. Your Parking Revenue shows the income accumulated in real time.
```

> 💡 Take a look at the screenshots: (docs/screenshots.png)

---

## 🗄️ Database Setup

The project uses **Entity Framework Core** with a Code-First approach.

Connection string is configured in `appsettings.Staging.json`:

```json
"ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=ParkingLots;User Id=sa;Password=MYPASSWORD;TrustServerCertificate=True"
}
```

To create the database:

```bash
export ASPNETCORE_ENVIRONMENT=Staging
dotnet ef database update
```

**Database is empty and has no seeded data. 
Registering an Owner and a Driver and their corresponding entity [Car, Garage] is a prerequisite.**
---

## ⚙️ Configuration

Key settings in `appsettings.Staging.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=ParkingLots;User Id=sa;Password=MYPASSWORD;TrustServerCertificate=True"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  }
}
```
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

*Built as part of the **ASP.NET Fundamentals** course of SoftUni - January 2026.*
