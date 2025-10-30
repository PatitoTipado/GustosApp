using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using GustosApp.API.Mapping;
using GustosApp.API.Middleware;
using GustosApp.Application;
using GustosApp.Application.Interfaces;
using GustosApp.Application.UseCases;
using GustosApp.Domain.Interfaces;
using GustosApp.Infraestructure;
using GustosApp.Infraestructure.ML;
using GustosApp.Infraestructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
// Usar System.Text.Json para manejar el secreto de Firebase
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// =====================
//   Firebase / Auth
// =====================

//(en la carpeta /secrets)
var firebaseKeyPath = Path.Combine(builder.Environment.ContentRootPath, "secrets", "firebase-key.json");
var firebaseProjectId = "gustosapp-5c3c9";

// Inicializar Firebase solo si no está inicializado (Admin SDK: útil p/ scripts, NO requerido para validar JWT)
if (FirebaseApp.DefaultInstance == null)
{
    FirebaseApp.Create(new AppOptions()
    {
        Credential = GoogleCredential.FromFile(firebaseKeyPath)
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
    });

builder.Services.AddSingleton<IEmbeddingService>(sp =>
{
    var env = sp.GetRequiredService<IWebHostEnvironment>();
    var modelPath = Path.Combine(AppContext.BaseDirectory, "ML", "model.onnx");
    var tokPath = Path.Combine(AppContext.BaseDirectory, "ML", "tokenizer.json");
    return new OnnxEmbeddingService(modelPath, tokPath);
});

// Autorización explícita 
builder.Services.AddAuthorization();

// =====================
//    Controllers / JSON
// =====================
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });

// =====================
//   EF Core / SQL Server
// =====================
builder.Services.AddDbContext<GustosDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// =====================
//   Repositorios
// =====================
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepositoryEF>();
builder.Services.AddScoped<IRestriccionRepository, RestriccionRepositoryEF>();
builder.Services.AddScoped<ICondicionMedicaRepository, CondicionMedicaRepositoryEF>();
builder.Services.AddScoped<IGustoRepository, GustoRepositoryEF>();
builder.Services.AddScoped<IGrupoRepository, GrupoRepositoryEF>();
builder.Services.AddScoped<IMiembroGrupoRepository, MiembroGrupoRepositoryEF>();
builder.Services.AddScoped<IInvitacionGrupoRepository, InvitacionGrupoRepositoryEF>();
builder.Services.AddScoped<IReviewRepository, ReviewRepositoryEF>();
builder.Services.AddScoped<IGustosGrupoRepository, GustosGrupoRepositoryEF>();
// Chat repository
builder.Services.AddScoped<GustosApp.Domain.Interfaces.IChatRepository, GustosApp.Infraestructure.Repositories.ChatRepositoryEF>();
builder.Services.AddScoped<IRestauranteRepository, RestauranteRepositoryEF>();

// =====================
//    UseCases existentes
// =====================
builder.Services.AddScoped<ObtenerUsuarioUseCase>();
builder.Services.AddScoped<RegistrarUsuarioUseCase>();
builder.Services.AddScoped<ObtenerCondicionesMedicasUseCase>();
builder.Services.AddScoped<ObtenerGustosUseCase>();
builder.Services.AddScoped<ObtenerRestriccionesUseCase>();
builder.Services.AddScoped<CrearGrupoUseCase>();
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
builder.Services.AddScoped<SugerirGustosUseCase>();
builder.Services.AddScoped<SugerirGustosUseCase>();
builder.Services.AddScoped<BuscarRestaurantesCercanosUseCase>();
builder.Services.AddScoped<ActualizarDetallesRestauranteUseCase>();
builder.Services.AddScoped<RemoverMiembroGrupoUseCase>();
builder.Services.AddScoped<SugerirGustosSobreUnRadioUseCase>();
// UseCases y repositorios de amistad
builder.Services.AddScoped<GustosApp.Domain.Interfaces.ISolicitudAmistadRepository, GustosApp.Infraestructure.Repositories.SolicitudAmistadRepositoryEF>();
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
//    CORS
// =====================
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins("http://localhost:3000", "http://localhost:5174")
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

// =====================
//   Pipeline HTTP
// =====================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(MyAllowSpecificOrigins);

app.UseMiddleware<ManejadorErrorMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
