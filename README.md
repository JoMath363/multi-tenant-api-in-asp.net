# 🏢 Multi-Tenant API Challenge in ASP.NET  

A **multi-tenant project management API** built with ASP.NET, designed to simulate a **real-world SaaS challenge**.  
Each company (tenant) has isolated users, projects, and tasks. Features include role-based access control, subscription plans, and tenant-aware queries.  

---

## 🚀 Features
- **Multi-tenancy**: data isolation per company  
- **Authentication**: JWT tokens including tenant context  
- **Role-based access**: `Admin`, `Manager`, `Member`  
- **Subscription plans**:
  - Free → max 3 projects
  - Standart → max 10 projects
  - Premium → unlimited projects

---

## 📦 Core Entities
- **Tenant** → represents a company  
- **User** → belongs to a tenant, has a role  
- **Project** → scoped to a tenant  
- **Task** → scoped to a project & tenant  

---

## 🔗 API Endpoints

### 🔹 Tenant Management
- `GET /tenants/account` → Retrieve all accounts associated with the tenant.  
- `POST /tenants/register` → Register a new tenant and automatically create its admin account.  
- `PATCH /tenants/plan` → Update the tenant's subscription plan.  
- `DELETE /tenants` → Delete the tenant along with all its accounts, projects, and tasks.  

### 🔹 Account Management
- `GET /accounts/tenant` → Retrieve information about the tenant of the logged-in account.  
- `POST /accounts/register` → Register a new account within a tenant.  
- `POST /accounts/login` → Log in and retrieve access credentials for an account.  
- `PATCH /accounts/{id}/role/{role}` → Change the role of a specific account.  
- `DELETE /accounts/{id}` → Remove a specific account.  

### 🔹 Projects
- `GET /projects` → List all projects for the tenant.  
- `GET /projects/{projectId}` → Retrieve a specific project by its ID.  
- `POST /projects` → Create a new project.  
- `PATCH /projects/{projectId}` → Update an existing project.  
- `DELETE /projects/{projectId}` → Delete a project along with all its tasks.  

### 🔹 Tasks
- `GET /projects/{projectId}/tasks` → List all tasks within a specific project.  
- `GET /projects/{projectId}/tasks/{taskId}` → Retrieve a specific task by its ID.  
- `POST /projects/{projectId}/tasks` → Create a new task under a specific project.  
- `PATCH /projects/{projectId}/tasks/{taskId}` → Update a specific task in a project.  
- `DELETE /projects/{projectId}/tasks/{taskId}` → Delete a specific task from a project.  

## 🔧 Tutorial: Setup & Installation

### 1. Clone the Repository

```bash
git clone https://github.com/JoMath363/multi-tenant-api-in-asp.net.git
cd multi-tenant-api-in-asp.net
```

### 2. Restore Dependencies

```bash
dotnet restore
```
### 3. Apply Database Migrations

```bash
dotnet ef database update
```
The project uses SQLite by default. You can change the connection string in appsettings.json if needed.

### 4. Run the API

```bash
dotnet run
```
The API runs on http://localhost:5217 by default.

### 5. Open Swagger UI

Access the interactive API documentation at:
http://localhost:5217/swagger/index.html

