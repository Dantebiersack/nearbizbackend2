using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using nearbizbackend.Data;
using Npgsql;
using System.Net;
using System.Net.Sockets;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ====== JWT CONFIG ======
var jwtSection = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSection["Key"] ?? "DEFAULT_DEV_KEY");

// ====== EF CORE (Postgres / Supabase) ======
builder.Services.AddDbContext<NearBizDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Permitir comportamiento legacy de timestamps (opcional)
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// ====== CORS ======
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

// ====== AUTENTICACIÓN JWT ======
builder.Services
    .AddAuthentication(options =>
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
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = jwtSection["Issuer"],
            ValidAudience = jwtSection["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// ====== CONTROLLERS ======
builder.Services.AddControllers().AddJsonOptions(o =>
{
    o.JsonSerializerOptions.PropertyNamingPolicy = null;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ====== BUILD APP ======
var app = builder.Build();

// --- Endpoint raíz para probar la API ---
app.MapGet("/", () => Results.Ok("NearBiz API online"));

app.MapGet("/api/health/net", async () =>
{
    try
    {
        var cs = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");
        var sb = new NpgsqlConnectionStringBuilder(cs);
        var addrs = await Dns.GetHostAddressesAsync(sb.Host);
        foreach (var ip in addrs)
        {
            try
            {
                using var tcp = new TcpClient(AddressFamily.InterNetwork);
                await tcp.ConnectAsync(ip, sb.Port);
                return Results.Ok(new { host = sb.Host, ip = ip.ToString(), ok = true });
            }
            catch (Exception ex)
            {
                // sigue probando la siguiente IP
            }
        }
        return Results.Problem($"No pude conectar a {sb.Host}:{sb.Port} por IPv4/IPv6.");
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.ToString());
    }
});


// --- Swagger SIEMPRE activo (útil en Render) ---
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
