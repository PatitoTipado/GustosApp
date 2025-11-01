using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GustosApp.Domain.Interfaces;
using GustosApp.Application.DTO;
using System.Security.Claims;

namespace GustosApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PagoController : ControllerBase
    {
        private readonly IPagoService _pagoService;
        private readonly IUsuarioRepository _usuarioRepository;

        public PagoController(IPagoService pagoService, IUsuarioRepository usuarioRepository)
        {
            _pagoService = pagoService;
            _usuarioRepository = usuarioRepository;
        }

        [HttpPost("crear")]
        [Authorize]
        public async Task<IActionResult> CrearPago([FromBody] CrearPagoRequest request, CancellationToken ct)
        {
            try
            {
                var firebaseUid = GetFirebaseUid();
                
                // Verificar que el usuario existe
                var usuario = await _usuarioRepository.GetByFirebaseUidAsync(firebaseUid, ct);
                if (usuario == null)
                    return Unauthorized(new { message = "Usuario no encontrado" });

                // Verificar que el usuario no es ya premium
                if (usuario.EsPremium())
                    return BadRequest(new { message = "El usuario ya tiene plan Premium" });

                var initPoint = await _pagoService.CrearPreferenciaPagoAsync(
                    firebaseUid, 
                    request.Email, 
                    request.NombreCompleto
                );

                var response = new CrearPagoResponse
                {
                    InitPoint = initPoint,
                    Status = "created"
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        [HttpPost("webhook")]
        [AllowAnonymous]
        public async Task<IActionResult> WebhookPago([FromBody] WebhookPagoRequest request)
        {
            try
            {
                // Verificar que es una notificación de pago
                if (request.Type == "payment")
                {
                    var procesado = await _pagoService.ProcesarNotificacionPagoAsync(request.Data.Id);
                    if (procesado)
                    {
                        return Ok(new { message = "Pago procesado correctamente" });
                    }
                }

                return Ok(new { message = "Notificación recibida" });
            }
            catch (Exception ex)
            {
                // En un escenario real, registrarías este error
                Console.WriteLine($"Error en webhook: {ex.Message}");
                return Ok(); // Siempre devolver 200 para evitar que MercadoPago reenvíe
            }
        }

        [HttpGet("verificar/{pagoId}")]
        [Authorize]
        public async Task<IActionResult> VerificarPago(string pagoId)
        {
            try
            {
                var aprobado = await _pagoService.VerificarEstadoPagoAsync(pagoId);
                return Ok(new { aprobado });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al verificar pago", details = ex.Message });
            }
        }

        [HttpGet("beneficios")]
        [AllowAnonymous]
        public IActionResult ObtenerBeneficiosPremium()
        {
            var beneficios = new BeneficiosPremiumDto { Precio = 9999.99m };
            return Ok(beneficios);
        }

        [HttpPost("crear-test")]
        [AllowAnonymous] // Temporal para pruebas sin autenticación
        public async Task<IActionResult> CrearPagoTest([FromBody] CrearPagoTestRequest request)
        {
            try
            {
                var initPoint = await _pagoService.CrearPreferenciaPagoAsync(
                    request.UsuarioId ?? "test-user", 
                    request.Email ?? "test@example.com", 
                    request.NombreCompleto ?? "Usuario de Prueba"
                );

                var response = new CrearPagoResponse
                {
                    InitPoint = initPoint,
                    Status = "created"
                };

                return Ok(new { success = true, data = response });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        private string GetFirebaseUid()
        {
            var firebaseUid = User.FindFirst("user_id")?.Value
                            ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                            ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrWhiteSpace(firebaseUid))
                throw new UnauthorizedAccessException("No se encontró el UID de Firebase en el token.");

            return firebaseUid;
        }
    }
}