using System.Collections.Generic;

namespace UniversadTablero.Models
{
    public class AlumnoMaterias
    {
        public string Estado { get; set; }

        public int AlumnoID { get; set; }
        public Alumno Alumno { get; set; }

        public int MateriaID { get; set; }
        public Materia Materia { get; set; }

        public ICollection<Nota> Notas { get; set; }

    }
}