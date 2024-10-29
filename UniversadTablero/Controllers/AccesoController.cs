using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using UniversadTablero.Models;

namespace UniversadTablero.Controllers
{
    public class AccesoController : Controller
    {
        static string cadenaConexion = "Data Source= (localdb)\\MSSQLLocalDB; Initial Catalog=Universidad; Integrated Security=True";

        // GET: Acceso
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Register()
        {
            return View();
        }

        public ActionResult Dashboard()
        {
            ViewBag.ListaProfesores = ListarProfesores();
            ViewBag.ListaMaterias = ListarMaterias();
            ViewBag.ListaAlumnos = ListarAlumnos();
            ViewBag.ListaNotas = ListarNotas();
            ViewBag.ListaAlumnoMaterias = ListarAlumnoPorMaterias();

            return View();
        }

        [HttpPost]
        public ActionResult Registrar(Usuario oUsuario)
        {
            bool registrado;
            string mensaje;

            if (oUsuario.Clave == oUsuario.ConfirmarClave)
            {

                oUsuario.Clave = ConvertirSha256(oUsuario.Clave);
            }
            else
            {
                ViewData["Mensaje"] = "Las contraseñas no coinciden";
                return View();
            }

            using (SqlConnection cn = new SqlConnection(cadenaConexion))
            {

                SqlCommand cmd = new SqlCommand("sp_RegistrarUsuario", cn);
                cmd.Parameters.AddWithValue("Correo", oUsuario.Correo);
                cmd.Parameters.AddWithValue("Clave", oUsuario.Clave);
                cmd.Parameters.Add("Registrado", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("Mensaje", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();

                cmd.ExecuteNonQuery();

                registrado = Convert.ToBoolean(cmd.Parameters["Registrado"].Value);
                mensaje = cmd.Parameters["Mensaje"].Value.ToString();


            }

            ViewData["Mensaje"] = mensaje;

            if (registrado)
            {
                return RedirectToAction("Login", "Acceso");
            }
            else
            {
                return View("Login");
            }

        }

        //metodo Login
        [HttpPost]
        public ActionResult Login(Usuario oUsuario)
        {
            oUsuario.Clave = ConvertirSha256(oUsuario.Clave);

            using (SqlConnection cn = new SqlConnection(cadenaConexion))
            {

                SqlCommand cmd = new SqlCommand("sp_ValidarUsuario", cn);
                cmd.Parameters.AddWithValue("Correo", oUsuario.Correo);
                cmd.Parameters.AddWithValue("Clave", oUsuario.Clave);
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();

                oUsuario.UsuarioId = Convert.ToInt32(cmd.ExecuteScalar().ToString());

            }

            if (oUsuario.UsuarioId != 0)
            {

                Session["usuario"] = oUsuario;
                return RedirectToAction("Dashboard", "Acceso");
            }
            else
            {
                ViewData["Mensaje"] = "usuario no encontrado";
                return View("Login");
            }
        }

        //metodo para encriptar la clave
        public static string ConvertirSha256(string texto)
        {
            try
            {
                // Verificar que el texto no sea nulo
                if (string.IsNullOrEmpty(texto))
                    return string.Empty;

                using (SHA256 sha256 = SHA256.Create())
                {
                    // Convertir el texto a bytes
                    byte[] bytes = Encoding.UTF8.GetBytes(texto);

                    // Computar el hash de los bytes y convertirlo a string hexadecimal
                    byte[] hash = sha256.ComputeHash(bytes);

                    // Convertir el hash a string hexadecimal 
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
            catch (Exception ex)
            {
                // loguear la excepción en un sistema de logging real
                Console.WriteLine($"Error al encriptar: {ex.Message}");
                return string.Empty;
            }
        }

        public List<Profesor> ListarProfesores()
        {
            List<Profesor> lista = new List<Profesor>();

            using (SqlConnection cn = new SqlConnection(cadenaConexion))
            {
                // Consulta SQL directa
                string query = "SELECT * FROM Profesores";
                SqlCommand cmd = new SqlCommand(query, cn);

                cn.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    Profesor oProfesor = new Profesor()
                    {
                        ProfesorID = Convert.ToInt32(dr["ProfesorID"].ToString()),
                        NombreyApellido = dr["NombreyApellido"].ToString(),
                        Correo = dr["Correo"].ToString(),
                        DNI = dr["DNI"].ToString(),
                        Telefono = dr["Telefono"].ToString(),

                    };

                    lista.Add(oProfesor);
                }
            }

            return lista;
        }

        public List<Materia> ListarMaterias()
        {
            List<Materia> lista = new List<Materia>();

            using (SqlConnection cn = new SqlConnection(cadenaConexion))
            {
                // Consulta SQL con JOIN para incluir el NombreyApellido del profesor
                string query = @"
                    SELECT 
                        m.MateriaID, 
                        m.Nombre, 
                        m.Descripcion, 
                        m.ProfesorID, 
                        p.NombreyApellido AS NombreProfesor
                    FROM 
                        Materias m
                    INNER JOIN 
                        Profesores p ON m.ProfesorID = p.ProfesorID";

                SqlCommand cmd = new SqlCommand(query, cn);

                cn.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    Materia oMateria = new Materia()
                    {
                        MateriaID = Convert.ToInt32(dr["MateriaID"].ToString()),
                        Nombre = dr["Nombre"].ToString(),
                        Descripcion = dr["Descripcion"].ToString(),
                        ProfesorID = Convert.ToInt32(dr["ProfesorID"].ToString()),
                        Profesor = new Profesor() { NombreyApellido = dr["NombreProfesor"].ToString() }
                    };

                    lista.Add(oMateria);
                }
            }

            return lista;
        }

        public List<Alumno> ListarAlumnos()
        {
            List<Alumno> lista = new List<Alumno>();

            using (SqlConnection cn = new SqlConnection(cadenaConexion))
            {
                // Consulta SQL directa
                string query = "SELECT * FROM Alumnos";
                SqlCommand cmd = new SqlCommand(query, cn);

                cn.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    Alumno oAlumno = new Alumno()
                    {
                        AlumnoID = Convert.ToInt32(dr["AlumnoID"].ToString()),
                        NombreyApellido = dr["NombreyApellido"].ToString(),
                        DNI = dr["DNI"].ToString(),
                        FechaNacimiento = Convert.ToDateTime(dr["FechaNacimiento"].ToString()),
                        Correo = dr["Correo"].ToString(),
                        Telefono = dr["Telefono"].ToString(),
                    };

                    lista.Add(oAlumno);
                }
            }

            return lista;
        }

        public List<Nota> ListarNotas()
        {
            List<Nota> lista = new List<Nota>();

            using (SqlConnection cn = new SqlConnection(cadenaConexion))
            {
                // Consulta SQL directa
                string query = "SELECT NotaID, AlumnoID, MateriaID, Valor, Fecha FROM Notas";
                SqlCommand cmd = new SqlCommand(query, cn);

                cn.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    Nota oNota = new Nota()
                    {
                        NotaID = Convert.ToInt32(dr["NotaID"].ToString()),
                        AlumnoID = Convert.ToInt32(dr["AlumnoID"].ToString()),
                        MateriaID = Convert.ToInt32(dr["MateriaID"].ToString()),
                        Valor = Convert.ToDecimal(dr["Valor"].ToString()),
                        Fecha = Convert.ToDateTime(dr["Fecha"].ToString()),
                        Alumno = ListarAlumnos().Where(x => x.AlumnoID == Convert.ToInt32(dr["AlumnoID"].ToString())).FirstOrDefault(),
                        Materia = ListarMaterias().Where(x => x.MateriaID == Convert.ToInt32(dr["MateriaID"].ToString())).FirstOrDefault()
                    };

                    lista.Add(oNota);
                }
            }

            return lista;
        }

        public List<AlumnoMaterias> ListarAlumnoPorMaterias()
        {
            List<AlumnoMaterias> lista = new List<AlumnoMaterias>();
            using (SqlConnection cn = new SqlConnection(cadenaConexion))
            {
                string query = @"
                    SELECT 
                        a.NombreyApellido,
                        m.Nombre AS NombreMateria,
                        am.Estado
                    FROM AlumnoMaterias am
                    INNER JOIN Alumnos a ON am.AlumnoID = a.AlumnoID     
                    INNER JOIN Materias m ON am.MateriaID = m.MateriaID"; // Utilizo Join y As para obtener los nombres de las tablas relacionadas

                SqlCommand cmd = new SqlCommand(query, cn);
                cn.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        AlumnoMaterias oAlumnoMateria = new AlumnoMaterias()
                        {
                            Alumno = new Alumno() { NombreyApellido = dr["NombreyApellido"].ToString() },
                            Materia = new Materia() { Nombre = dr["NombreMateria"].ToString() },
                            Estado = dr["Estado"].ToString()
                        };
                        lista.Add(oAlumnoMateria);
                    }
                }
            }
            return lista;
        }


    }
}