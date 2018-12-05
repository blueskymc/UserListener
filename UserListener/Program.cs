using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace UserListener
{
    internal static class Program
    {
        [DllImport("user32.dll")]
        private static extern bool FlashWindow(IntPtr hWnd, bool bInvert);

        [DllImport("user32.dll")]
        private static extern bool FlashWindowEx(int pfwi);

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        private static void Main()
        {
            if (RunningInstance() == null)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new LoginForm());
            }
            else
            {
                MessageBox.Show("只能启动一个程序！");
            }
            //bool runone;
            //System.Threading.Mutex run = new System.Threading.Mutex(true, "single_test", out runone);
            //if (runone)
            //{
            //    run.ReleaseMutex();
            //    Application.EnableVisualStyles();
            //    Application.SetCompatibleTextRenderingDefault(false);
            //    LoginForm frm = new LoginForm();
            //    int hdc = frm.Handle.ToInt32(); // write to ...
            //    Application.Run(frm);
            //    IntPtr a = new IntPtr(hdc);
            //}
            //else
            //{
            //    MessageBox.Show("已经运行了一个实例了。");
            //    //IntPtr hdc = new IntPtr(1312810); // read from...
            //    //bool flash = FlashWindow(hdc, true);
            //}
        }

        //public static System.Diagnostics.Process RunningInstance()
        //{
        //    System.Diagnostics.Process current = System.Diagnostics.Process.GetCurrentProcess();
        //    System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcesses();
        //    foreach (System.Diagnostics.Process process in processes) //查找相同名称的进程
        //    {
        //        if (process.Id != current.Id) //忽略当前进程
        //        { //确认相同进程的程序运行位置是否一样.
        //            if (process.MainModule.FileName
        //                 == current.MainModule.FileName)
        //            { //Return the other process instance.
        //                return process;
        //            }
        //        }
        //    } //No other instance was found, return null.
        //    return null;
        //}

        private static Process RunningInstance()
        {
            Process current = Process.GetCurrentProcess();
            Process[] processes = Process.GetProcessesByName(current.ProcessName);
            //遍历与当前进程名称相同的进程列表
            foreach (Process process in processes)
            {
                //如果实例已经存在则忽略当前进程
                if (process.Id != current.Id)
                {
                    //保证要打开的进程同已经存在的进程来自同一文件路径
                    if (System.Reflection.Assembly.GetExecutingAssembly().Location.Replace("/", "\\") == current.MainModule.FileName)
                    {
                        //返回已经存在的进程
                        return process;
                    }
                }
            }
            return null;
        }
    }
}