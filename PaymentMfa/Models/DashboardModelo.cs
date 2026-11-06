using System.Collections.Generic;

namespace PaymentMfa.Models
{
    public class DashboardModelo
    {
        public string Nombre { get; set; }
        public string Usuario { get; set; }
        public string Oid { get; set; }
        public string InicioSesion { get; set; }
        public string SesionExpira { get; set; }
        public string Metodos { get; set; }
        public List<Verificacion> Verificaciones { get; set; }
        public List<DatoClaim> Claims { get; set; }
    }

    public class Verificacion
    {
        public string Nombre { get; set; }
        public string Detalle { get; set; }
        // Clase CSS de la etiqueta: si, no o info
        public string Estado { get; set; }
        public string Etiqueta { get; set; }
    }

    public class DatoClaim
    {
        public string Tipo { get; set; }
        public string Valor { get; set; }
    }
}
