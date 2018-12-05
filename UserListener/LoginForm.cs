using System;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace UserListener
{
    public partial class LoginForm : Form
    {
        [DllImport("user32.dll", EntryPoint = "ShowWindow")]
        public static extern int ShowWindow(IntPtr hwnd, int nCmdShow);

        public event EventHandler TimeUp;

        private User user;
        private string fileName;
        private int x, y;
        private DateTime start;
        private bool ff = true;
        private string StopTime;
        private double timeLogOut;

        public LoginForm()
        {
            InitializeComponent();

            this.StartPosition = FormStartPosition.CenterScreen;

            x = Control.MousePosition.X;
            y = Control.MousePosition.Y;

            this.timer2.Interval = 1000;
            timeLogOut = getLogoutTime();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            user = new User();
            user.Name = textBoxUser.Text;
            //if (user.Name.Equals("admin", StringComparison.OrdinalIgnoreCase))
            //{
            //    return;
            //}
            fileName = user.Name + ".bin";
            if (File.Exists(fileName))
            {
                user = BinaryHelper.FileToObject(fileName);
                if (user.Password.Equals(textBoxPwd.Text))
                {
                    LoginInfo info = new LoginInfo();
                    info.LoginTime = DateTime.Now.ToString();
                    info.LogoffTime = info.LoginTime;
                    user.Logs.Add(info);
                }
                else
                {
                    MessageBox.Show("密码输入错误，请重新输入密码");
                    return;
                }
            }
            else
            {
                //if (string.IsNullOrEmpty(textBoxPwd.Text))
                //{
                //    MessageBox.Show("密码不能为空");
                //    return;
                //}
                user.Password = textBoxPwd.Text;
                LoginInfo info = new LoginInfo();
                info.LoginTime = DateTime.Now.ToString();
                info.LogoffTime = info.LoginTime;
                user.Logs.Add(info);
                BinaryHelper.BinaryFileSave(fileName, user);
                MessageBox.Show("已创建新学员：" + user.Name + "；请牢记密码：" + user.Password);
            }
            this.Visible = false;
            MainForm form = new MainForm(user, this);
            form.Show();
            form.FormClose += Form_FormClose;

            this.timer2.Enabled = true;
        }

        private void Form_FormClose(object sender, EventArgs e)
        {
            if (ff && !string.IsNullOrEmpty(StopTime))
            {
                user.Logs[user.Logs.Count - 1].LogoffTime = StopTime;
            }
            else
            {
                user.Logs[user.Logs.Count - 1].LogoffTime = DateTime.Now.ToString();
            }
            BinaryHelper.BinaryFileSave(fileName, user);
            this.textBoxUser.Clear();
            this.textBoxPwd.Clear();
            this.Visible = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            user.Logs[user.Logs.Count - 1].LogoffTime = DateTime.Now.ToString();
            BinaryHelper.BinaryFileSave(fileName, user);
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            int x1 = Control.MousePosition.X;
            int y1 = Control.MousePosition.Y;

            if ((x == x1) && (y == y1) && ff)
            {
                start = DateTime.Now;
                ff = false;
            }
            if (x != x1 || y != y1)
            {
                x = x1;
                y = y1;
                start = DateTime.Now;
                ff = true;
            }

            TimeSpan ts = DateTime.Now.Subtract(start);

            if (ts.Minutes >= timeLogOut)
            {
                timer2.Stop();
                StopTime = DateTime.Now.AddMinutes(-timeLogOut).ToString();
                user.RestCount++;
                BinaryHelper.BinaryFileSave(fileName, user);
                ff = true;
                showWindow();
                string msg = string.Format("您已休息{0}分钟，并记录到档案，请抓紧学习！\n点击确定后重新登录，培训时间重新计时！",
                    timeLogOut);
                DialogResult result = MessageBox.Show(msg, "学习提醒", MessageBoxButtons.OK, MessageBoxIcon.Information);

                if (result == DialogResult.OK)
                {
                    if (TimeUp != null)
                        TimeUp(this, new System.EventArgs());
                }
            }
        }

        private void showWindow()
        {
            string name = Process.GetCurrentProcess().ProcessName;
            Process[] p = Process.GetProcessesByName(name);
            if (p.Length == 1)
            {
                ShowWindow(p[0].MainWindowHandle, 1);
            }
        }

        private double getLogoutTime()
        {
            double t = 10;
            try
            {
                using (StreamReader sr = new StreamReader("配置.ini", System.Text.Encoding.Default))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.StartsWith("登出分钟数"))
                        {
                            string time = line.Substring(line.IndexOf("=") + 1);
                            t = double.Parse(time);
                        }
                    }
                }
            }
            catch { }
            return t;
        }
    }
}