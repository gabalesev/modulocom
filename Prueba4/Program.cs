using System;
using System.Collections.Generic;
using System.Text;
using LibreriaConexion;
using System.Collections;
using System.Net.NetworkInformation;
using LibreriaClases.Clases;
using LibreriaClases;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Linq;
using BinConfig;
using System.Net;
using Certificar;
using ProtocoloLib;
using LoggerLib;

namespace Prueba1
{


    public class Program
    {
        private static char[] byteToChar(byte[] bytes)
        {
            char[] ch = new char[bytes.Length];
            for (int i = 0; i < bytes.Length; i++)
            {
                ch[i] = (char)bytes[i];
            }
            return ch;
        }

        static void Main(string[] args)
        {
            
            Certificado certifica = new Certificado();
            //UnitTest1 ut = new UnitTest1();
            BaseConfig bc = new BaseConfig();

            

            #region //Prueba de QUINIELA 

            TransacQuinielaB jue = new TransacQuinielaB();
            jue.TipoApuesta = new byte[] { 0x06, 0x06, 0x07, 0x06, 0x0b };
            jue.NumeroAp1 = new string[] { "0233", "077", "12", "2411", "33" };
            jue.RangoDesde1 = new byte[] { 0x01, 0x01, 0x01, 0x01, 0x00 };
            jue.RangoHasta1 = new byte[] { 0x01, 0x01, 0x05, 0x14, 0x00 };
            jue.NumeroAp2 = new string[] { null, null, "34", null, "77" };
            jue.NumeroAp3 = new string[] { null, null, null, null, "12" };
            jue.RangoDesde2 = new byte[] { 0x00, 0x00, 0x01, 0x00, 0x00 };
            jue.RangoHasta2 = new byte[] { 0x00, 0x00, 0x0a, 0x00, 0x00 };
            jue.Importe = new ushort[] { 300, 600, 300, 400, 200 };//en centavos   

            //jue.TipoApuesta = new byte[] { 0x06, 0x07 };
            //jue.NumeroAp1 = new string[] { "12", "21" };
            //jue.RangoDesde1 = new byte[] { 0x01, 0x01 };
            //jue.RangoHasta1 = new byte[] { 0x01, 0x01 };
            //jue.NumeroAp2 = new string[] { null, "45" };
            //jue.NumeroAp3 = new string[] { null, null };
            //jue.RangoDesde2 = new byte[] { 0x00, 0x01 };
            //jue.RangoHasta2 = new byte[] { 0x00, 0x05 };
            //jue.Importe = new ushort[] { 1000, 1000 };//en centavos
            
            #endregion

            try
            {
                Opera opera = new Opera();
                //string archivo = "C:\\BetmakerTP\\Config.Bin";
                ArchivoConfig lee = new ArchivoConfig();                
                
                Errorof errConfig = opera.LeeArchivo(ref lee);
                Error errBConfig = bc.LeeBaseConfig(ref bc);

                if (errConfig.Error != 0)
                {
                    lee = new ArchivoConfig();

                    #region //PARAMETROS CONFIGURACION PARA CONFIG
                    bc.Terminal = 80732555;//1300000006;
                    bc.Tarjeta = 19511;//50026;
                    bc.TerminalModelo = EnumTerminalModelo.TML;
                    bc.MAC = new byte[] { 0x15, 0xBE, 0x07, 0x91, 0xFD, 0x32, 0xA4, 0xB3 };//{ 0x8b, 0x3d, 0x39, 0xff, 0x6a, 0xdd, 0x16, 0xb8 };//{ 0x5e, 0x01, 0xd2, 0x69, 0x78, 0x8b, 0x7d, 0x02 }; { 0xa0, 0xca, 0x14, 0x1d, 0xba, 0xdf, 0x7b, 0x44 };
                    bc.MsgMAC = new byte[] { 0x00, 0x91, 0x00, 0x07, 0x00, 0x32, 0xBE, 0xB3, 0x00, 0x15, 0xFD, 0xA4};
                    //lee.EncryptMAC = mac;//new byte[] { 0x00 };

                    lee.ImpresoraReportes = "impresoraPDF";
                    lee.ImpresoraTicket = "THERMAL Receipt Printer";

                    lee.MaskEscape = 0xfc;

                    //lee.MaskEnmascara = new byte[] { 0x01, 0x03, 0xfc };
                    //lee.MaskDesenmascara = new byte[] { 0x06, 0x07, 0x05 };

                    lee.MaskEnmascara = new byte[] { 0x01, 0x03, 0x04, 0x10, 0x1e, 0x9e, 0xfc, 0x83, 0x84, 0x0d, 0x8d, 0x90, 0xff };
                    lee.MaskDesenmascara = new byte[] { 0x06, 0x07, 0xdd, 0x0a, 0x09, 0x41, 0x05, 0xde, 0xdf, 0x15, 0x11, 0x0b, 0x08 };

                    lee.LogPath = "C:\\BetmakerTP\\Logs\\";
                    lee.LogFileName = "LogDisp.lg";
                    lee.LogMaxFileSize = 10485760;
                    lee.NumeringWithSecuential = false;
                    lee.LevelLog = EnumMessageType.DEBUG;

                    lee.IpTerminal = IPAddress.Parse("133.61.1.12");
                    lee.IpMask = IPAddress.Parse("255.255.0.0");
                    lee.DW = IPAddress.Parse("133.61.1.30");
                    lee.DNS = IPAddress.Parse("133.61.1.194");

                    lee.PathPRN = "C:\\BetmakerTP\\Conexion\\";
                    lee.ArchivoPRN = "ArchivoPRN.xml";
                    lee.DefaultServer = IPAddress.Parse("133.61.1.71");
                    lee.Host = "Win7x86";
                    lee.Port = 9950; //MENDOZA
                    lee.Telefono = "08006665807";

                    lee.PCName = "PCjorge";

                    lee.FTPServer = IPAddress.Parse("133.61.1.195");
                    lee.FTPport = 21;
                    lee.FTPUser = "pruebaftp";
                    lee.FTPPassword = "pruebaftp";
                    lee.FTPWorkingDirectory = "Reportes";

                    #endregion

                    //opera.GeneraArchivo(archivo, lee);
                }

                LogBMTP.InicializaLog(lee, EnumNivelLog.Trace);                
                

                #region //Prueba de paquete A
                Terminal paqA = new Terminal();
                var entrada = bc;// new BaseConfig();
                //var salida = bc.LeeBaseConfig(ref entrada);

                if (false/*salida.CodError != 0*/)
                {                    
                    //Exception ex = new Exception(salida.Descripcion);
                    //throw ex;
                }
                else
                {
                    paqA.Tarjeta = entrada.Tarjeta;
                    paqA.NumeroTerminal = entrada.Terminal;
                    paqA.MacTarjeta = entrada.MsgMAC;
                }


                
                byte[] mac = new byte[] { 0xdf, 0x72, 0x0f, 0xae, 0xdf, 0xd4, 0xe9, 0x1e, 0xdf, 0x8e, 0x1f, 0x61 };//{ 0x00, 0xc2, 0x00, 0x71, 0x00, 0x09, 0xb3, 0x5a, 0x00, 0xde, 0xbf, 0x82 };//{  0x8e, 0xe9, 0x0f, 0x72, 0x1f, 0xd4, 0x61, 0x1e }      0xdf, , , 0xae, 0xdf, , , , 0xdf,,,  };


                //paqA.Tarjeta = lee.Tarjeta;//53164;//tarjeta  54781 //58977
                //paqA.NumeroTerminal = lee.Terminal;//terminal 
                paqA.FechaHora = DateTime.Now;
                
                Version assemblyversion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                paqA.Version = (ushort)((assemblyversion.Major * 1000) + (assemblyversion.Minor * 10) + (assemblyversion.Build));//version
                 
                //paqA.MacTarjeta = lee.MsgMAC;//mac
                paqA.Tipo = EnumTerminalModelo.TML; //0x0c;
                
                #endregion                
                
                MonedaJuego Monedas = new MonedaJuego();

                Comunicacion com = new Comunicacion(bc, lee, EnumProyecto.MENDOZA, Monedas);
                
                ProtocoloLib.TransacManager.ProtoConfig.CLAVE_TARJETA = BitConverter.GetBytes(0x8EE9AE721FD4611E).Reverse().ToArray();

                //Error errCxn = Comunicacion.AbrePuerto();

                Error errCxn = com.Conectar(paqA, EnumModoConexion.ETHERNET);
                Agente agente = new Agente();
                if (errCxn.CodError != 0)
                {
                    Console.Write("Error: " + errCxn.CodError);
                    Console.WriteLine(" " + errCxn.Descripcion + "\n");
                    
                    Environment.Exit(0);
                }
                else
                {
                    IList objsRec = com.InteraccionAB(ref paqA);

                    TransaccionMSG mensaje;
                    if(objsRec.Count == 6)
                        mensaje = (TransaccionMSG)objsRec[5];
                    

                    if (objsRec[1] is Agente)
                    {
                        agente = (Agente)objsRec[1];
                        
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
                    else if (objsRec.Count > 1)
                    {
                        bool validValue = false;
                        while (!validValue)                        
                        {
                            Console.WriteLine("Seleccione el tipo de mensaje: ");
                            Console.WriteLine("(1) Quiniela");
                            Console.WriteLine("(2) Loto");
                            Console.WriteLine("(3) Quini6");
                            Console.WriteLine("(4) Brinco");
                            Console.WriteLine("(5) Combinada");
                            Console.WriteLine("(6) Polla");
                            Console.WriteLine("(7) Anulación");
                            Console.WriteLine("(8) Reimpresión");
                            Console.WriteLine("(9) Pago premio");
                            Console.WriteLine("(t) Totales");
                            Console.WriteLine("(r) Reportes");
                            Console.WriteLine("(l) Lotipago // (m) Reimpresión Lotipago // (u) Pedido Último Pago");
                            Console.WriteLine("(o) Mostrar Última Apuesta");
                            Console.WriteLine("(s) Pide Habilitaciones de Subagencias");
                            Console.WriteLine("(v) Modifica Habilitaciones de Subagencias");
                            Console.WriteLine("(x) Salir");

                            ConsoleKeyInfo messageType = Console.ReadKey();
                            Console.WriteLine();

                            switch (messageType.KeyChar.ToString())
                            {
                                case "1":
                                    #region //QUINIELA
                                    objsRec2 = com.InteraccionPQ1(PedidosSorteos.QUINIELA, Convert.ToUInt32(paqA.NumeroTerminal), EnumEstadoParametrosOff.HABILITADO);
                                    if (objsRec2.Count > 0 && objsRec2[0] != null && objsRec2[0] is Error)
                                    {
                                        Error psQErr = (Error)objsRec2[0];
                                        if (psQErr.CodError != 0)
                                        {
                                            Console.Write("Error: " + psQErr.CodError);
                                            Console.WriteLine(" " + psQErr.Descripcion + "\n");
                                        }
                                        else if (objsRec[1] is Agente && objsRec2[1] is ParamSorteoQuiniela)
                                        {
                                            psQErr = (Error)objsRec2[0];
                                            if (psQErr.CodError != 0)
                                            {
                                                Console.Write("Error: " + psQErr.CodError);
                                                Console.WriteLine(" " + psQErr.Descripcion + "\n");
                                            }
                                            else if (objsRec[1] is Agente && objsRec2[1] is ParamSorteoQuiniela)
                                            {                                             

                                                ParamSorteoQuiniela psQ = (ParamSorteoQuiniela)objsRec2[1];
                                                TransacQuinielaH cabecera = new TransacQuinielaH();

                                                cabecera.Sorteo = (ushort)psQ.SorteosNumeros[0];
                                                cabecera.FechaHora = DateTime.Now;
                                                cabecera.NroSecuencia = 1;
                                                //cabecera.Entes = psQ.SorteoBmpEntes[0];
                                                byte[] byteEnte = {1,2,3};
                                                //byte[] bt = Conversiones.SeteaBits(byteEnte, 1, true);
                                                //cabecera.Entes = bt[0];
                                                cabecera.CantApu = 5;


                                                objsRec3 = com.InteraccionPQ2(cabecera, jue, PedidosSorteos.QUINIELA);
                                                if (objsRec3[0] != null && objsRec3[0] is Error)
                                                {
                                                    Error TransErr = (Error)objsRec3[0];
                                                    if (TransErr.CodError != 0)
                                                    {
                                                        Console.Write("Error: " + TransErr.CodError);
                                                        Console.WriteLine(" " + TransErr.Descripcion + "\n");
                                                    }
                                                    else if (objsRec3[1] is TransacQuinielaH)
                                                    {
                                                        TransacQuinielaH transRta = (TransacQuinielaH)objsRec3[1];
                                                        certifica.CertificadoQuiniela(transRta.Protocolo, bc.MAC, (int)paqA.Tarjeta, (int)paqA.NumeroTerminal, ref transRta.Certificado);

                                                        LogBMTP.LogBuffer(byteToChar(transRta.Protocolo), "Test LoggeLib", transRta.Protocolo.Length, EnumNivelLog.Trace);

                                                        Console.WriteLine("Número de apuesta de QUINIELA: " + transRta.id_ticket + "\n");
                                                        Console.WriteLine("Número de certificado: " + transRta.Certificado + "\n");
                                                        Console.WriteLine("Fecha y hora de Host: " + transRta.Timehost + "\n");

                                                        cabeceraAnul.id_ticket = transRta.id_ticket;
                                                        cabeceraAnul.Certificado = transRta.Certificado;
                                                        cabeceraAnul.TipoTransacc = transRta.TipoTransacc;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    validValue = false;
                                    break;
                                    #endregion
                                case "2":
                                    #region // LOTO
                                    objsRec2 = com.InteraccionPQ1(PedidosSorteos.LOTO, Convert.ToUInt32(paqA.NumeroTerminal), EnumEstadoParametrosOff.HABILITADO);
                                    if (objsRec2.Count > 0 && objsRec2[0] != null && objsRec2[0] is Error)
                                    {
                                        Error psQErr = (Error)objsRec2[0];
                                        if (psQErr.CodError != 0)
                                        {
                                            Console.Write("Error: " + psQErr.CodError);
                                            Console.WriteLine(" " + psQErr.Descripcion + "\n");
                                        }
                                        else if (objsRec[1] is Agente && objsRec2[1] is ParamSorteoPoceado)
                                        {
                                            agente = (Agente)objsRec[1];
                                            Console.WriteLine("Agencia: " + agente.Nombre + "\nNúmero de Agencia: " + agente.Numero + "\n");

                                            ParamSorteoPoceado psP = (ParamSorteoPoceado)objsRec2[1];

                                            TransacPoceado juePoc = new TransacPoceado();

                                            Console.Write("Ingrese tipo de apuesta (0,1,2): ");
                                            int i = Convert.ToUInt16(Console.ReadLine(), 10);

                                            juePoc.FechaHora = DateTime.Now;
                                            juePoc.Sorteo = (ushort)psP.Sorteo;
                                            Console.WriteLine("Automatica o Manual (55 o 56): ");
                                            byte tra = (byte)Convert.ToUInt16(Console.ReadLine(), 16);
                                            juePoc.TipoTransacc = tra;//0xe5;//cambiar según juego (brinco f5 f6 , loto 55 56 o quini6 e5 e6-- )
                                            juePoc.TipoApuesta = (byte)psP.Adicional[i];
                                            Console.WriteLine("Tipo de documento (0 = SD, 1 = DNI ...): 0");
                                            juePoc.TipoDocumento = 2;
                                            Console.WriteLine("Número de documento: 33274366");
                                            juePoc.NroDocumento = 33274366;
                                            juePoc.NroSecuencia = 1;
                                            juePoc.Importe = psP.Importe[i]*7;//7 digitos - 8 dig * 28 - 6 queda igual * 10 14 15  
                                            juePoc.JackBandera = 1;
                                            juePoc.JackJugada = new string[2] { "2","5" };
                                            juePoc.Jugada = new byte[] { 0x01, 0x04, 0x0b, 0x12, 0x1b, 0x1d, 0x20};

                                            objsRec3 = com.InteraccionPQ2(juePoc, PedidosSorteos.LOTO);

                                            

                                            if (objsRec3[0] != null && objsRec3[0] is Error)
                                            {
                                                psQErr = (Error)objsRec3[0];
                                                if (psQErr.CodError != 0)
                                                {
                                                    Console.Write("Error: " + psQErr.CodError);
                                                    Console.WriteLine(" " + psQErr.Descripcion + "\n");
                                                }
                                                else if (objsRec3.Count > 1 && objsRec3[1] is TransacPoceado)
                                                {
                                                    TransacPoceado transRta = (TransacPoceado)objsRec3[1];

                                                    certifica.CertificadoPoceado(transRta.Protocolo, bc.MAC, (int)paqA.Tarjeta, (int)paqA.NumeroTerminal, ref transRta.Certificado); 

                                                    Console.WriteLine("Timehost: " + transRta.Timehost + "\n");
                                                    Console.WriteLine("ID de transacción: " + transRta.id_transacc + "\n");
                                                    Console.WriteLine("ID Ticket: " + transRta.id_ticket + "\n");
                                                }
                                            }
                                        }
                                    }
                                    validValue = false;
                                    break;
                                    #endregion
                                case "3":
                                    #region //QUINI6
                                    objsRec2 = com.InteraccionPQ1(PedidosSorteos.QUINI6, Convert.ToUInt32(paqA.NumeroTerminal), EnumEstadoParametrosOff.HABILITADO);
                                    if (objsRec2.Count > 0 && objsRec2[0] != null && objsRec2[0] is Error)
                                    {
                                        Error psQErr = (Error)objsRec2[0];
                                        if (psQErr.CodError != 0)
                                        {
                                            Console.Write("Error: " + psQErr.CodError);
                                            Console.WriteLine(" " + psQErr.Descripcion + "\n");
                                        }
                                        else if (objsRec[1] is Agente && objsRec2[1] is ParamSorteoPoceado)
                                        {
                                            agente = (Agente)objsRec[1];
                                            Console.WriteLine("Agencia: " + agente.Nombre + "\nNúmero de Agencia: " + agente.Numero + "\n");

                                            ParamSorteoPoceado psP = (ParamSorteoPoceado)objsRec2[1];

                                            TransacPoceado juePoc = new TransacPoceado();

                                            Console.Write("Ingrese tipo de apuesta (0,1,2): ");
                                            int i = Convert.ToUInt16(Console.ReadLine(), 10);

                                            juePoc.FechaHora = DateTime.Now;
                                            juePoc.Sorteo = (ushort)psP.Sorteo;
                                            Console.WriteLine("Automatica o Manual (e5 o e6): ");
                                            byte tre = (byte)Convert.ToUInt16(Console.ReadLine(), 16);
                                            juePoc.TipoTransacc = tre;//0xe5;//cambiar según juego (brinco f5 f6 , loto 55 56 o quini6 e5 e6-- )
                                            juePoc.TipoApuesta = (byte)psP.Adicional[i];
                                            Console.WriteLine("Tipo de documento (0 = SD, 1 = DNI ...): 0");
                                            juePoc.TipoDocumento = 0;
                                            Console.WriteLine("Número de documento: 0");
                                            juePoc.NroDocumento = 0;
                                            juePoc.NroSecuencia = 1;
                                            juePoc.Importe = psP.Importe[i];
                                            juePoc.Jugada = new byte[] { 0x04, 0x0b, 0x12, 0x1b, 0x1d, 0x20 };

                                            objsRec3 = com.InteraccionPQ2(juePoc, PedidosSorteos.QUINI6);

                                            if (objsRec3[0] != null && objsRec3[0] is Error)
                                            {
                                                psQErr = (Error)objsRec3[0];

                                                if (psQErr.CodError != 0)
                                                {
                                                    Console.Write("Error: " + psQErr.CodError);
                                                    Console.WriteLine(" " + psQErr.Descripcion + "\n");
                                                }
                                                else if (objsRec3.Count > 1 && objsRec3[1] is TransacPoceado)
                                                {                                                    
                                                    TransacPoceado transRta = (TransacPoceado)objsRec3[1];
                                                    certifica.CertificadoPoceado(transRta.Protocolo, bc.MAC, (int)paqA.Tarjeta, (int)paqA.NumeroTerminal, ref transRta.Certificado);
                                                    Console.WriteLine("Timehost: " + transRta.Timehost + "\n");
                                                    Console.WriteLine("ID de transacción: " + transRta.id_transacc + "\n");
                                                    Console.WriteLine("ID Ticket: " + transRta.id_ticket + "\n");
                                                }
                                            }
                                        }
                                    }
                                    validValue = false;
                                    break;
                                    #endregion
                                case "4":
                                    #region // BRINCO
                                    objsRec2 = com.InteraccionPQ1(PedidosSorteos.BRINCO, Convert.ToUInt32(paqA.NumeroTerminal), EnumEstadoParametrosOff.HABILITADO);
                                    if (objsRec2.Count > 0 && objsRec2[0] != null && objsRec2[0] is Error)
                                    {
                                        Error psQErr = (Error)objsRec2[0];
                                        if (psQErr.CodError != 0)
                                        {
                                            Console.Write("Error: " + psQErr.CodError);
                                            Console.WriteLine(" " + psQErr.Descripcion + "\n");
                                        }
                                        else if (objsRec[1] is Agente && objsRec2[1] is ParamSorteoPoceado)
                                        {

                                            agente = (Agente)objsRec[1];
                                            Console.WriteLine("Agencia: " + agente.Nombre + "\nNúmero de Agencia: " + agente.Numero + "\n");

                                            ParamSorteoPoceado psP = (ParamSorteoPoceado)objsRec2[1];

                                            TransacPoceado juePoc = new TransacPoceado();

                                            int i = 0;

                                            juePoc.FechaHora = DateTime.Now;
                                            juePoc.Sorteo = (ushort)psP.Sorteo;
                                            Console.WriteLine("Automatica o Manual (f5 o f6): ");
                                            byte tre = (byte)Convert.ToUInt16(Console.ReadLine(), 16);
                                            juePoc.TipoTransacc = tre;//0xe5;//cambiar según juego (brinco f5 f6 , loto 55 56 o quini6 e5 e6-- )
                                            juePoc.TipoApuesta = (byte)psP.Adicional[i];
                                            Console.WriteLine("Tipo de documento (0 = SD, 1 = DNI ...): 0");
                                            juePoc.TipoDocumento = 0;
                                            Console.WriteLine("Número de documento: 0");
                                            juePoc.NroDocumento = 0;
                                            juePoc.NroSecuencia = 1;
                                            juePoc.Importe = psP.Importe[i];
                                            juePoc.Jugada = new byte[] { 0x04, 0x0b, 0x12, 0x1b, 0x1d, 0x20 };

                                            objsRec3 = com.InteraccionPQ2(juePoc, PedidosSorteos.BRINCO);

                                            if (objsRec3[0] != null && objsRec3[0] is Error)
                                            {
                                                psQErr = (Error)objsRec3[0];
                                                if (psQErr.CodError != 0)
                                                {
                                                    Console.Write("Error: " + psQErr.CodError);
                                                    Console.WriteLine(" " + psQErr.Descripcion + "\n");
                                                }
                                                else if (objsRec3.Count > 1 && objsRec3[1] is TransacPoceado)
                                                {
                                                    TransacPoceado transRta = (TransacPoceado)objsRec3[1];
                                                    certifica.CertificadoPoceado(transRta.Protocolo, bc.MAC, (int)paqA.Tarjeta, (int)paqA.NumeroTerminal, ref transRta.Certificado);
                                                    Console.WriteLine("Timehost: " + transRta.Timehost + "\n");
                                                    Console.WriteLine("ID de transacción: " + transRta.id_transacc + "\n");
                                                    Console.WriteLine("ID Ticket: " + transRta.id_ticket + "\n");
                                                }
                                            }
                                        }
                                    }
                                    validValue = false;
                                    break;
                                    #endregion
                                case "5":
                                    #region //COMBINADA
                                    objsRec2 = com.InteraccionPQ1(PedidosSorteos.COMBINADA_MZA, Convert.ToUInt32(paqA.NumeroTerminal), EnumEstadoParametrosOff.DESHABILITADO);

                                    if (objsRec2.Count > 0 && objsRec2[0] != null && objsRec2[0] is Error)
                                    {
                                        Error psQErr = (Error)objsRec2[0];
                                        if (psQErr.CodError != 0)
                                        {
                                            Console.Write("Error: " + psQErr.CodError);
                                            Console.WriteLine(" " + psQErr.Descripcion + "\n");
                                        }
                                        else if (objsRec[1] is Agente && objsRec2[1] is ParamSorteoPoceado)
                                        {
                                            agente = (Agente)objsRec[1];
                                            Console.WriteLine("Agencia: " + agente.Nombre + "\nNúmero de Agencia: " + agente.Numero + "\n");

                                            ParamSorteoPoceado psP = (ParamSorteoPoceado)objsRec2[1];

                                            TransacPoceado juePoc = new TransacPoceado();

                                            int i = 5;
                                            Console.Write("Cantidad de números (5,6,7,8): " + i + "\n");

                                            juePoc.FechaHora = DateTime.Now;
                                            juePoc.Sorteo = (ushort)psP.Sorteo;
                                            Console.WriteLine("Automatica o Manual (c5 o c6): ");

                                            juePoc.TipoTransacc = (byte)Convert.ToUInt16(Console.ReadLine(), 16);//cambiar según juego (combinada c5 c6 c7 o polla 51 52, quiniela poceada 65 66)
                                            juePoc.TipoApuesta = (byte)i;//cambiar según juego (combinada cantidad de numeros (5 6 7 8) o polla rango(10 15 20))
                                            Console.WriteLine("Tipo de documento (0 = SD, 1 = DNI ...): 0");
                                            juePoc.TipoDocumento = 0;
                                            Console.WriteLine("Número de documento: 0");
                                            juePoc.NroDocumento = 0;
                                            juePoc.NroSecuencia = 1;
                                            juePoc.Importe = (uint)psP.Importe[i - 5];

                                            juePoc.Jugada = new byte[] { 0x02, 0x04, 0x22, 0x0a, 0x0f, /*0x23, 0x24, 0x25, 0x26, 0x27 */};

                                            objsRec3 = com.InteraccionPQ2(juePoc, PedidosSorteos.COMBINADA_MZA);

                                            if (objsRec3[0] != null && objsRec3[0] is Error)
                                            {
                                                psQErr = (Error)objsRec3[0];
                                                if (psQErr.CodError != 0)
                                                {
                                                    Console.Write("Error: " + psQErr.CodError);
                                                    Console.WriteLine(" " + psQErr.Descripcion + "\n");
                                                }
                                                else if (objsRec3.Count > 1 && objsRec3[1] is TransacPoceado)
                                                {
                                                    TransacPoceado transRta = (TransacPoceado)objsRec3[1];
                                                    //certifica.CertificadoPoceado(transRta.Protocolo, bc.MAC, (int)paqA.Tarjeta, (int)paqA.NumeroTerminal, ref transRta.Certificado);

                                                    poceadoAnul.id_ticket = transRta.id_ticket;// para la posterior reimpresion
                                                    poceadoAnul.TipoTransacc = transRta.TipoTransacc;

                                                    Console.WriteLine("Timehost: " + transRta.Timehost + "\n");
                                                    Console.WriteLine("ID de transacción: " + transRta.id_transacc + "\n");
                                                    Console.WriteLine("ID Ticket: " + transRta.id_ticket + "\n");
                                                }
                                            }
                                        }
                                    }

                                    validValue = false;
                                    break;
                                    #endregion
                                case "6":
                                    #region // POLLA
                                    objsRec2 = com.InteraccionPQ1(PedidosSorteos.POLLA, Convert.ToUInt32(paqA.NumeroTerminal), EnumEstadoParametrosOff.HABILITADO);


                                    if (objsRec2.Count > 0 && objsRec2[0] != null && objsRec2[0] is Error)
                                    {
                                        Error psQErr = (Error)objsRec2[0];
                                        if (psQErr.CodError != 0)
                                        {
                                            Console.Write("Error: " + psQErr.CodError);
                                            Console.WriteLine(" " + psQErr.Descripcion + "\n");
                                        }
                                        else if (objsRec[1] is Agente && objsRec2[1] is ParamSorteoPoceado)
                                        {
                                            agente = (Agente)objsRec[1];
                                            Console.WriteLine("Agencia: " + agente.Nombre + "\nNúmero de Agencia: " + agente.Numero + "\n");

                                            ParamSorteoPoceado psP = (ParamSorteoPoceado)objsRec2[1];

                                            TransacPoceado juePoc = new TransacPoceado();

                                            int i = 1;
                                            Console.Write("Rango (1, 15, 20): " + i + "\n");

                                            juePoc.FechaHora = DateTime.Now;
                                            juePoc.Sorteo = (ushort)psP.Sorteo;
                                            Console.WriteLine("Automatica o Manual (51 o 52): ");
                                            juePoc.TipoTransacc = (byte)Convert.ToUInt16(Console.ReadLine(), 16); ;//cambiar según juego (combinada c5 c6 c7 o polla 51 52)
                                            juePoc.TipoApuesta = 15;//cambiar según juego (combinada cantidad de numeros (5 6 7 8) o polla rango(10 15 20))
                                            Console.WriteLine("Tipo de documento (0 = SD, 1 = DNI ...): 0 \n");
                                            juePoc.TipoDocumento = 0;
                                            Console.WriteLine("Número de documento: 0 \n");
                                            juePoc.NroDocumento = 0;
                                            juePoc.NroSecuencia = 1;
                                            juePoc.Importe = (uint)psP.Importe[i];

                                            juePoc.Jugada = new byte[] { 0x02, 0x04, 0x22, 0x0a, 0x0f, 0x23, 0x24, 0x25, 0x26, 0x27 };

                                            objsRec3 = com.InteraccionPQ2(juePoc, PedidosSorteos.POLLA);

                                            if (objsRec3[0] != null && objsRec3[0] is Error)
                                            {
                                                psQErr = (Error)objsRec3[0];
                                                if (psQErr.CodError != 0)
                                                {
                                                    Console.Write("Error: " + psQErr.CodError);
                                                    Console.WriteLine(" " + psQErr.Descripcion + "\n");
                                                }
                                                else if (objsRec3.Count > 1 && objsRec3[1] is TransacPoceado)
                                                {
                                                    TransacPoceado transRta = (TransacPoceado)objsRec3[1];
                                                    certifica.CertificadoPoceado(transRta.Protocolo, bc.MAC, (int)paqA.Tarjeta, (int)paqA.NumeroTerminal, ref transRta.Certificado);

                                                    poceadoAnul.id_ticket = transRta.id_ticket;// para la posterior reimpresion
                                                    poceadoAnul.TipoTransacc = transRta.TipoTransacc;

                                                    Console.WriteLine("Timehost: " + transRta.Timehost + "\n");
                                                    Console.WriteLine("ID de transacción: " + transRta.id_transacc + "\n");
                                                    Console.WriteLine("ID Ticket: " + transRta.id_ticket + "\n");
                                                }
                                            }
                                        }
                                    }
                                    validValue = false;
                                    break;
                                    #endregion
                                case "L":
                                    #region // LOTERIA
                                    objsRec2 = com.InteraccionPQ1(PedidosSorteos.LOTERIA, Convert.ToUInt32(paqA.NumeroTerminal), EnumEstadoParametrosOff.HABILITADO);

                                    if (objsRec2.Count > 0 && objsRec2[0] != null && objsRec2[0] is Error)
                                    {
                                        Error psQErr = (Error)objsRec2[0];
                                        if (psQErr.CodError != 0)
                                        {
                                            Console.Write("Error: " + psQErr.CodError);
                                            Console.WriteLine(" " + psQErr.Descripcion + "\n");
                                        }
                                        else if (objsRec[1] is Agente && objsRec2[1] is ParamSorteoPoceado)
                                        {
                                            
                                        }
                                    }
                                    validValue = false;
                                    break;
                                    #endregion                                
                                case "7":
                                    #region //ANULACIÓN MANUAL

                                    Error err = new Error();

                                    //TransacQuinielaH cabecera1 = new TransacQuinielaH();
                                    //TransacQuinielaB cuerpos1 = new TransacQuinielaB();
                                    //AnulReimpQuiniela anulacion1 = new AnulReimpQuiniela();

                                    TransacPoceado transacP = new TransacPoceado();
                                    AnulReimpPoceado anulacion = new AnulReimpPoceado();

                                    err = com.ReimprimirUltimo(paqA.Tarjeta, EnumJuegos.QUINIELAPOCEADA, paqA.NumeroTerminal, 
                                        EnumAnulReimp.ULT_REIMP_COMPLETO, ref transacP, ref anulacion);

                                    Console.WriteLine("Ingrese apuesta (c5, c6, 51, 52, 80): ");
                                    transacP.TipoTransacc = (byte)Convert.ToUInt16(Console.ReadLine(), 16);

                                    Console.WriteLine("Ingrese Id Ticket: ");
                                    transacP.id_ticket = Console.ReadLine();

                                    Console.WriteLine("Ingrese certificado: ");
                                    transacP.Certificado = Console.ReadLine();

                                    anulacion.TipoTransacc = 0xa0;

                                    err = com.AnularPQ((uint)paqA.Tarjeta, ref transacP, ref anulacion);

                                    validValue = false;
                                    break;
                                    #endregion
                                case "8":
                                    #region //REIMPRESIÓN MANUAL
                                    Error err2 = new Error();

                                    //Console.WriteLine("Ingrese apuesta (c5, c6, 51, 52, 80): ");
                                    //byte ap = (byte)Convert.ToUInt16(Console.ReadLine(), 16);
                                    byte ap = (byte)EnumTipoTransaccion.QPOCEADA_AUTO;
                                    if (ap == (byte)EnumTipoTransaccion.QPOCEADA_AUTO || ap == (byte)EnumTipoTransaccion.QPOCEADA_MANUAL)
                                    {
                                        cabeceraAnul.TipoTransacc = ap;
                                        Console.WriteLine("Ingrese Id Ticket: ");
                                        cabeceraAnul.id_ticket = Console.ReadLine();
                                        //Console.WriteLine("Ingrese nro offline: ");
                                        //int nroOf = Convert.ToUInt16(Console.ReadLine());
                                        anulacionQ.TipoTransacc = (byte)EnumAnulReimp.REIMP;
                                        err2 = com.ReimprimirPQ(paqA.Tarjeta, ref poceadoAnul, ref anulacionP);
                                    }
                                    else if(ap == (byte)EnumTipoTransaccion.QUINIELA)
                                    {
                                        poceadoAnul.TipoTransacc = ap;
                                        Console.WriteLine("Ingrese Id Ticket: ");
                                        poceadoAnul.id_ticket = Console.ReadLine();
                                        Console.WriteLine("Ingrese nro offline: ");
                                        int nroOf = Convert.ToUInt16(Console.ReadLine());
                                        anulacionP.TipoTransacc = (byte)EnumAnulReimp.ANUL;
                                        err2 = com.ReimprimirPQ(paqA.Tarjeta, ref cabeceraAnul, ref cuerposAnul, ref anulacionQ);
                                    }
                                    if (err2 != null)
                                    {
                                        if (err2.CodError != 0)
                                        {
                                            Console.Write("Error: " + err2.CodError);
                                            Console.WriteLine(" " + err2.Descripcion + "\n");
                                        }
                                        else
                                        {
                                            
                                            if (ap == (byte)EnumTipoTransaccion.QUINIELA)
                                            {
                                                int y = certifica.CertificadoQuiniela(cabeceraAnul.Protocolo, bc.MAC, (int)paqA.Tarjeta, (int)paqA.NumeroTerminal, ref anulacionQ.Certificado); 
                                                Console.WriteLine("Fecha Sorteo: " + anulacionQ.FechaSorteo + "\n");
                                                Console.WriteLine("Reimpresiones: " + anulacionQ.Reimpresion + "\n");
                                                Console.WriteLine("ID Ticket REIMPRESIÓN: " + anulacionQ.id_ticket + "\n");
                                                Console.WriteLine("Certificado REIMPRESIÓN: " + anulacionQ.Certificado + "\n");
                                            }
                                            else
                                            {
                                                int y = certifica.CertificadoPoceado(poceadoAnul.Protocolo, bc.MAC, (int)paqA.Tarjeta, (int)paqA.NumeroTerminal, ref anulacionP.Certificado); 
                                                Console.WriteLine("Fecha Sorteo: " + anulacionP.FechaSorteo + "\n");
                                                Console.WriteLine("Reimpresiones: " + anulacionP.Reimpresion + "\n");
                                                Console.WriteLine("ID Ticket REIMPRESIÓN: " + anulacionP.id_ticket + "\n");
                                            }
                                        }
                                    }

                                    validValue = false;
                                    break;
                                    #endregion
                                case "9":
                                    #region//PAGO PREMIOS
                                    Error errPagoPrem1 = new Error();
                                    TransacPoceado pagopremio = new TransacPoceado();
                                    Console.WriteLine("ID TICKET: ");
                                    pagopremio.id_ticket = Console.ReadLine();

                                    errPagoPrem1 = com.PagoPremio1PQ(paqA.Tarjeta, ref pagopremio);
                                    if (errPagoPrem1.CodError != 0)
                                    {
                                        Console.WriteLine("Error: " + errPagoPrem1.CodError + " " + errPagoPrem1.Descripcion + "\n");
                                    }
                                    else
                                    {
                                        AnulReimpPoceado premio = new AnulReimpPoceado();
                                        Error errPagoPrem2 = new Error();
                                        Console.WriteLine("Importe premio: " + pagopremio.Importe + "\n");
                                        Console.WriteLine("CERTIFICADO: ");
                                        pagopremio.Certificado = Console.ReadLine();
                                        errPagoPrem2 = com.PagoPremio2PQ(paqA, pagopremio, ref premio);
                                        if (errPagoPrem2.CodError != 0)
                                        {
                                            Console.WriteLine("Error: " + errPagoPrem2.CodError + " " + errPagoPrem2.Descripcion + "\n");
                                        }
                                        else
                                        {
                                            Console.WriteLine("Sorteo: " + premio.Sorteo + "\n");
                                            Console.WriteLine("Fecha Sorteo: " + premio.FechaSorteo + "\n");
                                            Console.WriteLine("Fecha Pago Premio: " + premio.FechaHora + "\n");
                                            Console.WriteLine("ID Premio: " + premio.id_ticket + "\n");
                                            Console.WriteLine("ID Apuesta: " + pagopremio.id_ticket + "\n");
                                        }
                                    }
                                    break;
                                    #endregion
                                case "j":
                                    #region//PAGO PREMIOS OFF
                                    Error errPagoPremOff1 = new Error();
                                    TransacPoceado pagopremioOff = new TransacPoceado();
                                    Console.WriteLine("ID TICKET: ");
                                    pagopremioOff.id_ticket = Console.ReadLine();

                                    errPagoPremOff1 = com.PagoPremioOff1PQ(paqA.Tarjeta, ref pagopremioOff);
                                    if (errPagoPremOff1.CodError != 0)
                                    {
                                        Console.WriteLine("Error: " + errPagoPremOff1.CodError + " " + errPagoPremOff1.Descripcion + "\n");
                                    }
                                    else
                                    {
                                        AnulReimpPoceado premio = new AnulReimpPoceado();
                                        Error errPagoPremOff2 = new Error();
                                        Console.WriteLine("Importe premio: " + pagopremioOff.Importe + "\n");
                                        Console.WriteLine("CERTIFICADO: ");
                                        pagopremioOff.Certificado = Console.ReadLine();
                                        errPagoPremOff2 = com.PagoPremioOff2PQ(paqA, pagopremioOff, ref premio);
                                        if (errPagoPremOff2.CodError != 0)
                                        {
                                            Console.WriteLine("Error: " + errPagoPremOff2.CodError + " " + errPagoPremOff2.Descripcion + "\n");
                                        }
                                        else
                                        {
                                            Console.WriteLine("Sorteo: " + premio.Sorteo + "\n");
                                            Console.WriteLine("Fecha Sorteo: " + premio.FechaSorteo + "\n");
                                            Console.WriteLine("Fecha Pago Premio: " + premio.FechaHora + "\n");
                                            Console.WriteLine("ID Premio: " + premio.id_ticket + "\n");
                                            Console.WriteLine("ID Apuesta: " + pagopremioOff.id_ticket + "\n");
                                        }
                                    }
                                    break;
                                    #endregion
                                case "t":
                                case "T":
                                    #region //TOTALES

                                    Error err4 = new Error();
                                    Totales totales = new Totales();
                                    totales.FechaHora = DateTime.Now;

                                    if (objsRec[1] is Agente)
                                    {
                                        agente = (Agente)objsRec[1];
                                        totales.Agente = agente;
                                    }
                                    totales.Terminal = paqA;

                                    Console.WriteLine("Ingrese tipo de total (xAgente fe, xTerminal fd, xSorteo fc): ");
                                    totales.TipoTransacc = (byte)Convert.ToUInt16(Console.ReadLine(), 16);
                                    Console.WriteLine("Ingrese juego: ");
                                    totales.Juego = (uint)Convert.ToUInt32(Console.ReadLine());

                                    if (totales.TipoTransacc == (byte)PedidoTotales.SORTEO_SERVICIO)
                                    {
                                        Console.WriteLine("Ingrese sorteo: ");
                                        totales.Sorteo = (uint)Convert.ToUInt32(Console.ReadLine());
                                    }

                                    err4 = com.Totales(ref totales);

                                    if (totales.TipoTransacc == (byte)PedidoTotales.SORTEO_SERVICIO)
                                    {
                                        Console.WriteLine("Sorteo: " + totales.Sorteo + "\n");
                                    }
                                    Console.WriteLine("Día de pedido: " + totales.FechaHora + "\n");
                                    Console.WriteLine("Fecha del total: " + totales.TimeHost + "\n");
                                    Console.WriteLine("Importe Ventas: " + totales.VtaNetaImp + "\n");
                                    Console.WriteLine("Cantidad Ventas: " + totales.VtaNetaCant + "\n");
                                    Console.WriteLine("Importe Premios Pagados: " + totales.PremiosPagadosImp + "\n");
                                    Console.WriteLine("Cantidad Premios Pagados: " + totales.PremiosPagadosCant + "\n");
                                    Console.WriteLine("Caja: " + totales.TotalCaja + "\n");
                                    Console.WriteLine("Importe Reimpresiones: " + totales.ReimpresionesImp + "\n");
                                    Console.WriteLine("Cantidad : " + totales.ReimpresionesCant + "\n");
                                    Console.WriteLine("Importe Anulaciones:" + totales.AnulacionesImp + "\n");
                                    Console.WriteLine("Cantidad : " + totales.AnulacionesCant + "\n");

                                    break;
                                    #endregion
                                case "r":
                                case "R":
                                    #region //REPORTES
                                    Console.WriteLine("(1) Reporte Boleta Deposito");
                                    Console.WriteLine("(2) Reporte Extracto");
                                    Console.WriteLine("(3) Reporte Premios Totales");
                                    Console.WriteLine("(4) Reporte Premios Pagados Sorteo");
                                    Console.WriteLine("(5) Reporte Premios Pendiente Detalle");
                                    Console.WriteLine("(6) Reporte Premios Prescriptos Detalle");
                                    Console.WriteLine("(7) Reporte Premios Pagados Detalle");
                                    Console.WriteLine("(8) Reporte Resultado de un Sorteo Agente");
                                    Console.WriteLine("(9) Reporte Reimpersión Pago de Premios");
                                    Console.WriteLine("(a) Reporte Lotipago");
                                    Console.WriteLine("(b) Reporte Lotipago");
                                    Console.WriteLine("(i) Pedido Insumos");
                                    Console.WriteLine("(r) Carga rollo");
                                    Console.WriteLine("(x) Salir");

                                    messageType = Console.ReadKey();
                                    Console.WriteLine();

                                    switch (messageType.KeyChar.ToString())
                                    {
                                        case "1":
                                            #region //REPORTES Boleta de Deposito
                                            Error errRepo = new Error();
                                            Reportes repo = new Reportes();

                                            if (objsRec[1] is Agente)
                                            {
                                                agente = (Agente)objsRec[1];
                                            }
                                            Console.WriteLine("Ingrese fecha: ");
                                            Console.WriteLine("Día: ");
                                            int dia = Convert.ToUInt16(Console.ReadLine());
                                            Console.WriteLine("Mes: ");
                                            int mes = Convert.ToUInt16(Console.ReadLine());
                                            Console.WriteLine("Año: ");
                                            int anio = Convert.ToUInt16(Console.ReadLine());
                                            Console.WriteLine("Ingrese pin: ");
                                            repo.Clave = Convert.ToUInt16(Console.ReadLine());
                                            Console.WriteLine("Ingrese agente: ");
                                            int agen = Convert.ToInt32(Console.ReadLine());
                                            repo.Fecha = new DateTime(anio, mes, dia);

                                            errRepo = com.PedirReporteBoletaDeposito(ref repo, paqA, agen);

                                            Print(repo.Buffer, 2);

                                            break;
                                            #endregion
                                        case "2":
                                            #region //REPORTES Extracto
                                            Error errRepoEx = new Error();
                                            Reportes repoEx = new Reportes();

                                            if (objsRec[1] is Agente)
                                            {
                                                agente = (Agente)objsRec[1];
                                            }
                                            EnumJuegos ju = 0;
                                            Console.WriteLine("Ingrese sorteo: ");
                                            repoEx.Sorteo = Convert.ToUInt16(Console.ReadLine());
                                            Console.WriteLine("Juego: QUINIELA = 1, LOTO = 3, QUINI6 = 4, COMBINADA = 5, BRINCO = 6, POLLA = 10. ");
                                            string op = Console.ReadLine();
                                            do
                                            {
                                                switch (op)
                                                {
                                                    case "1":
                                                        ju = EnumJuegos.QUINIELA; break;
                                                    case "3":
                                                        ju = EnumJuegos.LOTO; break;
                                                    case "4":
                                                        ju = EnumJuegos.QUINI6; break;
                                                    case "5":
                                                        ju = EnumJuegos.COMBINADA; break;
                                                    case "6":
                                                        ju = EnumJuegos.BRINCO; break;
                                                    case "10":
                                                        ju = EnumJuegos.POLLA; break;
                                                    default:
                                                        Console.WriteLine("Debe seleccionar una opción válida. Seleccionó: " + messageType.KeyChar.ToString());
                                                        break;
                                                }
                                            }
                                            while (op != "1" && op != "3" && op != "4" && op != "5" && op != "6" && op != "10");

                                            errRepoEx = com.PedirReporteExtractos(ref repoEx, paqA, 1, 1, ju, 2);

                                            Console.WriteLine(repoEx.Buffer + " \n");

                                            break;
                                            #endregion
                                        case "3":
                                            #region //REPORTES Premios Totales
                                            Error errRepoPT = new Error();
                                            Reportes repoPT = new Reportes();

                                            if (objsRec[1] is Agente)
                                            {
                                                agente = (Agente)objsRec[1];
                                            }
                                            ju = 0;

                                            Console.WriteLine("Juego: QUINIELA = 1, LOTO = 3, QUINI6 = 4, COMBINADA = 5, BRINCO = 6, POLLA = 10. ");
                                            op = Console.ReadLine();
                                            do
                                            {
                                                switch (op)
                                                {
                                                    case "1":
                                                        ju = EnumJuegos.QUINIELA; break;
                                                    case "3":
                                                        ju = EnumJuegos.LOTO; break;
                                                    case "4":
                                                        ju = EnumJuegos.QUINI6; break;
                                                    case "5":
                                                        ju = EnumJuegos.COMBINADA; break;
                                                    case "6":
                                                        ju = EnumJuegos.BRINCO; break;
                                                    case "10":
                                                        ju = EnumJuegos.POLLA; break;
                                                    default:
                                                        Console.WriteLine("Debe seleccionar una opción válida. Seleccionó: " + messageType.KeyChar.ToString());
                                                        break;
                                                }
                                            }
                                            while (op != "1" && op != "3" && op != "4" && op != "5" && op != "6" && op != "10");
                                            Console.WriteLine("Ingrese pin: ");
                                            repoPT.Clave = Convert.ToUInt16(Console.ReadLine());
                                            Console.WriteLine("Ingrese fecha: ");
                                            Console.WriteLine("Día: ");
                                            dia = Convert.ToUInt16(Console.ReadLine());
                                            Console.WriteLine("Mes: ");
                                            mes = Convert.ToUInt16(Console.ReadLine());
                                            Console.WriteLine("Año: ");
                                            anio = Convert.ToUInt16(Console.ReadLine());

                                            repoPT.Fecha = new DateTime(anio, mes, dia);
                                            Console.WriteLine("Ingrese agente: ");
                                            agen = Convert.ToInt32(Console.ReadLine());
                                            errRepoPT = com.PedirReportePremiosTotales(ref repoPT, paqA, agen, ju);

                                            Console.WriteLine(repoPT.Buffer + " \n");

                                            break;
                                            #endregion
                                        case "4":
                                            #region //REPORTES Premios Pagados Sorteo
                                            Error errRepoPS = new Error();
                                            Reportes repoPS = new Reportes();

                                            if (objsRec[1] is Agente)
                                            {
                                                agente = (Agente)objsRec[1];
                                            }
                                            ju = 0;

                                            Console.WriteLine("Juego: QUINIELA = 1, LOTO = 3, QUINI6 = 4, COMBINADA = 5, BRINCO = 6, POLLA = 10. ");
                                            op = Console.ReadLine();
                                            do
                                            {
                                                switch (op)
                                                {
                                                    case "1":
                                                        ju = EnumJuegos.QUINIELA; break;
                                                    case "3":
                                                        ju = EnumJuegos.LOTO; break;
                                                    case "4":
                                                        ju = EnumJuegos.QUINI6; break;
                                                    case "5":
                                                        ju = EnumJuegos.COMBINADA; break;
                                                    case "6":
                                                        ju = EnumJuegos.BRINCO; break;
                                                    case "10":
                                                        ju = EnumJuegos.POLLA; break;
                                                    default:
                                                        Console.WriteLine("Debe seleccionar una opción válida. Seleccionó: " + messageType.KeyChar.ToString());
                                                        break;
                                                }
                                            }
                                            while (op != "1" && op != "3" && op != "4" && op != "5" && op != "6" && op != "10");
                                            Console.WriteLine("Ingrese pin: ");
                                            repoPS.Clave = Convert.ToUInt16(Console.ReadLine());
                                            Console.WriteLine("Ingrese fecha: ");
                                            Console.WriteLine("Día: ");
                                            dia = Convert.ToUInt16(Console.ReadLine());
                                            Console.WriteLine("Mes: ");
                                            mes = Convert.ToUInt16(Console.ReadLine());
                                            Console.WriteLine("Año: ");
                                            anio = Convert.ToUInt16(Console.ReadLine());

                                            repoPS.Fecha = new DateTime(anio, mes, dia);
                                            Console.WriteLine("Ingrese agente: ");
                                            agen = Convert.ToInt32(Console.ReadLine());
                                            errRepoPS = com.PedirReportePremiosPagadosSorteo(ref repoPS, paqA, agen, ju);

                                            Console.WriteLine(repoPS.Buffer + " \n");

                                            break;
                                            #endregion
                                        case "5":
                                            #region //REPORTES Premios Pendientes Detalle
                                            Error errRepoPD = new Error();
                                            Reportes repoPD = new Reportes();

                                            if (objsRec[1] is Agente)
                                            {
                                                agente = (Agente)objsRec[1];
                                            }
                                            ju = 0;

                                            Console.WriteLine("Juego: QUINIELA = 1, LOTO = 3, QUINI6 = 4, COMBINADA = 5, BRINCO = 6, POLLA = 10. ");
                                            op = Console.ReadLine();
                                            do
                                            {
                                                switch (op)
                                                {
                                                    case "1":
                                                        ju = EnumJuegos.QUINIELA; break;
                                                    case "3":
                                                        ju = EnumJuegos.LOTO; break;
                                                    case "4":
                                                        ju = EnumJuegos.QUINI6; break;
                                                    case "5":
                                                        ju = EnumJuegos.COMBINADA; break;
                                                    case "6":
                                                        ju = EnumJuegos.BRINCO; break;
                                                    case "10":
                                                        ju = EnumJuegos.POLLA; break;
                                                    default:
                                                        Console.WriteLine("Debe seleccionar una opción válida. Seleccionó: " + messageType.KeyChar.ToString());
                                                        break;
                                                }
                                            }
                                            while (op != "1" && op != "3" && op != "4" && op != "5" && op != "6" && op != "10");
                                            Console.WriteLine("Ingrese pin: ");
                                            repoPD.Clave = Convert.ToUInt16(Console.ReadLine());
                                            Console.WriteLine("Ingrese fecha: ");
                                            Console.WriteLine("Día: ");
                                            dia = Convert.ToUInt16(Console.ReadLine());
                                            Console.WriteLine("Mes: ");
                                            mes = Convert.ToUInt16(Console.ReadLine());
                                            Console.WriteLine("Año: ");
                                            anio = Convert.ToUInt16(Console.ReadLine());

                                            repoPD.Fecha = new DateTime(anio, mes, dia);
                                            Console.WriteLine("Ingrese agente: ");
                                            agen = Convert.ToInt32(Console.ReadLine());
                                            errRepoPD = com.PedirRepPremPendDetalle(ref repoPD, paqA, agen, ju);

                                            Console.WriteLine(repoPD.Buffer + " \n");

                                            break;
                                            #endregion
                                        case "6":
                                            #region //REPORTES Premios Prescriptos Detalle
                                            Error errRepoPrD = new Error();
                                            Reportes repoPrD = new Reportes();

                                            if (objsRec[1] is Agente)
                                            {
                                                agente = (Agente)objsRec[1];
                                            }
                                            ju = 0;

                                            Console.WriteLine("Juego: QUINIELA = 1, LOTO = 3, QUINI6 = 4, COMBINADA = 5, BRINCO = 6, POLLA = 10. ");
                                            op = Console.ReadLine();
                                            do
                                            {
                                                switch (op)
                                                {
                                                    case "1":
                                                        ju = EnumJuegos.QUINIELA; break;
                                                    case "3":
                                                        ju = EnumJuegos.LOTO; break;
                                                    case "4":
                                                        ju = EnumJuegos.QUINI6; break;
                                                    case "5":
                                                        ju = EnumJuegos.COMBINADA; break;
                                                    case "6":
                                                        ju = EnumJuegos.BRINCO; break;
                                                    case "10":
                                                        ju = EnumJuegos.POLLA; break;
                                                    default:
                                                        Console.WriteLine("Debe seleccionar una opción válida. Seleccionó: " + messageType.KeyChar.ToString());
                                                        break;
                                                }
                                            }
                                            while (op != "1" && op != "3" && op != "4" && op != "5" && op != "6" && op != "10");
                                            Console.WriteLine("Ingrese pin: ");
                                            repoPrD.Clave = Convert.ToUInt16(Console.ReadLine());
                                            Console.WriteLine("Ingrese fecha: ");
                                            Console.WriteLine("Día: ");
                                            dia = Convert.ToUInt16(Console.ReadLine());
                                            Console.WriteLine("Mes: ");
                                            mes = Convert.ToUInt16(Console.ReadLine());
                                            Console.WriteLine("Año: ");
                                            anio = Convert.ToUInt16(Console.ReadLine());

                                            repoPrD.Fecha = new DateTime(anio, mes, dia);
                                            Console.WriteLine("Ingrese agente: ");
                                            agen = Convert.ToInt32(Console.ReadLine());
                                            errRepoPrD = com.PedirRepPremPresDetalle(ref repoPrD, paqA, agen, ju);

                                            Console.WriteLine(repoPrD.Buffer + " \n");

                                            break;
                                            #endregion
                                        case "7":
                                            #region //REPORTES Premios Pagados Detalle
                                            Error errRepoPgD = new Error();
                                            Reportes repoPgD = new Reportes();

                                            if (objsRec[1] is Agente)
                                            {
                                                agente = (Agente)objsRec[1];
                                            }
                                            ju = 0;

                                            Console.WriteLine("Juego: QUINIELA = 1, LOTO = 3, QUINI6 = 4, COMBINADA = 5, BRINCO = 6, POLLA = 10. ");
                                            op = Console.ReadLine();
                                            do
                                            {
                                                switch (op)
                                                {
                                                    case "1":
                                                        ju = EnumJuegos.QUINIELA; break;
                                                    case "3":
                                                        ju = EnumJuegos.LOTO; break;
                                                    case "4":
                                                        ju = EnumJuegos.QUINI6; break;
                                                    case "5":
                                                        ju = EnumJuegos.COMBINADA; break;
                                                    case "6":
                                                        ju = EnumJuegos.BRINCO; break;
                                                    case "10":
                                                        ju = EnumJuegos.POLLA; break;
                                                    default:
                                                        Console.WriteLine("Debe seleccionar una opción válida. Seleccionó: " + messageType.KeyChar.ToString());
                                                        break;
                                                }
                                            }
                                            while (op != "1" && op != "3" && op != "4" && op != "5" && op != "6" && op != "10");
                                            Console.WriteLine("Ingrese pin: ");
                                            repoPgD.Clave = Convert.ToUInt16(Console.ReadLine());
                                            Console.WriteLine("Ingrese fecha: ");
                                            Console.WriteLine("Día: ");
                                            dia = Convert.ToUInt16(Console.ReadLine());
                                            Console.WriteLine("Mes: ");
                                            mes = Convert.ToUInt16(Console.ReadLine());
                                            Console.WriteLine("Año: ");
                                            anio = Convert.ToUInt16(Console.ReadLine());

                                            repoPgD.Fecha = new DateTime(anio, mes, dia);
                                            Console.WriteLine("Ingrese agente: ");
                                            agen = Convert.ToInt32(Console.ReadLine());
                                            errRepoPgD = com.PedirRepPremPagDetalle(ref repoPgD, paqA, agen, ju);

                                            Console.WriteLine(repoPgD.Buffer + " \n");

                                            break;
                                            #endregion
                                        case "8":
                                            #region //REPORTES Resultado de un Sorteo Agente
                                            Error errRepoRSA = new Error();
                                            Reportes repoRSA = new Reportes();

                                            if (objsRec[1] is Agente)
                                            {
                                                agente = (Agente)objsRec[1];
                                            }
                                            ju = 0;

                                            Console.WriteLine("Juego: QUINIELA = 1, LOTO = 3, QUINI6 = 4, COMBINADA = 5, BRINCO = 6, POLLA = 10. ");
                                            op = Console.ReadLine();
                                            do
                                            {
                                                switch (op)
                                                {
                                                    case "1":
                                                        ju = EnumJuegos.QUINIELA; break;
                                                    case "3":
                                                        ju = EnumJuegos.LOTO; break;
                                                    case "4":
                                                        ju = EnumJuegos.QUINI6; break;
                                                    case "5":
                                                        ju = EnumJuegos.COMBINADA; break;
                                                    case "6":
                                                        ju = EnumJuegos.BRINCO; break;
                                                    case "10":
                                                        ju = EnumJuegos.POLLA; break;
                                                    default:
                                                        Console.WriteLine("Debe seleccionar una opción válida. Seleccionó: " + messageType.KeyChar.ToString());
                                                        break;
                                                }
                                            }
                                            while (op != "1" && op != "3" && op != "4" && op != "5" && op != "6" && op != "10");
                                            Console.WriteLine("Ingrese pin: ");
                                            repoRSA.Clave = Convert.ToUInt16(Console.ReadLine());
                                            Console.WriteLine("Ingrese fecha: ");
                                            Console.WriteLine("Día: ");
                                            dia = Convert.ToUInt16(Console.ReadLine());
                                            Console.WriteLine("Mes: ");
                                            mes = Convert.ToUInt16(Console.ReadLine());
                                            Console.WriteLine("Año: ");
                                            anio = Convert.ToUInt16(Console.ReadLine());

                                            repoRSA.Fecha = new DateTime(anio, mes, dia);
                                            Console.WriteLine("Ingrese agente: ");
                                            agen = Convert.ToInt32(Console.ReadLine());
                                            errRepoRSA = com.PedirRdoSorteoAgente(ref repoRSA, paqA, agen, ju);

                                            Console.WriteLine(repoRSA.Buffer + " \n");

                                            break;
                                            #endregion
                                        case "9":
                                            #region //REPORTES Reimpresión pago de Premios
                                            Error errRepoRPP = new Error();
                                            Reportes repoRPP = new Reportes();

                                            if (objsRec[1] is Agente)
                                            {
                                                agente = (Agente)objsRec[1];
                                            }

                                            Console.WriteLine("Ingrese ID de la transacción: ");
                                            string id = Console.ReadLine();

                                            Console.WriteLine("Ingrese pin: ");
                                            repoRPP.Clave = Convert.ToUInt16(Console.ReadLine());

                                            Console.WriteLine("Ingrese origen: ");
                                            int origen = Convert.ToUInt16(Console.ReadLine());

                                            Console.WriteLine("Ingrese agente: ");
                                            agen = Convert.ToInt32(Console.ReadLine());
                                            errRepoRPP = com.PedirRepReimpPagoPrem(ref repoRPP, paqA, agen, id, origen);

                                            Console.WriteLine(repoRPP.Buffer + " \n");

                                            break;
                                            #endregion
                                        case "a":
                                        case "A":
                                            #region //REPORTES Lotipago
                                            Error errRepoL = new Error();
                                            Reportes repoL = new Reportes();

                                            if (objsRec[1] is Agente)
                                            {
                                                agente = (Agente)objsRec[1];
                                            }

                                            Console.WriteLine("Ingrese pin: ");
                                            repoL.Clave = Convert.ToUInt16(Console.ReadLine());

                                            Console.WriteLine("Ingrese agente: ");
                                            agen = Convert.ToInt32(Console.ReadLine());

                                            Console.WriteLine("Ingrese fecha: ");
                                            Console.WriteLine("Día: ");
                                            dia = Convert.ToUInt16(Console.ReadLine());
                                            Console.WriteLine("Mes: ");
                                            mes = Convert.ToUInt16(Console.ReadLine());
                                            Console.WriteLine("Año: ");
                                            anio = Convert.ToUInt16(Console.ReadLine());

                                            repoL.Subtipo = 1;

                                            repoL.Fecha = new DateTime(anio, mes, dia);

                                            errRepoL = com.PedirRepLotipago(ref repoL, paqA, agen, 10, 2010);

                                            Console.WriteLine(repoL.Buffer + " \n");

                                            break;
                                            #endregion
                                        case "b":
                                            #region //REPORTES SubAgencia
                                            Error errRepoSubAg = new Error();
                                            Reportes repoSubAg = new Reportes();

                                            if (objsRec[1] is Agente)
                                            {
                                                agente = (Agente)objsRec[1];
                                            }
                                            Console.WriteLine("Ingrese fecha: ");
                                            Console.WriteLine("Día: ");
                                            dia = Convert.ToUInt16(Console.ReadLine());
                                            Console.WriteLine("Mes: ");
                                            mes = Convert.ToUInt16(Console.ReadLine());
                                            Console.WriteLine("Año: ");
                                            anio = Convert.ToUInt16(Console.ReadLine());
                                            Console.WriteLine("Ingrese pin: ");
                                            repoSubAg.Clave = Convert.ToUInt16(Console.ReadLine());
                                            Console.WriteLine("Ingrese agente: ");
                                            agen = Convert.ToInt32(Console.ReadLine());
                                            repoSubAg.Fecha = new DateTime(anio, mes, dia);

                                            errRepoSubAg = com.PedirReporteSubAgencia(ref repoSubAg, paqA, agen, 1, 10);

                                            Print(repoSubAg.Buffer, 2);

                                            break;
                                            #endregion
                                        case "c":
                                            #region //REPORTES Certificado Retenciones
                                            Error errRepoCR = new Error();
                                            Reportes repoCR = new Reportes();

                                            if (objsRec[1] is Agente)
                                            {
                                                agente = (Agente)objsRec[1];
                                            }
                                            Console.WriteLine("Ingrese fecha: ");
                                            Console.WriteLine("Día: ");
                                            dia = Convert.ToUInt16(Console.ReadLine());
                                            Console.WriteLine("Mes: ");
                                            mes = Convert.ToUInt16(Console.ReadLine());
                                            Console.WriteLine("Año: ");
                                            anio = Convert.ToUInt16(Console.ReadLine());
                                            Console.WriteLine("Ingrese pin: ");
                                            repoCR.Clave = Convert.ToUInt16(Console.ReadLine());
                                            Console.WriteLine("Ingrese agente: ");
                                            agen = Convert.ToInt32(Console.ReadLine());
                                            repoCR.Fecha = new DateTime(anio, mes, dia);

                                            errRepoCR = com.PedirReporteCertificacionRetenciones(ref repoCR, paqA, agen, 1, 10);

                                            Print(repoCR.Buffer, 2);

                                            break;
                                            #endregion
                                        case "e":
                                            #region //REPORTES Extractos
                                            errRepoEx = new Error();
                                            repoEx = new Reportes();
                                            if (objsRec[1] is Agente)
                                            {
                                                agente = (Agente)objsRec[1];
                                            }                                            
                                            Console.WriteLine("Ingrese pin: ");
                                            repoEx.Clave = Convert.ToUInt16(Console.ReadLine());
                                            Console.WriteLine("Ingrese agente: ");
                                            agen = Convert.ToInt32(Console.ReadLine());                                            

                                            errRepoEx = com.PedirReporteExtractosLoteria(ref repoEx, paqA, agen, (byte)1);

                                            Print(repoEx.Buffer, 2);

                                            break;
                                            #endregion
                                        case "f":
                                            #region //REPORTES Loteria
                                            errRepoEx = new Error();
                                            repoEx = new Reportes();
                                            if (objsRec[1] is Agente)
                                            {
                                                agente = (Agente)objsRec[1];
                                            }
                                            
                                            Console.WriteLine("Ingrese sorteo: ");
                                            repoEx.Sorteo = Convert.ToUInt16(Console.ReadLine());
                                            Console.WriteLine("Ingrese billete: ");
                                            agen = Convert.ToInt32(Console.ReadLine());
                                            repoEx.Fecha = DateTime.Now;                                            

                                            errRepoEx = com.PedirReporteLoteria(ref repoEx, agen, (int)50027, (int)45, (int)paqA.NumeroTerminal, (int)agente.Numero);

                                            if(errRepoEx.CodError == 0)
                                                Print(repoEx.Buffer, 2);

                                            break;
                                            #endregion
                                        case "R":
                                            #region //REPORTES Loteria
                                            errRepoEx = new Error();
                                            repoEx = new Reportes();
                                            if (objsRec[1] is Agente)
                                            {
                                                agente = (Agente)objsRec[1];
                                                errRepoEx = com.PedirReporteReimpresionLoteria(ref repoEx, paqA, agente.Numero/1000, "000000000000000000");
                                            }

                                            if (errRepoEx.CodError == 0)
                                                Print(repoEx.Buffer, 2);

                                            break;
                                            #endregion
                                        case "v":
                                        case "V":
                                            #region //REPORTES Cambio Clave
                                            Error errRepoCC = new Error();
                                            

                                            if (objsRec[1] is Agente)
                                            {
                                                agente = (Agente)objsRec[1];
                                            }
                                            Console.WriteLine("Ingrese agente: ");
                                            agen = Convert.ToInt32(Console.ReadLine());
                                            Console.WriteLine("Ingrese pin actual: ");
                                            uint pin = (uint)Convert.ToUInt16(Console.ReadLine());
                                            uint pin1 = 0, pin2 = 0;
                                            Console.WriteLine("Ingrese nuevo pin: ");
                                            pin1 = (uint)Convert.ToUInt16(Console.ReadLine());
                                            Console.WriteLine("Reingrese nuevo pin: ");
                                            pin2 = (uint)Convert.ToUInt16(Console.ReadLine());

                                            errRepoCC = com.PedirCambioClave(paqA, agen, pin, pin1, pin2);

                                            if (errRepoCC.CodError != 0)
                                            {
                                                Console.WriteLine(errRepoCC.CodError + " " + errRepoCC.Descripcion);
                                            }
                                            
                                            break;
                                            #endregion
                                        case "i":
                                        case "I":
                                            #region //REPORTES Pedido Insumos
                                            Error errRepoPI = new Error();
                                            Insumos insumos = new Insumos();
                                            insumos.Pedidos = new byte[5];                                            
                                            
                                            if (objsRec[1] is Agente)
                                            {
                                                agente = (Agente)objsRec[1];
                                            }
                                            Console.WriteLine("Cosa 1: ");
                                            insumos.Pedidos[0] = Convert.ToByte(Console.ReadLine());
                                            Console.WriteLine("Cosa 2: ");
                                            insumos.Pedidos[1] = Convert.ToByte(Console.ReadLine());

                                            errRepoPI = com.PedirInsumos(paqA, agente.Numero, ref insumos);

                                            if (errRepoPI.CodError != 0)
                                            {
                                                Console.WriteLine(errRepoPI.CodError + " " + errRepoPI.Descripcion);
                                            }
                                            else
                                            {
                                                Console.WriteLine("Error raro o.0");
                                            }
                                            break;
                                            #endregion
                                        case "r":
                                            #region//REPORTES Carga Rollo
                                            Error errRepoCRo = new Error();
                                            if (objsRec[1] is Agente)
                                            {
                                                agente = (Agente)objsRec[1];
                                            }
                                            CambioRollo cambioRollo = new CambioRollo();
                                            Console.WriteLine("Ingrese nro rollo: ");
                                            cambioRollo.NroRollo = Console.ReadLine();

                                            errRepoCRo = com.CargaRollo(paqA, ref cambioRollo, DateTime.Now); 
 
                                            if (errRepoCRo.CodError != 0)
                                            {
                                                Console.WriteLine(errRepoCRo.CodError + " " + errRepoCRo.Descripcion);
                                            }
                                            else
                                            {
                                                //muestro
                                            }
                                            break;
                                            #endregion
                                        case "z":
                                            #region //REPORTE Reimpresion Descarga
                                            errRepoEx = new Error();
                                            repoEx = new Reportes();
                                            if (objsRec[1] is Agente)
                                            {
                                                agente = (Agente)objsRec[1];
                                                errRepoEx = com.PedirReporteReimpDescarga(ref repoEx, paqA, agente.Numero/1000, 344556);
                                            }

                                            if (errRepoEx.CodError == 0)
                                                Print(repoEx.Buffer, 2);
                                            break;
                                            #endregion
                                        case "x":
                                        case "X":
                                            validValue = true;
                                            break;
                                        default:
                                            Console.WriteLine("Debe seleccionar un valor válido. Seleccionó: " + messageType.KeyChar.ToString());
                                            validValue = false;
                                            break;
                                            
                                    }
                                    break;
                                    #endregion
                                case "l":                                
                                    #region //LOTIPAGO
                                    EstadoEmpresa estEmp = new EstadoEmpresa();
                                    CodigosBarras codBarras = new CodigosBarras();
                                    Error errL = com.PedirEstadoEmpresa(paqA, ref estEmp);
                                    Error errL2;
                                    Error errL3;
                                    Error errL4;
                                    if (errL.CodError != 0)
                                        Console.WriteLine(errL.Descripcion);
                                    else if (errL.CodError == 0)
                                    {
                                        codBarras.CantCodigos = 1;
                                        codBarras.CodigoBarra = new string[1];
                                        codBarras.CodigoBarra[0] = "00820259783892"; //"0390000002100045220708310800046497";//"00820000065573"; //"0140000004300147110808310800151061";//"0192323232300003231705310500003283";//"0141132131300021122704300400021199";//"0290023232303223321705310503275974";//"0192323232300003231705310500003283";//"0702323232303223321705310503275979";// "588000056941307915246044397";//"0141132131300021122704300400021199";//"0192323232300003231705310500003283";////"4570023232313113423434343434343000232323131134";//"0059501022005066000408300852";//"588000040830505100595008526";//"386000000010000002814010320057";//"0691140004400033001203310300033739";//"0701135973300201781003310300206720";////"0711142073500060001203310300061330";//"386000000010000002814010320057";//"386000191069051001090150320059";//"0391141836300005001203310300005119";// "0691140004400033001203310300033739";//Console.ReadLine();
                                        //codBarras.CodigoBarra[1] = "0000114183635";// "0000114000440";
                                        codBarras.Longitud = new int[1];
                                        codBarras.Longitud[0] = codBarras.CodigoBarra[0].Length;
                                        //codBarras.Longitud[1] = codBarras.CodigoBarra[1].Length;

                                        DatosPago datosPago = new DatosPago();
                                        errL2 = com.PedirDatosPago(paqA, codBarras, ref datosPago);
                                        if (errL2.CodError != 0)
                                            Console.WriteLine(errL2.Descripcion);
                                        else
                                        {
                                            Console.WriteLine("Codigo de empresa: " + datosPago.CodEmpresa);

                                            string[] txt = new string[10];
                                            int cantLineas = 0;
                                            datosPago.ImportePago = 72;

                                            errL3 = com.PedirDatosRapipago(paqA, codBarras, ref datosPago, ref txt, ref cantLineas, 0);
                                            errL4 = com.ConfirmacionImpresionPago(ref datosPago);

                                            Console.WriteLine("Código de pago: " + datosPago.IdPagoIVISA);
                                            //errL2 = com.PedirCodigoServicio(paqA, ref datosUltPago);
                                        }
                                    }

                                    break;
                                
                                case "m":
                                    DatosPago datosPag = new DatosPago();
                                    int cantL = 0;
                                    string[] texto = new string[10];
                                    Console.WriteLine("IdPago: ");
                                    datosPag.IdPagoIVISA = Convert.ToUInt32(Console.ReadLine());                                     
                                    Error errM = com.PedirReimpPago(paqA, ref datosPag, ref texto, ref cantL);
                                    if (errM.CodError != 0) {
                                        Console.WriteLine(errM.CodError + " ");
                                        Console.WriteLine(errM.Descripcion);
                                    }else
                                    { Console.WriteLine("Certificado reimpresión: " + datosPag.Certificado);}
                                    break;
                                case "u":
                                case "U":
                                    DatosPago datosUltPago = new DatosPago();
                                    Error errPU = com.PedirUltimoPago(paqA, ref datosUltPago);
                                    if (errPU.CodError != 0) {
                                        Console.WriteLine(errPU.CodError + " ");
                                        Console.WriteLine(errPU.Descripcion);
                                    } else
                                    { Console.WriteLine("Código último pago: " + datosUltPago.IdPagoIVISA); }                                    
                                    break;
                                case "k":
                                    byte[] prueb = new byte[] { 0xdf, 0x72, 0x0f, 0xae, 0xdf, 0xd4, 0xe9, 0x1e, 0xdf, 0x8e, 0x1f, 0x61, 0xdf, 0x72, 0x0f, 0xae, 0xdf, 0xd4, 0xe9, 0x1e, 0xdf, 0x8e, 0x1f, 0x61, 0xdf, 0x72, 0x0f, 0xae, 0xdf, 0xd4, 0xe9, 0x1e, 0xdf, 0x8e };
                                    string cb = BitConverter.ToString(prueb, 0, 34);
                                    break;
                                case "x":
                                case "X":
                                    validValue = true;
                                    break;
                                default:
                                    Console.WriteLine("Debe seleccionar un valor válido. Seleccionó: " + messageType.KeyChar.ToString());
                                    validValue = false;
                                    break;
                                    #endregion
                                case "o":
                                    #region //REIMPRIMIR ULTIMO
                                    //TransacQuinielaH cab = new TransacQuinielaH();
                                    //TransacQuinielaB bod = new TransacQuinielaB();
                                    AnulReimpPoceado ultReimp = new AnulReimpPoceado();
                                    //cab.TipoTransacc = (byte)EnumTipoTransaccion.QUINIELA;
                                    TransacPoceado transac = new TransacPoceado();
                                    transac.TipoTransacc = (byte)EnumTipoTransaccion.QPOCEADA_AUTO;
                                    //Console.WriteLine("Ingrese Id Ticket: ");
                                    //cab.id_ticket = Console.ReadLine();

                                    err2 = com.ReimprimirUltimo(paqA.Tarjeta, EnumJuegos.QUINIELAPOCEADA, paqA.NumeroTerminal, EnumAnulReimp.ULT_REIMP_COMPLETO, 
                                        ref transac, ref ultReimp);

                                    if (err2 != null)
                                    {
                                        if (err2.CodError != 0)
                                        {
                                            Console.Write("Error: " + err2.CodError);
                                            Console.WriteLine(" " + err2.Descripcion + "\n");
                                        }
                                        else
                                        {
                                            Console.WriteLine("ID Ticket última quiniela: " + transac.id_ticket + "\n");
                                            Console.WriteLine("Importe total: " + transac.Importe + "\n");
                                            Console.WriteLine("Reimpresiones: " + ultReimp.Reimpresion + "\n");
                                            Console.WriteLine("ID Ticket pedido reimpresion : " + ultReimp.id_ticket + "\n");                                            
                                        }
                                    }

                                    break;
                                    #endregion                                
                                case "p": case "P":
                                    #region //UNIT TEST
                                    Console.WriteLine("Carga de Sorteo de Quiniela == InteraccionPQ1 ? ");
                                    //ut.CargaSorteoVsInteraccionPQ1(lee);
                                    Console.WriteLine("Carga de Sorteo de Loto == InteraccionPQ1 ? ");
                                    //ut.CargaSorteoVsInteraccionPQ1(paqA, com, agente, lee, PedidosSorteos.LOTO);
                                    Console.WriteLine("Carga de Sorteo de Quini6 == InteraccionPQ1 ? ");
                                    //ut.CargaSorteoVsInteraccionPQ1(paqA, com, agente, lee, PedidosSorteos.QUINI6);
                                    Console.WriteLine("Carga de Sorteo de Brinco == InteraccionPQ1 ? ");
                                    //ut.CargaSorteoVsInteraccionPQ1(paqA, com, agente, lee, PedidosSorteos.BRINCO);
                                    break;
                                    #endregion                                
                                case "s":
                                    #region //PEDIDO HABILITACIONES SUBAGENCIAS
                                    HabilitacionSubagente report = new HabilitacionSubagente();
                                    err = com.PedidoHabilitacionesSubAg(ref report, paqA, agente.Numero);
                                    break;
                                    #endregion
                                case "v":
                                    #region //MODIFICA HABILITACIONES SUBAGENCIAS
                                    report = new HabilitacionSubagente();
                                    
                                    report.cantidad = 3;
                                    report.motivo = new byte[] {1, 0 , 1 };
                                    report.nombre = new string[] {"Pedro", "Juan", "Laura"};
                                    report.status = new byte[] {0, 0, 1 };
                                    report.subAgente = new int[] { 6001, 6002, 6003 };
                                    
                                    err = com.ModificaHabilitacionesSubAg(ref report, paqA, agente.Numero);
                                    break;
                                    #endregion                                
                                case "a":
                                    #region //TOTALES LOTIPAGO
                                    Totales dt = new Totales();
                                    com.PedirTotalesLotipago(paqA, ref dt, 2, paqA.NumeroTerminal);
                                    break;
                                    #endregion
                                case "w":
                                    #region //TOTALES GENERALES
                                    TotalesGenerales totGenerales = new TotalesGenerales();
                                    totGenerales.TipoTransacc = (uint)PedidoTotales.AGENTE;
                                    totGenerales.Terminal = paqA;
                                    totGenerales.Agente = agente;
                                    com.TotalesAll(ref totGenerales);
                                    break;
                                    #endregion
                                case "e":
                                    #region //PEDIDO JUGADA POCEADA
                                    TransacLoteria loteria = new TransacLoteria();
                                    TransacPoceado traPoc = new TransacPoceado();
                                    TransacQuinielaH cab = new TransacQuinielaH();
                                    TransacQuinielaB body = new TransacQuinielaB();
                                    Console.WriteLine("Ingrese Id Ticket: ");
                                    traPoc.id_ticket = Console.ReadLine();
                                    cab.id_ticket = traPoc.id_ticket;
                                    EnumJuegos juego = new EnumJuegos();
                                    uint cantNumeros = 0;

                                    com.PedirJugada(paqA, ref traPoc, ref cantNumeros, ref cab, ref body, ref loteria, ref juego);

                                    break;
                                    #endregion
                                case "y":
                                    #region //VERIFICA TELEKINO
                                    TransacPoceado kino = new TransacPoceado();
                                    Console.WriteLine("Ingrese Id Ticket: ");
                                    kino.id_ticket = Console.ReadLine();

                                    uint prem = 0;
                                    uint aciertos = 0;
                                    err = com.VerificaTelekino(paqA, ref kino, ref prem, ref aciertos);

                                    break;
                                #endregion
                                case "q":
                                    #region // Q4

                                    TotalesGenerales totQ4 = new TotalesGenerales();
                                    Console.WriteLine("Ingrese fecha: ");
                                    Console.WriteLine("Día: ");
                                    int dia2 = Convert.ToUInt16(Console.ReadLine());

                                    Console.WriteLine("Mes: ");
                                    int mes2 = Convert.ToUInt16(Console.ReadLine());
                                    
                                    Console.WriteLine("Año: ");                                    
                                    int anio2 = Convert.ToUInt16(Console.ReadLine());

                                    Console.WriteLine("Ingrese agente: ");
                                    int agen2 = Convert.ToInt32(Console.ReadLine());

                                    DateTime fecha = new DateTime(anio2, mes2, dia2);

                                    err = com.TotalesQ4(paqA, ref totQ4, fecha, agen2); 
                                    break;
                                    #endregion
                            }
                        }

                        validValue = false;

                        while (!validValue)
                        {
                            Console.WriteLine("(1) Anulación Quiniela");
                            Console.WriteLine("(2) Reimpresión Quiniela");
                            Console.WriteLine("(3) Anulación Combinada o Polla");
                            Console.WriteLine("(4) Reimpresión Combinada o Polla");
                            Console.WriteLine("(x) Salir");

                            ConsoleKeyInfo messageType = Console.ReadKey();
                            Console.WriteLine();
                            Error err = new Error();

                            switch (messageType.KeyChar.ToString())
                            {
                                case "1":
                                    #region // Anulación Quiniela

                                    err = com.AnularPQ(paqA.Tarjeta, ref cabeceraAnul, ref cuerposAnul, ref anulacionQ);

                                    if (err != null)
                                    {
                                        if (err.CodError != 0)
                                        {
                                            Console.Write("Error: " + err.CodError);
                                            Console.WriteLine(" " + err.Descripcion + "\n");
                                        }
                                        else
                                        {
                                            Console.WriteLine("Fecha Sorteo: " + anulacionQ.FechaSorteo + "\n");
                                            Console.WriteLine("Descripción: " + anulacionQ.NombreSorteo + "\n");
                                            Console.WriteLine("ID Ticket ANULADO: " + anulacionQ.id_ticket + "\n");

                                        }
                                    }

                                    validValue = false;
                                    break;
                                    #endregion
                                case "2":
                                    #region // Reimpresión Quiniela

                                    err = com.ReimprimirPQ(paqA.Tarjeta, ref cabeceraAnul, ref cuerposAnul, ref anulacionQ);

                                    if (err != null)
                                    {
                                        if (err.CodError != 0)
                                        {
                                            Console.Write("Error: " + err.CodError);
                                            Console.WriteLine(" " + err.Descripcion + "\n");
                                        }
                                        else
                                        {
                                            Console.WriteLine("Fecha Sorteo: " + anulacionQ.FechaSorteo + "\n");
                                            Console.WriteLine("Reimpresiones: " + anulacionQ.Reimpresion + "\n");
                                            Console.WriteLine("ID Ticket REIMPRESIÓN: " + anulacionQ.id_ticket + "\n");
                                            Console.WriteLine("Certificado REIMPRESIÓN: " + anulacionQ.Certificado + "\n");
                                        }
                                    }

                                    validValue = false;
                                    break;
                                    #endregion
                                case "3":
                                    #region //Anulación Combinada o Polla
                                    //    TransacPoceado trans2 = (TransacPoceado)objsRec3[1];
                                    //    objsRec4 = com.AnulaReimprimePQ(paqA.Tarjeta, 0x52, trans2.id_ticket, "4360194521463512", (byte)0xa0);

                                    //    if (objsRec4[0] != null && objsRec4[0] is Error)
                                    //    {
                                    //        Error psQErr = (Error)objsRec4[0];
                                    //        if (psQErr.CodError != 0)
                                    //        {
                                    //            Console.Write("Error: " + psQErr.CodError);
                                    //            Console.WriteLine(" " + psQErr.Descripcion + "\n");
                                    //        }
                                    //        else if (objsRec4.Count > 1 && objsRec4[1] is AnulReimpPoceado)
                                    //        {
                                    //            AnulReimpPoceado transRta = (AnulReimpPoceado)objsRec4[1];
                                    //            Console.WriteLine("Fecha Sorteo: " + transRta.FechaSorteo + "\n");
                                    //            Console.WriteLine("Descripción: " + transRta.NombreSorteo + "\n");
                                    //            Console.WriteLine("ID Ticket ANULADO: " + transRta.id_ticket + "\n");
                                    //        }
                                    //    }
                                    validValue = true;
                                    break;
                                    #endregion
                                case "4":
                                    #region //Reimpresión Combinada o Polla
                                    err = com.ReimprimirPQ(paqA.Tarjeta, ref poceadoAnul, ref anulacionP);

                                    if (err != null)
                                    {
                                        if (err.CodError != 0)
                                        {
                                            Console.Write("Error: " + err.CodError);
                                            Console.WriteLine(" " + err.Descripcion + "\n");
                                        }
                                        else
                                        {
                                            Console.WriteLine("Fecha Sorteo: " + anulacionP.FechaSorteo + "\n");
                                            Console.WriteLine("Descripción: " + anulacionP.NombreSorteo + "\n");
                                            Console.WriteLine("Reimpresión: " + anulacionP.Reimpresion + "\n");
                                            Console.WriteLine("ID Ticket REIMPRESIÓN: " + anulacionP.id_ticket + "\n");
                                        }
                                    }

                                    validValue = false;
                                    break;
                                    #endregion
                                case "x":
                                case "X":
                                    com.CortarCom(true);
                                    Environment.Exit(0);
                                    validValue = true;
                                    break;
                                default:
                                    Console.WriteLine("Debe seleccionar un valor válido. Seleccionó: " + messageType.KeyChar.ToString());
                                    validValue = false;
                                    break;
                            }
                        }
                    }
                }
            }
            catch(Exception e)
            {
                Console.WriteLine("Exception: {0}", e.ToString());
            }
        }

        public static void Print(byte[] buffer, int margenIzq)
        {
            string sEsc = "\x1b";
            string sGS = "\x1d";
            string escSalto = sEsc + "d" + "\x01";
            string asciiSalto = "\r";
            string escLetraGrande = sEsc + "!\x68";
            string asciiLetraGrande = Char.ConvertFromUtf32(18);
            string asciiFinLetraGrande = Char.ConvertFromUtf32(17);
            string asciiCodBarra = Char.ConvertFromUtf32(20);
            string escFinLetraGrande = sEsc + "!\0";
            string escCortePapel = sEsc + "i";
            string sgsAlturaCodBarras = sGS + "h28";
            string sgsCodBarra = "    " + sgsAlturaCodBarras + sGS + "k" + "\x46" + "\x16";
            string nombreImpresora = "THERMAL Receipt Printer";

            string stringText = System.Text.Encoding.UTF8.GetString(buffer);
            string[] arrayText = stringText.Select(c => c.ToString()).ToArray();

            StringBuilder copiaText = new StringBuilder();

            // Inicializa impresora
            RawPrinterHelper.SendStringToPrinter(nombreImpresora, sEsc + "@");

            // Agrega la cancelacion de letra grande al final de cada linea
            bool agregaFinLetraGrande = false;

            // Margen izquierdo
            for (int i = 0; i < margenIzq; i++)
            {
                copiaText.Append(" ");
            }

            foreach (string caracter in arrayText)
            {
                copiaText.Append(caracter);

                // Comienzo negrita
                if (caracter == asciiLetraGrande)
                {
                    agregaFinLetraGrande = true;
                }
                else if (caracter == asciiSalto && agregaFinLetraGrande)
                {
                    copiaText.Append(asciiFinLetraGrande);
                    agregaFinLetraGrande = false;
                }

                // Margen izquierdo
                if (caracter == asciiSalto)
                {
                    for (int i = 0; i < margenIzq; i++)
                    {
                        copiaText.Append(" ");
                    }
                }
            }

            // Agrega saltos de linea al final
            for (int i = 0; i < 6; i++) copiaText.Append(asciiSalto);


            // Vuelve a convertir el texto a un array
            stringText = copiaText.ToString();
            arrayText = stringText.Select(c => c.ToString()).ToArray();

            // Salto de linea 
            arrayText = arrayText.Select(s => s.Replace(asciiSalto, escSalto)).ToArray();

            // Letra grande
            arrayText = arrayText.Select(s => s.Replace(asciiLetraGrande, escLetraGrande)).ToArray();

            // Fin letra grande
            arrayText = arrayText.Select(s => s.Replace(asciiFinLetraGrande, escFinLetraGrande)).ToArray();

            // Codigo de Barras
            arrayText = arrayText.Select(s => s.Replace(asciiCodBarra, sgsCodBarra)).ToArray();

            stringText = ConvertStringArrayToString(arrayText);

            // Imprime reporte
            RawPrinterHelper.SendStringToPrinter(nombreImpresora, stringText);

            CortarPapel(nombreImpresora);

            // Inicializa impresora
            RawPrinterHelper.SendStringToPrinter(nombreImpresora, sEsc + "@");
        }

        static string ConvertStringArrayToString(string[] array)
        {
            //
            // Concatenate all the elements into a StringBuilder.
            //
            StringBuilder builder = new StringBuilder();
            foreach (string value in array)
            {
                builder.Append(value);
            }
            return builder.ToString();
        }

        public static void CortarPapel(string nombreImpresora)
        {
            string ESC = "\x1b";
            string cut = ESC + "i";
            RawPrinterHelper.SendStringToPrinter(nombreImpresora, cut);
        }

        public static void WriteMultiLineByteArray(byte[] bytes, string name)
        {
            //if (dbg == "debug")
            //{
                const int rowSize = 16;
                const string underLine = "--------------------------------";
                int iter;

                Console.WriteLine(name);
                Console.WriteLine(underLine.Substring(0,
                    Math.Min(name.Length, underLine.Length)));

                for (iter = 0; iter < bytes.Length - rowSize; iter += rowSize)
                {
                    Console.Write(BitConverter.ToString(bytes, iter, rowSize));
                    Console.WriteLine("-");
                }

                Console.WriteLine(BitConverter.ToString(bytes, iter));
                Console.WriteLine();
            //}
        }

        public static void WriteMultiLineByteArray(IList objs, string name)
        {
            //if (dbg == "debug")
            //{
                const string underLine = "---------------------------------";

                Console.WriteLine(name);
                Console.WriteLine(underLine.Substring(0,
                    Math.Min(name.Length, underLine.Length)));

                foreach (Object ob in objs)
                {
                    if (ob is Array && ob is char[])
                    {
                        char[] arr = (char[])ob;
                        for (int k = 0; k < arr.Length; k++)
                        {
                            Console.Write(arr[k]);
                            Console.Write("-");
                        }
                    }
                    else if (ob is Array && ob is int[])
                    {
                        int[] arr = (int[])ob;
                        for (int k = 0; k < arr.Length; k++)
                        {
                            Console.Write(arr[k]);
                            Console.Write("-");
                        }
                    }
                    else if (!(ob is byte[]))
                    {
                        Console.Write(ob);
                        Console.WriteLine("-");
                    }
                }
                Console.WriteLine();
            //}
        }
    }
}
