using Microsoft.Data.SqlClient;
using Progra2.Models;
using System.Data;
using System.Text.RegularExpressions;

namespace Progra2.Data
{
    public class UsuarioData
    {
        // Método para listar usuarios en orden alfabético o filtrando por un campo específico
        public List<UsuarioModel> ListarUsuarios(string campo)
        {
            var oLista = new List<UsuarioModel>();

            var cn = new Conexion();

            // Abre la conexión
            using (var conexion = new SqlConnection(cn.getCadenaSQL()))
            {
                conexion.Open();

                // Llama al stored procedure para listar usuarios
                SqlCommand cmd = new SqlCommand("dbo.ListarUsuarios", conexion);

                // Determina cómo filtrar el campo
                int num = campo switch
                {
                    null or "" => 0, // Lista todo si el campo está vacío o es nulo
                    _ when ContieneNumeros(campo) => 1, // Filtra por ID si contiene números
                    _ when ContieneLetrasYEspacios(campo) => 2, // Filtra por nombre si contiene letras y espacios
                    _ => throw new ArgumentException("El campo no contiene un valor válido")
                };

                cmd.Parameters.AddWithValue("inCodigo", num);
                cmd.Parameters.AddWithValue("inCampo", campo);
                cmd.CommandType = CommandType.StoredProcedure;

                using (var dr = cmd.ExecuteReader())
                {
                    // Lee los registros obtenidos del stored procedure
                    while (dr.Read())
                    {
                        oLista.Add(new UsuarioModel()
                        {
                            id = (int)Convert.ToInt64(dr["Id"]),
                            username = dr["Username"].ToString(),
                            password = dr["Password"].ToString()
                        });
                    }
                }
            }
            return oLista;
        }

        // Método para autenticar usuarios
        public bool AutenticarUsuario(string username, string password)
        {
            var cn = new Conexion();
            bool isAuthenticated = false;

            // Abre la conexión
            using (var conexion = new SqlConnection(cn.getCadenaSQL()))
            {
                conexion.Open();

                // Llama al stored procedure para verificar el usuario
                SqlCommand cmd = new SqlCommand("dbo.VerificarUsuario", conexion);
                cmd.Parameters.AddWithValue("inUsername", username);
                cmd.Parameters.AddWithValue("inPassword", password);
                cmd.CommandType = CommandType.StoredProcedure;

                using (var dr = cmd.ExecuteReader())
                {
                    // Si el stored procedure devuelve un resultado, el usuario está autenticado
                    if (dr.Read())
                    {
                        isAuthenticated = true;
                    }
                }
            }
            return isAuthenticated;
        }


        // Funciones privadas para validar el campo
        private bool ContieneNumeros(string cadena)
        {
            string patron = @"^\d+$";
            Regex regex = new Regex(patron);
            return regex.IsMatch(cadena);
        }

        private bool ContieneLetrasYEspacios(string cadena)
        {
            string patron = @"^[a-zA-Z\s]+$";
            Regex regex = new Regex(patron);
            return regex.IsMatch(cadena);
        }
    }
}
