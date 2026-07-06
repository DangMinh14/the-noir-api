# Thé Noir API

ASP.NET Core (.NET 10) backend for [Thé Noir](https://github.com/DangMinh14/the-noir), a fictional French-style tea house brand in Vietnam. Controllers + services architecture, EF Core with SQLite for local development.

## Run locally

```bash
dotnet ef database update   # creates thenoir.db (first run only)
dotnet run --launch-profile http
```

Then open:

- `http://localhost:5051/scalar` — interactive API reference, test endpoints from the browser (Development only)
- `http://localhost:5051/openapi/v1.json` — OpenAPI document (Development only)

Requires the .NET 10 SDK and the EF Core CLI tool (`dotnet tool install --global dotnet-ef`).

## Endpoints

Full CRUD for both resources:

| Method | Route | |
|---|---|---|
| GET | `/api/products` | list, ordered by `sortOrder` |
| GET | `/api/products/{id}` | 404 if missing |
| POST | `/api/products` | 201 + Location header |
| PUT | `/api/products/{id}` | full replace, 404 if missing |
| DELETE | `/api/products/{id}` | 204, 404 if missing |

Same shape under `/api/cities`.

## Structure

```
Controllers/   # HTTP layer: routing, status codes
Services/      # business logic, CRUD against the DbContext
Models/        # EF Core entities
Dtos/          # request bodies with validation attributes
Data/          # AppDbContext
Migrations/
```

Data lives in the database only (no seed data in code). The local `thenoir.db` is gitignored; back it up with `sqlite3 thenoir.db ".dump" > backup.sql`. SQLite is a local-dev convenience; the target production database is PostgreSQL.

## Roadmap

- [x] CRUD: products, cities, categories
- [x] Auth + admin-only writes (JWT, User/Admin roles)
- [x] Image upload for products
- [ ] Orders/cart (no Order entity yet — nothing to actually purchase)
- [ ] Real email service (forgot-password currently returns the reset token directly, dev-only)
- [ ] Newsletter subscription
- [ ] Tea tasting booking

Full cross-repo audit and phased plan: [the-noir/ROADMAP.md](https://github.com/DangMinh14/the-noir/blob/main/ROADMAP.md).
