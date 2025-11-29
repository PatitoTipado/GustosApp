
using GustosApp.API.DTO;
using GustosApp.Application.Interfaces;
using GustosApp.Application.UseCases.UsuarioUseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using GustosApp.Domain.Model;

namespace GustosApp.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AutenticacionController : BaseApiController
    {
        private readonly IFirebaseAuthService _firebase;
        private readonly ObtenerUsuarioUseCase _obtenerUsuario;
        private readonly RegistrarUsuarioUseCase _registrarUsuario;

        public AutenticacionController(
            IFirebaseAuthService firebase, 
            ObtenerUsuarioUseCase obtenerUsuario,
            RegistrarUsuarioUseCase registrarUsuario)
        {
            _obtenerUsuario = obtenerUsuario;
            _firebase = firebase;
            _registrarUsuario = registrarUsuario;
        }
        [Authorize]
        [HttpPost("refresh-claims")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RefreshClaims(CancellationToken ct)
        {
            try
            {
                var uid = GetFirebaseUid(); 
                if (uid == null) 
                {
                    Console.WriteLine("[RefreshClaims] ❌ UID inválido");
                    return Unauthorized(new { message = "UID inválido" });
                }

                Console.WriteLine($"[RefreshClaims] 🔍 Buscando usuario con UID: {uid}");
                
                Usuario? usuario = null;
                
                try
                {
                    usuario = await _obtenerUsuario.HandleAsync(FirebaseUid: uid, ct: ct);
                    Console.WriteLine($"[RefreshClaims] ✅ Usuario encontrado en BD: {usuario.Email}");
                }
                catch (Exception ex) when (ex.Message == "Usuario no encontrado")
                {
                    Console.WriteLine($"[RefreshClaims] ⚠️ Usuario no encontrado en BD. Intentando crear desde Firebase...");
                    usuario = null;
                }
                
                // Si el usuario no existe en la BD, crearlo automáticamente desde Firebase
                if (usuario == null)
                {
                    try
                    {
                        // Obtener información del usuario de Firebase
                        var firebaseUser = await _firebase.GetUserByUidAsync(uid);
                        
                        if (firebaseUser == null)
                        {
                            Console.WriteLine($"[RefreshClaims] ❌ Usuario no encontrado en Firebase");
                            return Unauthorized(new { message = "Usuario no encontrado en Firebase" });
                        }

                        Console.WriteLine($"[RefreshClaims] ✅ Usuario encontrado en Firebase: {firebaseUser.Email}");

                        // Crear usuario en la BD
                        var nuevoUsuario = new Usuario(
                            firebaseUid: uid,
                            email: firebaseUser.Email ?? "",
                            nombre: firebaseUser.DisplayName?.Split(' ').FirstOrDefault() ?? "Usuario",
                            apellido: firebaseUser.DisplayName?.Split(' ').Skip(1).FirstOrDefault() ?? "",
                            idUsuario: Guid.NewGuid().ToString(),
                            fotoPerfilUrl: firebaseUser.PhotoUrl ?? ""
                        );

                        Console.WriteLine($"[RefreshClaims] 💾 Registrando usuario en BD...");
                        usuario = await _registrarUsuario.HandleAsync(uid, nuevoUsuario, ct);
                        
                        if (usuario == null)
                        {
                            Console.WriteLine($"[RefreshClaims] ❌ Error al crear usuario en BD");
                            return BadRequest(new { message = "Error al crear usuario en la base de datos" });
                        }

                        Console.WriteLine($"[RefreshClaims] ✅ Usuario creado exitosamente en BD");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[RefreshClaims] ❌ Excepción al registrar usuario: {ex.Message}");
                        Console.WriteLine($"[RefreshClaims] Stack trace: {ex.StackTrace}");
                        return BadRequest(new { message = $"Error al registrar usuario automáticamente: {ex.Message}" });
                    }
                }

                Console.WriteLine($"[RefreshClaims] 🔑 Estableciendo claims para rol: {usuario.Rol}");
                await _firebase.SetUserRoleAsync(usuario.FirebaseUid, usuario.Rol.ToString());

                Console.WriteLine($"[RefreshClaims] ✅ Claims actualizados correctamente");
                return Ok(new { message = "Claims actualizados", rol = usuario.Rol.ToString() });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[RefreshClaims] ❌ Error general: {ex.Message}");
                Console.WriteLine($"[RefreshClaims] Stack trace: {ex.StackTrace}");
                return StatusCode(500, new { message = $"Error interno: {ex.Message}" });
            }
        }
    }
}
