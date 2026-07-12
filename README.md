# Freelance Projects Admin Panel

A private admin panel for managing freelance projects — client info, requirements,
contacts, documents, invoices, and notes, all in one place.

**Stack:** ASP.NET Core 9 · Blazor WebAssembly · Entity Framework Core · PostgreSQL (Supabase) · ASP.NET Core Identity

## Features

- Projects list with search and status filtering
- Per-project tabs: Overview, Requirements checklist, Contacts, Documents, Invoices, Notes timeline
- Document upload with private cloud storage and signed-URL downloads
- Single-admin authentication — no public registration

## Getting started

Requires the .NET 9 SDK and a PostgreSQL database (e.g. a free [Supabase](https://supabase.com) project with a private storage bucket).

Configure secrets with [user-secrets](https://learn.microsoft.com/aspnet/core/security/app-secrets) from the `AdminPanel/` folder:

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<postgres-connection-string>"
dotnet user-secrets set "Supabase:Url" "<project-url>"
dotnet user-secrets set "Supabase:ServiceRoleKey" "<service-role-key>"
dotnet user-secrets set "Supabase:StorageBucket" "<bucket-name>"
dotnet user-secrets set "AdminUser:Email" "<admin-email>"
dotnet user-secrets set "AdminUser:Password" "<admin-password>"
```

Then run:

```bash
dotnet run --project AdminPanel
```

Migrations apply automatically on startup and the admin account is seeded on first run.
Never commit real credentials — configuration placeholders live in `appsettings.json`, real values stay in user-secrets.
