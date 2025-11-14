namespace GustosApp.API.Background
{
    public class NotificacionesBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<NotificacionesBackgroundService> _logger;

        public NotificacionesBackgroundService(IServiceProvider services, ILogger<NotificacionesBackgroundService> logger)
        {
            _services = services;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation(" Servicio de notificaciones inteligentes iniciado.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _services.CreateScope();
                    var servicio = scope.ServiceProvider.GetRequiredService<NotificacionesInteligentesService>();

                    await servicio.GenerarRecomendacionesPersonalizadasAsync(stoppingToken);

                    //await servicio.GenerarReengagementAsync(stoppingToken);

                    _logger.LogInformation(" Notificaciones inteligentes generadas correctamente.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error generando notificaciones inteligentes");
                }

                // Ejecutar cada 24 horas
                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
        }
    }
}
