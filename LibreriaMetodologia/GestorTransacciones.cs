using LibreriaClases;
using LibreriaClases.Clases;
using LibreriaRegistro;
using LibreriaConversiones;
using MonoLibrary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibreriaMetodologia
{
    public class GestorTransacciones
    {
        #region Propiedades

        // SECUENCIAS ACK Y NACK A ENVIAR
        public ushort ordenAckE = 0, ordenMsgNackE = 0;

        // SECUENCIAS ACK Y NACK A RECIBIR
        public ushort ordenAckR = 0, ordenMsgNackR = 0;

        public EnumPaquete TipoPaqueteEntrada { get; private set; }
        public EnumPaquete TipoPaqueteSalida { get; private set; }

        public int EntradaLength { get; private set; }
        public int SalidaLength { get; private set; }

        public static ConfiguracionComunicacion ConfiguracionComunicacion { get; set; }

        #endregion

        #region Metodos

        public Error Pack(Terminal terminal, string[] ultimaConexion, out byte[] salida, EnumPaquete tipo, ushort orden)
        {
            byte[] entrada = { };

            if (terminal != null && GestorTransacciones.ConfiguracionComunicacion.NackEnvio == NackEnv.SINERROR)
            {
                entrada = ConstructorMenEnv.CrearA(terminal, ultimaConexion, GestorTransacciones.ConfiguracionComunicacion.LocalIP);

                var label = tipo == EnumPaquete.DATOS
                ? "Mensaje " + Encoding.UTF8.GetChars(entrada, 0, 1).FirstOrDefault().ToString()
                : "Envia " + tipo.ToString();
                ModuloDeRegistro.LogBuffer(entrada, label + " ( " + entrada.Length.ToString() + "b )", entrada.Length, NLog.LogLevel.Info);
            }
            else
                ModuloDeRegistro.LogBuffer(entrada, "Envia NACK tipo " + ConfiguracionComunicacion.NackEnvio + " ( " + entrada.Length.ToString() + "b )", entrada.Length, NLog.LogLevel.Error);

            TipoPaqueteSalida = tipo;

            if (tipo == EnumPaquete.ACK || tipo == EnumPaquete.EOT)
                salida = new byte[256];
            else
                salida = new byte[(entrada.Length * 2) + 9];

            if (entrada == null)
                return new Error("Buffer a enviar nulo.", (int)ErrProtocolo.PARAM_NULO_INVALIDO, 0);

            Enmascarador enmask = new Enmascarador(GestorTransacciones.ConfiguracionComunicacion.Configuracion);
            CRC16CCITT crc = new CRC16CCITT(0);
            Empaquetador emp = new Empaquetador();

            string tipoMen = "";
            if (tipo == EnumPaquete.DATOS)
                tipoMen = Encoding.UTF8.GetString(entrada, 0, 1);
            else
                tipoMen = tipo.ToString();

            int longi = 0;

            byte[] aEnviar3;

            if (GestorTransacciones.ConfiguracionComunicacion.NackEnvio == NackEnv.SINERROR)
            {
                byte[] aEnviar2;
                Error Err = emp.Empaqueta(entrada, out aEnviar2, entrada.Length, tipo, orden);

                ModuloDeRegistro.LogBuffer(aEnviar2, "Empaquetado ( " + aEnviar2.Length.ToString() + "b )", aEnviar2.Length, NLog.LogLevel.Debug);

                if (Err.CodError != 0)
                    return Err;

                longi = 0;

                enmask.Enmascara(aEnviar2, aEnviar2.Length, ref salida, ref longi);

                Array.Resize(ref salida, longi);

                ModuloDeRegistro.LogBuffer(salida, "Enmascarado ( " + salida.Length.ToString() + "b )", salida.Length, NLog.LogLevel.Debug);
            }
            else // envia NACK con el tipo de error 
            {
                byte[] nac = new byte[1];
                nac = ConstructorMenEnv.CrearNack((byte)GestorTransacciones.ConfiguracionComunicacion.NackEnvio);

                byte[] aEnviar2;
                Error Err = emp.Empaqueta(nac, out aEnviar2, nac.Length, EnumPaquete.NACK, orden);
                if (Err.CodError != 0)
                    return Err;

                aEnviar3 = crc.AgregaCRCAlBuffer(aEnviar2, aEnviar2.Length);
                longi = 0;

                entrada = new byte[aEnviar3.Length + 1];
                entrada = aEnviar3;
                Array.Resize(ref entrada, aEnviar3.Length + 1);
                entrada[aEnviar3.Length] = 0x03;

                enmask.Enmascara(aEnviar3, aEnviar3.Length, ref salida, ref longi);
                salida[longi] = 0x03;
                longi++;
                Array.Resize(ref salida, longi);
            }

            return new Error();
        }

        public Error Unpack(byte[] entrada, out byte[] salida)
        {
            Enmascarador enmask = new Enmascarador(ConfiguracionComunicacion.Configuracion);
            CRC16CCITT crc = new CRC16CCITT(0);
            Empaquetador emp = new Empaquetador();

            int lonMenR = 0;
            byte[] menRArr = new byte[0];

            if (entrada[entrada.Length - 1] != 0x03)
            {
                foreach (byte byt in entrada)
                {
                    lonMenR++;
                    if (byt == 0x03)
                    {
                        Array.Resize(ref entrada, lonMenR);
                        break;
                    }
                }
                menRArr = new byte[lonMenR];
            }
            else
                menRArr = new byte[entrada.Length];

            ModuloDeRegistro.LogBuffer(entrada, "Enmascarado ( " + entrada.Length.ToString() + "b )", entrada.Length, NLog.LogLevel.Debug);

            enmask.Desenmascara(entrada, entrada.Length, ref menRArr, ref lonMenR);

            ModuloDeRegistro.LogBuffer(menRArr, "Empaquetado ( " + menRArr.Length.ToString() + "b )", menRArr.Length, NLog.LogLevel.Debug);

            if (crc.CompruebaCRC(menRArr, lonMenR))
            {
                Error Err = emp.Desempaqueta(menRArr, out salida, ref lonMenR, 2, ref ordenAckE);

                TipoPaqueteEntrada = (EnumPaquete)Empaquetador.TipoPaqueteRecibido;

                if (Err.CodError != 0)
                {
                    if (Err.CodError == (int)ErrProtocolo.LONGITUD ||
                        Err.CodError == (int)ErrProtocolo.INICIO ||
                        Err.CodError == (int)ErrProtocolo.FIN)
                    {
                        GestorTransacciones.ConfiguracionComunicacion.NackEnvio = NackEnv.EMPAQUETADO;
                    }

                    return Err;
                }
            }
            else
            {
                GestorTransacciones.ConfiguracionComunicacion.NackEnvio = NackEnv.CRC;
                salida = null;
                return new Error("Error protocolo: CRC recibido inválido.", (int)ErrProtocolo.CRC, 0);
            }

            return new Error();
        }

        public IList MapeoObjetos(byte[] bufferRec, int tipoEsperado, ushort orden, string tipoMen)
        {
            #region // TipoPaqRec EOT ACK NACK
            if (Empaquetador.TipoPaqueteRecibido == (byte)EnumPaquete.EOT)
                return new List<object>() { "Eot" };
            else if (Empaquetador.TipoPaqueteRecibido == (byte)EnumPaquete.ACK)
                return new List<object>() { "Ack" };
            else if (Empaquetador.TipoPaqueteRecibido == (byte)EnumPaquete.NACK)
            {
                if (bufferRec[0].ToString() == "i")
                    return new List<object>() { "B" };
                else
                    return new List<object>() { Encoding.UTF8.GetString(bufferRec, 0, 1) };
            }
            #endregion

            char letra = Encoding.UTF8.GetChars(bufferRec, 0, 1).FirstOrDefault();

            if (!Char.IsLetter(letra) && Empaquetador.TipoPaqueteRecibido != (byte)EnumPaquete.DATOSMULTIPLES)
                return new List<object>() { new Error("Mensaje sin letra.", (int)ErrProtocolo.DATOS_LETRA, 1) };
            if (bufferRec.Length <= 0)
                return new List<object>() { new Error("Mensaje sin datos.", (int)ErrProtocolo.DATOS_VACIOS, 1) };
            if (tipoMen.Substring(0, 1).FirstOrDefault() != letra &&
                Empaquetador.TipoPaqueteRecibido != (byte)EnumPaquete.DATOSMULTIPLES &&
                letra != 'L' &&
                letra != 'E')
                return new List<object>() { new Error("Letra de datos incorrecta.", (int)ErrProtocolo.DATOS_LETRA, 1) };

            IList objetos = new List<object>();

            if (tipoEsperado == 2 || tipoEsperado == 5 || tipoEsperado == 6 || tipoEsperado == 4)
            {
                #region //EXTRACCION DE ERROR
                //cada mensaje puede tener el cod de error en distintas posiciones
                DataConverter codErr = DataConverter.BigEndian;
                uint er = 0;
                if (Empaquetador.TipoPaqueteRecibido != (byte)EnumPaquete.DATOSMULTIPLES)
                {
                    if (tipoMen == "B")
                    {
                        byte[] err = new byte[2]; Array.ConstrainedCopy(bufferRec, 1, err, 0, 2); er = codErr.GetUInt16(err, 0);
                    }
                    else if (tipoMen.Contains("SAck") && letra != 'E')
                    {
                    }
                    else if (tipoMen.Contains("Q") || tipoMen.Contains("q") || tipoMen.Contains("D") && letra != 'E')
                    {
                        byte[] err = new byte[2]; Array.ConstrainedCopy(bufferRec, 2, err, 0, 2); er = codErr.GetUInt16(err, 0);
                    }
                    else if ((tipoMen.Contains("xR") || tipoMen.Contains("sR")) && letra == 'L')
                    {
                        byte[] err = new byte[4]; Array.ConstrainedCopy(bufferRec, 1, err, 0, 4); er = codErr.GetUInt32(err, 0);
                    }
                    else if (letra == 'E' || Encoding.UTF8.GetString(bufferRec, 0, 1) == "GE")
                    {
                        byte[] err = new byte[2]; Array.ConstrainedCopy(bufferRec, 6, err, 0, 2); er = codErr.GetUInt16(err, 0);
                    }
                }
                #endregion               

                if (er == 0)
                {
                    ConfiguracionComunicacion.NackEnvio = NackEnv.SINERROR;
                    switch (tipoMen)
                    {
                        case "B": objetos = ConstructorMenRec.recibeMensajeB(bufferRec); break;

                            // Acá podrán añadirse, a demanda, más estructuras de paquetes de datos

                    }
                }
                else if (er != 0)
                {
                    switch (tipoMen)
                    {
                        case "B": objetos = ConstructorMenRec.recibeMensajeBErr(bufferRec); break;

                            // Acá podrán añadirse, a demanda, más estructuras de paquetes de error 
                    }
                }
            }
            else if (Empaquetador.TipoPaqueteRecibido == (byte)EnumPaquete.NACK)
            {
                objetos.Add(Encoding.UTF8.GetString(bufferRec));
                //PeTR.LogMessage("Recibe NACK causa " + Encoding.UTF8.GetString(bufferRec), EnumMessageType.ERROR);
                return objetos;
            }

            return objetos;
        }

        #endregion

        #region Aux
        private static char[] byteToChar(byte[] bytes)
        {
            char[] ch = new char[bytes.Length];
            for (int i = 0; i < bytes.Length; i++)
            {
                ch[i] = (char)bytes[i];
            }
            return ch;
        }
        #endregion
    }
}
