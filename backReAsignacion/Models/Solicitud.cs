using System;
namespace backReAsignacion.Models
{
    public class Solicitud
    {
        
        public int folio { get; set; }
        public string nombre { get; set; }
        public string solicitud { get; set; }
        public string estatus_descripcion { get; set; }
        public int idsap { get; set; }
        public int tipo_solicitud { get; set; }
        public int tipo_solicitud_goce { get; set; }
        public Nullable<System.DateTime> fecha_solicitud { get; set; }
        public Nullable<System.DateTime> fecha_inicio { get; set; }
        public Nullable<System.DateTime> fecha_fin { get; set; }
        public string observacion_solicitante { get; set; }
        public Nullable<int> estatus { get; set; }
        public Nullable<int> idsap_aprobador { get; set; }
        public string email_aprobador { get; set; }
        public bool reasignado { get; set; }
        public bool notificado { get; set; }
        public Nullable<System.DateTime> fecha_asignacion { get; set; }
    }
}
