﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using scg_clinicasur.Data;
using scg_clinicasur.Models;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace scg_clinicasur.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly ApplicationDbContext _context;

        // Inyección del DbContext a través del constructor
        public UsuariosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Método Index para mostrar el listado de usuarios con filtro de roles
        [HttpGet]
        public async Task<IActionResult> Index(string roles, string estado, string searchName)
        {
            // Verificar el rol del usuario en la sesión
            var userRole = HttpContext.Session.GetString("UserRole");

            // Si el usuario no es administrador o doctor, no permitir acceso
            if (userRole != "doctor" && userRole != "administrador")
            {
                return Forbid(); // O redirigir a una página de error
            }

            var usuarios = _context.Usuarios.Include(u => u.roles).AsQueryable();

            // Filtrar por rol "paciente" si es un doctor
            if (userRole == "doctor")
            {
                usuarios = usuarios.Where(u => u.roles.nombre_rol.ToLower() == "paciente");
            }
            else if (!string.IsNullOrEmpty(roles)) // Filtrar por roles si no es doctor
            {
                var selectedRoles = roles.Split(',');
                usuarios = usuarios.Where(u => selectedRoles.Contains(u.roles.nombre_rol.ToLower()));
            }

            // Filtrar por estado
            if (!string.IsNullOrEmpty(estado))
            {
                var selectedEstados = estado.Split(',');
                usuarios = usuarios.Where(u => selectedEstados.Contains(u.estado.ToLower()));
            }

            // Filtrar por nombre o apellido
            if (!string.IsNullOrEmpty(searchName))
            {
                usuarios = usuarios.Where(u => (u.nombre + " " + u.apellido).ToLower().Contains(searchName.ToLower()));
            }

            ViewData["searchName"] = searchName;
            return View(await usuarios.ToListAsync());
        }

        [HttpGet]
        public IActionResult Crear()
        {
            // Obtener el rol del usuario en la sesión
            var userRole = HttpContext.Session.GetString("UserRole");

            // Verificar si el usuario tiene acceso
            if (userRole != "doctor" && userRole != "administrador")
            {
                return Forbid(); // O redirigir a una página de error o acceso denegado
            }

            // Si el usuario es doctor, solo puede crear pacientes
            if (userRole == "doctor")
            {
                ViewBag.Roles = new SelectList(_context.Roles.Where(r => r.nombre_rol == "paciente"), "id_rol", "nombre_rol");
            }
            else if (userRole == "administrador")
            {
                // Los administradores pueden crear usuarios con cualquier rol
                ViewBag.Roles = new SelectList(_context.Roles, "id_rol", "nombre_rol");
            }

            // Estados permitidos
            ViewBag.Estados = new SelectList(new[] { "activo", "inactivo" });

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Usuario usuario)
        {
            // Obtener el rol del usuario en la sesión
            var userRole = HttpContext.Session.GetString("UserRole");

            // Validar que la contraseña sea requerida solo en la creación
            if (string.IsNullOrWhiteSpace(usuario.contraseña))
            {
                ModelState.AddModelError("contraseña", "La contraseña es requerida.");
            }

            // Comprobar si ya existe un usuario con el mismo correo electrónico
            var existingUser = await _context.Usuarios.SingleOrDefaultAsync(u => u.correo.ToLower() == usuario.correo.ToLower());
            if (existingUser != null)
            {
                ModelState.AddModelError("correo", "Ya existe un usuario registrado con este correo electrónico.");
            }

            // Si el usuario es doctor, validar que solo puede crear pacientes
            if (userRole == "doctor" && usuario.id_rol != _context.Roles.SingleOrDefault(r => r.nombre_rol == "paciente").id_rol)
            {
                ModelState.AddModelError("id_rol", "No tienes permiso para crear este tipo de usuario.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Establecer la fecha de registro del usuario
                    usuario.fecha_registro = DateTime.Now;

                    // Guardar el nuevo usuario en la base de datos si el modelo es válido
                    _context.Add(usuario);
                    await _context.SaveChangesAsync();

                    // Obtener el rol del usuario creado
                    var rolUsuario = await _context.Roles
                                        .Where(r => r.id_rol == usuario.id_rol)
                                        .Select(r => r.nombre_rol)
                                        .FirstOrDefaultAsync();

                    // Enviar notificación por correo electrónico
                    var smtpClient = new SmtpClient("smtp.outlook.com")
                    {
                        Port = 587,
                        Credentials = new NetworkCredential("jrojas30463@ufide.ac.cr", "QsEfT0809*"),
                        EnableSsl = true,
                    };

                    string subject = "Bienvenido/a al Sistema";
                    string body = $"Hola {usuario.nombre},<br/><br/>" +
                                  $"Tu cuenta ha sido creada exitosamente en nuestro sistema.<br/><br/>" +
                                  $"Tus credenciales de acceso son:<br/>" +
                                  $"Usuario: {usuario.correo}<br/>" +
                                  $"Contraseña: La que elegiste durante el registro.<br/>" +
                                  $"Rol: {rolUsuario}<br/><br/>" +  // Incluir el nombre del rol
                                  $"Por favor, accede al sistema utilizando tus credenciales.<br/><br/>" +
                                  $"Gracias.";

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress("jrojas30463@ufide.ac.cr"),
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = true,
                    };
                    mailMessage.To.Add(usuario.correo);

                    try
                    {
                        // Intentar enviar el correo
                        await smtpClient.SendMailAsync(mailMessage);
                        TempData["SuccessMessage"] = "Usuario creado exitosamente y se ha enviado una notificación al correo.";
                    }
                    catch (Exception ex)
                    {
                        // Manejar errores en el envío del correo
                        TempData["ErrorMessage"] = $"El usuario fue creado, pero ocurrió un error al enviar el correo: {ex.Message}";
                    }

                    return RedirectToAction("Index");
                }
                catch (DbUpdateException ex)
                {
                    // Error de base de datos
                    TempData["ErrorMessage"] = "Hubo un error al intentar registrar el usuario. Por favor, inténtalo nuevamente más tarde.";
                }
                catch (TimeoutException ex)
                {
                    // Error de tiempo de espera
                    TempData["ErrorMessage"] = "El sistema tardó demasiado en responder. Por favor, inténtalo más tarde.";
                }
                catch (Exception ex)
                {
                    // Cualquier otro tipo de excepción
                    TempData["ErrorMessage"] = "Ocurrió un error inesperado. Por favor, intenta nuevamente más tarde.";
                }
            }

            // Si el modelo no es válido o hubo errores, volver a cargar los roles y estados para mostrarlos en la vista
            if (userRole == "doctor")
            {
                ViewBag.Roles = new SelectList(_context.Roles.Where(r => r.nombre_rol == "paciente"), "id_rol", "nombre_rol");
            }
            else if (userRole == "administrador")
            {
                ViewBag.Roles = new SelectList(_context.Roles, "id_rol", "nombre_rol");
            }

            ViewBag.Estados = new SelectList(new[] { "activo", "inactivo" });
            return View(usuario);
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            // Buscamos el usuario por su ID
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            // Cargar los roles y estados para el dropdown
            ViewBag.Roles = new SelectList(_context.Roles, "id_rol", "nombre_rol", usuario.id_rol);
            ViewBag.Estados = new SelectList(new[] { "activo", "inactivo" }, usuario.estado);  // Cargar el estado actual
            return View(usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, Usuario usuario)
        {
            if (id != usuario.id_usuario)
            {
                return BadRequest("El ID en la URL no coincide con el ID del usuario.");
            }

            try
            {
                // Buscar el usuario existente en la base de datos
                var usuarioExistente = await _context.Usuarios.FindAsync(id);
                if (usuarioExistente == null)
                {
                    TempData["ErrorMessage"] = "El usuario no existe.";
                    return RedirectToAction("Index"); // Redirigir al index si no se encuentra el usuario
                }

                // Eliminar la validación de contraseña si el campo está vacío
                bool isPasswordUpdated = !string.IsNullOrWhiteSpace(usuario.contraseña); // Comprobar si la contraseña se actualizó
                if (!isPasswordUpdated)
                {
                    // Remover el error de validación relacionado con la contraseña en el servidor
                    ModelState.Remove("contraseña");
                }

                if (ModelState.IsValid)
                {
                    // Solo actualizar la contraseña si se ha proporcionado una nueva
                    if (isPasswordUpdated)
                    {
                        // Implementar el hash de la contraseña si es necesario
                        usuarioExistente.contraseña = usuario.contraseña;
                    }

                    // Actualizamos los demás campos
                    usuarioExistente.nombre = usuario.nombre;
                    usuarioExistente.apellido = usuario.apellido;
                    usuarioExistente.correo = usuario.correo;
                    usuarioExistente.telefono = usuario.telefono;
                    usuarioExistente.id_rol = usuario.id_rol;
                    usuarioExistente.estado = usuario.estado;

                    // Guardamos los cambios en la base de datos
                    await _context.SaveChangesAsync();

                    // Enviar notificación por correo electrónico
                    var smtpClient = new SmtpClient("smtp.outlook.com")
                    {
                        Port = 587,
                        Credentials = new NetworkCredential("jrojas30463@ufide.ac.cr", "QsEfT0809*"),
                        EnableSsl = true,
                    };

                    string subject = "Actualización de Información";
                    string body;

                    // Si la contraseña fue actualizada
                    if (isPasswordUpdated)
                    {
                        subject = "Actualización de Contraseña";
                        body = $"Hola {usuarioExistente.nombre},<br/><br/>" +
                               $"Tu contraseña ha sido actualizada exitosamente.<br/><br/>" +
                               $"Si no realizaste esta actualización, por favor contacta de inmediato con el administrador para asegurar la seguridad de tu cuenta.";
                    }
                    else
                    {
                        // Si solo se actualizaron otros detalles
                        body = $"Hola {usuarioExistente.nombre},<br/><br/>" +
                               $"Tu información ha sido actualizada exitosamente.<br/><br/>" +
                               $"Detalles de la actualización:<br/>" +
                               $"Nombre: {usuarioExistente.nombre} {usuarioExistente.apellido}<br/>" +
                               $"Correo: {usuarioExistente.correo}<br/>" +
                               $"Teléfono: {usuarioExistente.telefono}<br/>" +
                               $"Estado: {usuarioExistente.estado}<br/><br/>" +
                               $"Si no realizaste estos cambios, contacta con el administrador.";
                    }

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress("jrojas30463@ufide.ac.cr"),
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = true,
                    };
                    mailMessage.To.Add(usuarioExistente.correo);

                    try
                    {
                        // Intentar enviar el correo
                        await smtpClient.SendMailAsync(mailMessage);
                        TempData["SuccessMessage"] = "Usuario actualizado exitosamente y se ha enviado una notificación al correo.";
                    }
                    catch (Exception ex)
                    {
                        // Manejar errores en el envío del correo
                        TempData["ErrorMessage"] = $"Error al enviar el correo: {ex.Message}";
                    }

                    return RedirectToAction("Index");
                }
            }
            catch (DbUpdateException ex)
            {
                // Manejar errores de actualización de base de datos
                TempData["ErrorMessage"] = "Ocurrió un error al actualizar la base de datos. Por favor, inténtelo de nuevo más tarde.";
            }
            catch (TimeoutException ex)
            {
                TempData["ErrorMessage"] = "La operación tardó demasiado tiempo. Por favor, inténtelo más tarde.";
            }
            catch (Exception ex)
            {
                // Manejar cualquier otro tipo de excepción
                TempData["ErrorMessage"] = "Ocurrió un error inesperado. Por favor, inténtelo nuevamente.";
            }

            // Si llega aquí, recargar la vista con el mensaje de error
            ViewBag.Roles = new SelectList(_context.Roles, "id_rol", "nombre_rol", usuario.id_rol);
            ViewBag.Estados = new SelectList(new[] { "activo", "inactivo" }, usuario.estado);
            return View(usuario);
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.id_usuario == id);
        }

        [HttpGet]
        public async Task<IActionResult> Eliminar(int id)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.roles) // Incluir el rol del usuario si es necesario
                .FirstOrDefaultAsync(u => u.id_usuario == id);

            if (usuario == null)
            {
                TempData["ErrorMessage"] = "El usuario no existe.";
                return RedirectToAction("Index");
            }

            return View(usuario); // Pasamos el usuario a la vista de confirmación
        }

        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            try
            {
                var usuario = await _context.Usuarios.FindAsync(id);
                if (usuario == null)
                {
                    TempData["ErrorMessage"] = "El usuario no existe.";
                    return RedirectToAction("Index");
                }

                // Eliminar el usuario de la base de datos
                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Usuario eliminado exitosamente.";
                return RedirectToAction("Index");
            }
            catch (DbUpdateException dbEx)
            {
                // Error relacionado con la actualización de la base de datos
                TempData["ErrorMessage"] = "Ocurrió un error al eliminar el usuario en la base de datos. Por favor, inténtelo nuevamente más tarde.";
                // Opcional: registrar detalles del error
            }
            catch (TimeoutException timeoutEx)
            {
                // Error de tiempo de espera
                TempData["ErrorMessage"] = "La operación tomó demasiado tiempo. Por favor, inténtelo nuevamente más tarde.";
                // Opcional: registrar detalles del error
            }
            catch (Exception ex)
            {
                // Cualquier otro error inesperado
                TempData["ErrorMessage"] = "Ocurrió un error inesperado. Por favor, inténtelo nuevamente.";
            }

            // En caso de error, redirigir al índice para mostrar el mensaje de error
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Detalles(int id)
        {
            // Buscamos el usuario por su ID e incluimos la relación con los roles
            var usuario = await _context.Usuarios
                                        .Include(u => u.roles)
                                        .FirstOrDefaultAsync(u => u.id_usuario == id);

            if (usuario == null)
            {
                return NotFound();
            }

            // Enviamos el modelo a la vista de detalles
            return View(usuario);
        }
    }
}