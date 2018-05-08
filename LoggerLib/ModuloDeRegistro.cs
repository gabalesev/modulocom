using LibreriaClases;
using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.IO;
using System.Text;

namespace LibreriaRegistro
{
    public static class ModuloDeRegistro
    {
        private static readonly Logger registrador = LogManager.GetCurrentClassLogger();

        private const string ENCABEZADO_16 = "Linea | 0  1  2  3  4  5  6  7  8  9  A  B  C  D  E  F  | 0123456789ABCDEF";
        private const char ENCABEZADO_LINEA = '=';
        private const int BASE_HEXA = 16;

        private static LogLevel NIVEL_DE_REGISTRO;


        public static void InicializaRegistrador(string directorioBase, int longitudMaxima, LogLevel nivelRegistro, string nombreArchivo)
        {
            NIVEL_DE_REGISTRO = nivelRegistro;

            if (!Directory.Exists(directorioBase))
            {
                Directory.CreateDirectory(directorioBase);
            }

            var consolaDeRegistro = new ColoredConsoleTarget
            {
                Name = "console",
                Layout = "${level} ${message}"
            };

            var archivoDeRegistro = new FileTarget
            {
                FileName = directorioBase + nombreArchivo,
                Layout = "${level} ${message}",
                ArchiveAboveSize = longitudMaxima,
                ArchiveNumbering = ArchiveNumberingMode.Sequence
            };
            
            var reglaConsola = new LoggingRule("*", nivelRegistro, consolaDeRegistro);
            var reglaArchivo = new LoggingRule("*", nivelRegistro, archivoDeRegistro);

            LoggingConfiguration confLog = new LoggingConfiguration();
            confLog.AddTarget("console", consolaDeRegistro);
            confLog.AddTarget("fileM", archivoDeRegistro);
            confLog.LoggingRules.Add(reglaConsola);
            confLog.LoggingRules.Add(reglaArchivo);
#if DEBUG
            LogManager.ThrowExceptions = true;
#endif
            LogManager.Configuration = confLog;
        }

        public static void RegistrarMensaje(string mensaje, LogLevel nivelMensaje, bool incluirFechaHora)
        {
            if (incluirFechaHora)
            {
                mensaje = DateTime.Now.ToString("dd/MM/yy-H:mm:ss: ") + mensaje;
            }

            LogManager.Configuration.Reload();

            // Logueo solo si el nivel de log del mensaje es mayor o igual al configurado globalmente
            if (nivelMensaje >= NIVEL_DE_REGISTRO)
            {
                switch(nivelMensaje.Name)
                {
                    case "Trace": registrador.Trace(mensaje); break;
                    case "Debug": registrador.Debug(mensaje); break;
                    case "Info": registrador.Info(mensaje); break;
                    case "Warn": registrador.Warn(mensaje); break;
                    case "Error": registrador.Error(mensaje); break;
                    case "Fatal": registrador.Fatal(mensaje); break;
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
        public static void LogBuffer(byte[] bufferInBytes, string titulo, int longitudBufferUsado, LogLevel nivelLog)
        {
            var buffer = byteToChar(bufferInBytes);

            if (NIVEL_DE_REGISTRO.Equals(LogLevel.Off))
            {
                // Esta configurado para no loguear nada
                return;
            }

            if (longitudBufferUsado > buffer.Length)
            {
                //ERROR: longitud no valida.
                RegistrarMensaje(string.Format("Cantidad de buffer usado({0}) mayor que el tamaño del buffer({1}).\n", longitudBufferUsado, buffer.Length), LogLevel.Error, false);
                return;
            }
            StringBuilder sb = new StringBuilder();

            //Registro título
            sb.AppendLine(titulo);

            string lineaDoble = string.Empty;
            string logRegistryNumber = string.Empty;
            string logEmptyChar = string.Empty;
            string loguerChar = "   ";
            char oneItemBuffer = ' ';
            char[] numberToHexa = new char[16] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f' };

            //Logueo HEADER_16
            sb.AppendLine(ENCABEZADO_16);

            //Logueo linea doble
            sb.AppendLine(lineaDoble.PadLeft(ENCABEZADO_16.Length, ENCABEZADO_LINEA));

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
                oneLine += intToHexa(i).PadLeft(4, '0') + "0 | ";

                //Itero dentro de una linea en cada valor a loguear
                for (int j = 0; j < BASE_HEXA; j++)
                {
                    if (i * BASE_HEXA + j >= longitudBufferUsado)
                    {
                        oneLine += loguerChar;
                    }
                    else
                    {
                        oneItemBuffer = buffer[i * BASE_HEXA + j];

                        oneLine += numberToHexa[oneItemBuffer >> 4].ToString() + numberToHexa[oneItemBuffer & 0xf].ToString() + " ";                        
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

            RegistrarMensaje(sb.ToString(), nivelLog, true);
        }

        public static void Test()
        {
            Console.Out.WriteLine("Vamos a suponer que estamos yendo a trabjar y para ello utilizamos el transporte público:");
            Console.Out.WriteLine("------------------------------------------------------------------");

            RegistrarMensaje("Trazar: La muchedumbre de gente en la calle.", LogLevel.Trace, true);
            RegistrarMensaje("Depurar: ¿Hacia dónde nos dirijimos y porqué?", LogLevel.Debug, true);
            RegistrarMensaje("Info: ¿En qué estación de omnibus estamos?", LogLevel.Info, true);
            RegistrarMensaje("Advertir: Estamos distraidos y no estamos mirando si viene el omnibus.", LogLevel.Warn, true);
            RegistrarMensaje("Error: Subimos al autobus incorrecto.", LogLevel.Error, true);
            RegistrarMensaje("Fatal: Nos atropeyó el omnibus.", LogLevel.Fatal, true);

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
