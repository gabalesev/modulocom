﻿using LibreriaClases;
using LibreriaClases.Clases;
using LibreriaRegistro;
using LibreriaMetodologia;
using System;
using System.Collections;
using System.Collections.Generic;

namespace LibreriaModuloTransaccional
{
    partial class Comunicacion
    {
        // TRANSACCIONES
        public IList InteraccionAB(ref Terminal datosA, bool interno = false)
        {
            GestorArchivoConfiguracionConexion cxn = new GestorArchivoConfiguracionConexion();
            IList objs = new List<object>();
            try
            {
                GestorTransacciones.ordenAckE = 0;
                GestorTransacciones.ordenMsgNackE = 0;
                GestorTransacciones.ConfiguracionComunicacion.NackEnvio = NackEnv.SINERROR;

                datosA.Tipo = GestorTransacciones.ConfiguracionComunicacion.BASE_CONFIG.TerminalModelo;

                Enviar(datosA, EnumPaquete.DATOS, GestorTransacciones.ordenMsgNackE);

                GestorTransacciones.ordenMsgNackE++;
                Errorof errOf = new Errorof();

                //Recibo Mensaje B y envío ACK o NACK
                do
                {
                    var bytes = new byte[GestorTransacciones.ConfiguracionComunicacion.LongitudBuffer];
                    objs = Recibir(bytes, 2, 0, "B");

                    //atrapa errores de recepción
                    if (objs.Count > 1 && !(objs[0] is string))
                    {
                        Enviar(null, EnumPaquete.ACK, GestorTransacciones.ordenAckE);
                    }
                    else
                    {
                        break;
                    }

                }
                while (GestorTransacciones.ConfiguracionComunicacion.NackEnvio != NackEnv.SINERROR);

                if (objs == null || objs.Count == 0)
                {
                    throw new Exception("Lista de objetos(objs) regreso vacía de Recibir()");
                }
                else if (objs[0] is string)
                {
                    Error err = MensajeNack(objs[0]);
                    objs.Insert(0, err);
                }
                else if (objs[0].GetType() == typeof(Error) && ((Error)objs[0]).CodError != 0)
                {
                    foreach (Object obj in objs)
                    {
                        if (obj is PRN)
                        {
                            cxn.Crear_XMLprn(((PRN)obj).Nombre1, ((PRN)obj).Nombre2, ((PRN)obj).Nombre3, ((PRN)obj).Port1, ((PRN)obj).Port2, ((PRN)obj).Port3,
                                ((PRN)obj).Telefono1, ((PRN)obj).Telefono2, ((PRN)obj).Telefono3, GestorTransacciones.ConfiguracionComunicacion.Configuracion);
                        }
                        else if (obj is Error)
                            ((Error)objs[0]).Estado = 0;
                    }
                    return objs;
                }
                else if (objs[0] is Error && ((Error)objs[0]).CodError == 0 && objs[1] is Agente)
                {
                    Agente agente = (Agente)objs[1];
                    Agente agen = new Agente(agente, out errOf);
                    if (errOf.Error != 0)
                    {
                        ((Error)objs[0]).CodError = (uint)errOf.Error;
                        ((Error)objs[0]).Descripcion = errOf.Mensaje;
                        ((Error)objs[0]).Estado = 2;
                    }
                    PRN prn = (PRN)objs[2];

                    Terminal terminal = (Terminal)objs[3];
                    datosA.indiceTelecarga = terminal.indiceTelecarga;
                    datosA.longArchivo = terminal.longArchivo;
                    datosA.correo = terminal.correo;
                    datosA.defLengConc1 = terminal.defLengConc1;
                    datosA.defLengConc2 = terminal.defLengConc2;
                    datosA.statusConc = terminal.statusConc;

                    if (interno)
                    {
                        cxn.Crear_XMLprn(prn.Nombre1, prn.Nombre2, prn.Nombre3, prn.Port1, prn.Port2, prn.Port3, prn.Telefono1, prn.Telefono2, prn.Telefono3, GestorTransacciones.ConfiguracionComunicacion.Configuracion);
                        Desconectar(interno);
                        return objs;
                    }
                    else
                    {
                        cxn.Crear_XMLprn(prn.Nombre1, prn.Nombre2, prn.Nombre3, prn.Port1, prn.Port2, prn.Port3, prn.Telefono1, prn.Telefono2, prn.Telefono3, GestorTransacciones.ConfiguracionComunicacion.Configuracion);
                    }
                }

                return objs;
            }

            catch (Exception ex)
            {
                Error err = new Error();
                err.CodError = (int)ErrComunicacion.LOGIN;
                err.Descripcion = "Ocurrió un error en la comunicación al intentar enviar credenciales."; //Definir que descripción pasar con Jorge
                err.Estado = 0;

                ModuloDeRegistro.RegistrarMensaje("Excepción: " + err.CodError + " " + err.Descripcion, NivelRegistroExcepciones, REGISTRAR_HORA);
                ModuloDeRegistro.RegistrarMensaje("Excepción: " + ex.Message, NivelRegistroDebug, REGISTRAR_HORA);

                objs.Insert(0, err);
                if (objs.Count > 1)
                    objs.RemoveAt(1);

                return objs;
            }
        }
        /*public IList InteraccionPQ1(PedidosSorteos sort, UInt32 terminal, EnumEstadoParametrosOff estadoParametrosOff, string path = @"C:\BetmakerTP\Off")
        {
            try
            {
                ModuloDeRegistro.RegistrarMensaje("Pedido de sorteo: " + sort + " Terminal: " + terminal, lvlLogTransaccion, TimeStampLog);

                Enviar(ConstructorMenEnv.crearP_PedidoSorteo((byte)sort, terminal), EnumPaquete.DATOS, TR.ordenMsgNackE);

                TR.ordenMsgNackE++;

                Errorof errOf = new Errorof();

                IList objs = new List<object>();
                bytes = new byte[ProtocoloConfig.TamBuffer];

                do
                {
                    ModuloDeRegistro.RegistrarMensaje("Parametros " + sort, lvlLogTransaccion, TimeStampLog);
                    objs = Recibir(bytes, (ushort)EnumPaquete.DATOS, 0, "Q1");

                    //atrapa errores de recepción
                    if (objs.Count > 1 && !(objs[0] is string))
                    {
                        byte[] ACK = { };
                        Enviar(ACK, EnumPaquete.ACK, TR.ordenAckE);
                    }
                    else
                    {
                        break;
                    }
                }
                while (!TransacManager.ProtoConfig.NACK_ENV.Equals(NackEnv.SINERROR));

                if (objs == null || objs.Count == 0)
                {
                    throw new Exception("Lista de objetos(objs) regreso vacía de Recibir()");
                }
                else if (objs[0] is string)
                {
                    Error err = MensajeNack(objs[0]);
                    objs[0] = err;
                }
                return objs;
            }
            catch (Exception ex)
            {
                Error errEx = EmpalmeErrorExcepcion(ex, ErrComunicacion.PEDIDO_SORTEOex, "Ha ocurrido un problema durante la descarga de parametros.");
                return new List<object> { errEx };
            }
        }*/
        
    }
}
