using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GustosApp.Domain.Interfaces
{
    public interface IPagoService
    {
        Task<string> CrearPreferenciaPagoAsync(string firebaseUid, string email, string nombreCompleto);
        Task<bool> ProcesarNotificacionPagoAsync(string pagoId);
        Task<bool> VerificarEstadoPagoAsync(string pagoId);
        Task<bool> VerificarYProcesarPagosPendientesAsync(string firebaseUid);
    }
}