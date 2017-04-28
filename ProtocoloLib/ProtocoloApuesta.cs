using System;
using System.Collections.Generic;
using System.Collections;
using MonoLibrary;
using LibreriaClases.Clases;

namespace LibreriaProtocolo
{
    public class ProtocoloApuesta
    {
        /// <summary>
        /// Recibe una apuesta poceada de Combinada y la empaqueta
        /// </summary>

        /// <summary>
        /// Convierte el mensaje a array de bytes.
        /// </summary>
        /// <param name="cabecera">Título de mensaje</param>
        static public byte[] EmpaquetaQuiniela(TransacQuinielaH cabecera, TransacQuinielaB juegos)
        {
            //header    
            byte[] pacHeader = DataConverter.Pack("^bbbSSbbbSbbbb", 0x80, 0x00, 0x01, cabecera.Sorteo, cabecera.NroSecuencia, cabecera.Entes, cabecera.FechaHora.Day,
                cabecera.FechaHora.Month, cabecera.FechaHora.Year, cabecera.FechaHora.Hour, cabecera.FechaHora.Minute, cabecera.FechaHora.Second, cabecera.CantApu);
            byte[] pacSal1 = new byte[1024];
            int paqIndex = pacHeader.Length;

            Array.Copy(pacHeader, pacSal1, paqIndex);

            //bodys           
            int limiteApu = 0;

            for (int i = 0; i < cabecera.CantApu; i++)
            {
                if (juegos.TipoApuesta[i] == 6 && limiteApu < 6)
                {
                    byte[] apuesta = Conversiones.IntToBCD(Convert.ToUInt16(juegos.NumeroAp1[i]), juegos.NumeroAp1[i].Length);
                    byte[] pacBody = DataConverter.Pack("^bbbbbS", juegos.TipoApuesta[i], apuesta[0], apuesta[1], juegos.RangoDesde1[i], juegos.RangoHasta1[i], juegos.Importe[i]);
                    Array.Copy(pacBody, 0, pacSal1, paqIndex, 7);
                    limiteApu++;
                    paqIndex = paqIndex + 7;
                }
                else if (juegos.TipoApuesta[i] == 7 && limiteApu < 6)
                {
                    byte[] apuesta1 = Conversiones.IntToBCD(Convert.ToUInt16(juegos.NumeroAp1[i]), juegos.NumeroAp1[i].Length);
                    byte[] apuesta2 = Conversiones.IntToBCD(Convert.ToUInt16(juegos.NumeroAp2[i]), juegos.NumeroAp2[i].Length);
                    byte[] pacBody = DataConverter.Pack("^bbbbbSbbbb", juegos.TipoApuesta[i], apuesta1[0], apuesta1[1], juegos.RangoDesde1[i], juegos.RangoHasta1[i], juegos.Importe[i], apuesta2[0], apuesta2[1], juegos.RangoDesde2[i], juegos.RangoHasta2[i]);
                    Array.Copy(pacBody, 0, pacSal1, paqIndex, 11);
                    limiteApu = limiteApu + 2;
                    //i++;
                    paqIndex = paqIndex + 11;
                }
                else if (juegos.TipoApuesta[i] == 11 && limiteApu < 7)
                {
                    byte[] apuesta1 = Conversiones.IntToBCD(Convert.ToUInt16(juegos.NumeroAp1[i]), juegos.NumeroAp1[i].Length);
                    byte[] apuesta2 = Conversiones.IntToBCD(Convert.ToUInt16(juegos.NumeroAp2[i]), juegos.NumeroAp2[i].Length);
                    byte[] apuesta3 = Conversiones.IntToBCD(Convert.ToUInt16(juegos.NumeroAp3[i]), juegos.NumeroAp3[i].Length);
                    byte[] pacBody = new byte[7];

                    pacBody = DataConverter.Pack("^bbbbbS", juegos.TipoApuesta[i], apuesta1[1], apuesta2[1], apuesta3[1], 0xff, juegos.Importe[i]);

                    Array.Copy(pacBody, 0, pacSal1, paqIndex, 7);
                    limiteApu++;
                    paqIndex = paqIndex + 7;
                    //no se contempla la posibilidad de que halla error de datos, es decir, que un 7 o 6 sean cambiados por alguna razon
                }
                //aca se agregarian las otras apuestas
                if (limiteApu == 7) { break; }
            }

            byte[] paqFinal = new byte[0];
            
            paqFinal = DataConverter.Pack("^I", ConstructorMenEnv.TARJETA);
            
            Array.Copy(paqFinal, 0, pacSal1, paqIndex, paqFinal.Length );
            paqIndex = paqIndex + paqFinal.Length;

            byte[] cds = { 0xcd, 0xcd, 0xcd, 0xcd, 0xcd, 0xcd, 0xcd, 0xcd };//TO DO - CDs necesarios NO BORRAR / averiguar porque los necesita
            Array.Copy(cds, 0, pacSal1, paqIndex, 8);
            paqIndex = paqIndex + 8;

            Array.Resize(ref pacSal1, paqIndex);
            pacSal1[1] = Convert.ToByte(paqIndex - 12);

            TransacManager.ProtoConfig.PROTOCOLO = pacSal1;
            //ComunicacionOffline.PROTOCOLO = pacSal1;
            Array.Resize(ref TransacManager.ProtoConfig.PROTOCOLO, pacSal1.Length - 12);
            //Array.Resize(ref ComunicacionOffline.PROTOCOLO, pacSal1.Length - 12);

            // Y colocar Ref en la definicion de parametros de este metodo //LibreriaClases.Clases.TransacQuinielaH.Protocolo = pacSal1; 
            return pacSal1;
        }

        /// <summary>
        /// Convierte array de bytes en mensaje
        /// </summary>
        /// <param name="byteRec"> Buffer de bytes a interpretar </param>
        /// <returns></returns>
        static public IList DesempaquetaQuiniela(byte[] byteRec)
        {
            TransacManager.ProtoConfig.PROTOCOLO = byteRec;
            Array.Resize(ref TransacManager.ProtoConfig.PROTOCOLO, byteRec.Length);

            Error err = new Error();
            byte cantApu;
            byte[] apuesta1 = new byte[2];
            byte[] apuesta2 = new byte[2];
            byte[] apuesta3 = new byte[1];
            int limiteApu = 0;
            int paqIndex = 0;

            // Header
            //-------
            TransacQuinielaH transaccRtaH = new TransacQuinielaH();

            transaccRtaH.TipoTransacc = (byte)Conversiones.AgregaDigito(byteRec, 0); // Tipo de Transacción
            transaccRtaH.Sorteo = (ushort)Conversiones.AgregaDigito16(byteRec, 3); // Sorteo
            transaccRtaH.NroSecuencia = (uint)Conversiones.AgregaDigito16(byteRec, 5); // Nro Secuencia
            transaccRtaH.Entes = (byte)Conversiones.AgregaDigito(byteRec, 7); // Entes

            // Fecha Completa
            int dia = (int)Conversiones.AgregaDigito(byteRec, 8); // Dia
            int mes = (int)Conversiones.AgregaDigito(byteRec, 9); // Mes 
            int anio = (int)Conversiones.AgregaDigito16(byteRec, 10); // Año
            int hora = (int)Conversiones.AgregaDigito(byteRec, 12); // Hora 
            int minuto = (int)Conversiones.AgregaDigito(byteRec, 13); // Minutos 
            int segundo = (int)Conversiones.AgregaDigito(byteRec, 14); // Segundos

            DateTime fecha = new DateTime(anio, mes, dia, hora, minuto, segundo);
            transaccRtaH.FechaHora = fecha;

            cantApu = (byte)Conversiones.AgregaDigito(byteRec, 15); // Cantidad de apuestas

            transaccRtaH.CantApu = cantApu;

            // Bodys
            //------
            TransacQuinielaB transaccRtaB = new TransacQuinielaB();

            transaccRtaB.NumeroAp1 = new string[cantApu];
            transaccRtaB.NumeroAp2 = new string[cantApu];
            transaccRtaB.NumeroAp3 = new string[cantApu];
            transaccRtaB.RangoDesde1 = new byte[cantApu];
            transaccRtaB.RangoDesde2 = new byte[cantApu];
            transaccRtaB.RangoHasta1 = new byte[cantApu];
            transaccRtaB.RangoHasta2 = new byte[cantApu];
            transaccRtaB.TipoApuesta = new byte[cantApu];
            transaccRtaB.Importe = new ushort[cantApu];

            for (int i = 0; i < cantApu; i++)
            {
                apuesta1 = new byte[2];
                apuesta2 = new byte[2];
                apuesta3 = new byte[1];

                transaccRtaB.TipoApuesta[i] = (byte)Conversiones.AgregaDigito(byteRec, 16 + paqIndex);     // Tipo de Apuesta

                // Directa
                if (transaccRtaB.TipoApuesta[i] == 6 && limiteApu < 6)
                {
                    apuesta1[0] = (byte)Conversiones.AgregaDigito(byteRec, 17 + paqIndex);
                    apuesta1[1] = (byte)Conversiones.AgregaDigito(byteRec, 18 + paqIndex);

                    transaccRtaB.NumeroAp1[i] = Conversiones.BCDtoString(apuesta1);                        // Numero Apostado 1
                    transaccRtaB.RangoDesde1[i] = (byte)Conversiones.AgregaDigito(byteRec, 19 + paqIndex); // Rango Desde
                    transaccRtaB.RangoHasta1[i] = (byte)Conversiones.AgregaDigito(byteRec, 20 + paqIndex); // Rango Hasta
                    transaccRtaB.Importe[i] = (ushort)Conversiones.AgregaDigito16(byteRec, 21 + paqIndex); // Importe

                    limiteApu++;
                    paqIndex += 7;
                }

                // Redoblona
                if (transaccRtaB.TipoApuesta[i] == 7 && limiteApu < 6)
                {
                    apuesta1[0] = (byte)Conversiones.AgregaDigito(byteRec, 17 + paqIndex);
                    apuesta1[1] = (byte)Conversiones.AgregaDigito(byteRec, 18 + paqIndex);

                    transaccRtaB.NumeroAp1[i] = Conversiones.BCDtoString(apuesta1);                        // Numero Apostado 1
                    transaccRtaB.RangoDesde1[i] = (byte)Conversiones.AgregaDigito(byteRec, 19 + paqIndex); // Rango Desde 1
                    transaccRtaB.RangoHasta1[i] = (byte)Conversiones.AgregaDigito(byteRec, 20 + paqIndex); // Rango Hasta 1
                    transaccRtaB.Importe[i] = (ushort)Conversiones.AgregaDigito16(byteRec, 21 + paqIndex); // Importe

                    apuesta2[0] = (byte)Conversiones.AgregaDigito(byteRec, 23 + paqIndex);
                    apuesta2[1] = (byte)Conversiones.AgregaDigito(byteRec, 24 + paqIndex);

                    transaccRtaB.NumeroAp2[i] = Conversiones.BCDtoString(apuesta2);                        // Numero Apostado 2
                    transaccRtaB.RangoDesde2[i] = (byte)Conversiones.AgregaDigito(byteRec, 25 + paqIndex); // Rango Desde 2
                    transaccRtaB.RangoHasta2[i] = (byte)Conversiones.AgregaDigito(byteRec, 26 + paqIndex); // Rango Hasta 2

                    limiteApu += 2;
                    paqIndex += 11;
                }


                // Poceada (toma 2 digitos de los 3 nros mas grandes apostados, ordenados de mayor a menor)
                if (transaccRtaB.TipoApuesta[i] == 11 && limiteApu < 7)
                {
                    // Toma solo los dos digitos menos significativos
                    Array.Resize(ref apuesta1, 1);
                    Array.Resize(ref apuesta2, 1);

                    // Poceada no maneja rangos
                    Array.Resize(ref transaccRtaB.RangoDesde1, cantApu - 1);
                    Array.Resize(ref transaccRtaB.RangoHasta1, cantApu - 1);
                    Array.Resize(ref transaccRtaB.RangoDesde2, cantApu - 1);
                    Array.Resize(ref transaccRtaB.RangoHasta2, cantApu - 1);

                    apuesta1[0] = (byte)Conversiones.AgregaDigito(byteRec, 17 + paqIndex);
                    transaccRtaB.NumeroAp1[i] = Conversiones.BCDtoString(apuesta1);                        // Numero Apostado 1

                    apuesta2[0] = (byte)Conversiones.AgregaDigito(byteRec, 18 + paqIndex);
                    transaccRtaB.NumeroAp2[i] = Conversiones.BCDtoString(apuesta2);                        // Numero Apostado 2

                    apuesta3[0] = (byte)Conversiones.AgregaDigito(byteRec, 19 + paqIndex);
                    transaccRtaB.NumeroAp3[i] = Conversiones.BCDtoString(apuesta3);                        // Numero Apostado 3

                    transaccRtaB.Importe[i] = (ushort)Conversiones.AgregaDigito16(byteRec, 21 + paqIndex); // Importe

                    limiteApu++;
                    paqIndex += 7;
                }
            }

            IList paq = new List<object> { err, transaccRtaH, transaccRtaB };
            return paq;
        }
    }
}
