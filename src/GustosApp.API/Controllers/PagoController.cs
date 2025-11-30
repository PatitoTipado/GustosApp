using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GustosApp.Domain.Interfaces;
using GustosApp.API.DTO;
using System.Security.Claims;
using GustosApp.Domain.Model.@enum;
using GustosApp.API.DTO;

namespace GustosApp.API.Controllers
{
    [Authorize]
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
        [ProducesResponseType(typeof(CrearPagoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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
                Console.WriteLine($"🔔 [WebhookController] Notificación recibida");
                Console.WriteLine($"🔔 [WebhookController] Type: {request.Type}");
                Console.WriteLine($"🔔 [WebhookController] Data.Id: {request.Data?.Id}");
                
                // Verificar que es una notificación de pago
                if (request.Type == "payment")
                {
                    Console.WriteLine($"🔔 [WebhookController] Es notificación de pago, procesando...");
                    
                    var procesado = await _pagoService.ProcesarNotificacionPagoAsync(request.Data.Id);
                    
                    if (procesado)
                    {
                        Console.WriteLine($"✅ [WebhookController] Pago procesado correctamente");
                        return Ok(new { message = "Pago procesado correctamente" });
                    }
                    else
                    {
                        Console.WriteLine($"⚠️ [WebhookController] Pago no procesado (posiblemente no aprobado o ya procesado)");
                    }
                }
                else
                {
                    Console.WriteLine($"ℹ️ [WebhookController] Tipo de notificación no es payment: {request.Type}");
                }

                return Ok(new { message = "Notificación recibida" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [WebhookController] Error en webhook: {ex.Message}");
                Console.WriteLine($"❌ [WebhookController] StackTrace: {ex.StackTrace}");
                return Ok(); // Siempre devolver 200 para evitar que MercadoPago reenvíe
            }
        }

        [HttpGet("verificar/{pagoId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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
            var beneficios = new BeneficiosPremiumDto { Precio = 50.00m };
            return Ok(beneficios);
        }

        [HttpGet("diagnostico")]
        [AllowAnonymous]
        public IActionResult Diagnostico([FromServices] Microsoft.Extensions.Configuration.IConfiguration config)
        {
            var accessToken = config["MercadoPago:AccessToken"];
            var publicKey = config["MercadoPago:PublicKey"];
            
            return Ok(new
            {
                hasAccessToken = !string.IsNullOrEmpty(accessToken),
                accessTokenPrefix = accessToken?.Substring(0, Math.Min(15, accessToken.Length)),
                hasPublicKey = !string.IsNullOrEmpty(publicKey),
                publicKeyPrefix = publicKey?.Substring(0, Math.Min(15, publicKey.Length)),
                isProduction = accessToken?.StartsWith("APP_USR") ?? false
            });
        }

        [HttpGet("config")]
        [AllowAnonymous]
        public IActionResult GetConfig([FromServices] Microsoft.Extensions.Configuration.IConfiguration config)
        {
            var publicKey = config["MercadoPago:PublicKey"];
            
            if (string.IsNullOrEmpty(publicKey))
            {
                return BadRequest(new { message = "MercadoPago no configurado" });
            }
            
            return Ok(new
            {
                publicKey = publicKey,
                isTestMode = publicKey.StartsWith("TEST-") || publicKey.StartsWith("APP_USR")
            });
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

        [HttpGet("verificar-reciente")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> VerificarPagoReciente()
        {
            try
            {
                var firebaseUid = GetFirebaseUid();
                var usuario = await _usuarioRepository.GetByFirebaseUidAsync(firebaseUid);

                if (usuario == null)
                {
                    return NotFound(new { aprobado = false, message = "Usuario no encontrado" });
                }

                // Verificar si el usuario ya es Premium
                if (usuario.Plan == PlanUsuario.Plus)
                {
                    return Ok(new { aprobado = true, message = "Usuario ya es Premium" });
                }

                // Si no es Premium, devolver false
                return Ok(new { aprobado = false, message = "Usuario aún no es Premium" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { aprobado = false, message = ex.Message });
            }
        }

        [HttpGet("verificar-estado-premium")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> VerificarEstadoPremium(CancellationToken ct)
        {
            try
            {
                Console.WriteLine("🔍 [VerificarEstadoPremium] Verificando estado del usuario...");
                
                var firebaseUid = GetFirebaseUid();
                Console.WriteLine($"🔍 [VerificarEstadoPremium] FirebaseUid: {firebaseUid}");
                
                var usuario = await _usuarioRepository.GetByFirebaseUidAsync(firebaseUid, ct);

                if (usuario == null)
                {
                    Console.WriteLine($"❌ [VerificarEstadoPremium] Usuario no encontrado");
                    return NotFound(new { success = false, isPremium = false, message = "Usuario no encontrado" });
                }

                // Si el usuario aún no es Premium, intentar verificar pagos pendientes
                if (usuario.Plan != PlanUsuario.Plus)
                {
                    Console.WriteLine($"🔍 [VerificarEstadoPremium] Usuario no es Premium, verificando pagos pendientes...");
                    var pagoAprobado = await _pagoService.VerificarYProcesarPagosPendientesAsync(firebaseUid);
                    
                    if (pagoAprobado)
                    {
                        Console.WriteLine($"✅ [VerificarEstadoPremium] Pago aprobado encontrado, recargando usuario...");
                        // Recargar el usuario para obtener el estado actualizado
                        usuario = await _usuarioRepository.GetByFirebaseUidAsync(firebaseUid, ct);
                    }
                }

                var isPremium = usuario.Plan == PlanUsuario.Plus;
                Console.WriteLine($"✅ [VerificarEstadoPremium] Usuario: {usuario.Email}, Plan: {usuario.Plan}, IsPremium: {isPremium}");

                return Ok(new 
                { 
                    success = true,
                    isPremium = isPremium,
                    plan = usuario.Plan.ToString(),
                    message = isPremium ? "Usuario es Premium" : "Usuario es Free"
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [VerificarEstadoPremium] Error: {ex.Message}");
                return StatusCode(500, new { success = false, isPremium = false, message = ex.Message });
            }
        }

        [HttpPost("forzar-premium-dev")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ForzarPremiumDev(CancellationToken ct)
        {
            try
            {
                Console.WriteLine("🔧 [ForzarPremiumDev] SOLO PARA DESARROLLO - Forzando Premium...");
                
                var firebaseUid = GetFirebaseUid();
                Console.WriteLine($"🔧 [ForzarPremiumDev] FirebaseUid: {firebaseUid}");
                
                var usuario = await _usuarioRepository.GetByFirebaseUidAsync(firebaseUid, ct);

                if (usuario == null)
                {
                    Console.WriteLine($"❌ [ForzarPremiumDev] Usuario no encontrado");
                    return NotFound(new { success = false, message = "Usuario no encontrado" });
                }

                if (usuario.Plan == PlanUsuario.Plus)
                {
                    Console.WriteLine($"ℹ️ [ForzarPremiumDev] Usuario ya es Premium");
                    return Ok(new { success = true, message = "Usuario ya es Premium", plan = "Plus" });
                }

                Console.WriteLine($"🔄 [ForzarPremiumDev] Actualizando usuario a Premium...");
                await _usuarioRepository.UpdatePlanAsync(firebaseUid, PlanUsuario.Plus, ct);
                Console.WriteLine($"✅ [ForzarPremiumDev] Usuario actualizado a Premium");

                return Ok(new 
                { 
                    success = true,
                    isPremium = true,
                    message = "Usuario actualizado a Premium (DEV MODE)",
                    plan = "Plus"
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [ForzarPremiumDev] Error: {ex.Message}");
                return StatusCode(500, new { success = false, message = ex.Message });
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