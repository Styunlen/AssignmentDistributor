using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace AssignmentDistributor
{
    class OptionsManger
    {
        private static List<string> Options = new List<string>();
        public const int g_optVer = 100;
        public static string g_strOptHead = string.Format("#ver {0};", g_optVer);//配置文件头部，用于描述配置文件相关信息
        public static string g_optFile = Environment.CurrentDirectory + "/options.txt";

        //判断配置文件是否过时
        static public bool CheckOptVersion()
        {
            try
            {
                StreamReader sr = new StreamReader(g_optFile);
                string strOptHead = sr.ReadLine();
                sr.Close();
                int iVer = Convert.ToInt32(strOptHead.Split(';')[0].Split(' ')[1]);
                    if (iVer == g_optVer)
                        return true;
                    else
                        return false;
            }
            catch (Exception ex)
            {
                ErrorManger.OutPutErrAndReboot(ex);
            }
            return false;
        }

        static public void LoadOptions()//加载并应用设置
        {
            try
            {
                if (File.Exists(g_optFile))
                {
                    if (CheckOptVersion())//每次读取时，判断配置文件版本号，如果不对，则重置配置文件为默认配置;(//备注：每次修改完版本，记得修改重置配置文件函数中的版本号)配置文件头部已用静态变量存储
                    {
                        StreamReader sr = new StreamReader(g_optFile);
                        Options.Clear();
                        string lineOption;
                        while ((lineOption = sr.ReadLine()) != null)
                            Options.Add(lineOption);
                        sr.Close();
                    }
                    else
                        ResetOptionToDefault();
                }
                else
                {
                    ResetOptionToDefault();
                }
            }
            catch (Exception ex)
            {
                ErrorManger.OutPutErrAndReboot(ex);
            }
        }

        static public string GetOptionVal(string optionName,string defaultErrValue = "NULL")
        {
            try
            {
                foreach (string temp in Options)
                {
                    string[] nameVal = temp.Split(':');//每行数据既包含选项名，也包含选项值(格式:"optName:optVal")
                    if (nameVal[0] == optionName)
                    {
                        return nameVal[1];
                    }
                }
            }
            catch(Exception ex)
            {
                ErrorManger.OutPutErrAndReboot(ex);
            }
            return defaultErrValue;
        }

        //若当前配置中有该选项，调整该选项，并返回真，否则返回假
        static public bool SetOptionVal(string optionName,string optionVal)
        {
            try
            {
                for (int i = 0; i < Options.Count ; i++)
                {
                    string temp = Options[i];
                    string[] nameVal = temp.Split(':');//每行数据既包含选项名，也包含选项值(格式:"optName:optVal")
                    if (nameVal[0] == optionName)
                    {
                        Options[i] = String.Format("{0}:{1}", optionName, optionVal);
                        return true;
                    }        
                }
            }
            catch (Exception ex)
            {
                ErrorManger.OutPutErrAndReboot(ex);
            }
            return false;
        }

        static public bool AddOptionVal(string optionName, string optionVal)
        {
            try
            {
                Options.Add(String.Format("{0}:{1}", optionName, optionVal));
                return true;
            }
            catch (Exception ex)
            {
                ErrorManger.OutPutErrAndReboot(ex);
            }
            return false;
        }

        //删除所有配置项
        static public bool CleanOptions()
        {
            try
            {
                Options.Clear();
                return true;
            }
            catch (Exception ex)
            {
                ErrorManger.OutPutErrAndReboot(ex);
            }
            return false;
        }

        static public bool SaveOptions()
        {
            try
            {
                StreamWriter sw = new StreamWriter(g_optFile, false);
                foreach (string opt in Options)
                    sw.WriteLine(opt);
                sw.Close();
                return true;
            }
            catch (Exception ex)
            {
                ErrorManger.OutPutErrAndReboot(ex);
            }
            return false;
        }

        static public void ResetOptionToDefault()
        {  
            OptionsManger.CleanOptions();
            Options.Add(g_strOptHead);
            OptionsManger.AddOptionVal("PeopleNum", "4");
            OptionsManger.AddOptionVal("PeoplePerDayNum", "1");
            OptionsManger.AddOptionVal("RoundNum", "10");
            OptionsManger.SaveOptions();
        }

        //public static bool g_isReadingMode = false;

        static public void ApplyOption(MainWindow mw)
        {
            try
            {
                mw.slider_peopleNum.Value = Convert.ToInt32(OptionsManger.GetOptionVal("PeopleNum"));
                mw.slider_peoplePerDayNum.Value = Convert.ToInt32(OptionsManger.GetOptionVal("PeoplePerDayNum"));
                mw.slider_roundNum.Value = Convert.ToInt32(OptionsManger.GetOptionVal("RoundNum"));
            }
            catch (Exception ex)
            {
                ErrorManger.OutPutErrAndReboot(ex);
            }
        }
    }
}
