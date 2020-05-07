using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FirstFloor.ModernUI.Windows.Controls;
using MC启动基础.Model;

namespace MC启动基础.Pages.Home
{
    /// <summary>
    /// Interaction logic for Start.xaml
    /// </summary>
    public partial class Start : UserControl
    {
        public Start()
        {
            InitializeComponent();
            //获取MC类
            MC mc = MC.GetMC();
            //将Name绑定到mc的Name属性
            GameName.SetBinding(TextBox.TextProperty, new Binding("Name") {Source=mc,UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,Mode = BindingMode.TwoWay });
            //设置Vers的源由MCseting提供
            Vers.ItemsSource = MCSetting.AllVer;
            //创建启动路由命令
            RoutedCommand StartCommand = new RoutedCommand();
            //设置命令快捷键
            StartCommand.InputGestures.Add(new KeyGesture(Key.S, ModifierKeys.Alt));
            //创造命令绑定
            CommandBinding StartCommandBinding = new CommandBinding(StartCommand);
            //将Ver绑定
            Vers.SetBinding(ComboBox.SelectedItemProperty,new Binding("Ver") {Source = mc,UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,Mode = BindingMode.TwoWay });
            //设置什么时候命令可以执行
            StartCommandBinding.CanExecute += (s, e) =>
            {
                if (mc.IsReady())
                {
                    e.CanExecute = true;
                }
                else
                {
                    e.CanExecute = false;
                }
                e.Handled = true;
            };
            //设置执行的内容
            StartCommandBinding.Executed += (s, e) => 
            {
                var setting = MCsettingJSONModel.GetConfig;
                setting.Name = mc.Name;
                var mcsetting = MCSetting.GetSetting();
                setting.JavaPath = mcsetting.JavaPath;
                setting.MaxRam = mcsetting.MaxRam;
                setting.MCVer = mc.Ver.Name;
                MCsettingJSONModel.GetConfig = setting;
                try
                {
                    Load.Visibility = Visibility.Visible;
                    mc.Start();
                    Environment.Exit(0);
                }
                catch (ApplicationException ex)
                {
                   var r = ModernDialog.ShowMessage($"在启动时出现了异常:{Environment.NewLine}信息:{ex.Message}{Environment.NewLine}源:{ex.Source}{Environment.NewLine}在方法{ex.TargetSite}中{Environment.NewLine}堆栈跟踪:{ex.StackTrace}", "异常", MessageBoxButton.YesNo);
                    if(r == MessageBoxResult.Yes)
                    {
                        Environment.FailFast("因异常而终止");
                    }
                }
                catch (System.IO.DirectoryNotFoundException ex)
                {
                    var r = ModernDialog.ShowMessage($"在启动时出现了异常:{Environment.NewLine}信息:{ex.Message}{Environment.NewLine}源:{ex.Source}{Environment.NewLine}在方法{ex.TargetSite}中{Environment.NewLine}堆栈跟踪:{ex.StackTrace}", "异常", MessageBoxButton.YesNo);
                    if (r == MessageBoxResult.Yes)
                    {
                        Environment.FailFast("因异常而终止");
                    }
                }
                catch (System.IO.FileNotFoundException ex)
                {
                    var r = ModernDialog.ShowMessage($"在启动时出现了异常:{Environment.NewLine}信息:{ex.Message}{Environment.NewLine}源:{ex.Source}{Environment.NewLine}在方法{ex.TargetSite}中{Environment.NewLine}堆栈跟踪:{ex.StackTrace}", "异常", MessageBoxButton.YesNo);
                    if (r == MessageBoxResult.Yes)
                    {
                        Environment.FailFast("因异常而终止");
                    }
                }
                finally
                {
                    Load.Visibility = Visibility.Hidden;
                }
                e.Handled = true;
            };
            //添加命令到控件
            Starter.Command = StartCommand;
            //添加命令绑定
            Starter.CommandBindings.Add(StartCommandBinding);
            
        }
    }
}
