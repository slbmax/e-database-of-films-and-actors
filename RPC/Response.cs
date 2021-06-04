using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using ClassLib;
namespace RPC
{
    [XmlRoot("response")]
    public class Response<T>
    {
       public T value;
       public bool error;
    }
}