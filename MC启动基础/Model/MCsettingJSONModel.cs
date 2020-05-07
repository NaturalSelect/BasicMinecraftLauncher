using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.IO;

namespace MC启动基础.Model
{
    public class MCsettingJSONModel
    {
        public string Name { get; set; }
        public string JavaPath { get; set; }
        public int MaxRam { get; set; }
        public string MCVer { get; set; }
        private static MCsettingJSONModel JSON;
        private MCsettingJSONModel()
        {
        }
        public static MCsettingJSONModel GetConfig
        {
            get
            {
                if(JSON == null)
                {
                    var path = AppDomain.CurrentDomain.BaseDirectory + @"\setting.json";
                    if (File.Exists(path))
                    {
                        using(FileStream fs = new FileStream(path, FileMode.Open))
                        {
                            StreamReader sr = new StreamReader(fs, Encoding.UTF8);
                            JObject jObject = JObject.Parse(sr.ReadToEnd());
                            JSON = new MCsettingJSONModel()
                            {
                                JavaPath = jObject["JavaPath"].ToString(),
                                MCVer = jObject["MCVer"].ToString(),
                                Name = jObject["Name"].ToString()
                            };
                            int i;
                            if (int.TryParse(jObject["MaxRam"].ToString(),out i))
                            {
                                JSON.MaxRam = i;
                            }
                            else
                            {
                                JSON.MaxRam = 1024;
                            }
                        }
                    }
                    else
                    {
                        using(FileStream fs = new FileStream(path, FileMode.Create))
                        {
                            StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
                            JObject jobject = JObject.FromObject(new MCsettingJSONModel()
                            {
                                Name = "",
                                MaxRam = 1024,
                                JavaPath = "",
                                MCVer = ""
                            });
                            sw.Write(jobject.ToString());
                            sw.Flush();
                            fs.Flush();
                        }
                        return GetConfig;
                    }
                }
                return JSON;
            }
            set
            {
                var path = AppDomain.CurrentDomain.BaseDirectory + @"\setting.json";
                using(FileStream fs = new FileStream(path, FileMode.Create))
                {
                    StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
                    JObject jObject = JObject.FromObject(value);
                    sw.Write(jObject);
                    sw.Flush();
                    fs.Flush();
                    sw.Close();
                    JSON = null;
                }
            }
        }
    }
}
