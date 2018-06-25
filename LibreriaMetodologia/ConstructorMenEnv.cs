using LibreriaClases;
using LibreriaClases.Clases;
using MonoLibrary;
using System;
using System.Text;

namespace LibreriaMetodologia
{
    public static class ConstructorMenEnv
    {
        // Arma mensaje A
        public static byte[] CrearA(Terminal ter, string[] UltimaCon, System.Net.IPAddress ipLocal)
        {
            var fechHoraConFormato = ter.FechaHora.ToString("ddMMyyHHmmss");

            // Metadatos y cabecera del mensaje
            byte[] pacHead = DataConverter.Pack("^$8SbbII$8S", "A", 0, (int)GestorTransacciones.ConfiguracionComunicacion.TipoConexion, ter.Tipo, ter.Tarjeta, ter.NumeroTerminal,
                fechHoraConFormato, ter.Version);
            byte[] pacSal1 = new byte[1024];

            int lon = pacHead.Length + ter.MacTarjeta.Length;

            Array.Copy(pacHead, pacSal1, pacHead.Length);
            Array.Copy(ter.MacTarjeta, 0, pacSal1, pacHead.Length, ter.MacTarjeta.Length);

            // Cuerpo del mensaje
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

        // Arma mensaje P
        public static byte[] CrearP(byte tipo, UInt32 numTerminal)
        {
            byte[] pacTerm = DataConverter.Pack("^$8bI", "P", tipo, numTerminal);
            return pacTerm;
        }

        // Arma Nack
        public static byte[] CrearNack(byte err)
        {
            return DataConverter.Pack("^b", err);
        }
     
        // Rellena espacios sobrantes para un dato de tipo string
        public static byte[] CalcularBytes(string dato, int cantBytes)
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