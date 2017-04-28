using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Security.Permissions;
using System.Net.NetworkInformation;
using MonoLibrary;
using System.Collections;
using LibreriaClases;

namespace LibreriaProtocolo
{
    public class Crc16Ccitt
    {
        const ushort poly = 4129;
        ushort[] table = new ushort[256];
        ushort initialValue = 0;

        public ushort ComputeChecksum(byte[] bytes)
        {
            ushort crc = this.initialValue;
            for (int i = 0; i < bytes.Length; i++)
            {
                crc = (ushort)((crc << 8) ^ table[((crc >> 8) ^ (0xff & bytes[i]))]);
            }
            return crc;
        }

        public byte[] ComputeChecksumBytes(byte[] bytes)
        {
            ushort crc = ComputeChecksum(bytes);
            return new byte[] { (byte)(crc >> 8), (byte)(crc & 0x00ff) };
        }

        public byte[] ComputeChecksumBytes(byte[] bytes, int longitud)
        {
            byte[] buffer = new byte[longitud];
            Array.Copy(bytes, buffer, longitud);
            ushort crc = ComputeChecksum(bytes);
            return new byte[] { (byte)(crc >> 8), (byte)(crc & 0x00ff) };
        }

        public byte[] AddCrcToBuffer(byte[] bytes, int longitud /*, ushort numpaq*/)
        {
            byte[] buffer = new byte[longitud + 2];

            ushort rescrc;

            Array.Copy(bytes, buffer, longitud);
            
            rescrc = ComputeChecksum(buffer);

            buffer[longitud] = (byte)(rescrc >> 8);

            buffer[longitud + 1] = (byte)(rescrc & 0x00ff);

            return buffer;
        }

        public bool compruebaCrc(byte[] buffer, int longitud)
        {
            byte[] bufCrc = new byte[longitud - 1];
            Array.Copy(buffer, bufCrc, longitud - 1);
            bufCrc[longitud - 2] = 0x00;
            bufCrc[longitud - 3] = 0x00;

            byte[] crcEnt = new byte[2];

            Array.ConstrainedCopy(buffer, longitud - 3, crcEnt, 0, 2);

            DataConverter dat = DataConverter.BigEndian;

            ushort crcEntrada = (ushort)dat.GetUInt16(crcEnt, 0);


            ushort crcSalida = (ushort)ComputeChecksum(bufCrc);

            if (crcEntrada == crcSalida)
            {
                return true;
            }
            else
            {
                TransacManager.ProtoConfig.NACK_ENV = NackEnv.CRC;
                return false;
            }
        }

        public Crc16Ccitt(InitialCrcValue initialValue)
        {
            this.initialValue = (ushort)initialValue;
            ushort temp, a;
            for (int i = 0; i < table.Length; i++)
            {
                temp = 0;
                a = (ushort)(i << 8);
                for (int j = 0; j < 8; j++)
                {
                    if (((temp ^ a) & 0x8000) != 0)
                    {
                        temp = (ushort)((temp << 1) ^ poly);
                    }
                    else
                    {
                        temp <<= 1;
                    }
                    a <<= 1;
                }
                table[i] = temp;
            }
        }
    }
}
