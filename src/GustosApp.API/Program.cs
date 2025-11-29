using FirebaseAdmin;
using FluentValidation;
using FluentValidation.AspNetCore;
using Google.Api;
using Google.Apis.Auth.OAuth2;
using GustosApp.API.Hubs;
using GustosApp.API.Hubs.GustosApp.API.Hubs;
using GustosApp.API.Hubs.Services;
using GustosApp.API.Mapping;
using GustosApp.API.Middleware;
using GustosApp.API.Templates.Email;
using GustosApp.API.Validations.OpinionRestaurantes;
using GustosApp.Application.Handlers;
using GustosApp.Application.Interfaces;
using GustosApp.Application.Services;
using GustosApp.Application.UseCases;
using GustosApp.Application.UseCases.AmistadUseCases;
using GustosApp.Application.UseCases.GrupoUseCases;
using GustosApp.Application.UseCases.GrupoUseCases.ChatGrupoUseCases;
using GustosApp.Application.UseCases.GrupoUseCases.InvitacionGrupoUseCases;
using GustosApp.Application.UseCases.NotificacionUseCases;
using GustosApp.Application.UseCases.RestauranteUseCases;
using GustosApp.Application.UseCases.RestauranteUseCases.OpinionesRestaurantes;
using GustosApp.Application.UseCases.RestauranteUseCases.SolicitudRestauranteUseCases;
using GustosApp.Application.UseCases.UsuarioUseCases;
using GustosApp.Application.UseCases.UsuarioUseCases.CondicionesMedicasUseCases;
using GustosApp.Application.UseCases.UsuarioUseCases.GustoUseCases;
using GustosApp.Application.UseCases.UsuarioUseCases.RestriccionesUseCases;
using GustosApp.Application.UseCases.VotacionUseCases;
using GustosApp.Application.Validations.Restaurantes;
using GustosApp.Domain.Interfaces;
using GustosApp.Infraestructure;
using GustosApp.Infraestructure.Files;
using GustosApp.Infraestructure.ML;
using GustosApp.Infraestructure.Ocr;
using GustosApp.Infraestructure.Parsing;
using GustosApp.Infraestructure.Repositories;
using GustosApp.Infraestructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using GustosApp.Application.Services;
using GustosApp.Application.UseCases.GrupoUseCases;
using GustosApp.Application.UseCases.GrupoUseCases.InvitacionGrupoUseCases;
using GustosApp.Application.UseCases.AmistadUseCases;
using GustosApp.Application.UseCases.UsuarioUseCases;
using GustosApp.Application.UseCases.GrupoUseCases.ChatGrupoUseCases;
using GustosApp.Application.UseCases.NotificacionUseCases;
using GustosApp.Application.UseCases.UsuarioUseCases.GustoUseCases;
using GustosApp.Application.UseCases.UsuarioUseCases.CondicionesMedicasUseCases;
using GustosApp.Application.UseCases.UsuarioUseCases.RestriccionesUseCases;
using GustosApp.Application.UseCases.VotacionUseCases;
using GustosApp.Infraestructure.Services;
using Microsoft.AspNetCore.Authorization;
using GustosApp.Application.UseCases.RestauranteUseCases.SolicitudRestauranteUseCases;
using GustosApp.API.Templates.Email;
using GustosApp.Application.Validations.Restaurantes;
using FluentValidation.AspNetCore;
using GustosApp.API.Validations.OpinionRestaurantes;
using GustosApp.Application.UseCases.RestauranteUseCases.OpinionesRestaurantes;
using System.Globalization;



var builder = WebApplication.CreateBuilder(args);


// =====================
//   Firebase / Auth
// =====================

// Preparamos logger REAL vía DI
var firebaseLogger = builder.Services
    .BuildServiceProvider()
    .GetRequiredService<ILoggerFactory>()
    .CreateLogger("Firebase");

// Intentamos obtener JSON desde Azure (producción)
var firebaseJson = builder.Configuration["FIREBASE_SERVICE_ACCOUNT_JSON"];

// Project ID con fallback local
var firebaseProjectId =
    builder.Configuration["FIREBASE_PROJECTID"]
    ?? "gustosapp-5c3c9";

// Ruta local para desarrollo
var localFirebasePath = Path.Combine(
    builder.Environment.ContentRootPath,
    "secrets",
    "firebase-key.json"
);

firebaseLogger.LogInformation(
    "🔍 Iniciando configuración de Firebase (Entorno: {Env})",
    builder.Environment.EnvironmentName
);

if (FirebaseApp.DefaultInstance == null)
{
    try
    {
        if (!string.IsNullOrWhiteSpace(firebaseJson))
        {
            firebaseLogger.LogInformation("🔥 Inicializando Firebase desde JSON (PRODUCCIÓN)");

            FirebaseApp.Create(new AppOptions
            {
                Credential = GoogleCredential.FromJson(firebaseJson)
            });
        }
        else if (File.Exists(localFirebasePath))
        {
            firebaseLogger.LogInformation("💻 Inicializando Firebase desde archivo local: {Path}",
                localFirebasePath);

            FirebaseApp.Create(new AppOptions
            {
                Credential = GoogleCredential.FromFile(localFirebasePath)
            });
        }
        else
        {
            firebaseLogger.LogError("❌ No se encontró ninguna credencial de Firebase (ni JSON ni archivo)");
            throw new Exception("No se pueden cargar credenciales Firebase.");
        }

        firebaseLogger.LogInformation("✅ Firebase inicializado correctamente.");
    }
    catch (Exception ex)
    {
        firebaseLogger.LogError(ex, "❌ Error inicializando Firebase");
        throw;
    }
}



// =====================
//   JWT Bearer
// =====================

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = $"https://securetoken.google.com/{firebaseProjectId}";

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = $"https://securetoken.google.com/{firebaseProjectId}",
            ValidateAudience = true,
            ValidAudience = firebaseProjectId,
            ValidateLifetime = true
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var logger = context.HttpContext.RequestServices
                    .GetRequiredService<ILoggerFactory>()
                    .CreateLogger("JWT");

                logger.LogInformation("📥 Request a: {Path}", context.Request.Path);

                // Prioridad 1: Cookie
                if (context.Request.Cookies.TryGetValue("token", out var raw))
                {
                    logger.LogInformation("🍪 Token encontrado en cookie.");
                    context.Token = raw;
                    return Task.CompletedTask;
                }

                // Prioridad 2: Query (SignalR)
                var accessToken = context.Request.Query["access_token"];
                if (!string.IsNullOrEmpty(accessToken))
                {
                    logger.LogInformation("🔗 Token encontrado en QueryString (SignalR)");
                    context.Token = accessToken;
                    return Task.CompletedTask;
                }

                // Prioridad 3: Header Authorization
                var authHeader = context.Request.Headers["Authorization"].ToString();
                if (!string.IsNullOrEmpty(authHeader) &&
                    authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    context.Token = authHeader.Substring("Bearer ".Length).Trim();
                    return Task.CompletedTask;
                }

                logger.LogWarning("⚠️ No se recibió token por Cookie, QueryString ni Header.");
                return Task.CompletedTask;
            },

            OnTokenValidated = context =>
            {
                var logger = context.HttpContext.RequestServices
                    .GetRequiredService<ILoggerFactory>()
                    .CreateLogger("JWT");

                logger.LogInformation("✅ Token validado correctamente");

                return Task.CompletedTask;
            },

            OnAuthenticationFailed = context =>
            {
                var logger = context.HttpContext.RequestServices
                    .GetRequiredService<ILoggerFactory>()
                    .CreateLogger("JWT");

                logger.LogError(context.Exception,
                    "❌ Error en autenticación JWT: {Error}",
                    context.Exception.Message);

                return Task.CompletedTask;
            }
        };
    });

// =====================
//      REDIS
// =====================

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var logger = sp.GetRequiredService<ILogger<IConnectionMultiplexer>>();
    var redisConfig = builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379";

    logger.LogInformation("🚀 Conectando a Redis con: {RedisConnectionString}", redisConfig);

    return ConnectionMultiplexer.Connect(redisConfig);
});



builder.Services.AddSingleton<IFileStorageService>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var logger = sp.GetRequiredService<ILogger<FirebaseStorageService>>();

    return new FirebaseStorageService(
        firebaseJson,
        localFirebasePath,
        config,
        logger
    );
});


builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());


builder.Services.AddScoped<IEmbeddingService>(sp =>
{
    var env = sp.GetRequiredService<IWebHostEnvironment>();
    var modelPath = Path.Combine(AppContext.BaseDirectory, "ML", "model.onnx");
    var tokPath = Path.Combine(AppContext.BaseDirectory, "ML", "tokenizer.json");
    return new OnnxEmbeddingService(modelPath, tokPath);
});

// Autorización explícita 
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy =>
    {
        policy.RequireClaim("rol", "Admin");
    });

    options.AddPolicy("DuenoRestaurante", policy =>
    {
        policy.RequireClaim("rol", "DuenoRestaurante");
    });

    options.AddPolicy("PendienteRestaurante", policy =>
    {
        policy.RequireClaim("rol", "PendienteRestaurante");
    });
});


builder.Services.AddSingleton<IOcrService>(sp =>
{
    var env = sp.GetRequiredService<IWebHostEnvironment>();
    var logger = sp.GetRequiredService<ILogger<IOcrService>>();

    logger.LogInformation("Inicializando Google Vision OCR...");

    // 1) PRODUCCIÓN VIA BASE64
    var base64 = Environment.GetEnvironmentVariable("GOOGLE_VISION_KEY_BASE64");

    if (!string.IsNullOrWhiteSpace(base64))
    {
        try
        {
            logger.LogInformation("Usando GOOGLE_VISION_KEY_BASE64...");

            var bytes = Convert.FromBase64String(base64);
            var jsonString = System.Text.Encoding.UTF8.GetString(bytes);

            logger.LogInformation("Credencial Base64 decodificada correctamente.");

            return new GoogleVisionOcrService(jsonString);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error decodificando GOOGLE_VISION_KEY_BASE64.");
        }
    }
    else
    {
        logger.LogWarning("GOOGLE_VISION_KEY_BASE64 no está configurado.");
    }

    // 2) LOCAL VIA ARCHIVO
    var localPath = Path.Combine(env.ContentRootPath, "secrets", "google-vision.json");
    logger.LogInformation("Intentando cargar credencial local: {Path}", localPath);

    if (!File.Exists(localPath))
    {
        logger.LogError("No existe google-vision.json en: {Path}", localPath);
        throw new FileNotFoundException("No se encontró google-vision.json en /secrets", localPath);
    }

    var jsonFile = File.ReadAllText(localPath);
    logger.LogInformation("Archivo local cargado correctamente.");

    return new GoogleVisionOcrService(jsonFile);
});



builder.Services.AddScoped<IMenuParser, SimpleMenuParser>();

//Autorizacion para acceder a ciertas rutas si el registro del usuario no esta completo
builder.Services.AddAuthorization(opt =>
{
    opt.AddPolicy("RegistroIncompleto", p =>
        p.Requirements.Add(new RegistroIncompletoRequirement()));
});


builder.Services
    .AddFluentValidationAutoValidation()
    .AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<CrearSolicitudRestauranteValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CrearOpinionRestauranteValidator>();



// =====================
//    Controllers / JSON
// =====================
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

//REDIS
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
});

// =====================
//   EF Core / SQL Server
// =====================
builder.Services.AddDbContext<GustosDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
            sqlOptions => sqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)));


// =====================
//   Repositorios
// =====================
builder.Services.AddScoped<IHttpDownloader, HttpDownloader>();
builder.Services.AddScoped<IRecomendadorRestaurantes, SugerirGustosSobreUnRadioUseCase>();
builder.Services.AddScoped<IConstruirPreferencias, ConstruirPreferenciasUseCase>();

builder.Services.AddScoped<IAuthorizationHandler, RegistroIncompletoHandler>();

builder.Services.AddScoped<ICacheService, RedisCacheService>();

builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddScoped<IUsuarioRepository, UsuarioRepositoryEF>();
builder.Services.AddScoped<IRestriccionRepository, RestriccionRepositoryEF>();
builder.Services.AddScoped<ICondicionMedicaRepository, CondicionMedicaRepositoryEF>();
builder.Services.AddScoped<IGustoRepository, GustoRepositoryEF>();
builder.Services.AddScoped<IGrupoRepository, GrupoRepositoryEF>();
builder.Services.AddScoped<IMiembroGrupoRepository, MiembroGrupoRepositoryEF>();
builder.Services.AddScoped<IInvitacionGrupoRepository, InvitacionGrupoRepositoryEF>();
builder.Services.AddScoped<IGustosGrupoRepository, GustosGrupoRepositoryEF>();
builder.Services.AddScoped<INotificacionRepository, NotificacionRepositoryEF>();

builder.Services.AddScoped<INotificacionRealtimeService, SignalRNotificacionRealtimeService>();
builder.Services.AddScoped<IChatRealTimeService, SignalRChatRealtimeService>();
builder.Services.AddScoped<ISolicitudAmistadRealtimeService, SignalRSolicitudAmistadRealtimeService>();

builder.Services.AddScoped<IEnviarMensajeGrupoUseCase, EnviarMensajeGrupoUseCase>();

builder.Services.AddScoped<IUsuariosActivosService, UsuariosActivosService>();
builder.Services.AddScoped<IOpinionRestauranteRepository, OpinionRestauranteRepositoryEF>();
builder.Services.AddScoped<IRestauranteEstadisticasRepository, RestauranteEstadisticasRepositoryEF>();
builder.Services.AddScoped<IRestauranteRepository, RestauranteRepositoryEF>();
builder.Services.AddScoped<IUsuarioRestauranteFavoritoRepository, UsuarioRestauranteFavoritoEF>();
builder.Services.AddScoped<ISolicitudRestauranteRepository, SolicitudRestauranteRepositoryEF>();
builder.Services.AddScoped<IRestauranteMenuRepository, RestauranteMenuRepositoryEF>();

builder.Services.AddScoped<IFirebaseAuthService, FirebaseAuthService>();
builder.Services.AddScoped<IEmailTemplateService, EmailTemplateService>();

builder.Services.AddScoped<ISolicitudAmistadRepository, SolicitudAmistadRepositoryEF>();

// Votaciones
builder.Services.AddScoped<IVotacionRepository, VotacionRepository>();
builder.Services.AddScoped<IniciarVotacionUseCase>();
builder.Services.AddScoped<RegistrarVotoUseCase>();
builder.Services.AddScoped<ObtenerResultadosVotacionUseCase>();
builder.Services.AddScoped<CerrarVotacionUseCase>();
builder.Services.AddScoped<SeleccionarGanadorRuletaUseCase>();


// Chat repository
builder.Services.AddScoped<IChatRepository,ChatRepositoryEF>();

builder.Services.AddScoped<IRestauranteRepository, RestauranteRepositoryEF>();
builder.Services.AddScoped<GustosApp.Domain.Interfaces.IChatRepository, GustosApp.Infraestructure.Repositories.ChatRepositoryEF>();

// =====================
//    UseCases existentes
// =====================
builder.Services.AddScoped<ObtenerUsuarioUseCase>();
builder.Services.AddScoped<RegistrarUsuarioUseCase>();
builder.Services.AddScoped<ObtenerCondicionesMedicasUseCase>();
builder.Services.AddScoped<ObtenerGustosUseCase>();
builder.Services.AddScoped<ObtenerRestriccionesUseCase>();
builder.Services.AddScoped<CrearGrupoUseCase>();
builder.Services.AddScoped<ActualizarNombreGrupoUseCase>();
builder.Services.AddScoped<InvitarUsuarioGrupoUseCase>();
builder.Services.AddScoped<UnirseGrupoUseCase>();
builder.Services.AddScoped<AbandonarGrupoUseCase>();
builder.Services.AddScoped<ObtenerGruposUsuarioUseCase>();
builder.Services.AddScoped<ObtenerInvitacionesUsuarioUseCase>();
builder.Services.AddScoped<AceptarInvitacionGrupoUseCase>();
builder.Services.AddScoped<GuardarCondicionesUseCase>();
builder.Services.AddScoped<ObtenerGrupoDetalleUseCase>();
builder.Services.AddScoped<GuardarGustosUseCase>();
builder.Services.AddScoped<GuardarRestriccionesUseCase>();
builder.Services.AddScoped<ObtenerGustosFiltradosUseCase>();
builder.Services.AddScoped<ObtenerResumenRegistroUseCase>();
builder.Services.AddScoped<FinalizarRegistroUseCase>();
builder.Services.AddScoped<RemoverMiembroGrupoUseCase>();
builder.Services.AddScoped<SugerirGustosSobreUnRadioUseCase>();
builder.Services.AddScoped<CrearNotificacionUseCase>();
builder.Services.AddScoped<ObtenerNotificacionesUsuarioUseCase>();
builder.Services.AddScoped<ObtenerNotificacionUsuarioUseCase>();
builder.Services.AddScoped<MarcarNotificacionLeidaUseCase>();
builder.Services.AddScoped<ConstruirPreferenciasUseCase>();
builder.Services.AddScoped<ActualizarValoracionRestauranteUseCase>();
builder.Services.AddScoped<CrearSolicitudRestauranteUseCase>();
builder.Services.AddScoped<AprobarSolicitudRestauranteUseCase>();
builder.Services.AddScoped<ObtenerSolicitudRestaurantesPorIdUseCase>();
builder.Services.AddScoped<ObtenerDatosRegistroRestauranteUseCase>();
builder.Services.AddScoped<ObtenerSolicitudesPorTipoUseCase>();
builder.Services.AddScoped<RechazarSolicitudRestauranteUseCase>();
builder.Services.AddScoped<ActualizarValoracionRestauranteUseCase>();
builder.Services.AddScoped<RecomendacionIAUseCase>();
 builder.Services.AddScoped<ActualizarPerfilUsuarioUseCase>();
// UseCases y repositorios de amistad

builder.Services.AddScoped<EnviarSolicitudAmistadUseCase>();
builder.Services.AddScoped<ObtenerSolicitudesPendientesUseCase>();
builder.Services.AddScoped<AceptarSolicitudUseCase>();
builder.Services.AddScoped<RechazarSolicitudUseCase>();
builder.Services.AddScoped<ObtenerAmigosUseCase>();
builder.Services.AddScoped<EliminarAmigoUseCase>();
builder.Services.AddScoped<EliminarGrupoUseCase>();
builder.Services.AddScoped<ObtenerChatGrupoUseCase>();
builder.Services.AddScoped<EnviarMensajeGrupoUseCase>();
builder.Services.AddScoped<ActualizarGustosAGrupoUseCase>();
builder.Services.AddScoped<ObtenerPreferenciasGruposUseCase>();
builder.Services.AddScoped<EliminarGustosGrupoUseCase>();
builder.Services.AddScoped<DesactivarMiembroDeGrupoUseCase>();
builder.Services.AddScoped<IServicioPreferenciasGrupos,ServicioPreferenciasGrupos>();
builder.Services.AddScoped<EliminarNotificacionUseCase>();
builder.Services.AddScoped<ObtenerGustosPaginacionUseCase>();
builder.Services.AddScoped<BuscarGustoPorCoincidenciaUseCase>();
builder.Services.AddScoped<ObtenerGustosSeleccionadosPorUsuarioYParaFiltrarUseCase>();
builder.Services.AddScoped<BuscarUsuariosUseCase>();
builder.Services.AddScoped<ConfirmarAmistadEntreUsuarios>();
builder.Services.AddScoped<VerificarSiMiembroEstaEnGrupoUseCase>();
builder.Services.AddScoped<ObtenerRestaurantesAleatoriosGrupoUseCase>();
builder.Services.AddScoped<ActivarMiembroDeGrupoUseCase>();
builder.Services.AddScoped<EnviarRecomendacionesUsuariosActivosUseCase>();
builder.Services.AddScoped<CrearOpinionRestauranteUseCase>();
builder.Services.AddScoped<NotificacionesInteligentesService>();
builder.Services.AddScoped<BuscarRestaurantesUseCase>();
builder.Services.AddScoped<AgregarUsuarioRestauranteFavoritoUseCase>();
builder.Services.AddScoped<RegistrarTop3IndividualRestaurantesUseCase>();
builder.Services.AddScoped<RegistrarTop3GrupoRestaurantesUseCase>();
builder.Services.AddScoped<RegistrarVisitaPerfilRestauranteUseCase>();
builder.Services.AddScoped<ObtenerMetricasRestauranteUseCase>();
builder.Services.AddScoped<ActualizarRestauranteDashboardUseCase>();
builder.Services.AddScoped<ObtenerRestaurantesFavoritosUseCase>();
builder.Services.AddScoped<ObtenerRestauranteDetalleUseCase>();

builder.Services.AddHttpClient<IRecomendacionAIService, RecomendacionAIService>();
builder.Services.Configure<GeminiSettings>(builder.Configuration.GetSection("GeminiSettings"));

// Para notificaciones en tiempo real
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
});

builder.Services.AddSingleton<IUserIdProvider, FirebaseUserIdProvider>();

// =====================
//    Restaurantes (DI)
// =====================
// antes: builder.Services.AddAplicacionRestaurantes();
GustosApp.Infraestructure.DependencyInjection.AddInfraRestaurantes(builder.Services);


// =====================
//    Swagger
// =====================


builder.Services.AddHttpClient();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "GustosApp API",
        Version = "v1"
    });

    // 🔐 Esquema Bearer (JWT) para botón Authorize
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Pegá tu idToken de Firebase con el prefijo 'Bearer '.\nEjemplo: Bearer eyJhbGciOi..."
    });

    // 🔒 Requisito global (aplica Bearer a todos los endpoints)
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
var culture = CultureInfo.InvariantCulture;
CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;

// =====================
//        CORS
// =====================

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

// Detectamos ambiente
var isDevelopment = builder.Environment.IsDevelopment();

// Obtenemos origins del env (solo producción)
var allowedOriginsString = builder.Configuration["CORS_ALLOWED_ORIGINS"];

var allowedOrigins = allowedOriginsString?
    .Split(',', StringSplitOptions.RemoveEmptyEntries)
    .Select(s => s.Trim())
    .ToArray() ?? Array.Empty<string>();

builder.Services.AddCors(options =>
{
    options.AddPolicy(MyAllowSpecificOrigins, policy =>
    {
        if (isDevelopment)
        {
            // 🌱 LOCAL DEVELOPMENT
            policy
                .WithOrigins(
                    "http://localhost:3000",
                    "http://localhost:5174",
                    "https://lois-membranous-glancingly.ngrok-free.dev"
                )
                .AllowCredentials()
                .AllowAnyHeader()
                .AllowAnyMethod();
        }
        else
        {
            // 🌐 PRODUCTION
            if (allowedOrigins.Length == 0)
            {
                throw new Exception("CORS_ALLOWED_ORIGINS no configurado en producción.");
            }

            policy
                .WithOrigins(allowedOrigins)
                .AllowCredentials()
                .AllowAnyHeader()
                .AllowAnyMethod();
        }
    });
});

/* (Opcional) Exigir role=negocio para crear restaurantes
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("SoloNegocio", policy =>
        policy.RequireClaim("role", "negocio").RequireAuthenticatedUser());
});
*/

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    // Obtener el Logger
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    // Obtener la instancia de IConfiguration
    var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

    // Obtener la sección específica
    var geminiSection = configuration.GetSection("GeminiSettings");

    // 4. Imprimir la sección en el Logger
    if (geminiSection.Exists())
    {
        logger.LogInformation("🚀 Valores de la sección GeminiSettings:");

        // Iterar sobre los pares clave-valor dentro de la sección
        foreach (var child in geminiSection.GetChildren())
        {
            // Nota: El valor puede ser nulo si la clave tiene sub-secciones
            logger.LogInformation($"\t{child.Key} = {child.Value ?? "[Sub-sección o Nulo]"}");
        }
    }
    else
    {
        logger.LogWarning("⚠️ La sección GeminiSettings no fue encontrada en la configuración.");
    }
}

// =====================
//   Pipeline HTTP
// =====================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<ManejadorErrorMiddleware>();

// CORS debe ir antes de UseRouting para SignalR
app.UseCors(MyAllowSpecificOrigins);

app.UseStaticFiles(); // Habilitar archivos estáticos

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();
app.MapHub<ChatHub>("/chatHub");
app.MapHub<NotificacionesHub>("/notificacionesHub");
app.MapHub<SolicitudesAmistadHub>("/solicitudesAmistadHub");

app.Run();
