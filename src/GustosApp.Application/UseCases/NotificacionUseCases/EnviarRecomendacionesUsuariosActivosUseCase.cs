using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using GustosApp.Application.DTO.PlacesV1;
using GustosApp.Application.Interfaces;
using GustosApp.Application.UseCases.RestauranteUseCases;
using GustosApp.Application.UseCases.UsuarioUseCases;
using GustosApp.Domain.Common;
using GustosApp.Domain.Interfaces;

namespace GustosApp.Application.UseCases.NotificacionUseCases
{
    public class EnviarRecomendacionesUsuariosActivosUseCase
    {
        private readonly IUsuariosActivosService _usuariosActivos;
        private readonly ICacheService _cache;
        private readonly SugerirGustosSobreUnRadioUseCase _recomendador;
        private readonly ConstruirPreferenciasUseCase _construirPreferencias;
        private readonly IEmailService _email;
        private readonly IUsuarioRepository _usuariosRepo;
        private readonly IServicioRestaurantes _serviceRestaurante;

        public EnviarRecomendacionesUsuariosActivosUseCase(
            IUsuariosActivosService usuariosActivos,
            ICacheService cache,
            SugerirGustosSobreUnRadioUseCase recomendador,
            ConstruirPreferenciasUseCase construirPreferencias,
            IEmailService email,
            IUsuarioRepository usuariosRepo,
            IServicioRestaurantes serviceRestaurante)
        {
            _usuariosActivos = usuariosActivos;
            _cache = cache;
            _recomendador = recomendador;
            _construirPreferencias = construirPreferencias;
            _email = email;
            _usuariosRepo = usuariosRepo;
            _serviceRestaurante = serviceRestaurante;
        }


        //validar q sea moderador usuario rol
        public async Task<bool> HandleAsync(string firebaseUid,CancellationToken ct)
        {
            var activos = _usuariosActivos.GetUsuariosActivos();
            bool mandoNotif= false;
            foreach (var uid in activos)
            {
                var ubicacion = await _cache.GetAsync<UserLocation>($"usuario:{uid}:location");
                if (ubicacion == null) continue;

                var usuario = await _usuariosRepo.GetByFirebaseUidAsync(uid, ct);
                if (usuario == null) continue;

                var res = await _serviceRestaurante.BuscarAsync(
                rating: 3.5,
                tipo: null,
                plato: "",
                lat: ubicacion.Lat,
                lng: ubicacion.Lng,
                radioMetros: ubicacion.Radio
            );

                var preferencias = await _construirPreferencias.HandleAsync(
                    uid,
                    amigoUsername: null,
                    grupoId: null,
                    gustosDelFiltro: null,
                    ct);

             

                var recomendaciones = await _recomendador.Handle(
                    preferencias,
                    res,
                    1,
                    ct
                );

                if (!recomendaciones.Any())
                    throw new Exception("Algo anda mal viejo");


                await _email.EnviarEmailAsync(
                    usuario.Email,
                    "Recomendación personalizada",
                    $"Según tus gustos y ubicación, te recomendamos: {recomendaciones[0].Nombre}"
                );
                mandoNotif = true;
            }
            return mandoNotif;
        }
       
    }

}
