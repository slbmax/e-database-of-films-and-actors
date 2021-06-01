using System;
using System.IO;
using System.Xml.Serialization;
using System.Collections.Generic;
using ClassLib;
namespace ConsoleApp
{
    public static class XML
    {
        public static void Export(string filePath, List<Review> reviews)
        {
            XmlSerializer ser = new XmlSerializer(typeof(List<Review>));
            StreamWriter writer = new StreamWriter(filePath);
            ser.Serialize(writer, reviews);
            writer.Close();
        }
        public static List<Review> Import(string filePath)
        {
            XmlSerializer ser = new XmlSerializer(typeof(List<Review>));
            string xmlData = File.ReadAllText("./opa.xml");
            StringReader reader = new StringReader(xmlData);
            List<Review> reviews = (List<Review>)ser.Deserialize(reader);
            reader.Close();
            return reviews;
        }
    }
}