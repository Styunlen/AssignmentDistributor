using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace AssignmentDistributor
{
    class ErrorManger
    {
        //清除错误文件
        static public void RemoveErrorLogs()
        {
            File.Delete(Environment.CurrentDirectory + "/ErrorRebootCount.txt");//一切正常后，删除错误计数文件
        }

        static public void CheckErr()
        {
            if (File.Exists(Environment.CurrentDirectory + "/Error.txt"))
            {
                Window_Error we = new Window_Error();
                we.ShowDialog();
            }
        }
        static public void OutPutErrAndReboot(Exception ex)
        {
            try
            {
                FileStream fs = File.Create(Environment.CurrentDirectory + "/Error.txt");
                fs.Close();
                string exMsg = "啊哦，出错了！\r\n";
                exMsg += "错误信息:" + ex.Message + "\r\n" + ex.InnerException.Message;
                exMsg += "\r\n出错函数:" + ex.TargetSite;
                exMsg += "\r\n出错对象:" + ex.Source;
                exMsg += "\r\n出错堆栈信息:\r\n" + ex.StackTrace;
                File.WriteAllText(Environment.CurrentDirectory + "/Error.txt", exMsg);
                if (!File.Exists(Environment.CurrentDirectory + "/ErrorRebootCount.txt"))
                {
                    File.Create(Environment.CurrentDirectory + "/ErrorRebootCount.txt").Close();
                    File.WriteAllText(Environment.CurrentDirectory + "/ErrorRebootCount.txt", "0");
                }
                File.WriteAllText(Environment.CurrentDirectory + "/ErrorRebootCount.txt", (Convert.ToInt32(File.ReadAllText(Environment.CurrentDirectory + "/ErrorRebootCount.txt")) + 1).ToString());
                if (Convert.ToInt32(File.ReadAllText(Environment.CurrentDirectory + "/ErrorRebootCount.txt")) <= 5)//防止无限重启
                    Process.Start(Process.GetCurrentProcess().MainModule.FileName);
                System.Environment.Exit(0);
            }
            catch
            {
                FileStream fs = File.Create(Environment.CurrentDirectory + "/Error.txt");
                fs.Close();
                string exMsg = "啊哦，出错了！\r\n";
                exMsg += "错误信息:" + ex.Message;/* + "\r\n" + ex.InnerException.Message;*/
                exMsg += "\r\n出错函数:" + ex.TargetSite;
                exMsg += "\r\n出错对象:" + ex.Source;
                exMsg += "\r\n出错堆栈信息:\r\n" + ex.StackTrace;
                File.WriteAllText(Environment.CurrentDirectory + "/Error.txt", exMsg);
                if (!File.Exists(Environment.CurrentDirectory + "/ErrorRebootCount.txt"))
                {
                    File.Create(Environment.CurrentDirectory + "/ErrorRebootCount.txt").Close();
                    File.WriteAllText(Environment.CurrentDirectory + "/ErrorRebootCount.txt", "0");
                }
                File.WriteAllText(Environment.CurrentDirectory + "/ErrorRebootCount.txt", (Convert.ToInt32(File.ReadAllText(Environment.CurrentDirectory + "/ErrorRebootCount.txt")) + 1).ToString());
                if (Convert.ToInt32(File.ReadAllText(Environment.CurrentDirectory + "/ErrorRebootCount.txt")) <= 5)
                    Process.Start(Process.GetCurrentProcess().MainModule.FileName);
                System.Environment.Exit(0);
            }

        }
    }
}
