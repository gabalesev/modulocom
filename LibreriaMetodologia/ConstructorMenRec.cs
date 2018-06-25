using System;
using System.Collections.Generic;
using MonoLibrary;
using System.Collections;
using LibreriaClases;
using LibreriaClases.Clases;
using LibreriaConversiones;

namespace LibreriaMetodologia
{
    static class ConstructorMenRec
    {        
        // Arma mensaje recibido B
        public static IList recibeMensajeB(byte[] byteRec)
        {
            Agente agente = new Agente();
            PRN prn = new PRN(); // Archivo de configuración de conexión
            Error err = new Error();
            Terminal ter = new Terminal();
            
            err.CodError = (ushort)Conversiones.AgregaDigito16(byteRec, 1);
            ter.indiceTelecarga = (byte)Conversiones.AgregaDigito(byteRec, 3);
            ter.longArchivo = (UInt32)Conversiones.AgregaDigito32(byteRec, 4);
            ter.correo = (byte)Conversiones.AgregaDigito(byteRec, 8);

            prn.Nombre1 = Conversiones.AgregaCadena(byteRec, 9, 10);
            prn.Nombre2 = Conversiones.AgregaCadena(byteRec, 19, 10);
            prn.Nombre3 = Conversiones.AgregaCadena(byteRec, 29, 10);

            prn.Port1 = Conversiones.AgregaCadena(byteRec, 39, 15);
            prn.Port2 = Conversiones.AgregaCadena(byteRec, 54, 15);
            prn.Port3 = Conversiones.AgregaCadena(byteRec, 69, 15);

            prn.Telefono1 = Conversiones.AgregaCadena(byteRec, 84, 15);
            prn.Telefono2 = Conversiones.AgregaCadena(byteRec, 99, 15);
            prn.Telefono3 = Conversiones.AgregaCadena(byteRec, 114, 15);

            int aux = (int)Conversiones.AgregaDigito32(byteRec, 129);

            agente.Numero = aux;
            agente.Nombre = Conversiones.AgregaCadena(byteRec, 133, 40);
            agente.Direccion = Conversiones.AgregaCadena(byteRec, 173, 40);
            agente.Localidad = Conversiones.AgregaCadena(byteRec, 213, 25);

            ter.defLengConc1 = (uint)Conversiones.AgregaDigito16(byteRec, 238);
            ter.defLengConc2 = (uint)Conversiones.AgregaDigito16(byteRec, 240);
            ter.statusConc = Conversiones.AgregaCadena(byteRec, 242, 3);

            IList menB = new List<object> { err, agente, prn, ter }; 
            return menB;
        }

        // Arma mensaje de error B recibido
        public static IList recibeMensajeBErr(byte[] byteRec)
        {
            Array.Resize(ref byteRec, 169);
            
            PRN prn = new PRN();
            Error err = new Error();
            
            err.CodError = (UInt16)Conversiones.AgregaDigito16(byteRec, 1);

            prn.Nombre1 = Conversiones.AgregaCadena(byteRec, 9, 10);
            prn.Nombre2 = Conversiones.AgregaCadena(byteRec, 19, 10);
            prn.Nombre3 = Conversiones.AgregaCadena(byteRec, 29, 10);

            prn.Port1 = Conversiones.AgregaCadena(byteRec, 39, 15);
            prn.Port2 = Conversiones.AgregaCadena(byteRec, 54, 15);
            prn.Port3 = Conversiones.AgregaCadena(byteRec, 69, 15);

            prn.Telefono1 = Conversiones.AgregaCadena(byteRec, 84, 15);
            prn.Telefono2 = Conversiones.AgregaCadena(byteRec, 99, 15);
            prn.Telefono3 = Conversiones.AgregaCadena(byteRec, 114, 15);

            err.Descripcion = Conversiones.AgregaCadena(byteRec, 129, 40);

            IList menB = new List<object> { err, prn }; 
            return menB;
        }
        
        
    }
}
