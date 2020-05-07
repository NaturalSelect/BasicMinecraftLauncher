using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json.Linq;

namespace MC启动基础.Model
{
    public class MCSetting:INotifyPropertyChanged
    {
        private MCSetting()
        {
            var setting = MCsettingJSONModel.GetConfig;
            MaxRam = setting.MaxRam;
            JavaPath = setting.JavaPath;
        }
        private static MCSetting Setting;
        public static MCSetting GetSetting()
        {
            if (Setting == null)
            {
                Setting = new MCSetting();
            }
            return Setting;
        }
        private static ObservableCollection<MCVer> allVer;
        public static ObservableCollection<MCVer> AllVer {
            get
            {
                if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + @".minecraft\versions"))
                {
                    if (allVer == null)
                    {
                        allVer = new ObservableCollection<MCVer>();
                        var vs = Directory.GetDirectories(AppDomain.CurrentDomain.BaseDirectory + @".minecraft\versions");
                        foreach (var v in vs)
                        {
                            allVer.Add(new MCVer() { Name = v.Split('\\').Last(), Path = v });
                        }
                        return allVer;
                    }
                }
                else
                {
                    var re = FirstFloor.ModernUI.Windows.Controls.ModernDialog.ShowMessage("无法找到版本文件夹。是否结束应用程序", "异常", System.Windows.MessageBoxButton.YesNo);
                    if(re == System.Windows.MessageBoxResult.Yes)
                    {
                        Environment.FailFast("找不到版本文件夹");
                    }
                    else
                    {
                        
                        allVer = new ObservableCollection<MCVer>();
                    }
                }
                return allVer;
            }
        }
        private int maxRam = 1024;

        public event PropertyChangedEventHandler PropertyChanged;

        public int MaxRam
        {
            get
            {
                return maxRam;
            }
            set
            {
                maxRam = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MaxRam"));
            }
        }
        private string javaPath;
        public string JavaPath
        {
            get
            {
                return javaPath;
            }
            set
            {
                javaPath = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("JavaPath"));
            }
        }
    }
}
