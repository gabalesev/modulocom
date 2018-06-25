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
using LibreriaMetodologia;
using LibreriaRegistro;
using System.Text;

namespace LibreriaModuloTransaccional
{
    public partial class Comunicacion: IDisposable
    {
        public OperaOffLine opOff = new OperaOffLine();

        public GestorTransacciones GestorTransacciones;

        private IList<string> TIPOS_NACK = new List<string> { "B", "E", "T", "P" };
                
        #region Propiedades Logger        
        private readonly NLog.LogLevel NivelRegistroTransaccion = NLog.LogLevel.Info;
        private readonly NLog.LogLevel NivelRegistroCabeceraBuffer = NLog.LogLevel.Info;
        private readonly NLog.LogLevel NivelRegistroCxn = NLog.LogLevel.Info;
        private readonly NLog.LogLevel NivelRegistroError = NLog.LogLevel.Error;
        private readonly NLog.LogLevel NivelRegistroExcepciones = NLog.LogLevel.Fatal;
        private readonly NLog.LogLevel NivelRegistroDebug = NLog.LogLevel.Debug;
        private const bool REGISTRAR_HORA = true;
        #endregion

        #region CONSTRUCTORES
        public Comunicacion(BaseConfig baseConf, ArchivoConfig conf)
        {
            GestorTransacciones.ConfiguracionComunicacion = new ConfiguracionComunicacion(baseConf, conf);

            var NivelLog = NLog.LogLevel.Off;

            switch (GestorTransacciones.ConfiguracionComunicacion.Configuracion.LevelLog)
            {
                case EnumMessageType.DEBUG: NivelLog = NLog.LogLevel.Trace; break;
                case EnumMessageType.NORMAL: NivelLog = NLog.LogLevel.Info; break;
                case EnumMessageType.WARNING: NivelLog = NLog.LogLevel.Warn; break;
                case EnumMessageType.ERROR: NivelLog = NLog.LogLevel.Error; break;
                case EnumMessageType.NOTHING: NivelLog = NLog.LogLevel.Off; break;
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

            

            ModuloDeRegistro.InicializaRegistrador(conf.LogPath, conf.LogMaxFileSize, NivelLog, fName);

            GestorTransacciones = new GestorTransacciones();
        }
        public Comunicacion(BaseConfig baseConf, ArchivoConfig conf, bool interno)
        {
            var NivelLog = NLog.LogLevel.Off;

            switch (GestorTransacciones.ConfiguracionComunicacion.Configuracion.LevelLog)
            {
                case EnumMessageType.DEBUG: NivelLog = NLog.LogLevel.Trace; break;
                case EnumMessageType.NORMAL: NivelLog = NLog.LogLevel.Info; break;
                case EnumMessageType.WARNING: NivelLog = NLog.LogLevel.Warn; break;
                case EnumMessageType.ERROR: NivelLog = NLog.LogLevel.Error; break;                
                case EnumMessageType.NOTHING: NivelLog = NLog.LogLevel.Off; break;
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

            ModuloDeRegistro.InicializaRegistrador(conf.LogPath, conf.LogMaxFileSize, NivelLog, fName);

            GestorTransacciones = new GestorTransacciones();
        }
        #endregion        

        #region CONEXIÓN
        private static IPEndPoint ipEndPoint;
        private Socket Socket;
        private static string[] UltimaConexionOptima = new string[3];

        public Error Conectar(Terminal ter, EnumModoConexion modoConexion)
        {
            Error cxnErr = new Error();

            ModuloDeRegistro.RegistrarMensaje("TERMINAL: " + ter.NumeroTerminal + " USUARIO: " + ter.Tarjeta, NivelRegistroCxn, REGISTRAR_HORA);
            
            #region // DIAL UP
            if (modoConexion == EnumModoConexion.DIALUP)
            {
                GestorTransacciones.ConfiguracionComunicacion.TipoConexion = EnumModoConexion.DIALUP;
                if (GestorTransacciones.ConfiguracionComunicacion.Configuracion.CxnOnlineHabilitado)
                {
                    ModuloDeRegistro.RegistrarMensaje("Modo de conexión: TELEFONO", NivelRegistroCxn, REGISTRAR_HORA);
                    ModuloDeRegistro.RegistrarMensaje("(Timeout DialUp: " + GestorTransacciones.ConfiguracionComunicacion.Configuracion.CxnTelTimeout + ")", NivelRegistroCxn, REGISTRAR_HORA);                    
                    
                    cxnErr = ConectarDialUp(ter);
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
                GestorTransacciones.ConfiguracionComunicacion.TipoConexion = EnumModoConexion.ETHERNET;
                if (GestorTransacciones.ConfiguracionComunicacion.Configuracion.CxnOnlineHabilitado)
                {
                    ModuloDeRegistro.RegistrarMensaje("Modo de conexión: ETHERNET \n", NivelRegistroCxn, REGISTRAR_HORA);
                    cxnErr = ConectarAServidor(ter);
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

                if (GestorTransacciones.ConfiguracionComunicacion.TipoConexion == EnumModoConexion.DIALUP)
                {
                    #region // Secuencia de desconexión DIAL UP
                    if (Socket != null)
                    {
                        if (Socket.Connected)
                        {
                            Enviar(null, EnumPaquete.EOT, 0);

                            // Disables sends and receives on a Socket.
                            System.Threading.Thread.Sleep(700);

                            Socket.Shutdown(SocketShutdown.Both);
                            Socket.Close();// (SocketShutdown.Both);
                            if (!Socket.Connected)
                                ModuloDeRegistro.RegistrarMensaje("COMUNICACIÓN FINALIZADA POR TERMINAL", NivelRegistroCxn, REGISTRAR_HORA);
                            else
                                ModuloDeRegistro.RegistrarMensaje("ATENCIÓN: COMUNICACIÓN NO HA SIDO FINALIZADA", NivelRegistroCxn, REGISTRAR_HORA);
                        }
                        else
                            ModuloDeRegistro.RegistrarMensaje("COMUNICACIÓN FINALIZADA POR SERVICIO", NivelRegistroCxn, REGISTRAR_HORA);
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
                else if (GestorTransacciones.ConfiguracionComunicacion.TipoConexion == EnumModoConexion.ETHERNET)
                {
                    #region // Secuencia de desconexión ETHERNET
                    if (Socket != null)
                    {
                        if (Socket.Connected)
                        {
                            Enviar(null, EnumPaquete.EOT, 0);

                            // Disables sends and receives on a Socket.
                            System.Threading.Thread.Sleep(700);

                            Socket.Shutdown(SocketShutdown.Both);
                            Socket.Close();// (SocketShutdown.Both);
                            if (!Socket.Connected)
                                ModuloDeRegistro.RegistrarMensaje("COMUNICACIÓN FINALIZADA POR TERMINAL", NivelRegistroCxn, REGISTRAR_HORA);
                            else
                                ModuloDeRegistro.RegistrarMensaje("ATENCIÓN: COMUNICACIÓN NO HA SIDO FINALIZADA", NivelRegistroCxn, REGISTRAR_HORA);
                        }
                        else
                        {
                            Socket.Close();// (SocketShutdown.Both);

                            ModuloDeRegistro.RegistrarMensaje("COMUNICACIÓN FINALIZADA POR SERVICIO", NivelRegistroCxn, REGISTRAR_HORA);
                        }
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
        private Error ConectarAServidor(Terminal ter)
        {
            string nombre1, nombre2, nombre3, port1, port2, port3, tel1, tel2, tel3;

            GestorArchivoConfiguracionConexion cxn = new GestorArchivoConfiguracionConexion();
            Error cxnErr = new Error();
            IPHostEntry ipHost;
            IPAddress ipAddr;

            try
            {
                #region // PROCESO DE CONEXIÓN

                IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());

                for (int i = 0; i < ipHostInfo.AddressList.Length; i++)
                {
                    GestorTransacciones.ConfiguracionComunicacion.LocalIP = ipHostInfo.AddressList[i]; //----------- Cambia de 1 elemento a 2.
                    if (GestorTransacciones.ConfiguracionComunicacion.LocalIP.AddressFamily == AddressFamily.InterNetwork) break;
                }

                IPEndPoint localEndPoint = new IPEndPoint(GestorTransacciones.ConfiguracionComunicacion.LocalIP, GestorTransacciones.ConfiguracionComunicacion.LocalPort);

                bool sale = false;
                int fusible = 0;
                while (sale == false) // INTENTA HASTA CONECTAR CON PRN
                {
                    SocketPermission permission = new SocketPermission(PermissionState.Unrestricted);
                    permission.Demand();

                    string res = cxn.Leer_XMLprn(out nombre1, out nombre2, out nombre3, out port1, out  port2, out port3, out tel1, out tel2, out tel3, GestorTransacciones.ConfiguracionComunicacion.Configuracion);

                    if (res == null && GestorTransacciones.ConfiguracionComunicacion.UsaCicloPRN) // EXISTE PRN
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

                                    ModuloDeRegistro.RegistrarMensaje("Resulve DNS: " + nom[j] + " - " + ipAddr, NivelRegistroCxn, REGISTRAR_HORA);

                                    ipEndPoint = new IPEndPoint(ipAddr, Convert.ToInt32(por[j]));

                                    Socket = new Socket(
                                    ipAddr.AddressFamily,// Specifies the addressing scheme
                                    SocketType.Stream,   // The type of socket 
                                    ProtocolType.Tcp     // Specifies the protocols 
                                    );

                                    Socket.NoDelay = false;   // Using the Nagle algorithm

                                    Socket.ReceiveTimeout = GestorTransacciones.ConfiguracionComunicacion.TimeoutSocket;
                                    Socket.SendTimeout = GestorTransacciones.ConfiguracionComunicacion.TimeoutSocket;

                                    Socket.Bind(localEndPoint);

                                    Socket.Connect(ipEndPoint);
                                }
                            }
                            catch (Exception e)
                            {
                                ipHost = null;
                                ModuloDeRegistro.RegistrarMensaje(" Intento con " + nom[j] + "(" + j + ") falló. \n" + e.Message + " " + e.InnerException, NivelRegistroDebug, REGISTRAR_HORA);
                                if (Socket != null)
                                    Socket.Close();
                            }
                            if (Socket != null && Socket.Connected == true)
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

                        if (Socket == null || Socket.Connected != true)
                        {
                            #region // FALLÓ PRN, INTENTA CON VALORES DEFAULT
                            cxn.Borrar_XMLprn(GestorTransacciones.ConfiguracionComunicacion.Configuracion);

                            try
                            {
                                while (!PuertoDisponible(localEndPoint.Port))
                                {
                                    localEndPoint.Port++;
                                }

                                ipAddr = GestorTransacciones.ConfiguracionComunicacion.Configuracion.DefaultServer;

                                ipEndPoint = new IPEndPoint(ipAddr, GestorTransacciones.ConfiguracionComunicacion.Configuracion.Port);

                                Socket = new Socket(
                                ipAddr.AddressFamily,// Asigno tipo de address 
                                SocketType.Stream,   // Tipo de socket
                                ProtocolType.Tcp     // Tipo de protocolo
                                );

                                Socket.NoDelay = false;   // Using the Nagle algorithm                                                               

                                Socket.ReceiveTimeout = GestorTransacciones.ConfiguracionComunicacion.TimeoutSocket;
                                Socket.SendTimeout = GestorTransacciones.ConfiguracionComunicacion.TimeoutSocket;

                                Socket.Bind(localEndPoint);

                                Socket.Connect(ipEndPoint);
                            }
                            catch (Exception e)
                            {
                                ipHost = null;
                                ModuloDeRegistro.RegistrarMensaje(e.Message + " \n" + e.InnerException, NivelRegistroDebug, REGISTRAR_HORA);
                            }

                            if (Socket.Connected != true)
                            {
                                cxnErr.CodError = (int)ErrComunicacion.CXN_SOCKET;
                                cxnErr.Descripcion = "Error de conexión. No pudo establecerse una comunicación con los valores del PRN, ni los por defecto.";
                                cxnErr.Estado = 0;
                                sale = true;
                            }
                            else
                            {
                                UltimaConexionOptima[0] = GestorTransacciones.ConfiguracionComunicacion.Configuracion.DefaultServer.ToString();
                                UltimaConexionOptima[1] = GestorTransacciones.ConfiguracionComunicacion.Configuracion.Port.ToString();
                                UltimaConexionOptima[2] = GestorTransacciones.ConfiguracionComunicacion.Configuracion.Telefono;

                                ModuloDeRegistro.RegistrarMensaje("CONEXIÓN EXITOSA CON VALORES DEFAULT:\n\nHOST: IP "
                                + ipEndPoint.Address + " Port " + ipEndPoint.Port + "\n"
                                + "BMTP: IP " + ((IPEndPoint)Socket.LocalEndPoint).Address + " Port " + ((IPEndPoint)Socket.LocalEndPoint).Port + "\n", NivelRegistroCxn, false);

                                Comunicacion cm = new Comunicacion(GestorTransacciones.ConfiguracionComunicacion.BASE_CONFIG, GestorTransacciones.ConfiguracionComunicacion.Configuracion, true);
                                cm.Socket = Socket;
                                IList rdo = cm.InteraccionAB(ref ter, true);
                                if (rdo[0] is Error)
                                {
                                    if (((Error)rdo[0]).CodError != 0)
                                    {
                                        ModuloDeRegistro.RegistrarMensaje(((Error)rdo[0]).Descripcion, NivelRegistroError, REGISTRAR_HORA);
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
                                fusible++;
                            }
                            #endregion
                        }
                    }
                    else // NO EXISTE PRN
                    {
                        #region // INTENTA CONECTAR CON CONFIG
                        try
                        {
                            if (!PuertoDisponible(GestorTransacciones.ConfiguracionComunicacion.Configuracion.Port) || !PuertoDisponible(localEndPoint.Port))
                            {
                                cxnErr.CodError = (int)ErrComunicacion.CXN_SOCKET;
                                cxnErr.Descripcion = "Error de conexión. Puerto default ocupado.";
                                cxnErr.Estado = 0;
                                return cxnErr;
                            }

                            ipAddr = GestorTransacciones.ConfiguracionComunicacion.Configuracion.DefaultServer;

                            ipEndPoint = new IPEndPoint(ipAddr, GestorTransacciones.ConfiguracionComunicacion.Configuracion.Port);

                            Socket = new Socket(
                            ipAddr.AddressFamily,// Asigno tipo de address de
                            SocketType.Stream,   // The type of socket 
                            ProtocolType.Tcp     // Specifies the protocols 
                            );

                            Socket.NoDelay = false;   // Using the Nagle algorithm

                            Socket.ReceiveTimeout = GestorTransacciones.ConfiguracionComunicacion.TimeoutSocket;
                            Socket.SendTimeout = GestorTransacciones.ConfiguracionComunicacion.TimeoutSocket;

                            Socket.Bind(localEndPoint);

                            Socket.Connect(ipEndPoint);                            
                        }
                        catch (Exception e)
                        {
                            ipHost = null;
                            ModuloDeRegistro.RegistrarMensaje(e.Message + " \n" + e.InnerException, NivelRegistroDebug, REGISTRAR_HORA);
                        }
                        if (Socket.Connected != true)
                        {
                            cxnErr.CodError = (int)ErrComunicacion.CXN_SOCKET;
                            cxnErr.Descripcion = "Error de conexión. No puede establecerse una comunicación con los valores por defecto.";
                            cxnErr.Estado = 0;
                            sale = true;
                        }
                        else
                        {
                            UltimaConexionOptima[0] = GestorTransacciones.ConfiguracionComunicacion.Configuracion.DefaultServer.ToString();
                            UltimaConexionOptima[1] = GestorTransacciones.ConfiguracionComunicacion.Configuracion.Port.ToString();
                            UltimaConexionOptima[2] = GestorTransacciones.ConfiguracionComunicacion.Configuracion.Telefono;

                            ModuloDeRegistro.RegistrarMensaje("CONEXIÓN EXITOSA CON VALORES DEFAULT:\n\nHOST: IP "
                                + ipEndPoint.Address + " Port " + ipEndPoint.Port + "\n"
                                + "BMTP: IP " + ((IPEndPoint)Socket.LocalEndPoint).Address + " Port " + ((IPEndPoint)Socket.LocalEndPoint).Port + "\n", NivelRegistroCxn, false);

                            if (GestorTransacciones.ConfiguracionComunicacion.UsaCicloPRN)
                            {
                                Comunicacion cm = new Comunicacion(GestorTransacciones.ConfiguracionComunicacion.BASE_CONFIG, GestorTransacciones.ConfiguracionComunicacion.Configuracion, true);
                                cm.Socket = Socket;

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
                    ModuloDeRegistro.RegistrarMensaje("Error: " + cxnErr.CodError + " Descripción: " + cxnErr.Descripcion + "\n", NivelRegistroError, REGISTRAR_HORA);
                else
                {
                    ModuloDeRegistro.RegistrarMensaje("CONEXIÓN EXITOSA:\n ", NivelRegistroCxn, REGISTRAR_HORA);
                    ModuloDeRegistro.RegistrarMensaje("HOST: IP " + ipEndPoint.Address + " Port " + ipEndPoint.Port + "\n", NivelRegistroCxn, REGISTRAR_HORA);
                    ModuloDeRegistro.RegistrarMensaje("BMTP: IP " + ((IPEndPoint)Socket.LocalEndPoint).Address + " Port " + ((IPEndPoint)Socket.LocalEndPoint).Port + "\n", NivelRegistroCxn, REGISTRAR_HORA);
                }

                return cxnErr;
            }
            catch (Exception ex)
            {
                cxnErr.CodError = (int)ErrComunicacion.CONEXIONex;
                cxnErr.Descripcion = "Error de conexión. No puede establecerse una comunicación.";
                cxnErr.Estado = 0;

                ModuloDeRegistro.RegistrarMensaje("Excepción: " + cxnErr.CodError + " " + cxnErr.Descripcion, NivelRegistroExcepciones, REGISTRAR_HORA);
                ModuloDeRegistro.RegistrarMensaje("Excepción: " + ex.ToString(), NivelRegistroExcepciones, REGISTRAR_HORA);

                return cxnErr;
            }

        }
       
        //Dialup
        private Error CreaVinculoTelefonico()
        {
            Error cxnErr = new Error();

            string telDefault = GestorTransacciones.ConfiguracionComunicacion.Configuracion.Telefono;// "08006665807";
            string prefijo = GestorTransacciones.ConfiguracionComunicacion.Configuracion.CxnTelPrefijo;// "11000";
            string separador = "w";
            if (String.IsNullOrEmpty(prefijo))
            {
                prefijo = "";
                separador = "";
            }
            string modemDefault = GestorTransacciones.ConfiguracionComunicacion.Configuracion.CxnTelNombreModem;// "Conexant USB CX93010 ACF Modem";
            string user = GestorTransacciones.ConfiguracionComunicacion.Configuracion.CxnTelUser;// "bmtp";
            string pass = GestorTransacciones.ConfiguracionComunicacion.Configuracion.CxnTelPass;// "bmtp";
            uint timeout = GestorTransacciones.ConfiguracionComunicacion.Configuracion.CxnTelTimeout;

            string nombre1, nombre2, nombre3, port1, port2, port3, tel1, tel2, tel3;

            GestorArchivoConfiguracionConexion cxn = new GestorArchivoConfiguracionConexion();
            
                string rdo = cxn.Leer_XMLprn(out nombre1, out nombre2, out nombre3, out port1, out  port2, out port3, out tel1, out tel2, out tel3, GestorTransacciones.ConfiguracionComunicacion.Configuracion);
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
                            ModuloDeRegistro.RegistrarMensaje("Se leyó PRN. Telefonos alternativos cargados.", NivelRegistroCxn, REGISTRAR_HORA);
                        }
                        else
                        {
                            ModuloDeRegistro.RegistrarMensaje("No hay PRN. Intentará conectar con número telefónico por defecto.", NivelRegistroCxn, REGISTRAR_HORA);
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
                        ModuloDeRegistro.RegistrarMensaje(e.ToString(), NivelRegistroExcepciones, REGISTRAR_HORA);
                        
                        cxnErr.CodError = (int)ErrComunicacion.CXN_TELEFONOex;
                        cxnErr.Descripcion = "Error de conexión. No puede establecerse una comunicación por telefono.";
                        cxnErr.Estado = 0;
                        ModuloDeRegistro.RegistrarMensaje("Excepción: " + cxnErr.CodError + " " + cxnErr.Descripcion, NivelRegistroExcepciones, REGISTRAR_HORA);
                        ModuloDeRegistro.RegistrarMensaje("Excepción: " + e.ToString(), NivelRegistroExcepciones, REGISTRAR_HORA);
                    }                    
                }

                return cxnErr;
            

        }
        private Error ConectarDialUp(Terminal ter)
        {
            Error cxnErr = new Error();

            ModuloDeRegistro.RegistrarMensaje("${level} TERMINAL: " + ter.NumeroTerminal + " TARJETA: " + ter.Tarjeta, NivelRegistroCxn, REGISTRAR_HORA);
            
            #region // Dial Up vía linea telefónica
            if (GestorTransacciones.ConfiguracionComunicacion.Configuracion.CxnDialUpHabilitado)
            {
                GestorTransacciones.ConfiguracionComunicacion.TipoConexion = EnumModoConexion.DIALUP_TEL;
                cxnErr = CreaVinculoTelefonico();
                if (cxnErr.CodError != 0)
                    return cxnErr;
                else
                {
                    cxnErr = ConectarAServidor(ter);
                    if (cxnErr.CodError != 0)
                        return cxnErr;
                }
            }
            #endregion

            return cxnErr;
        }
        #endregion
                
        #region ENVIO Y RECEPCION
        private void Enviar(Terminal terminal, EnumPaquete tipo, ushort orden, int intentos = 0)
        {
            ModuloDeRegistro.RegistrarMensaje("// ENVIA ////////////////////////////////////////////////////////////\n", NLog.LogLevel.Info, false);
           
            byte[] aEnviar4;
            Error Err = GestorTransacciones.Pack(terminal, UltimaConexionOptima, out aEnviar4, tipo, orden);

            if (Err.CodError != 0) { }

            Socket.Send(aEnviar4);
            
        }        
        private IList Recibir(byte[] aRecibir, int tipoEsperado, ushort orden, string tipoMen, int intentos = 0)
        {            
            Error Err = new Error();
            IList objMensaje = new List<object>();
            try
            {
                var bytesCount = Socket.Receive(aRecibir);
                Array.Resize(ref aRecibir, bytesCount);
                
                ModuloDeRegistro.RegistrarMensaje("// RECIBE ///////////////////////////////////////////////////////////\n", NLog.LogLevel.Info, false);
                byte[] salidaUnpack;
                Err = GestorTransacciones.Unpack(aRecibir, out salidaUnpack);                

                objMensaje = GestorTransacciones.MapeoObjetos(salidaUnpack, tipoEsperado, orden, tipoMen);

                #region LOGS SIN ENMASCARAR
                if (Err.CodError != 0)
                {
                    ModuloDeRegistro.RegistrarMensaje(Err.Descripcion, NivelRegistroError, REGISTRAR_HORA);
                    return new List<object>() { Err };
                }
                if (tipoEsperado == 4 && Empaquetador.TipoPaqueteRecibido == (byte)EnumPaquete.EOT)
                {
                    ModuloDeRegistro.RegistrarMensaje("Recibe EOT ", NivelRegistroTransaccion, REGISTRAR_HORA);
                    return objMensaje;
                }
                else if (Empaquetador.TipoPaqueteRecibido == (byte)EnumPaquete.EOT)
                {
                    ModuloDeRegistro.RegistrarMensaje("Recibe EOT INESPERADO en " + tipoMen, NLog.LogLevel.Warn, REGISTRAR_HORA);
                    return objMensaje;
                }
                else if (Empaquetador.TipoPaqueteRecibido == (byte)EnumPaquete.NACK)
                {
                    ModuloDeRegistro.RegistrarMensaje("Recibe NACK causa " + objMensaje[0] + " en " + tipoMen, NLog.LogLevel.Warn, REGISTRAR_HORA);
                    return objMensaje;
                }
                else if (tipoEsperado == (int)EnumPaquete.ACK && Empaquetador.TipoPaqueteRecibido == (byte)EnumPaquete.ACK)
                {
                    ModuloDeRegistro.RegistrarMensaje("Recibe ACK ", NivelRegistroTransaccion, REGISTRAR_HORA);
                    return objMensaje;
                }
                else if (Empaquetador.TipoPaqueteRecibido == (byte)EnumPaquete.ACK)
                {
                    ModuloDeRegistro.RegistrarMensaje("Recibe ACK INESPERADO en " + tipoMen, NLog.LogLevel.Warn, REGISTRAR_HORA);
                    return objMensaje;
                }
                                
                ModuloDeRegistro.LogBuffer(salidaUnpack, "Mensaje " + tipoMen.Substring(0, 1) + " ( " + salidaUnpack.Length.ToString() + "b )", salidaUnpack.Length, NivelRegistroTransaccion);

                #endregion
            }
            catch (SocketException es)
            {
                ModuloDeRegistro.RegistrarMensaje("Excepción de Socket en Recibir(): " + es.ToString(), NivelRegistroExcepciones, REGISTRAR_HORA);
                Socket.Close();
                objMensaje.Add(new Error("Se perdió el vínculo de conexión.", (uint)ErrComunicacion.CXN_SOCKETex, (uint)0));
            }
            catch (Exception ex)
            {
                //if (CONFIG.LevelLog == EnumMessageType.ERROR || CONFIG.LevelLog == EnumMessageType.DEBUG)
                ModuloDeRegistro.RegistrarMensaje("Excepción en Recibir(): " + ex.ToString(), NivelRegistroExcepciones, REGISTRAR_HORA);
            }
            
            return objMensaje;            
        }
        #endregion
        
        #region Manejo errores
        private Error MensajeNack(object tipoNack)
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
        private Error EmpalmeErrorExcepcion(Exception ex, ErrComunicacion codigoError, string MensajeUsuario, uint estado = 99)
        {
            Error errEx = new Error();
            errEx.CodError = (uint)codigoError;
            errEx.Descripcion = MensajeUsuario;
            if ((Socket != null && Socket.Connected))
            {
                if (estado == 99)
                    errEx.Estado = 1;
                else if (estado != 99)
                    errEx.Estado = estado;
            }
            else
                errEx.Estado = 0;
            ModuloDeRegistro.RegistrarMensaje("Excepción: " + errEx.CodError + " " + errEx.Descripcion, NivelRegistroExcepciones, Comunicacion.REGISTRAR_HORA);
            ModuloDeRegistro.RegistrarMensaje("Excepción: " + ex.ToString(), NivelRegistroDebug, Comunicacion.REGISTRAR_HORA);
            return errEx;
        }
        #endregion
        #region Conexion        
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

        //protected virtual void Dispose(bool disposeManagedResources)
        //{
        //    if(Socket != null)
        //        Socket.Dispose();
        //}

        public void Dispose()
        {
            if (Socket != null)
                Socket.Dispose();

            GC.SuppressFinalize(this);
        }
    }    
}