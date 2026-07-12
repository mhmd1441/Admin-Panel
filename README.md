# Freelance Projects Admin Panel

A private admin panel for tracking your freelance projects: company/client info, status, financials,
contacts, contract/NDA documents, invoices, and a notes timeline per project.

Stack: ASP.NET Core Web API + Blazor WebAssembly (all C#), Entity Framework Core, Supabase (Postgres + Storage),
ASP.NET Core Identity for a single admin login.

## Project layout

- `AdminPanel/` — server: API controllers, EF Core `DbContext`, Identity, Supabase Storage service, hosts the compiled client
- `AdminPanel.Client/` — Blazor WebAssembly frontend (pages, API client)
- `AdminPanel.sln` — solution file

## 1. Create a Supabase project

1. Go to [supabase.com](https://supabase.com) and create a free project.
2. In **Storage**, create a new **private** bucket (uncheck "Public bucket"). Any name works —
   set the same name in the `Supabase:StorageBucket` secret below (this project currently uses `AdminBucket`).
3. In **Project Settings → Database**, copy the connection string (use the "Session pooler" or direct
   connection option — either works). It looks like:
   ```
   Host=aws-0-xxxx.pooler.supabase.com;Port=5432;Database=postgres;Username=postgres.xxxx;Password=YOUR-DB-PASSWORD;SSL Mode=Require;Trust Server Certificate=true
   ```
4. In **Project Settings → API**, copy the **Project URL** and the **service_role** secret key
   (not the `anon` key — the service role key is required for server-side uploads to a private bucket).

## 2. Configure local secrets

Never put real credentials in `appsettings.json` (it's committed to the repo). Use .NET user-secrets instead,
run from the `AdminPanel/` folder:

```bash
cd AdminPanel
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=...;Port=5432;Database=postgres;Username=...;Password=...;SSL Mode=Require;Trust Server Certificate=true"
dotnet user-secrets set "Supabase:Url" "https://YOUR-PROJECT-REF.supabase.co"
dotnet user-secrets set "Supabase:ServiceRoleKey" "YOUR-SERVICE-ROLE-KEY"
dotnet user-secrets set "Supabase:StorageBucket" "AdminBucket"
dotnet user-secrets set "AdminUser:Email" "you@example.com"
dotnet user-secrets set "AdminUser:Password" "choose-a-strong-password"
```

The admin account above is seeded automatically the first time the app starts (only if it doesn't already exist).
There is no public registration page — this is a single-admin panel.

## 3. Run the app

From the repo root:

```bash
dotnet run --project AdminPanel
```

This applies EF Core migrations automatically on startup, seeds the admin account, and serves both the API
and the Blazor WebAssembly app. Open the URL shown in the console (e.g. `https://localhost:7237`) and log in
with the admin email/password you set above.

## 4. What you get

- **Projects list** (`/`) — search and filter by status
- **Project detail** (`/projects/{id}`) — tabs for Overview, Contacts, Documents, Invoices, Notes/Timeline
- **New/Edit project** — company, title, description, status, dates, rate, budget, payment terms, notes
- **Documents tab** — upload contracts/NDAs/proposals; files are stored in your private Supabase Storage
  bucket and downloaded via short-lived signed URLs (the bucket itself is never public)

## Notes for future deployment

If you later deploy this somewhere reachable from the internet, set the same configuration keys as
environment variables (`ConnectionStrings__DefaultConnection`, `Supabase__Url`, `Supabase__ServiceRoleKey`,
`AdminUser__Email`, `AdminUser__Password`) instead of user-secrets, and make sure `AdminUser:Password` is
only used once (the seeder skips creation if the admin account already exists) — you can safely remove it
from config after first run.
