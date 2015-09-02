using BinConfig;
using LibreriaClases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ProtocoloLib
{
    public class ProtocoloConfig
    {
        public ArchivoConfig CONFIG { get; set; }
        public BaseConfig BASE_CONFIG { get; set; }
        
        
        public bool CON_CICLO_PRN { get; set; }
        public UInt16 LOCAL_PORT { get; set; }
        public IPAddress LOCAL_IP { get; set; }
        public EnumModoConexion TIPO_CXN;

        public const Int32 TamBufferReporte = 32000;
        public const Int32 TamBuffer = 1024;
        public const Int32 TimeoutSocket = 4000;
        
        public SerialNumeroOff nroOff; 

        public NackEnv NACK_ENV { get; set; }
        public NackRec NACK_REC { get; set; }

        public byte[] CLAVE_TARJETA;
        public byte[] PROTOCOLO;

        public ProtocoloConfig(BaseConfig baseConf, ArchivoConfig conf, bool conCicloPRN = true)
        {            
            nroOff = new SerialNumeroOff();

            CONFIG = conf;
            BASE_CONFIG = baseConf;
            CON_CICLO_PRN = conCicloPRN;

            CLAVE_TARJETA = new byte[8];
            PROTOCOLO = new byte[84];

            string nroTerStr = BASE_CONFIG.Terminal.ToString();
            LOCAL_PORT = Convert.ToUInt16("5" + nroTerStr.Substring(nroTerStr.Length - 4, 4));                        
        }
        public ProtocoloConfig(ArchivoConfig conf)
        {            
            CONFIG = conf;         
        }        
    }
}
