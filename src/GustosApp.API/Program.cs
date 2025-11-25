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

//(en la carpeta /secrets)
//var firebaseKeyPath = Path.Combine(builder.Environment.ContentRootPath, "secrets", "firebase-key.json");

var firebaseKeyPath = builder.Configuration["FIREBASE_SERVICE_ACCOUNT_JSON"];
var firebaseProjectId = builder.Configuration["FIREBASE_PROJECTID"];

// Inicializar Firebase solo si no está inicializado (Admin SDK: útil p/ scripts, NO requerido para validar JWT)
/*if (FirebaseApp.DefaultInstance == null)
{
    FirebaseApp.Create(new AppOptions()
    {
        Credential = GoogleCredential.FromFile(firebaseKeyPath)
    });
}*/

if (FirebaseApp.DefaultInstance == null)
{
    FirebaseApp.Create(new AppOptions()
    {
        Credential = GoogleCredential.FromJson(firebaseKeyPath)
    });
}

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());


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

        // Unificamos todo en un solo bloque de eventos
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                // 0. Preparar Logger
                var logger = context.HttpContext.RequestServices
                    .GetRequiredService<ILoggerFactory>()
                    .CreateLogger("JWT");

                logger.LogInformation("📥 Request a: {Path}", context.Request.Path);

                // Prioridad 1: Cookie "token"
                if (context.Request.Cookies.ContainsKey("token"))
                {
                    var raw = context.Request.Cookies["token"];
                    logger.LogInformation("🍪 Cookie 'token' encontrada: {TokenInicio}...",
                        raw?.Substring(0, Math.Min(15, raw.Length)));

                    context.Token = raw;
                    return Task.CompletedTask;
                }

                // Prioridad 2: Query string "access_token" (SignalR)
                var accessToken = context.Request.Query["access_token"];
                if (!string.IsNullOrEmpty(accessToken))
                {
                    logger.LogInformation("🔗 Token encontrado en QueryString (SignalR)");
                    context.Token = accessToken;
                    return Task.CompletedTask;
                }

                // Prioridad 3: Header Authorization (Manual check)
                // Nota: Normalmente JWTBearer lo hace solo, pero al sobrescribir este evento 
                // es seguro mantener tu lógica manual para garantizar que lo lea.
                var authHeader = context.Request.Headers["Authorization"].ToString();
                if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    // No logueamos aquí para no llenar la consola de logs "normales"
                    context.Token = authHeader.Substring("Bearer ".Length).Trim();
                    return Task.CompletedTask;
                }

                // Si llegamos acá, no se encontró token
                logger.LogWarning("⚠️ No llegó token en Cookie, QueryString ni Header");
                return Task.CompletedTask;
            },

            OnTokenValidated = context =>
            {
                var logger = context.HttpContext.RequestServices
                    .GetRequiredService<ILoggerFactory>()
                    .CreateLogger("JWT");

                logger.LogInformation("✅ Token VALIDADO correctamente");

                var claims = context.Principal.Claims
                    .Select(c => $"{c.Type} = {c.Value}");

                logger.LogInformation("🧩 Claims recibidos:\n{Claims}", string.Join("\n", claims));

                return Task.CompletedTask;
            },

            OnAuthenticationFailed = context =>
            {
                var logger = context.HttpContext.RequestServices
                    .GetRequiredService<ILoggerFactory>()
                    .CreateLogger("JWT");

                logger.LogError(context.Exception,
                    "❌ Falló la autenticación del JWT: {Error}", context.Exception.Message);

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddSingleton<IEmbeddingService>(sp =>
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


// OCR + Parser
builder.Services.AddSingleton<IOcrService>(sp =>
{
    var env = sp.GetRequiredService<IWebHostEnvironment>();

    var tessdataPath = Path.Combine(env.ContentRootPath, "tessdata");

    Console.WriteLine(" TESSDATA PATH => " + tessdataPath);
    Console.WriteLine(" Exists? => " + Directory.Exists(tessdataPath));

    foreach (var file in Directory.GetFiles(tessdataPath))
        Console.WriteLine(" " + file);

    return new TesseractOcrService(tessdataPath);
});


builder.Services.AddScoped<IMenuParser, SimpleMenuParser>();

//Autorizacion para acceder a ciertas rutas si el registro del usuario no esta completo
builder.Services.AddAuthorization(opt =>
{
    opt.AddPolicy("RegistroIncompleto", p =>
        p.Requirements.Add(new RegistroIncompletoRequirement()));
});



//##########
//REDIS
//############
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    // 1. Obtener el ILogger<T> del ServiceProvider (sp)
    //    Usamos ILogger<IConnectionMultiplexer> por convención, ya que es la clase que se está configurando.
    var logger = sp.GetRequiredService<ILogger<IConnectionMultiplexer>>();

    // 2. Obtener la configuración de la conexión Redis
    var config = builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379";

    // 3. Registrar el valor de la cadena de conexión
    //    Usamos LogInformation para registrar que se va a intentar conectar con esta configuración.
    logger.LogInformation("🚀 Conectando a Redis con la cadena de conexión: {RedisConnectionString}", config);

    // --- CONFIGURACIÓN DE FIREBASE Y LOGGING ---
    var firebaseKeyPath = builder.Configuration["FIREBASE_SERVICE_ACCOUNT_JSON"];
    var firebaseProjectId = builder.Configuration["FIREBASE_PROJECTID"];

    // Loguear los valores de Firebase
    if (!string.IsNullOrEmpty(firebaseKeyPath))
    {
        logger.LogInformation("🔑 Ruta del Service Account de Firebase: {FirebaseKeyPath}", firebaseKeyPath);
    }
    else
    {
        logger.LogWarning("⚠️ La ruta FIREBASE_SERVICE_ACCOUNT_JSON no está configurada.");
    }

    if (!string.IsNullOrEmpty(firebaseProjectId))
    {
        logger.LogInformation("💡 ID del Proyecto de Firebase: {FirebaseProjectId}", firebaseProjectId);
    }
    else
    {
        logger.LogWarning("⚠️ El ID del Proyecto de Firebase (FIREBASE_PROJECTID) no está configurado.");
    }

    // 4. Conectar al multiplexer
    return ConnectionMultiplexer.Connect(config);
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

builder.Services.AddScoped<IAuthorizationHandler, RegistroIncompletoHandler>();

builder.Services.AddScoped<ICacheService, RedisCacheService>();

builder.Services.AddScoped<IFileStorageService, FirebaseStorageService>();

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

builder.Services.AddScoped<ISolicitudAmistadRealtimeService, SignalRSolicitudAmistadRealtimeService>();

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
//    CORS
// =====================
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";


var allowedOriginsString = builder.Configuration["CORS_ALLOWED_ORIGINS"];

var allowedOrigins = allowedOriginsString?
    .Split(',', StringSplitOptions.RemoveEmptyEntries)
    .Select(s => s.Trim())
    .ToArray() ?? Array.Empty<string>();


builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins(allowedOrigins)
                  .AllowCredentials()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
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
