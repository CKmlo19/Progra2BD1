using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.Data.SqlClient;
using Progra2.Models;
using System.Data;
using System.Text.RegularExpressions;


namespace Progra2.Data
{
    public class EmpleadoData
    {
        // este metodo lista en orden alfabetico 
        public List<EmpleadoModel> Listar(string campo)
        {
            var oLista = new List<EmpleadoModel>();

            var cn = new Conexion();

            // abre la conexion
            using (var conexion = new SqlConnection(cn.getCadenaSQL()))
            {
                conexion.Open();
                // el procedure de listar
                SqlCommand cmd = new SqlCommand("dbo.ListarEmpleado", conexion);
                
                // la variable num tiene valores distintos dependiendo de lo que contenga campo
                int num = campo switch
                {
                    null or "" => 0, // Si el campo esta vacio o es nulo, lista todo
                    _ when ContieneNumeros(campo) => 1, // Si contiene números, filtra por ID
                    _ when ContieneLetrasYEspacios(campo) => 2, // Si contiene letras y espacios, filtra por nombre
                    _ => throw new ArgumentException("El campo no contiene un valor válido") // Si no cumple ninguna condicion
                                                                                            // Aunque no deberia ocurrir nunca
                };
                SqlParameter outputParam = new SqlParameter("@OutResultCode", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.AddWithValue("inCodigo", num);
                cmd.Parameters.AddWithValue("inCampo", campo);
                cmd.Parameters.Add(outputParam);
                cmd.CommandType = CommandType.StoredProcedure;

                using (var dr = cmd.ExecuteReader()) // este se utiliza cuando se retorna una gran cantidad de datos, por ejemplo la tabla completa
                {
                    // hace una lectura del procedimiento almacenado


                    while (dr.Read())
                    {
                        // Imprime el nombre de todas las columnas
                        for (int i = 0; i < dr.FieldCount; i++)
                        {
                            Console.WriteLine(dr.GetName(i) + ": " + dr[i]);
                        }
                        oLista.Add(new EmpleadoModel()
                        {
                            // tecnicamente hace un select, es por eso que se lee cada registro uno a uno que ya esta ordenado
                            Id = (int)Convert.ToInt64(dr["Id"]),
                            Nombre = dr["Nombre"].ToString(),
                            IdPuesto = (int)Convert.ToInt64(dr["idPuesto"]),
                            SaldoVacaciones = (short)Convert.ToInt16(dr["SaldoVacaciones"]),
                            FechaContratacion = (DateTime)dr["FechaContratacion"],
                            NombrePuesto = dr["NombrePuesto"].ToString(),
                            ValorDocumentoIdentidad = dr["ValorDocumentoIdentidad"].ToString()
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
                    cmd.Parameters.AddWithValue("inValorDocumentoIdentidad", oEmpleado.ValorDocumentoIdentidad.Trim()); // se le hace un trim a la hora de insertar
                    cmd.Parameters.AddWithValue("inIdPuesto", oEmpleado.IdPuesto); 
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

        public int Editar(EmpleadoModel oEmpleado)
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
                    SqlCommand cmd = new SqlCommand("dbo.EditarEmpleado", conexion);
                    cmd.Parameters.AddWithValue("inId", oEmpleado.Id);
                    cmd.Parameters.AddWithValue("inNombre", oEmpleado.Nombre.Trim()); // se le hace un trim a la hora de insertar
                    cmd.Parameters.AddWithValue("inIdPuesto", oEmpleado.IdPuesto);
                    cmd.Parameters.AddWithValue("inValorDocumentoIdentidad", oEmpleado.ValorDocumentoIdentidad);
                    // Configurar el parámetro de salida
                    SqlParameter outputParam = new SqlParameter("@OutResultCode", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(outputParam);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.ExecuteNonQuery();
                    // Registrar el script en la página para que se ejecute en el lado del cliente
                    resultado = (int)cmd.Parameters["@OutResultCode"].Value; // resultado es igual al codigo del output

                }


            }
            catch (Exception e)
            {
                resultado = 50008;

            }

            return resultado;

        }
        public int Eliminar(int id)
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
                    SqlCommand cmd = new SqlCommand("dbo.EliminarEmpleado", conexion);
                    cmd.Parameters.AddWithValue("inId", id); // se le hace un trim a la hora de insertar
                    // Configurar el parámetro de salida
                    SqlParameter outputParam = new SqlParameter("@OutResultCode", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(outputParam);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                    // Registrar el script en la página para que se ejecute en el lado del cliente
                    resultado = (int)cmd.Parameters["@OutResultCode"].Value; // resultado es igual al codigo del output

                }


            }
            catch (Exception e)
            {
                resultado = 50006;

            }


            return resultado;

        }

        public EmpleadoModel Obtener(int id)
        {
            var oEmpleado = new EmpleadoModel();

            var cn = new Conexion();

            // abre la conexion
            using (var conexion = new SqlConnection(cn.getCadenaSQL()))
            {
                conexion.Open();
                // el procedure de listar
                SqlCommand cmd = new SqlCommand("dbo.ObtenerEmpleado", conexion);
                cmd.Parameters.AddWithValue("inId", id);
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
                        oEmpleado.Id = (int)Convert.ToInt64(dr["id"]);
                        oEmpleado.IdPuesto = (int)Convert.ToInt64(dr["idPuesto"]);
                        oEmpleado.ValorDocumentoIdentidad = dr["ValorDocumentoIdentidad"].ToString();
                        oEmpleado.Nombre = dr["Nombre"].ToString();
                        oEmpleado.FechaContratacion = (DateTime)dr["FechaContratacion"];
                        oEmpleado.SaldoVacaciones = (short)(dr["SaldoVacaciones"]);
                    }
                }
            }
            return oEmpleado;
        }

        // Esta funcion sirve para listar los puestos que tienen un id vinculados 
        public List<PuestoModel> ListarPuesto()
        {
            var oLista = new List<PuestoModel>();

            var cn = new Conexion();

            // abre la conexion
            using (var conexion = new SqlConnection(cn.getCadenaSQL()))
            {
                conexion.Open();
                // el procedure de listar
                SqlCommand cmd = new SqlCommand("dbo.ListarPuestos", conexion);
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
                        oLista.Add(new PuestoModel()
                        {                            // tecnicamente hace un select, es por eso que se lee cada registro uno a uno que ya esta ordenado
                            Id = (int)Convert.ToInt64(dr["Id"]),
                            Nombre = dr["Nombre"].ToString(),
                            SalarioxHora = Convert.ToDecimal(dr["SalarioxHora"])
                        });
                    }
                }
            }
            return oLista;
        }



        // Funciones privadas
        private bool ContieneNumeros(string cadena)
        {
            // Patrón de expresión regular para verificar si la cadena contiene al menos un dígito
            string patron = @"^\d+$"; ;

            // Creamos un objeto Regex con el patrón especificado
            Regex regex = new Regex(patron);

            // Utilizamos el método IsMatch para verificar si la cadena coincide con el patrón
            return regex.IsMatch(cadena);
        }

        private bool ContieneLetrasYEspacios(string cadena)
        {
            // Patrón de expresión regular para verificar si la cadena contiene solo letras y espacios en blanco
            string patron = @"^[a-zA-Z\s]+$";

            // Creamos un objeto Regex con el patrón especificado
            Regex regex = new Regex(patron);

            // Utilizamos el método IsMatch para verificar si la cadena coincide con el patrón
            return regex.IsMatch(cadena);
        }
    }
}
