using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using System.Runtime.Serialization.Json;
namespace DEWD_DLL_Loading_Manager.Controllers
{

    [System.Runtime.Serialization.DataContract]
    public class Params
    {
        [System.Runtime.Serialization.DataMember]
        public string dll_path = "";
        [System.Runtime.Serialization.DataMember]
        public string class_name = "";
        [System.Runtime.Serialization.DataMember]
        public string method_name = "";
        [System.Runtime.Serialization.DataMember]
        public string short_name = "";
    }
    public class JsonHelper
    {
        public static string ToJson(Object obj, Type type)
        {

            MemoryStream ms = new MemoryStream();

            DataContractJsonSerializer seralizer = new DataContractJsonSerializer(type);


            seralizer.WriteObject(ms, obj);
            ms.Seek(0, SeekOrigin.Begin);

            StreamReader sr = new StreamReader(ms);
            string jsonstr = sr.ReadToEnd();

            //jsonstr = jsonstr.Replace("\"", "\\\"");

            sr.Close();
            ms.Close();
            return jsonstr;
        }
        public static Object FromJson(String jsonstr, Type type)
        {

            MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(jsonstr));

            DataContractJsonSerializer seralizer = new DataContractJsonSerializer(type);

            ms.Seek(0, SeekOrigin.Begin);

            Object res = seralizer.ReadObject(ms);


            ms.Close();
            return res;
        }

    }
}
