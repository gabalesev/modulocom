using System;
using System.Collections.Generic;
using LibreriaClases;
using LibreriaClases.Clases;
using LibreriaConexion;
using BinConfig;
using System.Collections;

namespace Demo
{
    class Demo
    {
        static void Main(string[] args)
        {
            Console.Beep();

            ArchivoConfig lee = new ArchivoConfig();

            Errorof errConfig = new Opera().LeeArchivo(ref lee);
            if (errConfig.Error != 0)
            {
                Console.WriteLine("Error al leer archivo de configuración.");
                Console.ReadLine();
            }
            else
            {
                lee.Port = 20900;
                lee.MaskDesenmascara = new byte[] { 0x06, 0x07, 0x05 };
                lee.MaskEnmascara = new byte[] { 0x01, 0x03, 0xfc };

                lee.LogPath = System.Configuration.ConfigurationManager.AppSettings["Registro_DirectorioBase"];
                lee.LogFileName = System.Configuration.ConfigurationManager.AppSettings["Registro_NombreArchivo"];
                lee.LevelLog = (EnumMessageType)Enum.Parse(typeof(EnumMessageType), System.Configuration.ConfigurationManager.AppSettings["Registro_Nivel"]);

                System.Net.IPHostEntry ipHostInfo = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());

                for (int i = 0; i < ipHostInfo.AddressList.Length; i++)
                {
                    if (ipHostInfo.AddressList[i].AddressFamily ==
                        System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        lee.DefaultServer = ipHostInfo.AddressList[i];
                        break;
                    }
                }
            }

            var bc = new BaseConfig();
            Error errBConfig = bc.LeeBaseConfig(ref bc);


            if (errBConfig.CodError != 0)
            {
                Console.WriteLine("Error al leer archivo base de configuración.");
                Console.ReadLine();
            }

            // Prueba de Paquete A
            Terminal paqA = ArmaPaqueteA(bc);

            bc.Tarjeta = 53772; // Valor forzado
            Comunicacion com = new Comunicacion(bc, lee);

            Error errCxn = com.Conectar(paqA, EnumModoConexion.ETHERNET);

            if (errCxn.CodError != 0)
            {
                com.Desconectar();
                Console.Read();
                Environment.Exit(0);
            }
            else
            {
                IList objsRec = com.InteraccionAB(ref paqA);

                TransaccionMSG mensaje;
                if (objsRec.Count == 6)
                    mensaje = (TransaccionMSG)objsRec[5];


                if (objsRec[1] is Agente)
                {
                    var agente = (Agente)objsRec[1];

                    Console.WriteLine("Agencia: " + agente.Nombre + "\nNúmero de Agencia: " + agente.Numero + "\n");
                    Error errOffline = new Error();
                }

                IList objsRec3 = new List<object>();
                IList objsRec2 = new List<object>();

                TransacQuinielaH cabeceraAnul = new TransacQuinielaH();
                TransacQuinielaB cuerposAnul = new TransacQuinielaB();
                AnulReimpQuiniela anulacionQ = new AnulReimpQuiniela();

                TransacPoceado poceadoAnul = new TransacPoceado();
                AnulReimpPoceado anulacionP = new AnulReimpPoceado();

                if (objsRec.Count < 2 && objsRec[0] is Error)
                {
                    Error err = (Error)objsRec[0];
                    if (err.CodError != 0)
                    {
                        Console.Write("Error: " + err.CodError);
                        Console.WriteLine(" " + err.Descripcion + "\n");

                    }
                }
            }
        }
        
        private static Terminal ArmaPaqueteA(BaseConfig bc)
        {
            var paqA = new Terminal();

            paqA.Tarjeta = bc.Tarjeta;
            paqA.NumeroTerminal = bc.Terminal;
            paqA.MacTarjeta = bc.MsgMAC;

            byte[] mac = new byte[] { 0xdf, 0x72, 0x0f, 0xae, 0xdf, 0xd4, 0xe9, 0x1e, 0xdf, 0x8e, 0x1f, 0x61 };
            //{ 0x00, 0xc2, 0x00, 0x71, 0x00, 0x09, 0xb3, 0x5a, 0x00, 0xde, 0xbf, 0x82 };

            paqA.FechaHora = DateTime.Now;

            Version assemblyversion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            paqA.Version = (ushort)((assemblyversion.Major * 1000) + (assemblyversion.Minor * 10) + (assemblyversion.Build));//version

            paqA.Tipo = EnumTerminalModelo.TML;

            return paqA;
        }
    }
}
