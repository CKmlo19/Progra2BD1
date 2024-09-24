using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Progra2.Data;
using Progra2.Models;
using System.Data;
using System.Data.SqlClient;

namespace MVC.Controllers
{
    public class UsuarioController 
    {
        private readonly DbContext _contexto;

        public UsuarioController(DbContext contexto)
        {
            _contexto = contexto;
        }

        public ActionResult Login()
        {
            return View("Login");
        }

        [HttpPost]
        public ActionResult Login(LoginModel l)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (SqlConnection con = new(_contexto.Valor))
                    {
                        using (SqlCommand cmd = new("sp_login", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add("@Usuario", SqlDbType.VarChar).Value = l.Usuario;
                            cmd.Parameters.Add("@Clave", SqlDbType.VarChar).Value = l.Clave;
                            con.Open();

                            SqlDataReader dr = cmd.ExecuteReader();

                            if (dr.Read())
                            {
                                Response.Cookies.Append("user", "Bienvenido " + l.Usuario);
                                return RedirectToAction("Index", "Home");
                            }
                            else
                            {
                                ViewData["error"] = "Error de credenciales";
                            }

                            con.Close();
                        }
                    }
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception)
            {
                return View("Login");
            }
            return View("Login");
        }

        public ActionResult Logout()
        {
            Response.Cookies.Delete("user");
            return RedirectToAction("Index", "Home");
        }
    }
}