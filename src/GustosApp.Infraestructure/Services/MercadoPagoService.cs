using MercadoPagoCore.Client.Preference;
using MercadoPagoCore.Client.Payment;
using MercadoPagoCore.Config;
using MercadoPagoCore.Resource.Preference;
using MercadoPagoCore.Resource.Payment;
using Microsoft.Extensions.Configuration;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;

namespace GustosApp.Infraestructure.Services
{
    public class MercadoPagoService : IPagoService
    {
        private readonly IConfiguration _configuration;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly string _accessToken;
        private readonly string _baseUrl;

        public MercadoPagoService(IConfiguration configuration, IUsuarioRepository usuarioRepository)
        {
            _configuration = configuration;
            _usuarioRepository = usuarioRepository;
            _accessToken = _configuration["MercadoPago:AccessToken"] ?? throw new InvalidOperationException("MercadoPago AccessToken no configurado");
            _baseUrl = _configuration["BaseUrl"] ?? "https://localhost:7000";
            
            MercadoPagoConfig.AccessToken = _accessToken;
        }

        public async Task<string> CrearPreferenciaPagoAsync(string firebaseUid, string email, string nombreCompleto)
        {
            try
            {
                var webhookUrl = _configuration["MercadoPago:WebhookUrl"];
                
                var request = new PreferenceRequest
                {
                    Items = new List<PreferenceItemRequest>
                    {
                        new PreferenceItemRequest
                        {
                            Title = "Plan Premium GustosApp",
                            Quantity = 1,
                            CurrencyId = "ARS",
                            UnitPrice = 50.00m
                        }
                    },
                    Payer = new PreferencePayerRequest
                    {
                        Email = email,
                        Name = nombreCompleto
                    },
                    // CRÍTICO: Guardar el Firebase UID para vincular el pago con el usuario
                    ExternalReference = firebaseUid,
                    NotificationUrl = webhookUrl,
                    BackUrls = new PreferenceBackUrlsRequest
                    {
                        Success = "https://gustosapp.com/pago/exito",
                        Failure = "https://gustosapp.com/pago/fallo",
                        Pending = "https://gustosapp.com/pago/pendiente"
                    }
                };

                var client = new PreferenceClient();
                var preference = await client.CreateAsync(request);

                return preference.InitPoint;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al crear preferencia de pago: {ex.Message}", ex);
            }
        }

        public async Task<bool> ProcesarNotificacionPagoAsync(string pagoId)
        {
            try
            {
                var client = new PaymentClient();
                var payment = await client.GetAsync(Convert.ToInt64(pagoId));

                if (payment.Status == "approved" && !string.IsNullOrEmpty(payment.ExternalReference))
                {
                    // Buscar usuario por Firebase UID (external reference)
                    var usuario = await _usuarioRepository.GetByFirebaseUidAsync(payment.ExternalReference);
                    if (usuario != null)
                    {
                        // Actualizar plan del usuario a Premium
                        usuario.ActualizarAPlan(PlanUsuario.Plus);
                        await _usuarioRepository.SaveChangesAsync();
                        return true;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                // Log del error (en un escenario real usarías un logger)
                Console.WriteLine($"Error al procesar notificación de pago {pagoId}: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> VerificarEstadoPagoAsync(string pagoId)
        {
            try
            {
                var client = new PaymentClient();
                var payment = await client.GetAsync(Convert.ToInt64(pagoId));
                return payment.Status == "approved";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al verificar estado de pago {pagoId}: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> VerificarYProcesarPagosPendientesAsync(string firebaseUid)
        {
            try
            {
                Console.WriteLine($"🔍 [VerificarPagosPendientes] Buscando pagos para FirebaseUid: {firebaseUid}");
                
                // Por ahora, simplemente verificamos si el usuario existe y lo actualizamos
                // En producción, MercadoPago llamará al webhook automáticamente
                var usuario = await _usuarioRepository.GetByFirebaseUidAsync(firebaseUid);
                
                if (usuario == null)
                {
                    Console.WriteLine($"❌ [VerificarPagosPendientes] Usuario no encontrado");
                    return false;
                }

                if (usuario.Plan == PlanUsuario.Plus)
                {
                    Console.WriteLine($"ℹ️ [VerificarPagosPendientes] Usuario ya es Premium");
                    return true;
                }

                // En desarrollo: Actualizamos directamente si hay un pendingPayment en localStorage
                // En producción: Solo el webhook debería actualizar
                Console.WriteLine($"⚠️ [VerificarPagosPendientes] Usuario no es Premium aún. Esperando webhook de MercadoPago...");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [VerificarPagosPendientes] Error: {ex.Message}");
                Console.WriteLine($"❌ StackTrace: {ex.StackTrace}");
                return false;
            }
        }
    }
}