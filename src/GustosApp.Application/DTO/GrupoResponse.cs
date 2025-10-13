namespace GustosApp.Application.DTO
{
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
        public List<MiembroGrupoResponse> Miembros { get; set; } = new List<MiembroGrupoResponse>();

        public GrupoResponse(Guid id, string nombre, string? descripcion, Guid administradorId, 
            string? administradorFirebaseUid, string administradorNombre, DateTime fechaCreacion, bool activo, 
            string? codigoInvitacion, DateTime? fechaExpiracionCodigo, int cantidadMiembros)
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

    public class MiembroGrupoResponse
    {
        public Guid Id { get; set; }
        public Guid UsuarioId { get; set; }
        public string? UsuarioFirebaseUid { get; set; }
        public string UsuarioNombre { get; set; } = string.Empty;
        public string UsuarioEmail { get; set; } = string.Empty;
        public DateTime FechaUnion { get; set; }
        public bool EsAdministrador { get; set; }

        public MiembroGrupoResponse(Guid id, Guid usuarioId, string? usuarioFirebaseUid, string usuarioNombre, 
            string usuarioEmail, DateTime fechaUnion, bool esAdministrador)
        {
            Id = id;
            UsuarioId = usuarioId;
            UsuarioFirebaseUid = usuarioFirebaseUid;
            UsuarioNombre = usuarioNombre;
            UsuarioEmail = usuarioEmail;
            FechaUnion = fechaUnion;
            EsAdministrador = esAdministrador;
        }
    }
}
