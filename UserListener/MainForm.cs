using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UserListener
{
    public partial class MainForm : Form
    {
        public event EventHandler FormClose;

        public MainForm(User user, LoginForm logForm)
        {
            InitializeComponent();
            if (user.Name.Equals("admin", StringComparison.OrdinalIgnoreCase))
                button1.Visible = true;
            this.labelUser.Text = string.Format("当前登录用户：{0}", user.Name);
            TimeSpan ts = user.LogTime;
            this.labelInfo.Text = string.Format("{0}累计登录时间：{1}天{2}小时{3}分{4}秒",
                user.Name, ts.Days, ts.Hours, ts.Minutes, ts.Seconds);
            this.labelRest.Text = string.Format("休息次数：{0}", user.RestCount);
            foreach (LoginInfo info in user.Logs)
            {
                listBox1.Items.Add(string.Format("登录时间：{0}------注销时间{1}", info.LoginTime, info.LogoffTime));
            }
            logForm.TimeUp += LogForm_TimeUp;
            this.timer1.Enabled = true;
            this.timer1.Start();
        }

        private void LogForm_TimeUp(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            this.Close();
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Text += "--正在存储用户信息...";
            System.Threading.Thread.Sleep(1000);
            e.Cancel = false;
            if (FormClose != null)
                FormClose(this, new System.EventArgs());
            this.Dispose();
            /*
            DialogResult result = MessageBox.Show("退出后，您的学习记录将会暂停！\n确认是否退出(Y/N)", "系统提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            if (result == DialogResult.OK)
            {
                this.Text += "--正在存储用户信息...";
                if (FormClose != null)
                    FormClose(this, new System.EventArgs());
                System.Threading.Thread.Sleep(1000);
                e.Cancel = false;
                this.Dispose();
            }
            else
            {
                e.Cancel = true;
            }*/
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
                this.ShowInTaskbar = false;
                this.notifyIcon.Visible = true;
                this.notifyIcon.ShowBalloonTip(5);
            }
        }

        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Show();
                this.WindowState = FormWindowState.Normal;
                notifyIcon.Visible = false;
                this.ShowInTaskbar = true;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
            this.timer1.Stop();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            List<User> allUsers = new List<User>();
            string fileName = "path.txt";
            if (File.Exists(fileName))
                using (StreamReader sr = new StreamReader(fileName))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        allUsers.AddRange(getUsersByPath(line));
                    }
                }
            if (allUsers.Count > 0)
            {
                Dictionary<string, TimeSpan> dictUserTimes = new Dictionary<string, TimeSpan>();
                foreach (var u in allUsers)
                {
                    if (dictUserTimes.ContainsKey(u.Name))
                    {
                        dictUserTimes[u.Name] += u.LogTime;
                    }
                    else
                    {
                        dictUserTimes.Add(u.Name, u.LogTime);
                    }
                }
                try
                {
                    using (StreamWriter sw = new StreamWriter("学员上机时长统计.csv", false, Encoding.Default))
                    {
                        sw.WriteLine("学员,上机总时长,天数,小时数,分钟数,秒数");
                        List<string> list = new List<string>();
                        foreach (var kvp in dictUserTimes)
                        {
                            sw.WriteLine(string.Format("{0},{1}天{2}小时{3}分{4}秒,{1},{2},{3},{4}",
                                kvp.Key, kvp.Value.Days, kvp.Value.Hours, kvp.Value.Minutes, kvp.Value.Seconds));
                            list.Add(string.Format("学员{0}的上机总时长为:{1}天{2}小时{3}分{4}秒",
                                kvp.Key, kvp.Value.Days, kvp.Value.Hours, kvp.Value.Minutes, kvp.Value.Seconds));
                        }
                        FormShowTotal formShowTotal = new FormShowTotal(list);
                        formShowTotal.Show();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private double addTime(User a, User b)
        {
            double rtn = 0;

            return rtn;
        }

        private List<User> getUsersByPath(string path)
        {
            List<User> users = new List<User>();
            List<string> files = getFileNames(path, "*.bin");
            foreach (var file in files)
            {
                User u = BinaryHelper.FileToObject(file);
                users.Add(u);
            }
            return users;
        }

        private List<string> getFileNames(string strPath, string filter)
        {
            if (!string.IsNullOrEmpty(strPath))
            {
                List<string> fileList = new List<string>();
                try
                {
                    DirectoryInfo di = new DirectoryInfo(strPath);
                    if (di.Exists)
                    {
                        foreach (FileInfo fi in di.GetFiles(filter))
                        {
                            fileList.Add(fi.FullName);
                        }
                    }
                }
                catch (IOException) { }
                catch (ArgumentException) { }
                return fileList;
            }
            return null;
        }
    }
}