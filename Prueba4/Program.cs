﻿using System;
using System.Collections.Generic;
using System.Text;
using LibreriaConexion;
using System.Collections;
using LibreriaClases.Clases;
using LibreriaClases;
using System.Linq;
using BinConfig;
using System.Net;
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

                #region //Prueba de paquete A
                Terminal paqA = new Terminal();
                var entrada = new BaseConfig();
                var salida = bc.LeeBaseConfig(ref entrada);

                if (salida.CodError != 0)
                {                    
                    Exception ex = new Exception(salida.Descripcion);
                    throw ex;
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

                Comunicacion com = new Comunicacion(bc, lee);
                
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
                            Console.WriteLine("(x) Salir");

                            ConsoleKeyInfo messageType = Console.ReadKey();
                            Console.WriteLine();

                            switch (messageType.KeyChar.ToString())
                            {
                                case "1":
                                    #region
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
                                                        //certifica.CertificadoQuiniela(transRta.Protocolo, bc.MAC, (int)paqA.Tarjeta, (int)paqA.NumeroTerminal, ref transRta.Certificado);

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
                            }
                        }

                        validValue = false;

                        while (!validValue)
                        {
                            Console.WriteLine("(x) Salir");

                            ConsoleKeyInfo messageType = Console.ReadKey();
                            Console.WriteLine();
                            Error err = new Error();

                            switch (messageType.KeyChar.ToString())
                            {                                
                                case "x":
                                case "X":
                                    com.Desconectar(false);
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
    }
}
