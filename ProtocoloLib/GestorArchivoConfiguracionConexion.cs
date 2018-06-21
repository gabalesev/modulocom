using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.IO;
using LibreriaClases;

namespace LibreriaMetodologia
{
    public class GestorArchivoConfiguracionConexion
    {

        public string crear_XMLprn(string nombre1, string nombre2, string nombre3, string port1, string port2, string port3, string tel1, string tel2, string tel3, ArchivoConfig conf)
        {

            try
            {
                string path = Directory.GetCurrentDirectory();

                path = path + conf.ArchivoPRN;

                XDocument miXML = new XDocument(new XDeclaration("1.0", "utf-8", "yes"),
                    new XElement("PRN",
                    new XElement("Nombre1", nombre1.Trim()),
                    new XElement("Nombre2", nombre2.Trim()),
                    new XElement("Nombre3", nombre3.Trim()),
                    new XElement("Port1", port1.Trim()), //6000
                    new XElement("Port2", port2.Trim()), //6500
                    new XElement("Port3", port3.Trim()), //7000
                    new XElement("Tel1", tel1.Trim()),
                    new XElement("Tel2", tel2.Trim()),
                    new XElement("Tel3", tel3.Trim())
                    ));
                miXML.Save(@path);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return null;

        }

        public string Leer_XMLprn(out string nombre1, out string nombre2, out string nombre3, out string port1, out string port2, out string port3, out string tel1, out string tel2, out string tel3, ArchivoConfig conf)
        {
            nombre1 = " ";
            nombre2 = " ";
            nombre3 = " ";
            port1 = " ";
            port2 = "";
            port3 = "";
            tel1 = "";
            tel2 = "";
            tel3 = " ";

            try
            {
                string path = Directory.GetCurrentDirectory();

                path = path + conf.ArchivoPRN;

                XDocument miXML = XDocument.Load(path);

                var seleccionados = from c in miXML.Descendants("PRN") select c.Element("Nombre1").Value;

                foreach (var item in seleccionados)
                {
                    nombre1 = item;
                }

                seleccionados = from c in miXML.Descendants("PRN") select c.Element("Nombre2").Value;

                foreach (var item in seleccionados)
                {
                    nombre2 = item;
                }

                seleccionados = from c in miXML.Descendants("PRN") select c.Element("Nombre3").Value;

                foreach (var item in seleccionados)
                {
                    nombre3 = item;
                }

                seleccionados = from c in miXML.Descendants("PRN") select c.Element("Port1").Value;

                foreach (var item in seleccionados)
                {
                    port1 = item;
                }
                seleccionados = from c in miXML.Descendants("PRN") select c.Element("Port2").Value;

                foreach (var item in seleccionados)
                {
                    port2 = item;
                }
                seleccionados = from c in miXML.Descendants("PRN") select c.Element("Port3").Value;

                foreach (var item in seleccionados)
                {
                    port3 = item;
                }
                seleccionados = from c in miXML.Descendants("PRN") select c.Element("Tel1").Value;

                foreach (var item in seleccionados)
                {
                    tel1 = item;
                }
                seleccionados = from c in miXML.Descendants("PRN") select c.Element("Tel2").Value;

                foreach (var item in seleccionados)
                {
                    tel2 = item;
                }
                seleccionados = from c in miXML.Descendants("PRN") select c.Element("Tel3").Value;

                foreach (var item in seleccionados)
                {
                    tel3 = item;
                }
                return null;

            }
            catch (FileNotFoundException)
            {
                return "NoFile";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }


        }

        public string Borrar_XMLprn(ArchivoConfig conf)
        {
            try
            {
                string path = conf.PathPRN;

                path = path + conf.ArchivoPRN;
                
                if (!File.Exists(path))
                    return "No existe archivo ArchivoPRN.";
                else
                    File.Delete(path);

                return null;
            }
            catch (FileNotFoundException)
            {
                return "NoFile";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        //public string Leer_XMLdefault(out string nombre, out string port, out string tel)
        //{
        //    nombre = " ";            
        //    port = " ";           
        //    tel = " ";

        //    try
        //    {
        //        string path = @"C:\BetmakerTP\Conexion"; //Directory.GetCurrentDirectory();

        //        path = path + @"\ConexionDefault.xml";

        //        XDocument miXML = XDocument.Load(path);

        //        var seleccionados = from c in miXML.Descendants("IPEndPoint") select c.Element("Nombre").Value;

        //        foreach (var item in seleccionados)
        //        {
        //            nombre = item;
        //        }

        //        seleccionados = from c in miXML.Descendants("IPEndPoint") select c.Element("Port").Value;

        //        foreach (var item in seleccionados)
        //        {
        //            port = item;
        //        }

        //        seleccionados = from c in miXML.Descendants("IPEndPoint") select c.Element("Tel").Value;

        //        foreach (var item in seleccionados)
        //        {
        //            tel = item;
        //        }                
        //        return null;

        //    }
        //    catch (FileNotFoundException)
        //    {
        //        return "C:\\BetmakerTP\\Conexion\\ConexionDefault.xml no existe.";
        //    }
        //    catch (Exception ex)
        //    {
        //        return ex.Message;
        //    }


        //}
    }
}
