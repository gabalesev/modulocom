// Decompiled with JetBrains decompiler
// Type: LibreriaClases.ArchivoConfig
// Assembly: LibreriaClases, Version=1.0.14.0, Culture=neutral, PublicKeyToken=null
// MVID: 50EE79A0-EB27-4448-9AD7-1911792168AB
// Assembly location: C:\SERVICIOS\dlls\LibreriaClases.dll

using System;
using System.Net;
using System.Runtime.Serialization;

namespace LibreriaClases
{
  [Serializable]
  public sealed class ArchivoConfig : ISerializable
  {
    public string ImpresoraReportes;
    public string ImpresoraTicket;
    public byte MaskEscape;
    public byte[] MaskEnmascara;
    public byte[] MaskDesenmascara;
    public string LogPath;
    public string LogFileName;
    public int LogMaxFileSize;
    public bool NumeringWithSecuential;
    public EnumMessageType LevelLog;
    public IPAddress IpTerminal;
    public IPAddress IpMask;
    public IPAddress DW;
    public IPAddress DNS;
    public string PathPRN;
    public string ArchivoPRN;
    public IPAddress DefaultServer;
    public string Host;
    public ushort Port;
    public string Telefono;
    public string PCName;
    public IPAddress FTPServer;
    public string FTPUser;
    [NonSerialized]
    public string FTPPassword;
    public byte[] FTPEncryptPassword;
    public int FTPport;
    public string FTPWorkingDirectory;
    [OptionalField(VersionAdded = 2)]
    public string CxnDefault;
    [OptionalField(VersionAdded = 2)]
    public bool CxnDialUpHabilitado;
    [OptionalField(VersionAdded = 2)]
    public bool CxnOfflineHabilitado;
    [OptionalField(VersionAdded = 2)]
    public bool CxnOnlineHabilitado;
    [OptionalField(VersionAdded = 2)]
    public string CxnTelUser;
    [OptionalField(VersionAdded = 2)]
    public string CxnTelPass;
    [OptionalField(VersionAdded = 2)]
    public string CxnTelNombreModem;
    [OptionalField(VersionAdded = 2)]
    public string CxnTelPrefijo;
    [OptionalField(VersionAdded = 2)]
    public uint CxnTelTimeout;
    [OptionalField(VersionAdded = 2)]
    public uint DelayApuesta;
    [OptionalField(VersionAdded = 2)]
    public uint DelayApuestaOff;
    [OptionalField(VersionAdded = 2)]
    public bool ImpresoraLPT;
    [OptionalField(VersionAdded = 2)]
    public short PuertoLPT;
    [OptionalField(VersionAdded = 2)]
    public bool CodigoBarras2de5;
    [OptionalField(VersionAdded = 2)]
    public string FontPath;
    [OptionalField(VersionAdded = 2)]
    public string FontCodigoBarras;
    [OptionalField(VersionAdded = 2)]
    public string XpsLocalDir;
    [OptionalField(VersionAdded = 2)]
    public string XpsExtension;
    [OptionalField(VersionAdded = 2)]
    public short XpsNroMaxArchivos;
    [OptionalField(VersionAdded = 2)]
    public bool MiniTicketOnLineQuiniela;
    [OptionalField(VersionAdded = 2)]
    public bool MiniTicketOffLineQuiniela;
    [OptionalField(VersionAdded = 2)]
    public bool MiniTicketOnLineQuini6;
    [OptionalField(VersionAdded = 2)]
    public bool MiniTicketOffLineQuini6;
    [OptionalField(VersionAdded = 2)]
    public bool MiniTicketOnLineLoto;
    [OptionalField(VersionAdded = 2)]
    public bool MiniTicketOffLineLoto;
    [OptionalField(VersionAdded = 2)]
    public bool MiniTicketOnLineBrinco;
    [OptionalField(VersionAdded = 2)]
    public bool MiniTicketOffLineBrinco;
    [OptionalField(VersionAdded = 2)]
    public bool MiniTicketOnLineAnulacion;
    [OptionalField(VersionAdded = 2)]
    public bool MiniTicketOffLineAnulacion;
    [OptionalField(VersionAdded = 2)]
    public bool MiniTicketOnLineReimpresion;
    [OptionalField(VersionAdded = 2)]
    public bool MiniTicketOffLineReimpresion;
    [OptionalField(VersionAdded = 2)]
    public bool MiniTicketOnLineQuinielaPoceada;
    [OptionalField(VersionAdded = 2)]
    public bool MiniTicketOffLineQuinielaPoceada;
    [OptionalField(VersionAdded = 2)]
    public bool MiniTicketOnLineLoteria;
    [OptionalField(VersionAdded = 2)]
    public bool MiniTicketOffLineLoteria;
    [OptionalField(VersionAdded = 2)]
    public bool MiniTicketOnLinePagoPremios;
    [OptionalField(VersionAdded = 2)]
    public bool MiniTicketOffLinePagoPremios;
    [OptionalField(VersionAdded = 3)]
    public bool MiniTicketOnLineCombinada;
    [OptionalField(VersionAdded = 3)]
    public bool MiniTicketOffLineCombinada;
    [OptionalField(VersionAdded = 3)]
    public bool MiniTicketOnLinePolla;
    [OptionalField(VersionAdded = 3)]
    public bool MiniTicketOffLinePolla;
    [OptionalField(VersionAdded = 2)]
    public string UnidadUsb;
    [OptionalField(VersionAdded = 2)]
    public string RutaSorteosOffTML;
    [OptionalField(VersionAdded = 2)]
    public string RutaSorteosOffUSB;
    [OptionalField(VersionAdded = 2)]
    public string SorteosOffArchivos;
    [OptionalField(VersionAdded = 2)]
    public bool EscQuiniela;
    [OptionalField(VersionAdded = 2)]
    public string NombrePuertoRadio;
    [OptionalField(VersionAdded = 2)]
    public int BaudRateRadio;
    [OptionalField(VersionAdded = 2)]
    public byte ParityRadio;
    [OptionalField(VersionAdded = 2)]
    public byte DataBitsRadio;
    [OptionalField(VersionAdded = 2)]
    public byte StopBitsRadio;
    [OptionalField(VersionAdded = 2)]
    public int WriteTimeOutRadio;
    [OptionalField(VersionAdded = 2)]
    public int ReadTimeOutRadio;
    [OptionalField(VersionAdded = 2)]
    public int ReadBufferSizeRadio;
    [OptionalField(VersionAdded = 2)]
    public int WriteBufferSizeRadio;
    [OptionalField(VersionAdded = 2)]
    public byte HandshakeRadio;
    [OptionalField(VersionAdded = 2)]
    public string NombrePuertoVisor;
    [OptionalField(VersionAdded = 2)]
    public int BaudRateVisor;
    [OptionalField(VersionAdded = 2)]
    public byte DataBitsVisor;
    [OptionalField(VersionAdded = 2)]
    public EnumImpresoraTicket ImpresoraTicketModelo;
    [OptionalField(VersionAdded = 2)]
    public EnumModoConexion ConexionDefault;
    [OptionalField(VersionAdded = 2)]
    public byte Version;

    public ArchivoConfig()
    {
      this.ImpresoraReportes = "";
      this.ImpresoraTicket = "";
      this.MaskEnmascara = new byte[8];
      this.MaskDesenmascara = new byte[8];
      this.LogPath = "";
      this.LogFileName = "";
      this.LogMaxFileSize = 0;
      this.NumeringWithSecuential = true;
      this.LevelLog = EnumMessageType.NORMAL;
      this.IpTerminal = IPAddress.Parse("0.0.0.0");
      this.IpMask = IPAddress.Parse("0.0.0.0");
      this.DW = IPAddress.Parse("0.0.0.0");
      this.DNS = IPAddress.Parse("0.0.0.0");
      this.PathPRN = "";
      this.ArchivoPRN = "";
      this.DefaultServer = IPAddress.Parse("0.0.0.0");
      this.Host = "";
      this.Port = (ushort) 0;
      this.Telefono = "";
      this.PCName = "";
      this.FTPServer = IPAddress.Parse("0.0.0.0");
      this.FTPUser = "";
      this.FTPport = 0;
      this.FTPPassword = "";
      this.FTPEncryptPassword = new byte[1];
      this.FTPWorkingDirectory = "";
      this.CxnDefault = "";
      this.CxnDialUpHabilitado = false;
      this.CxnOfflineHabilitado = false;
      this.CxnOnlineHabilitado = false;
      this.CxnTelUser = "";
      this.CxnTelPass = "";
      this.CxnTelNombreModem = "";
      this.CxnTelPrefijo = "";
      this.CxnTelTimeout = 0U;
      this.DelayApuesta = 0U;
      this.DelayApuestaOff = 0U;
      this.ImpresoraLPT = false;
      this.PuertoLPT = (short) 0;
      this.CodigoBarras2de5 = false;
      this.FontPath = "";
      this.FontCodigoBarras = "";
      this.XpsLocalDir = "";
      this.XpsExtension = "";
      this.XpsNroMaxArchivos = (short) 0;
      this.MiniTicketOnLineQuiniela = false;
      this.MiniTicketOffLineQuiniela = false;
      this.MiniTicketOnLineQuini6 = false;
      this.MiniTicketOffLineQuini6 = false;
      this.MiniTicketOnLineLoto = false;
      this.MiniTicketOffLineLoto = false;
      this.MiniTicketOnLineBrinco = false;
      this.MiniTicketOffLineBrinco = false;
      this.MiniTicketOnLineAnulacion = false;
      this.MiniTicketOffLineAnulacion = false;
      this.MiniTicketOnLineReimpresion = false;
      this.MiniTicketOffLineReimpresion = false;
      this.MiniTicketOnLineLoteria = false;
      this.MiniTicketOffLineLoteria = false;
      this.MiniTicketOffLineQuinielaPoceada = false;
      this.MiniTicketOnLineQuinielaPoceada = false;
      this.MiniTicketOffLinePagoPremios = false;
      this.MiniTicketOnLinePagoPremios = false;
      this.MiniTicketOffLineCombinada = false;
      this.MiniTicketOnLineCombinada = false;
      this.MiniTicketOffLinePolla = false;
      this.MiniTicketOnLinePolla = false;
      this.UnidadUsb = "";
      this.RutaSorteosOffTML = "";
      this.RutaSorteosOffUSB = "";
      this.SorteosOffArchivos = "";
      this.EscQuiniela = false;
      this.NombrePuertoRadio = "COM2";
      this.BaudRateRadio = 9600;
      this.ParityRadio = (byte) 0;
      this.DataBitsRadio = (byte) 8;
      this.StopBitsRadio = (byte) 1;
      this.WriteTimeOutRadio = 16000;
      this.ReadTimeOutRadio = 16000;
      this.ReadBufferSizeRadio = 1024;
      this.WriteBufferSizeRadio = 1024;
      this.HandshakeRadio = (byte) 2;
      this.NombrePuertoVisor = "COM3";
      this.BaudRateVisor = 9600;
      this.DataBitsVisor = (byte) 8;
      this.ImpresoraTicketModelo = EnumImpresoraTicket.RP80PRINTER;
      this.ConexionDefault = EnumModoConexion.ETHERNET;
      this.Version = (byte) 0;
    }

    public void GetDefaults(ref ArchivoConfig configDef)
    {
      configDef.CxnDefault = "Online";
      configDef.CxnDialUpHabilitado = false;
      configDef.CxnOfflineHabilitado = true;
      configDef.CxnOnlineHabilitado = true;
      configDef.CxnTelUser = "bmtp";
      configDef.CxnTelPass = "bmtp";
      configDef.CxnTelNombreModem = "Conexant USB CX93010 ACF Modem";
      configDef.CxnTelPrefijo = "";
      configDef.CxnTelTimeout = 4700U;
      configDef.DelayApuesta = 0U;
      configDef.DelayApuestaOff = 0U;
      configDef.ImpresoraLPT = true;
      configDef.PuertoLPT = (short) 889;
      configDef.CodigoBarras2de5 = true;
      configDef.FontPath = "C:\\WINDOWS\\Fonts\\";
      configDef.FontCodigoBarras = "FREE3OF9.TTF";
      configDef.XpsLocalDir = "C:\\BetmakerTP\\ReportesXPS";
      configDef.XpsExtension = ".xps";
      configDef.XpsNroMaxArchivos = (short) 12;
      configDef.MiniTicketOnLineQuiniela = false;
      configDef.MiniTicketOffLineQuiniela = true;
      configDef.MiniTicketOnLineQuini6 = false;
      configDef.MiniTicketOffLineQuini6 = true;
      configDef.MiniTicketOnLineLoto = false;
      configDef.MiniTicketOffLineLoto = true;
      configDef.MiniTicketOnLineBrinco = false;
      configDef.MiniTicketOffLineBrinco = true;
      configDef.MiniTicketOnLineAnulacion = false;
      configDef.MiniTicketOffLineAnulacion = true;
      configDef.MiniTicketOnLineReimpresion = false;
      configDef.MiniTicketOffLineReimpresion = true;
      configDef.MiniTicketOnLineQuinielaPoceada = true;
      configDef.MiniTicketOffLineQuinielaPoceada = true;
      configDef.MiniTicketOffLineLoteria = true;
      configDef.MiniTicketOnLineLoteria = true;
      configDef.MiniTicketOnLinePagoPremios = true;
      configDef.MiniTicketOffLinePagoPremios = false;
      configDef.MiniTicketOnLinePolla = false;
      configDef.MiniTicketOffLinePolla = true;
      configDef.MiniTicketOffLineCombinada = true;
      configDef.MiniTicketOnLineCombinada = false;
      configDef.UnidadUsb = "D:\\";
      configDef.RutaSorteosOffTML = "C:\\BetmakerTP\\Off";
      configDef.RutaSorteosOffUSB = "D:";
      configDef.SorteosOffArchivos = "*.bin";
      configDef.EscQuiniela = true;
      configDef.NombrePuertoRadio = "COM2";
      configDef.BaudRateRadio = 9600;
      configDef.ParityRadio = (byte) 0;
      configDef.DataBitsRadio = (byte) 8;
      configDef.StopBitsRadio = (byte) 1;
      configDef.WriteTimeOutRadio = 16000;
      configDef.ReadTimeOutRadio = 16000;
      configDef.ReadBufferSizeRadio = 1024;
      configDef.WriteBufferSizeRadio = 1024;
      configDef.HandshakeRadio = (byte) 2;
      configDef.NombrePuertoVisor = "COM3";
      configDef.BaudRateVisor = 9600;
      configDef.DataBitsVisor = (byte) 8;
      this.ImpresoraTicketModelo = EnumImpresoraTicket.RP80PRINTER;
      this.ConexionDefault = EnumModoConexion.ETHERNET;
      this.Version = (byte) 3;
    }

    [OnDeserializing]
    private void SetDefault(StreamingContext ctxt)
    {
      this.CxnDefault = "Online";
      this.CxnDialUpHabilitado = false;
      this.CxnOfflineHabilitado = true;
      this.CxnOnlineHabilitado = true;
      this.CxnTelUser = "bmtp";
      this.CxnTelPass = "bmtp";
      this.CxnTelNombreModem = "Conexant USB CX93010 ACF Modem";
      this.CxnTelPrefijo = "";
      this.CxnTelTimeout = 4700U;
      this.DelayApuesta = 0U;
      this.DelayApuestaOff = 0U;
      this.ImpresoraLPT = true;
      this.PuertoLPT = (short) 889;
      this.CodigoBarras2de5 = true;
      this.FontPath = "C:\\WINDOWS\\Fonts\\";
      this.FontCodigoBarras = "FREE3OF9.TTF";
      this.XpsLocalDir = "C:\\BetmakerTP\\ReportesXPS";
      this.XpsExtension = ".xps";
      this.XpsNroMaxArchivos = (short) 12;
      this.MiniTicketOnLineQuiniela = false;
      this.MiniTicketOffLineQuiniela = true;
      this.MiniTicketOnLineQuini6 = false;
      this.MiniTicketOffLineQuini6 = true;
      this.MiniTicketOnLineLoto = false;
      this.MiniTicketOffLineLoto = true;
      this.MiniTicketOnLineBrinco = false;
      this.MiniTicketOffLineBrinco = true;
      this.MiniTicketOnLineAnulacion = false;
      this.MiniTicketOffLineAnulacion = true;
      this.MiniTicketOnLineReimpresion = false;
      this.MiniTicketOffLineReimpresion = true;
      this.MiniTicketOnLineQuinielaPoceada = true;
      this.MiniTicketOffLineQuinielaPoceada = true;
      this.MiniTicketOffLineLoteria = true;
      this.MiniTicketOnLineLoteria = true;
      this.MiniTicketOffLinePagoPremios = false;
      this.MiniTicketOnLinePagoPremios = true;
      this.MiniTicketOffLineCombinada = true;
      this.MiniTicketOnLineCombinada = false;
      this.MiniTicketOnLinePolla = false;
      this.MiniTicketOffLinePolla = true;
      this.UnidadUsb = "D:\\";
      this.RutaSorteosOffTML = "C:\\BetmakerTP\\Off";
      this.RutaSorteosOffUSB = "D:";
      this.SorteosOffArchivos = "*.bin";
      this.EscQuiniela = true;
      this.NombrePuertoRadio = "COM2";
      this.BaudRateRadio = 9600;
      this.ParityRadio = (byte) 0;
      this.DataBitsRadio = (byte) 8;
      this.StopBitsRadio = (byte) 1;
      this.WriteTimeOutRadio = 16000;
      this.ReadTimeOutRadio = 16000;
      this.ReadBufferSizeRadio = 1024;
      this.WriteBufferSizeRadio = 1024;
      this.HandshakeRadio = (byte) 2;
      this.NombrePuertoVisor = "COM3";
      this.BaudRateVisor = 9600;
      this.DataBitsVisor = (byte) 8;
      this.ImpresoraTicketModelo = EnumImpresoraTicket.RP80PRINTER;
      this.ConexionDefault = EnumModoConexion.ETHERNET;
      this.Version = (byte) 3;
    }

    public ArchivoConfig(SerializationInfo info, StreamingContext ctxt)
    {
      this.ImpresoraReportes = (string) info.GetValue(nameof (ImpresoraReportes), typeof (string));
      this.ImpresoraTicket = (string) info.GetValue(nameof (ImpresoraTicket), typeof (string));
      this.MaskEscape = (byte) info.GetValue(nameof (MaskEscape), typeof (byte));
      this.MaskEnmascara = (byte[]) info.GetValue(nameof (MaskEnmascara), typeof (byte[]));
      this.MaskDesenmascara = (byte[]) info.GetValue(nameof (MaskDesenmascara), typeof (byte[]));
      this.LogPath = (string) info.GetValue(nameof (LogPath), typeof (string));
      this.LogFileName = (string) info.GetValue(nameof (LogFileName), typeof (string));
      this.LogMaxFileSize = (int) info.GetValue(nameof (LogMaxFileSize), typeof (int));
      this.NumeringWithSecuential = (bool) info.GetValue(nameof (NumeringWithSecuential), typeof (bool));
      this.LevelLog = (EnumMessageType) info.GetValue(nameof (LevelLog), typeof (EnumMessageType));
      this.IpTerminal = (IPAddress) info.GetValue(nameof (IpTerminal), typeof (IPAddress));
      this.IpMask = (IPAddress) info.GetValue("Mask", typeof (IPAddress));
      this.DW = (IPAddress) info.GetValue(nameof (DW), typeof (IPAddress));
      this.DNS = (IPAddress) info.GetValue(nameof (DNS), typeof (IPAddress));
      this.PathPRN = (string) info.GetValue(nameof (PathPRN), typeof (string));
      this.ArchivoPRN = (string) info.GetValue(nameof (ArchivoPRN), typeof (string));
      this.DefaultServer = (IPAddress) info.GetValue(nameof (DefaultServer), typeof (IPAddress));
      this.Host = (string) info.GetValue(nameof (Host), typeof (string));
      this.Port = (ushort) info.GetValue(nameof (Port), typeof (ushort));
      this.Telefono = (string) info.GetValue(nameof (Telefono), typeof (string));
      this.PCName = (string) info.GetValue(nameof (PCName), typeof (string));
      this.FTPServer = (IPAddress) info.GetValue(nameof (FTPServer), typeof (IPAddress));
      this.FTPport = (int) info.GetValue(nameof (FTPport), typeof (int));
      this.FTPUser = (string) info.GetValue(nameof (FTPUser), typeof (string));
      this.FTPEncryptPassword = (byte[]) info.GetValue(nameof (FTPEncryptPassword), typeof (byte[]));
      this.FTPWorkingDirectory = (string) info.GetValue(nameof (FTPWorkingDirectory), typeof (string));
      try
      {
        this.CxnDefault = (string) info.GetValue(nameof (CxnDefault), typeof (string));
        this.CxnOnlineHabilitado = (bool) info.GetValue(nameof (CxnOnlineHabilitado), typeof (bool));
        this.CxnOfflineHabilitado = (bool) info.GetValue(nameof (CxnOfflineHabilitado), typeof (bool));
        this.CxnDialUpHabilitado = (bool) info.GetValue(nameof (CxnDialUpHabilitado), typeof (bool));
        this.CxnTelUser = (string) info.GetValue(nameof (CxnTelUser), typeof (string));
        this.CxnTelPass = (string) info.GetValue(nameof (CxnTelPass), typeof (string));
        this.CxnTelNombreModem = (string) info.GetValue(nameof (CxnTelNombreModem), typeof (string));
        this.CxnTelPrefijo = (string) info.GetValue(nameof (CxnTelPrefijo), typeof (string));
        this.CxnTelTimeout = (uint) info.GetValue(nameof (CxnTelTimeout), typeof (uint));
        this.DelayApuesta = (uint) info.GetValue(nameof (DelayApuesta), typeof (uint));
        this.DelayApuestaOff = (uint) info.GetValue(nameof (DelayApuestaOff), typeof (uint));
        this.ImpresoraLPT = (bool) info.GetValue(nameof (ImpresoraLPT), typeof (bool));
        this.PuertoLPT = (short) info.GetValue(nameof (PuertoLPT), typeof (short));
        this.CodigoBarras2de5 = (bool) info.GetValue(nameof (CodigoBarras2de5), typeof (bool));
        this.FontPath = (string) info.GetValue(nameof (FontPath), typeof (string));
        this.FontCodigoBarras = (string) info.GetValue(nameof (FontCodigoBarras), typeof (string));
        this.XpsLocalDir = (string) info.GetValue(nameof (XpsLocalDir), typeof (string));
        this.XpsExtension = (string) info.GetValue(nameof (XpsExtension), typeof (string));
        this.XpsNroMaxArchivos = (short) info.GetValue(nameof (XpsNroMaxArchivos), typeof (short));
        this.MiniTicketOnLineQuiniela = (bool) info.GetValue(nameof (MiniTicketOnLineQuiniela), typeof (bool));
        this.MiniTicketOffLineQuiniela = (bool) info.GetValue(nameof (MiniTicketOffLineQuiniela), typeof (bool));
        this.MiniTicketOnLineQuini6 = (bool) info.GetValue(nameof (MiniTicketOnLineQuini6), typeof (bool));
        this.MiniTicketOffLineQuini6 = (bool) info.GetValue(nameof (MiniTicketOffLineQuini6), typeof (bool));
        this.MiniTicketOnLineLoto = (bool) info.GetValue(nameof (MiniTicketOnLineLoto), typeof (bool));
        this.MiniTicketOffLineLoto = (bool) info.GetValue(nameof (MiniTicketOffLineLoto), typeof (bool));
        this.MiniTicketOnLineBrinco = (bool) info.GetValue(nameof (MiniTicketOnLineBrinco), typeof (bool));
        this.MiniTicketOffLineBrinco = (bool) info.GetValue(nameof (MiniTicketOffLineBrinco), typeof (bool));
        this.MiniTicketOnLineAnulacion = (bool) info.GetValue(nameof (MiniTicketOnLineAnulacion), typeof (bool));
        this.MiniTicketOffLineAnulacion = (bool) info.GetValue(nameof (MiniTicketOffLineAnulacion), typeof (bool));
        this.MiniTicketOnLineReimpresion = (bool) info.GetValue(nameof (MiniTicketOnLineReimpresion), typeof (bool));
        this.MiniTicketOffLineReimpresion = (bool) info.GetValue(nameof (MiniTicketOffLineReimpresion), typeof (bool));
        this.MiniTicketOffLineLoteria = (bool) info.GetValue(nameof (MiniTicketOffLineLoteria), typeof (bool));
        this.MiniTicketOnLineLoteria = (bool) info.GetValue(nameof (MiniTicketOnLineLoteria), typeof (bool));
        this.MiniTicketOffLineQuinielaPoceada = (bool) info.GetValue(nameof (MiniTicketOffLineQuinielaPoceada), typeof (bool));
        this.MiniTicketOnLineQuinielaPoceada = (bool) info.GetValue(nameof (MiniTicketOnLineQuinielaPoceada), typeof (bool));
        this.UnidadUsb = (string) info.GetValue(nameof (UnidadUsb), typeof (string));
        this.RutaSorteosOffTML = (string) info.GetValue(nameof (RutaSorteosOffTML), typeof (string));
        this.RutaSorteosOffUSB = (string) info.GetValue(nameof (RutaSorteosOffUSB), typeof (string));
        this.SorteosOffArchivos = (string) info.GetValue(nameof (SorteosOffArchivos), typeof (string));
        this.EscQuiniela = (bool) info.GetValue(nameof (EscQuiniela), typeof (bool));
        this.NombrePuertoRadio = (string) info.GetValue(nameof (NombrePuertoRadio), typeof (string));
        this.BaudRateRadio = (int) info.GetValue(nameof (BaudRateRadio), typeof (int));
        this.ParityRadio = (byte) info.GetValue(nameof (ParityRadio), typeof (byte));
        this.DataBitsRadio = (byte) info.GetValue(nameof (DataBitsRadio), typeof (byte));
        this.StopBitsRadio = (byte) info.GetValue(nameof (StopBitsRadio), typeof (byte));
        this.WriteTimeOutRadio = (int) info.GetValue(nameof (WriteTimeOutRadio), typeof (int));
        this.ReadTimeOutRadio = (int) info.GetValue(nameof (ReadTimeOutRadio), typeof (int));
        this.ReadBufferSizeRadio = (int) info.GetValue(nameof (ReadBufferSizeRadio), typeof (int));
        this.WriteBufferSizeRadio = (int) info.GetValue(nameof (WriteBufferSizeRadio), typeof (int));
        this.HandshakeRadio = (byte) info.GetValue(nameof (HandshakeRadio), typeof (byte));
        this.NombrePuertoVisor = (string) info.GetValue(nameof (NombrePuertoVisor), typeof (string));
        this.BaudRateVisor = (int) info.GetValue(nameof (BaudRateVisor), typeof (int));
        this.DataBitsVisor = (byte) info.GetValue(nameof (DataBitsVisor), typeof (byte));
        this.ImpresoraTicketModelo = (EnumImpresoraTicket) info.GetValue(nameof (ImpresoraTicketModelo), typeof (EnumImpresoraTicket));
        this.ConexionDefault = (EnumModoConexion) info.GetValue(nameof (ConexionDefault), typeof (EnumModoConexion));
        this.Version = (byte) info.GetValue(nameof (Version), typeof (byte));
        this.MiniTicketOnLinePagoPremios = (bool) info.GetValue(nameof (MiniTicketOnLinePagoPremios), typeof (bool));
        this.MiniTicketOffLinePagoPremios = (bool) info.GetValue(nameof (MiniTicketOffLinePagoPremios), typeof (bool));
        this.MiniTicketOnLinePolla = (bool) info.GetValue(nameof (MiniTicketOnLinePolla), typeof (bool));
        this.MiniTicketOffLinePolla = (bool) info.GetValue(nameof (MiniTicketOffLinePolla), typeof (bool));
        this.MiniTicketOffLineCombinada = (bool) info.GetValue(nameof (MiniTicketOffLineCombinada), typeof (bool));
        this.MiniTicketOnLineCombinada = (bool) info.GetValue(nameof (MiniTicketOnLineCombinada), typeof (bool));
      }
      catch (SerializationException ex)
      {
      }
    }

    public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
    {
      info.AddValue("ImpresoraReportes", (object) this.ImpresoraReportes);
      info.AddValue("ImpresoraTicket", (object) this.ImpresoraTicket);
      info.AddValue("MaskEscape", this.MaskEscape);
      info.AddValue("MaskEnmascara", (object) this.MaskEnmascara);
      info.AddValue("MaskDesenmascara", (object) this.MaskDesenmascara);
      info.AddValue("LogPath", (object) this.LogPath);
      info.AddValue("LogFileName", (object) this.LogFileName);
      info.AddValue("LogMaxFileSize", this.LogMaxFileSize);
      info.AddValue("NumeringWithSecuential", this.NumeringWithSecuential);
      info.AddValue("LevelLog", (object) this.LevelLog);
      info.AddValue("IpTerminal", (object) this.IpTerminal);
      info.AddValue("Mask", (object) this.IpMask);
      info.AddValue("DW", (object) this.DW);
      info.AddValue("DNS", (object) this.DNS);
      info.AddValue("PathPRN", (object) this.PathPRN);
      info.AddValue("ArchivoPRN", (object) this.ArchivoPRN);
      info.AddValue("DefaultServer", (object) this.DefaultServer);
      info.AddValue("Host", (object) this.Host);
      info.AddValue("Port", this.Port);
      info.AddValue("Telefono", (object) this.Telefono);
      info.AddValue("PCName", (object) this.PCName);
      info.AddValue("FTPServer", (object) this.FTPServer);
      info.AddValue("FTPport", this.FTPport);
      info.AddValue("FTPUser", (object) this.FTPUser);
      info.AddValue("FTPEncryptPassword", (object) this.FTPEncryptPassword);
      info.AddValue("FTPWorkingDirectory", (object) this.FTPWorkingDirectory);
      info.AddValue("CxnDefault", (object) this.CxnDefault);
      info.AddValue("CxnDialUpHabilitado", this.CxnDialUpHabilitado);
      info.AddValue("CxnOfflineHabilitado", this.CxnOfflineHabilitado);
      info.AddValue("CxnOnlineHabilitado", this.CxnOnlineHabilitado);
      info.AddValue("CxnTelUser", (object) this.CxnTelUser);
      info.AddValue("CxnTelPass", (object) this.CxnTelPass);
      info.AddValue("CxnTelNombreModem", (object) this.CxnTelNombreModem);
      info.AddValue("CxnTelPrefijo", (object) this.CxnTelPrefijo);
      info.AddValue("CxnTelTimeout", this.CxnTelTimeout);
      info.AddValue("DelayApuesta", this.DelayApuesta);
      info.AddValue("DelayApuestaOff", this.DelayApuestaOff);
      info.AddValue("ImpresoraLPT", this.ImpresoraLPT);
      info.AddValue("PuertoLPT", this.PuertoLPT);
      info.AddValue("CodigoBarras2de5", this.CodigoBarras2de5);
      info.AddValue("FontPath", (object) this.FontPath);
      info.AddValue("FontCodigoBarras", (object) this.FontCodigoBarras);
      info.AddValue("XpsLocalDir", (object) this.XpsLocalDir);
      info.AddValue("XpsExtension", (object) this.XpsExtension);
      info.AddValue("XpsNroMaxArchivos", this.XpsNroMaxArchivos);
      info.AddValue("MiniTicketOnLineQuiniela", this.MiniTicketOnLineQuiniela);
      info.AddValue("MiniTicketOffLineQuiniela", this.MiniTicketOffLineQuiniela);
      info.AddValue("MiniTicketOnLineQuini6", this.MiniTicketOnLineQuini6);
      info.AddValue("MiniTicketOffLineQuini6", this.MiniTicketOffLineQuini6);
      info.AddValue("MiniTicketOnLineLoto", this.MiniTicketOnLineLoto);
      info.AddValue("MiniTicketOffLineLoto", this.MiniTicketOffLineLoto);
      info.AddValue("MiniTicketOnLineBrinco", this.MiniTicketOnLineBrinco);
      info.AddValue("MiniTicketOffLineBrinco", this.MiniTicketOffLineBrinco);
      info.AddValue("MiniTicketOnLineAnulacion", this.MiniTicketOnLineAnulacion);
      info.AddValue("MiniTicketOffLineAnulacion", this.MiniTicketOffLineAnulacion);
      info.AddValue("MiniTicketOnLineReimpresion", this.MiniTicketOnLineReimpresion);
      info.AddValue("MiniTicketOffLineReimpresion", this.MiniTicketOffLineReimpresion);
      info.AddValue("MiniTicketOnLineQuinielaPoceada", this.MiniTicketOnLineQuinielaPoceada);
      info.AddValue("MiniTicketOffLineQuinielaPoceada", this.MiniTicketOffLineQuinielaPoceada);
      info.AddValue("MiniTicketOnLineLoteria", this.MiniTicketOnLineLoteria);
      info.AddValue("MiniTicketOffLineLoteria", this.MiniTicketOffLineLoteria);
      info.AddValue("UnidadUsb", (object) this.UnidadUsb);
      info.AddValue("RutaSorteosOffTML", (object) this.RutaSorteosOffTML);
      info.AddValue("RutaSorteosOffUSB", (object) this.RutaSorteosOffUSB);
      info.AddValue("SorteosOffArchivos", (object) this.SorteosOffArchivos);
      info.AddValue("EscQuiniela", this.EscQuiniela);
      info.AddValue("NombrePuertoRadio", (object) this.NombrePuertoRadio);
      info.AddValue("BaudRateRadio", this.BaudRateRadio);
      info.AddValue("ParityRadio", this.ParityRadio);
      info.AddValue("DataBitsRadio", this.DataBitsRadio);
      info.AddValue("StopBitsRadio", this.StopBitsRadio);
      info.AddValue("WriteTimeOutRadio", this.WriteTimeOutRadio);
      info.AddValue("ReadTimeOutRadio", this.ReadTimeOutRadio);
      info.AddValue("ReadBufferSizeRadio", this.ReadBufferSizeRadio);
      info.AddValue("WriteBufferSizeRadio", this.WriteBufferSizeRadio);
      info.AddValue("HandshakeRadio", this.HandshakeRadio);
      info.AddValue("NombrePuertoVisor", (object) this.NombrePuertoVisor);
      info.AddValue("BaudRateVisor", this.BaudRateVisor);
      info.AddValue("DataBitsVisor", this.DataBitsVisor);
      info.AddValue("ImpresoraTicketModelo", (object) this.ImpresoraTicketModelo);
      info.AddValue("ConexionDefault", (object) this.ConexionDefault);
      info.AddValue("Version", this.Version);
      info.AddValue("MiniTicketOffLinePagoPremios", this.MiniTicketOffLinePagoPremios);
      info.AddValue("MiniTicketOnLinePagoPremios", this.MiniTicketOnLinePagoPremios);
      info.AddValue("MiniTicketOnLineCombinada", this.MiniTicketOnLineCombinada);
      info.AddValue("MiniTicketOffLineCombinada", this.MiniTicketOffLineCombinada);
      info.AddValue("MiniTicketOffLinePolla", this.MiniTicketOffLinePolla);
      info.AddValue("MiniTicketOnLinePolla", this.MiniTicketOnLinePolla);
    }
  }
}
