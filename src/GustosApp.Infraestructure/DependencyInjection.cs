using GustosApp.Application.Services;
using GustosApp.Infraestructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace GustosApp.Infraestructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfraRestaurantes(this IServiceCollection services)
        {
            services.AddScoped<IServicioRestaurantes, ServicioRestaurantes>();
            return services;
        }
    }
}
