
using ChurchAttendance.API.Middleware;
using ChurchAttendance.API.Seed;
using ChurchAttendance.API.Swagger;
using ChurchAttendance.API.Validators;
using ChurchAttendance.Core.Domain.Entities;
using ChurchAttendance.Core.Domain.RepositoryContracts;
using ChurchAttendance.Core.Domain.RepositoryContracts.V1;
using ChurchAttendance.Core.Options;
using ChurchAttendance.Core.ServiceContracts;
using ChurchAttendance.Core.ServiceContracts.V1;
using ChurchAttendance.Core.Services;
using ChurchAttendance.Core.Services.V1;
using ChurchAttendance.Infrastructure;
using ChurchAttendance.Infrastructure.Repositories;
using ChurchAttendance.Infrastructure.Repositories.V1;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Controllers: require authentication by default.
// Actions marked [AllowAnonymous] remain public.
builder.Services.AddControllers(options =>
{
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();

    options.Filters.Add(new AuthorizeFilter(policy));
});

builder.Services.AddAuthorization();

// Database
var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "Connection string 'DefaultConnection' is missing.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// Options
builder.Services.Configure<JWTSettings>(
    builder.Configuration.GetSection("JWTSettings"));

builder.Services.Configure<RefreshTokenOptions>(
    builder.Configuration.GetSection("RefreshToken"));

// Identity
builder.Services
    .AddIdentity<ApplicationUser, ApplicationRole>(options =>
    {
        options.Password.RequiredLength = 8;
        options.Password.RequireUppercase = false;
        options.Password.RequireDigit = true;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// Legacy services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAttendanceDayService, AttendanceDayService>();
builder.Services.AddScoped<IAttendanceDayRepository, AttendanceDayRepository>();
builder.Services.AddScoped<IAttendanceService, AttendanceService>();
builder.Services.AddScoped<IAttendanceRepository, AttendanceRepository>();
builder.Services.AddTransient<IJWTService, JWTService>();

// V1 repositories
builder.Services.AddScoped<IMemberRepository, MemberRepository>();
builder.Services.AddScoped<IV1AttendanceDayRepository, V1AttendanceDayRepository>();
builder.Services.AddScoped<IV1AttendanceRepository, V1AttendanceRepository>();
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<ISettingsRepository, SettingsRepository>();
builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();
builder.Services.AddScoped<ILevelRepository, LevelRepository>();

// V1 services
builder.Services.AddScoped<IMemberService, MemberService>();
builder.Services.AddScoped<IV1AttendanceDayService, V1AttendanceDayService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<ISettingsService, SettingsService>();
builder.Services.AddScoped<ILevelService, LevelService>();
builder.Services.AddScoped<IAuditService, AuditService>();
builder.Services.AddScoped<IV1AuthService, V1AuthService>();

// Current user / HTTP context
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

// JWT authentication
var jwtSettings = builder.Configuration
    .GetSection("JWTSettings")
    .Get<JWTSettings>()
    ?? throw new InvalidOperationException(
        "JWTSettings configuration is missing.");

if (string.IsNullOrWhiteSpace(jwtSettings.Key))
{
    throw new InvalidOperationException(
        "JWTSettings:Key must be configured.");
}

if (Encoding.UTF8.GetByteCount(jwtSettings.Key) < 32)
{
    throw new InvalidOperationException(
        "JWTSettings:Key must contain at least 32 bytes.");
}

if (string.IsNullOrWhiteSpace(jwtSettings.Issuer) ||
    string.IsNullOrWhiteSpace(jwtSettings.Audience))
{
    throw new InvalidOperationException(
        "JWTSettings:Issuer and JWTSettings:Audience must be configured.");
}

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme =
            JwtBearerDefaults.AuthenticationScheme;

        options.DefaultChallengeScheme =
            JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,

            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,

            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings.Key)),

            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });

// FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<
    AddAttendanceDayDTOValidator>();

// CORS
var allowedOrigins = builder.Configuration
    .GetSection("AllowedOrigins")
    .Get<string[]>() ?? Array.Empty<string>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("EcclisiyaFrontend", policy =>
    {
        if (allowedOrigins.Length > 0)
        {
            policy
                .WithOrigins(allowedOrigins)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        }
        else
        {
            // Development fallback only.
            // Do not enable credentials with AllowAnyOrigin().
            policy
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        }
    });
});

// Exception handling
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ApiExceptionHandler>();

// Swagger
builder.Services.AddEndpointsApiExplorer();


builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ChurchAttendance API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Enter your JWT access token."
        });

    options.AddSecurityRequirement(document =>
        new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", document)] =
                new List<string>()
        });
});


var app = builder.Build();

// Seed roles and initial administrator
using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider
        .GetRequiredService<ILogger<Program>>();

    try
    {
        await IdentitySeeder.SeedAsync(
            scope.ServiceProvider,
            app.Configuration,
            logger);
    }
    catch (Exception ex)
    {
        // Keeps the existing behavior. See the note below
        // about whether startup should continue after seeding fails.
        logger.LogError(ex, "Error during identity seeding.");
    }
}

// Middleware pipeline
app.UseExceptionHandler();

app.UseSwagger();

app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint(
        "/swagger/v1/swagger.json",
        "ChurchAttendance API v1");

    options.RoutePrefix = "openapi/ui";
});

app.UseHttpsRedirection();

app.UseCors("EcclisiyaFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
