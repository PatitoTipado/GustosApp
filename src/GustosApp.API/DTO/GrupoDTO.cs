using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GustosApp.API.DTO
{
    // -------------------------
    // 📦 Respuestas generales
    // -------------------------

    public class AbandonarGrupoResponse
    {
        public bool Success { get; set; }
        public string Mensaje { get; set; } = string.Empty;
    }

    public class EliminarGrupoResponse
    {
        public bool Success { get; set; }
        public string Mensaje { get; set; } = string.Empty;
    }

    public class ActualizarGustosGrupoResponse
    {
        public bool Success { get; set; }
        public string Mensaje { get; set; } = string.Empty;
    }

    public class RemoverMiembroResponse
    {
        public bool Success { get; set; }
        public string Mensaje { get; set; } = string.Empty;
    }

    public class ActualizarNombreGrupoRequest
    {
        [Required(ErrorMessage = "El nombre del grupo es requerido")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "El nombre debe tener entre 1 y 100 caracteres")]
        public string Nombre { get; set; } = string.Empty;
    }

    // -------------------------
    // 📦 Grupo principal
    // -------------------------

    public class GrupoResponse
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public Guid AdministradorId { get; set; }
        public string? AdministradorFirebaseUid { get; set; }
        public string AdministradorNombre { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; }
        public bool Activo { get; set; }
        public string? CodigoInvitacion { get; set; }
        public DateTime? FechaExpiracionCodigo { get; set; }
        public int CantidadMiembros { get; set; }
        public List<MiembroGrupoResponse> Miembros { get; set; } = new();

        public GrupoResponse() { }

        public GrupoResponse(
            Guid id,
            string nombre,
            string? descripcion,
            Guid administradorId,
            string? administradorFirebaseUid,
            string administradorNombre,
            DateTime fechaCreacion,
            bool activo,
            string? codigoInvitacion,
            DateTime? fechaExpiracionCodigo,
            int cantidadMiembros)
        {
            Id = id;
            Nombre = nombre;
            Descripcion = descripcion;
            AdministradorId = administradorId;
            AdministradorFirebaseUid = administradorFirebaseUid;
            AdministradorNombre = administradorNombre;
            FechaCreacion = fechaCreacion;
            Activo = activo;
            CodigoInvitacion = codigoInvitacion;
            FechaExpiracionCodigo = fechaExpiracionCodigo;
            CantidadMiembros = cantidadMiembros;
        }
    }

    // -------------------------
    // 📦 Miembros del grupo
    // -------------------------

    public class MiembroGrupoResponse
    {
        public Guid Id { get; set; }
        public Guid UsuarioId { get; set; }
        public string? UsuarioFirebaseUid { get; set; }
        public string UsuarioNombre { get; set; } = string.Empty;
        public string UsuarioEmail { get; set; } = string.Empty;
        public string UsuarioUsername { get; set; } = string.Empty;
        public DateTime FechaUnion { get; set; }
        public bool EsAdministrador { get; set; }
        public bool afectarRecomendacion { get; set; }

        public string? FotoPerfilUrl { get; set; }
        public MiembroGrupoResponse() { }

        public MiembroGrupoResponse(
            Guid id,
            Guid usuarioId,
            string? usuarioFirebaseUid,
            string usuarioNombre,
            string usuarioEmail,
            string usuarioUsername,
            DateTime fechaUnion,
            bool esAdministrador,
            bool afectarRecomendacion,
           string? FotoPerfilUrl)
        {
            Id = id;
            UsuarioId = usuarioId;
            UsuarioFirebaseUid = usuarioFirebaseUid;
            UsuarioNombre = usuarioNombre;
            UsuarioEmail = usuarioEmail;
            UsuarioUsername = usuarioUsername;
            FechaUnion = fechaUnion;
            EsAdministrador = esAdministrador;
            this.afectarRecomendacion = afectarRecomendacion;
            this.FotoPerfilUrl = FotoPerfilUrl;
        }
    }

    // -------------------------
    // 📦 Invitaciones
    // -------------------------

    public class InvitacionGrupoRequest
    {
        // Puede aceptar Email, Id o Username
        [EmailAddress(ErrorMessage = "El formato del email no es válido")]
        public string? EmailUsuario { get; set; }

        public Guid? UsuarioId { get; set; }

        // Buscar por username si no hay email o ID
        public string? UsuarioUsername { get; set; }

        [StringLength(200, ErrorMessage = "El mensaje personalizado no puede exceder los 200 caracteres")]
        public string? MensajePersonalizado { get; set; }
    }
    public class CrearGrupoRequest
    {
        [Required(ErrorMessage = "El nombre del grupo es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "La descripción no puede exceder los 500 caracteres")]
        public string? Descripcion { get; set; }
    }
    public class InvitacionGrupoResponse
    {
        public Guid Id { get; set; }
        public Guid GrupoId { get; set; }
        public string GrupoNombre { get; set; } = string.Empty;
        public Guid UsuarioInvitadoId { get; set; }
        public string UsuarioInvitadoNombre { get; set; } = string.Empty;
        public string UsuarioInvitadoEmail { get; set; } = string.Empty;
        public Guid UsuarioInvitadorId { get; set; }
        public string UsuarioInvitadorNombre { get; set; } = string.Empty;
        public DateTime FechaInvitacion { get; set; }
        public DateTime? FechaRespuesta { get; set; }
        public string Estado { get; set; } = string.Empty;
        public string? MensajePersonalizado { get; set; }
        public DateTime FechaExpiracion { get; set; }

        public InvitacionGrupoResponse() { }

        public InvitacionGrupoResponse(
            Guid id,
            Guid grupoId,
            string grupoNombre,
            Guid usuarioInvitadoId,
            string usuarioInvitadoNombre,
            string usuarioInvitadoEmail,
            Guid usuarioInvitadorId,
            string usuarioInvitadorNombre,
            DateTime fechaInvitacion,
            DateTime? fechaRespuesta,
            string estado,
            string? mensajePersonalizado,
            DateTime fechaExpiracion)
        {
            Id = id;
            GrupoId = grupoId;
            GrupoNombre = grupoNombre;
            UsuarioInvitadoId = usuarioInvitadoId;
            UsuarioInvitadoNombre = usuarioInvitadoNombre;
            UsuarioInvitadoEmail = usuarioInvitadoEmail;
            UsuarioInvitadorId = usuarioInvitadorId;
            UsuarioInvitadorNombre = usuarioInvitadorNombre;
            FechaInvitacion = fechaInvitacion;
            FechaRespuesta = fechaRespuesta;
            Estado = estado;
            MensajePersonalizado = mensajePersonalizado;
            FechaExpiracion = fechaExpiracion;
        }

    }
}


