using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OlimpBack.Application.Permissions;
using OlimpBack.Application.Services;
using OlimpBack.Infrastructure.Database;
using OlimpBack.Infrastructure.Database.Repositories;
using OlimpBack.MappingProfiles;
using OlimpBack.Utils;
// TEMPORARY: Redis is disabled for local development until Redis is available.
// using OlimpBack.Infrastructure.Redis;
// using StackExchange.Redis;
using System.Configuration;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using OlimpBack.Data;
using FluentValidation;
using FluentValidation.AspNetCore;
using OlimpBack.Infrastructure.Repositories.Conversations;
using OlimpBack.Application.Services.Conversations;
using OlimpBack.Application.Validators.Conversations;
using OlimpBack.Infrastructure.Repositories.Messages;
using OlimpBack.Application.Services.Messages;
using OlimpBack.Application.Validators.Messages;
using OlimpBack.Infrastructure.Repositories.Encryption;
using OlimpBack.Application.Services.Encryption;
using OlimpBack.Application.Validators.Encryption;
using OlimpBack.Infrastructure.Security;
using OlimpBack.Infrastructure.Middleware;
using OlimpBack.Hubs;
using OlimpBack.Infrastructure.Realtime;

Environment.SetEnvironmentVariable(
    "ASPNETCORE_ENVIRONMENT",
    "Development"
);

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://0.0.0.0:5154");


// Add services
builder.Services.AddControllers()
    .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<CreateConversationRequestValidator>());
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpClient();
builder.Services.AddMemoryCache();

// Security Hardening Services
builder.Services.AddScoped<IReplayProtectionService, ReplayProtectionService>();
builder.Services.AddScoped<IRateLimitService, RateLimitService>();
builder.Services.AddScoped<IDeviceTrustService, DeviceTrustService>();
builder.Services.AddScoped<IConversationAccessService, ConversationAccessService>();

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
    options.UseNpgsql(cs, npgsqlOptions =>
    {
        // Налаштування стійкості з'єднання (корисно для нестабільних тунелів)
        npgsqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorCodesToAdd: null
        );
    }));


// AutoMapper
builder.Services.AddAutoMapper(cfg => {
    cfg.AddProfile<MappingProfile>();
    cfg.AddProfile<ConversationProfile>();
    cfg.AddProfile<MessageProfile>();
    cfg.AddProfile<EncryptionProfile>();
});
// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:5173") // Example frontend URLs
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddSignalR();
builder.Services.AddSingleton<ISignalRConnectionManager, SignalRConnectionManager>();

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
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        },
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
builder.Services.AddScoped<IRoleMaskService, RoleMaskService>();
// TEMPORARY: Redis is disabled for local development until Redis is available.
// builder.Services.AddSingleton<IConnectionMultiplexer>(_ =>
// {
//     var redisConnection = builder.Configuration["Redis:ConnectionString"] ?? "localhost:6379,abortConnect=false";
//     return ConnectionMultiplexer.Connect(redisConnection);
// });
// builder.Services.AddSingleton<IRbacCacheService, RbacCacheService>();
// ==========================================
// РЕПОЗИТОРІЇ ТА СЕРВІСИ (Domain Modules)
// ==========================================

// Auth
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IAuthAppService, AuthAppService>();

// BindLoansMain
builder.Services.AddScoped<IBindLoansMainRepository, BindLoansMainRepository>();
builder.Services.AddScoped<IBindLoansMainService, BindLoansMainService>();

// MainDiscipline
builder.Services.AddScoped<IMainDisciplineRepository, MainDisciplineRepository>();
builder.Services.AddScoped<IMainDisciplineService, MainDisciplineService>();

// BindRolePermission
builder.Services.AddScoped<IBindRolePermissionRepository, BindRolePermissionRepository>();
builder.Services.AddScoped<IBindRolePermissionService, BindRolePermissionService>();

// Department
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();

// CatalogYearMain
builder.Services.AddScoped<ICatalogYearMainRepository, CatalogYearMainRepository>();
builder.Services.AddScoped<ICatalogYearMainService, CatalogYearMainService>();

// CatalogYearSelective
builder.Services.AddScoped<ICatalogYearSelectiveRepository, CatalogYearSelectiveRepository>();
builder.Services.AddScoped<ICatalogYearSelectiveService, CatalogYearSelectiveService>();

// DisciplineChoicePeriod
builder.Services.AddScoped<IDisciplineChoicePeriodRepository, DisciplineChoicePeriodRepository>();
builder.Services.AddScoped<IDisciplineChoicePeriodService, DisciplineChoicePeriodService>();

// DisciplineTabAdmin
builder.Services.AddScoped<IAdminDisciplineStudentListRepository, AdminDisciplineStudentListRepository>();
builder.Services.AddScoped<IDisciplineTabAdminRepository, DisciplineTabAdminRepository>();
builder.Services.AddScoped<IDisciplineTabAdminService, DisciplineTabAdminService>();

// DisciplineTab
builder.Services.AddScoped<IDisciplineTabRepository, DisciplineTabRepository>();
builder.Services.AddScoped<IDisciplineTabService, DisciplineTabService>();

// EducationalDegree
builder.Services.AddScoped<IEducationalDegreeRepository, EducationalDegreeRepository>();
builder.Services.AddScoped<IEducationalDegreeService, EducationalDegreeService>();

// EducationalProgram
builder.Services.AddScoped<IEducationalProgramRepository, EducationalProgramRepository>();
builder.Services.AddScoped<IEducationalProgramService, EducationalProgramService>();

// EducationStatus
builder.Services.AddScoped<IEducationStatusRepository, EducationStatusRepository>();
builder.Services.AddScoped<IEducationStatusService, EducationStatusService>();

// Faculty
builder.Services.AddScoped<IFacultyRepository, FacultyRepository>();
builder.Services.AddScoped<IFacultyService, FacultyService>();

// Filter
builder.Services.AddScoped<IFilterRepository, FilterRepository>();
builder.Services.AddScoped<IFilterService, FilterService>();

// Group
builder.Services.AddScoped<IGroupRepository, GroupRepository>();
builder.Services.AddScoped<IGroupService, GroupService>();

// StudentPage
builder.Services.AddScoped<IStudentPageRepository, StudentPageRepository>();
builder.Services.AddScoped<IStudentPageService, StudentPageService>();

// StudyForm
builder.Services.AddScoped<IStudyFormRepository, StudyFormRepository>();
builder.Services.AddScoped<IStudyFormService, StudyFormService>();


// Rating
builder.Services.AddScoped<IRatingRepository, RatingRepository>();
builder.Services.AddScoped<IRatingService, RatingService>();

builder.Services.AddScoped<INotificationTemplateRepository, NotificationTemplateRepository>();
builder.Services.AddScoped<INotificationTemplateService, NotificationTemplateService>();

// Normative
builder.Services.AddScoped<INormativeRepository, NormativeRepository>();
builder.Services.AddScoped<INormativeService, NormativeService>();

// TypeOfDiscipline
builder.Services.AddScoped<ITypeOfDisciplineRepository, TypeOfDisciplineRepository>();
builder.Services.AddScoped<ITypeOfDisciplineService, TypeOfDisciplineService>();

// Grade
builder.Services.AddScoped<IGradeRepository, GradeRepository>();
builder.Services.AddScoped<IGradeService, GradeService>();

// Conversations
builder.Services.AddScoped<IConversationRepository, ConversationRepository>();
builder.Services.AddScoped<IConversationService, ConversationService>();
builder.Services.AddScoped<IAnonymousConversationService, AnonymousConversationService>();

// Messages
builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddScoped<IMessageService, MessageService>();

// Encryption
builder.Services.AddScoped<IUserDeviceRepository, UserDeviceRepository>();
builder.Services.AddScoped<IPreKeyRepository, PreKeyRepository>();
builder.Services.AddScoped<IEncryptionSessionService, EncryptionSessionService>();

// Device Transfer
builder.Services.AddScoped<IDeviceTransferRepository, DeviceTransferRepository>();
builder.Services.AddScoped<IDeviceTransferService, DeviceTransferService>();
builder.Services.AddHostedService<DeviceTransferCleanupService>();

// ==========================================
// СЕРВІСИ БЕЗ РЕПОЗИТОРІЇВ (Поки що)
// ==========================================

// Notification
builder.Services.AddScoped<INotificationService, NotificationService>();

// Student
builder.Services.AddScoped<IStudentService, StudentService>();

// Import and AI Services
builder.Services.AddScoped<IWordProcessingService, WordProcessingService>();
builder.Services.AddScoped<IGeminiService, GeminiService>();
builder.Services.AddScoped<IImportService, ImportService>();

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
//app.Urls.Add("http://localhost:5154");
//app.Urls.Add("https://localhost:7011");

app.Logger.LogWarning($"Environment: {app.Environment.EnvironmentName}");

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Olimp API v1");
});

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.UseRouting();
app.UseMiddleware<SecurityHardeningMiddleware>();

app.UseAuthentication();
app.UseMiddleware<PermissionMiddleware>();
app.UseAuthorization();

app.MapControllers();
app.MapHub<EncryptedChatHub>("/hubs/chat");

app.Run();
