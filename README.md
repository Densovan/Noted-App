# Notes Application

A full-stack Notes application with CRUD functionality, search, filtering, and authentication.

## Tech Stack

- **Backend:** ASP.NET Core 8, Dapper, SQL Server
- **Frontend:** Vue 3 (Vite), TypeScript, Tailwind CSS, Pinia
- **Infrastructure:** Docker, Docker Compose

## How to Run

### Prerequisites

- Docker and Docker Compose installed.

### Steps

1. **Clone or download the project.**
2. **Start the environment:**
   ```bash
   docker-compose up -d --build
   ```
   This will start:
   - **SQL Server** on port 1433
   - **Backend API** at [http://localhost:5001](http://localhost:5001) (Swagger at `/swagger`)
3. **Run the Frontend locally (as Dockerizing the dev server is optional):**
   ```bash
   cd frontend
   npm install
   npm run dev
   ```
   The frontend will be available at [http://localhost:5173](http://localhost:5173).

## Features

- **Authentication:** Register and Login to manage your own notes.
- **Notes Management:** Create, Read, Update, and Delete notes.
- **Search & Filter:** Quickly find notes by title or content.
- **Sorting:** Sort notes by date or title.
- **Responsive Design:** Mobile-friendly UI built with Tailwind CSS.
- **Timestamps:** "Created At" and "Updated At" are automatically handled.

## Environment Variables

- **Backend:**
  - `ConnectionStrings__DefaultConnection`: SQL Server connection string.
  - `Jwt__Key`: JWT signing key.
- **Frontend:**
  - `VITE_API_URL`: URL of the backend API (default: `http://localhost:5000/api`).
