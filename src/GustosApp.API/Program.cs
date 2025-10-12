using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
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

// ====================================================================
// [MODIFICADO] 1. FIREBASE / AUTH - Leer configuraciones desde Azure
// ====================================================================

// 1. Obtener los valores de configuración
// El Project ID se lee de Firebase:ProjectId (para JWT Validation)
var firebaseProjectId = builder.Configuration.GetValue<string>("Firebase:ProjectId");

// El JSON secreto completo se lee de FIREBASE_SERVICE_ACCOUNT_JSON
var firebaseServiceAccountJson = builder.Configuration.GetValue<string>("FIREBASE_SERVICE_ACCOUNT_JSON");

// Validar que el Project ID esté presente para la autenticación JWT
if (string.IsNullOrEmpty(firebaseProjectId))
{
    throw new InvalidOperationException("La configuración 'Firebase:ProjectId' es requerida y debe estar definida en Azure App Settings.");
}

    if (!string.IsNullOrEmpty(firebaseServiceAccountJson))
    {
        try
        {
            var credential = GoogleCredential.FromJson(firebaseServiceAccountJson);

            FirebaseApp.Create(new AppOptions()
            {
                Credential = credential,
                ProjectId = firebaseProjectId
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al inicializar Firebase Admin con JSON de Azure: {ex.Message}");
        }
    }
    else
    {
        var firebaseKeyPath = Path.Combine(builder.Environment.ContentRootPath, "secrets", "firebase-key.json");
        if (File.Exists(firebaseKeyPath))
        {
            FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.FromFile(firebaseKeyPath),
                ProjectId = firebaseProjectId
            });
        }
    }



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

// ===============================================================
// [MODIFICADO] 2. EF Core / SQL Server - Con Resiliencia de Azure
// ===============================================================

builder.Services.AddDbContext<GustosDbContext>(options =>
    options.UseSqlServer(
        // 'DefaultConnection' se lee directamente de Azure Connection Strings
        builder.Configuration.GetValue<string>("DefaultConnection"),
        // Añado resiliencia por si la DB está en pausa (error transitorio común)
        sqlOptions => sqlOptions.EnableRetryOnFailure()
    ));

// =====================
//    Repositorios
// =====================
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepositoryEF>();
builder.Services.AddScoped<IRestriccionRepository, RestriccionRepositoryEF>();
builder.Services.AddScoped<ICondicionMedicaRepository, CondicionMedicaRepositoryEF>();
builder.Services.AddScoped<IGustoRepository, GustoRepositoryEF>();
builder.Services.AddScoped<IGrupoRepository, GrupoRepositoryEF>();
builder.Services.AddScoped<IMiembroGrupoRepository, MiembroGrupoRepositoryEF>();
builder.Services.AddScoped<IInvitacionGrupoRepository, InvitacionGrupoRepositoryEF>();
builder.Services.AddScoped<IRestauranteRepository, RestauranteRepositoryEF>();
builder.Services.AddScoped<IReviewRepository, ReviewRepositoryEF>();

// =====================
//    UseCases existentes
// =====================
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
builder.Services.AddScoped<GuardarGustosUseCase>();
builder.Services.AddScoped<GuardarRestriccionesUseCase>();
builder.Services.AddScoped<ObtenerGustosFiltradosUseCase>();
builder.Services.AddScoped<ObtenerResumenRegistroUseCase>();
builder.Services.AddScoped<FinalizarRegistroUseCase>();
builder.Services.AddScoped<SugerirGustosUseCase>();
builder.Services.AddScoped<SugerirGustosUseCase>();
builder.Services.AddScoped<BuscarRestaurantesCercanosUseCase>();
builder.Services.AddScoped<ActualizarDetallesRestauranteUseCase>();

// =====================
//    Restaurantes (DI)
// =====================
// antes: builder.Services.AddAplicacionRestaurantes();
GustosApp.Infraestructure.DependencyInjection.AddInfraRestaurantes(builder.Services);
builder.Services.AddScoped<IRestauranteRepository, RestauranteRepositoryEF>();


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
            // [MODIFICADO] 3. CORS: Permite que el despliegue en Azure funcione también
            var allowedOrigins = builder.Configuration.GetValue<string>("CORS_ALLOWED_ORIGINS")?.Split(',')
                ?? new[] { "http://localhost:3000", "http://localhost:5174" };

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

// =======================================================
// [NUEVO] 4. APLICAR MIGRACIONES AL INICIAR (Code First)
// =======================================================

// Esto crea la DB (si no existe) y aplica todas las migraciones pendientes.
// Es la forma más limpia de asegurar que el Code First funcione en Azure al hacer el despliegue.
using (var scope = app.Services.CreateScope())
{
    try
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<GustosDbContext>();
        dbContext.Database.Migrate();
        Console.WriteLine("Migraciones aplicadas con éxito a Azure SQL.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error crítico al aplicar migraciones: {ex.Message}");
        // Aquí podrías agregar lógica para asegurar que la app no inicie si falla la DB
        // Pero por ahora, solo registramos el error.
    }
}


// =====================
//   Pipeline HTTP
// =====================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(MyAllowSpecificOrigins);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
