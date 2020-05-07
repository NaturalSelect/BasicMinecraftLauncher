using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Diagnostics;
using Newtonsoft.Json.Linq;

namespace MC启动基础.Model
{
    public class MC : INotifyPropertyChanged
    {
        public MCVer Ver
        {
            get
            {
                return ver;
            }
            set
            {
                ver = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Ver"));
            }
        }
        private MCVer ver;
        private MC()
        {
            MCsettingJSONModel setting = MCsettingJSONModel.GetConfig;
            name = setting.Name;
        }
        private string name;
        public string Name {
            get
            {
                return name;
            }
            set
            {
                name = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Name"));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private static MC mc;
        public static MC GetMC()
        {
            if (mc == null)
            {
                mc = new MC();
                if (!string.IsNullOrEmpty(MCsettingJSONModel.GetConfig.MCVer))
                {
                    var q = from c in MCSetting.AllVer
                            where c.Name == MCsettingJSONModel.GetConfig.MCVer
                            select c;
                    if(q.Count() == 0)
                    {
                        mc.Ver = new MCVer();
                        FirstFloor.ModernUI.Windows.Controls.ModernDialog.ShowMessage("找不到在记录项的版本,已设置版本为空", "警告", System.Windows.MessageBoxButton.OK);
                    }
                    else
                    {
                        mc.Ver = q.First();
                    }
                }
            }
            return mc;
        }
        public bool IsReady()
        {
            if(mc.ver == null)
            {
                return false;
            }
            if(!string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Ver.Name) && !string.IsNullOrEmpty(MCSetting.GetSetting().JavaPath))
            {
                if (System.IO.Directory.Exists(Ver.Path))
                {
                    return true;
                }
            }
            return false;
        }
        public void Start()
        {
            if (IsReady())
            {
                var setting = MCSetting.GetSetting();
                Process p = new Process();
                var args = "";
                JObject JSON;
                var JSONPath = mc.Ver.Path + $@"\{mc.Ver.Name}.json";
                var natives = $@"{mc.Ver.Path}\{mc.Ver.Name}-natives";
                var gamePath = $@"{AppDomain.CurrentDomain.BaseDirectory}\.minecraft";
                using (System.IO.FileStream fs = new System.IO.FileStream(JSONPath, System.IO.FileMode.Open))
                {
                    System.IO.StreamReader sr = new System.IO.StreamReader(fs, Encoding.UTF8);
                    JSON = JObject.Parse(sr.ReadToEnd());
                    sr.Close();
                }
                List<string> TrueLibs = new List<string>();
                if (JSON == null)
                {
                    throw new ApplicationException("无法读取指定版本的JSON文件");
                }
                
                TrueLibs.AddRange(GetLib(JSONPath));
                //填充参数
                if (JSON["inheritsFrom"] != null)
                {
                    var inher = JSON["inheritsFrom"].ToString();
                    var qu = from c in MCSetting.AllVer
                             where c.Name == inher
                             select c;
                    if (qu.Count() == 0)
                    {
                        throw new System.IO.FileNotFoundException("找不到指定的版本继承");
                    }
                    else
                    {
                        try
                        {
                            TrueLibs.AddRange(GetLib(qu.First().Path + $@"\{qu.First().Name}.json"));
                            TrueLibs.Add($@"{qu.First().Path}\{qu.First().Name}.jar");
                        }
                        catch
                        {
                            throw;
                        }
                    }
                }
                else
                {
                    TrueLibs.Add($@"{gamePath}\versions\{mc.Ver.Name}\{mc.Ver.Name}.jar");
                }
                if (!System.IO.Directory.Exists(natives))
                {
                    throw new System.IO.DirectoryNotFoundException("该版本的natives目录不存在");
                }
                args = $"-XX:HeapDumpPath=MojangTricksIntelDriversForPerformance_javaw.exe_minecraft.exe.heap.dump -XX:+UseConcMarkSweepGC -XX:+CMSIncrementalMode -XX:-UseAdaptiveSizePolicy -XX:-OmitStackTraceInFastThrow -Xmn128m -Xmx{setting.MaxRam}m -Djava.library.path={natives} -Dfml.ignoreInvalidMinecraftCertificates=true -Dfml.ignorePatchDiscrepancies=true -Dhellominecraftlauncher.gamedir={gamePath}";
                args = args + " -cp ";
                foreach(var tl in TrueLibs)
                {
                    args = args + tl + ";";
                }
                args = args + " ";
                args = args +JSON["mainClass"].ToString()+" ";
                args = args + JSON["minecraftArguments"].ToString()+ " --height 480 --width 854";
                args = args.Replace("${auth_player_name}", mc.Name);
                args = args.Replace("${version_name}", $"\"BasicStart\"");
                args = args.Replace("${game_directory}",$"{gamePath}" );
                args = args.Replace("${assets_root}", $@"{gamePath}\assets ");
                args = args.Replace("${assets_index_name}", JSON["assets"].ToString());
                var Random = getRandStringEx(32).ToLower();
                args = args.Replace("${auth_uuid}",Random);
                args = args.Replace("${auth_access_token}", Random);
                args = args.Replace("${user_properties}", "{}");
                args = args.Replace("${user_type}", "Legacy");
                args = args.Replace(@"\\", @"\");
                ProcessStartInfo psi = new ProcessStartInfo(setting.JavaPath, args);
                p.StartInfo = psi;
                p.Start();
            }
            else
            {
                throw new ApplicationException("发生了一个未知的错误,无法启动游戏.");
            }
        }
        public static String getRandStringEx(int length)
        {
            char[] charList = {'0','1','2','3','4','5','6','7','8','9',
                               'A','B','C','D','E','F','G','H','I','J','K','L','M',
                               'N','O','P','Q','R','S','T','U','V','W','X','Y','Z',
                               'a','b','c','d','e','f','g','h','i','j','k','l','m',
                               'n','o','p','q','r','s','t','u','v','w','x','y','z'};
            char[] rev = new char[length];
            Random f = new Random();
            for (int i = 0; i < length; i++)
            {
                rev[i] = charList[Math.Abs(f.Next(127)) % length];
            }
            return new String(rev);
        }
        private static string[] GetLib(string JSONPath)
        {
            JObject JSON;
            using (System.IO.FileStream fs = new System.IO.FileStream(JSONPath, System.IO.FileMode.Open))
            {
                System.IO.StreamReader sr = new System.IO.StreamReader(fs, Encoding.UTF8);
                JSON = JObject.Parse(sr.ReadToEnd());
                sr.Close();
            }
            if (JSON == null)
            {
                throw new ApplicationException("无法读取指定版本的JSON文件");
            }
            var q = from c in JSON["libraries"].Children()
                    select c;
            List<string> Libs = new List<string>();
            foreach (var a in q)
            {
                if(a["rules"] == null && a["natives"] == null)
                {
                    StringBuilder builder = new StringBuilder();
                    builder.Append($@"{AppDomain.CurrentDomain.BaseDirectory}\.minecraft\libraries\");
                    string[] ns = a["name"].ToString().Split(':');
                    for (int b = 0; b < (ns.Count() - 1); b++)
                    {
                        if (ns[b].Split('_').Count() > 1)
                        {
                            builder.Append(ns[b] + @"\");
                        }
                        else
                        {
                            builder.Append(ns[b].Replace('.', '\\') + @"\");
                        }
                    }
                    builder.Append(ns.Last());
                    Libs.Add(builder.ToString());
                }
            }
            List<string> TrueLibs = new List<string>();
            foreach (var l in Libs)
            {
                if (System.IO.Directory.Exists(l))
                {
                    foreach (var f in System.IO.Directory.GetFiles(l))
                    {
                        if (f.Split('.').Last() == "jar")
                        {
                            if (f.Split('-').Last() == "64.jar" && Environment.Is64BitOperatingSystem)
                            {
                                TrueLibs.Add(f);
                                break;
                            }
                            if (f.Split('-').Last() == "32.jar" && !Environment.Is64BitOperatingSystem)
                            {
                                TrueLibs.Add(f);
                                break;
                            }
                            if (f.Split('-').Last() != "64.jar" && f.Split('-').Last() != "32.jar")
                            {
                                TrueLibs.Add(f);
                            }
                        }
                    }
                }
                else
                {
                    throw new ApplicationException($"找不到指定库:{Environment.NewLine}{l}");
                }
            }
            return TrueLibs.ToArray();
        }
    }
}
