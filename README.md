# FSP-API — Wildlife (k'aaxil ba'alilche')

REST API built with **ASP.NET Core (.NET 9)** for a wildlife sighting app. Users submit sightings with a photo and location, and administrators accept or reject them. The API also serves a species catalog, sighting maps and PDF fact sheets.

**Stack:** MediatR (CQRS) · SQL Server (ADO.NET + stored procedures) · JWT RS256 · BCrypt · PuppeteerSharp (PDF) · NLog · Swagger · Docker

## Project structure

```
FSP-API/
├── FSP.API/             # Controllers, Program.cs, config, email/PDF templates, wwwroot images
├── FSP.Application/     # MediatR commands (writes) and queries (reads)
├── FSP.Domain/          # Models, DTOs, enums
└── FSP.Infrastructure/  # Repositories, embedded SQL, PDF helper, error middleware, RSA public key
```

Request flow: Controller → MediatR handler → Repository → SQL Server.

## Configuration

**`appsettings.json`:** `ConnectionStrings:WebConnection`, `Jwt:PublicKeyPath`, `Jwt:Issuer`, `Jwt:Audience`, `MailSettings:Mail`, `MailSettings:Password`, `ImageSettings:RootPath`, `ImageSettings:RootPathCatalog`. Use User Secrets locally for credentials.

**Environment variables:**

| Variable | Purpose |
|---|---|
| `JWT_PRIVATE_KEY` | Path to the RSA private key (PEM) used to sign tokens |
| `SQL_KEY` | Key for the password-recovery stored procedures (can also come from `FSP.API/.env`) |
| `BASE_URL` | Public API URL, used to build image URLs |

## Running

```bash
cd FSP-API
dotnet run --project FSP.API
```

The API runs at `https://localhost:7279`. Swagger is at `/swagger` in `Development`.

With Docker:

```bash
cd FSP-API
docker build -t fsp-api .
docker run -p 8080:8080 -e ASPNETCORE_ENVIRONMENT=Docker \
  -e JWT_PRIVATE_KEY=/app/Keys/private_key.pem -e SQL_KEY=... -e BASE_URL=http://localhost:8080 \
  -v /path/to/keys:/app/Keys fsp-api
```

## Endpoints

All routes start with `/api`. 🔒 = token required; 👑 = `Admin` role required.

| Method | Route | Description |
|---|---|---|
| POST | `/Authentication/LogIn` | Log in, returns access and refresh tokens |
| POST | `/Authentication/refresh-token` | Get new tokens |
| GET | `/Authentication/Code?email=` | Email a recovery code |
| GET | `/Authentication/verify-code?email=&code=` | Exchange the code for a reset token |
| POST | `/Authentication/change-password` 🔒 | Change password (reset token) |
| POST | `/Users/Register` | Register a user |
| DELETE | `/Users?userId=` | Soft-delete a user |
| GET | `/Users/user-information` 🔒 | Current user's data |
| GET | `/Users/counts` 🔒 | Total users and records |
| POST | `/Animal/NewRecord` 🔒 | Create a sighting (Base64 image) |
| GET | `/Animal/AnimalRecordByUser/{status}` 🔒 | The user's own records |
| GET | `/Animal/Catalog?page=&size=` 🔒 | Paginated catalog |
| GET | `/Animal/common-noun` 🔒 | Common names |
| GET | `/Animal/by-Id/{id}` 🔒 | Species details |
| GET | `/Animal/map/{id}` 🔒 | Sighting map for a species |
| GET | `/Animal/bibliographic/{id}` 🔒 | Species fact sheet (PDF) |
| POST | `/Admin/new-catalog` 👑 | Add a species |
| PATCH | `/Admin/catalog` · `/Admin/catalog-img` 👑 | Update a species' details or image |
| GET | `/Admin/all-records/{status}` 👑 | All records by status |
| PATCH | `/Admin/process-record` 👑 | Accept or reject a record (emails the user) |
| GET | `/Admin/all-users/{active}` · `/Admin/RecordByUser/{userId}` 👑 | Users and records per user |
| GET | `/Admin/statistic` 👑 | Record totals by status |

`status` is `Pending`, `Accepted` or `Rejected`. List endpoints accept `?page=&size=`. Errors are returned as `{ "status", "error", "message" }`.
