using AutoMapper;
using Data.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace SCCD.Controllers
{
    [ApiController]
    public class GruposController : Controller
    {
        private IGrupoRepositorie _gruposRepositorie;
        private IUsuarioRepositorie _usuariosRepositorie;
        private readonly IMapper _mapper;

        public GruposController(IGrupoRepositorie grupoRepositorie,IUsuarioRepositorie usuariosRepositorie, IMapper mapper)
        {
            _gruposRepositorie = grupoRepositorie;
            _usuariosRepositorie = usuariosRepositorie;
            _mapper = mapper;
        }
     
    }
}
