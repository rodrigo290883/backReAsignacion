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
            if(args.Length == 0)
            {
                GetAppSettingsFile();
                ProcesaRegistrosPeriodo("");
            }
            else
            {
                GetAppSettingsFile();
                ProcesaRegistrosPeriodo(args[0]);
            }
                  
        }
        static void GetAppSettingsFile()
        {
            var builder = new ConfigurationBuilder()
                                 .SetBasePath(Directory.GetCurrentDirectory())
                                 .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            _iconfiguration = builder.Build();
        }
        static void ProcesaRegistrosPeriodo(string inicio)
        {
            var SolDAL = new SolicitudDAL(_iconfiguration);
            var lista  =  SolDAL.obtieneSolicitudes();
            foreach (Solicitud renglon in lista)
            {
                Console.WriteLine(renglon.folio + " - " + renglon.idsap + " - reAsignado:" + renglon.reasignado + " - notificado:" + renglon.notificado);
            }

            Console.WriteLine("Final del Proceso");
        }
    }
}