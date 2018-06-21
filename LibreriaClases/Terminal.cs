// Decompiled with JetBrains decompiler
// Type: LibreriaClases.Clases.Terminal
// Assembly: LibreriaClases, Version=1.0.14.0, Culture=neutral, PublicKeyToken=null
// MVID: 50EE79A0-EB27-4448-9AD7-1911792168AB
// Assembly location: C:\SERVICIOS\dlls\LibreriaClases.dll

using System;
using System.Diagnostics;
using System.Net;

namespace LibreriaClases.Clases
{
  public class Terminal
  {
    public uint NumeroTerminal;
    public uint Tarjeta;
    public byte[] MacTarjeta;
    public DateTime FechaHora;
    public ushort Version;
    public FileVersionInfo FileVersion;
    public IPAddress IP;
    public EnumTerminalModelo Tipo;
    public string nroRollo;
    public byte indiceTelecarga;
    public uint longArchivo;
    public byte correo;
    public uint defLengConc1;
    public uint defLengConc2;
    public string statusConc;
    public System.Version assemblyVersion;
  }
}
