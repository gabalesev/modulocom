using BinConfig;
using LibreriaClases;
using System;
using System.Net;

namespace LibreriaMetodologia
{
    public class ConfiguracionComunicacion
    {
        public ArchivoConfig Configuracion { get; set; }
        public BaseConfig BASE_CONFIG { get; set; }        
        
        public bool UsaCicloPRN { get; set; }
        public UInt16 LocalPort { get; set; }
        public IPAddress LocalIP { get; set; }
        public EnumModoConexion TipoConexion;

        public readonly Int32 LongitudBuffer = 1024;
        public readonly Int32 TimeoutSocket = 8000;
        
        //public SerialNumeroOff nroOff; 

        public NackEnv NackEnvio { get; set; }
        public NackRec NackRecepcion { get; set; }
        
        //public byte[] PROTOCOLO;

        public ConfiguracionComunicacion(BaseConfig baseConf, ArchivoConfig conf, bool conCicloPRN = true)
        {            
            //nroOff = new SerialNumeroOff();

            Configuracion = conf;
            BASE_CONFIG = baseConf;
            UsaCicloPRN = conCicloPRN;
            
            //PROTOCOLO = new byte[84];

            string nroTerStr = BASE_CONFIG.Terminal.ToString();
            LocalPort = Convert.ToUInt16("5" + nroTerStr.Substring(nroTerStr.Length - 4, 4));                        
        }
        public ConfiguracionComunicacion(ArchivoConfig conf)
        {            
            Configuracion = conf;         
        }        
    }
}
