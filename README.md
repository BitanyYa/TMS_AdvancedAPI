# TMS Advanced API

## Requirements

- .NET SDK 10
- PostgreSQL

## Configure the database

Set the PostgreSQL password before running the API. The committed `appsettings.json` contains a placeholder and does not contain credentials.

For local development, use user secrets:

```powershell
dotnet user-secrets init --project .\TmsApi.Api\TmsApi.Api.csproj
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=tms_db;Username=postgres;Password=YOUR_POSTGRES_PASSWORD" --project .\TmsApi.Api\TmsApi.Api.csproj
```

Apply the existing migration:

```powershell
dotnet ef database update --project .\TmsApi.Infrastructure\TmsApi.Infrastructure.csproj --startup-project .\TmsApi.Api\TmsApi.Api.csproj
```

## Run

```powershell
dotnet run --project .\TmsApi.Api\TmsApi.Api.csproj
```

The API listens on `http://localhost:5036` in the HTTP launch profile.

## Endpoints

- `GET /api/v1/courses`
- `GET /api/v2/courses`
- `POST /api/v2/enrollments`
- `GET /api/v2/enrollments/{enrollmentId}/schedule`