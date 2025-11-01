using GustosApp.Application.Services;
using GustosApp.Infraestructure.Services;
using GustosApp.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace GustosApp.Infraestructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfraRestaurantes(this IServiceCollection services)
        {
            services.AddScoped<IServicioRestaurantes, ServicioRestaurantes>();
            // Repositorios de grupos y solicitudes
            services.AddScoped<GustosApp.Domain.Interfaces.ISolicitudAmistadRepository, GustosApp.Infraestructure.Repositories.SolicitudAmistadRepositoryEF>();
            // Servicio de pagos
            services.AddScoped<IPagoService, MercadoPagoService>();
            return services;
        }
    }
}
