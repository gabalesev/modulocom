using System;
using LibreriaClases;
using LibreriaClases.Clases;

namespace LibreriaMetodologia
{
    public class Enmascarador
    {
        private readonly byte Escape;
        private readonly byte[] Mascara;
        private readonly byte[] ValoresAEnmascarar;

        public Enmascarador(ArchivoConfig conf)
        {
            Escape = conf.MaskEscape;            
            Mascara = conf.MaskEnmascara;
            ValoresAEnmascarar = conf.MaskDesenmascara;
        }

        public Error Desenmascara(byte[] entrada, int longent, ref byte[] salida, ref int longsal)
        {
            if (longent > entrada.Length)
            {
                GestorTransacciones.ConfiguracionComunicacion.NackEnvio = NackEnv.EMPAQUETADO;
                return new Error("Error protocolo: longitud incorrecta.", (int)ErrProtocolo.LONGITUD, 0);
            }

            int j = 0;

            for (int i = 0; i < longent; )
            {
                if (entrada[i] == Escape)
                {
                    int index = BuscaEnArr(ValoresAEnmascarar, entrada[i + 1]);
                    if (index == -1)
                    {
                        GestorTransacciones.ConfiguracionComunicacion.NackEnvio = NackEnv.EMPAQUETADO;
                        return new Error("Error protocolo: mascara incorrecta.", (int)ErrProtocolo.LONGITUD, 0);
                    }

                    salida[j] = Mascara[index];
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


                int index = BuscaEnArr(Mascara, entrada[i]);

                if (index < 0)
                {
                    salida[j] = entrada[i];
                    j++;
                }
                else
                {
                    salida[j] = Escape;
                    salida[j + 1] = ValoresAEnmascarar[index];
                    j = j + 2;
                }

                longsal = j;
            }

        }
        private int BuscaEnArr(byte[] arr, byte byt)
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
