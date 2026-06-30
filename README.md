 Grand Azure Hotel Management System

A modern Hotel Management System built with ASP.NET Core Razor Pages, C#, Entity Framework Core, and SQL Server.
The application is designed to simplify hotel operations by allowing customers to search and book rooms while providing hotel staff with powerful tools to manage reservations, rooms, customers, invoices, and hotel services through an intuitive dashboard.

 Overview
This project was developed as part of my university coursework to gain practical experience in building a real-world web application using Microsoft's .NET ecosystem.

The system follows a clean architecture with Entity Framework Core for data access and Razor Pages for server-side rendering, making it scalable, maintainable, and easy to extend.

 Features
 Customer Module
Customers can:

 Register and log in securely
 Browse available rooms
 Book rooms based on availability
 View booking history
 Cancel bookings
 Manage their profile
 View booking details
 Admin Module
Administrators can:
 Access an admin dashboard
 Manage hotel rooms
 Manage room categories
 Manage hotel staff
 View and manage all bookings
 Generate invoices
 Process customer payments
 Manage hotel services and amenities
 Update administrator profile
 Booking Workflow
here is receptionist and manager also exist with limited access
The booking lifecycle follows a complete hotel reservation process.

Customer Registration/Login
          │
          ▼
Search Available Rooms
          │
          ▼
Book Room
          │
          ▼
Booking Created
          │
          ▼
Admin Confirms Booking
          │
          ▼
Customer Check-In
          │
          ▼
Invoice & Payment
          │
          ▼
Customer Check-Out
          │
          ▼
Room Becomes Available Again
 Tech Stack
Technology	Description
ASP.NET Core Razor Pages	Web Framework
C#	Backend Programming Language
Entity Framework Core	ORM & Data Access
SQL Server	Database
HTML5	Frontend Structure
CSS3	Styling
JavaScript	Client-side Interactions
Bootstrap	Responsive UI
Font Awesome	Icons
 Project Architecture
Browser
   │
   ▼
Razor Pages
   │
   ▼
Page Models
   │
   ▼
Business Services
   │
   ▼
Repository Pattern
   │
   ▼
Entity Framework Core
   │
   ▼
SQL Server Database
📂 Project Structure
HMS
│
├── Core
├── Data
├── Extensions
├── Models
│   ├── Entities
│   └── ViewModels
├── Pages
│   ├── Account
│   ├── Admin
│   ├── Customer
│   └── Shared
├── Repositories
├── Services
├── wwwroot
├── Program.cs
└── appsettings.json
 Getting Started
Prerequisites

Before running the project, make sure you have:

Visual Studio 2022
.NET SDK
SQL Server / SQL Server Express / LocalDB
Installation
Clone the repository
git clone https://github.com/Nadir5423/HASERIKHAN-hms.git
Navigate into the project
cd HASERIKHAN-hms
Configure the database

Update the connection string inside appsettings.json

"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=HotelManagementDB;Trusted_Connection=True;MultipleActiveResultSets=true"
}
Apply Database Migrations
dotnet ef database update
Run the application
dotnet run

The application will start locally and display the URL in the terminal.

 Authentication

The system uses Cookie-Based Authentication with different roles:

 Customer
 Administrator
 Manager
 Receptionist

Role-based authorization ensures users only access pages relevant to their permissions.

 What I Learned

Building this project helped me strengthen my understanding of:

ASP.NET Core Razor Pages
Entity Framework Core
Repository Pattern
CRUD Operations
Authentication & Authorization
SQL Server Integration
Session Management
Clean Code Practices
Full-Stack Web Development
 Future Improvements
Email notifications
Online payment gateway integration
Room image gallery
Booking analytics dashboard
Customer reviews and ratings
Responsive mobile optimization
QR-based check-in
Real-time room availability updates
 Developer
Nadir AHmad

Computer Science Student

Passionate about building scalable web applications, AI-powered solutions, and solving real-world problems through software development.

 If you found this project interesting, consider giving it a Star on GitHub!
