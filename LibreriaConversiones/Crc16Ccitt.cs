using System;
using MonoLibrary;
using LibreriaClases;

namespace LibreriaConversiones
{
    public class CRC16CCITT
    {
        private const ushort POLINOMIO = 4129;
        private ushort[] Tabla = new ushort[256];
        private ushort ValorInicial = 0;

        private ushort ChequeaSuma(byte[] bytes)
        {
            ushort crc = this.ValorInicial;
            for (int i = 0; i < bytes.Length; i++)
            {
                crc = (ushort)((crc << 8) ^ Tabla[((crc >> 8) ^ (0xff & bytes[i]))]);
            }
            return crc;
        }

        //public byte[] ChequeaSumaDeBytes(byte[] bytes)
        //{
        //    ushort crc = ChequeaSuma(bytes);
        //    return new byte[] { (byte)(crc >> 8), (byte)(crc & 0x00ff) };
        //}

        //public byte[] ChequeaSumaDeBytes(byte[] bytes, int longitud)
        //{
        //    byte[] buffer = new byte[longitud];
        //    Array.Copy(bytes, buffer, longitud);
        //    ushort crc = ChequeaSuma(bytes);
        //    return new byte[] { (byte)(crc >> 8), (byte)(crc & 0x00ff) };
        //}

        public byte[] AgregaCRCAlBuffer(byte[] bytes, int longitud)
        {
            byte[] buffer = new byte[longitud + 2];

            ushort rescrc;

            Array.Copy(bytes, buffer, longitud);
            
            rescrc = ChequeaSuma(buffer);

            buffer[longitud] = (byte)(rescrc >> 8);

            buffer[longitud + 1] = (byte)(rescrc & 0x00ff);

            return buffer;
        }

        public bool CompruebaCRC(byte[] buffer, int longitud)
        {
            byte[] bufCrc = new byte[longitud - 1];
            Array.Copy(buffer, bufCrc, longitud - 1);
            bufCrc[longitud - 2] = 0x00;
            bufCrc[longitud - 3] = 0x00;

            byte[] crcEnt = new byte[2];

            Array.ConstrainedCopy(buffer, longitud - 3, crcEnt, 0, 2);

            DataConverter dat = DataConverter.BigEndian;

            ushort crcEntrada = (ushort)dat.GetUInt16(crcEnt, 0);


            ushort crcSalida = (ushort)ChequeaSuma(bufCrc);

            if (crcEntrada == crcSalida)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public CRC16CCITT(InitialCrcValue initialValue)
        {
            this.ValorInicial = (ushort)initialValue;
            ushort temp, a;
            for (int i = 0; i < Tabla.Length; i++)
            {
                temp = 0;
                a = (ushort)(i << 8);
                for (int j = 0; j < 8; j++)
                {
                    if (((temp ^ a) & 0x8000) != 0)
                    {
                        temp = (ushort)((temp << 1) ^ POLINOMIO);
                    }
                    else
                    {
                        temp <<= 1;
                    }
                    a <<= 1;
                }
                Tabla[i] = temp;
            }
        }
    }
}
