using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gimnasio.Utilidades.Model
{
    public class SocioModel
    {
        public string idSocio;
        public string nombre;
        public string nombrePaterno;
        public string nombreMaterno;
        public MembresiaType membresia;
        public DateTime vencimiento;
        public bool isSocioEnabled;
        public string exitType;

        public SocioModel() { }

        public SocioModel(string idSocio, string nombre, string nombrePaterno, string nombreMaterno, MembresiaType membresia, DateTime vencimiento, bool isSocioEnabled, string exitType)
        {
            IdSocio = idSocio;
            Nombre = nombre;
            NombrePaterno = nombrePaterno;
            NombreMaterno = nombreMaterno;
            Membresia = membresia;
            Vencimiento = vencimiento;
            IsSocioEnabled = isSocioEnabled;
            ExitType = exitType;
        }
        private string IdSocio
        {
            get { return idSocio; }
            set { idSocio = value; }
        }

        private string Nombre
        {
            get { return nombre; }
            set { nombre = value; }
        }

        private string NombrePaterno
        {
            get { return nombrePaterno; }
            set { nombrePaterno = value; }
        }
        private string NombreMaterno
        {
            get { return nombreMaterno; }
            set { nombreMaterno = value; }
        }
        private MembresiaType Membresia
        {
            get { return membresia; }
            set { membresia = value; }
        }
        private DateTime Vencimiento
        {
            get { return vencimiento; }
            set { vencimiento = value; }
        }
        private bool IsSocioEnabled
        {
            get { return isSocioEnabled; }
            set { isSocioEnabled = value; }
        }

        private string ExitType
        {
            get { return exitType; }
            set { exitType = value; }
        }

        public enum MembresiaType
        {
            None,
            Infinito,
            Anual,
            AnualPlus,
            Trimestral,
            TrimestralPlus,
            Mensual,
            MensualPlus,
            MedioDia,
            MedioDiaPlus,
            Visita
        }

    }
}
