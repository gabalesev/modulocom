using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Security.Permissions;
using System.Net.NetworkInformation;
using MonoLibrary;
using System.Collections;
using LibreriaClases;
using LibreriaClases.Clases;

namespace ProtocoloLib
{
    public class Empaquetador
    {
        public static byte secDatosMult = (byte)DatosMultSec.FIN;
        public static uint tamanioDatosMult = 0;
        public static byte tipoPaqRec = 0x00;
        private const byte INI_PAQ = 0x01;
        private const byte FIN_PAQ = 0x03;

        public Error Empaqueta(byte[] entrada, out byte[] salida, int longent, EnumPaquete tipo, ushort orden)
        {
            salida = new byte[longent + 6];

            if (longent != entrada.Length)
            {
                TransacManager.ProtoConfig.NACK_ENV = NackEnv.EMPAQUETADO;
                return new Error("Error protocolo: longitud incorrecta.", (int)ErrProtocolo.LONGITUD, 0);
            }

            salida[0] = INI_PAQ;

            salida[1] = (byte)tipo;

            Array.Copy(DataConverter.Pack("^S", orden), 0, salida, 2, 2);

            Array.Copy(DataConverter.Pack("^S", longent + 9), 0, salida, 4, 2);

            if (tipo == EnumPaquete.DATOS || tipo == EnumPaquete.NACK || tipo == EnumPaquete.DATOSMULTIPLES)
            {
                Array.Copy(entrada, 0, salida, 6, longent);
            }

            return new Error();
        }

        public Error Desempaqueta(byte[] entrada, out byte[] salida, ref int longent, int tipo, ref ushort ordenAckEnvio )
        {
            tipoPaqRec = entrada[1];
            salida = new byte[0];

            int lon = Conversiones.AgregaDigito16(entrada, 4); // dat.GetInt16(dig, 0);
            if (lon != entrada.Length)
            {
                TransacManager.ProtoConfig.NACK_ENV = NackEnv.EMPAQUETADO;
                return new Error("Error protocolo: longitud incorrecta.", (int)ErrProtocolo.LONGITUD, 0);
            }

            if (entrada[0] != INI_PAQ)
            {
                TransacManager.ProtoConfig.NACK_ENV = NackEnv.EMPAQUETADO;
                return new Error("Error protocolo: Inicio de paquete invalido.", (int)ErrProtocolo.INICIO, 0);
            }

            if(entrada[entrada.Length - 1] != FIN_PAQ)
            {
                TransacManager.ProtoConfig.NACK_ENV = NackEnv.EMPAQUETADO;
                return new Error("Error protocolo: Fin de paquete invalido.", (int)ErrProtocolo.FIN, 0);
            }

            ordenAckEnvio = (ushort)Conversiones.AgregaDigito16(entrada, 2);

            if (entrada[1] == (byte)EnumPaquete.EOT ||
                entrada[1] == (byte)EnumPaquete.DATOS ||
                entrada[1] == (byte)EnumPaquete.NACK ||
                entrada[1] == (byte)EnumPaquete.ACK)
            {
                salida = new byte[longent - 9];

                Array.ConstrainedCopy(entrada, 6, salida, 0, longent - 9);

                longent = longent - 9;
                return new Error();
            }
            else if(entrada[1] == (byte)EnumPaquete.DATOSMULTIPLES)
            {
                salida = new byte[longent - 15];

                secDatosMult = entrada[7];
                tamanioDatosMult = Conversiones.AgregaDigito32(entrada, 8); //dat.GetUInt32(dig2, 0);

                Array.ConstrainedCopy(entrada, 12, salida, 0, longent - 15);

                longent = longent - 15;
                return new Error();
            }
            else
            {                
                TransacManager.ProtoConfig.NACK_ENV = NackEnv.EMPAQUETADO;
                salida = null;
                return new Error("Error de protocolo: tipo paquete incorrecto.", (int)ErrProtocolo.TIPO_PAQUETE, 0);
            }
        }
    }
}
