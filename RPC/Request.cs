using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using ClassLib;

namespace RPC
{
    [XmlRoot("request")]
    public class Request
    {
        public string methodName;       
        public string[] methodParametrs;
    }
}