using System;

namespace UniversadTablero.Models
{
    public class Nota
    {
        public int NotaID { get; set; }
        public decimal Valor { get; set; }
        public DateTime Fecha { get; set; }

        // Relación con Alumno
        public int AlumnoID { get; set; }
        public Alumno Alumno { get; set; }

        // Relación con Materia
        public int MateriaID { get; set; }
        public Materia Materia { get; set; }
    }
}