using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace TallerReloj.Function.Entities
{
    public class RelojEntity: TableEntity
    {
        public int IdEmpleado { get; set; }

        public DateTime Fecha { get; set; }

       
        public int Tipo { get; set; }

        public bool Consolidado { get; set; }
    }
}
