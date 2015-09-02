using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MonoLibrary;
using Certificar;
using System.Collections;
using LibreriaClases;
using LibreriaClases.Clases;
using BinConfig;
using SqlManager;

namespace ProtocoloLib
{
    static class ConstructorMenRec
    {
        public static byte TIPO_TRANSACC = 0x00;
        //---------(B)---------//
        public static IList recibeMensajeB(byte[] byteRec)
        {
            //Array.Resize(ref byteRec, 245);

            //ParamConexion paramConex = new ParamConexion();
            Agente agente = new Agente();
            PRN prn = new PRN();
            Error err = new Error();
            Terminal ter = new Terminal();
            
            //paramConex.codPaquete = Conversiones.AgregaCadena(byteRec, 0, 1); // tipo de mensaje
            err.CodError = (ushort)Conversiones.AgregaDigito16(byteRec, 1); // codigo error
            ter.indiceTelecarga = (byte)Conversiones.AgregaDigito(byteRec, 3); // flag long file
            ter.longArchivo = (UInt32)Conversiones.AgregaDigito32(byteRec, 4); // long file
            ter.correo = (byte)Conversiones.AgregaDigito(byteRec, 8); // correo

            prn.Nombre1 = Conversiones.AgregaCadena(byteRec, 9, 10); //Usuario 0
            prn.Nombre2 = Conversiones.AgregaCadena(byteRec, 19, 10); //Usuario 1
            prn.Nombre3 = Conversiones.AgregaCadena(byteRec, 29, 10); //Usuario 2

            prn.Port1 = Conversiones.AgregaCadena(byteRec, 39, 15); // Direccion 0 
            prn.Port2 = Conversiones.AgregaCadena(byteRec, 54, 15); // Direccion 1 
            prn.Port3 = Conversiones.AgregaCadena(byteRec, 69, 15); // Direccion 2 

            prn.Telefono1 = Conversiones.AgregaCadena(byteRec, 84, 15); // Telefono 0
            prn.Telefono2 = Conversiones.AgregaCadena(byteRec, 99, 15); // Telefono 1
            prn.Telefono3 = Conversiones.AgregaCadena(byteRec, 114, 15); // Telefono 2

            int aux = (int)Conversiones.AgregaDigito32(byteRec, 129);

            agente.Numero = aux;
            agente.Nombre = Conversiones.AgregaCadena(byteRec, 133, 40); // Nombre de la agencia
            agente.Direccion = Conversiones.AgregaCadena(byteRec, 173, 40); // Direccion de la agencia
            agente.Localidad = Conversiones.AgregaCadena(byteRec, 213, 25); // Localidad de la agencia    

            ter.defLengConc1 = (uint)Conversiones.AgregaDigito16(byteRec, 238);
            ter.defLengConc2 = (uint)Conversiones.AgregaDigito16(byteRec, 240);
            ter.statusConc = Conversiones.AgregaCadena(byteRec, 242, 3);

            IList menB = new List<object> { err, agente, prn, ter }; 
            return menB;
        }
        public static IList recibeMensajeBErr(byte[] byteRec)
        {
            Array.Resize(ref byteRec, 169);
            
            PRN prn = new PRN();
            Error err = new Error();

            //codPaquete = Conversiones.AgregaCadena(byteRec, 0, 1);// Cod. de Paquete
            err.CodError = (UInt16)Conversiones.AgregaDigito16(byteRec, 1);// Cod. de ERROR
            //flagLongFile = (byte)Conversiones.AgregaDigito(byteRec, 3, 1);// FLag LongFile
            //longFile = (UInt32)Conversiones.AgregaDigito32(byteRec, 4, 4);// LongFile
            //correo = (byte)Conversiones.AgregaDigito(byteRec, 8, 1);// Correo

            prn.Nombre1 = Conversiones.AgregaCadena(byteRec, 9, 10); //  Usuario 0
            prn.Nombre2 = Conversiones.AgregaCadena(byteRec, 19, 10); // Usuario 1
            prn.Nombre3 = Conversiones.AgregaCadena(byteRec, 29, 10); // Usuario 2

            prn.Port1 = Conversiones.AgregaCadena(byteRec, 39, 15); // Direccion 0 //DUDA: string o int?
            prn.Port2 = Conversiones.AgregaCadena(byteRec, 54, 15); // Direccion 1 //DUDA: string o int?
            prn.Port3 = Conversiones.AgregaCadena(byteRec, 69, 15); // Direccion 2 //DUDA: string o int?

            prn.Telefono1 = Conversiones.AgregaCadena(byteRec, 84, 15); // Telefono 0
            prn.Telefono2 = Conversiones.AgregaCadena(byteRec, 99, 15); // Telefono 1
            prn.Telefono3 = Conversiones.AgregaCadena(byteRec, 114, 15); // Telefono 2

            err.Descripcion = Conversiones.AgregaCadena(byteRec, 129, 40); // Mensaje de ERROR

            IList menB = new List<object> { err, prn }; 
            return menB;
        }

        public static IList recibeMensajeQ1_Quiniela(byte[] byteRec) 
        {
            bool errorProtocolo = false;         
            ParamSorteoQuiniela paramSQ = new ParamSorteoQuiniela();
            Error err = new Error();

            Conversiones.AgregaCadena(byteRec, 0, 1); //Letra "Q"
            TIPO_TRANSACC = paramSQ.Tipo = (byte)Conversiones.AgregaDigito(byteRec, 1); //Tipo de juego
            err.CodError = (UInt16)Conversiones.AgregaDigito16(byteRec, 2); //Cod. de error
            //Conversiones.AgregaCadena(byteRec, 4, 18);//Id ticket vacío

            paramSQ.FechaHora = new DateTime(Conversiones.AgregaDigito16(byteRec, 24), Conversiones.AgregaDigito(byteRec, 23),
                Conversiones.AgregaDigito(byteRec, 22), Conversiones.AgregaDigito(byteRec, 26), Conversiones.AgregaDigito(byteRec, 27),
                Conversiones.AgregaDigito(byteRec, 28));

            paramSQ.EntesCant = (uint)Conversiones.AgregaDigito(byteRec, 29); //Cant de entes

            paramSQ.ImporteMaxApuesta = (UInt32)Conversiones.AgregaDigito32(byteRec, 30); //Importe Max Apuesta
            paramSQ.ImporteMinApuesta = (UInt16)Conversiones.AgregaDigito16(byteRec, 34); //Importe Min Apuesta
            paramSQ.ImporteMaxTicket = (UInt32)Conversiones.AgregaDigito32(byteRec, 36); //Importe Max Ticket
            paramSQ.ImporteMinTicket = (UInt16)Conversiones.AgregaDigito16(byteRec, 40); //Importe Min Ticket
            paramSQ.Incremento = (uint)Conversiones.AgregaDigito(byteRec, 42); //Incremento
            paramSQ.CantApuestas = (uint)Conversiones.AgregaDigito(byteRec, 43); //Cant de apuestas
            
            int lonArr = 44;
            paramSQ.EntesNombres = new string[paramSQ.EntesCant];
            for (int i = 0; i <= (paramSQ.EntesCant - 1); i++)
            {
                paramSQ.EntesNombres[i] = Conversiones.AgregaCadena(byteRec, lonArr, 10); //Nombre ente
                lonArr += 10;
            }

            paramSQ.SorteosFechas = new DateTime[10];
            paramSQ.SorteosFechaCierreOff = new DateTime[10];
            paramSQ.SorteosFechasCierre = new DateTime[10];
            
            paramSQ.SorteosNumeros = new ushort[10];
            paramSQ.SorteosNombres = new string[10];
            paramSQ.SorteosCantCifras = new uint[10];
            int[,] bmpran1 = new int[10,10];
            int[,] bmpran2 = new int[10,10];
            paramSQ.SorteoBmpEntes = new byte[10];
            paramSQ.SorteosCantLineas = new uint[10];
            paramSQ.TextosTicket = new string[20];
            
            int contSor = 0, lenBpm1 = 0, lenBpm2 = 0;
            while (lonArr < byteRec.Length && byteRec[lonArr] == 0xd1) //datos por horario de sorteo: mañana, tarde, noche
            {                
                Conversiones.AgregaDigito(byteRec, lonArr); //tipo de juego                

                paramSQ.SorteosFechas[contSor] = new DateTime(Conversiones.AgregaDigito16(byteRec, lonArr +3), Conversiones.AgregaDigito(byteRec, lonArr + 2),
                Conversiones.AgregaDigito(byteRec, lonArr+1)); //Fecha de apertura                                

                paramSQ.SorteosFechasCierre[contSor] = new DateTime(Conversiones.AgregaDigito16(byteRec, lonArr+7), Conversiones.AgregaDigito(byteRec, lonArr+6),
                Conversiones.AgregaDigito(byteRec, lonArr+5), Conversiones.AgregaDigito(byteRec, lonArr+9), Conversiones.AgregaDigito(byteRec, lonArr+10),
                Conversiones.AgregaDigito(byteRec, lonArr+11)); //Fecha y hora de cierre
                
                    if(Conversiones.EsFechaHoraValida(byteRec, ref paramSQ.SorteosFechaCierreOff[contSor], lonArr + 12, 7))
                    {
                        lonArr = lonArr + 7;
                    }
                    else
                        paramSQ.SorteosFechaCierreOff[contSor] = paramSQ.SorteosFechasCierre[contSor];
                
                paramSQ.SorteosNumeros[contSor] = (ushort)Conversiones.AgregaDigito32(byteRec, lonArr + 12); //SORTEO <-------------------
                paramSQ.SorteosNombres[contSor] = Conversiones.AgregaCadena(byteRec, lonArr + 16, 10); //nombre
                paramSQ.SorteosCantCifras[contSor] = (uint)Conversiones.AgregaDigito(byteRec, lonArr + 26); //cantidad de cifras
                
                byte[] bmpr1 = { byteRec[lonArr + 27], byteRec[lonArr + 28], byteRec[lonArr + 29] }; //bmprango1
                int[] bmpRangos1 = Conversiones.AgregaRangos(bmpr1);
                //byte[] bmpRanPrueba = Conversiones.ObtenerBits(bmpr1);
                //byte[] bmprPrueba = Conversiones.SeteaBits(bmpRanPrueba, bmpRanPrueba.Length);
                lenBpm1 = bmpRangos1.Length;
                for (int i = 0; i <= (lenBpm1-1); i++)
                {
                    bmpran1[contSor, i] = bmpRangos1[i];
                }                

                byte[] bmpr2 = { byteRec[lonArr + 30], byteRec[lonArr + 31], byteRec[lonArr + 32] }; //bmprango2
                int[] bmpRangos2 = Conversiones.AgregaRangos(bmpr2);
                lenBpm2 = bmpRangos2.Length;
                for (int i = 0; i <= (lenBpm2-1); i++)
                {
                    bmpran2[contSor, i] = bmpRangos2[i];
                }
                paramSQ.SorteoBmpEntes[contSor] = (byte)Conversiones.AgregaDigito(byteRec, lonArr + 33); //bmpentes

                //byte[] entes = Conversiones.ObtenerBits(paramSQ.SorteoBmpEntes);

                paramSQ.SorteosCantLineas[contSor] = (uint)Conversiones.AgregaDigito(byteRec, lonArr + 34); //cant de lineas --> CREO QUE ES BYTE CON LONG DE UN MENSAJE QUE CONTINUA DESPUES

                if (paramSQ.SorteosCantLineas[contSor] != 0)
                {                    
                    paramSQ.TextosTicket[contSor] = Conversiones.AgregaCadena(byteRec, lonArr + 35, ((int)paramSQ.SorteosCantLineas[contSor]) * 40);
                    lonArr += ((int)paramSQ.SorteosCantLineas[contSor]) * 40;                    
                }              
                contSor++;
                lonArr += 35;
            }

            if (paramSQ.SorteosNombres[0] == null) errorProtocolo = true;

            Array.Resize(ref paramSQ.TextosTicket, contSor);
            Array.Resize(ref paramSQ.SorteosFechas, contSor);
            Array.Resize(ref paramSQ.SorteosFechas, contSor);
            Array.Resize(ref paramSQ.SorteosFechasCierre, contSor);
            Array.Resize(ref paramSQ.SorteosFechaCierreOff, contSor);
            Array.Resize(ref paramSQ.SorteosNumeros, contSor);
            Array.Resize(ref paramSQ.SorteosNombres, contSor);
            Array.Resize(ref paramSQ.SorteosCantCifras, contSor);
            paramSQ.SorteosBmprango1 = Conversiones.ResizeArray(bmpran1, contSor, lenBpm1);
            paramSQ.SorteosBmprango2 = Conversiones.ResizeArray(bmpran2, contSor, lenBpm2);
            Array.Resize(ref paramSQ.SorteoBmpEntes, contSor);
            Array.Resize(ref paramSQ.SorteosCantLineas, contSor);           

            if (lonArr < byteRec.Length && byteRec[lonArr] != 0x00)
            {
                paramSQ.JuegoAdicionalCodigo = (byte)Conversiones.AgregaDigito(byteRec, lonArr);
                paramSQ.JuegoAdicionalNombre = Conversiones.AgregaCadena(byteRec, lonArr + 1, 10);
                paramSQ.JuegoAdicionalCant = (uint)Conversiones.AgregaDigito(byteRec, lonArr + 11);
                paramSQ.JuegoAdicionalDigitos = (uint)Conversiones.AgregaDigito(byteRec, lonArr + 12);
                paramSQ.JuegoAdicionalImporte = (uint)Conversiones.AgregaDigito16(byteRec, lonArr + 13);
                lonArr += 2;                
            }


            IList objsQ1;
            if (errorProtocolo)
            {
                err.CodError = (uint)ErrComunicacion.PROTOCOLO;
                err.Descripcion = "Ocurrió un error en la interpretación del protocolo para parametros de Quiniela.";
                err.Estado = 2;
                objsQ1 = new List<object> { err };               
            }
            else
            {
                objsQ1 = new List<object> { err, paramSQ };
            }               
            
            return objsQ1;
        }

        public static IList recibeMensajeQ2(byte[] byteRec)
        {
            Array.Resize(ref byteRec, 29);
            Error err = new Error();
            err.CodError = (uint)Conversiones.AgregaDigito16(byteRec, 2); //Cod. de error
            
            IList objsQ2 = new List<object>();
            objsQ2.Add(err);

            byte tipoTrans = (byte)Conversiones.AgregaDigito(byteRec, 1);

            
                TransacQuinielaH transaccRta = new TransacQuinielaH();// En esta clase se llenan campos que no se llenaron al enviarla.
                
                Conversiones.AgregaCadena(byteRec, 0, 1); //Letra "Q"
                transaccRta.TipoTransacc = (byte)Conversiones.AgregaDigito(byteRec, 1); //Tipo de transacción

                 
                transaccRta.id_transacc = Convert.ToUInt32(Conversiones.AgregaCadena(byteRec, 4, 18)); //Id ticket

                Transformar conv = new Transformar();

                transaccRta.id_ticket = conv.GenerateTicketIdst((transaccRta.id_transacc));

                transaccRta.Timehost = new DateTime(Conversiones.AgregaDigito16(byteRec, 24), Conversiones.AgregaDigito(byteRec, 23),
                    Conversiones.AgregaDigito(byteRec, 22), Conversiones.AgregaDigito(byteRec, 26), Conversiones.AgregaDigito(byteRec, 27),
                    Conversiones.AgregaDigito(byteRec, 28));

                //Certificado ft = new Certificado();

                transaccRta.Protocolo = TransacManager.ProtoConfig.PROTOCOLO;
                //Si se le pasa con CDs y TARJETA no funciona, aunque pasemos la longitud del protocolo sin contar los CDs y 
                //TARJETA, da la misma excepcion de incompatibilidad de longitudes.
                //ft.CertificadoQuiniela(Comunicacion.PROTOCOLO, Comunicacion.CLAVE_TARJETA, Comunicacion, ref transaccRta.Certificado, (int)EnumCertificado.TIPOCHECK,
                //    new QuinielaCheck());

                objsQ2.Add(transaccRta);
            
            
            return objsQ2;
        }

        public static IList recibeMensajeQErr(byte[] byteRec)
        {
            //Array.Resize(ref byteRec, 44);
            
            Error err = new Error();
            
            string codPaquete = Conversiones.AgregaCadena(byteRec, 0, 1); //Cod. de paquete
            Conversiones.AgregaDigito(byteRec, 1); //Tipo de transacción

            if (byteRec.Length == 44)
            {
                err.CodError = (uint)Conversiones.AgregaDigito16(byteRec, 2); //Cod. de error
                err.Descripcion = Conversiones.AgregaCadena(byteRec, 4, 39); //Descripción
            }
            else if (byteRec.Length == 46 || codPaquete == "L")
            {
                err.CodError = (uint)Conversiones.AgregaDigito32(byteRec, 1); //Cod. de error
                err.Descripcion = Conversiones.AgregaCadena(byteRec, 5, 39); //Descripción
            }
            else if (byteRec.Length == 49)
            {
                err.CodError = (uint)Conversiones.AgregaDigito32(byteRec, 5); //Cod. de error
                err.Descripcion = Conversiones.AgregaCadena(byteRec, 9, 39); //Descripción
            }
            else if (byteRec.Length == 48)
            {
                err.CodError = (uint)Conversiones.AgregaDigito16(byteRec, 6); //Cod. de error
                err.Descripcion = Conversiones.AgregaCadena(byteRec, 8, 40); //Descripción
            }            
            else if (byteRec.Length == 45)
            {
                err.CodError = (uint)Conversiones.AgregaDigito16(byteRec, 2); //Cod. de error
                err.Descripcion = Conversiones.AgregaCadena(byteRec, 4, 40); //Descripción
            }
            IList menQError = new List<object> { err };
            return menQError;
        }
    }
}
