using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace MushroomTracker
{
    class JsonHandler
    {

        public static string convertToString(Mushroom mushroom)
        {
            MemoryStream stream = new MemoryStream();
            DataContractJsonSerializer jsonSer =
            new DataContractJsonSerializer(typeof(Mushroom));
            jsonSer.WriteObject(stream, mushroom);
            stream.Position = 0;
            StreamReader sr = new StreamReader(stream);
            return sr.ReadToEnd();
        }


        internal static Mushroom deserializeMushroom(string json)
        {
            byte[] data = Encoding.UTF8.GetBytes(json);
            MemoryStream memStream = new MemoryStream(data);
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(Mushroom));
            return (Mushroom) serializer.ReadObject(memStream);
        }
    }
}
