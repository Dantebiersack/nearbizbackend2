using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using nearbizbackend.Data;
using nearbizbackend.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ========= JWT =========
var jwtSection = builder.Configuration.GetSection("Jwt");
var keyString = jwtSection["Key"]
    ?? throw new InvalidOperationException("Missing Jwt:Key (configura Jwt__Key en variables de entorno).");
var key = Encoding.UTF8.GetBytes(keyString);

// ========= EF Core (Postgres / Supabase) =========
builder.Services.AddDbContext<NearBizDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
       .EnableDetailedErrors()
       .EnableSensitiveDataLogging()); // quítalo en prod si no quieres logs detallados

// Opcional compat de timestamps
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// ========= CORS =========
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", p => p
        .AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod());
});

// ========= Auth / JWT =========
builder.Services.AddAuthentication(o =>
{
    o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(o =>
{
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        ValidIssuer = jwtSection["Issuer"],
        ValidAudience = jwtSection["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// ========= Controllers / Swagger =========
builder.Services.AddControllers().AddJsonOptions(o =>
{
    o.JsonSerializerOptions.PropertyNamingPolicy = null;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ========= Endpoints de health / ping =========
app.MapGet("/", () => Results.Ok("NearBiz API online"));

app.MapGet("/api/health/db", async (NearBizDbContext db) =>
{
    try
    {
        var ok = await db.Database.CanConnectAsync();
        var n = await db.Set<Usuario>().CountAsync();
        return Results.Ok(new { canConnect = ok, usuarios = n });
    }
    catch (Exception ex)
    {
        return Results.Problem(detail: ex.Message, statusCode: 500);
    }
});

app.MapGet("/api/health/jwt", (IConfiguration cfg) =>
{
    var jwt = cfg.GetSection("Jwt");
    var hasKey = !string.IsNullOrWhiteSpace(jwt["Key"]);
    return Results.Ok(new { hasKey, issuer = jwt["Issuer"], audience = jwt["Audience"] });
});

// ========= Middlewares (orden correcto para Render) =========
// NO usar app.UseHttpsRedirection() en Render (el proxy ya maneja HTTPS)
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowAll");     // CORS antes de Auth
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
