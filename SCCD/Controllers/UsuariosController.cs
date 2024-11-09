using AutoMapper;
using Data.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Model.Entities;
using Dtos;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Security.Cryptography;

namespace SCCD.Controllers
{
    [ApiController]    
    public class UsuariosController : Controller
    {
        private IUsuarioRepositorie _usuariosRepositorie;
        private IPersonaRepositorie _personasRepositorie;
        private IAulaRepositorie _aulaRepositorie;
        private IGrupoRepositorie _grupoRepositorie;
        private ILoginAuditRepositorie _loginAuditRepositorie;
        private readonly IMapper _mapper;
        private Session _session = Session.GetInstance();

        public UsuariosController(IUsuarioRepositorie usuariosRepositorie, IPersonaRepositorie personasRepositorie, IAulaRepositorie aulaRepositorie,
            IGrupoRepositorie grupoRepositorie, ILoginAuditRepositorie loginAuditRepositorie, IMapper mapper)
        {
            _usuariosRepositorie = usuariosRepositorie;
            _personasRepositorie = personasRepositorie;
            _aulaRepositorie = aulaRepositorie;
            _grupoRepositorie = grupoRepositorie;
            _loginAuditRepositorie = loginAuditRepositorie;
            _mapper = mapper;
        }
        
        [HttpPost]
        [Route("/[controller]/[action]")]
        public IActionResult LogIn([FromBody] LoginRequest loginRequest)
        {
            try
            {
                Usuario user = loginRequest.Email != "" ? _usuariosRepositorie.ObtenerUserWthGroupsWithEmail(loginRequest.Email) 
                    : _usuariosRepositorie.ObtenerUserWthGroups(loginRequest.Username, loginRequest.Clave);
                if (user == null)
                {
                    return NotFound("Usuario no encontrado");
                }
                else
                {
                    _session.IdUserLogueado = user.Id.ToString();
                    _session.EmailUserLogueado = user.Email;
                    _session.UserNameUserLogueado = user.Username;
                    var persona = _personasRepositorie.ObtenerPersonaDeUsuario(user.Id);
                    if (persona != null)
                    {
                        LoggedInUser loggedInUser = new LoggedInUser
                        {
                            Id = persona.Id,
                            Nombre = persona.Nombre,
                            Apellido = persona.Apellido,
                            DNI = persona.DNI,
                            Email = persona.Email,
                            Telefono = persona.Telefono,
                            Domicilio = persona.Domicilio,
                            Usuario = persona.Usuario,
                            Institucion = persona.Institucion,
                            NotasRecibidas = persona.NotasRecibidas,
                            NotasLeidas = persona.NotasLeidas,
                            NotasFirmadas = persona.NotasFirmadas,
                            Roles = new List<Grupo>()
                        };
                        if (persona.Usuario.Grupos.Count() > 0)
                        {
                            foreach (var rol in persona.Usuario.Grupos)
                            {
                                loggedInUser.Roles.Add(rol);
                            }
                        }
                        RegistrarLogin();
                        return Ok(loggedInUser);
                    }
                    else
                    {
                        return NotFound("Usuario sin Persona asignada");
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("/[controller]/[action]")]
        public IActionResult RegistrarLogin()
        {
            try
            {                
                var persona = _personasRepositorie.ObtenerPersonaDeUsuario(Convert.ToInt32(_session.IdUserLogueado));
                if (!(persona is Admin))
                {
                    LoginAudit nuevoLogueo = new LoginAudit
                    {
                        UsuarioLogueado = _usuariosRepositorie.ObtenerAsync(Convert.ToInt32(_session.IdUserLogueado)),
                        FechaYHoraLogin = DateTime.Now
                    };

                    _loginAuditRepositorie.Agregar(nuevoLogueo);
                }
                
                return Ok(true);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("/[controller]/[action]")]
        public IActionResult ActualizarLoginAuditEnLogoutAction()
        {
            try
            {
                if (_session.IdUserLogueado != "" && _session.IdUserLogueado != null)
                {
                    var audits = _loginAuditRepositorie.ObtenerLoginsDeUsuario(Convert.ToInt32(_session.IdUserLogueado));
                    var ultimoAudit = audits.Last();
                    ultimoAudit.FechaYHoraLogout = DateTime.Now;

                    _loginAuditRepositorie.Modificar(ultimoAudit);                   
                }

                return Ok(true);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("/[controller]/[action]")]
        public Persona LoggedIn()
        {            
            if (_session.EmailUserLogueado == null || _session.EmailUserLogueado == string.Empty)
            {
                return null;
            }
            else
            {
                var user = _usuariosRepositorie.ObtenerAsync(Convert.ToInt32(_session.IdUserLogueado));
                if (user != null)
                {
                    return _personasRepositorie.ObtenerPersonaDeUsuario(user.Id);
                }
                else
                {
                    return null;
                }
            }
        }

        [HttpGet]
        [Route("/[controller]/[action]")]
        public string GetEmailPersonaLogueada()
        {
            return _session.EmailUserLogueado;
        }


        [HttpGet]
        [Route("/[controller]/[action]")]
        public bool LogOff()
        {
            ActualizarLoginAuditEnLogoutAction();
            _session.EmailUserLogueado = string.Empty;
            _session.UserNameUserLogueado= string.Empty;
            _session.IdUserLogueado = string.Empty;

            return true;
        }

        [HttpGet]
        [Route("/[controller]/[action]")]
        public IActionResult ObtenerTipoPersonaLogueada()
        {
            try
            {
                var persona = _personasRepositorie.ObtenerPersonaDeUsuario(Convert.ToInt32(_usuariosRepositorie.ObtenerAsync(Convert.ToInt32(_session.IdUserLogueado)).Id));

                if (persona != null)
                {
                    if (persona.Usuario.Grupos != null)
                    {
                        List<Grupo> grupos = new List<Grupo>();
                        foreach (var grupo in persona.Usuario.Grupos)
                        {
                            grupos.Add(grupo);
                        }
                        return Ok(grupos);
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

        [HttpGet]
        [Route("/[controller]/[action]")]
        public IActionResult ObtenerUsuariosSistema()
        {
            try
            {
                return Ok(_usuariosRepositorie.ObtenerUsuariosSistema());
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }           
        }

        [HttpGet]
        [Route("/[controller]/[action]")]
        public IActionResult ObtenerUsuariosSinAsignacionPersona()
        {
            try
            {
                List<Usuario> usuariosSinAsignacion = new List<Usuario>(); 
                var usuariosSistema = _usuariosRepositorie.ObtenerUsuariosSistema();
                if (usuariosSistema != null)
                {
                    var personasSistema = _personasRepositorie.ObtenerTodosAsync();
                    if (personasSistema != null)
                    {
                        foreach (var usuario in usuariosSistema)
                        {
                            var usuarioAsignado = personasSistema.Any(x => x.Usuario == usuario);
                            if (!usuarioAsignado)
                            {
                                usuariosSinAsignacion.Add(usuario);
                            }
                        }
                        return Ok(usuariosSinAsignacion);
                    }
                    else
                    {
                        return NotFound(false);
                    }
                }
                else
                {
                    return NotFound(false);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet]
        [Route("/[controller]/[action]/{idUser}")]
        public IActionResult ObtenerRolesUsuario(int idUser)
        {
            try
            {
                if (idUser == 0 || idUser == null)
                {
                    return BadRequest(false);
                }
                else
                {
                    var user = _usuariosRepositorie.ObtenerAsync(idUser);
                    if (user != null && user.Grupos != null)
                    {
                        return Ok(user.Grupos);
                    }
                    else
                    {
                        return NotFound(false);
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost]
        [Route("/[controller]/[action]")]
        public IActionResult AgregarUsuario([FromBody] UsuarioACrear nuevoUser)
        {
            try
            {
                if (nuevoUser != null)
                {
                    var users = _usuariosRepositorie.ObtenerTodosAsync();
                    if (users != null)
                    {
                        foreach (var user in users)
                        {
                            if (nuevoUser.Email == user.Email)
                            {
                                return BadRequest("Ya existe un Usuario con ese Email registrado");
                            }
                            else if(nuevoUser.Username == user.Username)
                            {
                                return BadRequest("Ya existe un Usuario con ese Username registrado");
                            }
                        }
                    }
                    else
                    {
                        return NotFound(false);
                    }
                    Usuario nuevoUsuario = new Usuario
                    {
                        Email = nuevoUser.Email,
                        Username = nuevoUser.Username,
                        Clave = nuevoUser.Clave,
                    };
                    if (nuevoUser.RolesSeleccionados.Count() > 0)
                    {
                        var grupos = _grupoRepositorie.ObtenerTodosAsync();
                        if (grupos != null && grupos.Count() > 0)
                        {
                            foreach (var grupo in grupos)
                            {
                                if (nuevoUser.RolesSeleccionados.Contains(grupo.Tipo))
                                {
                                    nuevoUsuario.Grupos.Add(grupo);
                                }
                            }
                        }
                        else
                        {
                            return NotFound(false);
                        }                   
                    }
                    _usuariosRepositorie.Agregar(nuevoUsuario);
                    if (nuevoUsuario.Grupos.Count() > 0)
                    {
                        _grupoRepositorie.AgregarUserAGrupo(nuevoUsuario);
                    }                    
                    return Ok(true);
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
        [Route("/[controller]/[action]/{idUser}")]        
        public IActionResult EditarUsuario(int idUser, [FromBody] UsuarioACrear usuarioAModificar)
        {
            try
            {
                if (idUser == 0 || idUser == null || usuarioAModificar == null)
                {
                    return BadRequest(false);
                }
                else
                {
                    var users = _usuariosRepositorie.ObtenerTodosAsync();
                    if (users == null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        foreach (var user in users)
                        {
                            if (user.Id != idUser)
                            {
                                if (user.Email == usuarioAModificar.Email)
                                {
                                    return BadRequest("Ya existe un Usuario con ese Email registrado");
                                }
                                else if (user.Username == usuarioAModificar.Username)
                                {
                                    return BadRequest("Ya existe un Usuario con ese Username registrado");
                                }
                            }                            
                        }
                    }
                    var usuario = _usuariosRepositorie.ObtenerAsync(idUser);
                    if (usuario != null)
                    {
                        usuario.Email = usuarioAModificar.Email;
                        usuario.Username = usuarioAModificar.Username;
                        if (usuario.Clave != usuarioAModificar.Clave)
                        {
                            usuario.Clave = usuarioAModificar.Clave;
                        }
                        var grupos = _grupoRepositorie.ObtenerTodosAsync();
                        foreach (var rol in usuarioAModificar.RolesSeleccionados)
                        {
                            var contieneRol = usuario.Grupos.Any(x => x.Tipo == rol);
                            if (!contieneRol)
                            {
                                var rolAAgregar = grupos.Where(x => x.Tipo == rol).FirstOrDefault();
                                if (rolAAgregar != null)
                                {
                                    usuario.Grupos.Add(rolAAgregar);
                                }
                                else
                                {
                                    return NotFound(false);
                                }
                            }
                        }

                        ICollection<Grupo> gruposUserCopia = new List<Grupo>();
                        gruposUserCopia = usuario.Grupos.ToList();

                        foreach (var grupoDeUsuario in gruposUserCopia)
                        {
                            var contieneRol = usuarioAModificar.RolesSeleccionados.Any(x => x == grupoDeUsuario.Tipo);
                            if (!contieneRol)
                            {
                                usuario.Grupos.Remove(grupoDeUsuario);
                                if (grupoDeUsuario.Tipo == "Docente")
                                {
                                    var personaDeUsuario = _personasRepositorie.ObtenerPersonaDeUsuario(usuario.Id);
                                    if (personaDeUsuario != null)
                                    {
                                        _aulaRepositorie.EliminarDocenteDeAulasAsignadas(personaDeUsuario.Id);
                                    }
                                    else
                                    {
                                        return NotFound(false);
                                    }

                                }
                            }
                        }                  
                        
                        _usuariosRepositorie.Modificar(usuario);
                        if (usuario.Grupos.Count() > 0)
                        {
                            _grupoRepositorie.AgregarUserAGrupo(usuario);
                        }
                        return Ok(true);
                    }
                    else
                    {
                        return NotFound(false);
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet]
        [Route("/[controller]/[action]/{email}")]                
        public string RecuperarClave(string email)
        {
            if (email != null)
            {
                var existe = _usuariosRepositorie.UsuarioExistente(email);
                if (existe)
                {
                    var token = GenerateToken();
                    EnviarToken(email, token, "recuperacionClave");
                    return token;
                }
                else
                {
                    return "ERROR";
                }

            }
            return "ERROR";
        }

        [HttpGet]
        [Route("/[controller]/[action]/{claveAdmin}")]
        public IActionResult ValidarClaveAdmin(string ClaveAdmin)
        {
            try
            {
                if (ClaveAdmin != null && ClaveAdmin != "")
                {
                    var userLogueado = _usuariosRepositorie.ObtenerAsync(Convert.ToInt32(_session.IdUserLogueado));
                    if (userLogueado != null)
                    {
                        if (userLogueado.Clave == ClaveAdmin)
                        {
                            return Ok(true);
                        }
                        else
                        {
                            return BadRequest(false);
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
            catch (Exception)
            {
                return BadRequest(false);
            }
           
        }

        [HttpGet]
        [Route("/[controller]/[action]/{email}/{firmaDe}")]
        public string EnviarTokenSeguridad(string email, string firmaDe)
        {
            if (email != null)
            {
                var token = GenerateToken();
                if (firmaDe == "Historial")
                {
                    EnviarToken(email, token, "firmaHistorial");
                }
                else
                {
                    EnviarToken(email, token, "firmaNota");
                }
                
                return token;             

            }
            return "ERROR";
        }

        [HttpGet]
        [Route("/[controller]/[action]")]
        public IActionResult ObtenerRolesSistema()
        {
            try
            {
                return Ok(_usuariosRepositorie.ObtenerRolesSistema());
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }            
        }

        public static string GenerateToken()
        {
             Random random = new Random();
             string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
             string numbers = "0123456789";
            var letterChars = new string(Enumerable.Repeat(letters, 3)
                .Select(s => s[random.Next(s.Length)]).ToArray());

            var numberChars = new string(Enumerable.Repeat(numbers, 3)
                .Select(s => s[random.Next(s.Length)]).ToArray());

            return letterChars + numberChars;
        }        


        [HttpPut]
        [Route("/[controller]/[action]")]        
        public bool RecuperacionClave([FromBody] RecuperacionClave clave)
        {
            if (clave.ClaveNueva != null)
            {                
                _usuariosRepositorie.RecuperarClaveUser(clave.EmailUsuario, clave.ClaveNueva);
                return true;                
            }
            return false;
        }

        [HttpPut]
        [Route("/[controller]/[action]")]
        public IActionResult ResetearClave([FromBody] ReseteoClave clave)
        {
            try
            {
                if (clave != null)
                {
                    var userLogueado = _usuariosRepositorie.ObtenerAsync(Convert.ToInt32(_session.IdUserLogueado));
                    if (userLogueado != null)
                    {
                        if (userLogueado.Clave != clave.ClaveActual)
                        {
                            return BadRequest("La clave actual es incorrecta");
                        }
                        else if(clave.ClaveNueva == clave.ConfirmacionClaveNueva)
                        {
                            _usuariosRepositorie.ActualizarClaveUser(userLogueado.Id, clave.ClaveNueva);
                            return Ok(true);
                        }
                        else
                        {
                            return BadRequest("La clave nueva y su confirmacion no son iguales");
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
            catch (Exception)
            {
                return BadRequest(false);
            }
        }

        [HttpGet]
        [Route("/[controller]/[action]")]    
        public void EnviarToken(string email, string token, string motivo)
        {
            SmtpClient smtpClient = new SmtpClient();
            smtpClient.Host = "smtp.gmail.com";
            smtpClient.Port = 587;
            smtpClient.EnableSsl = true;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            string appPassword = config["AppPassword"];

            smtpClient.Credentials = new NetworkCredential("noreply.sccd@gmail.com", appPassword);

            MailMessage mailMessage = new MailMessage();
            mailMessage.Bcc.Add(email);

            
            MailAddress mail = new MailAddress("noreply.sccd@gmail.com");

            mailMessage.From = mail;
            mailMessage.IsBodyHtml = true;
            mailMessage.Priority = MailPriority.High;
            string base64 = "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";
            byte[] imageBytes = Convert.FromBase64String(base64);
            string imageName = Guid.NewGuid().ToString() + ".png";

            string htmlTemplate = $@"
            <html>
                <head>
                <link rel='preconnect' href='https://fonts.googleapis.com'>
                <link rel='preconnect' href='https://fonts.gstatic.com' crossorigin>
                <link href='https://fonts.googleapis.com/css2?family=Lato:ital,wght@0,100;0,300;0,400;0,700;0,900;1,100;1,300;1,400;1,700;1,900&display=swap' rel='stylesheet'>                    
                </head>
                <body>
                    <table style='border-radius: 40px; background-color: #2B89AB; margin: auto; width: 433px; height: 650px;'>
                        <tr style='text-align: center;'>
                            <td style='height: 25px;'>
                            <img src='https://cdn-icons-png.flaticon.com/512/16116/16116728.png' width='71' height='72' alt='' title='' class='img-small'>
                            </td>
                        </tr>
                        <tr>
                            <td style='border-radius: 40px; background-color: white; padding: 0px 20px 20px 20px;'>
                                <div class='divTextContainer' style='width: 99%; margin: 0px 4px 4px 4px;'>
                                    <img width='192' height='205' border=""0"" src='cid:{imageName}' style='display: block; background-position: center; background-repeat: no-repeat; background-size: contain; border: none; margin-left: 96px; margin-right: 96px; margin-bottom: 22px; margin-top: 10px;'>
                                    <p style='sans-serif; width: 100%; text-align: center; margin: 0;'>Su Token de Seguridad es:</p>                                                                        
                                    <p style='sans-serif; width: 100%; text-align: center; margin: 0;'>
                                        <strong style='display: inline-block; font-size: 24px; font-weight: bold; background-color: #f5f5f5; padding: 10px 20px; border-radius: 10px; margin-left: 120px; margin-right: 120px;'>
                                            {token.Substring(0, 3)}-{token.Substring(3)}
                                        </strong>
                                    </p>
                                    <p style='sans-serif; width: 100%; text-align: center;'><strong style='margin-left: 110px; margin-right: 110px; margin-bottom: unset;'>No lo comparta con nadie.</strong></p>
                                    <br>
                                    <p style = sans-serif; width: 100%; font-weight: 700; text-align: center; margin: 0;' > Nota: Esta dirección de correo envía emails automáticos. Por favor, no responda.</p>                              
                                    <p style = sans-serif; width: 100%; font-weight: 700; text-align: center; margin: 0;' >Ante cualquier duda o inconveniente, comuníquese con el docente y/o directivo correspondiente.</p>
                                    <br>
                                    <p style = sans-serif; width: 100%; font-weight: bold; text-align: center; margin: 0; text-decoration: underline;' ><strong><u>Referencias:</u></strong></p>
                                    <table style='width: 60%;'>
                                        <tr>
                                            <td style='text-align: left;'>
                                                <div style='display: inline-block; width: 15px; height: 15px; background-color: blue; margin-right: 5px;'></div> Nuevo
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style='text-align: left;'>
                                                <div style='display: inline-block; width: 15px; height: 15px; background-color: darkgreen; margin-right: 5px;'></div> Aceptado/Firmado
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style='text-align: left;'>
                                                <div style='display: inline-block; width: 15px; height: 15px; background-color: red; margin-right: 5px;'></div> Denegado
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style='text-align: left;'>
                                                <div style='display: inline-block; width: 15px; height: 15px; background-color: yellow; margin-right: 5px;'></div> Leido/Modificado
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style='text-align: left;'>
                                                <div style='display: inline-block; width: 15px; height: 15px; background-color: deepskyblue; margin-right: 5px;'></div> Seguridad
                                            </td>
                                        </tr>
                                    </table>                           
                                </div>
                            </td>
                        </tr>
                    </table>
                </body>
            </ html > ";

            mailMessage.Body = htmlTemplate;
            MemoryStream ms = new MemoryStream(imageBytes);
            LinkedResource inline = new LinkedResource(ms, "image/png")
            {
                ContentId = imageName,
                TransferEncoding = TransferEncoding.Base64
            };

            AlternateView view = AlternateView.CreateAlternateViewFromString(htmlTemplate, null, MediaTypeNames.Text.Html);
            view.LinkedResources.Add(inline);

            mailMessage.AlternateViews.Add(view);            

            HttpContext.Session.SetString("SecurityToken", mailMessage.Body);
            HttpContext.Session.SetString("emailUsuarioRecuperaClave", email);

            mailMessage.BodyEncoding = System.Text.Encoding.UTF8;
            if (motivo == "recuperacionClave")
            {
                mailMessage.Subject = "Recuperacion de Clave - Token de Seguridad";
            }
            else if(motivo == "firmaHistorial")
            {
                mailMessage.Subject = "Firma de Historial - Token de Seguridad";
            }else if(motivo == "firmaNota")
            {
                mailMessage.Subject = "Firma de Nota - Token de Seguridad";
            }
            
            mailMessage.SubjectEncoding = System.Text.Encoding.UTF8;

            smtpClient.Send(mailMessage);
        }        
  
        [HttpGet]
        [Route("/[controller]/[action]")]
        static string Encrypt(string clave)
        {
            try
            {
                string claveEncrypted = "";
                string publickey = "12345678";
                string secretkey = "87654321";
                byte[] secretkeyByte = { };
                secretkeyByte = System.Text.Encoding.UTF8.GetBytes(secretkey);
                byte[] publickeybyte = { };
                publickeybyte = System.Text.Encoding.UTF8.GetBytes(publickey);
                MemoryStream ms = null;
                CryptoStream cs = null;
                byte[] inputbyteArray = System.Text.Encoding.UTF8.GetBytes(clave);
                using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
                {
                    ms = new MemoryStream();
                    cs = new CryptoStream(ms, des.CreateEncryptor(publickeybyte, secretkeyByte), CryptoStreamMode.Write);
                    cs.Write(inputbyteArray, 0, inputbyteArray.Length);
                    cs.FlushFinalBlock();
                    claveEncrypted = Convert.ToBase64String(ms.ToArray());
                }
                return claveEncrypted;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        [HttpDelete]
        [Route("/[controller]/[action]/{idUser}")]        
        public IActionResult EliminarUsuario(int idUser)
        {
            try
            {
                if (idUser == 0 || idUser == null)
                {
                    return BadRequest(false);
                }
                else
                {
                    var personaAsignada = _personasRepositorie.ObtenerPersonaDeUsuario(idUser);
                    if (personaAsignada != null)
                    {
                        _aulaRepositorie.CheckearAulasAsignadasAPersona(personaAsignada);
                        _personasRepositorie.Borrar(personaAsignada.Id);
                    }
                    _usuariosRepositorie.Borrar(idUser);
                    return Ok(true);
                }                
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
           
        }      
    }
}
