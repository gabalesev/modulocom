using LibreriaClases;
using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoggerLib
{
    public static class LogBMTP
    {
        private static readonly Logger lo = LogManager.GetCurrentClassLogger();
        private static LogMessageGenerator logM;

        private static string BASE_DIR;//"C:\\TMLW\\LOGS\\";
        private static string FILE_NAME;//"SocketTCP.lg";
        private static int MAX_SIZE_FILE; // 512; //10MB
        private static bool NUMERING_WITH_SEQUENTIAL;//true;
        private static EnumNivelLog LEVEL_LOG;//EnumMessageType.DEBUG;

        // private static FileInfo fi = null;

        private static FileTarget fileTargetM;
        private static ColoredConsoleTarget console;
        private static LoggingRule logRuleConsole;
        private static LoggingRule logRuleFileM;

        #region Constantes
        private const string HEADER_16 = "Offset   | 0  1  2  3  4  5  6  7  8  9  A  B  C  D  E  F  | 0123456789ABCDEF";
        private const string HEADER_10 = "Offset   |  0   1   2   3   4   5   6   7   8   9  | 0123456789";
        private const string HEADER_08 = "Offset   |  0   1   2   3   4   5   6   7  | 01234567";
        private const string F_LAYOUT_MENSAJE = "{message}";
        private const string F_LAYOUT_BUFFER = "";
        private const char HEADER_LINE = '=';

        private const string BaseDir = "BaseDir";
        private const string FileName = "FileName";
        private const string MaxSizeFile = "MaxSizeFile";
        private const string NumeringWithSequential = "NumeringWithSequential";
        private const string LevelLog = "LevelLog";
        #endregion

        public static void InicializaLog(ArchivoConfig lee, EnumNivelLog lvlLog, string fileName, string tipo = "")
        {
            try
            {
                BASE_DIR = lee.LogPath;
                MAX_SIZE_FILE = lee.LogMaxFileSize;
                NUMERING_WITH_SEQUENTIAL = lee.NumeringWithSecuential;
                LEVEL_LOG = lvlLog; //(LogLevel)lee.LevelLog;
                FILE_NAME = fileName;
            }
            catch
            {
            }

            if (!Directory.Exists(BASE_DIR))
            {
                Directory.CreateDirectory(BASE_DIR);
            }

            LoggingConfiguration confLog = new LoggingConfiguration();

            console = new ColoredConsoleTarget
            {
                Name = "console",
                Layout = "${shortdate} ${level} ${message}"
            };

            fileTargetM = new FileTarget
            {
                FileName = BASE_DIR + FILE_NAME,
                Layout = "${message}",
                ArchiveAboveSize = MAX_SIZE_FILE,
                ArchiveNumbering = ArchiveNumberingMode.Sequence
            };

            LogLevel lv = LogLevel.Off;
            switch (LEVEL_LOG)
            {
                case EnumNivelLog.Trace: lv = LogLevel.Trace; break;
                case EnumNivelLog.Debug: lv = LogLevel.Debug; break;
                case EnumNivelLog.Info: lv = LogLevel.Info; break;
                case EnumNivelLog.Error: lv = LogLevel.Error; break;
                case EnumNivelLog.Warn: lv = LogLevel.Warn; break;
                case EnumNivelLog.Fatal: lv = LogLevel.Fatal; break;
            }

            logRuleConsole = new LoggingRule("*", lv, console);
            logRuleFileM = new LoggingRule("*", lv, fileTargetM);

            confLog.AddTarget("console", console);
            confLog.AddTarget("fileM", fileTargetM);
            confLog.LoggingRules.Add(logRuleConsole);
            confLog.LoggingRules.Add(logRuleFileM);
#if DEBUG
            LogManager.ThrowExceptions = true;
#endif
            LogManager.Configuration = confLog;

            logM += new LogMessageGenerator(LogMensaje);
        }

        public static void InicializaLog(EnumNivelLog lvlLog, string fileName, string tipo = "")
        {
            BASE_DIR = @"C:\Logs\";
            MAX_SIZE_FILE = 512;
            NUMERING_WITH_SEQUENTIAL = true;
            LEVEL_LOG = lvlLog;
            FILE_NAME = fileName;


            if (!Directory.Exists(BASE_DIR))
            {
                Directory.CreateDirectory(BASE_DIR);
            }

            LoggingConfiguration confLog = new LoggingConfiguration();

            console = new ColoredConsoleTarget
            {
                Name = "console",
                Layout = "${shortdate} ${level} ${message}"
            };

            fileTargetM = new FileTarget
            {
                FileName = BASE_DIR + FILE_NAME,
                Layout = "${message}",
                ArchiveAboveSize = MAX_SIZE_FILE,
                ArchiveNumbering = ArchiveNumberingMode.Sequence
            };

            LogLevel lv = LogLevel.Off;
            switch (LEVEL_LOG)
            {
                case EnumNivelLog.Trace: lv = LogLevel.Trace; break;
                case EnumNivelLog.Debug: lv = LogLevel.Debug; break;
                case EnumNivelLog.Info: lv = LogLevel.Info; break;
                case EnumNivelLog.Error: lv = LogLevel.Error; break;
                case EnumNivelLog.Warn: lv = LogLevel.Warn; break;
                case EnumNivelLog.Fatal: lv = LogLevel.Fatal; break;
            }

            logRuleConsole = new NLog.Config.LoggingRule("*", lv, console);
            logRuleFileM = new NLog.Config.LoggingRule("*", lv, fileTargetM);

            confLog.AddTarget("console", console);
            confLog.AddTarget("fileM", fileTargetM);
            confLog.LoggingRules.Add(logRuleConsole);
            confLog.LoggingRules.Add(logRuleFileM);
#if DEBUG
            LogManager.ThrowExceptions = true;
#endif
            LogManager.Configuration = confLog;

            logM += new LogMessageGenerator(LogMensaje);
        }

        private static string LogMensaje() => "LogMessageGenerator Handler";
        public static void LogMessage(string message, EnumNivelLog messageType, bool withTimeStamp)
        {
            if (withTimeStamp)
            {
                message = DateTime.Now.ToString("dd/MM/yy-H:mm:ss: ") + message;
            }

            LogManager.Configuration.Reload();

            // Logueo solo si el nivel de log del mensaje es mayor o igual al configurado
            if (messageType >= LEVEL_LOG)
            {
                switch (LEVEL_LOG)
                {
                    case EnumNivelLog.Trace: lo.Trace(message, logM); break;
                    case EnumNivelLog.Debug: lo.Debug(message, logM); break;
                    case EnumNivelLog.Info: lo.Info(message, logM); break;
                    case EnumNivelLog.Warn: lo.Warn(message, logM); break;
                    case EnumNivelLog.Error: lo.Error(message, logM); break;
                    case EnumNivelLog.Fatal: lo.Fatal(message, logM); break;

                }
            }
        }

        public static void LogBuffer(char[] buffer, string title, int lenghtUsedBuffer, EnumNivelLog nivelLog, int numberBase = 16)
        {
            if (LEVEL_LOG.Equals(EnumMessageType.NOTHING))
            {
                // Esta configurado para no loguear nada
                return;
            }

            if (lenghtUsedBuffer > buffer.Length)
            {
                //ERROR: Base de numeracion no valida.
                LogMessage(string.Format("dump:Cantidad de buffer usado({0}) mayor que el tamaño del buffer({1}).\n", lenghtUsedBuffer, buffer.Length), EnumNivelLog.Error, false);
                return;
            }
            StringBuilder sb = new StringBuilder();
            //Logueo Title
            sb.AppendLine(title);

            string lineaDoble = string.Empty;
            string logRegistryNumber = string.Empty;
            string logEmptyChar = string.Empty;
            string loguerChar = "   ";
            string loguerCharNo16 = "    ";
            char oneItemBuffer = ' ';
            char[] numberToHexa = new char[16] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f' };

            switch (numberBase)
            {
                case 16:
                    //Logueo HEADER_16
                    sb.AppendLine(HEADER_16);

                    //Logueo linea doble
                    sb.AppendLine(lineaDoble.PadLeft(HEADER_16.Length, HEADER_LINE));
                    break;
                case 10:
                    //Logueo HEADER_10
                    sb.AppendLine(HEADER_10);

                    //Logueo linea doble
                    sb.AppendLine(lineaDoble.PadLeft(HEADER_10.Length, HEADER_LINE));
                    break;
                /*case 8:
                    //Logueo HEADER_08
                    sb.AppendLine(HEADER_08);

                    //Logueo linea doble
                    sb.AppendLine(lineaDoble.PadLeft(HEADER_08.Length, HEADER_LINE));
                    break;*/
                default:
                    //ERROR: Base de numeracion no valida.
                    LogMessage(string.Format("dump:base={0} inválida.\n", numberBase), EnumNivelLog.Error, false);
                    return;
            }
            //Calculo cantidad de lineas que voy a tener que loguear
            int countLines;
            if (lenghtUsedBuffer % numberBase == 0)
            {
                countLines = lenghtUsedBuffer / numberBase;
            }
            else
            {
                countLines = (lenghtUsedBuffer / numberBase) + 1;
            }

            //Itero en la cantidad de lineas
            for (int i = 0; i < countLines; i++)
            {
                string oneLine = string.Empty;

                //Logueo numero de registro: 
                switch (numberBase)
                {
                    case 16:
                        oneLine += intToHexa(i).PadLeft(7, '0') + "0 | ";
                        break;
                    case 10:
                        oneLine += i.ToString().PadLeft(7, '0') + "0 | ";
                        break;
                        /*case 8:
                            oneLine += intToOct(i).PadLeft(7, '0') + "0 | ";
                            break;*/
                }
                //Itero dentro de una linea en cada valor a loguear
                for (int j = 0; j < numberBase; j++)
                {
                    if (i * numberBase + j >= lenghtUsedBuffer)
                    {
                        if (numberBase == 16)
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
                        oneItemBuffer = buffer[i * numberBase + j];
                        switch (numberBase)
                        {
                            case 16:
                                oneLine += numberToHexa[oneItemBuffer >> 4].ToString() + numberToHexa[oneItemBuffer & 0xf].ToString() + " ";
                                break;
                            case 10:
                                oneLine += string.Format("{0} ", Convert.ToByte(oneItemBuffer).ToString().PadLeft(3, '0'));
                                break;
                                /*case 8:
                                    oneLine += string.Format("{0} ", Convert.ToByte(oneItemBuffer).ToString().PadLeft(3, '0'));
                                    break;*/
                        }
                    }
                }
                oneLine += "| ";
                //Itero dentro de una linea en cada valor a loguear como Ascii
                for (int j = 0; j < numberBase; j++)
                {
                    if (i * numberBase + j < lenghtUsedBuffer)
                    {
                        oneItemBuffer = buffer[i * numberBase + j];
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
            //lo.Trace("Trace: The chatter of people on the street");
            LogMessage("Depurar: ¿Hacia dónde nos dirijimos y porqué?", EnumNivelLog.Debug, true);
            //lo.Debug("Debug: Where are you going and why?");
            LogMessage("Info: ¿En qué estación de omnibus estamos?", EnumNivelLog.Info, true);
            //lo.Info("Info: What bus station you're at.");
            LogMessage("Advertir: Estamos distraidos y no estamos mirando si viene el omnibus.", EnumNivelLog.Warn, true);
            //lo.Warn("Warn: You're playing on the phone and not looking up for your bus");
            LogMessage("Error: Subimos al autobus incorrecto.", EnumNivelLog.Error, true);
            //lo.Error("Error: You get on the wrong bus.");
            LogMessage("Fatal: Nos atropeyó el omnibus.", EnumNivelLog.Fatal, true);
            //lo.Fatal("Fatal: You are run over by the bus.");

            Console.Out.WriteLine("");
            Console.Out.WriteLine("Registro finalizado.");
            Console.Out.WriteLine("Presionar cualquier tecla para finalizar");

            Console.ReadKey();
        }

        private static string intToHexa(int decValue) => decValue.ToString("X");

        private static int hexaToInt(string hexValue) => int.Parse(hexValue, System.Globalization.NumberStyles.HexNumber);
    }
}
