using GustosApp.Infraestructure;
using Microsoft.EntityFrameworkCore;

namespace GustosApp.API.Tests.Infraestructura;

public static class DbContextEnMemoria
{
    public static GustosDbContext Crear(string nombreBd)
    {
        var opciones = new DbContextOptionsBuilder<GustosDbContext>()
            .UseInMemoryDatabase(nombreBd)
            .EnableSensitiveDataLogging()
            .Options;

        var ctx = new GustosDbContext(opciones);
        ctx.Database.EnsureDeleted();
        ctx.Database.EnsureCreated();
        return ctx;
    }
}
