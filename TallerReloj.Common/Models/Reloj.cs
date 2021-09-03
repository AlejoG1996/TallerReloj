using System;

namespace TallerReloj.Common.Models
{
    public class Reloj
    {
        public int IdEmpleado { get; set; }

        public DateTime Fecha { get; set; }

        
        public int Tipo { get; set; }

        public bool Consolidado { get; set; }
    }
}
