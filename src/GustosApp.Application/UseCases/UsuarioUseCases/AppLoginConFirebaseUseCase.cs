using FirebaseAdmin.Auth;
using GustosApp.Application.Interfaces;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;
using GustosApp.Domain.Model.@enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GustosApp.Application.UseCases.UsuarioUseCases
{
    public class AppLoginConFirebaseUseCase
    {
        private readonly IUsuarioRepository _repositorioUsuario;
        private readonly IRestauranteRepository _repositorioRestaurante;
        private readonly IFirebaseAuthService _authService; // lo uso para el test

        public AppLoginConFirebaseUseCase(IUsuarioRepository repositorioUsuario, IRestauranteRepository repositorioRestaurante, IFirebaseAuthService authService)
        {
            _repositorioUsuario = repositorioUsuario;
            _repositorioRestaurante = repositorioRestaurante;
            _authService = authService;
        }

        public async Task<string> HandleAsync(string idToken)
        {
            try
            {
                var decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(idToken);
                return decodedToken.Uid;
            }
            catch (FirebaseAuthException ex)
            {
                throw new UnauthorizedAccessException("Token de Firebase inválido", ex);
            }
        }

        public async Task<(string firebaseUid, string tipoUsuario)> HandleAsyncCrearUsuario(string idToken)
        {
            try
            {
                var decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(idToken);
                string firebaseUid = decodedToken.Uid;

                var usuario = await _repositorioUsuario.GetByFirebaseUidAsync(firebaseUid);
                if (usuario != null)
                    return (firebaseUid, "Usuario");

                var nuevoUsuario = new Usuario
                {
                    FirebaseUid = firebaseUid,
                    Email = decodedToken.Claims["email"]?.ToString() ?? "",
                    Nombre = decodedToken.Claims["nombre"]?.ToString() ?? "",
                    Apellido = decodedToken.Claims["apellido"]?.ToString() ?? "",
                    Plan = PlanUsuario.Free,
                    FechaRegistro = DateTime.UtcNow,
                };

                await _repositorioUsuario.AddAsync(nuevoUsuario, CancellationToken.None);
                await _repositorioUsuario.SaveChangesAsync(CancellationToken.None);


                return (firebaseUid, "Usuario");
            }
            catch (FirebaseAuthException ex)
            {
                throw new UnauthorizedAccessException("Token de Firebase inválido", ex);
            }
        }

        public async Task<(string firebaseUid, string tipoUsuario)> HandleAsyncCrearRestaurante(string idToken)
        {
            try
            {
                var decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(idToken);
                string firebaseUid = decodedToken.Uid;

                var restaurante = await _repositorioRestaurante.GetByFirebaseUidAsync(firebaseUid);
                if (restaurante != null)
                    return (firebaseUid, "Restaurante");

                var nuevoRestaurante = new Restaurante
                {
                    PropietarioUid = firebaseUid,
                    Nombre = decodedToken.Claims["nombre"]?.ToString() ?? "",
                    NombreNormalizado = decodedToken.Claims["nombreNormalizado"]?.ToString() ?? "",
                    Direccion = decodedToken.Claims["direccion"]?.ToString() ?? "",
                };

                await _repositorioRestaurante.AddAsync(nuevoRestaurante, CancellationToken.None);
                await _repositorioRestaurante.SaveChangesAsync(CancellationToken.None);

                return (firebaseUid, "Restaurante");
            }
            catch (FirebaseAuthException ex)
            {
                throw new UnauthorizedAccessException("Token de Firebase inválido", ex);
            }
        }

       


    }
}
