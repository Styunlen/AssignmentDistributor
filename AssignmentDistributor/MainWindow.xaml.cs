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
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace AssignmentDistributor
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            ErrorManger.CheckErr();//检查程序是否崩溃重启
            OptionsManger.LoadOptions();
            OptionsManger.ApplyOption(this);
            ErrorManger.RemoveErrorLogs();//一切正常，删除错误日志
        }

        private void Flyout_Settings_IsOpenChanged(object sender, RoutedEventArgs e)
        {
            try
            {
                OptionsManger.SetOptionVal("PeopleNum", slider_peopleNum.Value.ToString());
                OptionsManger.SetOptionVal("PeoplePerDayNum", slider_peoplePerDayNum.Value.ToString());
                OptionsManger.SetOptionVal("RoundNum", slider_roundNum.Value.ToString());
                OptionsManger.SaveOptions();
                this.ShowMessageAsync("提示","设置已保存！");
            }
            catch(Exception ex)
            {
                ErrorManger.OutPutErrAndReboot(ex);
            }


        }

        private void button_settings_Click(object sender, RoutedEventArgs e)
        {
            Flyout_Settings.IsOpen = true;
        }

        private void button_about_Click(object sender, RoutedEventArgs e)
        {
            Window_About wa = new Window_About(new Point(this.Left, this.Top));
            wa.ShowDialog();
        }
    }
}
