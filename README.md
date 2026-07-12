<div align="center">

# 🏥 Medicare

### A Full-Stack Telemedicine Platform

Medicare is a modern healthcare management platform connecting doctors and patients through secure, real-time video consultations, appointment scheduling, online payments, and cloud-based medical services.

This repository contains the **backend API**, built with **ASP.NET Core** following **Onion Architecture**, developed by me as the backend developer. The platform is consumed by a separate **Angular** frontend client.

![.NET](https://img.shields.io/badge/.NET-10-512BD4?style=for-the-badge&logo=dotnet)
![Angular](https://img.shields.io/badge/Angular-21-DD0031?style=for-the-badge&logo=angular)
![SQL Server](https://img.shields.io/badge/SQL%20Server-CC2927?style=for-the-badge&logo=microsoftsqlserver)
![Redis](https://img.shields.io/badge/Redis-DC382D?style=for-the-badge&logo=redis)
![SignalR](https://img.shields.io/badge/SignalR-RealTime-0078D7?style=for-the-badge)
![WebRTC](https://img.shields.io/badge/WebRTC-Video-333333?style=for-the-badge&logo=webrtc)
![Stripe](https://img.shields.io/badge/Stripe-Payments-635BFF?style=for-the-badge&logo=stripe)
![License](https://img.shields.io/badge/License-MIT-green?style=for-the-badge)

</div>

---

## 📑 Table of Contents

- [Overview](#-overview)
- [Features](#-features)
- [Architecture](#-architecture)
- [Tech Stack](#-tech-stack)
- [Solution / Onion Layers](#-solution--onion-layers)
- [Backend Modules](#-backend-modules)
- [My Contribution](#-my-contribution)
- [Third-Party Integrations](#-third-party-integrations)
- [Authentication Flow](#-authentication-flow)
- [Video Consultation Flow](#-video-consultation-flow-signalr--webrtc)
- [Project Structure](#-project-structure)
- [Getting Started](#-getting-started)
- [Configuration](#-configuration)
- [Running the Project](#-running-the-project)
- [API Documentation](#-api-documentation)
- [Screenshots](#-screenshots)
- [Future Improvements](#-future-improvements)
- [Contributors](#-contributors)
- [License](#-license)

---

## 📖 Overview

**Medicare** is a telemedicine platform that digitizes the doctor–patient relationship end to end — from registration and appointment booking to live video consultations and payment.

The platform supports:

- 👤 Patient registration & profile management
- 🩺 Doctor management & availability
- 📅 Appointment booking & scheduling
- 💬 Online consultations with doctor notes
- 🎥 Secure real-time video sessions (WebRTC + SignalR)
- 💳 Online payments (Stripe)
- 📧 Email verification & notifications (SendGrid)
- 🔐 OTP verification (Redis)
- ☁️ Cloud-based medical image management (Cloudinary)

As the backend developer, I designed and built the entire API using **Onion Architecture**, keeping the domain model independent of infrastructure concerns and making the system testable, maintainable, and easy to extend. The frontend (Angular) is a separate client consuming this API.

---

## ✨ Features

- ✅ JWT Authentication & Role-Based Authorization
- ✅ Appointment Scheduling & Management
- ✅ Consultation Requests & Status Tracking
- ✅ Patient & Doctor Profile Management
- ✅ Admin Dashboard
- ✅ Real-Time Video Consultation (WebRTC)
- ✅ Real-Time Notifications (SignalR)
- ✅ Stripe Payment Gateway Integration
- ✅ Redis-based OTP Verification
- ✅ Cloudinary Image Upload & Management
- ✅ SendGrid Transactional Emails
- ✅ Clean, Layered Onion Architecture
- ✅ Global Exception Handling & Validation
- ✅ Swagger / OpenAPI Documentation

---

## 🏗 Architecture

Medicare follows **Onion Architecture**, where dependencies flow inward — the Domain layer has no dependency on outer layers, and infrastructure concerns (database, external services) are plugged in through abstractions.

```
                ┌─────────────────────────────┐
                │           API Layer          │
                │  Controllers · Middleware     │
                │  Filters · SignalR Hubs       │
                └───────────────┬───────────────┘
                                │
                ┌───────────────▼───────────────┐
                │      Infrastructure Layer      │
                │  Redis · Cloudinary · SendGrid │
                │  Stripe · WebRTC Signaling     │
                └───────────────┬───────────────┘
                                │
                ┌───────────────▼───────────────┐
                │       Persistence Layer        │
                │  EF Core · Repositories · UoW  │
                └───────────────┬───────────────┘
                                │
                ┌───────────────▼───────────────┐
                │       Application Layer        │
                │  Services · DTOs · Interfaces  │
                │  Validation · Business Rules   │
                └───────────────┬───────────────┘
                                │
                ┌───────────────▼───────────────┐
                │          Domain Layer          │
                │   Entities · Enums · Exceptions │
                │        (No dependencies)        │
                └─────────────────────────────────┘
```

> 📌 Add your exported architecture diagram image at `docs/architecture.png` and reference it here:
> `![Architecture](docs/architecture.png)`

---

## 🧰 Tech Stack

| Frontend | Backend | Database | Cloud / Infra | Real-Time |
|---|---|---|---|---|
| Angular | ASP.NET Core (Onion Architecture) | SQL Server | Cloudinary | SignalR |
| Tailwind CSS | EF Core | Redis | SendGrid | WebRTC |
| TypeScript | C# | — | Stripe | — |

---

## 🧅 Solution / Onion Layers

| Layer | Responsibility |
|---|---|
| **Domain** | Core entities, enums, domain exceptions, and business rules. No dependency on any other layer. |
| **Application** | Use cases, service interfaces, validation, and business logic orchestration. Depends only on Domain. |
| **Shared** | DTOs used for communication between layers (e.g. Application ↔ API, Application ↔ Infrastructure). |
| **Persistence** | EF Core `DbContext`, entity configurations, repositories, and Unit of Work implementation. |
| **Infrastructure.Cloudinary** | Implementation of media/image storage using Cloudinary. |
| **Infrastructure.SendGrid** | Implementation of transactional email sending using SendGrid. |
| **Infrastructure.Stripe** | Implementation of payment processing using Stripe. |
| **Infrastructure.Caching** | Redis-based caching and OTP storage. |
| **Infrastructure.Hangfire** | Background jobs and scheduled tasks (e.g. reminders, cleanup). |
| **API** | Controllers, SignalR hubs, middleware, filters, and dependency injection composition root. |

Each external service (Cloudinary, SendGrid, Stripe, Redis, Hangfire) is isolated in its **own infrastructure layer**, implementing interfaces defined in the Application layer. This keeps integrations decoupled and easy to swap or test independently.

---

## 🧩 Project Modules

The Medicare backend is organized into the following modules:

- **AdminModule**
- **AppointmentModule**
- **Caching**
- **ConsultationModule** (Session Module)
- **DoctorModule**
- **Hangfire**
- **IdentityModule**
- **PatientModule**
- **PaymentModule**
- **ReviewModule**

### Module Highlights

**Identity Module**
- User registration & login
- JWT token generation & refresh
- Role-based authorization (Patient / Doctor / Admin)

**Patient Module**
- Patient profile & medical information
- Consultation history

**Consultation / Session Module**
- Consultation requests
- Status management (Pending / Approved / Completed / Cancelled)
- Video session initiation via SignalR + WebRTC

**Admin Module**
- User management
- Platform statistics & reports

---

## 👨‍💻 My Contribution

As the backend developer, I was primarily responsible for:

- **Project setup** — solution structure, Onion Architecture layering, and initial configuration
- **Identity Module** — authentication, JWT, role-based authorization
- **Patient Module** — patient profiles & medical information
- **Consultation Module** (Session Module) — consultation requests, status management, and video session handling (SignalR + WebRTC)
- **Admin Module** — user management & platform reports

I also contributed to other modules built by the team (e.g. Appointment, Doctor, Payment, Review) with fixes, reviews, and feature support alongside my teammates.

I also integrated:

- JWT Authentication
- Redis (OTP verification & caching)
- Cloudinary (media storage)
- SendGrid (transactional email)
- SignalR (real-time notifications)
- WebRTC (video calls)

---

## 🔗 Third-Party Integrations

| Service | Purpose |
|---|---|
| **Stripe** | Online payment processing |
| **Redis** | OTP storage & caching |
| **Cloudinary** | Medical image & media storage |
| **SendGrid** | Transactional email delivery |
| **SignalR** | Real-time notifications & signaling |
| **WebRTC** | Peer-to-peer video consultations |

---

## 🔐 Authentication Flow

1. User registers → email verification code sent via **SendGrid**
2. Verification code cached temporarily in **Redis**
3. User confirms email → account activated
4. Login issues a **JWT access token** (+ refresh token)
5. Subsequent requests are authorized via role-based **JWT** claims

---

## 🎥 Video Consultation Flow (SignalR + WebRTC)

1. Patient books an appointment → Doctor approves the consultation
2. At session time, both parties connect to a **SignalR Hub**
3. SignalR exchanges **WebRTC SDP offers/answers** and **ICE candidates**
4. A direct **peer-to-peer WebRTC connection** is established for video/audio
5. Session events (join, leave, end) are broadcast in real time via SignalR

---

## 📁 Project Structure

```
Medicare/
├── src/
│   ├── Medicare.Domain/                    # Entities, Enums, Domain Exceptions
│   ├── Medicare.Application/               # Services, Interfaces, Validation, Business Logic
│   ├── Medicare.Shared/                    # DTOs shared across layers
│   ├── Medicare.Persistence/               # DbContext, Repositories, Migrations, Unit of Work
│   ├── Medicare.Infrastructure.Cloudinary/ # Cloudinary media storage integration
│   ├── Medicare.Infrastructure.SendGrid/   # SendGrid email integration
│   ├── Medicare.Infrastructure.Stripe/     # Stripe payment integration
│   ├── Medicare.Infrastructure.Caching/    # Redis caching / OTP storage
│   ├── Medicare.Infrastructure.Hangfire/   # Background jobs & scheduling
│   └── Medicare.API/                       # Controllers, SignalR Hubs, Middleware, Program.cs
│
├── docs/                                   # Diagrams & screenshots
├── Medicare.sln
└── README.md
```

> ℹ️ The Angular frontend that consumes this API lives in a separate repository.

---

## 🚀 Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- SQL Server (LocalDB or full instance)
- Redis instance
- Stripe / Cloudinary / SendGrid accounts (for API keys)

### Clone the Repository

```bash
git clone https://github.com/<your-username>/Medicare.git
cd Medicare
```

### Setup & Run

```bash
cd src/Medicare.API
dotnet restore
dotnet ef database update
dotnet run
```

---

## ⚙️ Configuration

Create an `appsettings.Development.json` (or use **User Secrets**) in `Medicare.API` with the following structure — **never commit real secrets**:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=MedicareDb;Trusted_Connection=True;TrustServerCertificate=True"
  },
  "Redis": {
    "ConnectionString": "localhost:6379"
  },
  "Jwt": {
    "Issuer": "Medicare",
    "Audience": "MedicareClient",
    "Key": "YOUR_SECRET_KEY",
    "ExpiryMinutes": 60
  },
  "Cloudinary": {
    "CloudName": "YOUR_CLOUD_NAME",
    "ApiKey": "YOUR_API_KEY",
    "ApiSecret": "YOUR_API_SECRET"
  },
  "SendGrid": {
    "ApiKey": "YOUR_SENDGRID_KEY",
    "FromEmail": "no-reply@medicare.com"
  },
  "Stripe": {
    "PublishableKey": "YOUR_PUBLISHABLE_KEY",
    "SecretKey": "YOUR_SECRET_KEY"
  }
}
```

> 💡 Use `dotnet user-secrets` locally and environment variables / a secrets manager in production.

---

## ▶️ Running the Project

| Component | Command | URL |
|---|---|---|
| Backend API | `dotnet run` (in `Medicare.API`) | `https://localhost:5001` |
| Swagger UI | — | `https://localhost:5001/swagger` |

---

## 📚 API Documentation

Interactive API documentation is available via **Swagger** once the backend is running:

```
https://localhost:5001/swagger
```

Example endpoints:

| Method | Endpoint | Description |
|---|---|---|
| `POST` | `/api/auth/register` | Register a new user |
| `POST` | `/api/auth/login` | Authenticate & receive JWT |
| `GET` | `/api/patients/{id}` | Get patient profile |
| `POST` | `/api/consultations` | Create a consultation request |
| `PUT` | `/api/consultations/{id}/status` | Update consultation status |
| `POST` | `/api/payments/create-intent` | Create a Stripe payment intent |

> 📌 Replace with your actual endpoint list, or link to a generated Postman/Swagger export.

---

## 📷 Screenshots

> Add screenshots to `docs/screenshots/` and reference them below.

| Login | Dashboard |
|---|---|
| ![Login](docs/screenshots/login.png) | ![Dashboard](docs/screenshots/dashboard.png) |

| Consultation | Video Session |
|---|---|
| ![Consultation](docs/screenshots/consultation.png) | ![Video Session](docs/screenshots/video-session.png) |

---

## 🔮 Future Improvements

- 💬 In-app chat messaging between doctor & patient
- 📋 Electronic medical records (EMR) module
- 🤖 AI-assisted symptom analysis
- 🔔 Push notifications
- 📱 Mobile application (Flutter / React Native)

---

## 👥 Contributors

**Backend:**
- Mohamed Elbarbary

> The Angular frontend for this platform is maintained in a separate repository by the frontend team.

---

## 📄 License

This project is licensed under the **MIT License** — see the [LICENSE](LICENSE) file for details.
