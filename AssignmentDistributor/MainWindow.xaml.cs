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
using System.IO;
using System.Diagnostics;
using System.Net;
using MahApps.Metro.Controls;
using LitJson;
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
                if(!Flyout_Settings.IsOpen)
                {
                    OptionsManger.SetOptionVal("PeopleNum", slider_peopleNum.Value.ToString());
                    OptionsManger.SetOptionVal("PeoplePerDayNum", slider_peoplePerDayNum.Value.ToString());
                    OptionsManger.SetOptionVal("RoundNum", slider_roundNum.Value.ToString());
                    OptionsManger.SaveOptions();
                    this.ShowMessageAsync("提示","设置已保存！");
                }
                else
                {
                    OptionsManger.LoadOptions();//如果手动更改了配置文件，也要即使更新
                    OptionsManger.ApplyOption(this);
                }
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

        private void tile_generate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int PeopleNum = Convert.ToInt32(slider_peopleNum.Value);
                int PeoplePerDay = Convert.ToInt32(slider_peoplePerDayNum.Value);
                int Round = Convert.ToInt32(slider_roundNum.Value);
                int[] flags = new int[PeopleNum];
                string ret = "序号\t星期\t日期\r\n";
                Random rd = new Random(Guid.NewGuid().GetHashCode());
                DateTime time = DateTime.Now;
                for (int i = 0; i < Round; i++)
                {
                    for (int x = 0; x < PeopleNum; x++)
                    {
                        for (int y = 0; y < PeoplePerDay; y++)
                        {
                            int index = rd.Next(0, PeopleNum);
                            while (flags[index] >= PeoplePerDay)
                            {
                                index = rd.Next(0, PeopleNum);
                            }
                            flags[index]++;
                            index++;//将从0开始的索引值，转换为从1开始的序号
                            ret += index.ToString() + "\t" + time.DayOfWeek + "\t" + time.ToShortDateString();
                            time = time.AddDays(1);
                        }
                        ret += "\r\n";
                    }
                    for (int x = 0; x < PeopleNum; x++)
                    {
                        flags[x] = 0;
                    }
                }
                StreamWriter sw = new StreamWriter(Environment.CurrentDirectory + "/sheet.xls", false);
                sw.WriteLine(ret);
                sw.Close();
                sw = new StreamWriter(Environment.CurrentDirectory + "/sheet.txt", false);
                sw.WriteLine(ret);
                sw.Close();
                this.ShowMessageAsync("提示", "生成成功！");
                Process.Start(Environment.CurrentDirectory + "./sheet.xls");
            }
            catch (Exception ex)
            {
                ErrorManger.OutPutErrAndReboot(ex);
            }

        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            label_timeNow.Content = "今天是" + DateTime.Now.ToShortDateString() + " " + DateTime.Now.DayOfWeek;
            if (File.Exists(Environment.CurrentDirectory + "/sheet.txt"))
            {
                tabControl.SelectedIndex = 1;
                StreamReader sr = new StreamReader(Environment.CurrentDirectory + "/sheet.txt");
                List<string> sheetData = new List<string>();
                string line;
                while ((line = sr.ReadLine()) != null)
                    sheetData.Add(line);
                string indexStr = sheetData.Find(str=> { return str.Contains(DateTime.Now.ToShortDateString()); });
            }
            try
            {
                WebClient wc = new WebClient();
                wc.OpenReadCompleted += new OpenReadCompletedEventHandler((object s, OpenReadCompletedEventArgs ret) => {
                    StreamReader sr = new StreamReader(ret.Result);
                    JsonData hitokotoJson = JsonMapper.ToObject(sr.ReadToEnd());
                    textBox_hitokoto.Text = String.Format("{0}——{1}「{2}」", hitokotoJson["hitokoto"], hitokotoJson["from_who"], hitokotoJson["from"]);
                    wc.Dispose();
                });
                wc.OpenReadAsync(new Uri("https://v1.hitokoto.cn"));
            }
            catch
            {
                textBox_hitokoto.Text = "网络异常，一言获取失败";
            }
        }
    }
}
