using LibreriaClases;
using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.IO;
using System.Text;

namespace LoggerLib
{
    public static class LogBMTP
    {
        private static readonly Logger lo = LogManager.GetCurrentClassLogger();

        private static string DIRECTORIO_BASE;
        private static string NOMBRE_ARCHIVO;
        private static int LONGITUD_MAXIMA_ARCHIVO;
        private static EnumNivelLog NIVEL_DE_REGISTRO;

        private static FileTarget archivoDeRegistro;
        private static ColoredConsoleTarget consolaDeRegistro;
        private static LoggingRule reglaConsola;
        private static LoggingRule reglaArchivo;

        private const string ENCABEZADO_16 = "Offset   | 0  1  2  3  4  5  6  7  8  9  A  B  C  D  E  F  | 0123456789ABCDEF";
        private const char ENCABEZADO_LINEA = '=';
        private const int BASE_HEXA = 16;

        public static void InicializaLog(ArchivoConfig lee, EnumNivelLog lvlLog, string fileName, string tipo = "")
        {
            try
            {
                DIRECTORIO_BASE = lee.LogPath;
                LONGITUD_MAXIMA_ARCHIVO = lee.LogMaxFileSize;
                NIVEL_DE_REGISTRO = lvlLog; //(LogLevel)lee.LevelLog;
                NOMBRE_ARCHIVO = fileName;
            }
            catch
            {
            }

            if (!Directory.Exists(DIRECTORIO_BASE))
            {
                Directory.CreateDirectory(DIRECTORIO_BASE);
            }

            LoggingConfiguration confLog = new LoggingConfiguration();

            consolaDeRegistro = new ColoredConsoleTarget
            {
                Name = "console",
                Layout = "${shortdate} ${level} ${message}"
            };

            archivoDeRegistro = new FileTarget
            {
                FileName = DIRECTORIO_BASE + NOMBRE_ARCHIVO,
                Layout = "${message}",
                ArchiveAboveSize = LONGITUD_MAXIMA_ARCHIVO,
                ArchiveNumbering = ArchiveNumberingMode.Sequence
            };

            LogLevel lv = LogLevel.Off;
            switch (NIVEL_DE_REGISTRO)
            {
                case EnumNivelLog.Trace: lv = LogLevel.Trace; break;
                case EnumNivelLog.Debug: lv = LogLevel.Debug; break;
                case EnumNivelLog.Info: lv = LogLevel.Info; break;
                case EnumNivelLog.Error: lv = LogLevel.Error; break;
                case EnumNivelLog.Warn: lv = LogLevel.Warn; break;
                case EnumNivelLog.Fatal: lv = LogLevel.Fatal; break;
            }

            reglaConsola = new LoggingRule("*", lv, consolaDeRegistro);
            reglaArchivo = new LoggingRule("*", lv, archivoDeRegistro);

            confLog.AddTarget("console", consolaDeRegistro);
            confLog.AddTarget("fileM", archivoDeRegistro);
            confLog.LoggingRules.Add(reglaConsola);
            confLog.LoggingRules.Add(reglaArchivo);
#if DEBUG
            LogManager.ThrowExceptions = true;
#endif
            LogManager.Configuration = confLog;
        }

        public static void InicializaLog(EnumNivelLog lvlLog, string fileName, string tipo = "")
        {
            DIRECTORIO_BASE = @"C:\Logs\";
            LONGITUD_MAXIMA_ARCHIVO = 512;
            NIVEL_DE_REGISTRO = lvlLog;
            NOMBRE_ARCHIVO = fileName;

            if (!Directory.Exists(DIRECTORIO_BASE))
            {
                Directory.CreateDirectory(DIRECTORIO_BASE);
            }

            LoggingConfiguration confLog = new LoggingConfiguration();

            consolaDeRegistro = new ColoredConsoleTarget
            {
                Name = "console",
                Layout = "${shortdate} ${level} ${message}"
            };

            archivoDeRegistro = new FileTarget
            {
                FileName = DIRECTORIO_BASE + NOMBRE_ARCHIVO,
                Layout = "${message}",
                ArchiveAboveSize = LONGITUD_MAXIMA_ARCHIVO,
                ArchiveNumbering = ArchiveNumberingMode.Sequence
            };

            LogLevel lv = LogLevel.Off;
            switch (NIVEL_DE_REGISTRO)
            {
                case EnumNivelLog.Trace: lv = LogLevel.Trace; break;
                case EnumNivelLog.Debug: lv = LogLevel.Debug; break;
                case EnumNivelLog.Info: lv = LogLevel.Info; break;
                case EnumNivelLog.Error: lv = LogLevel.Error; break;
                case EnumNivelLog.Warn: lv = LogLevel.Warn; break;
                case EnumNivelLog.Fatal: lv = LogLevel.Fatal; break;
            }

            reglaConsola = new NLog.Config.LoggingRule("*", lv, consolaDeRegistro);
            reglaArchivo = new NLog.Config.LoggingRule("*", lv, archivoDeRegistro);

            confLog.AddTarget("console", consolaDeRegistro);
            confLog.AddTarget("fileM", archivoDeRegistro);
            confLog.LoggingRules.Add(reglaConsola);
            confLog.LoggingRules.Add(reglaArchivo);
#if DEBUG
            LogManager.ThrowExceptions = true;
#endif
            LogManager.Configuration = confLog;
        }

        public static void LogMessage(string message, EnumNivelLog messageType, bool withTimeStamp)
        {
            if (withTimeStamp)
            {
                message = DateTime.Now.ToString("dd/MM/yy-H:mm:ss: ") + message;
            }

            LogManager.Configuration.Reload();

            // Logueo solo si el nivel de log del mensaje es mayor o igual al configurado
            if (messageType >= NIVEL_DE_REGISTRO)
            {
                switch (NIVEL_DE_REGISTRO)
                {
                    case EnumNivelLog.Trace: lo.Trace(message); break;
                    case EnumNivelLog.Debug: lo.Debug(message); break;
                    case EnumNivelLog.Info: lo.Info(message); break;
                    case EnumNivelLog.Warn: lo.Warn(message); break;
                    case EnumNivelLog.Error: lo.Error(message); break;
                    case EnumNivelLog.Fatal: lo.Fatal(message); break;

                }
            }
        }

        /// <summary>
        /// Registra un buffer de datos        
        /// </summary>
        /// <param name="buffer">Contenido principal</param>
        /// <param name="titulo">Titulo que </param>
        /// <param name="longitudBufferUsado">Longitud del buffer</param>
        /// <param name="nivelLog">Nivel del registrador</param>
        public static void LogBuffer(byte[] bufferInBytes, string titulo, int longitudBufferUsado, EnumNivelLog nivelLog)
        {
            var buffer = byteToChar(bufferInBytes);

            if (NIVEL_DE_REGISTRO.Equals(EnumNivelLog.Off))
            {
                // Esta configurado para no loguear nada
                return;
            }

            if (longitudBufferUsado > buffer.Length)
            {
                //ERROR: longitud no valida.
                LogMessage(string.Format("dump:Cantidad de buffer usado({0}) mayor que el tamaño del buffer({1}).\n", longitudBufferUsado, buffer.Length), EnumNivelLog.Error, false);
                return;
            }
            StringBuilder sb = new StringBuilder();

            //Registro título
            sb.AppendLine(titulo);

            string lineaDoble = string.Empty;
            string logRegistryNumber = string.Empty;
            string logEmptyChar = string.Empty;
            string loguerChar = "   ";
            string loguerCharNo16 = "    ";
            char oneItemBuffer = ' ';
            char[] numberToHexa = new char[16] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f' };

            switch (BASE_HEXA)
            {
                case 16:
                    //Logueo HEADER_16
                    sb.AppendLine(ENCABEZADO_16);

                    //Logueo linea doble
                    sb.AppendLine(lineaDoble.PadLeft(ENCABEZADO_16.Length, ENCABEZADO_LINEA));
                    break;

                default:
                    //ERROR: Base de numeracion no valida.
                    LogMessage(string.Format("dump:base={0} inválida.\n", BASE_HEXA), EnumNivelLog.Error, false);
                    return;
            }
            //Calculo cantidad de lineas que voy a tener que loguear
            int countLines;
            if (longitudBufferUsado % BASE_HEXA == 0)
            {
                countLines = longitudBufferUsado / BASE_HEXA;
            }
            else
            {
                countLines = (longitudBufferUsado / BASE_HEXA) + 1;
            }

            //Itero en la cantidad de lineas
            for (int i = 0; i < countLines; i++)
            {
                string oneLine = string.Empty;

                //Logueo numero de registro: 
                oneLine += intToHexa(i).PadLeft(7, '0') + "0 | ";

                //Itero dentro de una linea en cada valor a loguear
                for (int j = 0; j < BASE_HEXA; j++)
                {
                    if (i * BASE_HEXA + j >= longitudBufferUsado)
                    {
                        if (BASE_HEXA == 16)
                        {
                            oneLine += loguerChar;
                        }
                        else
                        {
                            oneLine += loguerCharNo16;
                        }
                    }
                    else
                    {
                        oneItemBuffer = buffer[i * BASE_HEXA + j];

                        oneLine += numberToHexa[oneItemBuffer >> 4].ToString() + numberToHexa[oneItemBuffer & 0xf].ToString() + " ";
                        break;
                    }
                }
                oneLine += "| ";

                //Itero dentro de una linea en cada valor a loguear como Ascii
                for (int j = 0; j < BASE_HEXA; j++)
                {
                    if (i * BASE_HEXA + j < longitudBufferUsado)
                    {
                        oneItemBuffer = buffer[i * BASE_HEXA + j];
                        if (oneItemBuffer > 0x1f && oneItemBuffer < 0x7f)
                        {
                            oneLine += oneItemBuffer;
                        }
                        else
                        {
                            oneLine += '.';
                        }
                    }
                    else
                    {
                        oneLine += " ";
                    }
                }
                sb.AppendLine(oneLine);
            }

            LogMessage(sb.ToString(), nivelLog, true);
        }

        public static void Test()
        {
            Console.Out.WriteLine("Vamos a suponer que estamos yendo a trabjar y para ello utilizamos el transporte público:");
            Console.Out.WriteLine("------------------------------------------------------------------");

            LogMessage("Trazar: La muchedumbre de gente en la calle.", EnumNivelLog.Trace, true);
            LogMessage("Depurar: ¿Hacia dónde nos dirijimos y porqué?", EnumNivelLog.Debug, true);
            LogMessage("Info: ¿En qué estación de omnibus estamos?", EnumNivelLog.Info, true);
            LogMessage("Advertir: Estamos distraidos y no estamos mirando si viene el omnibus.", EnumNivelLog.Warn, true);
            LogMessage("Error: Subimos al autobus incorrecto.", EnumNivelLog.Error, true);
            LogMessage("Fatal: Nos atropeyó el omnibus.", EnumNivelLog.Fatal, true);

            Console.Out.WriteLine("");
            Console.Out.WriteLine("Registro finalizado.");
            Console.Out.WriteLine("Presionar cualquier tecla para finalizar");

            Console.ReadKey();
        }

        // Conversiones
        private static string intToHexa(int decValue) => decValue.ToString("X");
                
        private static char[] byteToChar(byte[] bytes)
        {
            char[] ch = new char[bytes.Length];
            for (int i = 0; i < bytes.Length; i++)
            {
                ch[i] = (char)bytes[i];
            }
            return ch;
        }        
    }
}
