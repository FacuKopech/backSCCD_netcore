using Data.Contracts;
using Microsoft.AspNetCore.Mvc;
using Model.Entities;

namespace SCCD.Controllers
{
    [ApiController]
    public class InstitucionesController : Controller
    {
        private IInstitucionRepositorie _institucionRepositorie;

        public InstitucionesController(IInstitucionRepositorie institucionRepositorie)
        {
            _institucionRepositorie = institucionRepositorie;
        }


        [HttpGet]
        [Route("/[controller]/[action]")]
        public IActionResult ObtenerInstitucionesSistema()
        {
            try
            {
                return Ok(_institucionRepositorie.ObtenerTodosAsync());
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost]
        [Route("/[controller]/[action]")]
        public IActionResult AgregarInstitucion([FromBody] Institucion nuevaInstitucion)
        {
            try
            {
                if (nuevaInstitucion != null)
                {
                    var instituciones = _institucionRepositorie.ObtenerTodosAsync();
                    if (instituciones != null)
                    {
                        if (instituciones.Any(x => x.Nombre == nuevaInstitucion.Nombre))
                        {
                            return BadRequest("Ya existe una Institucion con ese Nombre registrado");
                        }else if (instituciones.Any(x => x.Telefono == nuevaInstitucion.Telefono))
                        {
                            return BadRequest("Ya existe una Institucion con ese Telefono registrado");
                        }
                        else
                        {
                            Institucion institucionAAgregar = new Institucion { 
                                Nombre = nuevaInstitucion.Nombre,
                                Direccion = nuevaInstitucion.Direccion,
                                Telefono = nuevaInstitucion.Telefono,
                                Ciudad = nuevaInstitucion.Ciudad
                            };
                            _institucionRepositorie.Agregar(institucionAAgregar);
                            return Ok(true);
                        }
                    }
                    else
                    {
                        return NotFound(false);
                    }
                }
                else
                {
                    return BadRequest(false);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPut]
        [Route("/[controller]/[action]/{IdInstitucion}")]
        public IActionResult EditarInstitucion(int IdInstitucion, [FromBody] Institucion institucionModificar)
        {
            try
            {
                if (IdInstitucion != null && IdInstitucion > 0 && institucionModificar != null)
                {
                    var instituciones = _institucionRepositorie.ObtenerTodosAsync();
                    var institucion = _institucionRepositorie.ObtenerAsync(IdInstitucion);
                    if (instituciones != null)
                    {
                        if (instituciones.Any(x => x.Nombre == institucionModificar.Nombre && x.Id != institucion.Id))
                        {
                            return BadRequest("Ya existe una Institucion con ese Nombre registrado");
                        }
                        else if (instituciones.Any(x => x.Telefono == institucionModificar.Telefono && x.Id != institucion.Id))
                        {
                            return BadRequest("Ya existe una Institucion con ese Telefono registrado");
                        }
                        else
                        {
                            
                            if (institucion != null)
                            {
                                institucion.Nombre = institucionModificar.Nombre;
                                institucion.Direccion = institucionModificar.Direccion;
                                institucion.Telefono = institucionModificar.Telefono;
                                institucion.Ciudad = institucionModificar.Ciudad;
                                _institucionRepositorie.Modificar(institucion);
                            }

                            return Ok(true);
                        }
                    }
                    else
                    {
                        return NotFound(false);
                    }
                }
                else
                {
                    return BadRequest(false);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }


        [HttpDelete]
        [Route("/[controller]/[action]/{idInstitucion}")]
        public IActionResult EliminarInstitucion(int IdInstitucion)
        {
            try
            {
                if (IdInstitucion != null && IdInstitucion > 0)
                {
                    _institucionRepositorie.Borrar(IdInstitucion);
                }
                else
                {
                    return BadRequest(false);
                }
                return Ok(true);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
