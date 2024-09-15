using Microsoft.Data.SqlClient;
using Progra2.Models;
using System.Data;


namespace Progra2.Data
{
    public class EmpleadoData
    {
        // este metodo lista en orden alfabetico 
        public List<EmpleadoModel> Listar()
        {
            var oLista = new List<EmpleadoModel>();

            var cn = new Conexion();

            // abre la conexion
            using (var conexion = new SqlConnection(cn.getCadenaSQL()))
            {
                conexion.Open();
                // el procedure de listar
                SqlCommand cmd = new SqlCommand("dbo.ListarEmpleado", conexion);
                // Configurar el parámetro de salida
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
                        oLista.Add(new EmpleadoModel()
                        {
                            // tecnicamente hace un select, es por eso que se lee cada registro uno a uno que ya esta ordenado
                            Id = (int)Convert.ToInt64(dr["Id"]),
                            Nombre = dr["Nombre"].ToString(),
                            //Salario = Convert.ToDecimal(dr["Salario"])
                        });
                    }
                }
            }
            return oLista;
        }

        public int Insertar(EmpleadoModel oEmpleado)
        {
            int resultado;

            try
            {
                var cn = new Conexion();

                // abre la conexion
                using (var conexion = new SqlConnection(cn.getCadenaSQL()))
                {
                    conexion.Open();
                    // el procedure de listar
                    SqlCommand cmd = new SqlCommand("dbo.InsertarEmpleado", conexion);
                    cmd.Parameters.AddWithValue("inNombre", oEmpleado.Nombre.Trim()); // se le hace un trim a la hora de insertar
                    //cmd.Parameters.AddWithValue("inSalario", oEmpleado.Salario);
                    // Configurar el parámetro de salida
                    SqlParameter outputParam = new SqlParameter("@OutResultCode", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(outputParam);
                    //cmd.Parameters.AddWithValue("OutResultCode", 0); // en un inicio se coloca en 0
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                    //resultado = Convert.ToInt32(cmd.ExecuteScalar()); //
                    resultado = (int)cmd.Parameters["@OutResultCode"].Value; // resultado es igual al codigo del output
                }
            }
            catch (Exception e)
            {
                string error = e.Message;
                resultado = 50006;

            }
            return resultado;
        }
    }
}
