using Microsoft.Data.SqlClient;
using Progra2.Models;
using System.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
namespace Progra2.Data
{
    public class MovimientoData
    {
        public List<MovimientoModel> Listar(int idEmpleado)
        {
            var oLista = new List<MovimientoModel>();

            var cn = new Conexion();

            // abre la conexion
            using (var conexion = new SqlConnection(cn.getCadenaSQL()))
            {
                conexion.Open();
                // el procedure de listar
                SqlCommand cmd = new SqlCommand("dbo.ListarMovimiento", conexion);
                cmd.Parameters.AddWithValue("inId", idEmpleado); // se le coloca un 0 en el outresultcode

                SqlParameter outputParam = new SqlParameter("@OutResultCode", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(outputParam);
                cmd.CommandType = CommandType.StoredProcedure;
                using (var dr = cmd.ExecuteReader()) // este se utiliza cuando se retorna una gran cantidad de datos, por ejemplo la tabla completa
                {
                    // hace una lectura del procedimiento almacenado
                    while (dr.Read())
                    {
                        oLista.Add(new MovimientoModel()
                        {
                            // tecnicamente hace un select, es por eso que se lee cada registro uno a uno que ya esta ordenado
                            Id = (int)Convert.ToInt64(dr["Id"]),
                            Fecha = ((DateTime)dr["Fecha"]).Date,
                            Monto = (int)Convert.ToInt64(dr["Monto"]),
                            NuevoSaldo = (int)Convert.ToInt64(dr["NuevoSaldo"]),
                            NombreUsuario = dr["NombreUsuario"].ToString(),
                            NombreTipoMovimiento = dr["NombreTipoMovimiento"].ToString(),
                            PostIP = dr["PostInIp"].ToString(),
                            PostTime = (DateTime)dr["PostTime"]
                        });
                    }
                }
            }


            return oLista;
        }

        public int Insertar(MovimientoModel oMovimiento, string IP)
        {
            int resultado;

            try
            {
                var cn = new Conexion();

                // abre la conexion
                using (var conexion = new SqlConnection(cn.getCadenaSQL()))
                {
                    conexion.Open();
                    SqlCommand cmd1 = new SqlCommand("dbo.InsertarMovimiento", conexion);
                    cmd1.CommandType = CommandType.StoredProcedure;


                    cmd1.Parameters.AddWithValue("@inIdEmpleado", EmpleadoModel.GetInstance().Id);
                    Console.WriteLine(EmpleadoModel.GetInstance().Id);
                    cmd1.Parameters.AddWithValue("@inIdTipoMovimiento", oMovimiento.IdTipoMovimiento);
                    cmd1.Parameters.AddWithValue("@inIdPostByUser", UsuarioModel.GetInstance().id);
                    Console.WriteLine(UsuarioModel.GetInstance().id);
                    cmd1.Parameters.AddWithValue("@inMonto", oMovimiento.Monto);
                    cmd1.Parameters.AddWithValue("@inPostInIp", IP);
                    cmd1.Parameters.Add("@OutResultCode", SqlDbType.Int).Direction = ParameterDirection.Output;
                    cmd1.ExecuteNonQuery();
                    resultado = (int)cmd1.Parameters["@OutResultCode"].Value; // resultado es igual al codigo del output;

                }


            }
            catch (Exception e)
            {
                resultado = 50008;
                Console.WriteLine("Error: " + e.Message);
            }


            return resultado;

        }

        public List<TipoMovimientoModel> ListarTiposMovimientos()
        {
            var oLista = new List<TipoMovimientoModel>();

            var cn = new Conexion();

            // abre la conexion
            using (var conexion = new SqlConnection(cn.getCadenaSQL()))
            {
                conexion.Open();
                // el procedure de listar
                SqlCommand cmd = new SqlCommand("dbo.ListarTipoMovimiento", conexion);
                SqlParameter outputParam = new SqlParameter("@OutResultCode", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(outputParam);

                cmd.CommandType = CommandType.StoredProcedure;
                using (var dr = cmd.ExecuteReader()) // este se utiliza cuando se retorna una gran cantidad de datos, por ejemplo la tabla completa
                {
                    // hace una lectura del procedimiento almacenado
                    while (dr.Read())
                    {
                        oLista.Add(new TipoMovimientoModel()
                        {                            // tecnicamente hace un select, es por eso que se lee cada registro uno a uno que ya esta ordenado
                            Id = (int)Convert.ToInt64(dr["Id"]),
                            Nombre = dr["Nombre"].ToString(),
                            TipoAccion = dr["TipoAccion"].ToString()
                        });
                    }
                }
            }
            return oLista;
        }

    }
}
