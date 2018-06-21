using BinConfig;
using LibreriaClases;
using LibreriaClases.Clases;
using LibreriaRegistro;
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

        //SECUENCIAS PARA ENVIAR Y DE CONTROL DE PAQUETES RECIBIDOS
        public ushort ordenAckE = 0, ordenMsgNackE = 0, ordenAckR = 0, ordenMsgNackR = 0; 

        public EnumPaquete TipoPaqueteEntrada { get; set; }
        public EnumPaquete TipoPaqueteSalida { get; set; }

        public int EntradaLength { get; set; }
        public int SalidaLength { get; set; }        

        public EnumTipoTransaccion TipoTransaccion { get; set; }

        public static MetodologiaConfig ProtoConfig { get; set; } 

        #endregion

        #region Metodos

        public Error Pack(byte[] entrada, out byte[] salida, EnumPaquete tipo, ushort orden)
        {
            if (tipo == EnumPaquete.ACK || tipo == EnumPaquete.EOT)
                salida = new byte[256];
            else
                salida = new byte[(entrada.Length * 2) + 9];

            if (entrada == null)            
                return new Error("Buffer a enviar nulo.", (int)ErrProtocolo.PARAM_NULO_INVALIDO , 0);

            Enmascarador enmask = new Enmascarador(GestorTransacciones.ProtoConfig.CONFIG);
            Crc16Ccitt crc = new Crc16Ccitt(0);
            Empaquetador emp = new Empaquetador();

            string tipoMen = "";
            if (tipo == EnumPaquete.DATOS) tipoMen = Encoding.UTF8.GetString(entrada, 0, 1);
            else tipoMen = tipo.ToString();

            int longi = 0;

            byte[] aEnviar3;

            if (GestorTransacciones.ProtoConfig.NACK_ENV == NackEnv.SINERROR)
            {
                byte[] aEnviar2;
                Error Err = emp.Empaqueta(entrada, out aEnviar2, entrada.Length, tipo, orden);

                ModuloDeRegistro.LogBuffer(aEnviar2, "Empaquetado ( " + aEnviar2.Length.ToString() + "b )", aEnviar2.Length, NLog.LogLevel.Debug);

                if (Err.CodError != 0)
                    return Err;

                //aEnviar3 = crc.AddCrcToBuffer(aEnviar2, aEnviar2.Length);
                longi = 0;

                // entrada no se usa, para que estaba esto??
                //entrada = new byte[aEnviar3.Length + 1]; 
                //entrada = aEnviar3;
                //Array.Resize(ref entrada, aEnviar3.Length + 1);
                //entrada[aEnviar3.Length] = 0x03;

                enmask.Enmascara(aEnviar2, aEnviar2.Length, ref salida, ref longi);
                //salida[longi] = Empaquetador.;
                //longi++;
                Array.Resize(ref salida, longi);

                ModuloDeRegistro.LogBuffer(salida, "Enmascarado ( " + salida.Length.ToString() + "b )", salida.Length, NLog.LogLevel.Debug);
            }
            else // envio NACK con el tipo de error (TransacManager.ProtoConfig.NACK_ENV)
            {
                byte[] nac = new byte[1];
                nac = ConstructorMenEnv.crearNack((byte)GestorTransacciones.ProtoConfig.NACK_ENV);

                byte[] aEnviar2;                
                Error Err = emp.Empaqueta(nac, out aEnviar2, nac.Length, EnumPaquete.NACK, orden);
                if (Err.CodError != 0)
                    return Err;

                aEnviar3 = crc.AddCrcToBuffer(aEnviar2, aEnviar2.Length);
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
            Enmascarador enmask = new Enmascarador(ProtoConfig.CONFIG);
            Crc16Ccitt crc = new Crc16Ccitt(0);
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

            if (crc.compruebaCrc(menRArr, lonMenR))
            {
                Error Err = emp.Desempaqueta(menRArr, out salida, ref lonMenR, 2, ref ordenAckE);
                if (Err.CodError != 0)
                    return Err;
            }
            else
            {
                GestorTransacciones.ProtoConfig.NACK_ENV = NackEnv.CRC;
                salida = null;
                return new Error("Error protocolo: CRC recibido inválido.", (int)ErrProtocolo.CRC, 0);
            }
            //if (CONFIG.LevelLog == EnumMessageType.DEBUG) pe.LogBuffer(byteToChar(aRecibir), "Paquete sin desenmascarar: ", 16, aRecibir.Length);
            //pe.LogBuffer(byteToChar(menRArr), "Paquete desenmascarado: ", 16, menRArr.Length);
            //Comunicacion.setErrorNACK(NackEnv.SINERROR);
            return new Error();
        }

        public IList MapeoObjetos(byte[] bufferRec, int tipoEsperado, ushort orden, string tipoMen)
        {            
            #region // TipoPaqRec EOT ACK NACK
            if (Empaquetador.tipoPaqRec == (byte)EnumPaquete.EOT)            
                return new List<object>() { "Eot" };
            else if (Empaquetador.tipoPaqRec == (byte)EnumPaquete.ACK)
                return new List<object>() { "Ack" };
            else if (Empaquetador.tipoPaqRec == (byte)EnumPaquete.NACK)
            {
                if (bufferRec[0].ToString() == "i")
                    return new List<object>() { "B" };
                else
                    return new List<object>() { Encoding.UTF8.GetString(bufferRec, 0, 1) };
            }
            #endregion

            char letra = Encoding.UTF8.GetChars(bufferRec, 0, 1).FirstOrDefault();

            if (!Char.IsLetter(letra) && Empaquetador.tipoPaqRec != (byte)EnumPaquete.DATOSMULTIPLES)
                return new List<object>() { new Error("Mensaje sin letra.", (int)ErrProtocolo.DATOS_LETRA, 1) };
            if (bufferRec.Length <= 0)
                return new List<object>() { new Error("Mensaje sin datos.", (int)ErrProtocolo.DATOS_VACIOS, 1) };
            if(tipoMen.Substring(0, 1).FirstOrDefault() != letra && 
                Empaquetador.tipoPaqRec != (byte)EnumPaquete.DATOSMULTIPLES && 
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
                if (Empaquetador.tipoPaqRec != (byte)EnumPaquete.DATOSMULTIPLES)
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
                    ProtoConfig.NACK_ENV = NackEnv.SINERROR;                            
                    switch (tipoMen)
                    {
                        case "B": objetos = ConstructorMenRec.recibeMensajeB(bufferRec); break;
                        case "Q1":
                            {
                                if (bufferRec[1] == (byte)PedidosSorteos.QUINIELA && bufferRec.Length > 29)
                                {
                                    objetos = ConstructorMenRec.recibeMensajeQ1_Quiniela(bufferRec);
                                }                                
                                else
                                {
                                    Error erro = new Error();
                                    erro.CodError = 1001;
                                    erro.Descripcion = "Error de protocolo.";
                                    objetos.Add(erro);
                                }
                                break;
                            }
                        case "Q2": objetos = ConstructorMenRec.recibeMensajeQ2(bufferRec); break;
                        
                    }
                }
                else if (er != 0)
                {
                    switch (tipoMen)
                    {
                        case "B": objetos = ConstructorMenRec.recibeMensajeBErr(bufferRec); break;
                        case "Q2":
                        case "Q1": objetos = ConstructorMenRec.recibeMensajeQErr(bufferRec); break;      
                    }
                }
            }
            else if (Empaquetador.tipoPaqRec == (byte)EnumPaquete.NACK)
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
