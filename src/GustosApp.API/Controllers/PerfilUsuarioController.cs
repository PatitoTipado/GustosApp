using AutoMapper;
using GustosApp.API.DTO;
using GustosApp.Application.UseCases.UsuarioUseCases;
using GustosApp.Domain.Common;
using GustosApp.Domain.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GustosApp.API.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class PerfilUsuarioController : BaseApiController
    {
        private readonly ActualizarPerfilUsuarioUseCase _actualizarUsuario;
        private readonly ObtenerRestaurantesFavoritosUseCase _obtenerFavoritos;
        private readonly IMapper _mapper;


        public PerfilUsuarioController(ActualizarPerfilUsuarioUseCase actualizarUsuario,
            ObtenerRestaurantesFavoritosUseCase obtenerFavoritos, IMapper mapper)
        {

            _actualizarUsuario = actualizarUsuario;
            _obtenerFavoritos = obtenerFavoritos;
            _mapper = mapper;
        }

        
        [HttpPut]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> EditarDatosPerfil(
           [FromForm] EditarDatosUsuarioDTO dto,
           CancellationToken ct)
        {
            var uid = GetFirebaseUid();
            if (uid == null)
                return Unauthorized("UID inválido");

            FileUpload? foto = null;

            if (dto.FotoPerfil != null)
            {
                foto = new FileUpload
                {
                    Content = dto.FotoPerfil.OpenReadStream(),
                    FileName = dto.FotoPerfil.FileName,
                    ContentType = dto.FotoPerfil.ContentType
                };
            }

            var usuario = await _actualizarUsuario.HandleAsync(
                uid, dto.Email, dto.Nombre,dto.Apellido,
                foto, dto.EsPrivado, ct
            );

            return Ok(new
            {
                message = "Perfil actualizado",
                usuario = new
                {
                  
                    usuario.Email,
                    usuario.Nombre,
                    usuario.Apellido,
                    usuario.FotoPerfilUrl,
                    usuario.EsPrivado
                }
            });
        }

       
        [HttpGet("favoritos")]
        public async Task<IActionResult> GetFavoritos(CancellationToken ct)
        {
            var uid = GetFirebaseUid();
            var favoritos = await _obtenerFavoritos.HandleAsync(uid, ct);

             var response= _mapper.Map<List<RestauranteFavoritoDto>>(favoritos);
            return Ok(favoritos);
        }

    }

}
