using System;
using System.Net;
using System.Net.Sockets;
using System.Security.Permissions;
using System.Net.NetworkInformation;
using System.Collections;
using System.Collections.Generic;
using LibreriaClases.Clases;
using LibreriaClases;
using BinConfig;
using System.IO.Ports;
using DotRas;
using System.Linq;
using ProtocoloLib;
using LoggerLib;

namespace LibreriaConexion
{
    public partial class Comunicacion: IDisposable
    {
        public OperaOffLine opOff = new OperaOffLine();

        public TransacManager TR;

        private IList<string> TIPOS_NACK = new List<string> { "B", "E", "T", "P" };

        public int bytesCount { get; set; }
        public byte[] bytes { get; set; }

        //tipo de paquete esperado
        public static EnumPaquete tipoPaq = EnumPaquete.DATOS;

        #region Propiedades Logger
        public EnumNivelLog NivelLog;
        public const EnumNivelLog lvlLogTransaccion = EnumNivelLog.Info;
        public const EnumNivelLog lvlLogCabeceraBuffer = EnumNivelLog.Info;
        public const EnumNivelLog lvlLogCxn = EnumNivelLog.Info;
        public const EnumNivelLog lvlLogError = EnumNivelLog.Error;
        public const EnumNivelLog lvlLogExcepciones = EnumNivelLog.Fatal;
        public const EnumNivelLog lvlLogDebug = EnumNivelLog.Debug;
        public const bool TimeStampLog = true;
        #endregion

        #region CONSTRUCTORES
        public Comunicacion(BaseConfig baseConf, ArchivoConfig conf)
        {
            TransacManager.ProtoConfig = new ProtocoloConfig(baseConf, conf);

            switch (TransacManager.ProtoConfig.CONFIG.LevelLog)
            {
                case EnumMessageType.DEBUG: NivelLog = EnumNivelLog.Trace; break;
                case EnumMessageType.ERROR: NivelLog = EnumNivelLog.Error; break;
                case EnumMessageType.NORMAL: NivelLog = EnumNivelLog.Info; break;
                case EnumMessageType.NOTHING: NivelLog = EnumNivelLog.Off; break;
                case EnumMessageType.WARNING: NivelLog = EnumNivelLog.Warn; break;
            }

            DateTime d = DateTime.Now;

            string fName = conf.LogFileName.Split('.')[0]
                + d.Year.ToString().PadLeft(2, '0')
                + d.Month.ToString().PadLeft(2, '0')
                + d.Day.ToString().PadLeft(2, '0')
                + d.Hour.ToString().PadLeft(2, '0')
                + d.Minute.ToString().PadLeft(2, '0')
                + d.Second.ToString().PadLeft(2, '0')
                + "." + conf.LogFileName.Split('.')[1];


            LogBMTP.InicializaLog(conf, NivelLog, fName);

            TR = new TransacManager();
        }
        public Comunicacion(BaseConfig baseConf, ArchivoConfig conf, bool interno)
        {
            switch(TransacManager.ProtoConfig.CONFIG.LevelLog)
            {
                case EnumMessageType.DEBUG: NivelLog = EnumNivelLog.Trace; break;
                case EnumMessageType.ERROR: NivelLog = EnumNivelLog.Error; break;
                case EnumMessageType.NORMAL: NivelLog = EnumNivelLog.Info; break;
                case EnumMessageType.NOTHING: NivelLog = EnumNivelLog.Off; break;
                case EnumMessageType.WARNING: NivelLog = EnumNivelLog.Warn; break;
            }

            DateTime d = DateTime.Now;

            string fName = conf.LogFileName.Split('.')[0]
                + d.Year.ToString().PadLeft(2, '0')
                + d.Month.ToString().PadLeft(2, '0')
                + d.Day.ToString().PadLeft(2, '0')
                + d.Hour.ToString().PadLeft(2, '0')
                + d.Minute.ToString().PadLeft(2, '0')
                + d.Second.ToString().PadLeft(2, '0')
                + "." + conf.LogFileName.Split('.')[1];

            LogBMTP.InicializaLog(conf, NivelLog, fName);

            TR = new TransacManager();
        }
        #endregion        

        #region CONEXIÓN
        public static IPEndPoint ipEndPoint;
        public Socket sender;
        public static string[] UltimaConexionOptima = new string[3];
        public bool esPorPuertoSerie = false;
        public static SerialPort port1;
        public static string inputSerial;
        private static byte bytePuertoSerie = 0;        

        public Error Conectar(Terminal ter, EnumModoConexion modoConexion)
        {
            Error cxnErr = new Error();

            LogBMTP.LogMessage("TERMINAL: " + ter.NumeroTerminal + " TARJETA: " + ter.Tarjeta, lvlLogCxn, TimeStampLog);
            //LogBMTP.LogMessage("TARJETA: " + ter.Tarjeta, CONFIG.LevelLog);
            //LogBMTP.LogMessage("(Level log: " + lvlLogCabeceraTransaccion, TimeStampLog.ToString() + ")", lvlLogCxn, TimeStampLog);          

            #region // DIAL UP
            if (modoConexion == EnumModoConexion.DIALUP)
            {
                TransacManager.ProtoConfig.TIPO_CXN = EnumModoConexion.DIALUP;
                if (TransacManager.ProtoConfig.CONFIG.CxnOnlineHabilitado)
                {
                    LogBMTP.LogMessage("Modo de conexión: TELEFONO", lvlLogCxn, TimeStampLog);
                    LogBMTP.LogMessage("(Timeout DialUp: " + TransacManager.ProtoConfig.CONFIG.CxnTelTimeout + ")", lvlLogCxn, TimeStampLog);                    
                    
                    cxnErr = AbrePuertoTelefono();
                    if (cxnErr.CodError != 0)
                        return cxnErr;

                    cxnErr = Crea_PRN_TelefonoSerie(ter);
                    if (cxnErr.CodError != 0)
                        return cxnErr;                    
                }
                else
                {
                    cxnErr.CodError = (uint)ErrComunicacion.CONEXION;
                    cxnErr.Descripcion = "Modo de conexión DialUp/PorTelefono no habilitado.";
                    cxnErr.Estado = 0;
                    return cxnErr;
                }
            }
            #endregion
            #region // ETHERNET
            else if (modoConexion == EnumModoConexion.ETHERNET)
            {
                TransacManager.ProtoConfig.TIPO_CXN = EnumModoConexion.ETHERNET;
                if (TransacManager.ProtoConfig.CON_CICLO_PRN)
                {
                    LogBMTP.LogMessage("Modo de conexión: ETHERNET", lvlLogCxn, TimeStampLog);
                    cxnErr = Crea_PRN_Socket(ter);
                    if (cxnErr.CodError != 0)
                        return cxnErr;
                }
                else if (!TransacManager.ProtoConfig.CON_CICLO_PRN)
                {
                    LogBMTP.LogMessage("Modo de conexión: ETHERNET y TESTING", lvlLogCxn, TimeStampLog);
                    cxnErr = Crea_PRN_Socket_TEST(ter);
                    if (cxnErr.CodError != 0)
                        return cxnErr;
                }
                else
                {
                    cxnErr.CodError = (uint)ErrComunicacion.CONEXION;
                    cxnErr.Descripcion = "Modo de conexión Online no habilitado.";
                    cxnErr.Estado = 0;
                    return cxnErr;
                }
            }
            #endregion
            #region // RADIO
            else if (modoConexion == EnumModoConexion.RADIO)
            {
                TransacManager.ProtoConfig.TIPO_CXN = EnumModoConexion.RADIO;
                if (TransacManager.ProtoConfig.CONFIG.CxnOnlineHabilitado)
                {
                    LogBMTP.LogMessage("Modo de conexión: RADIO", lvlLogCxn, TimeStampLog);

                    cxnErr = AbrePuertoRadio();
                    if (cxnErr.CodError != 0)
                        return cxnErr;

                    cxnErr = Crea_PRN_Radio(ter);
                    if (cxnErr.CodError != 0)
                        return cxnErr;
                }
                else
                {
                    cxnErr.CodError = (uint)ErrComunicacion.CONEXION;
                    cxnErr.Descripcion = "Modo de conexión Online no habilitado.";
                    cxnErr.Estado = 0;
                    return cxnErr;
                }
            }
            #endregion
            
            return cxnErr;
        }
        public Error Desconectar(bool interno = false)
        {
            try
            {
                Error err = new Error();

                if (TransacManager.ProtoConfig.TIPO_CXN == EnumModoConexion.DIALUP)
                {
                    #region // Secuencia de desconexión DIAL UP
                    if (sender != null)
                    {
                        if (sender.Connected)
                        {
                            byte[] EOT = { };
                            Enviar(EOT, EnumPaquete.EOT, 0);

                            // Disables sends and receives on a Socket.
                            System.Threading.Thread.Sleep(700);

                            sender.Shutdown(SocketShutdown.Both);
                            sender.Close();// (SocketShutdown.Both);
                            if (!sender.Connected)
                                LogBMTP.LogMessage("COMUNICACIÓN FINALIZADA POR TERMINAL", lvlLogCxn, TimeStampLog);
                            else
                                LogBMTP.LogMessage("ATENCIÓN: COMUNICACIÓN NO HA SIDO FINALIZADA", lvlLogCxn, TimeStampLog);
                        }
                        else
                            LogBMTP.LogMessage("COMUNICACIÓN FINALIZADA POR SERVICIO", lvlLogCxn, TimeStampLog);
                    }
                    ICollection<RasConnection> conecciones = RasConnection.GetActiveConnections();

                    if (conecciones.Count > 0 && !interno)
                    {
                        RasConnection conn1 = conecciones.Where(o => o.EntryName == "BMTP Dial up").First();
                        if (conn1.GetConnectionStatus().ConnectionState == RasConnectionState.Connected)
                            conn1.HangUp();
                    }
                    #endregion
                }
                else if (TransacManager.ProtoConfig.TIPO_CXN == EnumModoConexion.ETHERNET)
                {
                    #region // Secuencia de desconexión ETHERNET
                    if (sender != null)
                    {
                        if (sender.Connected)
                        {
                            byte[] EOT = { };
                            Enviar(EOT, EnumPaquete.EOT, 0);

                            // Disables sends and receives on a Socket.
                            System.Threading.Thread.Sleep(700);

                            sender.Shutdown(SocketShutdown.Both);
                            sender.Close();// (SocketShutdown.Both);
                            if (!sender.Connected)
                                LogBMTP.LogMessage("COMUNICACIÓN FINALIZADA POR TERMINAL", lvlLogCxn, TimeStampLog);
                            else
                                LogBMTP.LogMessage("ATENCIÓN: COMUNICACIÓN NO HA SIDO FINALIZADA", lvlLogCxn, TimeStampLog);
                        }
                        else
                        {
                            sender.Close();// (SocketShutdown.Both);

                            LogBMTP.LogMessage("COMUNICACIÓN FINALIZADA POR SERVICIO", lvlLogCxn, TimeStampLog);
                        }
                    }
                    #endregion
                }
                else if (TransacManager.ProtoConfig.TIPO_CXN == EnumModoConexion.RADIO)
                {
                    #region // Secuencia de desconexión RADIO
                    if (port1 != null && port1.IsOpen)
                    {
                        byte[] EOT = { };
                        System.Threading.Thread.Sleep(100);
                        Enviar(EOT, EnumPaquete.EOT, 0);
                        System.Threading.Thread.Sleep(100);

                        CierraPuertoSerie();
                    }
                    #endregion
                }


                err.CodError = 0;
                err.Descripcion = "";
                err.Estado = 1;

                return err;
            }
            catch (Exception ex)
            {
                return this.EmpalmeErrorExcepcion(ex, ErrComunicacion.CONEXIONex, "Ha ocurrido un problema al intentar cerrar la conexión.", 1);
            }

        }       

        //Ethernet
        private Error Crea_PRN_Socket(Terminal ter)
        {
            string nombre1, nombre2, nombre3, port1, port2, port3, tel1, tel2, tel3;

            Conexion cxn = new Conexion();
            Error cxnErr = new Error();
            IPHostEntry ipHost;
            IPAddress ipAddr;

            try
            {
                #region // PROCESO DE CONEXIÓN

                IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());

                for (int i = 0; i < ipHostInfo.AddressList.Length; i++)
                {
                    TransacManager.ProtoConfig.LOCAL_IP = ipHostInfo.AddressList[i]; //----------- Cambia de 1 elemento a 2.
                    if (TransacManager.ProtoConfig.LOCAL_IP.AddressFamily == AddressFamily.InterNetwork) break;
                }

                IPEndPoint localEndPoint = new IPEndPoint(TransacManager.ProtoConfig.LOCAL_IP, TransacManager.ProtoConfig.LOCAL_PORT);

                bool sale = false;
                int fusible = 0;
                while (sale == false) // INTENTA HASTA CONECTAR CON PRN
                {
                    SocketPermission permission = new SocketPermission(PermissionState.Unrestricted);
                    permission.Demand();

                    string res = cxn.Leer_XMLprn(out nombre1, out nombre2, out nombre3, out port1, out  port2, out port3, out tel1, out tel2, out tel3, TransacManager.ProtoConfig.CONFIG);

                    if (res == null && TransacManager.ProtoConfig.CON_CICLO_PRN) // EXISTE PRN
                    {
                        #region // INTENTA CONECTAR CON PRN
                        string[] nom = { nombre1, nombre2, nombre3 };
                        string[] por = { port1, port2, port3 };
                        string[] tel = { tel1, tel2, tel3 };

                        for (int j = 0; j < 3; j++)
                        {
                            try
                            {
                                ipHost = Dns.GetHostEntry(nom[j]);
                                if (ipHost is IPHostEntry)
                                {
                                    ipAddr = null;
                                    for (int i = 0; i < ipHost.AddressList.Length; i++)
                                    {
                                        ipAddr = ipHost.AddressList[i]; //----------- Cambia de 1 elemento a 2.
                                        if (ipAddr.AddressFamily == AddressFamily.InterNetwork) break;
                                    }

                                    LogBMTP.LogMessage("Resulve DNS: " + nom[j] + " - " + ipAddr, lvlLogCxn, TimeStampLog);

                                    ipEndPoint = new IPEndPoint(ipAddr, Convert.ToInt32(por[j]));

                                    sender = new Socket(
                                    ipAddr.AddressFamily,// Specifies the addressing scheme
                                    SocketType.Stream,   // The type of socket 
                                    ProtocolType.Tcp     // Specifies the protocols 
                                    );

                                    sender.NoDelay = false;   // Using the Nagle algorithm

                                    sender.ReceiveTimeout = ProtocoloConfig.TimeoutSocket;
                                    sender.SendTimeout = ProtocoloConfig.TimeoutSocket;

                                    sender.Bind(localEndPoint);

                                    sender.Connect(ipEndPoint);
                                }
                            }
                            catch (Exception e)
                            {
                                ipHost = null;
                                LogBMTP.LogMessage(" Intento con " + nom[j] + "(" + j + ") falló. \n" + e.Message + " " + e.InnerException, lvlLogDebug, TimeStampLog);
                                if (sender != null)
                                    sender.Close();
                            }
                            if (sender != null && sender.Connected == true)
                            {
                                UltimaConexionOptima[0] = nom[j];
                                UltimaConexionOptima[1] = por[j];
                                UltimaConexionOptima[2] = tel[j];
                                sale = true;
                                break;
                            }
                        }
                        #endregion

                        if (fusible >= 2 && sale == false)
                        {
                            sale = true;
                            cxnErr.CodError = (int)ErrComunicacion.CXN_SOCKET;
                            cxnErr.Descripcion = "Error de conexión. No puede establecerse una comunicación. Se superaron los intentos de conexión.";
                            cxnErr.Estado = 0;
                            break;
                        }

                        if (sender == null || sender.Connected != true)
                        {
                            #region // FALLÓ PRN, INTENTA CON VALORES DEFAULT
                            cxn.Borrar_XMLprn(TransacManager.ProtoConfig.CONFIG);

                            try
                            {
                                while (!PuertoDisponible(localEndPoint.Port))
                                {
                                    localEndPoint.Port++;
                                }

                                ipAddr = TransacManager.ProtoConfig.CONFIG.DefaultServer;

                                ipEndPoint = new IPEndPoint(ipAddr, TransacManager.ProtoConfig.CONFIG.Port);

                                sender = new Socket(
                                ipAddr.AddressFamily,// Asigno tipo de address 
                                SocketType.Stream,   // Tipo de socket
                                ProtocolType.Tcp     // Tipo de protocolo
                                );

                                sender.NoDelay = false;   // Using the Nagle algorithm                                                               

                                sender.ReceiveTimeout = ProtocoloConfig.TimeoutSocket;
                                sender.SendTimeout = ProtocoloConfig.TimeoutSocket;

                                sender.Bind(localEndPoint);

                                sender.Connect(ipEndPoint);
                            }
                            catch (Exception e)
                            {
                                ipHost = null;
                                LogBMTP.LogMessage(e.Message + " \n" + e.InnerException, lvlLogDebug, TimeStampLog);
                            }

                            if (sender.Connected != true)
                            {
                                cxnErr.CodError = (int)ErrComunicacion.CXN_SOCKET;
                                cxnErr.Descripcion = "Error de conexión. No pudo establecerse una comunicación con los valores del PRN, ni los por defecto.";
                                cxnErr.Estado = 0;
                                sale = true;
                            }
                            else
                            {
                                UltimaConexionOptima[0] = TransacManager.ProtoConfig.CONFIG.DefaultServer.ToString();
                                UltimaConexionOptima[1] = TransacManager.ProtoConfig.CONFIG.Port.ToString();
                                UltimaConexionOptima[2] = TransacManager.ProtoConfig.CONFIG.Telefono;

                                LogBMTP.LogMessage("CONEXIÓN EXITOSA CON VALORES DEFAULT:\n ", lvlLogCxn, TimeStampLog);
                                LogBMTP.LogMessage("HOST: IP " + ipEndPoint.Address + " Port " + ipEndPoint.Port + "\n", lvlLogCxn, TimeStampLog);
                                LogBMTP.LogMessage("BMTP: IP " + ((IPEndPoint)sender.LocalEndPoint).Address + " Port " + ((IPEndPoint)sender.LocalEndPoint).Port + "\n", lvlLogCxn, TimeStampLog);

                                Comunicacion cm = new Comunicacion(TransacManager.ProtoConfig.BASE_CONFIG, TransacManager.ProtoConfig.CONFIG, true);
                                cm.sender = sender;
                                IList rdo = cm.InteraccionAB(ref ter, true);
                                if (rdo[0] is Error)
                                {
                                    if (((Error)rdo[0]).CodError != 0)
                                    {
                                        cxnErr = (Error)rdo;
                                        sale = true;
                                    }
                                }
                                else
                                {
                                    cxnErr.CodError = (int)ErrComunicacion.CXN_NO_GENERA_PRN;
                                    cxnErr.Descripcion = "Error de conexión. No se pudo obtener el PRN.";
                                    cxnErr.Estado = 0;
                                }
                                fusible++;
                            }
                            #endregion
                        }
                    }
                    else // NO EXISTE PRN
                    {
                        #region // INTENTA CONECTAR CON CONFIG.BIN
                        try
                        {
                            if (!PuertoDisponible(TransacManager.ProtoConfig.CONFIG.Port) || !PuertoDisponible(localEndPoint.Port))
                            {
                                cxnErr.CodError = (int)ErrComunicacion.CXN_SOCKET;
                                cxnErr.Descripcion = "Error de conexión. Puerto default ocupado.";
                                cxnErr.Estado = 0;
                                return cxnErr;
                            }

                            ipAddr = TransacManager.ProtoConfig.CONFIG.DefaultServer;

                            ipEndPoint = new IPEndPoint(ipAddr, TransacManager.ProtoConfig.CONFIG.Port);

                            sender = new Socket(
                            ipAddr.AddressFamily,// Asigno tipo de address de
                            SocketType.Stream,   // The type of socket 
                            ProtocolType.Tcp     // Specifies the protocols 
                            );

                            sender.NoDelay = false;   // Using the Nagle algorithm

                            sender.ReceiveTimeout = ProtocoloConfig.TimeoutSocket;
                            sender.SendTimeout = ProtocoloConfig.TimeoutSocket;

                            sender.Bind(localEndPoint);

                            sender.Connect(ipEndPoint);                            
                        }
                        catch (Exception e)
                        {
                            ipHost = null;
                            LogBMTP.LogMessage(e.Message + " \n" + e.InnerException, lvlLogDebug, TimeStampLog);
                        }
                        if (sender.Connected != true)
                        {
                            cxnErr.CodError = (int)ErrComunicacion.CXN_SOCKET;
                            cxnErr.Descripcion = "Error de conexión. No puede establecerse una comunicación con los valores por defecto.";
                            cxnErr.Estado = 0;
                            sale = true;
                        }
                        else
                        {
                            UltimaConexionOptima[0] = TransacManager.ProtoConfig.CONFIG.DefaultServer.ToString();
                            UltimaConexionOptima[1] = TransacManager.ProtoConfig.CONFIG.Port.ToString();
                            UltimaConexionOptima[2] = TransacManager.ProtoConfig.CONFIG.Telefono;

                            LogBMTP.LogMessage("CONEXIÓN EXITOSA CON VALORES DEFAULT:\n ", lvlLogCxn, TimeStampLog);
                            LogBMTP.LogMessage("HOST: IP " + ipEndPoint.Address + " Port " + ipEndPoint.Port + "\n", lvlLogCxn, TimeStampLog);
                            LogBMTP.LogMessage("BMTP: IP " + ((IPEndPoint)sender.LocalEndPoint).Address + " Port " + ((IPEndPoint)sender.LocalEndPoint).Port + "\n", lvlLogCxn, TimeStampLog);

                            if (TransacManager.ProtoConfig.CON_CICLO_PRN)
                            {
                                Comunicacion cm = new Comunicacion(TransacManager.ProtoConfig.BASE_CONFIG, TransacManager.ProtoConfig.CONFIG, true);
                                cm.sender = sender;

                                IList rdo = cm.InteraccionAB(ref ter, true);
                                if (rdo[0] is Error)
                                {
                                    if (((Error)rdo[0]).CodError != 0)
                                    {
                                        cxnErr = (Error)rdo[0];
                                        sale = true;
                                    }
                                }
                                else
                                {
                                    cxnErr.CodError = (int)ErrComunicacion.CXN_NO_GENERA_PRN;
                                    cxnErr.Descripcion = "Error de conexión. No se pudo obtener el PRN.";
                                    cxnErr.Estado = 0;
                                }
                            }
                            fusible++;
                        }
                        #endregion

                        if (fusible >= 2 && sale == false)
                        {
                            sale = true;
                            cxnErr.CodError = (int)ErrComunicacion.CXN_SOCKETex;
                            cxnErr.Descripcion = "Error de conexión. No puede establecerse una comunicación. Se superaron los intentos de conexión.";
                            cxnErr.Estado = 0;
                            break;
                        }
                    }
                }
                #endregion

                if (cxnErr.CodError != 0)
                    LogBMTP.LogMessage("Error de conexión: " + cxnErr.CodError + "\n" + " Descripción: " + cxnErr.Descripcion + "\n", lvlLogCxn, TimeStampLog);
                else
                {
                    LogBMTP.LogMessage("CONEXIÓN EXITOSA:\n ", lvlLogCxn, TimeStampLog);
                    LogBMTP.LogMessage("HOST: IP " + ipEndPoint.Address + " Port " + ipEndPoint.Port + "\n", lvlLogCxn, TimeStampLog);
                    LogBMTP.LogMessage("BMTP: IP " + ((IPEndPoint)sender.LocalEndPoint).Address + " Port " + ((IPEndPoint)sender.LocalEndPoint).Port + "\n", lvlLogCxn, TimeStampLog);
                }

                return cxnErr;
            }
            catch (Exception ex)
            {
                cxnErr.CodError = (int)ErrComunicacion.CONEXIONex;
                cxnErr.Descripcion = "Error de conexión. No puede establecerse una comunicación.";
                cxnErr.Estado = 0;

                LogBMTP.LogMessage("Excepción: " + cxnErr.CodError + " " + cxnErr.Descripcion, lvlLogExcepciones, TimeStampLog);
                LogBMTP.LogMessage("Excepción: " + ex.ToString(), lvlLogExcepciones, TimeStampLog);

                return cxnErr;
            }

        }
        private Error Crea_PRN_Socket_TEST(Terminal ter)
        {
            //string nombre1, nombre2, nombre3, port1, port2, port3, tel1, tel2, tel3;

            Conexion cxn = new Conexion();
            Error cxnErr = new Error();
            // IPHostEntry ipHost;
            IPAddress ipAddr;
            IPAddress ipAddrLocal;

            try
            {

                IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                ipAddrLocal = null;

                for (int i = 0; i < ipHostInfo.AddressList.Length; i++)
                {
                    ipAddrLocal = ipHostInfo.AddressList[i]; //----------- Cambia de 1 elemento a 2.
                    if (ipAddrLocal.AddressFamily == AddressFamily.InterNetwork) break;
                }

                IPEndPoint localEndPoint = new IPEndPoint(ipAddrLocal, TransacManager.ProtoConfig.LOCAL_PORT);

                SocketPermission permission = new SocketPermission(PermissionState.Unrestricted
                    //NetworkAccess.Connect,    // Connection permission
                    //TransportType.Tcp,        // Defines transport types
                    //"",                       // Gets the IP addresses
                    //SocketPermission.AllPorts // All ports
                    );
                //SocketPermissionAttribute per = new SocketPermissionAttribute(SecurityAction.
                permission.Demand();

                #region // INTENTA CONECTAR CON CONFIG.BIN
                try
                {
                    ipAddr = TransacManager.ProtoConfig.CONFIG.DefaultServer;

                    ipEndPoint = new IPEndPoint(ipAddr, TransacManager.ProtoConfig.CONFIG.Port);

                    sender = new Socket(
                    ipAddr.AddressFamily,// Specifies the addressing scheme
                    SocketType.Stream,   // The type of socket 
                    ProtocolType.Tcp     // Specifies the protocols 
                    );

                    sender.NoDelay = false;   // Using the Nagle algorithm

                    sender.Bind(localEndPoint);

                    sender.Connect(ipEndPoint);

                    //sender.Send(DataConverter.Pack("^$8", "Holaaaaaaaaa <EOF>"));
                }
                catch (Exception e)
                {
                    //   ipHost = null;
                    LogBMTP.LogMessage(e.Message + " \n" + e.InnerException, lvlLogDebug, TimeStampLog);
                }
                if (sender.Connected != true)
                {
                    cxnErr.CodError = (int)ErrComunicacion.CXN_SOCKET;
                    cxnErr.Descripcion = "Error de conexión. No puede establecerse una comunicación con los valores por defecto.";
                    cxnErr.Estado = 0;
                }
                else
                {
                    UltimaConexionOptima[0] = TransacManager.ProtoConfig.CONFIG.DefaultServer.ToString();
                    UltimaConexionOptima[1] = TransacManager.ProtoConfig.CONFIG.Port.ToString();
                    UltimaConexionOptima[2] = TransacManager.ProtoConfig.CONFIG.Telefono;

                    LogBMTP.LogMessage("CONEXIÓN EXITOSA CON VALORES DEFAULT:\n ", lvlLogCxn, TimeStampLog);
                    LogBMTP.LogMessage("HOST: IP " + ipEndPoint.Address + " Port " + ipEndPoint.Port + "\n", lvlLogCxn, TimeStampLog);
                    LogBMTP.LogMessage("BMTP: IP " + ((IPEndPoint)sender.LocalEndPoint).Address + " Port " + ((IPEndPoint)sender.LocalEndPoint).Port + "\n", lvlLogCxn, TimeStampLog);
                }
                #endregion

                if (cxnErr.CodError != 0)
                    LogBMTP.LogMessage("Error de conexión: " + cxnErr.CodError + "\n" + " Descripción: " + cxnErr.Descripcion + "\n", lvlLogError, TimeStampLog);
                else
                {
                    LogBMTP.LogMessage("CONEXIÓN EXITOSA:\n ", lvlLogCxn, TimeStampLog);
                    LogBMTP.LogMessage("HOST: IP " + ipEndPoint.Address + " Port " + ipEndPoint.Port + "\n", lvlLogCxn, TimeStampLog);
                    LogBMTP.LogMessage("BMTP: IP " + ((IPEndPoint)sender.LocalEndPoint).Address + " Port " + ((IPEndPoint)sender.LocalEndPoint).Port + "\n", lvlLogCxn, TimeStampLog);
                }

                return cxnErr;
            }
            catch (Exception ex)
            {
                cxnErr.CodError = (int)ErrComunicacion.CONEXIONex;
                cxnErr.Descripcion = "Error de conexión. No puede establecerse una comunicación.";
                cxnErr.Estado = 0;

                LogBMTP.LogMessage("Excepción: " + cxnErr.CodError + " " + cxnErr.Descripcion, lvlLogExcepciones, TimeStampLog);
                LogBMTP.LogMessage("Excepción: " + ex.ToString(), lvlLogExcepciones, TimeStampLog);

                return cxnErr;
            }

        }       
       
        //Dialup
        private Error CreaVPNxTel()
        {
            Error cxnErr = new Error();

            string telDefault = TransacManager.ProtoConfig.CONFIG.Telefono;// "08006665807";
            string prefijo = TransacManager.ProtoConfig.CONFIG.CxnTelPrefijo;// "11000";
            string separador = "w";
            if (String.IsNullOrEmpty(prefijo))
            {
                prefijo = "";
                separador = "";
            }
            string modemDefault = TransacManager.ProtoConfig.CONFIG.CxnTelNombreModem;// "Conexant USB CX93010 ACF Modem";
            string user = TransacManager.ProtoConfig.CONFIG.CxnTelUser;// "bmtp";
            string pass = TransacManager.ProtoConfig.CONFIG.CxnTelPass;// "bmtp";
            uint timeout = TransacManager.ProtoConfig.CONFIG.CxnTelTimeout;

            string nombre1, nombre2, nombre3, port1, port2, port3, tel1, tel2, tel3;

            Conexion cxn = new Conexion();
            
                string rdo = cxn.Leer_XMLprn(out nombre1, out nombre2, out nombre3, out port1, out  port2, out port3, out tel1, out tel2, out tel3, TransacManager.ProtoConfig.CONFIG);
                string[] tel = new string[4];

                if (rdo == null)
                {
                    tel[0] = tel1;
                    tel[1] = tel2;
                    tel[2] = tel3;
                    tel[3] = telDefault;
                }
                else
                    tel[0] = telDefault;


                string path = RasPhoneBook.GetPhoneBookPath(RasPhoneBookType.User);

                using (RasPhoneBook pbk = new RasPhoneBook())
                {
                    pbk.Open(path);

                    // Find the device that will be used to dial the connection.
                    RasDevice device = RasDevice.GetDevices().Where(u => u.Name == modemDefault && u.DeviceType == RasDeviceType.Modem).First();

                    RasEntry entry;

                    ICollection<RasConnection> conecciones = RasConnection.GetActiveConnections();


                    if (conecciones.Count > 0)
                    {
                        RasConnection conn1 = conecciones.Where(o => o.EntryName == "BMTP Dial up").First();
                        conn1.HangUp();
                    }

                    pbk.Entries.Clear();
                    
                    try
                    {
                        entry = RasEntry.CreateDialUpEntry("BMTP Dial up", prefijo + separador + tel[0], device);

                        if (rdo == null)
                        {
                            entry.AlternatePhoneNumbers.Add(prefijo + separador + tel[1]);
                            entry.AlternatePhoneNumbers.Add(prefijo + separador + tel[2]);
                            entry.AlternatePhoneNumbers.Add(prefijo + separador + telDefault);
                            LogBMTP.LogMessage("Se leyó PRN. Telefonos alternativos cargados.", lvlLogCxn, TimeStampLog);
                        }
                        else
                        {
                            LogBMTP.LogMessage("No hay PRN. Intentará conectar con número telefónico por defecto.", lvlLogCxn, TimeStampLog);
                        }

                        //entry.Options.ModemLights = true;
                        //entry.Options.ShowDialingProgress = true;
                        //entry.Options.TerminalBeforeDial = true;
                        //entry.Options.TerminalAfterDial = true;

                        pbk.Entries.Add(entry);

                        using (RasDialer dialer = new RasDialer())
                        {
                            dialer.Dispose();
                            dialer.EntryName = "BMTP Dial up";
                            dialer.PhoneBookPath = path;
                            dialer.Credentials = new NetworkCredential(user, pass);

                            dialer.Timeout = (int)timeout;
                            dialer.Options.DisableReconnect = false;
                            dialer.Options.SetModemSpeaker = true;

                            dialer.Dial();                            
                        }
                    }
                    catch (Exception e)
                    {
                        pbk.Entries.Clear();
                        LogBMTP.LogMessage(e.ToString(), lvlLogExcepciones, TimeStampLog);
                        
                        cxnErr.CodError = (int)ErrComunicacion.CXN_TELEFONOex;
                        cxnErr.Descripcion = "Error de conexión. No puede establecerse una comunicación por telefono.";
                        cxnErr.Estado = 0;
                        LogBMTP.LogMessage("Excepción: " + cxnErr.CodError + " " + cxnErr.Descripcion, lvlLogExcepciones, TimeStampLog);
                        LogBMTP.LogMessage("Excepción: " + e.ToString(), lvlLogExcepciones, TimeStampLog);
                    }                    
                }

                return cxnErr;
            

        }
        public Error ConectarDialUp(Terminal ter)
        {
            Error cxnErr = new Error();

            //pe = new Logger(TransacManager.ProtoConfig.CONFIG);
            LogBMTP.LogMessage("${level} TERMINAL: " + ter.NumeroTerminal + " TARJETA: " + ter.Tarjeta, lvlLogCxn, TimeStampLog);
            //LogBMTP.LogMessage("(Level log: " + lvlLogCabeceraTransaccion, TimeStampLog.ToString() + ")", lvlLogCxn, TimeStampLog);

            #region // Dial Up vía linea telefónica
            if (TransacManager.ProtoConfig.CONFIG.CxnDialUpHabilitado)
            {
                TransacManager.ProtoConfig.TIPO_CXN = EnumModoConexion.DIALUP_TEL;
                cxnErr = CreaVPNxTel();
                if (cxnErr.CodError != 0)
                    return cxnErr;
                else
                {
                    cxnErr = Crea_PRN_Socket(ter);
                    if (cxnErr.CodError != 0)
                        return cxnErr;
                }
            }
            #endregion

            return cxnErr;
        }

        //Dialup Serie
        private Error AbrePuertoTelefono()
        {
            Error portErr = new Error();
            esPorPuertoSerie = true;
            try
            {
                //pe = new Logger(TransacManager.ProtoConfig.CONFIG);

                string[] portsCom = SerialPort.GetPortNames();
                string portComDisponibles = "Puertos disponibles: ";
                foreach (string st in portsCom)
                {
                    portComDisponibles = portComDisponibles + st + " ";
                }

                LogBMTP.LogMessage(portComDisponibles, lvlLogDebug, TimeStampLog);
                LogBMTP.LogMessage("Puerto para TELEFONO: " + "COM11", lvlLogDebug, TimeStampLog); //TODO puerto telefono

                port1 = new SerialPort("COM11", 115200, (Parity)TransacManager.ProtoConfig.CONFIG.ParityRadio, TransacManager.ProtoConfig.CONFIG.DataBitsRadio, (StopBits)TransacManager.ProtoConfig.CONFIG.StopBitsRadio);
                port1.ReadTimeout = TransacManager.ProtoConfig.CONFIG.ReadTimeOutRadio;
                port1.WriteTimeout = TransacManager.ProtoConfig.CONFIG.WriteTimeOutRadio;
                port1.ReadBufferSize = TransacManager.ProtoConfig.CONFIG.ReadBufferSizeRadio;
                port1.WriteBufferSize = TransacManager.ProtoConfig.CONFIG.WriteBufferSizeRadio;
                port1.ErrorReceived += port1_ErrorReceived;
                port1.NewLine = "\r";
                //port1.Handshake = Handshake.XOnXOff;//(Handshake)CONFIG.HandshakeRadio; //Handshake.RequestToSend;             

                port1.Open();
                System.Threading.Thread.Sleep(120);
                CambiaSeniales(port1);

                return portErr;
            }
            catch (Exception ex)
            {
                portErr.CodError = (int)ErrComunicacion.CONEX_SERIALex;
                portErr.Descripcion = "Error de conexión. No puedo establecerse la comunicación.";
                portErr.Estado = 0;
                LogBMTP.LogMessage("Excepción: " + portErr.CodError + " " + portErr.Descripcion, lvlLogExcepciones, TimeStampLog);
                LogBMTP.LogMessage("Excepción: " + ex.ToString(), lvlLogExcepciones, TimeStampLog);

                return portErr;
            }
        }
        private bool LogInTelefono(string routerUser, string tel0800)
        {
            int inp = 0;
            System.Threading.Thread.Sleep(1000);
            inputSerial = port1.ReadExisting();

            Error errTelefonoSerie = new Error();
            LogBMTP.LogMessage("InputSerial Login -1: ", lvlLogDebug, TimeStampLog);
            LogBMTP.LogMessage(inputSerial, lvlLogDebug, TimeStampLog);
            while (!inputSerial.Contains("name") && inp < 15)
            {
                inp++;
                //port1.DtrEnable = false;
                port1.RtsEnable = true;

                //port1.Write(new byte[]{0x0d}, 0,1);

                port1.Write("AT DT " + "11000" + tel0800 + "\r");
                System.Threading.Thread.Sleep(39000); //EsperaPuertoSerie(port1, 9 + tel0800.Length);//200//400

                LogBMTP.LogMessage("CDHolding: " + port1.CDHolding.ToString() + " CtsHolding: " + port1.CtsHolding.ToString(), lvlLogDebug, TimeStampLog);
                if (port1.CDHolding && port1.CtsHolding)
                {
                    inputSerial = port1.ReadExisting();
                    port1.RtsEnable = false;
                    //port1.DtrEnable = true;


                    LogBMTP.LogMessage("InputSerial Login " + inp + ": ", lvlLogDebug, TimeStampLog);
                    LogBMTP.LogMessage(inputSerial, lvlLogDebug, TimeStampLog);

                    if (inputSerial.Length > 30)
                        inputSerial = inputSerial.Substring(inputSerial.Length - 30);

                    errTelefonoSerie = LeeErrTelefonoSerie(inputSerial);
                    if (errTelefonoSerie.CodError != 0 && errTelefonoSerie.Descripcion != "SinRespuesta")
                    {
                        LogBMTP.LogMessage("Error Telefono: intento de conexión " + inp + ": " + errTelefonoSerie.CodError + " - " + errTelefonoSerie.Descripcion, lvlLogError, TimeStampLog);
                        return false;
                    }
                }
            }

            inp = 0;

            System.Threading.Thread.Sleep(500);
            port1.WriteLine("ivicom"); //routerUser

            EsperaPuertoSerie(port1, "ivicom".Length);

            while (inp < 3)//10
            {
                inp++;
                //if (port1.DsrHolding)
                //{
                EsperaPuertoSerie(port1, 350);
                inputSerial = port1.ReadExisting();

                if (inputSerial.Length > 30)
                    inputSerial = inputSerial.Substring(inputSerial.Length - 30);

                if (inputSerial.Contains("Connected"))
                {
                    LogBMTP.LogMessage("CONEXIÓN EXITOSA.", lvlLogCxn, TimeStampLog);
                    port1.DiscardInBuffer();
                    return true;
                }
                else if (inputSerial.Contains("BUSY") || inputSerial.Contains("NO CARRIER"))
                {
                    System.Threading.Thread.Sleep(500);
                }
                else
                {
                    errTelefonoSerie = LeeErrTelefonoSerie(inputSerial);
                    if (errTelefonoSerie.CodError != 0 && errTelefonoSerie.Descripcion != "SinRespuesta")
                    {
                        LogBMTP.LogMessage("Error Telefono: " + errTelefonoSerie.CodError + " - " + errTelefonoSerie.Descripcion, lvlLogError, TimeStampLog);
                        return false;
                    }
                }
            }

            port1.RtsEnable = false;
            return false;
        }

        //Radio Serie
        private Error AbrePuertoRadio()
        {
            Error portErr = new Error();
            esPorPuertoSerie = true;
            try
            {
                //pe = new Logger(TransacManager.ProtoConfig.CONFIG);                

                string[] portsCom = SerialPort.GetPortNames();
                string portComDisponibles = "Puertos disponibles: ";
                foreach (string st in portsCom)
                {
                    portComDisponibles = portComDisponibles + st + " ";
                }

                LogBMTP.LogMessage(portComDisponibles, lvlLogDebug, TimeStampLog);
                LogBMTP.LogMessage("Puerto para RADIO: " + TransacManager.ProtoConfig.CONFIG.NombrePuertoRadio, lvlLogDebug, TimeStampLog);

                port1 = new SerialPort(TransacManager.ProtoConfig.CONFIG.NombrePuertoRadio, TransacManager.ProtoConfig.CONFIG.BaudRateRadio, (Parity)TransacManager.ProtoConfig.CONFIG.ParityRadio, TransacManager.ProtoConfig.CONFIG.DataBitsRadio, (StopBits)TransacManager.ProtoConfig.CONFIG.StopBitsRadio);
                port1.ReadTimeout = TransacManager.ProtoConfig.CONFIG.ReadTimeOutRadio;
                port1.WriteTimeout = TransacManager.ProtoConfig.CONFIG.WriteTimeOutRadio;
                port1.ReadBufferSize = TransacManager.ProtoConfig.CONFIG.ReadBufferSizeRadio;
                port1.WriteBufferSize = TransacManager.ProtoConfig.CONFIG.WriteBufferSizeRadio;
                port1.ErrorReceived += port1_ErrorReceived;
                //port1.Handshake = Handshake.XOnXOff;//(Handshake)CONFIG.HandshakeRadio; //Handshake.RequestToSend;             
                
                port1.Open();
                System.Threading.Thread.Sleep(120);
                CambiaSeniales(port1);

                return portErr;
            }           
            catch (Exception ex)
            {
                portErr.CodError = (int)ErrComunicacion.CONEX_SERIALex;
                portErr.Descripcion = "Error de conexión. No puedo establecerse la comunicación.";
                portErr.Estado = 0;
                LogBMTP.LogMessage("Excepción: " + portErr.CodError + " " + portErr.Descripcion, lvlLogExcepciones, TimeStampLog);
                LogBMTP.LogMessage("Excepción: " + ex.ToString(), lvlLogExcepciones, TimeStampLog);

                return portErr;
            }
        }
        private bool LogInRadio(string routerUser)
        {
            int inp = 0;
            System.Threading.Thread.Sleep(TransacManager.ProtoConfig.CONFIG.ReadTimeOutRadio);
            inputSerial = port1.ReadExisting();

            string errRadio = "";
            LogBMTP.LogMessage("InputSerial Login -1: ", lvlLogDebug, TimeStampLog);
            LogBMTP.LogMessage(inputSerial, lvlLogDebug, TimeStampLog);
            while (!inputSerial.Contains("name") && inp < 15)
            {
                inp++;
                port1.RtsEnable = false;
                //port1.Write(new byte[]{0x0d}, 0,1);
                LogBMTP.LogMessage("CDHolding: " + port1.CDHolding.ToString() + " CtsHolding: " + port1.CtsHolding.ToString(), lvlLogDebug, TimeStampLog);
                if (port1.CDHolding && port1.CtsHolding)
                {
                    //port1.RtsEnable = true;
                    EsperaPuertoSerie(port1, 200);//400
                    port1.RtsEnable = true;
                    inputSerial = port1.ReadExisting();

                    LogBMTP.LogMessage("InputSerial Login " + inp + ": ", lvlLogDebug, TimeStampLog);
                    LogBMTP.LogMessage(inputSerial, lvlLogDebug, TimeStampLog);

                    if (inputSerial.Length > 30)
                        inputSerial = inputSerial.Substring(inputSerial.Length - 30);

                    errRadio = LeeErrRadio(inputSerial);
                    if (errRadio != "SinError" && errRadio != "SinRespuesta")
                    {
                        LogBMTP.LogMessage("Error radio " + inp + ": " + errRadio.ToString(), lvlLogDebug, TimeStampLog);
                        return false;
                    }
                }
            }

            inp = 0;

            System.Threading.Thread.Sleep(500);
            port1.WriteLine(routerUser);

            EsperaPuertoSerie(port1, routerUser.Length);

            while (inp < 3)//10
            {
                inp++;
                //if (port1.DsrHolding)
                //{
                EsperaPuertoSerie(port1, 350);
                inputSerial = port1.ReadExisting();

                if (inputSerial.Contains("Open"))
                {
                    LogBMTP.LogMessage("CONEXIÓN EXITOSA.", lvlLogCxn, TimeStampLog);
                    port1.DiscardInBuffer();
                    return true;
                }
                else if (inputSerial.Contains("%"))
                {
                    if (inputSerial.Length > 30)
                        inputSerial = inputSerial.Substring(inputSerial.Length - 30);

                    errRadio = LeeErrRadio(inputSerial);
                    if (errRadio != "SinError" && errRadio != "SinRespuesta")
                        return false;
                }
                else
                    System.Threading.Thread.Sleep(500);//Estaba debajo de inp++
                //}
            }

            port1.RtsEnable = false;
            return false;
        }
        private Error Crea_PRN_Radio(Terminal ter)
        {
            string nombre1, nombre2, nombre3, port1, port2, port3, tel1, tel2, tel3;

            Conexion cxn = new Conexion();
            Error cxnErr = new Error();

            try
            {
                #region // PROCESO DE CONEXIÓN
                bool sale = false;
                int fusible = 0;
                while (sale == false) // INTENTA HASTA CONECTAR CON PRN
                {
                    string res = cxn.Leer_XMLprn(out nombre1, out nombre2, out nombre3, out port1, out  port2, out port3, out tel1, out tel2, out tel3, TransacManager.ProtoConfig.CONFIG);

                    if (res == null) // EXISTE PRN
                    {
                        #region // INTENTA CONECTAR CON PRN
                        string[] nom = { nombre1, nombre2, nombre3 };
                        string[] por = { port1, port2, port3 };
                        string[] tel = { tel1, tel2, tel3 };

                        for (int j = 0; j < 3; j++)
                        {
                            try
                            {
                                if (nom[j] != null)
                                {
                                    sale = LogInRadio(nom[j]);
                                }
                            }
                            catch (Exception e)
                            {
                                LogBMTP.LogMessage("RADIO: Excepción al intentar con PRN:Nombre " + j.ToString(), lvlLogCxn, TimeStampLog);
                                LogBMTP.LogMessage(e.Message + " \n" + e.InnerException, lvlLogDebug, TimeStampLog);
                            }
                            if (sale == true)
                            {
                                UltimaConexionOptima[0] = nom[j];
                                UltimaConexionOptima[1] = por[j];
                                UltimaConexionOptima[2] = tel[j];
                                break;
                            }
                            else
                                LogBMTP.LogMessage("RADIO: Intento con PRN: Nombre " + nom[j] + " falló.", lvlLogCxn, TimeStampLog);
                        }
                        #endregion

                        if (fusible >= 2 && sale == false)
                        {
                            sale = true;
                            cxnErr.CodError = (int)ErrComunicacion.CONEX_SERIAL;
                            cxnErr.Descripcion = "Error de conexión. No puede establecerse una comunicación. Se superaron los intentos de conexión.";
                            cxnErr.Estado = 0;
                            break;
                        }

                        if (sale == false)
                        {
                            #region // FALLÓ PRN, INTENTA CON CONFIG.BIN
                            cxn.Borrar_XMLprn(TransacManager.ProtoConfig.CONFIG);

                            try
                            {
                                sale = LogInRadio(TransacManager.ProtoConfig.CONFIG.Host);
                            }
                            catch (Exception e)
                            {
                                LogBMTP.LogMessage("RADIO: Excepción al intentar con valores por defecto.", lvlLogExcepciones, TimeStampLog);
                                LogBMTP.LogMessage(e.Message + " \n" + e.InnerException, lvlLogDebug, TimeStampLog);
                            }

                            if (sale == false)
                            {
                                LogBMTP.LogMessage("RADIO: Intento valores por defecto: Nombre " + TransacManager.ProtoConfig.CONFIG.Host + " falló.", lvlLogCxn, TimeStampLog);
                                cxnErr.CodError = (int)ErrComunicacion.CONEX_SERIAL;
                                cxnErr.Descripcion = "Error de conexión. No pudo establecerse una comunicación con los valores del PRN, ni los por defecto.";
                                cxnErr.Estado = 0;

                                CierraPuertoSerie();

                                sale = true;
                            }
                            else
                            {
                                UltimaConexionOptima[0] = TransacManager.ProtoConfig.CONFIG.DefaultServer.ToString();
                                UltimaConexionOptima[1] = TransacManager.ProtoConfig.CONFIG.Port.ToString();
                                UltimaConexionOptima[2] = TransacManager.ProtoConfig.CONFIG.Telefono;

                                Comunicacion cm = new Comunicacion(TransacManager.ProtoConfig.BASE_CONFIG, TransacManager.ProtoConfig.CONFIG);
                                IList rdo = cm.InteraccionAB(ref ter, true);

                                if (rdo[0] is Error)
                                {
                                    if (((Error)rdo[0]).CodError != 0)
                                    {
                                        cxnErr = (Error)rdo;
                                        sale = true;
                                    }
                                    else
                                    {
                                        CierraPuertoSerie();
                                        cxnErr = AbrePuertoRadio();
                                        if (cxnErr.CodError == 0)
                                            sale = false;
                                        else
                                        {
                                            CierraPuertoSerie();
                                            return cxnErr;
                                        }
                                    }
                                }
                                else
                                {
                                    cxnErr.CodError = (int)ErrComunicacion.CXN_NO_GENERA_PRN;
                                    cxnErr.Descripcion = "Error de conexión. No se pudo obtener el PRN.";
                                    cxnErr.Estado = 0;
                                    CierraPuertoSerie();
                                    return cxnErr;
                                }
                                fusible++;
                            }
                            #endregion
                        }
                    }
                    else // NO EXISTE PRN
                    {
                        #region // INTENTA CONECTAR CON CONFIG.BIN
                        try
                        {
                            sale = LogInRadio(TransacManager.ProtoConfig.CONFIG.Host);
                        }
                        catch (Exception e)
                        {
                            LogBMTP.LogMessage("RADIO: Excepción al intentar con valores por defecto.", lvlLogExcepciones, TimeStampLog);
                            LogBMTP.LogMessage(e.Message + " \n" + e.InnerException, lvlLogDebug, TimeStampLog);
                        }

                        if (sale == false)
                        {
                            LogBMTP.LogMessage("RADIO: Intento valores por defecto: Nombre " + TransacManager.ProtoConfig.CONFIG.Host + " falló.", lvlLogCxn, TimeStampLog);
                            cxnErr.CodError = (int)ErrComunicacion.CONEX_SERIAL;
                            cxnErr.Descripcion = "Error de conexión. No pudo establecerse una comunicación con los valores del PRN, ni los por defecto.";
                            cxnErr.Estado = 0;
                            CierraPuertoSerie();
                            sale = true;
                        }
                        else
                        {
                            UltimaConexionOptima[0] = TransacManager.ProtoConfig.CONFIG.DefaultServer.ToString();
                            UltimaConexionOptima[1] = TransacManager.ProtoConfig.CONFIG.Port.ToString();
                            UltimaConexionOptima[2] = TransacManager.ProtoConfig.CONFIG.Telefono;

                            Comunicacion cm = new Comunicacion(TransacManager.ProtoConfig.BASE_CONFIG, TransacManager.ProtoConfig.CONFIG);
                            IList rdo = cm.InteraccionAB(ref ter, true);
                            if (rdo[0] is Error)
                            {
                                if (((Error)rdo[0]).CodError != 0)
                                {
                                    cxnErr = (Error)rdo;
                                    sale = true;
                                }
                                else
                                {
                                    CierraPuertoSerie();
                                    cxnErr = AbrePuertoRadio();
                                    if (cxnErr.CodError == 0)
                                        sale = false;
                                    else
                                    {
                                        CierraPuertoSerie();
                                        return cxnErr;
                                    }
                                }
                            }
                            else
                            {
                                cxnErr.CodError = (int)ErrComunicacion.CXN_NO_GENERA_PRN;
                                cxnErr.Descripcion = "Error de conexión. No se pudo obtener el PRN.";
                                cxnErr.Estado = 0;
                                CierraPuertoSerie();
                                return cxnErr;
                            }
                            fusible++;
                        }
                        #endregion

                        if (fusible >= 2 && sale == false)
                        {
                            sale = true;
                            cxnErr.CodError = (int)ErrComunicacion.CONEXION;
                            cxnErr.Descripcion = "Error de conexión. No puede establecerse una comunicación. Se superaron los intentos de conexión.";
                            cxnErr.Estado = 0;

                            CierraPuertoSerie();
                            break;
                        }
                    }
                }
                #endregion

                if (cxnErr.CodError != 0)
                    LogBMTP.LogMessage("Error de conexión: " + cxnErr.CodError + "\n" + " Descripción: " + cxnErr.Descripcion + "\n", lvlLogError, TimeStampLog);

                return cxnErr;
            }
            catch (Exception ex)
            {
                cxnErr.CodError = (int)ErrComunicacion.CONEX_SERIALex;
                cxnErr.Descripcion = "Error de conexión. No puede establecerse una comunicación.";
                cxnErr.Estado = 0;

                LogBMTP.LogMessage("Excepción: " + cxnErr.CodError + " " + cxnErr.Descripcion, lvlLogExcepciones, TimeStampLog);
                LogBMTP.LogMessage("Excepción: " + ex.ToString(), lvlLogExcepciones, TimeStampLog);

                return cxnErr;
            }

        }
        private Error Crea_PRN_TelefonoSerie(Terminal ter)
        {
            string nombre1, nombre2, nombre3, port1, port2, port3, tel1, tel2, tel3;

            Conexion cxn = new Conexion();
            Error cxnErr = new Error();

            try
            {
                #region // PROCESO DE CONEXIÓN
                bool sale = false;
                int fusible = 0;
                while (sale == false) // INTENTA HASTA CONECTAR CON PRN
                {
                    string res = cxn.Leer_XMLprn(out nombre1, out nombre2, out nombre3, out port1, out  port2, out port3, out tel1, out tel2, out tel3, TransacManager.ProtoConfig.CONFIG);

                    if (res == null) // EXISTE PRN
                    {
                        #region // INTENTA CONECTAR CON PRN
                        string[] nom = { nombre1, nombre2, nombre3 };
                        string[] por = { port1, port2, port3 };
                        string[] tel = { tel1, tel2, tel3 };

                        for (int j = 0; j < 3; j++)
                        {
                            try
                            {
                                if (nom[j] != null)
                                {
                                    sale = LogInTelefono(nom[j], tel[j]);
                                }
                            }
                            catch (Exception e)
                            {
                                LogBMTP.LogMessage("TELEFONO: Excepción al intentar con PRN:Nombre " + j.ToString(), lvlLogExcepciones, TimeStampLog);
                                LogBMTP.LogMessage(e.Message + " \n" + e.InnerException, lvlLogDebug, TimeStampLog);
                            }
                            if (sale == true)
                            {
                                UltimaConexionOptima[0] = nom[j];
                                UltimaConexionOptima[1] = por[j];
                                UltimaConexionOptima[2] = tel[j];
                                break;
                            }
                            else
                                LogBMTP.LogMessage("TELEFONO: Intento con PRN: Nombre " + nom[j] + " falló.", lvlLogCxn, TimeStampLog);
                        }
                        #endregion

                        if (fusible >= 2 && sale == false)
                        {
                            sale = true;
                            cxnErr.CodError = (int)ErrComunicacion.CONEX_SERIAL;
                            cxnErr.Descripcion = "Error de conexión. No puede establecerse una comunicación. Se superaron los intentos de conexión.";
                            cxnErr.Estado = 0;
                            break;
                        }

                        if (sale == false)
                        {
                            #region // FALLÓ PRN, INTENTA CON CONFIG.BIN
                            cxn.Borrar_XMLprn(TransacManager.ProtoConfig.CONFIG);

                            try
                            {
                                sale = LogInTelefono(TransacManager.ProtoConfig.CONFIG.Host, TransacManager.ProtoConfig.CONFIG.Telefono);
                            }
                            catch (Exception e)
                            {
                                LogBMTP.LogMessage("TELEFONO: Excepción al intentar con valores por defecto.", lvlLogExcepciones, TimeStampLog);
                                LogBMTP.LogMessage(e.Message + " \n" + e.InnerException, lvlLogDebug, TimeStampLog);
                            }

                            if (sale == false)
                            {
                                LogBMTP.LogMessage("TELEFONO: Intento valores por defecto: Nombre " + TransacManager.ProtoConfig.CONFIG.Host + " falló.", lvlLogCxn, TimeStampLog);
                                cxnErr.CodError = (int)ErrComunicacion.CONEX_SERIAL;
                                cxnErr.Descripcion = "Error de conexión. No pudo establecerse una comunicación con los valores del PRN, ni los por defecto.";
                                cxnErr.Estado = 0;

                                CierraPuertoSerie();

                                sale = true;
                            }
                            else
                            {
                                UltimaConexionOptima[0] = TransacManager.ProtoConfig.CONFIG.DefaultServer.ToString();
                                UltimaConexionOptima[1] = TransacManager.ProtoConfig.CONFIG.Port.ToString();
                                UltimaConexionOptima[2] = TransacManager.ProtoConfig.CONFIG.Telefono;

                                Comunicacion cm = new Comunicacion(TransacManager.ProtoConfig.BASE_CONFIG, TransacManager.ProtoConfig.CONFIG);
                                IList rdo = cm.InteraccionAB(ref ter, true);

                                if (rdo[0] is Error)
                                {
                                    if (((Error)rdo[0]).CodError != 0)
                                    {
                                        cxnErr = (Error)rdo;
                                        sale = true;
                                    }
                                    else
                                    {
                                        CierraPuertoSerie();
                                        cxnErr = AbrePuertoTelefono();
                                        if (cxnErr.CodError == 0)
                                            sale = false;
                                        else
                                        {
                                            CierraPuertoSerie();
                                            return cxnErr;
                                        }
                                    }
                                }
                                else
                                {
                                    cxnErr.CodError = (int)ErrComunicacion.CXN_NO_GENERA_PRN;
                                    cxnErr.Descripcion = "Error de conexión. No se pudo obtener el PRN.";
                                    cxnErr.Estado = 0;
                                    CierraPuertoSerie();
                                    return cxnErr;
                                }
                                fusible++;
                            }
                            #endregion
                        }
                    }
                    else // NO EXISTE PRN
                    {
                        #region // INTENTA CONECTAR CON CONFIG.BIN
                        try
                        {
                            sale = LogInTelefono(TransacManager.ProtoConfig.CONFIG.Host, TransacManager.ProtoConfig.CONFIG.Telefono);
                        }
                        catch (Exception e)
                        {
                            LogBMTP.LogMessage("TELEFONO: Excepción al intentar con valores por defecto.", lvlLogExcepciones, TimeStampLog);
                            LogBMTP.LogMessage(e.Message + " \n" + e.InnerException, lvlLogDebug, TimeStampLog);
                        }

                        if (sale == false)
                        {
                            LogBMTP.LogMessage("TELEFONO: Intento valores por defecto: Nombre " + TransacManager.ProtoConfig.CONFIG.Host + " falló.", lvlLogCxn, TimeStampLog);
                            cxnErr.CodError = (int)ErrComunicacion.CONEX_SERIAL;
                            cxnErr.Descripcion = "Error de conexión. No pudo establecerse una comunicación con los valores del PRN, ni los por defecto.";
                            cxnErr.Estado = 0;
                            CierraPuertoSerie();
                            sale = true;
                        }
                        else
                        {
                            UltimaConexionOptima[0] = TransacManager.ProtoConfig.CONFIG.DefaultServer.ToString();
                            UltimaConexionOptima[1] = TransacManager.ProtoConfig.CONFIG.Port.ToString();
                            UltimaConexionOptima[2] = TransacManager.ProtoConfig.CONFIG.Telefono;

                            Comunicacion cm = new Comunicacion(TransacManager.ProtoConfig.BASE_CONFIG, TransacManager.ProtoConfig.CONFIG);
                            IList rdo = cm.InteraccionAB(ref ter, true);
                            if (rdo[0] is Error)
                            {
                                if (((Error)rdo[0]).CodError != 0)
                                {
                                    cxnErr = (Error)rdo;
                                    sale = true;
                                }
                                else
                                {
                                    CierraPuertoSerie();
                                    cxnErr = AbrePuertoTelefono();
                                    if (cxnErr.CodError == 0)
                                        sale = false;
                                    else
                                    {
                                        CierraPuertoSerie();
                                        return cxnErr;
                                    }
                                }
                            }
                            else
                            {
                                cxnErr.CodError = (int)ErrComunicacion.CXN_NO_GENERA_PRN;
                                cxnErr.Descripcion = "Error de conexión. No se pudo obtener el PRN.";
                                cxnErr.Estado = 0;
                                CierraPuertoSerie();
                                return cxnErr;
                            }
                            fusible++;
                        }
                        #endregion

                        if (fusible >= 2 && sale == false)
                        {
                            sale = true;
                            cxnErr.CodError = (int)ErrComunicacion.CONEXION;
                            cxnErr.Descripcion = "Error de conexión. No puede establecerse una comunicación. Se superaron los intentos de conexión.";
                            cxnErr.Estado = 0;

                            CierraPuertoSerie();
                            break;
                        }
                    }
                }
                #endregion

                if (cxnErr.CodError != 0)
                    LogBMTP.LogMessage("Error de conexión: " + cxnErr.CodError + "\n" + " Descripción: " + cxnErr.Descripcion + "\n", lvlLogError, TimeStampLog);

                return cxnErr;
            }
            catch (Exception ex)
            {
                cxnErr.CodError = (int)ErrComunicacion.CONEX_SERIALex;
                cxnErr.Descripcion = "Error de conexión. No puede establecerse una comunicación.";
                cxnErr.Estado = 0;

                LogBMTP.LogMessage("Excepción: " + cxnErr.CodError + " " + cxnErr.Descripcion, lvlLogExcepciones, TimeStampLog);
                LogBMTP.LogMessage("Excepción: " + ex.ToString(), lvlLogExcepciones, TimeStampLog);

                return cxnErr;
            }

        }

        static void port1_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {            
            SerialPort sp = (SerialPort)sender;            
        }        
        private static void CierraPuertoSerie()
        {
            if (port1 != null)
            {
                if (port1.IsOpen)
                {
                    port1.RtsEnable = false;
                    System.Threading.Thread.Sleep(100);
                    port1.Close();
                }
                port1.Dispose();
            }
        }
        #endregion

        #region TRANSACCIONES
        
        #endregion
        
        #region ENVIO Y RECEPCION
        public void Enviar(byte[] aEnviar, EnumPaquete tipo, ushort orden, int intentos = 0)
        {
            if (TransacManager.ProtoConfig.NACK_ENV == NackEnv.SINERROR)            
                LogBMTP.LogBuffer(aEnviar, "Envia " + tipo.ToString() + " ( " + aEnviar.Length.ToString() + "b )", aEnviar.Length, lvlLogTransaccion);
            else
                LogBMTP.LogBuffer(aEnviar, "Envia NACK tipo " + TransacManager.ProtoConfig.NACK_ENV + " ( " + aEnviar.Length.ToString() + "b )", aEnviar.Length, lvlLogError);

            byte[] aEnviar4;
            Error Err = TR.Pack(aEnviar, out aEnviar4, tipo, orden);

            if (Err.CodError != 0) { }


            //if (NivelLog <= lvlLogDebug)
                LogBMTP.LogBuffer(aEnviar4, "Enmascarado ( " + aEnviar4.Length.ToString() + "b )", aEnviar4.Length, lvlLogDebug);            

            if (!esPorPuertoSerie)
                bytesCount = sender.Send(aEnviar4);
            else
            {
                try
                {                    
                    if (port1.CtsHolding == true)
                    {
                        if (TransacManager.ProtoConfig.TIPO_CXN == EnumModoConexion.RADIO)
                        {
                            System.Threading.Thread.Sleep(100);                            
                            port1.Write(aEnviar4, 0, aEnviar4.Length);
                            EsperaPuertoSerie(port1, aEnviar4.Length);
                            //System.Threading.Thread.Sleep(100);                        
                        }
                        else if(TransacManager.ProtoConfig.TIPO_CXN == EnumModoConexion.DIALUP )
                        {
                            System.Threading.Thread.Sleep(100);
                            
                            port1.Write(aEnviar4, 0, aEnviar4.Length);
                            port1.WriteLine("01 02 00 00 00 4d 41 00 00 03 0d 00 00 c3 6b 4d 7c 6d 07 33 30 30 39 31 34 31 37 30 34 32 30 00 6d 00 20 00 c1 00 87 ab 67 00 96 14 86 30 38 30 30 36 36 36 35 38 30 37 20 20 20 20 20 20 20 57 69 6e 37 78 38 36 02 00 51 40 f4 83 03");

                            EsperaPuertoSerie(port1, aEnviar4.Length);
                        }
                    }
                    else
                    {
                        if (intentos < 3)
                        {
                            intentos++;
                            Enviar(new byte[ProtocoloConfig.TamBuffer], tipo, orden, intentos);
                        }
                        else
                            throw new ArgumentException("Señal CTS (Clear To Send) en falso. Cantidad máxima de intentos superada.");
                    }                   
                }
                catch (Exception e)
                {
                        throw e;
                }
                    
            }            
        }        
        public IList Recibir(byte[] aRecibir, int tipoEsperado, ushort orden, string tipoMen, int intentos = 0)
        {            
            Error Err = new Error();
            IList objMensaje = new List<object>();
            try
            {
                #region ETHERNET
                if (!esPorPuertoSerie)
                {
                    bytesCount = sender.Receive(aRecibir);
                    Array.Resize(ref aRecibir, bytesCount);
                }
                #endregion
                #region TELEFONO
                else if (TransacManager.ProtoConfig.TIPO_CXN == EnumModoConexion.DIALUP)
                {
                    byte[] bytepr = new byte[ProtocoloConfig.TamBuffer];
                    int con = 0;
                    try
                    {
                        //System.Threading.Thread.Sleep(100);
                        EsperaPuertoSerie(port1, port1.ReceivedBytesThreshold);
                        if (port1.DsrHolding == true)
                        {
                            LogBMTP.LogMessage("Comienza a buscar 0x01", lvlLogDebug, TimeStampLog);
                            while (bytePuertoSerie != 0x01)
                            {
                                bytePuertoSerie = Convert.ToByte(port1.ReadByte());
                                if (bytePuertoSerie.ToString() == "%")
                                {
                                    string msgEr = port1.ReadExisting();
                                    LogBMTP.LogMessage(msgEr, lvlLogError, TimeStampLog);
                                    objMensaje.Insert(0, new Error("Ha ocurrido una falla en la conexión.", (uint)ErrComunicacion.CONEX_SERIAL, 0));
                                    return objMensaje;
                                }

                                bytepr[con] = (byte)bytePuertoSerie;
                                con++;
                            }

                            LogBMTP.LogMessage("Encuentra 0x01", lvlLogDebug, TimeStampLog);
                            Array.Resize(ref bytepr, con);
                            if (bytepr.Length > 2 && NivelLog <= lvlLogDebug)
                                LogBMTP.LogBuffer(bytepr, "Bytes de Radio ajenos al protocolo normal. Normalmente no viene nada.", bytepr.Length, lvlLogDebug);

                            aRecibir = new byte[ProtocoloConfig.TamBuffer];
                            aRecibir[0] = bytePuertoSerie;
                            con = 1;

                            while (bytePuertoSerie != 0x03)
                            {
                                bytePuertoSerie = LeeBytePuertoSerie(ref objMensaje);

                                if (bytePuertoSerie.ToString() == "%")
                                {
                                    return objMensaje;
                                }

                                aRecibir[con] = bytePuertoSerie;
                                con++;
                            }
                            LogBMTP.LogMessage("Encuentra 0x03", lvlLogDebug, TimeStampLog);
                            Array.Resize(ref aRecibir, con);
                            //port1.RtsEnable = false;
                            port1.DiscardInBuffer();
                        }
                        else
                        {
                            objMensaje.Insert(0, new Error("Se ha perdido el vínculo físico.", (uint)ErrComunicacion.CONEX_SERIAL, 0));
                            return objMensaje;
                        }

                    }
                    catch (Exception e)
                    {
                        CierraPuertoSerie();
                        LogBMTP.LogMessage("Vínculo telefónico caído. Restableciendo...", lvlLogTransaccion, TimeStampLog);
                        if (intentos < 3)
                        {
                            LogBMTP.LogMessage("Reintento de conexión " + intentos.ToString(), lvlLogDebug, TimeStampLog);

                            Error erPort = AbrePuertoTelefono();

                            if (erPort.CodError == 0)
                            {
                                if (intentos < 3 && LogInTelefono("motor4", UltimaConexionOptima[2]))
                                {
                                    intentos++;
                                    return Recibir(new byte[ProtocoloConfig.TamBuffer], tipoEsperado, orden, tipoMen, intentos);
                                }
                            }
                            LogBMTP.LogMessage("Falló reintento " + intentos.ToString(), lvlLogDebug, TimeStampLog);
                            throw e;
                        }
                        else
                        {
                            Array.Resize(ref bytepr, con);
                            LogBMTP.LogBuffer(bytepr, "Superados intentos. Esto se recibió: ", bytepr.Length, lvlLogDebug);
                            throw e;
                        }
                    }
                }
                #endregion
                #region PUERTO SERIE
                else if (TransacManager.ProtoConfig.TIPO_CXN == EnumModoConexion.RADIO)
                {
                    byte[] bytepr = new byte[ProtocoloConfig.TamBuffer];
                    int con = 0;
                    try
                    {
                        //System.Threading.Thread.Sleep(100);
                        EsperaPuertoSerie(port1, port1.ReceivedBytesThreshold);
                        //if (port1.DsrHolding == true && port1.CDHolding == true)
                        //{
                        LogBMTP.LogMessage("Comienza a buscar 0x01", lvlLogDebug, TimeStampLog);
                        while (bytePuertoSerie != 0x01)
                        {
                            bytePuertoSerie = Convert.ToByte(port1.ReadByte());
                            if (bytePuertoSerie.ToString() == "%")
                            {
                                string msgEr = port1.ReadExisting();
                                LogBMTP.LogMessage(msgEr, lvlLogError, TimeStampLog);
                                objMensaje.Insert(0, new Error("Ha ocurrido una falla en la conexión.", (uint)ErrComunicacion.CONEX_SERIAL, (uint)0));
                                return objMensaje;
                            }

                            bytepr[con] = (byte)bytePuertoSerie;
                            con++;
                        }

                        LogBMTP.LogMessage("Encuentra 0x01", lvlLogDebug, TimeStampLog);
                        Array.Resize(ref bytepr, con);
                        if (bytepr.Length > 2 && NivelLog <= lvlLogDebug)
                            LogBMTP.LogBuffer(bytepr, "Bytes de Radio ajenos al protocolo normal. Normalmente no viene nada.", bytepr.Length, lvlLogDebug);

                        aRecibir = new byte[ProtocoloConfig.TamBuffer];
                        aRecibir[0] = bytePuertoSerie;
                        con = 1;

                        while (bytePuertoSerie != 0x03)
                        {
                            bytePuertoSerie = LeeBytePuertoSerie(ref objMensaje);

                            if (bytePuertoSerie.ToString() == "%")
                            {
                                return objMensaje;
                            }

                            aRecibir[con] = bytePuertoSerie;
                            con++;
                        }
                        LogBMTP.LogMessage("Encuentra 0x03", lvlLogDebug, TimeStampLog);
                        Array.Resize(ref aRecibir, con);
                        //port1.RtsEnable = false;
                        port1.DiscardInBuffer();
                        //}
                        //else
                        //{
                        //    mensajeR.Insert(0, new Error("Se ha perdido el vínculo físico.", (uint)ErrComunicacion.CONEX_SERIAL, 0));
                        //    return mensajeR;
                        //}

                    }
                    catch (Exception e)
                    {
                        CierraPuertoSerie();


                        LogBMTP.LogMessage("Vínculo de radio caído. Restableciendo...", lvlLogTransaccion, TimeStampLog);
                        if (intentos < 3)
                        {
                            LogBMTP.LogMessage("Reintento de conexión " + intentos.ToString(), lvlLogDebug, TimeStampLog);

                            Error erPort = AbrePuertoRadio();

                            if (erPort.CodError == 0)
                            {
                                if (intentos < 3 && LogInRadio(UltimaConexionOptima[0]))
                                {
                                    intentos++;
                                    return Recibir(new byte[ProtocoloConfig.TamBuffer], tipoEsperado, orden, tipoMen, intentos);
                                }
                            }
                            LogBMTP.LogMessage("Falló reintento " + intentos.ToString(), lvlLogDebug, TimeStampLog);
                            throw e;
                        }
                        else
                        {
                            Array.Resize(ref bytepr, con);
                            LogBMTP.LogBuffer(bytepr, "Bytes de Radio: fallaron " + intentos.ToString() + " intentos. Esto se recibió: ", bytepr.Length, lvlLogDebug);
                            throw e;
                        }
                    }
                }
                #endregion

                #region LOG ENMASCARADO                
                //if (NivelLog <= lvlLogDebug)
                    LogBMTP.LogBuffer(aRecibir, "Enmascarado ( " + aRecibir.Length.ToString() + "b )", aRecibir.Length, lvlLogDebug);
                #endregion

                byte[] salidaUnpack;
                Err = TR.Unpack(aRecibir, out salidaUnpack);                

                objMensaje = TR.MapeoObjetos(salidaUnpack, tipoEsperado, orden, tipoMen);

                #region LOGS SIN ENMASCARAR
                if (Err.CodError != 0)
                {
                    LogBMTP.LogMessage(Err.Descripcion, lvlLogError, TimeStampLog);
                    return new List<object>() { Err };
                }
                if (tipoEsperado == 4 && Empaquetador.tipoPaqRec == (byte)EnumPaquete.EOT)
                {
                    LogBMTP.LogMessage("Recibe EOT ", lvlLogTransaccion, TimeStampLog);
                    return objMensaje;
                }
                else if (Empaquetador.tipoPaqRec == (byte)EnumPaquete.EOT)
                {
                    LogBMTP.LogMessage("Recibe EOT INESPERADO en " + tipoMen, EnumNivelLog.Warn, TimeStampLog);
                    return objMensaje;
                }
                else if (Empaquetador.tipoPaqRec == (byte)EnumPaquete.NACK)
                {
                    LogBMTP.LogMessage("Recibe NACK causa " + objMensaje[0] + " en " + tipoMen, EnumNivelLog.Warn, TimeStampLog);
                    return objMensaje;
                }
                else if (tipoEsperado == 6 && Empaquetador.tipoPaqRec == (byte)EnumPaquete.ACK)
                {
                    LogBMTP.LogMessage("Recibe ACK ", lvlLogTransaccion, TimeStampLog);
                    return objMensaje;
                }
                else if (Empaquetador.tipoPaqRec == (byte)EnumPaquete.ACK)
                {
                    LogBMTP.LogMessage("Recibe ACK INESPERADO en " + tipoMen, EnumNivelLog.Warn, TimeStampLog);
                    return objMensaje;
                }

                LogBMTP.LogBuffer(salidaUnpack, "Recibe " + tipoMen.Substring(0, 1) + " ( " + salidaUnpack.Length.ToString() + "b )", salidaUnpack.Length, lvlLogTransaccion);

                #endregion
            }
            catch (SocketException es)
            {
                //if (CONFIG.LevelLog == lvlLogExcepciones, TimeStampLog || CONFIG.LevelLog == EnumMessageType.DEBUG)
                LogBMTP.LogMessage("Excepción de Socket en Recibir(): " + es.ToString(), lvlLogExcepciones, TimeStampLog);
                sender.Close();
                objMensaje.Add(new Error("Se perdió el vínculo de conexión.", (uint)ErrComunicacion.CXN_SOCKETex, (uint)0));
            }
            catch (Exception ex)
            {
                //if (CONFIG.LevelLog == EnumMessageType.ERROR || CONFIG.LevelLog == EnumMessageType.DEBUG)
                LogBMTP.LogMessage("Excepción en Recibir(): " + ex.ToString(), lvlLogExcepciones, TimeStampLog);
            }
            
            return objMensaje;            
        }
        #endregion
        
        #region Manejo errores
        public Error MensajeNack(object tipoNack)
        {
            Error er = new Error();
            if ((TIPOS_NACK.Contains((string)tipoNack)))
            {
            switch ((string)tipoNack)
            {
                case "B": er.CodError = (int)NackRec.B; break;
                case "T": er.CodError = (int)NackRec.T; break;
                case "P": er.CodError = (int)NackRec.P; break;
                case "E": er.CodError = (int)NackRec.E; break;
                }            
                er.Descripcion = " Recibido Nack causa " + (string)tipoNack + ".";
                
                //IList objs = new List<object>();
                //bytes = new byte[TamBuffer];
                
                //LogBMTP.LogMessage("Respuesta ", CONFIG.LevelLog);
                //objs = RecibirSgte(bytes, 4, 0, "EOT");

                //if(objs[0] is string && ((string)objs[0]) == "Eot")
                //{
                //    tipoNack = "Eot";
                //}
            }

            if(((string)tipoNack) == "Eot")
            {
                er = Desconectar();
                if(er.CodError == 0)
                {
                    er.CodError = (uint)ErrComunicacion.CONEXION;
                    er.Descripcion = "Recibido EOT. Conexión terminada exitosamente.";
                    er.Estado = 0;
                }
            }

            return er;
        }
        public Error EmpalmeErrorExcepcion(Exception ex, ErrComunicacion codigoError, string MensajeUsuario, uint estado = 99)
        {
            Error errEx = new Error();
            errEx.CodError = (uint)codigoError;
            errEx.Descripcion = MensajeUsuario;
            if ((sender != null && sender.Connected) || (esPorPuertoSerie && Comunicacion.port1.IsOpen))
            {
                if (estado == 99)
                    errEx.Estado = 1;
                else if (estado != 99)
                    errEx.Estado = estado;
            }
            else
                errEx.Estado = 0;
            LogBMTP.LogMessage("Excepción: " + errEx.CodError + " " + errEx.Descripcion, Comunicacion.lvlLogExcepciones, Comunicacion.TimeStampLog);
            LogBMTP.LogMessage("Excepción: " + ex.ToString(), Comunicacion.lvlLogDebug, Comunicacion.TimeStampLog);
            return errEx;
        }
        #endregion
        #region Conexion
        private void CambiaSeniales(SerialPort port)
        {
            port.DtrEnable = !port.DtrEnable;
            port.RtsEnable = !port.RtsEnable;
        }
        private byte LeeBytePuertoSerie(ref IList objs)
        {
            byte by = Convert.ToByte(port1.ReadByte()); 
            if (by.ToString() == "%")
            {
                string msgEr = port1.ReadExisting();
                LogBMTP.LogMessage(msgEr, lvlLogError, TimeStampLog);
                objs.Add(new Error("Ha ocurrido una falla en la conexión.", (uint)ErrComunicacion.CONEX_SERIAL, (uint)0));
            }
            return by;
        }        
        private void EsperaPuertoSerie(SerialPort port, int cantBytRec)
        {
            decimal deltaBaud = 0;
            int ms = 0;
            deltaBaud = (decimal)1000 / port1.BaudRate;

            ms = (int)(deltaBaud * (cantBytRec * (port1.DataBits + 2)));
            System.Threading.Thread.Sleep(ms*10);//2400            
        }
        private string LeeErrRadio(string inputStr)
        {           
            int i = inputStr.Length;
            string salida = "";
            char[] chArr = inputStr.ToCharArray();
            if (i != 0)
            {
                i = inputStr.Length - 1;

                while (chArr[i] != '%' && i != 0)
                    i--;

                if (i == 0)
                    salida = "SinError";
                else
                {
                    salida = inputStr.Substring(i);
                    LogBMTP.LogMessage(salida, lvlLogError, TimeStampLog);
                }
            }
            else
                salida = "SinRespuesta";

            return salida;
        }
        private Error LeeErrTelefonoSerie(string input)
        {
            Error err = new Error();
            if (input.Length > 0 && input.Contains("NO CARRIER"))
                err = new Error("Conexión telefónica interrumpida 'NO CARRIER'.", (uint)ErrComunicacion.CONEX_SERIAL, 1);
            else if (input.Length > 0 && input.Contains("BUSY"))
                err = new Error("Linea ocupada 'BUSY'.", (uint)ErrComunicacion.CONEX_SERIAL, 1);
            else if (input.Length > 0 && input.Contains("NO DIALTONE"))
                err = new Error("Sin tono o linea no conectada 'NO DIALTONE'.", (uint)ErrComunicacion.CONEX_SERIAL, 0);
            else if (input.Length > 0) { }
            else  err = new Error("SinRespuesta", (uint)ErrComunicacion.CONEX_SERIAL, 1);

            return err;                
        }
        private bool PuertoDisponible(int puerto)
        {
            IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            TcpConnectionInformation[] tcpConnInfoArray = ipGlobalProperties.GetActiveTcpConnections();

            foreach (TcpConnectionInformation tcpi in tcpConnInfoArray)
            {
                if (tcpi.LocalEndPoint.Port == puerto)
                {
                    return false;
                }
            }

            return true;
        }
        #endregion

        public virtual void Dispose(bool disposeManagedResources)
        {
            if(sender != null)
                sender.Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }    
}