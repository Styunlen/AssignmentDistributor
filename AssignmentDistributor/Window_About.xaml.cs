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
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace AssignmentDistributor
{
    /// <summary>
    /// Window_About.xaml 的交互逻辑
    /// </summary>
    public partial class Window_About
    {
        public Window_About(Point pStartupPos)
        {
            InitializeComponent();
            this.Left = pStartupPos.X + (525 - this.Width) / 2;
            this.Top = pStartupPos.Y + (350 - this.Height) / 2;
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            string text;
            text = textBox_about_app.Text;
            FileVersionInfo fv = Process.GetCurrentProcess().MainModule.FileVersionInfo;
            textBox_about_app.Text = text.Replace("$ver", fv.FileVersion);
        }

        private void MetroWindow_KeyDown(object sender, KeyEventArgs e)
        {
            this.DragMove();
        }

        private void button_checkForUpdate_Click(object sender, RoutedEventArgs e)
        {
            MetroDialogSettings mds = new MetroDialogSettings();
            mds.AffirmativeButtonText = "好的";
            mds.NegativeButtonText = "算了";
            var v = this.ShowMessageAsync("提示", "是否跳转到作者的博客以检查更新？", MessageDialogStyle.AffirmativeAndNegative, mds);
            v.ContinueWith((ret) => {
                if (ret.Result == MessageDialogResult.Affirmative)
                {
                    System.Diagnostics.Process.Start("https://styunlen.cn/softlib");
                }
            });
        }
    }
}