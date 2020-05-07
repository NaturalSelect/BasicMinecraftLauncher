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
using MC启动基础.Model;

namespace MC启动基础.Pages.Settings
{
    /// <summary>
    /// Interaction logic for MCsettings.xaml
    /// </summary>
    public partial class MCsettings : UserControl
    {
        MC启动基础.Model.MCSetting setting = MC启动基础.Model.MCSetting.GetSetting();
        public MCsettings()
        {
            InitializeComponent();
            JavaPath.SetBinding(TextBox.TextProperty, new Binding("JavaPath") { Source = setting, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Mode = BindingMode.TwoWay });
            MaxRAM.SetBinding(TextBox.TextProperty, new Binding("MaxRam") { Source = setting, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Mode = BindingMode.TwoWay });
        }

        private void FindPath_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog()
            {
                Multiselect = false,
                InitialDirectory =@"C;\"
            };
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                setting.JavaPath = ofd.FileName;
            }
        }
    }
}
