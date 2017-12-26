using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace UserListener
{
    [XmlRoot(ElementName = "User")]
    [Serializable]
    public class User
    {
        [XmlElement(ElementName = "学员名")]
        public string Name { get; set; }

        [XmlElement(ElementName = "密码")]
        public string Password { get; set; }

        [XmlElement(ElementName = "登录信息")]
        public List<LoginInfo> Logs { get; set; }

        [XmlElement(ElementName = "休息次数")]
        public int RestCount { get; set; }

        public TimeSpan LogTime
        {
            get
            {
                TimeSpan value = new TimeSpan();
                foreach (LoginInfo info in Logs)
                {
                    DateTime dtIn = DateTime.Parse(info.LoginTime);
                    DateTime dtOff;
                    try
                    {
                        dtOff = DateTime.Parse(info.LogoffTime);
                    }
                    catch
                    {
                        dtOff = dtIn;
                    }
                    TimeSpan ts = dtOff.Subtract(dtIn).Duration();
                    value = value.Add(ts).Duration();
                }
                return value;
            }
        }

        private string dateDiff(DateTime DateTime1, DateTime DateTime2)
        {
            string dateDiff = null;
            TimeSpan ts = DateTime1.Subtract(DateTime2).Duration();
            dateDiff = ts.Days.ToString() + "天" + ts.Hours.ToString() + "小时" + ts.Minutes.ToString() + "分钟" + ts.Seconds.ToString() + "秒";
            return dateDiff;
        }

        public User()
        {
            Logs = new List<LoginInfo>();
        }
    }

    [Serializable]
    public class LoginInfo
    {
        [XmlAttribute(AttributeName = "登录时间")]
        public string LoginTime { get; set; }

        [XmlAttribute(AttributeName = "注销时间")]
        public string LogoffTime { get; set; }

        public LoginInfo()
        { }
    }
}