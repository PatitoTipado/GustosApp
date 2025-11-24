
using GustosApp.Application.Interfaces;
using Microsoft.Extensions.Logging;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;
using GustosApp.Application.UseCases.UsuarioUseCases.GustoUseCases;
using GustosApp.Application.UseCases.RestauranteUseCases;

public class NotificacionesInteligentesService
{
    private readonly IUsuarioRepository _usuarios;
    private readonly IRestauranteRepository _restaurantes;
    private readonly INotificacionRepository _notificaciones;
    private readonly INotificacionRealtimeService _realtime;
    private readonly SugerirGustosSobreUnRadioUseCase _sugerirGustos;
    private readonly IEmailService _emailService;
    private readonly ObtenerGustosUseCase _obtenerGustos;
    private readonly ILogger<NotificacionesInteligentesService> _logger;

    public NotificacionesInteligentesService(
        IUsuarioRepository usuarios,
        IRestauranteRepository restaurantes,
        INotificacionRepository notificaciones,
        INotificacionRealtimeService realtime,
        ObtenerGustosUseCase obtenerGustos,
        SugerirGustosSobreUnRadioUseCase sugerirGustos,
        IEmailService emailService,
        ILogger<NotificacionesInteligentesService> logger)
    {
        _usuarios = usuarios;
        _restaurantes = restaurantes;
        _notificaciones = notificaciones;
        _realtime = realtime;
        _sugerirGustos = sugerirGustos;
        _obtenerGustos = obtenerGustos;
        _emailService = emailService;
        _logger = logger;
    }

    // 1. Recomendaciones personalizadas
   /* public async Task GenerarRecomendacionesPersonalizadasAsync(CancellationToken ct = default)
    {
        var usuarios = await _usuarios.GetAllAsync(limit: 100, ct);
        _logger.LogInformation("Iniciando generación de recomendaciones para {count} usuarios", usuarios.Count());

        int totalNotificaciones = 0;

        foreach (var usuario in usuarios)
        {
            ct.ThrowIfCancellationRequested();

            if (usuario.Gustos == null || !usuario.Gustos.Any())
                continue;
            if (string.IsNullOrEmpty(usuario.FirebaseUid))
                continue;

            try
            {
                var preferenciasUsuario = await _obtenerGustos.HandleAsync(usuario.FirebaseUid, ct);
                var recomendaciones = await _sugerirGustos.Handle(preferenciasUsuario, 10, ct);

                if (recomendaciones == null || !recomendaciones.Any())
                    continue;

                var notificaciones = recomendaciones.Take(1).Select(rest => new Notificacion
                {
                    UsuarioDestinoId = usuario.Id,
                    Titulo = "Recomendación personalizada 🍽️",
                    Mensaje = $"Basado en tus gustos, te recomendamos {rest.Nombre}",
                    Tipo = TipoNotificacion.Recomendacion,
                    FechaCreacion = DateTime.UtcNow
                }).ToList();

                //  Guardar todas las notificaciones en paralelo
                var guardarTasks = notificaciones.Select(n => _notificaciones.crearAsync(n, ct));
                await Task.WhenAll(guardarTasks);

                // Enviar SignalR y Email en paralelo
                var envioTasks = notificaciones.Select(async n =>
                {
                    // Notificación en tiempo real
                    await _realtime.EnviarNotificacionAsync(
                        usuario.FirebaseUid,
                        n.Titulo,
                        n.Mensaje,
                        "Recomendacion",
                        ct);

                    // Email HTML
                    var cuerpoHtml = $@"
                    <h2>🍽️ Recomendación para vos</h2>
                    <p>Hola {usuario.Nombre},</p>
                    <p>{n.Mensaje}</p>
                    <p><a href='https://gustosapp.com/restaurantes'>Ver más restaurantes recomendados</a></p>
                    <hr/>
                    <small>Este mensaje fue generado automáticamente por GustosApp</small>
                ";

                    // Se lanza en paralelo para no bloquear
                    _ = Task.Run(() => _emailService.EnviarEmailAsync(usuario.Email, n.Titulo, cuerpoHtml, ct));
                });

                await Task.WhenAll(envioTasks);

                totalNotificaciones += notificaciones.Count;

                _logger.LogInformation("Usuario {uid}: {count} recomendaciones enviadas", usuario.FirebaseUid, notificaciones.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generando recomendaciones para el usuario {uid}", usuario.FirebaseUid);
            }
        }

        _logger.LogInformation("Finalizó la generación de recomendaciones. Total enviadas: {total}", totalNotificaciones);
    }

    /*  //  2. Re-engagement (usuarios inactivos)
      public async Task GenerarReengagementAsync(CancellationToken ct = default)
      {
          var usuarios = await _usuarios.ObtenerInactivosAsync(diasSinActividad: 5, ct);

          foreach (var usuario in usuarios)
          {
              var notificacion = new Notificacion
              {
                  UsuarioId = usuario.Id,
                  Titulo = "¡Te extrañamos! 👋",
                  Mensaje = "Hace unos días que no calificás restaurantes. ¿Querés dejar tu opinión?",
                  Tipo = TipoNotificacion.Reengagement,
                  FechaCreacion = DateTime.UtcNow
              };

              await _notificaciones.AddAsync(notificacion, ct);
              await _realtime.EnviarNotificacionAsync(usuario.FirebaseUid, notificacion.Titulo, notificacion.Mensaje, "Reengagement", ct);
          }
      }

      //  3. Actividad social
      public async Task GenerarActividadSocialAsync(EventoSocial evento, CancellationToken ct = default)
      {
          foreach (var usuario in evento.Destinatarios)
          {
              var notificacion = new Notificacion
              {
                  UsuarioId = usuario.Id,
                  Titulo = evento.Titulo,
                  Mensaje = evento.Mensaje,
                  Tipo = TipoNotificacion.Social,
                  FechaCreacion = DateTime.UtcNow
              };

              await _notificaciones.AddAsync(notificacion, ct);
              await _realtime.EnviarNotificacionAsync(usuario.FirebaseUid, evento.Titulo, evento.Mensaje, "Social", ct);
          }
      }
    */
}
