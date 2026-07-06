# Thé Noir API

ASP.NET Core (.NET 10) backend for [Thé Noir](https://github.com/DangMinh14/the-noir), a fictional French-style tea house brand in Vietnam. Minimal APIs organized by feature folders, EF Core with SQLite for local development.

## Run locally

```bash
dotnet ef database update   # creates and seeds thenoir.db (first run only)
dotnet run --launch-profile http
```

Then open:

- `http://localhost:5051/api/products` — signature drinks
- `http://localhost:5051/api/maisons` — cities and locations
- `http://localhost:5051/scalar` — interactive API reference, test endpoints from the browser (Development only)
- `http://localhost:5051/openapi/v1.json` — OpenAPI document (Development only)

Requires the .NET 10 SDK and the EF Core CLI tool (`dotnet tool install --global dotnet-ef`).

## Structure

```
Features/
  Products/    # entity + endpoints per feature
  Maisons/
Data/
  AppDbContext.cs   # DbContext + seed data
Migrations/
```

SQLite is a local-dev convenience; the target production database is PostgreSQL (swap `UseSqlite` for `UseNpgsql` and regenerate migrations).

## Roadmap

- [x] Read-only catalog: products, maisons
- [ ] Newsletter subscription
- [ ] Auth + admin CRUD
- [ ] Tea tasting booking
