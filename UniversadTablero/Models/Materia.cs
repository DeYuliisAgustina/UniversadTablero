using System.Collections.Generic;

namespace UniversadTablero.Models
{
    public class Materia
    {
        public int MateriaID { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }

        public int ProfesorID { get; set; }
        public Profesor Profesor { get; set; }

        public ICollection<Alumno> Alumnos { get; set; }
        public ICollection<Nota> Notas { get; set; }
    }
}