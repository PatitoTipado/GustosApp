using FirebaseAdmin;
using Google.Api;
using FluentValidation;
using Google.Apis.Auth.OAuth2;
using GustosApp.API.Hubs;
using GustosApp.API.Hubs.GustosApp.API.Hubs;
using GustosApp.API.Hubs.Services;
using GustosApp.API.Mapping;
using GustosApp.API.Middleware;
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
using GustosApp.Application.UseCases.UsuarioUseCases;
using GustosApp.Application.UseCases.UsuarioUseCases.CondicionesMedicasUseCases;
using GustosApp.Application.UseCases.UsuarioUseCases.GustoUseCases;
using GustosApp.Application.UseCases.UsuarioUseCases.RestriccionesUseCases;
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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using GustosApp.Application.UseCases.VotacionUseCases;
using GustosApp.Application.UseCases.RestauranteUseCases.SolicitudRestauranteUseCases;
using GustosApp.API.Templates.Email;
using GustosApp.Application.Validations.Restaurantes;
using FluentValidation.AspNetCore;
using GustosApp.API.Validations.OpinionRestaurantes;
using GustosApp.Application.UseCases.RestauranteUseCases.OpinionesRestaurantes;


AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);


var builder = WebApplication.CreateBuilder(args);

// =====================
//   Firebase / Auth
// =====================

//(en la carpeta /secrets)
//var firebaseKeyPath = Path.Combine(builder.Environment.ContentRootPath, "secrets", "firebase-key.json");
//var firebaseProjectId = "gustosapp-5c3c9";

var firebaseKeyPath = builder.Configuration["FIREBASE_SERVICE_ACCOUNT_JSON"];
var firebaseProjectId = builder.Configuration["Firebase:ProjectId"];

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


builder.Services.AddAutoMapper(typeof(ApiMapeoPerfil));
// Validación de JWT emitidos por Firebase (securetoken)
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
                // Si hay cookie "token", úsala como fuente del JWT
                if (context.Request.Cookies.ContainsKey("token"))
                {
                    context.Token = context.Request.Cookies["token"];
                }
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
    var config = builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379";
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
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


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
try
{

    var app = builder.Build();

// =====================
//   Pipeline HTTP
// =====================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// CORS debe ir antes de UseRouting para SignalR
app.UseCors(MyAllowSpecificOrigins);

app.UseStaticFiles(); // Habilitar archivos estáticos

app.UseRouting();


app.UseMiddleware<ManejadorErrorMiddleware>();
app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();
app.MapHub<ChatHub>("/chatHub");
app.MapHub<NotificacionesHub>("/notificacionesHub");
app.MapHub<SolicitudesAmistadHub>("/solicitudesAmistadHub");

app.Run();
}
catch (Exception ex)
{
    Console.WriteLine("🔥 ERROR FATAL: " + ex);
    throw;
}
