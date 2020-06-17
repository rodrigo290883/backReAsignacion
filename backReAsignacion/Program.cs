using backReAsignacion.DAL;
using backReAsignacion.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
namespace backReAsignacion
{
    class Program
    {
        private static IConfiguration _iconfiguration;
        static void Main(string[] args)
        {
            
                GetAppSettingsFile();
                ProcesaReAsignaciones();
                  
        }
        static void GetAppSettingsFile()
        {
            var builder = new ConfigurationBuilder()
                                 .SetBasePath(Directory.GetCurrentDirectory())
                                 .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            _iconfiguration = builder.Build();
        }
        static void ProcesaReAsignaciones()
        {
            var SolDAL = new SolicitudDAL(_iconfiguration);
            var lista  =  SolDAL.obtieneSolicitudes();
            var logDal = new LogDAL(_iconfiguration);
            foreach (Solicitud renglon in lista)
            {
                var log = new LogClass { idsap = renglon.idsap, log = "Re Asignacion de solicitud con folio: "+renglon.folio+" al aprobador con idsap: "+renglon.idsap_aprobador ,fecha_creacion = renglon.fecha_asignacion, idsap_creacion=101010   };
                Console.WriteLine(renglon.folio + " - " + renglon.idsap + " - reAsignado:" + renglon.reasignado + " - notificado:" + renglon.notificado);
                logDal.createLog(log);
            }

            Console.WriteLine("Final del Proceso");
        }
    }
}