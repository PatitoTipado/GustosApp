namespace GustosApp.API.DTO
{
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

        public InvitacionGrupoResponse(Guid id, Guid grupoId, string grupoNombre, 
            Guid usuarioInvitadoId, string usuarioInvitadoNombre, string usuarioInvitadoEmail,
            Guid usuarioInvitadorId, string usuarioInvitadorNombre, DateTime fechaInvitacion,
            DateTime? fechaRespuesta, string estado, string? mensajePersonalizado, DateTime fechaExpiracion)
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
