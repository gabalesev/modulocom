using LibreriaClases;
using LibreriaClases.Clases;
using MonoLibrary;
using System;
using System.Collections;
using System.Linq;
using System.Text;

namespace LibreriaMetodologia
{
    public static class ConstructorMenEnv
    {
        public static UInt32 TARJETA = 0;        

        /// <summary>
        /// Arma mensaje A para vincular con el servidor
        /// </summary>
        /// <param name="ter"></param>
        /// <param name="UltimaCon"></param>
        /// <param name="ipLocal"></param>
        /// <returns></returns>
        public static byte[] crearA_Logueo(Terminal ter, string[] UltimaCon, System.Net.IPAddress ipLocal)
        {
            string datos1 = UltimaCon[0];
            string datos2 = UltimaCon[1];
            string datos3 = UltimaCon[2];

            //Fecha/Hora real con ceros agregados si fuese necesario
            string dia, mes, anio, hora, min, seg;
            if ((Convert.ToString(ter.FechaHora.Day).Length < 2)) { dia = "0" + Convert.ToString(ter.FechaHora.Day); } else { dia = Convert.ToString(ter.FechaHora.Day); }
            if ((Convert.ToString(ter.FechaHora.Month).Length < 2)) { mes = "0" + Convert.ToString(ter.FechaHora.Month); } else { mes = Convert.ToString(ter.FechaHora.Month); }
            if ((Convert.ToString(ter.FechaHora.Year).Length > 2)) { anio = Convert.ToString(ter.FechaHora.Year).Remove(0, 2); } else { anio = Convert.ToString(ter.FechaHora.Year); }
            if ((Convert.ToString(ter.FechaHora.Hour).Length < 2)) { hora = "0" + Convert.ToString(ter.FechaHora.Hour); } else { hora = Convert.ToString(ter.FechaHora.Hour); }
            if ((Convert.ToString(ter.FechaHora.Minute).Length < 2)) { min = "0" + Convert.ToString(ter.FechaHora.Minute); } else { min = Convert.ToString(ter.FechaHora.Minute); }
            if ((Convert.ToString(ter.FechaHora.Second).Length < 2)) { seg = "0" + Convert.ToString(ter.FechaHora.Second); } else { seg = Convert.ToString(ter.FechaHora.Second); }                       
            

            //header
            byte[] pacHead = DataConverter.Pack("^$8SbbII$8$8$8$8$8$8S", "A", 0, (int)GestorTransacciones.ProtoConfig.TIPO_CXN, ter.Tipo, ter.Tarjeta, ter.NumeroTerminal, dia, mes, anio, hora, min, seg, ter.Version);
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

        /// <summary>
        /// Arma mensaje P
        /// </summary>
        /// <param name="tipo"></param>
        /// <param name="numTerminal"></param>
        /// <returns></returns>
        public static byte[] crearP_PedidoSorteo(byte tipo, UInt32 numTerminal)
        {
            byte[] pacTerm = DataConverter.Pack("^$8bI", "P", tipo, numTerminal);
            return pacTerm;
        }

        /// <summary>
        /// Arma transaccion NACK
        /// </summary>
        /// <param name="err"></param>
        /// <returns></returns>
        public static byte[] crearNack(byte err)
        {
            return DataConverter.Pack("^b", err);
        }     

        /// <summary>
        /// Rellena espacios sobrantes
        /// </summary>
        /// <param name="dato"></param>
        /// <param name="cantBytes"></param>
        /// <returns></returns>
        public static byte[] calcBytes(string dato, int cantBytes)
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
    }
}
