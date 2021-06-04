using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using ClassLib;
namespace RPC
{
    public class ServerSerializer
    {
        public static string SerializeRequest(Request request)
        {
            XmlSerializer ser = new XmlSerializer(typeof(Request));
            StringWriter writer = new StringWriter();
            ser.Serialize(writer, request);
            writer.Close();
            return writer.ToString();
        }
        public static Request DeserlizeRequest(string xmlData)
        {            
            XmlSerializer ser = new XmlSerializer(typeof(Request));
            StringReader reader = new StringReader(xmlData);
            Request value = (Request)ser.Deserialize(reader);
            reader.Close();
            return value;
        }
        public static string SerializeResponse<T>(Response<T> response)
        {
            XmlSerializer ser = new XmlSerializer(typeof(Response<T>));
            StringWriter writer = new StringWriter();
            ser.Serialize(writer, response);
            writer.Close();
            return writer.ToString();
        }
        public static Response<T> DeserializeResponse<T>(string xmlData)
        {
            XmlSerializer ser = new XmlSerializer(typeof(Response<T>));
            StringReader reader = new StringReader(xmlData);
            Response<T> value = (Response<T>)ser.Deserialize(reader);
            reader.Close();
            return value;
        }
    }
}