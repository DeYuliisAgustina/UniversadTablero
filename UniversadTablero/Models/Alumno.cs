using System;
using System.Collections.Generic;

namespace UniversadTablero.Models
{
    public class Alumno
    {
        public int AlumnoID { get; set; }
        public string NombreyApellido { get; set; }
        public string DNI { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string Correo { get; set; }
        public string Telefono { get; set; }

        // Relación con AlumnoMateria
        public ICollection<AlumnoMaterias> AlumnoMaterias { get; set; }
    }
}