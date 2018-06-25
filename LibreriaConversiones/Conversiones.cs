using System;
using System.Collections;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using MonoLibrary;

namespace LibreriaConversiones
{
  public class Conversiones
  {
    public static IEnumerable<bool> GetBits(byte b)
    {
      for (int i = 0; i < 8; ++i)
      {
        yield return ((uint) b & 128U) > 0U;
        b *= (byte) 2;
      }
    }

    private static byte EnciendeBit(byte nrobase, byte adiciona)
    {
      switch (adiciona)
      {
        case 0:
          nrobase ^= (byte) 128;
          break;
        case 1:
          nrobase ^= (byte) 64;
          break;
        case 2:
          nrobase ^= (byte) 32;
          break;
        case 3:
          nrobase ^= (byte) 16;
          break;
        case 4:
          nrobase ^= (byte) 8;
          break;
        case 5:
          nrobase ^= (byte) 4;
          break;
        case 6:
          nrobase ^= (byte) 2;
          break;
        case 7:
          nrobase ^= (byte) 1;
          break;
      }
      return nrobase;
    }

    public static byte[] Int_a_BCD(int input, int cantdig)
    {
      if (input > 9999 || input < 0)
        throw new ArgumentOutOfRangeException(nameof (input));
      if (cantdig > 4 || cantdig < 0)
        throw new ArgumentOutOfRangeException(nameof (cantdig));
      int num1 = input / 1000;
      int num2 = (input -= num1 * 1000) / 100;
      int num3 = (input -= num2 * 100) / 10;
      int num4 = input -= num3 * 10;
      byte[] numArray = new byte[2];
      switch (cantdig)
      {
        case 1:
          numArray[0] = (byte) 170;
          numArray[1] = (byte) (160 | num4);
          break;
        case 2:
          numArray[0] = (byte) 170;
          numArray[1] = (byte) (num3 << 4 | num4);
          break;
        case 3:
          numArray[0] = (byte) (160 | num2);
          numArray[1] = (byte) (num3 << 4 | num4);
          break;
        case 4:
          numArray[0] = (byte) (num1 << 4 | num2);
          numArray[1] = (byte) (num3 << 4 | num4);
          break;
      }
      return numArray;
    }

    public static string BCDtoString(byte[] bcds)
    {
      int num1 = 0;
      string empty = string.Empty;
      foreach (byte bcd in bcds)
      {
        int num2 = (int) bcd >> 4;
        if (num2 != 10)
          empty += num2.ToString();
        int num3 = num1 + 1;
        int num4 = (int) bcd & 15;
        if (num4 != 10)
          empty += num4.ToString();
        num1 = num3 + 1;
      }
      return empty;
    }

    public static string AgregaCadena(byte[] bRec, int startInd, int longitud)
    {
      byte[] bytes = new byte[longitud];
      Array.ConstrainedCopy((Array) bRec, startInd, (Array) bytes, 0, longitud);
      return Encoding.Default.GetString(bytes);
    }

    public static uint AgregaDigito32(byte[] bRec, int startInd)
    {
      byte[] data = new byte[4];
      Array.ConstrainedCopy((Array) bRec, startInd, (Array) data, 0, 4);
      return DataConverter.BigEndian.GetUInt32(data, 0);
    }

    public static int AgregaDigito16(byte[] bRec, int startInd)
    {
      byte[] data = new byte[2];
      Array.ConstrainedCopy((Array) bRec, startInd, (Array) data, 0, 2);
      return (int) DataConverter.BigEndian.GetUInt16(data, 0);
    }

    public static int AgregaDigito(byte[] bRec, int startInd)
    {
      byte[] numArray = new byte[1];
      Array.ConstrainedCopy((Array) bRec, startInd, (Array) numArray, 0, 1);
      return (int) numArray[0];
    }

    public static int[] AgregaRangos(byte[] bmp)
    {
      int[] array = new int[20];
      int newSize = 0;
      char[] chArray = new char[24]
      {
        '0',
        '0',
        '0',
        '0',
        '0',
        '0',
        '0',
        '0',
        '0',
        '0',
        '0',
        '0',
        '0',
        '0',
        '0',
        '0',
        '0',
        '0',
        '0',
        '0',
        '0',
        '0',
        '0',
        '0'
      };
      int num = 0;
      for (int index = 0; index < bmp.Length; ++index)
      {
        char[] charArray = Convert.ToString(bmp[index], 2).ToCharArray();
        Array.ConstrainedCopy((Array) charArray, 0, (Array) chArray, num + (8 - charArray.Length), charArray.Length);
        num += 8;
      }
      for (int index = 0; index < chArray.Length; ++index)
      {
        if (chArray[index] == '1')
        {
          array[newSize] = index + 1;
          ++newSize;
        }
      }
      Array.Resize<int>(ref array, newSize);
      return array;
    }

    public static T[,] ResizeArray<T>(T[,] original, int x, int y)
    {
      T[,] objArray = new T[x, y];
      int num = Math.Min(original.GetLength(0), objArray.GetLength(0));
      int length = Math.Min(original.GetLength(1), objArray.GetLength(1));
            for (int index = 0; index < num; ++index)
            {
                Array.Copy(
                    (Array)original, index * original.GetLength(0), 
                    (Array)objArray, index * objArray.GetLength(1), 
                    length);
            }

      return objArray;
    }

    public static string Arma10Alfanumericos(int num)
    {
      string str1 = num.ToString();
      string str2 = "";
      for (int index = 0; index < 10 - str1.Length; ++index)
        str2 = "0" + str2;
      return str2 + str1;
    }

    public static string Arma10Alfanumericos(short num)
    {
      string str1 = num.ToString();
      string str2 = "";
      for (int index = 0; index < 10 - str1.Length; ++index)
        str2 = "0" + str2;
      return str2 + str1;
    }

    public static bool EsFechaHoraValida(byte[] buffer, ref DateTime fechaHora, int startInd, int longitud)
    {
      string s = Conversiones.AgregaDigito(buffer, startInd).ToString() + "/" + (object) Conversiones.AgregaDigito(buffer, startInd + 1) + "/" + (object) Conversiones.AgregaDigito16(buffer, startInd + 2) + " " + (object) Conversiones.AgregaDigito(buffer, startInd + 4) + ":" + (object) Conversiones.AgregaDigito(buffer, startInd + 5) + ":" + (object) Conversiones.AgregaDigito(buffer, startInd + 6);
      DateTimeStyles styles = DateTimeStyles.None;
      CultureInfo cultureInfo = new CultureInfo("es-AR", false);
      return DateTime.TryParse(s, (IFormatProvider) cultureInfo, styles, out fechaHora);
    }

    public static bool EsFechaHoraValida(int dia, int mes, short anio, int hora, int min, int seg, ref DateTime fechaHora)
    {
      return DateTime.TryParse(dia.ToString() + "/" + (object) mes + "/" + (object) anio + " " + (object) hora + ":" + (object) min + ":" + (object) seg, out fechaHora);
    }

    public static char[] byteToChar(byte[] bytes)
    {
      char[] chArray = new char[bytes.Length];
      for (int index = 0; index < bytes.Length; ++index)
        chArray[index] = (char) bytes[index];
      return chArray;
    }
  }
}
