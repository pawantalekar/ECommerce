# ECommerce Project

A full-stack E-commerce application featuring a modern Angular frontend and a robust .NET microservices backend.

## 🚀 Project Overview

The project follows a **Microservices Architecture** with a **Clean Architecture** approach in the backend. It consists of multiple independent services handling specific business domains (Auth, Catalog, Cart, Order, Payment, Review).

## 🛠️ Technology Stack

### Frontend
- **Framework:** [Angular](https://angular.dev/) (v20.3.4)
- **Styling:** Bootstrap 5, PrimeNG
- **State Management:** RxJS
- **Tooling:** Angular CLI

### Backend
- **Framework:** .NET (ASP.NET Core)
- **Architecture:** Microservices, Clean Architecture (Domain, Application, Infrastructure, API)
- **Database:** SQL Server (implied from EF Core configurations)
- **Testing:** xUnit / NUnit (Ecom.Test)

## 📂 Project Structure

```
ECommerce/
├── backend/                  # .NET Backend Solution
│   └── src/
│       ├── Ecom.Api/         # Microservices API Projects
│       │   ├── AuthService.Api
│       │   ├── CartService.Api
│       │   ├── CatalogService.Api
│       │   ├── OrderService.Api
│       │   ├── PaymentService.Api
│       │   └── ReviewService.Api
│       ├── Ecom.Application/ # Core Business Logic
│       ├── Ecom.Domain/      # Domain Entities & Interfaces
│       ├── Ecom.Infrastructure/ # Data Access & External Services
│       └── Ecom.Test/        # Unit & Integration Tests
│
└── frontend/                 # Angular Frontend
    └── ecom-frontend/
        ├── src/
        └── package.json
```

## ⚙️ Prerequisites

Ensure you have the following installed locally:
- **Node.js**: v20.7.0 or higher
- **Angular CLI**: `npm install -g @angular/cli`
- **.NET SDK**: .NET 8.0 or higher (Visual Studio 2022 recommended)
- **SQL Server**: Local or remote instance

## 🏁 Getting Started

### Backend Setup

1.  **Navigate to the backend solution:**
    ```bash
    cd backend/src
    ```
2.  **Open in Visual Studio:**
    Open `src.sln` in Visual Studio 2022.
3.  **Run Microservices:**
    - You can run specific services (e.g., `AuthService.Api`) by setting them as the Startup Project in Visual Studio.
    - Alternatively, using the CLI:
      ```bash
      dotnet run --project Ecom.Api/AuthService.Api
      dotnet run --project Ecom.Api/CatalogService.Api
      # ... run other services as needed
      ```
4.  **Database Configuration:**
    Ensure your `appsettings.json` in each API project points to a valid SQL Server instance. You may need to run migrations:
    ```bash
    dotnet ef database update --project Ecom.Infrastructure --startup-project Ecom.Api/AuthService.Api
    ```
    *(Note: Migration commands may vary depending on where the DbContext is configured used.)*

### Frontend Setup

1.  **Navigate to the frontend directory:**
    ```bash
    cd frontend/ecom-frontend
    ```
2.  **Install dependencies:**
    ```bash
    npm install
    ```
3.  **Start the development server:**
    ```bash
    ng serve
    ```
4.  **Access the application:**
    Open your browser and navigate to `http://localhost:4200/`.

## 🧪 Running Tests

### Backend Tests
```bash
cd backend/src
dotnet test
```

### Frontend Tests
```bash
cd frontend/ecom-frontend
ng test
```

