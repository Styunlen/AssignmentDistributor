using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.IO;

namespace AssignmentDistributor
{
    /// <summary>
    /// Window_Error.xaml 的交互逻辑
    /// </summary>
    public partial class Window_Error : MetroWindow
    {
        public Window_Error()
        {
            InitializeComponent();
            if (File.Exists(Environment.CurrentDirectory + "/Error.txt"))
            {
                this.Invoke(()=>{ textBox_error.Text = File.ReadAllText(Environment.CurrentDirectory + "/Error.txt"); });
                File.Delete(Environment.CurrentDirectory + "/Error.txt");
            }
            if (File.Exists(Environment.CurrentDirectory + "/ErrorRebootCount.txt"))
            {
                this.Invoke(() => { label_count.Content= label_count.Content.ToString().Replace("{n}", File.ReadAllText(Environment.CurrentDirectory + "/ErrorRebootCount.txt")); });
                //File.Delete(Environment.CurrentDirectory + "/Error.txt");主窗口初始化正常完成后才能删除，否则还是像原来一样无限重启
            }  
        }

        private void MetroWindow_Closed(object sender, EventArgs e)
        {
            
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            string apology;
            apology = "非常抱歉程序出错了TOT\r\n人有失足,马有失蹄，更何况是程序呢！\r\n";
            apology += "再次对这意料之外的错误感到抱歉,也欢迎您将错误信息反馈给作者";
            this.ShowMessageAsync("抱歉", apology);
        }
    }
}
