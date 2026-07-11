using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace TheNoir.Api.Data;

// SQLite has no timezone type, so every DateTime round-trips through the
// database with Kind=Unspecified. System.Text.Json then serializes it
// without a "Z" suffix, which browsers parse as local time instead of UTC,
// shifting every displayed timestamp by the client's UTC offset. The app
// only ever writes DateTime.UtcNow, so it's safe to stamp Kind=Utc back on
// read without changing the underlying value.
public class UtcDateTimeConverter() : ValueConverter<DateTime, DateTime>(
    v => v,
    v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

public class NullableUtcDateTimeConverter() : ValueConverter<DateTime?, DateTime?>(
    v => v,
    v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v);
