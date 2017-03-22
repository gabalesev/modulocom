using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Security.Permissions;
using System.Net.NetworkInformation;
using MonoLibrary;
using System.Collections;
using BinConfig;
using LibreriaClases;
using LibreriaClases.Clases;

namespace ProtocoloLib
{
    public class Enmascarador
    {
        byte Escape;        
        byte[] mask;
        byte[] reemp;

        public Enmascarador(ArchivoConfig conf)
        {
            Escape = conf.MaskEscape;            
            mask = conf.MaskEnmascara;
            reemp = conf.MaskDesenmascara;
        }

        public Error Desenmascara(byte[] entrada, int longent, ref byte[] salida, ref int longsal)
        {
            if (longent > entrada.Length)
            {
                TransacManager.ProtoConfig.NACK_ENV = NackEnv.EMPAQUETADO;
                return new Error("Error protocolo: longitud incorrecta.", (int)ErrProtocolo.LONGITUD, 0);
            }

            int j = 0;

            for (int i = 0; i < longent; )
            {
                if (entrada[i] == Escape)
                {
                    int index = buscaEnArr(reemp, entrada[i + 1]);
                    if (index == -1)
                    {
                        TransacManager.ProtoConfig.NACK_ENV = NackEnv.EMPAQUETADO;
                        return new Error("Error protocolo: mascara incorrecta.", (int)ErrProtocolo.LONGITUD, 0);
                    }

                    salida[j] = mask[index];
                    i = i + 2;
                    j++;
                }
                else
                {
                    salida[j] = entrada[i];
                    i = i + 1;
                    j++;
                }
            }

            longsal = j;
            Array.Resize(ref salida, longsal);

            return new Error();
        }
        public void Enmascara(byte[] entrada, int longent, ref byte[] salida, ref int longsal)
        {
            //WriteMultiLineByteArray(mask, "Mascara");
            //WriteMultiLineByteArray(mask, "Mascara Ordenada");

            salida[0] = entrada[0];

            int j = 1;

            for (int i = 1; i < longent; i++)
            {
                if(i == longent - 1)
                {
                    salida[j] = entrada[i];
                    longsal++;
                    break;
                }


                int index = buscaEnArr(mask, entrada[i]);

                if (index < 0)
                {
                    salida[j] = entrada[i];
                    j++;
                }
                else
                {
                    salida[j] = Escape;
                    salida[j + 1] = reemp[index];
                    j = j + 2;
                }

                longsal = j;
            }

        }
        public int buscaEnArr(byte[] arr, byte byt)
        {
            for (int k = 0; k < arr.Length; k++)
            {
                if (arr[k] == byt)
                {
                    return k;
                }
            }
            return -1;
        }
    }
}
