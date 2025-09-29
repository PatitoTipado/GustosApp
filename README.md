# GustosApp

GustosApp – Configuración del proyecto
Este proyecto usa Clean Architecture con:
ASP.NET Core 8 (Web API).
Entity Framework Core (Code First) con SQL Server.
Firebase Authentication para autenticación de usuarios.
Swagger para probar endpoints.

 Pasos para levantar el proyecto
1. Requisitos previos
Visual Studio 2022 o superior con soporte para .NET 8.
SQL Server Express (o una instancia de SQL Server local).
Una cuenta en Firebase Console


2. Configuración de la base de datos
Verificá que tengas SQL Server Express corriendo.
Configurá la cadena de conexión en appsettings.Development.json dentro de GustosApp.API:
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=GustosAppDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}

En Visual Studio, abrí la Package Manager Console (PMC):
Menú: Tools → NuGet Package Manager → Package Manager Console.
Ejecutá los siguientes comandos (asegurate de seleccionar como Default Project a GustosApp.Infraestructure):

Add-Migration InitialCreate -Project GustosApp.Infraestructure -StartupProject GustosApp.API
Update-Database -Project GustosApp.Infraestructure -StartupProject GustosApp.API


 Esto va a crear automáticamente la base de datos GustosAppDb con todas las tablas necesarias.

3. Configuración de Firebase
En Firebase Console, creá un proyecto (o usá el existente).
Copiá el Project ID (ej: gustosapp-5c3c9).
En Program.cs de GustosApp.API, actualizá la variable:

var firebaseProjectId = "gustosapp-5c3c9"; // tu project ID real


(Opcional) Si usás Firebase Admin SDK para notificaciones o manejo de usuarios:
Descargá la firebase-key.json desde Firebase Console → Service Accounts.
Guardala en la carpeta /secrets dentro del proyecto.
Asegurate de que esté en el .gitignore.

4. Ejecutar la API

Seleccioná GustosApp.API como Startup Project.
Presioná F5 o Ctrl+F5.
La API se levantará en:
https://localhost:5001/swagger
http://localhost:5000/swagger

5. Probar con Swagger

Primero, registrá o logueá un usuario en Firebase (usando SDK o REST API).
Copiá el idToken que devuelve Firebase.
En Swagger → botón Authorize → pegá:
Bearer <idToken>
Probá los endpoints de /api/usuarios.

📂 Estructura del proyecto
/src
  /GustosApp.Domain
  /GustosApp.Application
  /GustosApp.Infraestructure
  /GustosApp.API
/tests
  /GustosApp.Domain.Tests
  /GustosApp.Application.Tests
  /GustosApp.Infrastructure.Tests
  /GustosApp.API.Tests
