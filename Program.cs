using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OlimpBack.Data;
using OlimpBack.MappingProfiles;
using OlimpBack.Services;
using OlimpBack.Utils;
using System.Configuration;
using System.Diagnostics;
using System.Text;
using System.Text.Json;


Environment.SetEnvironmentVariable(
    "ASPNETCORE_ENVIRONMENT",
    "Development"
);

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpClient();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Olimp API", Version = "v1" });
    
    // Add JWT Authentication to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var cs = builder.Configuration.GetConnectionString("DefaultConnection");

// Database connection
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MariaDbServerVersion(new Version(10, 6, 12)),
        mySqlOptions =>
        {
            mySqlOptions.EnableRetryOnFailure(
                maxRetryCount: 3,
                maxRetryDelay: TimeSpan.FromSeconds(5),
                errorNumbersToAdd: null
            );
        }
    );
});


// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddSwaggerGen(c =>
{
    c.SupportNonNullableReferenceTypes();
});


// JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // Set to true in production with HTTPS
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not found in configuration")))
    };

    options.Events = new JwtBearerEvents
    {
        // 1️⃣ Ошибка аутентификации (битый / просроченный токен)
        OnAuthenticationFailed = context =>
        {
            var logger = context.HttpContext.RequestServices
                .GetRequiredService<ILogger<Program>>();

            logger.LogError(context.Exception, "Authentication failed");

            if (context.Exception is SecurityTokenExpiredException)
            {
                context.Response.Headers.Add("Token-Expired", "true");
            }

            return Task.CompletedTask;
        },

        // 2️⃣ Нет токена или он невалиден → 401
        OnChallenge = async context =>
        {
            // ❗ ОБЯЗАТЕЛЬНО
            context.HandleResponse();

            var logger = context.HttpContext.RequestServices
                .GetRequiredService<ILogger<Program>>();

            logger.LogWarning(
                "Authentication challenge: {Error} {Description}",
                context.Error,
                context.ErrorDescription
            );

            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";

            var response = new
            {
                code = "unauthorized",
                message = "Access denied"
            };

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(response)
            );
        },

        // 3️⃣ Токен валиден, но нет прав → 403
        OnForbidden = async context =>
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "application/json";

            var response = new
            {
                code = "forbidden",
                message = "Insufficient rights"
            };

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(response)
            );
        },

        // 4️⃣ Успешная проверка токена
        OnTokenValidated = context =>
        {
            var logger = context.HttpContext.RequestServices
                .GetRequiredService<ILogger<Program>>();

            logger.LogInformation("Token validated successfully");

            return Task.CompletedTask;
        }
    };
});


// Add JWT Service
builder.Services.AddScoped<JwtService>();

// Auth services
builder.Services.AddScoped<IAuthAppService, AuthAppService>();

// Educational program services
builder.Services.AddScoped<IEducationalProgramService, EducationalProgramService>();

// Group services
builder.Services.AddScoped<IGroupService, GroupService>();

// Faculty services
builder.Services.AddScoped<IFacultyService, FacultyService>();

// Notification services
builder.Services.AddScoped<INotificationService, NotificationService>();

// Student services
builder.Services.AddScoped<IStudentService, StudentService>();

// Department services
builder.Services.AddScoped<IDepartmentService, DepartmentService>();

// BindLoansMain services
builder.Services.AddScoped<IBindLoansMainService, BindLoansMainService>();

// Discipline tab services
builder.Services.AddScoped<IDisciplineTabAdminService, DisciplineTabAdminService>();
builder.Services.AddScoped<IDisciplineTabService, DisciplineTabService>();

// Authorization
builder.Services.AddAuthorization();
builder.Services.AddHostedService<FileCleanupService>();
builder.Services.AddHostedService<DatabaseAvailabilityService>();


var app = builder.Build();

var env = builder.Environment.EnvironmentName;
Console.WriteLine(env);


//Process.Start(new ProcessStartInfo
//{
//    FileName = Path.Combine(AppContext.BaseDirectory, "start_tunnel.exe"),
//    UseShellExecute = true,
//    WindowStyle = ProcessWindowStyle.Hidden
//});



app.Urls.Clear();
app.Urls.Add("http://localhost:5154");
app.Urls.Add("https://localhost:7011");

app.Logger.LogWarning($"Environment: {app.Environment.EnvironmentName}");

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Olimp API v1");
});

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
