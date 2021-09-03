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
                int[] roundFlags = new int[PeopleNum];//用于标记指定序号在一个周期内已经值勤了的次数
                bool[] dayFlags = new bool[PeopleNum];//用于标记指定序号在一天内的值勤次数(防止两个人值勤时，生成的序号却是重复的)
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
                            //如果当前序号对应的人在一个周期内已经完成了指定次数的值勤，
                            //或者当前序号对应的人已经需要在这天值勤了，就换一个人代替他
                            //还有种可能陷入死循环的情况：
                            //1 2
                            //4 2     
                            //1 4     
                            //3 3
                            //正在思考如何解决它
                            while (roundFlags[index] >= PeoplePerDay || dayFlags[index])
                            {
                                index = rd.Next(0, PeopleNum);
                            }
                            roundFlags[index]++;
                            dayFlags[index] = true;
                            index++;//将从0开始的索引值，转换为从1开始的序号
                            ret += index.ToString() + " ";
                            time = time.AddDays(1);
                        }
                        for (int z = 0; z < PeopleNum; z++)
                        {
                            dayFlags[z] = false;
                        }
                        ret += "\t" + time.DayOfWeek + "\t" + time.ToShortDateString() + "\r\n";
                    }
                    for (int j = 0; j < PeopleNum; j++)
                    {
                        roundFlags[j] = 0;
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
            try
            {
                label_timeNow.Content = "今天是" + DateTime.Now.ToLongDateString() + " " + DateTime.Now.DayOfWeek;
                if (File.Exists(Environment.CurrentDirectory + "/sheet.txt"))
                {
                    tabControl.SelectedIndex = 1;
                    StreamReader sr = new StreamReader(Environment.CurrentDirectory + "/sheet.txt");
                    List<string> sheetData = new List<string>();
                    string line;
                    while ((line = sr.ReadLine()) != null)
                        sheetData.Add(line);
                    string searchStr = sheetData.Find(str => { return str.Contains(DateTime.Now.ToShortDateString()); });
                    if (searchStr != null)
                    {
                        //搜索到的字符串还要进一步检验，原因：2021/9/3 和 2021/9/30都包含 "2021/9/3"
                        //但却不能说后者符合查找条件
                        string[] temp = searchStr.Split('\t');
                        if (temp.Length == 3 && temp[2].Equals(DateTime.Now.ToShortDateString()))
                        {
                            label_index.Content = String.Format("该轮到{0}号值日啦！", temp[0]);
                        }
                    }
                    else
                    {                        
                        tabControl.SelectedIndex = 0;
                        label_index.Content = "值勤表可以重新生成喽！";
                        this.ShowMessageAsync("提示", "值勤表可以重新生成喽！");
                    }

                }
            }
            catch (Exception ex)
            {
                ErrorManger.OutPutErrAndReboot(ex);
            }
            
            try
            {
                WebClient wc = new WebClient();
                wc.OpenReadCompleted += new OpenReadCompletedEventHandler((object s, OpenReadCompletedEventArgs ret) => {
                    try
                    {
                        StreamReader sr = new StreamReader(ret.Result);
                        JsonData hitokotoJson = JsonMapper.ToObject(sr.ReadToEnd());
                        textBox_hitokoto.Text = hitokotoJson["hitokoto"].ToString();
                        textBox_hitokoto_author.Text = String.Format("——{0}「{1}」", hitokotoJson["from_who"], hitokotoJson["from"]);
                        wc.Dispose();
                    }
                    catch
                    {
                        textBox_hitokoto.Text = "网络异常，一言获取失败";
                    }
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
