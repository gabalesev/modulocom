using LibreriaClases;
using LibreriaClases.Clases;
using MonoLibrary;
using System;
using System.Collections;
using System.Linq;
using System.Text;

namespace ProtocoloLib
{
    public static class ConstructorMenEnv
    {
        public static UInt32 TARJETA = 0;        

        //---------(A)---------//
        #region LOGUEO
        public static byte[] crearA_Logueo(Terminal ter, string[] UltimaCon, System.Net.IPAddress ipLocal)
        {
            #region //variables mensaje A

           // string idBody1 = "N";
            string datos1 = UltimaCon[0];
           // string idBody2 = "P";
            string datos2 = UltimaCon[1];            
           // string idBody3 = "T";
            string datos3 = UltimaCon[2];

            //Fecha/Hora real con ceros agregados si fuese necesario
            string dia, mes, anio, hora, min, seg;
            if ((Convert.ToString(ter.FechaHora.Day).Length < 2)) { dia = "0" + Convert.ToString(ter.FechaHora.Day); } else { dia = Convert.ToString(ter.FechaHora.Day); }
            if ((Convert.ToString(ter.FechaHora.Month).Length < 2)) { mes = "0" + Convert.ToString(ter.FechaHora.Month); } else { mes = Convert.ToString(ter.FechaHora.Month); }
            if ((Convert.ToString(ter.FechaHora.Year).Length > 2)) { anio = Convert.ToString(ter.FechaHora.Year).Remove(0, 2); } else { anio = Convert.ToString(ter.FechaHora.Year); }
            if ((Convert.ToString(ter.FechaHora.Hour).Length < 2)) { hora = "0" + Convert.ToString(ter.FechaHora.Hour); } else { hora = Convert.ToString(ter.FechaHora.Hour); }
            if ((Convert.ToString(ter.FechaHora.Minute).Length < 2)) { min = "0" + Convert.ToString(ter.FechaHora.Minute); } else { min = Convert.ToString(ter.FechaHora.Minute); }
            if ((Convert.ToString(ter.FechaHora.Second).Length < 2)) { seg = "0" + Convert.ToString(ter.FechaHora.Second); } else { seg = Convert.ToString(ter.FechaHora.Second); }                       
            #endregion

            //header
            byte[] pacHead = DataConverter.Pack("^$8SbbII$8$8$8$8$8$8S", "A", 0, (int)TransacManager.ProtoConfig.TIPO_CXN, ter.Tipo, ter.Tarjeta, ter.NumeroTerminal, dia, mes, anio, hora, min, seg, ter.Version);
            byte[] pacSal1 = new byte[1024];
            
            TARJETA = ter.Tarjeta;           

            int lon = pacHead.Length + ter.MacTarjeta.Length;

            Array.Copy(pacHead, pacSal1, pacHead.Length);
            Array.Copy(ter.MacTarjeta, 0, pacSal1, pacHead.Length, ter.MacTarjeta.Length);
            
           

            string telefono = UltimaCon[2];
            for (int i = 0; i < 15 - UltimaCon[2].Length; i++)
            {
                telefono = telefono + " ";
            }
            telefono = telefono.Substring(0, 15);
            string user = UltimaCon[0];
            for (int i = 0; i < 10 - UltimaCon[0].Length; i++)
            {
                user = " " + user; ;
            }
            user = user.Substring(0, 10);

            byte[] pacSal2 = DataConverter.Pack("^$8$8", telefono, user);
            Array.Copy(pacSal2, 0, pacSal1, lon, pacSal2.Length);

            byte[] pacSal3 = DataConverter.Pack("^SS", 512, Convert.ToUInt16(UltimaCon[1]));
            byte[] ipBytes = ipLocal.GetAddressBytes();
            Array.Copy(pacSal3, 0, pacSal1, lon+pacSal2.Length, pacSal3.Length);
            Buffer.BlockCopy(ipBytes, 0, pacSal1, lon + pacSal2.Length + pacSal3.Length, ipBytes.Length);

            lon += pacSal2.Length + pacSal3.Length + ipBytes.Length;

            Array.Resize(ref pacSal1, lon);
            return pacSal1;
        }
        #endregion


        //---------(P)---------//
        #region PEDIDO SORTEOS
        public static byte[] crearP_PedidoSorteo(byte tipoJuego, UInt32 numTerminal)// Solicita SORTEO de cualquier tipo
        {
            byte[] pacTerm = DataConverter.Pack("^$8bI", "P", tipoJuego, numTerminal);
            return pacTerm;

            #region USA PROYECTO
            //if(TransacManager.ProtoConfig.PROYECTO != EnumProyecto.MENDOZA)
            //{
            //    if (tipoJuego == (byte)PedidosSorteos.LOTO)
            //    {
            //        byte[] pacTerm = DataConverter.Pack("^$8bIb", "P", tipoJuego, numTerminal, 0x01);
            //        return pacTerm;
            //    }
            //    else
            //    {
            //        byte[] pacTerm = DataConverter.Pack("^$8bI", "P", tipoJuego, numTerminal);
            //        return pacTerm;
            //    }
            //}
            //else
            //{
            //    if (tipoJuego == (byte)PedidosSorteos.LOTO)
            //    {
            //        byte[] pacTerm = DataConverter.Pack("^$8bIbbbbbbbbb", "P", tipoJuego, numTerminal, 0x01, 0xcd, 0xcd, 0xcd, 0xcd, 0xcd, 0xcd, 0xcd, 0xcd);
            //        return pacTerm;
            //    }
            //    else
            //    {
            //        byte[] pacTerm = DataConverter.Pack("^$8bIbbbbbbbbb", "P", tipoJuego, numTerminal, 0, 0xcd, 0xcd, 0xcd, 0xcd, 0xcd, 0xcd, 0xcd, 0xcd);
            //        return pacTerm;
            //    }
            //}
            #endregion
        }
        #endregion

        #region ENVIO DE APUESTAS
        public static byte[] crearP_QM(TransacQuinielaH cabecera, TransacQuinielaB juegos)// Quiniela Multiple
        {            
            byte[] tipoMsg = DataConverter.Pack("^$8", "P");
            byte[] protocolo = ProtocoloApuesta.EmpaquetaQuiniela(cabecera, juegos);

            byte[] paq = new byte[tipoMsg.Length + protocolo.Length];

            Array.Copy(tipoMsg, paq, tipoMsg.Length);
            Array.Copy(protocolo, 0, paq, tipoMsg.Length, protocolo.Length);

            //paq.Skip(1).Take(paq.Length - 13).ToArray();
                
            //Comunicacion.PROTOCOLO = protocolo;
            //Array.Resize(ref Comunicacion.PROTOCOLO, protocolo.Length - 12);

            return paq;
        }
        #endregion


        //---------------------//        
        #region PROTOCOLO 
        public static byte[] crearNack(byte err)
        {
            return DataConverter.Pack("^b", err);
        }

        public static byte[] creaX(string eco)
        {
            return DataConverter.Pack("^$8", "X"+eco);
        }
        #endregion        

        #region Auxiliares
        public static byte[] calcBytes(string dato, int cantBytes)// string
        {
            byte[] bytes = new byte[cantBytes];
            byte[] aux = Encoding.ASCII.GetBytes(dato);
            Array.Copy(aux, bytes, aux.Length);
            for (int i = aux.Length; i < cantBytes; i++)
            {
                bytes[i] = 0x20;
            }
            return bytes;
        }
        public static byte[] calcBytes(int dato, int cantBytes)// int
        {
            byte[] bytes = new byte[cantBytes];

            /*
            (value & 0x00000000000000FFUL) << 56 | (value & 0x000000000000FF00UL) << 40 |
            (value & 0x0000000000FF0000UL) << 24 | (value & 0x00000000FF000000UL) << 8 |
            (value & 0x000000FF00000000UL) >> 8 | (value & 0x0000FF0000000000UL) >> 24 |
            (value & 0x00FF000000000000UL) >> 40 | (value & 0xFF00000000000000UL) >> 56; 
            */

            return bytes;
        }
        #endregion
    }
}
