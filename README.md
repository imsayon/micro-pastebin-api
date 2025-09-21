# Micro-Pastebin API 📋

A simple, robust, and high-performance backend API service for creating and retrieving text snippets, inspired by Pastebin.com. This project was built to demonstrate a full backend development lifecycle, from API design to containerization and deployment.

---

## ✨ Key Features

- **Persistent Storage:** Uses PostgreSQL to permanently store data.
- **High-Speed Caching:** Implements a Redis caching layer to dramatically improve performance for frequently read data.
- **Secure Endpoints:** Protects creation endpoints with API Key authentication via custom middleware.
- **Containerized:** Fully containerized with Docker, including a multi-stage `Dockerfile` for an optimized, lightweight runtime image.

---

## 🛠️ Tech Stack

- **Backend Framework:** C# on .NET 8 (using ASP.NET Core Minimal APIs)
- **Primary Database:** PostgreSQL
- **Caching Layer:** Redis
- **API Security:** Custom ASP.NET Core Middleware
- **Object-Relational Mapper (ORM):** Entity Framework Core with code-first migrations.
- **Deployment:** Docker
- **API Testing:** Postman

---

## 🚀 How to Run

### Using the .NET SDK (Locally)

1.  **Prerequisites:** Ensure you have the .NET 8 SDK, Docker Desktop (for running PostgreSQL and Redis), and Postman installed.
2.  **Start Services:** Make sure your PostgreSQL and Redis Docker containers are running.
3.  **Configure:** Update the `Host`, `Port`, and `Password` in the connection strings within `Program.cs` to match your local setup.
4.  **Run:** Open a terminal in the project's root directory and run the command `dotnet run`. The API will be available at `http://localhost:5224` (or a similar port).

### Using Docker (Recommended)

1.  **Prerequisites:** Ensure you have Docker Desktop running.
2.  **Build the API Image:** Open a terminal and run `docker build -t micro-pastebin-api .`
3.  **Run the Container:** Run `docker run -p 8080:8080 --name api-container micro-pastebin-api`.
4.  **Access:** The API will be available at `http://localhost:8080`.