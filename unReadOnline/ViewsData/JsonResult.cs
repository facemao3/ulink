using System.IO;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System;
using Newtonsoft.Json;

namespace unReadOnline.ViewsData
{
    //[DataContract]
    public class JsonResult
    {
        //[DataMember]
        public bool isSuccessful
        {
            get;
            set;
        }

        //[DataMember]
        public string errorMessage
        {
            get;
            set;
        }

        //[DataMember]
        public string returnHtml
        {
            get;
            set;
        }

        public string ToJson()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                //DataContractJsonSerializer s = new DataContractJsonSerializer(GetType());
                //s.WriteObject(ms, this);

                //ms.Seek(0, SeekOrigin.Begin);

                //return Encoding.UTF8.GetString(ms.ToArray());

                string json = JavaScriptConvert.SerializeObject(this);
                return json;
            }
        }
    }
}