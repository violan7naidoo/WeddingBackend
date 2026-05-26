using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using OurBigDay.Api.Data;
using OurBigDay.Api.Entities;
using OurBigDay.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Prefer DATABASE_URL (postgresql:// URI). Must be converted to Npgsql key=value format because
// NpgsqlConnectionStringBuilder (used internally by EF Core) does not accept raw URIs.
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
var defaultConnection = builder.Configuration.GetConnectionString("DefaultConnection");
if (!string.IsNullOrWhiteSpace(databaseUrl))
{
    defaultConnection = BuildNpgsqlConnectionStringFromDatabaseUrl(databaseUrl);
}

if (string.IsNullOrWhiteSpace(defaultConnection) || !LooksLikePostgresConnectionString(defaultConnection))
{
    throw new InvalidOperationException(
        "Missing PostgreSQL connection. Set DATABASE_URL (postgresql:// URI) or ConnectionStrings__DefaultConnection to a Postgres connection string.");
}

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(defaultConnection);
});

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

var jwtKey = builder.Configuration["Jwt:Key"] ?? "OurBigDay-Wedding-Planner-Secret-Key-Phase2-Min32Chars!";
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ClockSkew = TimeSpan.Zero,
    };
});
builder.Services.AddAuthorization();

builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IWeddingDayService, WeddingDayService>();
builder.Services.AddScoped<IWeddingItemService, WeddingItemService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<DatabaseSeeder>();
builder.Services.AddControllers();

var corsOrigins = builder.Configuration["CORS_ORIGINS"] ?? "http://localhost:5173,http://localhost:3000";
var origins = corsOrigins.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
if (origins.Length == 0)
    origins = new[] { "http://localhost:5173", "http://localhost:3000" };

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        // Supports exact origins AND wildcard host patterns like:
        // - https://*.vercel.app
        // - https://our-big-day-*.vercel.app
        // - http://localhost:5173
        policy.SetIsOriginAllowed(origin => IsAllowedOrigin(origin, origins))
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

static bool IsAllowedOrigin(string origin, string[] allowedOrigins)
{
    if (string.IsNullOrWhiteSpace(origin)) return false;
    origin = origin.Trim().TrimEnd('/');

    if (!Uri.TryCreate(origin, UriKind.Absolute, out var originUri))
        return false;

    foreach (var allowed in allowedOrigins)
    {
        if (string.IsNullOrWhiteSpace(allowed)) continue;

        var allowedTrimmed = allowed.Trim().TrimEnd('/');
        if (allowedTrimmed == "*")
            return true;

        // Exact origin match (common case)
        if (!allowedTrimmed.Contains('*') &&
            string.Equals(allowedTrimmed, origin, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        // Wildcard host pattern match
        if (allowedTrimmed.Contains('*') && WildcardOriginMatches(originUri, allowedTrimmed))
            return true;
    }

    return false;
}

static bool WildcardOriginMatches(Uri originUri, string allowedPattern)
{
    // allowedPattern examples:
    // - https://*.vercel.app
    // - https://our-big-day-*.vercel.app
    // - *.vercel.app
    // - localhost:* (not recommended but supported if explicitly configured)

    string? requiredScheme = null;
    string hostAndPortPattern = allowedPattern;

    var schemeSepIdx = allowedPattern.IndexOf("://", StringComparison.Ordinal);
    if (schemeSepIdx >= 0)
    {
        requiredScheme = allowedPattern[..schemeSepIdx];
        hostAndPortPattern = allowedPattern[(schemeSepIdx + 3)..];
    }

    // Drop any path if present (we only care about the origin host[:port])
    var slashIdx = hostAndPortPattern.IndexOf('/', StringComparison.Ordinal);
    if (slashIdx >= 0)
        hostAndPortPattern = hostAndPortPattern[..slashIdx];

    if (!string.IsNullOrWhiteSpace(requiredScheme) &&
        !string.Equals(originUri.Scheme, requiredScheme, StringComparison.OrdinalIgnoreCase))
    {
        return false;
    }

    // Split host:port pattern (port optional)
    var hostPattern = hostAndPortPattern;
    var portPattern = "*";

    var lastColonIdx = hostAndPortPattern.LastIndexOf(':');
    if (lastColonIdx >= 0 && lastColonIdx < hostAndPortPattern.Length - 1)
    {
        hostPattern = hostAndPortPattern[..lastColonIdx];
        portPattern = hostAndPortPattern[(lastColonIdx + 1)..];
    }

    if (!WildcardMatch(originUri.Host, hostPattern))
        return false;

    if (portPattern != "*" && int.TryParse(portPattern, out var requiredPort))
        return originUri.Port == requiredPort;

    return true;
}

static bool WildcardMatch(string input, string pattern)
{
    // Case-insensitive wildcard match where '*' matches any substring
    if (string.IsNullOrWhiteSpace(pattern)) return false;
    var regex = "^" + Regex.Escape(pattern).Replace("\\*", ".*") + "$";
    return Regex.IsMatch(input, regex, RegexOptions.IgnoreCase);
}

static bool LooksLikePostgresConnectionString(string connectionString)
{
    if (connectionString.Contains("Host=", StringComparison.OrdinalIgnoreCase)) return true;
    if (connectionString.Contains("Username=", StringComparison.OrdinalIgnoreCase)) return true;
    if (connectionString.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase)) return true;
    if (connectionString.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase)) return true;
    return false;
}

static string BuildNpgsqlConnectionStringFromDatabaseUrl(string databaseUrl)
{
    // Converts postgresql://user:pass@host:5432/db?sslmode=require&channel_binding=require
    // into Npgsql key=value format. NpgsqlConnectionStringBuilder (used by EF Core internally)
    // does not accept raw postgresql:// URIs, so we must parse and rebuild.
    // channel_binding is intentionally ignored — not a valid Npgsql 8.x keyword; Neon still
    // accepts standard SCRAM-SHA-256 without it.
    if (!Uri.TryCreate(databaseUrl, UriKind.Absolute, out var uri))
        throw new InvalidOperationException("DATABASE_URL is not a valid absolute URI.");

    if (!string.Equals(uri.Scheme, "postgres", StringComparison.OrdinalIgnoreCase) &&
        !string.Equals(uri.Scheme, "postgresql", StringComparison.OrdinalIgnoreCase))
        throw new InvalidOperationException("DATABASE_URL must start with postgres:// or postgresql://");

    var userInfoParts = (uri.UserInfo ?? "").Split(':', 2);
    var username = userInfoParts.Length > 0 ? Uri.UnescapeDataString(userInfoParts[0]) : "";
    var password = userInfoParts.Length > 1 ? Uri.UnescapeDataString(userInfoParts[1]) : "";
    var database = uri.AbsolutePath.Trim('/');

    var sslMode = SslMode.Prefer;
    var query = uri.Query.TrimStart('?');
    if (!string.IsNullOrWhiteSpace(query))
    {
        foreach (var pair in query.Split('&', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var kv = pair.Split('=', 2);
            if (kv.Length != 2) continue;
            var key = Uri.UnescapeDataString(kv[0]);
            var value = Uri.UnescapeDataString(kv[1]);
            if (key.Equals("sslmode", StringComparison.OrdinalIgnoreCase))
            {
                sslMode = value.ToLowerInvariant() switch
                {
                    "disable"     => SslMode.Disable,
                    "prefer"      => SslMode.Prefer,
                    "require"     => SslMode.Require,
                    "verify-ca"   => SslMode.VerifyCA,
                    "verify-full" => SslMode.VerifyFull,
                    _             => sslMode
                };
            }
            // channel_binding is skipped — not supported as an Npgsql 8.x connection string keyword
        }
    }

    return new NpgsqlConnectionStringBuilder
    {
        Host     = uri.Host,
        Port     = uri.Port > 0 ? uri.Port : 5432,
        Username = username,
        Password = password,
        Database = database,
        SslMode  = sslMode,
    }.ConnectionString;
}

var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrEmpty(port) && int.TryParse(port, out var portNum))
    builder.WebHost.UseUrls($"http://0.0.0.0:{portNum}");

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
    await seeder.SeedAsync();
}

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
