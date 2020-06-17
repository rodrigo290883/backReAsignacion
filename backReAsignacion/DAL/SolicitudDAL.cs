using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using backReAsignacion.Models;
using Microsoft.Extensions.Configuration;
using System.Net.Mail;


namespace backReAsignacion.DAL
{
    public class SolicitudDAL
    {
        private string _connectionString;

        public SolicitudDAL(IConfiguration iconfiguration)
        {
            _connectionString = iconfiguration.GetConnectionString("MyConnection");
        }

        public List<Solicitud> obtieneSolicitudes()
        {
            var listaSolicitudes = new List<Solicitud>();

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT s.folio,s.idsap,s.fecha_inicio ,s.fecha_fin ,s.estatus , est.descripcion ,s.tipo_solicitud ,tip.solicitud ,s.observacion_solicitante ,sol.nombre,apr.idsap_padre,apr.email_line,s.fecha_solicitud,GETDATE() as fecha_creacion "+
                                                "FROM solicitudes s LEFT JOIN empleados sol ON s.idsap = sol.idsap LEFT JOIN empleados apr ON s.idsap_aprobador = apr.idsap "+
                                                "LEFT JOIN ctipos_solicitud tip ON s.tipo_solicitud = tip.id_tipo_solicitud LEFT JOIN cestatus est ON s.estatus = est.estatus "+
                                                "WHERE s.fecha_inicio >= GETDATE() and DATEDIFF(day, s.fecha_asignacion, GETDATE()) >= @dias", con);
                cmd.Parameters.AddWithValue("@dias", 5);

                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    var aux = new Solicitud();

                    aux.folio = Convert.ToInt32(rdr[0]);
                    aux.idsap = Convert.ToInt32(rdr[1]);
                    aux.fecha_inicio = Convert.ToDateTime(rdr.IsDBNull(2) ? null : rdr[2]);
                    aux.fecha_fin = Convert.ToDateTime(rdr.IsDBNull(3) ? null : rdr[3]);
                    aux.estatus_descripcion = rdr[5].ToString();
                    aux.solicitud = rdr[7].ToString();
                    aux.observacion_solicitante = rdr[8].ToString();
                    aux.nombre = rdr[9].ToString();
                    aux.idsap_aprobador = Convert.ToInt32(rdr[10]);
                    aux.email_aprobador = rdr[11].ToString();
                    aux.fecha_solicitud = Convert.ToDateTime(rdr.IsDBNull(12) ? null : rdr[12]);
                    aux.fecha_asignacion = Convert.ToDateTime(rdr.IsDBNull(13) ? null : rdr[13]);



                    if (reAsignar(aux))
                    {
                        aux.reasignado = true;

                        aux.notificado = notificarAsignacion(aux);
                    }
                    else
                    {
                        aux.reasignado = false;
                    }

                    listaSolicitudes.Add(aux);
                }
            }
                return listaSolicitudes;
        }

        public bool reAsignar(Solicitud sol)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("UPDATE solicitudes SET idsap_aprobador=@idsap_aprobador, ultima_notificacion= GETDATE(), fecha_asignacion = GETDATE() WHERE folio = @folio;", con);
                cmd.Parameters.AddWithValue("@idsap_aprobador", sol.idsap_aprobador);
                cmd.Parameters.AddWithValue("@folio", sol.folio);

                con.Open();

                if (cmd.ExecuteNonQuery()==1)
                    return true;
                else
                    return false;
            }
        }

        public bool notificarAsignacion(Solicitud sol)
        {
            string folio = sol.folio.ToString();
            string empleado = sol.nombre;
            string id_empleado = sol.idsap.ToString();
            string aprobador = sol.idsap_aprobador.ToString();
            string destino = sol.email_aprobador;
            string solicitud = sol.solicitud;
            string fecha_inicio = sol.fecha_inicio.ToString();
            string fecha_fin = sol.fecha_fin.ToString();
            string fecha_solicitud = sol.fecha_solicitud.ToString();
            string observacion = sol.observacion_solicitante;
            string estatus = sol.estatus_descripcion;

            try
            {
                string origen = "envio.correos.sistemas@gmail.com";

                string pass = "244466666";
                string asunto = "Se re Asigna Solicitud pendiente de Aprobacion Folio:" + folio;
                string mensage = "<head><style>img{width:100%;padding:0px;margin:0px;}tr{background-image:url('https://i.postimg.cc/FzTgvcWz/cuerpo-mail.png'); background-repeat: repeat-y;background-size:100% 100%; padding:0px; margin:0px;}td{padding:0px; margin:0px;}</style></head><table style='padding:0px;marging:0px;border:0px;border-collapse: collapse;border-spacing:0px;'><tr><td><img src='https://i.postimg.cc/1319y6Dv/encabezado-mail.png' /></td></tr><tr><td style='padding:5% 5%; color:#b41547; font-size: 18px; text-align: center;'>Se realizo una solicitud de vacaciones por parte de:<br>" + empleado + "</td></tr><tr><td style='padding:0% 5%; color: #5c2a7e; font-size: 18px; text-align: left;'>Tipo Solicitud: " + solicitud + "</td></tr><tr><td style='padding:0% 5%; color: #5c2a7e; font-size: 18px; text-align: left;'>Fecha Inicio: " + fecha_inicio + "</td></tr><tr><td style=' padding:0% 5%; color: #5c2a7e; font-size: 18px; text-align: left;'>Fecha Fin: " + fecha_fin + "</td></tr><tr><td style=' padding:0% 5%; color: #5c2a7e; font-size: 18px; text-align: left;'>Observacion Solicitante: " + observacion + "</td></tr><tr><td style=' padding:5% 5%; color:#b41547; font-size: 18px; text-align: center; '>Favor de ingresar al sitio de <a href='#'>vacaciones</a> para su aprobacion.</td></tr><tr><td><img src='https://i.postimg.cc/hvSK9qPN/pie-mail.png' /></td></tr></table>";


                MailMessage correo = new MailMessage(origen, destino);
                correo.IsBodyHtml = true;
                correo.Subject = asunto;
                correo.Body = mensage;

                SmtpClient cliente = new SmtpClient();
                cliente.Host = "smtp.gmail.com";
                cliente.EnableSsl = true;
                cliente.Port = 587;
                cliente.UseDefaultCredentials = false;
                cliente.Credentials = new System.Net.NetworkCredential(origen, pass);

                cliente.Send(correo);
                cliente.Dispose();
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }
}
